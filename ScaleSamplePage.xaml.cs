using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System.Windows.Controls;

namespace StarPRNTSDK
{
    public static class ScaleSampleManager
    {
        /// <summary>
        /// Sample : Getting scale status.
        /// </summary>
        public static Communication.PeripheralStatus GetScaleStatus(IPort port)
        {
            // Create IPeripheralConnectParser object.
            IPeripheralConnectParser parser = StarIoExt.CreateScaleConnectParser(ScaleModel.APS10);

            // Usage of parser sample is "Communication.ParseDoNotCheckCondition(IPeripheralCommandParser parser, IPort port)".
            Communication.CommunicationResult result = Communication.ParseDoNotCheckCondition(parser, port);

            // Check peripheral status.
            Communication.PeripheralStatus status = Communication.PeripheralStatus.Invalid;
            if (result == Communication.CommunicationResult.Success)
            {
                // Check parser property value.
                if (parser.IsConnected) // connect
                {
                    status = Communication.PeripheralStatus.Connect;
                }
                else // disconnect
                {
                    status = Communication.PeripheralStatus.Disconnect;
                }
            }
            else // communication error
            {
                status = Communication.PeripheralStatus.Impossible;
            }

            return status;
        }

        /// <summary>
        /// Sample : Getting displayed weight.
        /// </summary>
        public static Communication.PeripheralStatus GetScaleDisplayedWeight(IPort port, ref DisplayedWeightStatus weightStatus, ref string weight)
        {
            // Check scale status.
            Communication.PeripheralStatus status = GetScaleStatus(port);

            if (status != Communication.PeripheralStatus.Connect) // Scale is not connected.
            {
                return status;
            }

            // Create IScaleWeightParser object.
            IScaleWeightParser parser = StarIoExt.CreateScaleWeightParser(ScaleModel.APS10);

            // Usage of parser sample is "ScaleCommunication.ParseDoNotCheckCondition(IPeripheralCommandParser parser, IPort port)".
            Communication.CommunicationResult result = ScaleCommunication.ParseDoNotCheckCondition(parser, port);

            if (result != Communication.CommunicationResult.Success) // communication error.
            {
                return Communication.PeripheralStatus.Impossible;
            }

            // Check parser property value.
            weightStatus = parser.Status; // WeightStatus
            weight = parser.Weight; // weight

            return Communication.PeripheralStatus.Connect; // Success
        }

        /// <summary>
        /// Sample : Sending zero clear commands.
        /// </summary>
        public static Communication.PeripheralStatus SendZeroClearCommands(IPort port)
        {
            // Check scale status.
            Communication.PeripheralStatus status = GetScaleStatus(port);

            if (status != Communication.PeripheralStatus.Connect) // Scale is not connected.
            {
                return status;
            }

            // Create scale commands.
            byte[] scaleCommands = ScaleFunctions.CreateZeroClear();

            // Send scale commands.
            Communication.CommunicationResult result = Communication.SendCommandsDoNotCheckCondition(scaleCommands, port);

            if (result != Communication.CommunicationResult.Success)
            {
                return Communication.PeripheralStatus.Impossible;
            }
            else
            {
                return Communication.PeripheralStatus.Connect;
            }
        }

        /// <summary>
        /// Sample : Sending unit change commands.
        /// </summary>
        public static Communication.PeripheralStatus SendUnitChangeCommands(IPort port)
        {
            // Check scale status.
            Communication.PeripheralStatus status = GetScaleStatus(port);

            if (status != Communication.PeripheralStatus.Connect) // Scale is not connected.
            {
                return status;
            }

            // Create scale commands.
            byte[] scaleCommands = ScaleFunctions.CreateUnitChange();

            // Send scale commands.
            Communication.CommunicationResult result = Communication.SendCommandsDoNotCheckCondition(scaleCommands, port);

            if (result != Communication.CommunicationResult.Success)
            {
                return Communication.PeripheralStatus.Impossible;
            }
            else
            {
                return Communication.PeripheralStatus.Connect;
            }
        }

        public static Communication.PeripheralStatus CallScaleFunction(CallScaleFunctionClickEvent.ScallFunctionType type, IPort port, ref DisplayedWeightStatus weightStatus, ref string weight)
        {
            Communication.PeripheralStatus result = Communication.PeripheralStatus.Invalid;

            switch (type)
            {
                default:
                case CallScaleFunctionClickEvent.ScallFunctionType.CheckStatus:
                    result = GetScaleStatus(port);
                    break;

                case CallScaleFunctionClickEvent.ScallFunctionType.DisplayedWeight:
                    result = GetScaleDisplayedWeight(port, ref weightStatus, ref weight);
                    break;

                case CallScaleFunctionClickEvent.ScallFunctionType.ZeroClear:
                    result = SendZeroClearCommands(port);
                    break;

                case CallScaleFunctionClickEvent.ScallFunctionType.UnitChange:
                    result = SendUnitChangeCommands(port);
                    break;
            }

            return result;
        }

        public static Communication.PeripheralStatus CallScaleFunctionWithProgressBar(CallScaleFunctionClickEvent.ScallFunctionType type, ref DisplayedWeightStatus weightStatus, ref string weight)
        {
            Communication.PeripheralStatus result = Communication.PeripheralStatus.Invalid;

            DisplayedWeightStatus tempWeightStatus = DisplayedWeightStatus.Invalid;
            string tempWeight = "";

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {
                IPort port = null;

                try
                {
                    string portName = SharedInformationManager.GetSelectedPortName();
                    string portSettings = SharedInformationManager.GetSelectedPortStrrings();

                    port = Factory.I.GetPort(portName, portSettings, 30000);

                    result = CallScaleFunction(type, port, ref tempWeightStatus, ref tempWeight);
                }
                catch (PortException)
                {
                }
                finally
                {
                    if (port != null)
                    {
                        Factory.I.ReleasePort(port);
                    }
                }
            });

            progressBarWindow.ShowDialog();

            weightStatus = tempWeightStatus;
            weight = tempWeight;

            return result;
        }
    }

    public class CallScaleFunctionClickEvent : BaseCommand
    {
        public enum ScallFunctionType
        {
            CheckStatus,
            DisplayedWeight,
            ZeroClear,
            UnitChange
        }

        public ScallFunctionType Type { get; set; }

        public override void Execute(object parameter)
        {
            Communication.PeripheralStatus result = Communication.PeripheralStatus.Invalid;
            DisplayedWeightStatus weightStatus = DisplayedWeightStatus.Invalid;
            string weight = "";

            result = ScaleSampleManager.CallScaleFunctionWithProgressBar(Type, ref weightStatus, ref weight);

            if (result != Communication.PeripheralStatus.Connect ||
                Type == ScallFunctionType.CheckStatus)
            {
                Communication.ShowPeripheralStatusResultMessage("Scale", result);

                return;
            }

            switch (Type)
            {
                default:
                    Util.ShowMessage("Communication Result", "Success");
                    break;

                case ScallFunctionType.DisplayedWeight:
                    ShowScaleDisplayedWeight(result, weightStatus, weight);
                    break;
            }
        }

        public static void ShowScaleDisplayedWeight(Communication.PeripheralStatus status, DisplayedWeightStatus weightStatus, string weight)
        {
            string caption = GetDisplayedWeightStatusDescription(weightStatus);
            string message = weight;

            Util.ShowMessage(caption, message);
        }


        private static string GetDisplayedWeightStatusDescription(DisplayedWeightStatus weightStatus)
        {
            string description;

            switch (weightStatus)
            {
                default:
                case DisplayedWeightStatus.Zero:
                    description = "Success  [ Zero ]";
                    break;

                case DisplayedWeightStatus.NotInMotion:
                    description = "Success  [ Not in motion ]";
                    break;

                case DisplayedWeightStatus.Motion:
                    description = "Success  [ Motion ]";
                    break;
            }

            return description;
        }
    }

    public partial class ScaleSamplePage : Page
    {
        public ScaleSamplePage()
        {
            InitializeComponent();
        }
    }
}
