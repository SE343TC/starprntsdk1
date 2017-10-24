using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System;
using System.ComponentModel;
using System.Printing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StarPRNTSDK
{
    public partial class PrinterDriverWithScaleSamplePage : Page, INotifyPropertyChanged
    {
        private StarPrintPortJobMonitor starPrintPortJobMonitor;
        private IPort port;
        private Thread monitoringScaleThread;
        private object lockObject;
        private Communication.PeripheralStatus scaleStatus;

        /// <summary>
        /// Sample : Setting StarPrintJobMonitor object.
        /// </summary>
        private void SetStarPrintPortJobMonitor()
        {
            // Your printer PortName.
            string portName = SharedInformationManager.GetSelectedPortName();

            // Create StarPrintJobMonitor object.
            starPrintPortJobMonitor = new StarPrintPortJobMonitor(portName);
            starPrintPortJobMonitor.PrintQueueJobIsAdded += OnPrintQueueJobIsAdded;         // Add called event when printer queue job is added.
            starPrintPortJobMonitor.PrintQueueJobIsRemoved += OnPrintQueueJobIsRemoved;         // Add called event when printer queue job is added.
            starPrintPortJobMonitor.PrintQueueAllJobsAreCompleted += OnPrintQueueAllJobsAreCompleted; // Add called event when printer queue all jobs are completed.
            PrinterQueueJobCount = starPrintPortJobMonitor.JobCount; // Can get current printer queue job count.

            // start printer queue job monitoring.
            starPrintPortJobMonitor.Start();

            //starPrintPortJobMonitor.Stop(); // if you would like stop monitoring job call this method.
        }

        /// <summary>
        /// Sample : Event called when printer queue job is added.
        /// When printer queue jobs exist, stop monitoring printer to complete printer queue job.
        /// </summary>
        private void OnPrintQueueJobIsAdded(object sender, object e)
        {
            PrinterQueueJobCount = starPrintPortJobMonitor.JobCount;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                // If port is used, release port.
                if (port != null)
                {
                    bool result = ShowReleasePortConfirmWindow();

                    if (result)
                    {
                        IsMonitoring = false; // Call StopMonitoringPrinterThread().
                    }
                }
            })
            );
        }

        /// <summary>
        /// Sample : Event called when printer queue  all jobs are completed.
        /// When printer queue all jobs are completed, you can monitoring printer again.
        /// </summary>
        private void OnPrintQueueAllJobsAreCompleted(object sender, object e)
        {
            PrinterQueueJobCount = starPrintPortJobMonitor.JobCount;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (port == null)
                {
                    bool result = ShowReconnectConfirmWindow();

                    if (result)
                    {
                        IsMonitoring = true; // Call StopMonitoringPrinterThread().
                    }
                }
            })
            );
        }

        /// <summary>
        /// Sample : Event called when printer queue job is removed.
        /// </summary>
        private void OnPrintQueueJobIsRemoved(object sender, object e)
        {
            PrinterQueueJobCount = starPrintPortJobMonitor.JobCount;
        }

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

                OnStopMonitoringScale();
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

        private void OnStopMonitoringScale()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "Monitoring Scale is stopped.";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Black);
            })
            );
        }

        private void PrintReceiptViaPrinterDriverWithProgressBar()
        {
            string portName = SharedInformationManager.GetSelectedPortName();
            PrintQueue[] printQueues = starPrintPortJobMonitor.PrintQueues;
            LocalizeReceipt localizeReceipt = SharedInformationManager.GetSelectedLocalizeReceipt();

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {
                Thread thread = new Thread(
                () =>
                {
                    foreach (PrintQueue printQueue in printQueues)
                    {
                        PrinterDriverManager.PrintViaPrinterDriver(printQueue, localizeReceipt.CreateRasterImageText(), localizeReceipt.RasterReceiptFont);
                    }
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            });

            progressBarWindow.ShowDialog();

            Util.FocusMainWindow();
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

        private bool isShowConfirmWindow;

        private bool ShowReleasePortConfirmWindow()
        {
            if (isShowConfirmWindow)
            {
                return false;
            }

            SelectSettingWindow confirmWindow = new SelectSettingWindow();
            confirmWindow.Title = "Confirm";
            confirmWindow.SettingTitle = "Port is used, so can not print via printer driver. Release port?";
            confirmWindow.AcceptButtonContent = "Yes";
            confirmWindow.CancelButtonContent = "No";

            isShowConfirmWindow = true;
            bool result = (bool)confirmWindow.ShowDialog();
            isShowConfirmWindow = false;

            return result;
        }

        private bool ShowReconnectConfirmWindow()
        {
            if (isShowConfirmWindow)
            {
                return false;
            }

            SelectSettingWindow confirmWindow = new SelectSettingWindow();
            confirmWindow.Title = "Confirm";
            confirmWindow.SettingTitle = "Printer driver job is completed. Start monitoring scale?";
            confirmWindow.AcceptButtonContent = "Yes";
            confirmWindow.CancelButtonContent = "No";

            isShowConfirmWindow = true;
            bool result = (bool)confirmWindow.ShowDialog();
            isShowConfirmWindow = false;

            return result;
        }

        public static readonly DependencyProperty IsMonitoringProperty = DependencyProperty.Register("IsMonitoring", typeof(bool), typeof(PrinterDriverWithScaleSamplePage));

        public bool IsMonitoring
        {
            get
            {
                return (bool)GetValue(IsMonitoringProperty);
            }
            set
            {
                SetValue(IsMonitoringProperty, value);

                if (value)
                {
                    ConnectWithProgressBar();
                }
                else
                {
                    DisconnectWithProgressBar();
                }

                OnPropertyChanged("IsMonitoring");
            }
        }

        public int PrinterQueueJobCount
        {
            get
            {
                return printerQueueJobCount;
            }
            set
            {
                printerQueueJobCount = value;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    JobCountTextBlock.Text = "Printer Queue Job Count : " + printerQueueJobCount.ToString();

                    if (printerQueueJobCount == 0)
                    {
                        JobCountTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        JobCountTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    }
                })
                );
            }
        }
        private int printerQueueJobCount;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IsMonitoring = true;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            IsMonitoring = false;

            starPrintPortJobMonitor.Stop();
        }
        private void PrintViaPrinterDriverButton_Click(object sender, RoutedEventArgs e)
        {
            PrintReceiptViaPrinterDriverWithProgressBar();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            IsMonitoring = true;
        }

        private void ZeroClearButton_Click(object sender, RoutedEventArgs e)
        {
            SendZeroClearCommandsWithProgressBar();
        }

        private void UnitChangeButton_Click(object sender, RoutedEventArgs e)
        {
            SendUnitChangeCommandsWithProgressBar();
        }

        private void StatusMonitorCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox statusMonitorCheckBox = (CheckBox)sender;

            IsMonitoring = (bool)statusMonitorCheckBox.IsChecked;
        }

        public PrinterDriverWithScaleSamplePage()
        {
            InitializeComponent();

            SetStarPrintPortJobMonitor();

            StatusMonitorCheckBox.DataContext = this;

            lockObject = new object();

            isShowConfirmWindow = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
