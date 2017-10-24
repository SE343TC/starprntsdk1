using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StarPRNTSDK
{
    public static class Communication
    {

        public enum CommunicationResult
        {
            Success,
            ErrorUnknown,
            ErrorOpenPort,
            ErrorBeginCheckedBlock,
            ErrorEndCheckedBlock,
            ErrorWritePort,
            ErrorReadPort,
        }

        public enum PeripheralStatus
        {
            Invalid,
            Impossible,
            Connect,
            Disconnect
        }

        /// <summary>
        /// Sample : Sending commands to printer with check condition.
        /// </summary>
        public static CommunicationResult SendCommands(byte[] commands, string portName, string portSettings, int timeout)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            IPort port = null;

            try
            {
                result = CommunicationResult.ErrorOpenPort;

                port = Factory.I.GetPort(portName, portSettings, timeout);

                StarPrinterStatus status;

                result = CommunicationResult.ErrorBeginCheckedBlock;

                status = port.BeginCheckedBlock();

                if (status.Offline)
                {
                    string message = "Printer is Offline.";

                    if (status.ReceiptPaperEmpty)
                    {
                        message += "\nPaper is Empty.";
                    }

                    if (status.CoverOpen)
                    {
                        message += "\nCover is Open.";
                    }

                    throw new PortException(message);
                }

                result = CommunicationResult.ErrorWritePort;

                uint commandsLength = (uint)commands.Length;

                uint writtenLength = port.WritePort(commands, 0, commandsLength);

                if (writtenLength != commandsLength)
                {
                    throw new PortException("WritePort failed.");
                }

                result = CommunicationResult.ErrorEndCheckedBlock;

                status = port.EndCheckedBlock();

                if (status.Offline == true)
                {
                    string message = "Printer is Offline.";

                    if (status.ReceiptPaperEmpty == true)
                    {
                        message += "\nPaper is Empty.";
                    }

                    if (status.CoverOpen == true)
                    {
                        message += "\nCover is Open.";
                    }

                    throw new PortException(message);
                }

                result = CommunicationResult.Success;
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

            return result;
        }

        /// <summary>
        /// Sample : Sending commands to printer without check condition.
        /// </summary>
        public static CommunicationResult SendCommandsDoNotCheckCondition(byte[] commands, string portName, string portSettings, int timeout)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            IPort port = null;

            try
            {
                result = CommunicationResult.ErrorOpenPort;

                port = Factory.I.GetPort(portName, portSettings, timeout);

                StarPrinterStatus status;

                result = CommunicationResult.ErrorWritePort;

                status = port.GetParsedStatus();

                if (status.RawStatus.Length == 0)
                {
                    throw new PortException("Unable to communicate with printer.");
                }

                uint commandsLength = (uint)commands.Length;

                uint writtenLength = port.WritePort(commands, 0, commandsLength);

                if (writtenLength != commandsLength)
                {
                    throw new PortException("WritePort failed.");
                }

                result = CommunicationResult.ErrorWritePort;

                status = port.GetParsedStatus();

                result = CommunicationResult.Success;
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

            return result;
        }

        /// <summary>
        /// Sample : Sending commands to printer with check condition (already open port).
        /// </summary>
        public static CommunicationResult SendCommands(byte[] commands, IPort port)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            try
            {
                if (port == null)
                {
                    return CommunicationResult.ErrorOpenPort;
                }

                StarPrinterStatus status;

                result = CommunicationResult.ErrorBeginCheckedBlock;

                status = port.BeginCheckedBlock();

                if (status.Offline)
                {
                    string message = "Printer is Offline.";

                    if (status.ReceiptPaperEmpty)
                    {
                        message += "\nPaper is Empty.";
                    }

                    if (status.CoverOpen)
                    {
                        message += "\nCover is Open.";
                    }

                    throw new PortException(message);
                }

                result = CommunicationResult.ErrorWritePort;

                uint commandsLength = (uint)commands.Length;

                uint writtenLength = port.WritePort(commands, 0, commandsLength);

                if (writtenLength != commandsLength)
                {
                    throw new PortException("WritePort failed.");
                }

                result = CommunicationResult.ErrorEndCheckedBlock;

                status = port.EndCheckedBlock();

                if (status.Offline == true)
                {
                    string message = "Printer is Offline.";

                    if (status.ReceiptPaperEmpty == true)
                    {
                        message += "\nPaper is Empty.";
                    }

                    if (status.CoverOpen == true)
                    {
                        message += "\nCover is Open.";
                    }

                    throw new PortException(message);
                }

                result = CommunicationResult.Success;
            }
            catch (PortException)
            {
            }

            return result;
        }

        /// <summary>
        /// Sample : Sending commands to printer without check condition (already open port).
        /// </summary>
        public static CommunicationResult SendCommandsDoNotCheckCondition(byte[] commands, IPort port)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            try
            {
                if (port == null)
                {
                    return CommunicationResult.ErrorOpenPort;
                }

                StarPrinterStatus status;

                result = CommunicationResult.ErrorWritePort;

                status = port.GetParsedStatus();

                uint commandsLength = (uint)commands.Length;

                uint writtenLength = port.WritePort(commands, 0, commandsLength);

                if (writtenLength != commandsLength)
                {
                    throw new PortException("WritePort failed.");
                }

                result = CommunicationResult.ErrorWritePort;

                status = port.GetParsedStatus();

                result = CommunicationResult.Success;
            }
            catch (PortException)
            {
            }

            return result;
        }

        /// <summary>
        /// Sample : Retrieving printer status.
        /// </summary>
        public static CommunicationResult RetrieveStatus(ref StarPrinterStatus printerStatus, string portName, string portSettings, int timeout)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            IPort port = null;

            try
            {
                result = CommunicationResult.ErrorOpenPort;

                port = Factory.I.GetPort(portName, portSettings, timeout);

                result = CommunicationResult.ErrorReadPort;

                printerStatus = port.GetParsedStatus();

                result = CommunicationResult.Success;
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

            return result;
        }

        /// <summary>
        /// Sample : Getting printer firmware information.
        /// </summary>
        public static CommunicationResult RequestFirmwareInformation(ref Dictionary<string, string> firmwareInformation, IPort port)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            try
            {
                result = CommunicationResult.ErrorOpenPort;

                if (port == null)
                {
                    return CommunicationResult.ErrorOpenPort;
                }

                result = CommunicationResult.ErrorReadPort;

                firmwareInformation = port.GetFirmwareInformation();

                result = CommunicationResult.Success;
            }
            catch (PortException)
            {
            }

            return result;
        }

        /// <summary>
        /// Sample : Parse printer response.
        /// </summary>
        public static CommunicationResult ParseDoNotCheckCondition(IPeripheralCommandParser parser, IPort port)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            try
            {
                result = CommunicationResult.ErrorOpenPort;

                if (port == null)
                {
                    return CommunicationResult.ErrorOpenPort;
                }

                result = CommunicationResult.ErrorWritePort;

                StarPrinterStatus printerStatus = port.GetParsedStatus();

                byte[] commands = parser.SendCommands;

                port.WritePort(commands, 0, (uint)commands.Length);

                result = CommunicationResult.ErrorReadPort;
                byte[] readBuffer = new byte[1024];
                uint totalReceiveSize = 0;

                uint startDate = (uint)Environment.TickCount;

                while (true)
                {
                    Thread.Sleep(10);

                    uint receiveSize = port.ReadPort(ref readBuffer, totalReceiveSize, (uint)(readBuffer.Length - totalReceiveSize));

                    if (receiveSize > 0)
                    {
                        totalReceiveSize += receiveSize;
                    }

                    byte[] receiveData = new byte[totalReceiveSize];

                    Array.Copy(readBuffer, 0, receiveData, 0, totalReceiveSize);

                    if (parser.Parse(receiveData, (int)totalReceiveSize) == ParseResult.Success)
                    {
                        result = CommunicationResult.Success;

                        break;
                    }

                    if ((UInt32)Environment.TickCount - startDate >= 1000) // Timeout
                    {
                        throw new PortException("ReadPort timeout.");
                    }
                }

            }
            catch (PortException)
            {
            }

            return result;
        }

        /// <summary>
        /// Sample : Getting printer serial number.
        /// </summary>
        public static CommunicationResult GetSerialNumber(ref string serialNumber, IPort port)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            try
            {
                result = CommunicationResult.ErrorOpenPort;

                if (port == null)
                {
                    return CommunicationResult.ErrorOpenPort;
                }

                result = CommunicationResult.ErrorWritePort;

                StarPrinterStatus printerStatus = port.GetParsedStatus();

                byte[] getInformationCommands = new byte[] { 0x1b, 0x1d, 0x29, 0x49, 0x01, 0x00, 49 }; // ESC GS ) I pL pH fn (Transmit printer information command)

                port.WritePort(getInformationCommands, 0, (uint)getInformationCommands.Length);

                result = CommunicationResult.ErrorReadPort;
                byte[] readBuffer = new byte[1024];
                uint totalReceiveSize = 0;
                string information = "";

                uint startDate = (uint)Environment.TickCount;

                while (true)
                {
                    if ((UInt32)Environment.TickCount - startDate >= 3000) // Timeout
                    {
                        throw new PortException("ReadPort timeout.");
                    }

                    uint receiveSize = port.ReadPort(ref readBuffer, totalReceiveSize, (uint)(readBuffer.Length - totalReceiveSize));

                    if (receiveSize > 0)
                    {
                        totalReceiveSize += receiveSize;
                    }
                    else
                    {
                        continue;
                    }

                    byte[] receiveData = new byte[totalReceiveSize];

                    Array.Copy(readBuffer, 0, receiveData, 0, totalReceiveSize);

                    bool receiveResponse = false;

                    if (totalReceiveSize >= 2)
                    {
                        for (int i = 0; i < totalReceiveSize; i++)
                        {
                            if (receiveData[i] == 0x0a && // Check the footer of the command.
                               receiveData[i + 1] == 0x00)
                            {
                                for (int j = 0; j < totalReceiveSize - 9; j++)
                                {
                                    if (receiveData[j] == 0x1b &&
                                        receiveData[j + 1] == 0x1d &&
                                        receiveData[j + 2] == 0x29 &&
                                        receiveData[j + 3] == 0x49 &&
                                        receiveData[j + 4] == 0x01 &&
                                        receiveData[j + 5] == 0x00 &&
                                        receiveData[j + 6] == 49)
                                    {
                                        string responseStr = Encoding.ASCII.GetString(receiveData);

                                        int infoStartIndex = j + 7;                     // information start index.
                                        int infoEndIndex = (int)(totalReceiveSize - 2); // information end index.
                                        information = responseStr.Substring(infoStartIndex, infoEndIndex - infoStartIndex); // Extract information from priinter response.

                                        receiveResponse = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (receiveResponse)
                    {
                        break;
                    }
                }

                int serialNumberStartIndex = information.IndexOf("PrSrN="); // Check serial number tag.

                if (serialNumberStartIndex == -1)
                {
                    throw new PortException("Parse serial number failed.");
                }

                serialNumberStartIndex += "PrSrN=".Length;

                string temp = information.Substring(serialNumberStartIndex);

                int serialNumberEndIndex = temp.IndexOf(","); // Check comma.

                if (serialNumberEndIndex == -1) // Not find comma.
                {
                    serialNumberEndIndex = temp.Length; // End of information.
                }

                serialNumber = temp.Substring(0, serialNumberEndIndex); // Parse serial number information.

                int nullIndex = serialNumber.IndexOf("\0"); // Check null(for clone serial number).

                if(nullIndex != -1) // Find null.
                {
                    serialNumber = serialNumber.Substring(0, nullIndex);
                }

                result = CommunicationResult.Success;
            }
            catch (PortException)
            {
            }

            return result;
        }

        public static CommunicationResult RequestFirmwareInformation(ref Dictionary<string, string> firmwareInformation, string portName, string portSettings, int timeout)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            IPort port = null;

            try
            {
                result = CommunicationResult.ErrorOpenPort;

                port = Factory.I.GetPort(portName, portSettings, timeout);

                result = RequestFirmwareInformation(ref firmwareInformation, port);
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

            return result;
        }

        public static CommunicationResult ParseDoNotCheckCondition(IPeripheralCommandParser parser, string portName, string portSettings, int timeout)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            IPort port = null;

            try
            {
                result = CommunicationResult.ErrorOpenPort;

                port = Factory.I.GetPort(portName, portSettings, timeout);

                result = ParseDoNotCheckCondition(parser, port);
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

            return result;
        }

        public static CommunicationResult GetSerialNumber(ref string serialNumber, string portName, string portSettings, int timeout)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            IPort port = null;

            try
            {
                result = CommunicationResult.ErrorOpenPort;

                port = Factory.I.GetPort(portName, portSettings, timeout);

                result = GetSerialNumber(ref serialNumber, port);
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

            return result;
        }

        public static void SendCommandsWithProgressBarInternal(byte[] commands, IPort port, bool checkCondition, bool showResult)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {
                if (checkCondition)
                {
                    result = SendCommands(commands, port);
                }
                else
                {
                    result = SendCommandsDoNotCheckCondition(commands, port);
                }

            });

            progressBarWindow.ShowDialog();

            if (!showResult &&
               result == CommunicationResult.Success)
            {
                return;
            }

            ShowCommunicationResultMessage(result);
        }

        public static void SendCommandsWithProgressBarInternal(byte[] commands, string portName, string portSettings, int timeout, bool checkCondition, bool showResult)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {
                if (checkCondition)
                {
                    result = SendCommands(commands, portName, portSettings, timeout);
                }
                else
                {
                    result = SendCommandsDoNotCheckCondition(commands, portName, portSettings, timeout);
                }

            });

            progressBarWindow.ShowDialog();

            if (!showResult &&
               result == CommunicationResult.Success)
            {
                return;
            }

            ShowCommunicationResultMessage(result);
        }

        public static CommunicationResult ParseDoNotCheckConditionWithProgressBar(IPeripheralCommandParser parser, IPort port)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {

                result = ParseDoNotCheckCondition(parser, port);

            });

            progressBarWindow.ShowDialog();

            return result;
        }

        public static CommunicationResult ParseDoNotCheckConditionWithProgressBar(IPeripheralCommandParser parser, string portName, string portSettings, int timeout)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            IPort port = null;

            try
            {
                result = CommunicationResult.ErrorOpenPort;

                port = Factory.I.GetPort(portName, portSettings, timeout);

                result = ParseDoNotCheckConditionWithProgressBar(parser, port);
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

            return result;
        }

        public static CommunicationResult GetSerialNumberWithProgressBar(ref string serialNumber, IPort port)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            string temp = "";

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {
                result = GetSerialNumber(ref temp, port);

            });

            progressBarWindow.ShowDialog();

            serialNumber = temp;

            return result;
        }

        public static CommunicationResult GetSerialNumberWithProgressBar(ref string serialNumber, string portName, string portSettings, int timeout)
        {
            CommunicationResult result = CommunicationResult.ErrorUnknown;

            string temp = "";

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {
                result = GetSerialNumber(ref temp, portName, portSettings, timeout);

            });

            progressBarWindow.ShowDialog();

            serialNumber = temp;

            return result;
        }

        public static void SendCommandsWithProgressBar(byte[] commands, string portName, string portSettings, int timeout)
        {
            SendCommandsWithProgressBarInternal(commands, portName, portSettings, timeout, true, true);
        }

        public static void SendCommandsDoNotCheckConditionWithProgressBar(byte[] commands, string portName, string portSettings, int timeout)
        {
            SendCommandsWithProgressBarInternal(commands, portName, portSettings, timeout, false, true);
        }

        public static void SendCommandsWithProgressBar(byte[] commands, IPort port)
        {
            SendCommandsWithProgressBarInternal(commands, port, true, true);
        }

        public static void SendCommandsWithProgressBar(byte[] commands, IPort port, bool showResult)
        {
            SendCommandsWithProgressBarInternal(commands, port, true, showResult);
        }

        public static void SendCommandsDoNotCheckConditionWithProgressBar(byte[] commands, IPort port)
        {
            SendCommandsWithProgressBarInternal(commands, port, false, true);
        }

        public static void SendCommandsDoNotCheckConditionWithProgressBar(byte[] commands, IPort port, bool showResult)
        {
            SendCommandsWithProgressBarInternal(commands, port, false, showResult);
        }

        public static void ShowCommunicationResultMessage(CommunicationResult result)
        {
            string resultMessage = GetCommunicationResultMessage(result);

            Util.ShowMessage("Communication Result", resultMessage);
        }

        public static void ShowPeripheralStatusResultMessage(string peripheralName, PeripheralStatus status)
        {
            string resultMessage = GetPeripheralStatusResultMessage(status);

            Util.ShowMessage("Check Status", peripheralName + " " + resultMessage);
        }

        public static string GetCommunicationResultMessage(CommunicationResult result)
        {
            string message;

            switch (result)
            {
                case CommunicationResult.Success:
                    message = "Success!";
                    break;
                case CommunicationResult.ErrorOpenPort:
                    message = "Fail to openPort";
                    break;
                case CommunicationResult.ErrorBeginCheckedBlock:
                    message = "Printer is offline (beginCheckedBlock)";
                    break;
                case CommunicationResult.ErrorEndCheckedBlock:
                    message = "Printer is offline (endCheckedBlock)";
                    break;
                case CommunicationResult.ErrorReadPort:
                    message = "Read port error (readPort)";
                    break;
                case CommunicationResult.ErrorWritePort:
                    message = "Write port error (writePort)";
                    break;
                default:
                case CommunicationResult.ErrorUnknown
:
                    message = "Unknown error";
                    break;
            }

            return message;
        }

        public static string GetPeripheralStatusResultMessage(PeripheralStatus status)
        {
            string message;

            switch (status)
            {
                default:
                case PeripheralStatus.Impossible:
                    message = "Impossible";
                    break;

                case PeripheralStatus.Connect:
                    message = "Connect";
                    break;

                case PeripheralStatus.Disconnect:
                    message = "Disconnect";
                    break;

            }

            return message;
        }
    }
}
