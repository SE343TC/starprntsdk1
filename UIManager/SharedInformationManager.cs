using StarMicronics.SMCloudServicesSolution;
using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System;

namespace StarPRNTSDK
{
    internal static class SharedInformationManager
    {
        public static SelectedModelManager SelectedModelManager { get; set; }

        public static ReceiptInformationManager ReceiptInformationManager { get; set; }

        public static DisplayFunctionManager DisplayFunctionManager { get; set; }

        public static AllReceiptsInfoManager AllReceiptsInfoManager { get; set; }

        public static PeripheralChannel PeripheralChannel { get; set; }

        public static bool CheckCondition { get; set; }

        public static void SetSelectedModelInformation(ModelInformation modelInfo)
        {
            SelectedModelManager.SelectedModel = modelInfo;

            SaveModelInfo(modelInfo);
        }

        public static void SetSelectedPortInfo(PortInfo portInfo)
        {
            SelectedModelManager.SelectedPort = portInfo;

            SavePortInfo(portInfo);
        }

        public static void SetLanguage(Language language)
        {
            ReceiptInformationManager.Language = language;
        }

        public static void SetPaperSize(PaperSize paperSize)
        {
            ReceiptInformationManager.PaperSize = paperSize;
        }

        public static void SetBlackMarkType(BlackMarkType type)
        {
            SelectedModelManager.BlackMarkType = type;
        }

        public static void SetDisplayFunctionType(DisplayFunctionManager.FunctionType type)
        {
            DisplayFunctionManager.Type = type;
        }

        public static void SetDisplayInternationalType(DisplayInternationalType internationalType)
        {
            DisplayFunctionManager.SelectedInternationalType = internationalType;
        }

        public static void SetDisplayCodePageType(DisplayCodePageType codePageType)
        {
            DisplayFunctionManager.SelectedCodePageType = codePageType;
        }

        public static void SetAdditionDisplayFunctionType(DisplayFunctionManager.FunctionType type)
        {
            DisplayFunctionManager.AdditionType = type;
        }

        public static void SetAllReceiptsRegistrationFilePath(string filePath)
        {
            SMCloudServices.SetRegistrationConfigFilePath(filePath);

            Properties.Settings.Default.AllReceiptsRegistrationFilePath = filePath;

            Properties.Settings.Default.Save();
        }

        public static void ReplaceSelectedModelManagerParameter(SelectedModelManager source)
        {
            SelectedModelManager.SelectedModel = source.SelectedModel;
            SelectedModelManager.SelectedPort = source.SelectedPort;
        }

        public static string GetSelectedPortName()
        {
            return SelectedModelManager.SelectedPort.PortName;
        }

        public static string GetSelectedPortStrrings()
        {
            string portSettings = SelectedModelManager.SelectedModel.PortSettings;

            if (!portSettings.Contains(";l"))
            {
                portSettings += ";l";
            }

            return portSettings;
        }

        public static Emulation GetSelectedEmulation()
        {
            return SelectedModelManager.SelectedModel.DefaultEmulation;
        }

        public static LocalizeReceipt GetSelectedLocalizeReceipt()
        {
            return ReceiptInformationManager.LocalizeReceipt;
        }

        public static int GetSelectedActualPaperSize()
        {
            return ReceiptInformationManager.PaperSize.ActualPaperSize;
        }

        public static bool GetDrawerOpenStatus()
        {
            return SelectedModelManager.SelectedModel.DrawerOpenStatus;
        }

        public static int GetDisplayFunctionDefaultPatternIndex()
        {
            return DisplayFunctionManager.DefaultPatternIndex;
        }

        public static int GetDisplayFunctionDefaultAdditionPatternIndex()
        {
            return DisplayFunctionManager.DefaultAdditionPatternIndex;
        }

        public static bool GetAllReceiptsPrintReceipt()
        {
            return AllReceiptsInfoManager.PrintReceipt;
        }

        public static bool GetAllReceiptsPrintInformation()
        {
            return AllReceiptsInfoManager.PrintInformation;
        }

        public static bool GetAllReceiptsPrintQrCode()
        {
            return AllReceiptsInfoManager.PrintQrCode;
        }

        public static DisplayFunctionManager.FunctionType GetSelectedDisplayFunction()
        {
            return DisplayFunctionManager.Type;
        }

        public static DisplayFunctionManager.FunctionType GetSelectedAdditionDisplayFunction()
        {
            return DisplayFunctionManager.AdditionType;
        }

        public static DisplayInternationalType GetSelectedInternationalType()
        {
            return DisplayFunctionManager.SelectedInternationalType;
        }

        public static DisplayCodePageType GetSelectedCodePageType()
        {
            return DisplayFunctionManager.SelectedCodePageType;
        }

        public static BlackMarkType GetSelectedBlackMarkType()
        {
            return SelectedModelManager.BlackMarkType;
        }

        private static void SavePortInfo(PortInfo portInfo)
        {
            Properties.Settings.Default.PortName = portInfo.PortName;
            Properties.Settings.Default.MacAddress = portInfo.MacAddress;
            Properties.Settings.Default.ModelName = portInfo.ModelName;
            Properties.Settings.Default.USBSerialNumber = portInfo.USBSerialNumber;

            Properties.Settings.Default.Save();
        }

        private static void SaveModelInfo(ModelInformation modelInfo)
        {
            Properties.Settings.Default.SelectedModelIndex = (int)modelInfo.Model;
            Properties.Settings.Default.PortSettings = modelInfo.PortSettings;

            Properties.Settings.Default.Save();
        }

        public static void RestorePreviousInfo()
        {
            if (Properties.Settings.Default.PortName.Equals(""))
            {
                return;
            }

            string portName = Properties.Settings.Default.PortName;
            string macAddress = Properties.Settings.Default.MacAddress;
            string modelName = Properties.Settings.Default.ModelName;
            string usbSerialNumber = Properties.Settings.Default.USBSerialNumber;

            PortInfo portInfo = new PortInfo(portName, macAddress, modelName, usbSerialNumber);

            SelectedModelManager.SelectedPort = portInfo;

            int selectedModelIndex = Properties.Settings.Default.SelectedModelIndex;
            SelectedModelManager.SelectedModel = new ModelInformation((ModelInformation.PrinterModel)Enum.ToObject(typeof(ModelInformation.PrinterModel), selectedModelIndex));
            SelectedModelManager.SelectedModel.PortSettings = Properties.Settings.Default.PortSettings;
        }
    }
}
