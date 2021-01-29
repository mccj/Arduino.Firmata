using Arduino.Firmata.Protocol.Firmata;
using System;
using System.Linq;

namespace Arduino.Firmata.Protocol.AccelStepper
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
            var messageTypeByte = (byte)messageHeader.MessageBuffer[1];
            var messageSubTypeByte = (byte)messageHeader.MessageBuffer[2];
            if (messageTypeByte == AccelStepperProtocol.ACCELSTEPPER_DATA && messageSubTypeByte == 0x06)
                return CreateStepperPositionResponse(messageHeader);//步进报告位置
            else if (messageTypeByte == AccelStepperProtocol.ACCELSTEPPER_DATA && messageSubTypeByte == 0x0a)
                return CreateStepperMoveCompleteResponse(messageHeader);//步进移动完成
            else if (messageTypeByte == AccelStepperProtocol.ACCELSTEPPER_DATA && messageSubTypeByte == 0x24)
                return CreateMultiStepperMoveCompelteResponse(messageHeader);//批量步进移动完成
            else
                throw new NotImplementedException();
        }
        private StepperPosition GetStepperPosition(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var stepsNum = NumberExtensions.decode32BitSignedInteger((byte)messageHeader.MessageBuffer[4], (byte)messageHeader.MessageBuffer[5], (byte)messageHeader.MessageBuffer[6], (byte)messageHeader.MessageBuffer[7], (byte)messageHeader.MessageBuffer[8]);

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
            var groupNumber = messageHeader.MessageBuffer[3];

            var currentState = groupNumber;
            messageHeader._arduinoSession.EvintAccelStepper().OnMultiStepperMoveCompelteReceived(new FirmataEventArgs<int>(currentState));
            return new FirmataMessage<int>(currentState);
        }
    }
}
