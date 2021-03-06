﻿using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StarPRNTSDK
{
    public partial class BarcodeReaderExtSamplePage : Page
    {
        private IPort port;
        private Thread monitoringBarcodeReaderThread;
        private object lockObject;
        private Communication.PeripheralStatus barcodeReaderStatus;

        /// <summary>
        /// Sample : Starting monitoring barcode reader.
        /// </summary>
        public void Connect()
        {
            try
            {
                if (port == null)
                {
                    // Your printer PortName and PortSettings.
                    string portName = SharedInformationManager.GetSelectedPortName();
                    string portSettings = SharedInformationManager.GetSelectedPortStrrings();

                    port = Factory.I.GetPort(portName, portSettings, 10000);

                    // First, clear barcode reader buffer.
                    ClearBarcodeReaderBuffer();
                }
            }
            catch (PortException) // Port open is failed.
            {
                DidConnectFailed();

                return;
            }

            try
            {
                if (monitoringBarcodeReaderThread == null || monitoringBarcodeReaderThread.ThreadState == ThreadState.Stopped)
                {
                    monitoringBarcodeReaderThread = new Thread(MonitoringBarcodeReader);
                    monitoringBarcodeReaderThread.Name = "MonitoringBarcodeReaderThread";
                    monitoringBarcodeReaderThread.IsBackground = true;
                    monitoringBarcodeReaderThread.Start();
                }

            }
            catch (Exception) // Start monitoring barcode reader thread is failure.
            {
                DidConnectFailed();
            }

            barcodeReaderStatus = Communication.PeripheralStatus.Invalid;
        }

        /// <summary>
        /// Sample : Stoping monitoring barcode reader.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (monitoringBarcodeReaderThread != null)
                {
                    if ((monitoringBarcodeReaderThread.ThreadState & (ThreadState.Aborted | ThreadState.Stopped)) == 0)
                    {
                        monitoringBarcodeReaderThread.Abort();
                    }
                }

                if (monitoringBarcodeReaderThread != null)
                {
                    monitoringBarcodeReaderThread.Join();
                }

                monitoringBarcodeReaderThread = null;

                if (port != null)
                {
                    Factory.I.ReleasePort(port);

                    port = null;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Sample : Monitoring barcode reader process.
        /// </summary>
        private void MonitoringBarcodeReader()
        {
            // Your printer emulation.            
            Emulation emulation = SharedInformationManager.GetSelectedEmulation();

            while (true)
            {
                lock (lockObject)
                {
                    try
                    {
                        if (port != null)
                        {
                            CheckBarcodeReaderStatus(); // Check barcode reader status. (connect or disconnect)

                            if (barcodeReaderStatus == Communication.PeripheralStatus.Connect) // Barcode reader is connected.
                            {
                                ReadBarcodeReaderData();  // Read barcode reader data.
                            }
                        }
                    }
                    catch (PortException)
                    {
                        OnBarcodeReaderImpossible();
                    }
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Sample : Clearing barcode reader buffer.
        /// </summary>
        private void ClearBarcodeReaderBuffer()
        {
            byte[] clearBufferCommands = BcrFunctions.CreateClearBuffer();

            port.WritePort(clearBufferCommands, 0, (uint)clearBufferCommands.Length);
        }

        /// <summary>
        /// Sample : Checking barcode reader status.
        /// </summary>
        private void CheckBarcodeReaderStatus()
        {
            // Create IPeripheralConnectParser object.
            IPeripheralConnectParser parser = StarIoExt.CreateBcrConnectParser(BcrModel.POP1);

            // Usage of parser sample is "Communication.ParseDoNotCheckCondition(IPeripheralCommandParser parser, IPort port)".
            Communication.CommunicationResult result = Communication.ParseDoNotCheckCondition(parser, port);

            // Check peripheral status.
            barcodeReaderStatus = Communication.PeripheralStatus.Invalid;
            if (result == Communication.CommunicationResult.Success)
            {
                // Check parser property value.
                if (parser.IsConnected) // connect
                {
                    barcodeReaderStatus = Communication.PeripheralStatus.Connect;
                }
                else // disconnect
                {
                    barcodeReaderStatus = Communication.PeripheralStatus.Disconnect;
                }
            }
            else // communication error
            {
                barcodeReaderStatus = Communication.PeripheralStatus.Impossible;
            }

            switch (barcodeReaderStatus)
            {
                default:
                case Communication.PeripheralStatus.Impossible:
                    OnBarcodeReaderImpossible();
                    break;

                case Communication.PeripheralStatus.Connect:
                    OnBarcodeReaderConnect();
                    break;

                case Communication.PeripheralStatus.Disconnect:
                    OnBarcodeReaderDisconnect();
                    break;
            }
        }

        /// <summary>
        /// Sample : Reading barcode reader data.
        /// </summary>
        private void ReadBarcodeReaderData()
        {
            // Barcode reader data parser sample is in "BcrDataParser" class.
            BcrDataParser parser = new BcrDataParser();

            // Usage of parser sample is "Communication.ParseDoNotCheckCondition(IPeripheralCommandParser parser, IPort port)".
            Communication.CommunicationResult result = Communication.ParseDoNotCheckCondition(parser, port);

            if (result != Communication.CommunicationResult.Success)
            {
                OnBarcodeReaderImpossible();

                return;
            }

            // Check parser property value.
            byte[] barcodeData = parser.Data;

            if (barcodeData != null) // Barcode reader data is not empty.
            {
                OnBarcodeDataReceived(barcodeData);
            }
        }

        private void OnBarcodeDataReceived(byte[] receivedData)
        {
            string text = Encoding.UTF8.GetString(receivedData, 0, receivedData.Length);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                AddTextToList(text);
            })
            );
        }

        private void DidConnectFailed()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "Check the device. (Power and Bluetooth pairing)\nThen touch up the Refresh button.";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            })
            );
        }

        private void OnBarcodeReaderImpossible()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "Barcode Reader Impossible.";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            })
            );
        }

        private void OnBarcodeReaderConnect()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "Barcode Reader Connect.";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
            })
            );
        }

        private void OnBarcodeReaderDisconnect()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "Barcode Reader Disconnect.";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            })
            );
        }

        private void AddTextToList(string text)
        {
            string[] splittedTextArray = text.Split('\n');

            foreach (var splittedText in splittedTextArray)
            {
                if (!splittedText.Equals(""))
                {
                    string barcode = splittedText;

                    int index = splittedText.IndexOf("\r");

                    if (index != -1)
                    {
                        barcode = barcode.Substring(0, index);
                    }

                    ReadDataListBox.Items.Add(barcode);

                    PageScrollViewer.ScrollToBottom();
                }
            }
        }

        private void ClearTextFromList()
        {
            ReadDataListBox.Items.Clear();

            PageScrollViewer.ScrollToBottom();
        }

        private void ConnectWithProgressBar()
        {
            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {
                Connect();
            });

            progressBarWindow.ShowDialog();
        }

        private void DisconnectWithProgressBar()
        {
            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Disconneting Printer...", () =>
            {
                Disconnect();
            });

            progressBarWindow.ShowDialog();
        }

        public BarcodeReaderExtSamplePage()
        {
            InitializeComponent();

            lockObject = new object();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectWithProgressBar();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DisconnectWithProgressBar();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectWithProgressBar();

            ClearTextFromList();
        }
    }
}
