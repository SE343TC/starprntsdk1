using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;

namespace StarPRNTSDK
{
    public class SelectedModelManager : INotifyPropertyChanged
    {
        public SelectedModelManager()
        {
            if (SharedInformationManager.SelectedModelManager != null)
            {
                CopyProperty(SharedInformationManager.SelectedModelManager);
            }
            else
            {
                blackMarkType = BlackMarkType.Invalid;
            }

            SharedInformationManager.SelectedModelManager = this;
        }

        private void CopyProperty(SelectedModelManager source)
        {
            SelectedModel = source.SelectedModel;

            SelectedPort = source.SelectedPort;
        }

        internal ModelInformation SelectedModel
        {
            get
            {
                return selectedModel;
            }
            set
            {
                selectedModel = value;

                if (SharedInformationManager.SelectedModelManager.SelectedModel != value)
                {
                    SharedInformationManager.SetSelectedModelInformation(value);

                    CallPropertyChangedEvent();
                }
            }
        }

        private ModelInformation selectedModel;

        public PortInfo SelectedPort
        {
            get
            {
                return selectedPort;
            }
            set
            {
                selectedPort = value;

                if (SharedInformationManager.SelectedModelManager.SelectedPort != value)
                {
                    SharedInformationManager.SetSelectedPortInfo(value);

                    CallPropertyChangedEvent();
                }
            }
        }

        private PortInfo selectedPort;

        public bool IsSelect
        {
            get
            {
                return IsSelectModel();
            }
        }

        public bool IsUnSelect
        {
            get
            {
                return !IsSelectModel();
            }
        }

        public string SelectedModelDescription
        {
            get
            {
                return CreateSelectedModelDescription();
            }
        }

        public string[] SelectedPortNameAndPortSettings
        {
            get
            {
                return GetSelectedPortNameAndPortSettings();
            }
        }

        public bool BlackMarkDetectionIsEnabled
        {
            get
            {
                return GetBlackMarkDetectionIsEnabled();
            }
        }

        public BlackMarkType BlackMarkType
        {
            get
            {
                return GetBlackMarkType();
            }
            set
            {
                blackMarkType = value;
            }
        }

        private BlackMarkType blackMarkType;

        public bool PrinterSampleEnabled
        {
            get
            {
                if (IsUnSelect)
                {
                    return false;
                }

                return true;
            }
        }

        public bool BlackMarkSampleEnabled
        {
            get
            {
                if (IsUnSelect ||
                    SelectedModel.DefaultEmulation == Emulation.StarGraphic)
                {
                    return false;
                }

                return true;
            }
        }

        public bool PageModeSampleEnabled
        {
            get
            {
                if (IsUnSelect ||
                    SelectedModel.DefaultEmulation == Emulation.StarGraphic ||
                    SelectedModel.DefaultEmulation == Emulation.StarDotImpact)
                {
                    return false;
                }

                return true;
            }
        }

        public bool CashDrawerSampleEnabled
        {
            get
            {
                if (IsUnSelect ||
                    SelectedModel.SimpleModelName.StartsWith("SM-"))
                {
                    return false;
                }

                return true;
            }
        }

        public bool BarcodeReaderSampleEnabled
        {
            get
            {
                if (IsUnSelect ||
                    SelectedModel.Model != ModelInformation.PrinterModel.POP10)
                {
                    return false;
                }

                return true;
            }
        }

        public bool DisplaySampleEnabled
        {
            get
            {
                if (IsUnSelect ||
                    (SelectedModel.Model != ModelInformation.PrinterModel.POP10 &&
                     !IsTSP100IIIU())) // POP10 or TSP100IIIU
                {
                    return false;
                }

                return true;
            }
        }

        public bool ScaleSampleEnabled
        {
            get
            {
                if (IsUnSelect ||
                    SelectedModel.Model != ModelInformation.PrinterModel.POP10)
                {
                    return false;
                }

                return true;
            }
        }

        public bool CombinationSampleEnabled
        {
            get
            {
                if (IsUnSelect ||
                    SelectedModel.Model != ModelInformation.PrinterModel.POP10)
                {
                    return false;
                }

                return true;
            }
        }

        public bool CombinationPrinterDriverSampleEnabled
        {
            get
            {
                if (IsUnSelect ||
                    !IsExistPrinterDriver())
                {
                    return false;
                }

                return true;
            }
        }

        public bool APISampleEnabled
        {
            get
            {
                if (IsUnSelect)
                {
                    return false;
                }

                return true;
            }
        }

        public bool AllReceiptsSampleEnabled
        {
            get
            {
                if (IsUnSelect ||
                    SelectedModel.DefaultEmulation == Emulation.StarDotImpact)
                {
                    return false;
                }

                return true;
            }
        }

        public bool BluetoothSettingEnabled
        {
            get
            {
                if (IsUnSelect ||
                    !SelectedPort.PortName.StartsWith("BT:"))
                {
                    return false;
                }

                return true;
            }
        }

        public bool DeviceStatusSampleEnabled
        {
            get
            {
                if (IsUnSelect)
                {
                    return false;
                }

                return true;
            }
        }

        public bool SerialNumberSampleEnabled
        {
            get
            {
                if (IsUnSelect ||
                    (SelectedModel.Model != ModelInformation.PrinterModel.POP10 &&
                     !IsTSP100())) // POP10 or TSP100
                {
                    return false;
                }

                return true;
            }
        }

        public bool TextReceiptEnabled
        {
            get
            {
                if (SelectedModel.DefaultEmulation == Emulation.StarGraphic)
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
                if (SelectedModel.DefaultEmulation == Emulation.StarGraphic ||
                    SelectedModel.DefaultEmulation == Emulation.EscPos ||
                    SelectedModel.DefaultEmulation == Emulation.EscPosMobile)
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
                if (SelectedModel.DefaultEmulation == Emulation.StarDotImpact)
                {
                    return false;
                }

                return true;
            }
        }

        public bool RasterCouponEnabled
        {
            get
            {
                return true;
            }
        }

        private string CreateSelectedModelDescription()
        {
            if (!IsSelectModel())
            {
                return "Unselected State";
            }
            else
            {
                string modelName = CreateSelectedModelName();
                PortInfoManager manager = new PortInfoManager(SelectedPort);

                if (PortInfoManager.IsSerialPort(SelectedPort))
                {
                    return modelName + "\n" + SelectedPort.PortName + " ( " + SelectedModel.PortSettings + " )";
                }
                else
                {
                    return modelName + "\n" + manager.Description;
                }
            }
        }

        private string CreateSelectedModelName()
        {
            string modelName = "";

            if (SelectedPort.ModelName.Equals(""))
            {
                modelName = SelectedModel.SimpleModelName;

                int starPRNTIndex = SelectedModel.SimpleModelName.IndexOf(" StarPRNT");

                if (starPRNTIndex > 0)
                {
                    modelName = modelName.Substring(0, starPRNTIndex);
                }
            }
            else
            {
                modelName = SelectedPort.ModelName;
            }

            return modelName;
        }

        private bool GetBlackMarkDetectionIsEnabled()
        {
            bool isEnabled;

            switch (SelectedModel.DefaultEmulation)
            {
                case Emulation.StarLine:
                    isEnabled = true;
                    break;

                case Emulation.StarDotImpact:
                    isEnabled = true;
                    break;

                case Emulation.EscPos:
                    isEnabled = true;
                    break;

                default:
                    isEnabled = false;
                    break;
            }

            return isEnabled;
        }

        private BlackMarkType GetBlackMarkType()
        {
            BlackMarkType type = BlackMarkType.Invalid;

            if (BlackMarkDetectionIsEnabled)
            {
                type = blackMarkType;
            }

            return type;
        }

        private string[] GetSelectedPortNameAndPortSettings()
        {
            if (IsUnSelect)
            {
                return (string[])Enumerable.Empty<string>();
            }

            List<string> settingsList = new List<string>();

            settingsList.Add(SelectedPort.PortName);

            settingsList.Add(SelectedModel.PortSettings);

            return settingsList.ToArray();
        }

        private bool IsSelectModel()
        {
            return !(SelectedModel == null);
        }

        private bool IsExistPrinterDriver()
        {
            StarPrintPortJobMonitor jobMonitor = new StarPrintPortJobMonitor(SelectedPort.PortName);

            return (jobMonitor.PrintQueues.Length > 0);
        }

        private bool IsTSP100()
        {
            return (SelectedModel.Model == ModelInformation.PrinterModel.TSP113 ||
                    SelectedModel.Model == ModelInformation.PrinterModel.TSP143 ||
                    SelectedModel.Model == ModelInformation.PrinterModel.TSP143GT ||
                    SelectedModel.Model == ModelInformation.PrinterModel.TSP143GT);
        }

        private bool IsTSP100IIIU()
        {
            return SelectedPort.PortName.ToUpper().StartsWith("USBPRN") &&
                  (SelectedModel.Model == ModelInformation.PrinterModel.TSP113 || SelectedModel.Model == ModelInformation.PrinterModel.TSP143);
        }

        private void CallPropertyChangedEvent()
        {
            OnPropertyChanged("SelectedModel");
            OnPropertyChanged("SelectedPort");
            OnPropertyChanged("IsSelect");
            OnPropertyChanged("IsUnSelect");
            OnPropertyChanged("SelectedModelDescription");
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
