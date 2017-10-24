using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StarPRNTSDK
{
    public partial class ScaleExtSamplePage : Page
    {
        private IPort port;
        private Thread monitoringScaleThread;
        private object lockObject;
        private Communication.PeripheralStatus scaleStatus;

        /// <summary>
        /// Sample : Starting monitoring scale.
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
                if (monitoringScaleThread == null || monitoringScaleThread.ThreadState == ThreadState.Stopped)
                {
                    monitoringScaleThread = new Thread(MonitoringScale);
                    monitoringScaleThread.Name = "MonitoringScaleThread";
                    monitoringScaleThread.IsBackground = true;
                    monitoringScaleThread.Start();
                }
            }
            catch (Exception) // Start monitoring display thread is failure.
            {
                DidConnectFailed();
            }
        }

        /// <summary>
        /// Sample : Stoping monitoring scale.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (monitoringScaleThread != null)
                {
                    if ((monitoringScaleThread.ThreadState & (ThreadState.Aborted | ThreadState.Stopped)) == 0)
                    {
                        monitoringScaleThread.Abort();
                    }
                }

                if (monitoringScaleThread != null)
                {
                    monitoringScaleThread.Join();
                }

                monitoringScaleThread = null;

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
        /// Sample : Monitoring scale process.
        /// </summary>
        private void MonitoringScale()
        {
            while (true)
            {
                lock (lockObject)
                {
                    try
                    {
                        if (port != null)
                        {
                            CheckScaleStatus(); // Check scale status.

                            if (scaleStatus == Communication.PeripheralStatus.Connect) // Scale is connected.
                            {
                                CheckDisplayedWeight(); // Check displayed weight.
                            }
                        }
                    }
                    catch (PortException)
                    {
                        OnPrinterImpossible();
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void CheckScaleStatus()
        {
            scaleStatus = ScaleSampleManager.GetScaleStatus(port);

            switch (scaleStatus)
            {
                default:
                case Communication.PeripheralStatus.Impossible:
                    OnPrinterImpossible();
                    break;

                case Communication.PeripheralStatus.Connect:
                    OnScaleConnect();
                    break;

                case Communication.PeripheralStatus.Disconnect:
                    OnScaleDisconnect();
                    break;

            }
        }

        private void CheckDisplayedWeight()
        {
            DisplayedWeightStatus weightStatus = DisplayedWeightStatus.Invalid;
            string weight = "";

            Communication.PeripheralStatus status = ScaleSampleManager.GetScaleDisplayedWeight(port, ref weightStatus, ref weight);

            if (status == Communication.PeripheralStatus.Impossible)
            {
                OnPrinterImpossible();

                return;
            }

            switch (weightStatus)
            {
                default:
                case DisplayedWeightStatus.Zero:
                    OnZeroWeight(weight);
                    break;

                case DisplayedWeightStatus.NotInMotion:
                    OnNotInMotionWeight(weight);
                    break;

                case DisplayedWeightStatus.Motion:
                    OnMotionWeight(weight);
                    break;
            }
        }

        private void OnZeroWeight(string displayedWeight)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = displayedWeight;
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Green);
            })
            );
        }

        private void OnNotInMotionWeight(string displayedWeigh)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = displayedWeigh;
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
            })
            );
        }

        private void OnMotionWeight(string displayedWeigh)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = displayedWeigh;
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            })
            );
        }

        private void OnScaleConnect()
        {
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    StatusTextBlock.Text = "Scale Connect.";
            //    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
            //})
            //);
        }

        private void OnScaleDisconnect()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "Scale Disconnect.";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
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

        private void OnPrinterImpossible()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "Printer Impossible.";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            })
            );
        }

        private Communication.PeripheralStatus SendZeroClearCommands()
        {
            Communication.PeripheralStatus result = Communication.PeripheralStatus.Invalid;

            lock (lockObject)
            {
                result = ScaleSampleManager.SendZeroClearCommands(port);
            }

            return result;
        }

        private Communication.PeripheralStatus SendUnitChangeCommands()
        {
            Communication.PeripheralStatus result = Communication.PeripheralStatus.Invalid;

            lock (lockObject)
            {
                result = ScaleSampleManager.SendUnitChangeCommands(port);
            }

            return result;
        }

        private void SendZeroClearCommandsWithProgressBar()
        {
            Communication.PeripheralStatus result = Communication.PeripheralStatus.Invalid;

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {
                result = SendZeroClearCommands();
            });

            progressBarWindow.ShowDialog();

            if (result != Communication.PeripheralStatus.Connect)
            {
                Communication.ShowPeripheralStatusResultMessage("Scale", result);
            }
        }

        private void SendUnitChangeCommandsWithProgressBar()
        {
            Communication.PeripheralStatus result = Communication.PeripheralStatus.Invalid;

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {
                result = SendUnitChangeCommands();
            });

            progressBarWindow.ShowDialog();

            if (result != Communication.PeripheralStatus.Connect)
            {
                Communication.ShowPeripheralStatusResultMessage("Scale", result);
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
        }

        private void ZeroClearButton_Click(object sender, RoutedEventArgs e)
        {
            SendZeroClearCommandsWithProgressBar();
        }

        private void UnitChangeButton_Click(object sender, RoutedEventArgs e)
        {
            SendUnitChangeCommandsWithProgressBar();
        }

        public ScaleExtSamplePage()
        {
            InitializeComponent();

            lockObject = new object();
        }
    }
}
