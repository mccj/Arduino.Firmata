using Arduino.Firmata.Protocol.Firmata;
using System;
using System.Linq;

namespace Arduino.Firmata.Protocol.EEPROM
{
    /// <summary>
    /// Defines a serial port connection.
    /// </summary>
    /// <seealso href="http://arduino.cc/en/Reference/Serial">Serial reference for Arduino</seealso>
    public class EEPROMSysExMessage : ISysExMessage
    {
        public bool CanHeader(byte messageByte)
        {
            var ss = new[] {
                EEPROMProtocol.EEPROM_DATA
            };
            return ss.Contains(messageByte);
        }

        public IFirmataMessage Header(MessageHeader messageHeader)
        {
            var messageByte = (byte)messageHeader.MessageBuffer[1];
            var messageByte2 = (byte)messageHeader.MessageBuffer[2];
            if (messageByte == EEPROMProtocol.EEPROM_DATA && messageByte2 == EEPROMProtocol.EEPROM_LENGTH)
                return HeaderLengthMessage(messageHeader);
            else if (messageByte == EEPROMProtocol.EEPROM_DATA && messageByte2 == EEPROMProtocol.EEPROM_READ)
                return HeaderReadMessage(messageHeader);
            else if (messageByte == EEPROMProtocol.EEPROM_DATA && messageByte2 == EEPROMProtocol.EEPROM_GET)
                return HeaderGetMessage(messageHeader);
            else
                throw new NotImplementedException();
        }
        private IFirmataMessage HeaderLengthMessage(MessageHeader messageHeader)
        {
            var value = (int)NumberExtensions.decode32BitSignedInteger((byte)messageHeader.MessageBuffer[3], (byte)messageHeader.MessageBuffer[4], (byte)messageHeader.MessageBuffer[5], (byte)messageHeader.MessageBuffer[6], (byte)messageHeader.MessageBuffer[7]);

            var currentState = new EEPROM_LengthResponse
            {
                Value = value
            };
            return new FirmataMessage<EEPROM_LengthResponse>(currentState);
        }
        private IFirmataMessage HeaderReadMessage(MessageHeader messageHeader)
        {
            var value = NumberExtensions.decode8BitSignedByte((byte)messageHeader.MessageBuffer[3], (byte)messageHeader.MessageBuffer[4]);

            var currentState = new EEPROM_ReadResponse
            {
                Value = value
            };
            return new FirmataMessage<EEPROM_ReadResponse>(currentState);
        }
        private IFirmataMessage HeaderGetMessage(MessageHeader messageHeader)
        {
            var value7Bit = messageHeader.MessageBuffer.Skip(3).Take(messageHeader.MessageBufferIndex - 3).Select(f => (byte)f).ToArray();

            var value = ByteArrayExtensions.Decode7Bit(value7Bit);
            var currentState = new EEPROM_BytesResponse
            {
                Value = value
            };
            return new FirmataMessage<EEPROM_BytesResponse>(currentState);
        }
    }
}
