using StarMicronics.SMCloudServicesSolution;
using StarMicronics.StarIOExtension;
using System;
using System.ComponentModel;

namespace StarPRNTSDK
{
    public class AllReceiptsInfoManager : INotifyPropertyChanged
    {
        public AllReceiptsInfoManager()
        {
            if (SharedInformationManager.AllReceiptsInfoManager == null)
            {
                RestorePreviousSettings();
            }
            else
            {
                CopyProperty(SharedInformationManager.AllReceiptsInfoManager);
            }

            SharedInformationManager.AllReceiptsInfoManager = this;

            string filePath = Properties.Settings.Default.AllReceiptsRegistrationFilePath;

            SMCloudServices.SetRegistrationConfigFilePath(filePath);

            try
            {
                isRegistered = SMCloudServices.IsRegistered();
            }
            catch (InvalidOperationException)
            {
                isRegistered = false;
            }
        }

        private void RestorePreviousSettings()
        {
            PrintReceipt = Properties.Settings.Default.AllReceiptsPrintReceipt;
            PrintInformation = Properties.Settings.Default.AllReceiptsPrintInformation;
            PrintQrCode = Properties.Settings.Default.AllReceiptsPrintQrCode;
        }

        private void CopyProperty(AllReceiptsInfoManager source)
        {
            PrintReceipt = source.PrintReceipt;

            PrintInformation = source.PrintInformation;

            PrintQrCode = source.PrintQrCode;
        }

        public string RegistrationStateDescription
        {
            get
            {
                return CreateRegistrationStateDescription();
            }
        }

        public bool IsRegistered
        {
            get
            {
                return isRegistered;
            }
        }

        public bool IsUnRegistered
        {
            get
            {
                return !isRegistered;
            }
        }

        private bool isRegistered;

        public bool PrintReceipt
        {
            get
            {
                return printReceipt;
            }
            set
            {
                printReceipt = value;

                Properties.Settings.Default.AllReceiptsPrintReceipt = value;

                Properties.Settings.Default.Save();
            }
        }

        private bool printReceipt;

        public bool PrintInformation
        {
            get
            {
                return printInformation;
            }
            set
            {
                printInformation = value;

                Properties.Settings.Default.AllReceiptsPrintInformation = value;

                Properties.Settings.Default.Save();
            }
        }

        private bool printInformation;

        public bool PrintQrCode
        {
            get
            {
                return printQrCode;
            }
            set
            {
                printQrCode = value;

                Properties.Settings.Default.AllReceiptsPrintQrCode = value;

                Properties.Settings.Default.Save();
            }
        }

        private bool printQrCode;

        public bool TextReceiptEnabled
        {
            get
            {
                if (IsUnRegistered ||
                    SharedInformationManager.GetSelectedEmulation() == Emulation.StarGraphic)
                {
                    return false;
                }

                return true;
            }
        }

        public bool TextUTF8ReceiptEnabled
        {
            get
            {
                if (IsUnRegistered ||
                    SharedInformationManager.GetSelectedEmulation() == Emulation.StarGraphic ||
                    SharedInformationManager.GetSelectedEmulation() == Emulation.EscPos ||
                    SharedInformationManager.GetSelectedEmulation() == Emulation.EscPosMobile)
                {
                    return false;
                }

                return true;
            }
        }

        public bool RasterReceiptEnabled
        {
            get
            {
                if (IsUnRegistered ||
                    SharedInformationManager.GetSelectedEmulation() == Emulation.StarDotImpact)
                {
                    return false;
                }

                return true;
            }
        }

        public void NotifyIsRegisteredPropertyChanged()
        {
            isRegistered = SMCloudServices.IsRegistered();

            OnPropertyChanged("RegistrationStateDescription");

            OnPropertyChanged("IsRegistered");

            OnPropertyChanged("IsUnRegistered");

            OnPropertyChanged("TextReceiptEnabled");

            OnPropertyChanged("TextUTF8ReceiptEnabled");

            OnPropertyChanged("RasterReceiptEnabled");
        }

        private string CreateRegistrationStateDescription()
        {
            string description = "";

            if (IsRegistered)
            {
                description = "Registration";
            }
            else
            {
                description = "Unregistration State";
            }

            return description;
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
