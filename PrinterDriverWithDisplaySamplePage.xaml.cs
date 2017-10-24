using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System;
using System.ComponentModel;
using System.Printing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace StarPRNTSDK
{
    public partial class PrinterDriverWithDisplaySamplePage : Page, INotifyPropertyChanged
    {
        private StarPrintPortJobMonitor starPrintPortJobMonitor;
        private IPort port;
        private Thread monitoringDisplayThread;
        private object lockObject;

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
        /// Sample : Starting monitoring display.
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
                if (monitoringDisplayThread == null || monitoringDisplayThread.ThreadState == ThreadState.Stopped)
                {
                    monitoringDisplayThread = new Thread(MonitoringDisplay);
                    monitoringDisplayThread.Name = "MonitoringDisplayThread";
                    monitoringDisplayThread.IsBackground = true;
                    monitoringDisplayThread.Start();
                }

                isConnect = true;
            }
            catch (Exception) // Start monitoring display thread is failure.
            {
                DidConnectFailed();
            }
        }

        /// <summary>
        /// Sample : Stoping monitoring display.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (monitoringDisplayThread != null)
                {
                    if ((monitoringDisplayThread.ThreadState & (ThreadState.Aborted | ThreadState.Stopped)) == 0)
                    {
                        monitoringDisplayThread.Abort();
                    }
                }

                if (monitoringDisplayThread != null)
                {
                    monitoringDisplayThread.Join();
                }

                monitoringDisplayThread = null;

                if (port != null)
                {
                    Factory.I.ReleasePort(port);

                    port = null;
                }

                OnStopMonitoringDisplay();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Sample : Monitoring display process.
        /// </summary>
        private void MonitoringDisplay()
        {
            while (true)
            {
                lock (lockObject)
                {
                    try
                    {
                        if (port != null)
                        {
                            // Check display status.
                            Communication.PeripheralStatus status = DisplaySampleManager.GetDiaplayStatus(port);

                            switch (status)
                            {
                                default:
                                case Communication.PeripheralStatus.Impossible:
                                    OnPrinterImpossible();
                                    break;

                                case Communication.PeripheralStatus.Connect:
                                    OnDisplayConnect();
                                    break;

                                case Communication.PeripheralStatus.Disconnect:
                                    OnDisplayDisconnect();
                                    break;

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

        private void DidConnectFailed()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "Check the device. (Power and Bluetooth pairing)\nThen touch up the Refresh button.";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            })
            );
        }

        private void OnDisplayConnect()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "";
            })
            );
        }

        private void OnDisplayDisconnect()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "Display Disconnect.";
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

        private void OnStopMonitoringDisplay()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusTextBlock.Text = "Monitoring Display is stopped.";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Black);
            })
            );
        }

        private Communication.PeripheralStatus CallDisplayFunction(DisplayFunctionManager.FunctionType type, int selectedIndex)
        {
            Communication.PeripheralStatus result = Communication.PeripheralStatus.Invalid;

            lock (lockObject)
            {
                switch (type)
                {
                    case DisplayFunctionManager.FunctionType.Text:
                        result = DisplaySampleManager.DoTextPattern((DisplayFunctionManager.TextPattern)Enum.ToObject(typeof(DisplayFunctionManager.TextPattern), selectedIndex), port);
                        break;

                    case DisplayFunctionManager.FunctionType.Graphic:
                        result = DisplaySampleManager.DoGraphicPattern((DisplayFunctionManager.GraphicPattern)Enum.ToObject(typeof(DisplayFunctionManager.GraphicPattern), selectedIndex), port);
                        break;

                    case DisplayFunctionManager.FunctionType.TurnOnOff:
                        bool turnOn;
                        if (selectedIndex == 0)
                        {
                            turnOn = true;
                        }
                        else
                        {
                            turnOn = false;
                        }

                        result = DisplaySampleManager.DoTurnOnOffPattern(turnOn, port);
                        break;

                    case DisplayFunctionManager.FunctionType.Cursor:
                        result = DisplaySampleManager.DoCursorPattern((DisplayCursorMode)Enum.ToObject(typeof(DisplayCursorMode), selectedIndex), port);
                        break;

                    case DisplayFunctionManager.FunctionType.Contrast:
                        result = DisplaySampleManager.DoContrastPattern((DisplayContrastMode)Enum.ToObject(typeof(DisplayContrastMode), selectedIndex), port);
                        break;

                    case DisplayFunctionManager.FunctionType.CharacterSetInternational:
                        DisplayInternationalType internationalType = (DisplayInternationalType)Enum.ToObject(typeof(DisplayInternationalType), selectedIndex);

                        result = DisplaySampleManager.DoCharacterSetInternationalPattern(internationalType, port);

                        SharedInformationManager.SetDisplayInternationalType(internationalType);
                        break;

                    case DisplayFunctionManager.FunctionType.CharacterSetCodePage:
                        DisplayCodePageType codePageType = (DisplayCodePageType)Enum.ToObject(typeof(DisplayCodePageType), selectedIndex);

                        result = DisplaySampleManager.DoCharacterSetCodePagePattern(codePageType, port);

                        SharedInformationManager.SetDisplayCodePageType(codePageType);
                        break;

                    case DisplayFunctionManager.FunctionType.UserDefinedCharacter:
                        bool set;
                        if ((DisplayFunctionManager.UserDefinedCharacterPattern)Enum.ToObject(typeof(DisplayFunctionManager.UserDefinedCharacterPattern), selectedIndex) == DisplayFunctionManager.UserDefinedCharacterPattern.Set)
                        {
                            set = true;
                        }
                        else
                        {
                            set = false;
                        }

                        result = DisplaySampleManager.DoUserDefinedCharacterPattern(set, port);
                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        private void CallDisplayFunctionWithProgressBar(DisplayFunctionManager.FunctionType type, int selectedIndex)
        {
            Communication.PeripheralStatus result = Communication.PeripheralStatus.Invalid;

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {
                result = CallDisplayFunction(type, selectedIndex);
            });

            progressBarWindow.ShowDialog();

            if (result != Communication.PeripheralStatus.Connect)
            {
                Communication.ShowPeripheralStatusResultMessage("Display", result);
            }
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
            confirmWindow.SettingTitle = "Printer driver job is completed. Start monitoring display?";
            confirmWindow.AcceptButtonContent = "Yes";
            confirmWindow.CancelButtonContent = "No";

            isShowConfirmWindow = true;
            bool result = (bool)confirmWindow.ShowDialog();
            isShowConfirmWindow = false;

            return result;
        }

        public static readonly DependencyProperty IsMonitoringProperty = DependencyProperty.Register("IsMonitoring", typeof(bool), typeof(PrinterDriverWithDisplaySamplePage));

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

        private void SelectPatternComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox selectionChangedComboBox = (ComboBox)sender;

            DisplayFunctionManager.FunctionType type;

            if (selectionChangedComboBox == SelectPatternComboBox)
            {
                type = SharedInformationManager.GetSelectedDisplayFunction();
            }
            else
            {
                type = SharedInformationManager.GetSelectedAdditionDisplayFunction();
            }

            int selectedIndex = selectionChangedComboBox.SelectedIndex;

            if (isConnect && !notCallFunction && selectedIndex >= 0)
            {
                CallDisplayFunctionWithProgressBar(type, selectedIndex);
            }
        }

        private void DisplayFunction_Updated(object sender, DataTransferEventArgs e)
        {
            DisplayFunctionManager.FunctionType type = SharedInformationManager.GetSelectedDisplayFunction();

            int selectedIndex = SharedInformationManager.GetDisplayFunctionDefaultPatternIndex();

            if (isConnect)
            {
                CallDisplayFunctionWithProgressBar(type, selectedIndex);
            }

            notCallFunction = true;

            SelectPatternComboBox.SelectedIndex = selectedIndex;

            SelectAdditionPatternComboBox.SelectedIndex = SharedInformationManager.GetDisplayFunctionDefaultAdditionPatternIndex();

            notCallFunction = false;
        }

        private bool isConnect;

        private bool notCallFunction;

        public PrinterDriverWithDisplaySamplePage()
        {
            InitializeComponent();

            SetStarPrintPortJobMonitor();

            StatusMonitorCheckBox.DataContext = this;

            lockObject = new object();

            isShowConfirmWindow = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IsMonitoring = true;

            SharedInformationManager.DisplayFunctionManager.Type = DisplayFunctionManager.FunctionType.Text;
            SharedInformationManager.DisplayFunctionManager.AdditionType = DisplayFunctionManager.FunctionType.Invalid;
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

        private void StatusMonitorCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox statusMonitorCheckBox = (CheckBox)sender;

            IsMonitoring = (bool)statusMonitorCheckBox.IsChecked;
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
