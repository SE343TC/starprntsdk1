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
    internal class ModelDictionary
    {
        public static Dictionary<ModelInformation.PrinterModel, PrinterInfo> ModelInformationDictionary
        {
            get
            {
                return new Dictionary<ModelInformation.PrinterModel, PrinterInfo>()
                {
                    { ModelInformation.PrinterModel.L200,
                      new PrinterInfo("Star SM-L200",
                      new string[] {  "SM-L200 (STAR_T-001)" },
                                      "",
                      new string[] {  "STAR L200-", "STAR L204-" },
                                      "",
                                      false,
                                      "SM-L200") },

                    { ModelInformation.PrinterModel.L300,
                      new PrinterInfo("Star SM-L300",
                      new string[] {  "SM-L300 (STAR_T-001)" },
                                      "",
                      new string[] {  "STAR L300-", "STAR L304-" },
                                      "",
                                      false,
                                      "SM-L300") },

                    { ModelInformation.PrinterModel.S210i,
                      new PrinterInfo("Star SM-S210i",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "mini",
                                      false,
                                      "SM-S210i") },

                    { ModelInformation.PrinterModel.S220i,
                      new PrinterInfo("Star SM-S220i",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "mini",
                                      false,
                                      "SM-S220i") },

                    { ModelInformation.PrinterModel.S230i,
                      new PrinterInfo("Star SM-S230i",
                      new string[] {  "SM-S230i (STAR_T-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "mini",
                                      false,
                                      "SM-S230i") },

                    { ModelInformation.PrinterModel.T300i,
                      new PrinterInfo("Star SM-T300i",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "mini",
                                      false,
                                      "SM-T300i") },

                    { ModelInformation.PrinterModel.T400i,
                      new PrinterInfo("Star SM-T400i",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "mini",
                                      false,
                                      "SM-T400i") },

                    { ModelInformation.PrinterModel.S210i_StarPRNT,
                      new PrinterInfo("",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      false,
                                      "SM-S210i StarPRNT") },

                    { ModelInformation.PrinterModel.S220i_StarPRNT,
                      new PrinterInfo("",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      false,
                                      "SM-S220i StarPRNT") },

                    { ModelInformation.PrinterModel.S230i_StarPRNT,
                      new PrinterInfo("",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      false,
                                      "SM-S230i StarPRNT") },

                    { ModelInformation.PrinterModel.T300i_StarPRNT,
                      new PrinterInfo("",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      false,
                                      "SM-T300i StarPRNT") },

                    { ModelInformation.PrinterModel.T400i_StarPRNT,
                      new PrinterInfo("",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      false,
                                      "SM-T400i StarPRNT") },

                    { ModelInformation.PrinterModel.BSC10,
                      new PrinterInfo("Star BSC10",
                      new string[] {  "BSC10 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "escpos",
                                      true,
                                      "BSC10") },

                    { ModelInformation.PrinterModel.BSC10BR,
                      new PrinterInfo("Star BSC10BR",
                      new string[] {  "BSC10BR (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "escpos",
                                      true,
                                      "BSC10") },

                    { ModelInformation.PrinterModel.TSP043,
                      new PrinterInfo("Star TSP043",
                      new string[] {  "TSP043 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "escpos",
                                      true,
                                      "BSC10") },

                    { ModelInformation.PrinterModel.FVP10,
                      new PrinterInfo("Star FVP10",
                      new string[] {  "FVP10 (STR_T-001)", "FVP10 (STRP-001)", "FVP10 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "FVP10") },

                    { ModelInformation.PrinterModel.SP542,
                      new PrinterInfo("Star SP500 Cutter (SP542)",
                      new string[] {  "SP542 (STR-001)", "SP542 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "SP500") },

                    { ModelInformation.PrinterModel.SP512,
                      new PrinterInfo("Star SP500 TearBar (SP512)",
                      new string[] {  "SP512 (STR-001)", "SP512 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "SP500") },

                    { ModelInformation.PrinterModel.SP717,
                      new PrinterInfo("Star SP700 TearBar (SP717)",
                      new string[] {  "SP717 (STR-001)", "SP717 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "SP700") },

                    { ModelInformation.PrinterModel.SP747,
                      new PrinterInfo("Star SP700 Cutter (SP747)",
                      new string[] {  "SP747 (STR-001)", "SP747 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "SP700") },

                    { ModelInformation.PrinterModel.SP712,
                      new PrinterInfo("Star SP700 TearBar (SP712)",
                      new string[] {  "SP712 (STR-001)", "SP712 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "SP700") },

                    { ModelInformation.PrinterModel.SP742,
                      new PrinterInfo("Star SP700 Cutter (SP742)",
                      new string[] {  "SP742 (STR-001)", "SP742 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "SP700") },

                    { ModelInformation.PrinterModel.TSP654II,
                      new PrinterInfo("Star TSP650II Cutter (TSP654II)",
                      new string[] {  "TSP654 (STR_T-001)", "TSP654 (ESP-001)", "TSP651 (STR_T-001)", "TSP651 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "TSP650II") },

                    { ModelInformation.PrinterModel.TSP743II,
                      new PrinterInfo("Star TSP700II (TSP743II)",
                      new string[] {  "TSP743II (STR_T-001)", "TSP743II (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "TSP700II") },

                    { ModelInformation.PrinterModel.TSP847II,
                      new PrinterInfo("Star TSP800II (TSP847II)",
                      new string[] {  "ops_TSP800 (STR_R-U001)", "ops_TSP800 (STR_T-U001)", "TSP800 (STR_R-001)", "TSP800 (STR_R-U001)", "TSP800 (STR_T-001)", "TSP800 (STR_T-E001)", "TSP800 (STR_T-U001)",
                                      "TSP800 (STR-E001)", "TSP800 (STRP-E001)", "TSP800 (ESP-E001)", "TSP800(STRP-U001)", "TSP847II (STR_T-001)", "TSP847II (STRP-001)", "TSP847II (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "TSP800II") },

                    { ModelInformation.PrinterModel.TUP542,
                      new PrinterInfo("Star TUP500 (TUP542)",
                      new string[] {  "TUP542 (STR_T-001)", "TUP542 (STRP-001)", "TUP542 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      false,
                                      "TUP500") },

                    { ModelInformation.PrinterModel.TUP592,
                      new PrinterInfo("Star TUP500 Presenter (TUP592)",
                      new string[] {  "TUP592 (STR_T-001)", "TUP592 (STRP-001)", "TUP592 (ESP-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      false,
                                      "TUP500") },

                    { ModelInformation.PrinterModel.TUP942,
                      new PrinterInfo("Star TUP900 (TUP942)",
                      new string[] {  "TUP900 (STR_T-U001)", "TUP900 (STRP-U001)", "TUP900 (ESP-U001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      false,
                                      "TUP900") },

                    { ModelInformation.PrinterModel.TUP992,
                      new PrinterInfo("Star TUP900 Presenter (TUP992)",
                      new string[] {  "TUP992 (STR_T-U001)", "TUP992 (STRP-U001)", "TUP992 (ESP-U001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      false,
                                      "TUP900") },

                    { ModelInformation.PrinterModel.POP10,
                      new PrinterInfo("Star POP10",
                      new string[] {  "POP10 (STR-001)" },
                                      "",
                      new string[] {  "STAR mPOP-" },
                                      "",
                                      false,
                                      "mPOP") },

                    { ModelInformation.PrinterModel.TSP143,
                      new PrinterInfo("Star TSP100 Cutter (TSP143)",
                      new string[] {  "TSP143 (STR_T-001)", "TSP143IIIW (STR_T-001)", "TSP143IIILAN (STR_T-001)" },
                                      "TSP100LAN",
                      new string[] {  "TSP100-", "TSP113"},
                                      "",
                                      true,
                                      "TSP100") },

                    { ModelInformation.PrinterModel.TSP113,
                      new PrinterInfo("Star TSP100 Tear Bar (TSP113)",
                      new string[] {  "TSP113 (STR_T-001)" },
                                      "TSP100LAN",
                      new string[] {  "TSP100-", "TSP113"},
                                      "",
                                      true,
                                      "TSP100") },

                    { ModelInformation.PrinterModel.TSP143GT,
                      new PrinterInfo("Star TSP143GT Cutter",
                      new string[] {  "TSP143GT (STR_T-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "TSP100") },

                    { ModelInformation.PrinterModel.TSP113GT,
                      new PrinterInfo("Star TSP113GT Tear Bar",
                      new string[] {  "TSP113GT (STR_T-001)" },
                                      "",
                                      (string[])Enumerable.Empty<string>(),
                                      "",
                                      true,
                                      "TSP100") },

                     { ModelInformation.PrinterModel.SAC10,
                       new PrinterInfo("",
                       new string[] {  "SAC10" },
                                       "DK-AirCash",
                                       (string[])Enumerable.Empty<string>(),
                                       "",
                                       false,
                                       "SAC10") },

                     { ModelInformation.PrinterModel.SAC10W,
                       new PrinterInfo("",
                       new string[] {  "SAC10" },
                                       "DK-AirCash-W",
                                       (string[])Enumerable.Empty<string>(),
                                       "WL",
                                       false,
                                       "SAC10") },

                     { ModelInformation.PrinterModel.Unknown,
                       new PrinterInfo("",
                                       (string[])Enumerable.Empty<string>(),
                                       "",
                                       (string[])Enumerable.Empty<string>(),
                                       "",
                                       false,
                                       "") },

                };
            }
        }

#if (!StarIO)
        public static Dictionary<ModelInformation.PrinterModel, Emulation> ModelEmulationDictionary
        {
            get
            {
                return new Dictionary<ModelInformation.PrinterModel, Emulation>()
                {
                    { ModelInformation.PrinterModel.L200, Emulation.StarPRNT },

                    { ModelInformation.PrinterModel.L300, Emulation.StarPRNTL },

                    { ModelInformation.PrinterModel.S210i, Emulation.EscPosMobile },

                    { ModelInformation.PrinterModel.S220i, Emulation.EscPosMobile },

                    { ModelInformation.PrinterModel.S230i, Emulation.EscPosMobile },

                    { ModelInformation.PrinterModel.T300i, Emulation.EscPosMobile },

                    { ModelInformation.PrinterModel.T400i, Emulation.EscPosMobile },

                    { ModelInformation.PrinterModel.S210i_StarPRNT, Emulation.StarPRNT },

                    { ModelInformation.PrinterModel.S220i_StarPRNT, Emulation.StarPRNT },

                    { ModelInformation.PrinterModel.S230i_StarPRNT, Emulation.StarPRNT },

                    { ModelInformation.PrinterModel.T300i_StarPRNT, Emulation.StarPRNT },

                    { ModelInformation.PrinterModel.T400i_StarPRNT, Emulation.StarPRNT },

                    { ModelInformation.PrinterModel.BSC10, Emulation.EscPos },

                    { ModelInformation.PrinterModel.BSC10BR, Emulation.EscPos },

                    { ModelInformation.PrinterModel.TSP043, Emulation.EscPos },

                    { ModelInformation.PrinterModel.FVP10, Emulation.StarLine },

                    { ModelInformation.PrinterModel.SP542, Emulation.StarDotImpact },

                    { ModelInformation.PrinterModel.SP512, Emulation.StarDotImpact },

                    { ModelInformation.PrinterModel.SP717, Emulation.StarDotImpact },

                    { ModelInformation.PrinterModel.SP747, Emulation.StarDotImpact },

                    { ModelInformation.PrinterModel.SP712, Emulation.StarDotImpact },

                    { ModelInformation.PrinterModel.SP742, Emulation.StarDotImpact },

                    { ModelInformation.PrinterModel.TSP654II, Emulation.StarLine },

                    { ModelInformation.PrinterModel.TSP743II, Emulation.StarLine },

                    { ModelInformation.PrinterModel.TSP847II, Emulation.StarLine },

                    { ModelInformation.PrinterModel.TUP542, Emulation.StarLine },

                    { ModelInformation.PrinterModel.TUP592, Emulation.StarLine },

                    { ModelInformation.PrinterModel.TUP942, Emulation.StarLine },

                    { ModelInformation.PrinterModel.TUP992, Emulation.StarLine },

                    { ModelInformation.PrinterModel.POP10, Emulation.StarPRNT },

                    { ModelInformation.PrinterModel.TSP143, Emulation.StarGraphic },

                    { ModelInformation.PrinterModel.TSP113, Emulation.StarGraphic },

                    { ModelInformation.PrinterModel.TSP143GT, Emulation.StarGraphic },

                    { ModelInformation.PrinterModel.TSP113GT, Emulation.StarGraphic },

                    { ModelInformation.PrinterModel.SAC10, Emulation.StarLine },

                    { ModelInformation.PrinterModel.SAC10W, Emulation.StarLine },

                    { ModelInformation.PrinterModel.Unknown, Emulation.None },

                };
            }
        }
#endif

        public struct PrinterInfo
        {
            public string modelName;
            public string[] deviceId;
            public string nicName;
            public string[] btDeviceNamePrefix;
            public string defaultPortSettings;
            public bool changeDrawerOpenStatusIsEnabled;
            public string simpleModelName;

            public PrinterInfo(string modelName, string[] deviceId, string nicName, string[] btDeviceNamePrefix, string defaultPortSettings, bool changeDrawerOpenStatusIsEnabled, string simpleModelName)
            {
                this.modelName = modelName;
                this.deviceId = deviceId;
                this.nicName = nicName;
                this.btDeviceNamePrefix = btDeviceNamePrefix;
                this.defaultPortSettings = defaultPortSettings;
                this.changeDrawerOpenStatusIsEnabled = changeDrawerOpenStatusIsEnabled;
                this.simpleModelName = simpleModelName;
            }
        }
    }
}
