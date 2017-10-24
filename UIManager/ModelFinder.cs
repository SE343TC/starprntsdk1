using System;
using System.Collections.Generic;
using System.Linq;
#if (!StarIO)
using StarMicronics.StarIOExtension;
#endif

#if (StarIO)
namespace StarMicronics.StarIO
#else
namespace StarPRNTSDK
#endif
{
    internal static class ModelFinder
    {
        public static ModelInformation.PrinterModel FindPrinterModel(string modelName)
        {
            if (modelName.Equals(""))
            {
                return ModelInformation.PrinterModel.Unknown;
            }

            return GetPrinterModelWithSomothingName(modelName);
        }

        public static ModelInformation.PrinterModel FindPrinterModel(string deviceId, string nicName)
        {
            if (deviceId.Equals("") || nicName.Equals(""))
            {
                return ModelInformation.PrinterModel.Unknown;
            }

            return GetPrinterModelWithDeviceIdAndNicName(deviceId, nicName);
        }

        public static bool IsSupportedDriverQueueName(string queueName)
        {
            ModelInformation.PrinterModel model = GetPrinterModelWithModelName(queueName);

            if (model != ModelInformation.PrinterModel.Unknown)
            {
                return true;
            }

            return false;
        }

        public static ModelDictionary.PrinterInfo GetPrinterInfo(ModelInformation.PrinterModel model)
        {
            return ModelDictionary.ModelInformationDictionary[model];
        }

        public static string GetModelName(ModelInformation.PrinterModel model)
        {
            ModelDictionary.PrinterInfo printerInfo = GetPrinterInfo(model);

            return printerInfo.modelName;
        }

        public static string[] GetAllSupportedModelName()
        {
            List<string> modelNameList = new List<string>();

            foreach (ModelInformation.PrinterModel model in Enum.GetValues(typeof(ModelInformation.PrinterModel)))
            {
                if (model != ModelInformation.PrinterModel.Unknown)
                {
                    modelNameList.Add(GetModelName(model));
                }
            }

            return modelNameList.ToArray();
        }


        public static ModelInformation.PrinterModel GetPrinterModelWithDeviceIdAndNicName(string deviceId, string nicName)
        {
            foreach (KeyValuePair<ModelInformation.PrinterModel, ModelDictionary.PrinterInfo> pair in ModelDictionary.ModelInformationDictionary)
            {
                ModelInformation.PrinterModel model = pair.Key;
                ModelDictionary.PrinterInfo info = pair.Value;

                string[] refDeviceId = info.deviceId;
                string refNicName = info.nicName;

                if (ContainsString(refDeviceId, deviceId) && CompareString(refNicName, nicName))
                {
                    return model;
                }
            }

            return ModelInformation.PrinterModel.Unknown;
        }

        public static ModelInformation.PrinterModel GetPrinterModelWithSomothingName(string somethingName)
        {
            ModelInformation.PrinterModel model = ModelInformation.PrinterModel.Unknown;

            model = GetPrinterModelWithModelName(somethingName);

            if (model != ModelInformation.PrinterModel.Unknown)
            {
                return model;
            }

            model = GetPrinterModelWithDeviceId(somethingName);

            if (model != ModelInformation.PrinterModel.Unknown)
            {
                return model;
            }

            model = GetPrinterModelWithBtDeviceNamePrefix(somethingName);

            return model;
        }

        public static ModelInformation.PrinterModel GetPrinterModelWithModelName(string modelName)
        {
            foreach (KeyValuePair<ModelInformation.PrinterModel, ModelDictionary.PrinterInfo> pair in ModelDictionary.ModelInformationDictionary)
            {
                ModelInformation.PrinterModel model = pair.Key;
                ModelDictionary.PrinterInfo info = pair.Value;

                string refModelName = info.modelName;

                if (CompareString(modelName, refModelName))
                {
                    return model;
                }
            }

            return ModelInformation.PrinterModel.Unknown;
        }

        public static ModelInformation.PrinterModel GetPrinterModelWithDeviceId(string deviceId)
        {
            foreach (KeyValuePair<ModelInformation.PrinterModel, ModelDictionary.PrinterInfo> pair in ModelDictionary.ModelInformationDictionary)
            {
                ModelInformation.PrinterModel model = pair.Key;
                ModelDictionary.PrinterInfo info = pair.Value;

                string[] refDeviceId = info.deviceId;

                if (ContainsString(refDeviceId, deviceId))
                {
                    return model;
                }
            }

            return ModelInformation.PrinterModel.Unknown;
        }

        public static ModelInformation.PrinterModel GetPrinterModelWithBtDeviceNamePrefix(string btDeviceNamePrefix)
        {
            foreach (KeyValuePair<ModelInformation.PrinterModel, ModelDictionary.PrinterInfo> pair in ModelDictionary.ModelInformationDictionary)
            {
                ModelInformation.PrinterModel model = pair.Key;
                ModelDictionary.PrinterInfo info = pair.Value;

                string[] refBtDeviceNamePrefix = info.btDeviceNamePrefix;

                if (StartsWith(btDeviceNamePrefix, refBtDeviceNamePrefix))
                {
                    return model;
                }
            }

            return ModelInformation.PrinterModel.Unknown;
        }

        public static string[] GetDeviceId(ModelInformation.PrinterModel model)
        {
            ModelDictionary.PrinterInfo printerInfo = GetPrinterInfo(model);

            return printerInfo.deviceId;
        }

        public static string GetNicName(ModelInformation.PrinterModel model)
        {
            ModelDictionary.PrinterInfo printerInfo = GetPrinterInfo(model);

            return printerInfo.nicName;
        }

        public static string[] GetBtDeviceNamePrefix(ModelInformation.PrinterModel model)
        {
            ModelDictionary.PrinterInfo printerInfo = GetPrinterInfo(model);

            return printerInfo.btDeviceNamePrefix;
        }

        public static string GetDefaultPortSettings(ModelInformation.PrinterModel model)
        {
            ModelDictionary.PrinterInfo printerInfo = GetPrinterInfo(model);

            return printerInfo.defaultPortSettings;
        }

        public static bool GetChangeDrawerOpenStatusIsEnabled(ModelInformation.PrinterModel model)
        {
            ModelDictionary.PrinterInfo printerInfo = GetPrinterInfo(model);

            return printerInfo.changeDrawerOpenStatusIsEnabled;
        }

        public static string GetSimpleModelName(ModelInformation.PrinterModel model)
        {
            ModelDictionary.PrinterInfo printerInfo = GetPrinterInfo(model);

            return printerInfo.simpleModelName;
        }


#if (!StarIO)
        public static Emulation GetEmulation(ModelInformation.PrinterModel model)
        {
            return ModelDictionary.ModelEmulationDictionary[model];
        }
#endif

        private static bool StartsWith(string sourceStr, string[] findStrArray)
        {
            sourceStr = sourceStr.ToUpper();

            for (int i = 0; i < findStrArray.Length; i++)
            {
                findStrArray[i] = findStrArray[i].ToUpper();

                if (sourceStr.StartsWith(findStrArray[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool StartsWith(string sourceStr, string findStr)
        {
            sourceStr = sourceStr.ToUpper();
            findStr = findStr.ToUpper();

            return sourceStr.StartsWith(findStr);
        }

        private static bool ContainsString(string[] sourceStrArray, string findStr)
        {
            for (int i = 0; i < sourceStrArray.Length; i++)
            {
                sourceStrArray[i] = sourceStrArray[i].ToUpper();
            }

            findStr = findStr.ToUpper();

            return sourceStrArray.Contains(findStr);
        }

        private static bool ContainsString(string sourceStr, string findStr)
        {
            sourceStr = sourceStr.ToUpper();
            findStr = findStr.ToUpper();

            return sourceStr.Contains(findStr);
        }

        private static bool CompareChar(char ch1, char ch2)
        {
            ch1 = char.ToUpper(ch1);
            ch2 = char.ToUpper(ch2);

            return ch1.Equals(ch2);
        }

        private static bool CompareString(string str1, string str2)
        {
            str1 = str1.ToUpper();
            str2 = str2.ToUpper();

            return str1.Equals(str2);
        }
    }
}
