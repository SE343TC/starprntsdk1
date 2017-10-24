using StarMicronics.SMCloudServicesSolution;
using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System.Windows;
using System.Windows.Controls;

namespace StarPRNTSDK
{
    /// <summary>
    /// Sample : Getting library version.
    /// </summary>
    public static class LibraryVersionSampleManager
    {
        // StarIO
        public static string GetStarIOVersion()
        {
            return Factory.I.GetStarIOVersion();
        }

        // StarIOExtension
        public static string GetStarIOExtVersion()
        {
            return StarIoExt.GetStarIOExtVersion();
        }

        // StarCloudServices
        public static string GetSMCloudServicesVersion()
        {
            return SMCloudServices.GetSMCloudServicesVersion();
        }

        public static void ShowLibraryVersion()
        {
            string starIOVersion = GetStarIOVersion();
            string starIOExtVersion = GetStarIOExtVersion();
            string smCoudServicesVersion = GetSMCloudServicesVersion();

            string message = "StarIO : version " + starIOVersion + "\n" +
                             "StarIOExtension : version " + starIOExtVersion + "\n" +
                             "SMCloudServices : version " + smCoudServicesVersion;

            Util.ShowMessage("Library Version", message);
        }
    }

    /// <summary>
    /// Sample : Getting printer serial number.
    /// </summary>
    public static class SerialNumberSampleManager
    {
        public static void ShowPrinterSerialNumber()
        {
            // Your printer PortName and PortSettings.
            string portName = SharedInformationManager.GetSelectedPortName();
            string portSettings = SharedInformationManager.GetSelectedPortStrrings();

            string serialNumber = "";

            // Getting printer serial number is "Communication.GetSerialNumber(ref string serialNumber, IPort port)".
            Communication.CommunicationResult result = Communication.GetSerialNumberWithProgressBar(ref serialNumber, portName, portSettings, 30000);

            if (result == Communication.CommunicationResult.Success)
            {
                Util.ShowMessage("Serial Number", serialNumber);
            }
            else
            {
                Communication.ShowCommunicationResultMessage(result);
            }
        }
    }


    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RemoveAllJournals();
        }

        private void RemoveAllJournals()
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }
    }

    public class ShowSerialNumberClickEvent : BaseCommand
    {
        public override void Execute(object parameter)
        {
            bool result = Util.ShowConfirmMessage("Confirm", "This menu is for mPOP or TSP100III");

            if (result)
            {
                SerialNumberSampleManager.ShowPrinterSerialNumber();
            }
        }
    }

    public class ShowLibraryVersionClickEvent : BaseCommand
    {
        public override void Execute(object parameter)
        {
            LibraryVersionSampleManager.ShowLibraryVersion();
        }
    }
}
