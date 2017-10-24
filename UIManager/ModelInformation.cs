#if (!StarIO)
using StarMicronics.StarIOExtension;
#endif

#if (StarIO)
namespace StarMicronics.StarIO
#else
namespace StarPRNTSDK
#endif
{
    internal class ModelInformation
    {
        public enum PrinterModel : int
        {
            L200 = 0,
            L300,
            S210i,
            S220i,
            S230i,
            T300i,
            T400i,
            S210i_StarPRNT,
            S220i_StarPRNT,
            S230i_StarPRNT,
            T300i_StarPRNT,
            T400i_StarPRNT,
            BSC10,
            BSC10BR,
            TSP043,
            FVP10,
            SP542,
            SP512,
            SP717,
            SP747,
            SP712,
            SP742,
            TSP654II,
            TSP743II,
            TSP847II,
            TUP542,
            TUP592,
            TUP942,
            TUP992,
            POP10,
            TSP143,
            TSP113,
            TSP143GT,
            TSP113GT,
            SAC10,
            SAC10W,
            Unknown
        }

        public ModelInformation(PrinterModel model)
        {
            drawerOpenStatus = null;

            Model = model;
        }

        public ModelInformation() : this(PrinterModel.Unknown) { }

        public PrinterModel Model { get; set; }

        public string ModelName { get { return ModelFinder.GetModelName(Model); } }

        public string[] DeviceId { get { return ModelFinder.GetDeviceId(Model); } }

        public string NicName { get { return ModelFinder.GetNicName(Model); } }

        public string[] BtDeviceNamePrefix { get { return ModelFinder.GetBtDeviceNamePrefix(Model); } }

        public string DefaultPortSettings { get { return ModelFinder.GetDefaultPortSettings(Model); } }

        public bool ChangeDrawerOpenStatusIsEnabled { get { return ModelFinder.GetChangeDrawerOpenStatusIsEnabled(Model); } }

        public string SimpleModelName { get { return ModelFinder.GetSimpleModelName(Model); } }

#if (!StarIO)
        public Emulation DefaultEmulation { get { return ModelFinder.GetEmulation(Model); } }
#endif

        public string PortSettings
        {
            get
            {
                return GetPortSettings();
            }
            set
            {
                portSettings = value;
            }
        }

        private string portSettings;

        private string GetPortSettings()
        {
            if (portSettings == null)
            {
                return DefaultPortSettings;
            }
            else
            {
                if(!DefaultPortSettings.Equals("") && !portSettings.Contains(DefaultPortSettings))
                {
                    return portSettings + ";" + DefaultPortSettings;
                }
                else
                {
                    return portSettings;
                }
            }
        }

        public bool DrawerOpenStatus
        {
            get
            {
                return GetDrawerOpenStatus();
            }
            set
            {
                drawerOpenStatus = value;
            }
        }

        private bool? drawerOpenStatus;

        private bool GetDrawerOpenStatus()
        {
            if (drawerOpenStatus == null)
            {
                return true;
            }
            else
            {
                return (bool)drawerOpenStatus;
            }
        }
    }
}
