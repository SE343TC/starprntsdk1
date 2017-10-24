using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;
using System;
using System.Threading;

namespace StarPRNTSDK
{
    public static class ScaleCommunication
    {
        /// <summary>
        /// Sample : Parse printer response for getting scale displayed weight.
        /// </summary>
        public static Communication.CommunicationResult ParseDoNotCheckCondition(IPeripheralCommandParser parser, IPort port)
        {
            byte[] requestWeightToScaleCommand = parser.SendCommands;
            byte[] receiveWeightFromPrinterCommand = parser.ReceiveCommands;

            Communication.CommunicationResult result = Communication.CommunicationResult.ErrorUnknown;

            try
            {
                uint startDate = (uint)Environment.TickCount;

                byte[] readBuffer = new byte[1024];
                uint totalReceiveSize = 0;
                int requestCount = 0;

                while (true)
                {
                    if ((UInt32)Environment.TickCount - startDate >= 1000) // Timeout
                    {
                        throw new PortException("ReadPort timeout.");
                    }

                    result = Communication.CommunicationResult.ErrorWritePort;

                    if ((UInt32)Environment.TickCount - startDate >= 250 * requestCount) // Send request displayed weight commands every 250ms.
                    {
                        port.WritePort(requestWeightToScaleCommand, 0, (uint)requestWeightToScaleCommand.Length);

                        requestCount++;

                        Thread.Sleep(100);
                    }

                    // Send read weight from printer commands.
                    port.WritePort(receiveWeightFromPrinterCommand, 0, (uint)receiveWeightFromPrinterCommand.Length);

                    Thread.Sleep(100);

                    result = Communication.CommunicationResult.ErrorReadPort;

                    uint receiveSize = port.ReadPort(ref readBuffer, totalReceiveSize, (uint)(readBuffer.Length - totalReceiveSize));

                    if (receiveSize > 0)
                    {
                        totalReceiveSize += receiveSize;
                    }

                    byte[] receiveData = new byte[totalReceiveSize];

                    Array.Copy(readBuffer, 0, receiveData, 0, totalReceiveSize);

                    if (parser.Parse(receiveData, (int)totalReceiveSize) == ParseResult.Success)
                    {
                        result = Communication.CommunicationResult.Success;

                        break;
                    }
                }
            }
            catch (PortException)
            {

            }

            return result;
        }

        public static Communication.CommunicationResult ParseDoNotCheckCondition(IPeripheralCommandParser parser, string portName, string portSettings, int timeout)
        {
            Communication.CommunicationResult result = Communication.CommunicationResult.ErrorUnknown;

            IPort port = null;

            try
            {
                result = Communication.CommunicationResult.ErrorOpenPort;

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

        public static Communication.CommunicationResult ParseDoNotCheckConditionWithProgressBar(IPeripheralCommandParser parser, IPort port)
        {
            Communication.CommunicationResult result = Communication.CommunicationResult.ErrorUnknown;

            ProgressBarWindow progressBarWindow = new ProgressBarWindow("Communicating...", () =>
            {

                result = ParseDoNotCheckCondition(parser, port);

            });

            progressBarWindow.ShowDialog();

            return result;
        }

        public static Communication.CommunicationResult ParseDoNotCheckConditionWithProgressBar(IPeripheralCommandParser parser, string portName, string portSettings, int timeout)
        {
            Communication.CommunicationResult result = Communication.CommunicationResult.ErrorUnknown;

            IPort port = null;

            try
            {
                result = Communication.CommunicationResult.ErrorOpenPort;

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
    }
}
