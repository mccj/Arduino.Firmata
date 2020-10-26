using Arduino.Firmata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Solid.Arduino.Firmata
{
    /// <summary>
    /// Defines a serial port connection.
    /// </summary>
    /// <seealso href="http://arduino.cc/en/Reference/Serial">Serial reference for Arduino</seealso>
    public class AccelStepperSysExMessage : ISysExMessage
    {
        public bool CanHeader(byte messageByte)
        {
            var ss = new[] {
                AccelStepperProtocol.ACCELSTEPPER_DATA
            };
            return ss.Contains(messageByte);
        }

        public IFirmataMessage Header(MessageHeader messageHeader)
        {
            var messageByte = (byte)messageHeader._messageBuffer[1];
            var messageByte2 = (byte)messageHeader._messageBuffer[2];
            if (messageByte == AccelStepperProtocol.ACCELSTEPPER_DATA && messageByte2 == 0x06)
                return CreateStepperPositionResponse(messageHeader);//步进报告位置
            else if (messageByte == AccelStepperProtocol.ACCELSTEPPER_DATA && messageByte2 == 0x0a)
                return CreateStepperMoveCompleteResponse(messageHeader);//步进移动完成
            else if (messageByte == AccelStepperProtocol.ACCELSTEPPER_DATA && messageByte2 == 0x24)
                return CreateMultiStepperMoveCompelteResponse(messageHeader);//批量步进移动完成
            else
                throw new NotImplementedException();
        }
        private StepperPosition GetStepperPosition(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader._messageBuffer[3];
            var stepsNum = AccelStepperProtocol.decode32BitSignedInteger((byte)messageHeader._messageBuffer[4], (byte)messageHeader._messageBuffer[5], (byte)messageHeader._messageBuffer[6], (byte)messageHeader._messageBuffer[7], (byte)messageHeader._messageBuffer[8]);

            var currentState = new StepperPosition
            {
                DeviceNumber = deviceNumber,
                StepsNum = stepsNum
            };
            return currentState;
        }
        /// <summary>
        /// 步进报告位置
        /// </summary>
        /// <param name="messageHeader"></param>
        /// <returns></returns>
        private IFirmataMessage CreateStepperPositionResponse(MessageHeader messageHeader)
        {
            var currentState = GetStepperPosition(messageHeader);
            messageHeader._arduinoSession.EvintAccelStepper().OnStepperPositionReceived(new FirmataEventArgs<StepperPosition>(currentState));
            return new FirmataMessage<StepperPosition>(currentState);
        }
        private IFirmataMessage CreateStepperMoveCompleteResponse(MessageHeader messageHeader)
        {
            var currentState = GetStepperPosition(messageHeader);
            messageHeader._arduinoSession.EvintAccelStepper().OnStepperMoveCompleteReceived(new FirmataEventArgs<StepperPosition>(currentState));
            return new FirmataMessage<StepperPosition>(currentState);
        }
        private IFirmataMessage CreateMultiStepperMoveCompelteResponse(MessageHeader messageHeader)
        {
            var groupNumber = messageHeader._messageBuffer[3];

            var currentState = groupNumber;
            messageHeader._arduinoSession.EvintAccelStepper().OnMultiStepperMoveCompelteReceived(new FirmataEventArgs<int>(currentState));
            return new FirmataMessage<int>(currentState);
        }
    }
}
