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
            var messageByte = (byte)messageHeader.MessageBuffer[1];
            var messageByte2 = (byte)messageHeader.MessageBuffer[2];
            if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_CAN_SHOW)
                return CanShow(messageHeader);
            else if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_GET_PIXELS)
                return GetPixels(messageHeader);
            else if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_GET_BRIGHTNESS)
                return GetBrightness(messageHeader);
            else if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_GET_PIN)
                return GetPin(messageHeader);
            else if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_NUM_PIXELS)
                return NumPixels(messageHeader);
            else if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_GET_PIXEL_COLOR)
                return GetPixelColor(messageHeader);
            else if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_SINE8)
                return GetSine8(messageHeader);
            else if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_GAMMA8)
                return GetGamma8(messageHeader);
            else if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_GAMMA32)
                return GetGamma32(messageHeader);
            else if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_COLOR)
                return Color(messageHeader);
            else if (messageByte == NeoPixelProtocol.NEOPIXEL_DATA && messageByte2 == NeoPixelProtocol.NEOPIXEL_REPORT_COLORHSV)
                return ColorHSV(messageHeader);
            else
                throw new NotImplementedException();
        }
        private IFirmataMessage CanShow(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var value = Convert.ToBoolean(messageHeader.MessageBuffer[4]);

            var currentState = new CanShow
            {
                DeviceNumber = deviceNumber,
                Value = value
            };
            return new FirmataMessage<CanShow>(currentState);
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
        private IFirmataMessage GetBrightness(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var value = (byte)messageHeader.MessageBuffer[4];

            var currentState = new Brightness
            {
                DeviceNumber = deviceNumber,
                Value = value
            };
            return new FirmataMessage<Brightness>(currentState);
        }
        private IFirmataMessage GetPin(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var value = (byte)messageHeader.MessageBuffer[4];

            var currentState = new Pin
            {
                DeviceNumber = deviceNumber,
                Value = value
            };
            return new FirmataMessage<Pin>(currentState);
        }
        private IFirmataMessage NumPixels(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var value = (int)NumberExtensions.decode32BitSignedInteger((byte)messageHeader.MessageBuffer[4], (byte)messageHeader.MessageBuffer[5], (byte)messageHeader.MessageBuffer[6], (byte)messageHeader.MessageBuffer[7], (byte)messageHeader.MessageBuffer[8]);

            var currentState = new NumPixels
            {
                DeviceNumber = deviceNumber,
                Value = value
            };
            return new FirmataMessage<NumPixels>(currentState);
        }

        private IFirmataMessage GetPixelColor(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var value = (int)NumberExtensions.decode32BitSignedInteger((byte)messageHeader.MessageBuffer[4], (byte)messageHeader.MessageBuffer[5], (byte)messageHeader.MessageBuffer[6], (byte)messageHeader.MessageBuffer[7], (byte)messageHeader.MessageBuffer[8]);

            var currentState = new GetPixelColor
            {
                DeviceNumber = deviceNumber,
                Value = value
            };
            return new FirmataMessage<GetPixelColor>(currentState);
        }

        private IFirmataMessage GetSine8(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var value = (byte)messageHeader.MessageBuffer[4];

            var currentState = new Sine8
            {
                DeviceNumber = deviceNumber,
                Value = value
            };
            return new FirmataMessage<Sine8>(currentState);
        }
        private IFirmataMessage GetGamma8(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var value = (byte)messageHeader.MessageBuffer[4];

            var currentState = new Gamma8
            {
                DeviceNumber = deviceNumber,
                Value = value
            };
            return new FirmataMessage<Gamma8>(currentState);
        }
        private IFirmataMessage GetGamma32(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var value = (int)NumberExtensions.decode32BitSignedInteger((byte)messageHeader.MessageBuffer[4], (byte)messageHeader.MessageBuffer[5], (byte)messageHeader.MessageBuffer[6], (byte)messageHeader.MessageBuffer[7], (byte)messageHeader.MessageBuffer[8]);

            var currentState = new Gamma32
            {
                DeviceNumber = deviceNumber,
                Value = value
            };
            return new FirmataMessage<Gamma32>(currentState);
        }
        private IFirmataMessage Color(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var value = (int)NumberExtensions.decode32BitSignedInteger((byte)messageHeader.MessageBuffer[4], (byte)messageHeader.MessageBuffer[5], (byte)messageHeader.MessageBuffer[6], (byte)messageHeader.MessageBuffer[7], (byte)messageHeader.MessageBuffer[8]);

            var currentState = new PixelColor
            {
                DeviceNumber = deviceNumber,
                Value = value
            };
            return new FirmataMessage<PixelColor>(currentState);
        }
        private IFirmataMessage ColorHSV(MessageHeader messageHeader)
        {
            var deviceNumber = messageHeader.MessageBuffer[3];
            var value = (int)NumberExtensions.decode32BitSignedInteger((byte)messageHeader.MessageBuffer[4], (byte)messageHeader.MessageBuffer[5], (byte)messageHeader.MessageBuffer[6], (byte)messageHeader.MessageBuffer[7], (byte)messageHeader.MessageBuffer[8]);

            var currentState = new ColorHSV
            {
                DeviceNumber = deviceNumber,
                Value = value
            };
            return new FirmataMessage<ColorHSV>(currentState);
        }
    }
}
