using StarMicronics.StarIOExtension;
using System.Windows;
using System.Windows.Controls;

namespace StarPRNTSDK
{
    public static class BlackMarkSampleManager
    {
        /// <summary>
        /// Sample : Creating printing receipt with black mark commands.
        /// </summary>
        public static byte[] CreateLocalizeReceiptWithBlackMarkCommands(ReceiptInformationManager receiptInfo)
        {
            // Your printer emulation.
            Emulation emulation = SharedInformationManager.GetSelectedEmulation();

            // Select black mark type.
            BlackMarkType blackMarkType = SharedInformationManager.GetSelectedBlackMarkType();

            // Creating localize receipt commands sample is in "LocalizeReceipts/'Language'Receipt.cs"
            ReceiptInformationManager.ReceiptType type = receiptInfo.Type;
            LocalizeReceipt localizeReceipt = receiptInfo.LocalizeReceipt;

            byte[] commands;

            switch (receiptInfo.Type)
            {
                default:
                case ReceiptInformationManager.ReceiptType.Text:
                    commands = PrinterFunctions.CreateTextBlackMarkData(emulation, localizeReceipt, blackMarkType, false);
                    break;
            }

            return commands;
        }

        public static void Print(byte[] commands)
        {
            string portName = SharedInformationManager.GetSelectedPortName();
            string portSettings = SharedInformationManager.GetSelectedPortStrrings();

            Communication.SendCommandsWithProgressBar(commands, portName, portSettings, 30000);
        }

        public static void PrintLocalizeReceiptWithBlackMark(ReceiptInformationManager receiptInfo)
        {
            byte[] commands = CreateLocalizeReceiptWithBlackMarkCommands(receiptInfo);

            Print(commands);
        }
    }


    public class PrintLocalizeReceiptWithBlackMarkClickEvent : BaseCommand
    {
        public ReceiptInformationManager ReceiptInformationManager { get; set; }

        public override void Execute(object parameter)
        {
            BlackMarkSampleManager.PrintLocalizeReceiptWithBlackMark(ReceiptInformationManager);
        }
    }

    public partial class BlackMarkSamplePage : Page
    {
        public BlackMarkSamplePage()
        {
            InitializeComponent();

            SharedInformationManager.SetBlackMarkType(BlackMarkType.Valid);
        }

        private void DetectionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SharedInformationManager.SetBlackMarkType(BlackMarkType.ValidWithDetection);
        }

        private void DetectionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharedInformationManager.SetBlackMarkType(BlackMarkType.Valid);
        }
    }
}
