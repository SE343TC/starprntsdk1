using StarMicronics.StarIO;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StarPRNTSDK
{
    public partial class PrinterExtSamplePage : Page
    {
        public enum PrinterStatus
        {
            Invalid,
            Impossible,
            Online,
            Offline
        }

        public enum PaperStatus
        {
            Invalid,
            Ready,
            NearEmpty,
            Empty
        }

        public enum CoverStatus
        {
            Invalid,
            Open,
            Close
        }

        private IPort port;
        private Thread monitoringPrinterThread;
        private object lockObject;
        private PrinterStatus currentPrinterStatus;
        private PaperStatus currentPaperStatus;
        private CoverStatus currentCoverStatus;

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

            currentPrinterStatus = PrinterStatus.Invalid;
            currentPaperStatus = PaperStatus.Invalid;
            currentCoverStatus = CoverStatus.Invalid;
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

                            CheckPrinterStatus(status); // Check printer status.

                            CheckPaperStatus(status); // Check paper status.

                            CheckCoverStatus(status); // Check cover status.
                        }
                    }
                    catch (Exception) // Printer impossible
                    {
                        OnPrinterImpossible();
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void CheckPrinterStatus(StarPrinterStatus status)
        {
            if (status.Offline) // Printer offline
            {
                OnPrinterOffline();
            }
            else                // Printer online
            {
                OnPrinterOnline();
            }
        }

        private void CheckPaperStatus(StarPrinterStatus status)
        {
            if (status.ReceiptPaperEmpty)                 // Paper empty
            {
                OnPrinterPaperEmpty();
            }
            else if (status.ReceiptPaperNearEmptyInner || // Paper near empty
                     status.ReceiptPaperNearEmptyOuter)
            {
                OnPrinterPaperNearEmpty();
            }
            else                                          // Paper ready
            {
                OnPrinterPaperReady();
            }
        }

        private void CheckCoverStatus(StarPrinterStatus status)
        {
            if (status.CoverOpen) // Cover open
            {
                OnPrinterCoverOpen();
            }
            else                  // Cover close
            {
                OnPrinterCoverClose();
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
            if (currentPrinterStatus != PrinterStatus.Impossible)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    StatusTextBlock.Text = "Printer Impossible.";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                })
                );
            }

            currentPrinterStatus = PrinterStatus.Impossible;
        }

        private void OnPrinterOnline()
        {
            if (currentPrinterStatus != PrinterStatus.Online)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    StatusTextBlock.Text = "Printer Online.";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
                })
                );
            }

            currentPrinterStatus = PrinterStatus.Online;
        }

        private void OnPrinterOffline()
        {
            if (currentPrinterStatus != PrinterStatus.Offline)
            {
                //Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    StatusTextBlock.Text = "Printer Offline.";
                //    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                //})
                //);
            }

            currentPrinterStatus = PrinterStatus.Offline;
        }

        private void OnPrinterPaperNearEmpty()
        {
            if (currentPaperStatus != PaperStatus.NearEmpty)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    StatusTextBlock.Text = "Printer Paper Near Empty.";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Orange);
                })
                );
            }

            currentPaperStatus = PaperStatus.NearEmpty;
        }

        private void OnPrinterPaperEmpty()
        {
            if (currentPaperStatus != PaperStatus.Empty)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    StatusTextBlock.Text = "Printer Paper Empty.";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                })
                );
            }

            currentPaperStatus = PaperStatus.Empty;
        }

        private void OnPrinterPaperReady()
        {
            if (currentPaperStatus != PaperStatus.Ready)
            {
                //Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    StatusTextBlock.Text = "Printer Paper Ready.";
                //    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
                //})
                //);
            }

            currentPaperStatus = PaperStatus.Ready;
        }

        private void OnPrinterCoverOpen()
        {
            if (currentCoverStatus != CoverStatus.Open)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    StatusTextBlock.Text = "Printer Cover Open.";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                })
                );
            }

            currentCoverStatus = CoverStatus.Open;
        }

        private void OnPrinterCoverClose()
        {
            if (currentCoverStatus != CoverStatus.Close)
            {
                //Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    StatusTextBlock.Text = "Printer Cover Close.";
                //    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
                //})
                //);
            }

            currentCoverStatus = CoverStatus.Close;
        }

        private void PrintReceipt()
        {
            lock (lockObject)
            {
                ReceiptInformationManager receiptInfo = SharedInformationManager.ReceiptInformationManager;

                byte[] commands = PrinterSampleManager.CreateLocalizeReceiptCommands(receiptInfo);

                Communication.SendCommandsWithProgressBar(commands, port);
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

        public PrinterExtSamplePage()
        {
            InitializeComponent();

            lockObject = new object();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectWithProgressBar();

            PrintReceipt();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DisconnectWithProgressBar();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintReceipt();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectWithProgressBar();
        }
    }

}
