using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StarPRNTSDK
{
    public partial class CashDrawerExtSamplePage : Page
    {
        public enum CashDrawerStatus
        {
            Invalid,
            Impossible,
            Open,
            Close
        }

        private IPort port;
        private Thread monitoringPrinterThread;
        private object lockObject;
        private CashDrawerStatus currentCashDrawerStatus;

        /// <summary>
        /// Sample : Starting monitoring printer.
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
                }
            }
            catch (PortException) // Port open is failed.
            {
                DidConnectFailed();

                return;
            }

            try
            {
                if (monitoringPrinterThread == null || monitoringPrinterThread.ThreadState == ThreadState.Stopped)
                {
                    monitoringPrinterThread = new Thread(MonitoringPrinter);
                    monitoringPrinterThread.Name = "MonitoringPrinterThread";
                    monitoringPrinterThread.IsBackground = true;
                    monitoringPrinterThread.Start();
                }

            }
            catch (Exception) // Start monitoring printer thread is failure.
            {
                DidConnectFailed();
            }

            currentCashDrawerStatus = CashDrawerStatus.Invalid;
        }

        /// <summary>
        /// Sample : Stoping monitoring printer.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (monitoringPrinterThread != null)
                {
                    if ((monitoringPrinterThread.ThreadState & (ThreadState.Aborted | ThreadState.Stopped)) == 0)
                    {
                        monitoringPrinterThread.Abort();
                    }
                }

                if (monitoringPrinterThread != null)
                {
                    monitoringPrinterThread.Join();
                }

                monitoringPrinterThread = null;

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
        /// Sample : Monitoring printer process.
        /// </summary>
        private void MonitoringPrinter()
        {
            while (true)
            {
                lock (lockObject)
                {
                    try
                    {
                        if (port != null)
                        {
                            StarPrinterStatus status = port.GetParsedStatus();

                            // Your printer cash drawer open status.
                            bool cashDrawerOpenActiveHigh = SharedInformationManager.GetDrawerOpenStatus();

                            if (status.CompulsionSwitch == cashDrawerOpenActiveHigh) // Cash drawer open
                            {
                                OnCashDrawerOpen();

                            }
                            else
                            {
                                OnCashDrawerClose();                                 // Cash drawer close
                            }
                        }
                    }
                    catch (Exception) // Printer impossible
                    {
                        OnPrinterImpossible();
                    }

                    Thread.Sleep(1000);
                }
            }
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

        private void OnPrinterImpossible()
        {
            if (currentCashDrawerStatus != CashDrawerStatus.Impossible)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    StatusTextBlock.Text = "Printer Impossible.";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                })
                );
            }

            currentCashDrawerStatus = CashDrawerStatus.Impossible;
        }

        private void OnCashDrawerOpen()
        {
            if (currentCashDrawerStatus != CashDrawerStatus.Open)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    StatusTextBlock.Text = "Cash Drawer Open.";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Magenta);
                })
                );
            }

            currentCashDrawerStatus = CashDrawerStatus.Open;
        }


        private void OnCashDrawerClose()
        {
            if (currentCashDrawerStatus != CashDrawerStatus.Close)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    StatusTextBlock.Text = "Cash Drawer Close.";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
                })
                );
            }

            currentCashDrawerStatus = CashDrawerStatus.Close;
        }

        private void OpenCashDrawer()
        {
            lock (lockObject)
            {
                PeripheralChannel channel = SharedInformationManager.PeripheralChannel;
                bool checkCondition = SharedInformationManager.CheckCondition;

                Emulation emulation = SharedInformationManager.GetSelectedEmulation();
                byte[] commands = CashDrawerFunctions.CreateData(emulation, channel);

                if (checkCondition)
                {
                    Communication.SendCommandsWithProgressBar(commands, port);
                }
                else
                {
                    Communication.SendCommandsDoNotCheckConditionWithProgressBar(commands, port);
                }
            }
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

        public CashDrawerExtSamplePage()
        {
            InitializeComponent();

            lockObject = new object();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectWithProgressBar();

            OpenCashDrawer();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DisconnectWithProgressBar();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectWithProgressBar();
        }

        private void OpenDrawerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenCashDrawer();
        }
    }
}
