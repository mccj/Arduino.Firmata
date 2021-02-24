using Arduino.Firmata.Protocol.Firmata;
using System;
using System.Linq;

namespace Arduino.Firmata.Protocol.NeoPixel
{
    /// <summary>
    /// Defines a serial port connection.
    /// </summary>
    /// <seealso href="http://arduino.cc/en/Reference/Serial">Serial reference for Arduino</seealso>
    public class NeoPixelSysExMessage : ISysExMessage
    {
        public bool CanHeader(byte messageByte)
        {
            var ss = new[] {
                NeoPixelProtocol.NEOPIXEL_DATA
            };
            return ss.Contains(messageByte);
        }

        public IFirmataMessage Header(MessageHeader messageHeader)
        {
            var messageTypeByte = (byte)messageHeader.MessageBuffer[1];
            var messageSubTypeByte = (byte)messageHeader.MessageBuffer[2];
            if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_CAN_SHOW)
                return HeaderReturnBool(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_GET_PIXELS)
                return GetPixels(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_GET_BRIGHTNESS)
                return HeaderReturnByte(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_GET_PIN)
                return GetPin(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_NUM_PIXELS)
                return HeaderReturnInt(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_GET_PIXEL_COLOR)
                return HeaderReturnInt(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_SINE8)
                return HeaderReturnByte(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_GAMMA8)
                return HeaderReturnByte(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_GAMMA32)
                return HeaderReturnInt(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_COLOR)
                return HeaderReturnInt(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA && messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_REPORT_COLORHSV)
                return HeaderReturnInt(messageHeader);
            else if (messageTypeByte == NeoPixelProtocol.NEOPIXEL_DATA &&
             (messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_CONFIG || messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_BEGIN || messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_SHOW || messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_CLEAR
              || messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_SET_PIXEL_COLOR || messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_FILL || messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_UPDATE_LENGTH
              || messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_UPDATE_TYPE || messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_SET_PIN || messageSubTypeByte == NeoPixelProtocol.NEOPIXEL_SET_BRIGHTNESS)
             )
                return HeaderGenericVoidMessage(messageHeader);

            else
                throw new NotImplementedException();
        }
        private IFirmataMessage HeaderGenericVoidMessage(MessageHeader messageHeader)
        {
            var currentState = new GenericResponse
            {
                MessageType = (byte)messageHeader.MessageBuffer[1],
                MessageSubType = (byte)messageHeader.MessageBuffer[2],
                DeviceNumber = (byte)messageHeader.MessageBuffer[3],
            };
            return new FirmataMessage<GenericResponse>(currentState);
        }
        private IFirmataMessage HeaderReturnBool(MessageHeader messageHeader)
        {
            var value = Convert.ToBoolean(messageHeader.MessageBuffer[4]);

            var currentState = new GenericResponse<bool>
            {
                MessageType = (byte)messageHeader.MessageBuffer[1],
                MessageSubType = (byte)messageHeader.MessageBuffer[2],
                DeviceNumber = messageHeader.MessageBuffer[3],
                Value = value
            };
            return new FirmataMessage<GenericResponse<bool>>(currentState);
        }
        private IFirmataMessage HeaderReturnByte(MessageHeader messageHeader)
        {
            var value = NumberExtensions.decode8BitSignedByte((byte)messageHeader.MessageBuffer[4], (byte)messageHeader.MessageBuffer[5]);

            var currentState = new GenericResponse<byte>
            {
                MessageType = (byte)messageHeader.MessageBuffer[1],
                MessageSubType = (byte)messageHeader.MessageBuffer[2],
                DeviceNumber = messageHeader.MessageBuffer[3],
                Value = value
            };
            return new FirmataMessage<GenericResponse<byte>>(currentState);
        }
        private IFirmataMessage HeaderReturnInt(MessageHeader messageHeader)
        {
            var value = (int)NumberExtensions.decode32BitSignedInteger((byte)messageHeader.MessageBuffer[4], (byte)messageHeader.MessageBuffer[5], (byte)messageHeader.MessageBuffer[6], (byte)messageHeader.MessageBuffer[7], (byte)messageHeader.MessageBuffer[8]);

            var currentState = new GenericResponse<int>
            {
                MessageType = (byte)messageHeader.MessageBuffer[1],
                MessageSubType = (byte)messageHeader.MessageBuffer[2],
                DeviceNumber = messageHeader.MessageBuffer[3],
                Value = value
            };
            return new FirmataMessage<GenericResponse<int>>(currentState);
        }
        private IFirmataMessage GetPin(MessageHeader messageHeader)
        {
            var value = (byte)messageHeader.MessageBuffer[4];

            var currentState = new GenericResponse<byte>
            {
                MessageType = (byte)messageHeader.MessageBuffer[1],
                MessageSubType = (byte)messageHeader.MessageBuffer[2],
                DeviceNumber = messageHeader.MessageBuffer[3],
                Value = value
            };
            return new FirmataMessage<GenericResponse<byte>>(currentState);
        }
        private IFirmataMessage GetPixels(MessageHeader messageHeader)
        {
            //var deviceNumber = messageHeader.MessageBuffer[3];
            //var value = Convert.ToBoolean(messageHeader.MessageBuffer[4]);

            //var currentState = new CanShow
            //{
            //    DeviceNumber = deviceNumber,
            //    Value = value
            //};
            //return new FirmataMessage<CanShow>(currentState);
            throw new Exception("功能未实现");
        }
    }
}
