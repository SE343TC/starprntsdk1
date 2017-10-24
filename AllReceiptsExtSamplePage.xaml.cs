using StarMicronics.SMCloudServicesSolution;
using StarMicronics.StarIO;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StarPRNTSDK
{
    public partial class AllReceiptsExtSamplePage : Page
    {
        private IPort port;
        private Thread monitoringPrinterThread;
        private object lockObject;

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
            uint tickCount = (uint)Environment.TickCount;

            while (true)
            {
                // Check printer status is changed for update status to Star Cloud Services.
                isChangeStatus = false;

                lock (lockObject)
                {
                    try
                    {
                        if (port != null)
                        {
                            StarPrinterStatus status = port.GetParsedStatus();

                            // if printer status is changed "isChangeStatus" comes true.
                            CheckPrinterStatus(status); // Check printer status.

                            CheckPaperStatus(status); // Check paper status.

                            CheckCoverStatus(status); // Check cover status.

                            // Your printer cash drawer open status.
                            bool cashDrawerOpenActiveHigh = SharedInformationManager.GetDrawerOpenStatus();
                            CheckCashDrawerStatus(status, cashDrawerOpenActiveHigh);  // Check cash drawer status.
                        }
                    }
                    catch (Exception) // Printer impossible
                    {
                        OnPrinterImpossible();
                    }
                    finally
                    {
                        // if printer status is not changed for some times, update status.
                        if ((UInt32)Environment.TickCount - tickCount >= 300000)
                        {
                            isChangeStatus = true;
                        }

                        // if printer status is changed, upload printer status to Star Cloud Services.
                        if (isChangeStatus)
                        {
                            OnStatusUpdated();
                        }

                        tickCount = (UInt32)Environment.TickCount;
                    }

                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Sample : Uploading printer status to Star Cloud Services.
        /// </summary>
        private void OnStatusUpdated()
        {
            // Create printer status code.
            string statusCode = CreateCurrentPrinterStatusCode();

            // Upload status code to Star Cloud Services
            SMCSAllReceipts.UpdateStatus(statusCode);
        }

        /// <summary>
        /// Sample : Creating printer status code for upload to Star Cloud Services.
        /// </summary>
        private string CreateCurrentPrinterStatusCode()
        {
            int statusCode;

            if (currentPrinterStatus == PrinterStatus.Impossible) // Printer is impossible.
            {
                statusCode = 0x08000000;
            }
            else
            {
                statusCode = 0x00000000;

                if (currentPrinterStatus == PrinterStatus.Offline) // Printer is offline.
                {
                    statusCode |= 0x08000000;
                }

                if (currentPaperStatus == PaperStatus.Empty) // Paper is empty.
                {
                    statusCode |= 0x0000000c;
                }
                else if (currentPaperStatus == PaperStatus.NearEmpty) // Paper is near empty.
                {
                    statusCode |= 0x00000004;
                }

                if (currentCoverStatus == CoverStatus.Open) // Cover is open.
                {
                    statusCode |= 0x20000000;
                }

                if (currentCashDrawerStatus == CashDrawerStatus.Open) // Cash drawer is open.
                {
                    statusCode |= 0x04000000;
                }
            }

            return String.Format("{0:x8}", statusCode);
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
                isChangeStatus = true;

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
                isChangeStatus = true;

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
                isChangeStatus = true;

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
                isChangeStatus = true;

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
                isChangeStatus = true;

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
                isChangeStatus = true;

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
                isChangeStatus = true;

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
                isChangeStatus = true;

                //Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    StatusTextBlock.Text = "Printer Cover Close.";
                //    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
                //})
                //);
            }

            currentCoverStatus = CoverStatus.Close;
        }

        private void OnCashDrawerOpen()
        {
            if (currentCashDrawerStatus != CashDrawerStatus.Open)
            {
                isChangeStatus = true;

                //Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    StatusTextBlock.Text = "Cash Drawer Open.";
                //    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Magenta);
                //})
                //);
            }

            currentCashDrawerStatus = CashDrawerStatus.Open;
        }


        private void OnCashDrawerClose()
        {
            if (currentCashDrawerStatus != CashDrawerStatus.Close)
            {
                isChangeStatus = true;

                //Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    StatusTextBlock.Text = "Cash Drawer Close.";
                //    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Blue);
                //})
                //);
            }

            currentCashDrawerStatus = CashDrawerStatus.Close;
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

        private void CheckCashDrawerStatus(StarPrinterStatus status, bool cashDrawerOpenActiveHigh)
        {
            if (status.CompulsionSwitch == cashDrawerOpenActiveHigh) // Cash drawer open
            {
                OnCashDrawerOpen();

            }
            else
            {
                OnCashDrawerClose();                                 // Cash drawer close
            }
        }

        private void PrintReceipt()
        {
            lock (lockObject)
            {
                Communication.CommunicationResult result = Communication.CommunicationResult.ErrorUnknown;

                ReceiptInformationManager receiptInfo = SharedInformationManager.ReceiptInformationManager;

                ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
                {
                    byte[] commands = AllReceiptsSampleManager.CreateLocalizeReceiptWithAllReceiptsCommands(receiptInfo);

                    if (commands == null) // All print settings (Receipt, Information, QR Code) are OFF.
                    {
                        result = Communication.CommunicationResult.Success;

                        return;
                    }

                    result = Communication.SendCommands(commands, port);
                });

                progressBarWindow.ShowDialog();

                Communication.ShowCommunicationResultMessage(result);
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

        private PrinterStatus currentPrinterStatus;
        private PaperStatus currentPaperStatus;
        private CoverStatus currentCoverStatus;
        private CashDrawerStatus currentCashDrawerStatus;
        private bool isChangeStatus;

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

        public enum CashDrawerStatus
        {
            Invalid,
            Open,
            Close
        }

        public AllReceiptsExtSamplePage()
        {
            InitializeComponent();

            lockObject = new object();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AllReceiptsSampleManager.AddAllReceiptsFunctionEvent();

            ConnectWithProgressBar();

            PrintReceipt();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AllReceiptsSampleManager.RemoveAllReceiptsFunctionEvent();

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
