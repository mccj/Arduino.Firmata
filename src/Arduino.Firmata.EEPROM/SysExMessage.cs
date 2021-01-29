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
            var messageTypeByte = (byte)messageHeader.MessageBuffer[1];
            var messageSubTypeByte = (byte)messageHeader.MessageBuffer[2];
            if (messageTypeByte == EEPROMProtocol.EEPROM_DATA && messageSubTypeByte == EEPROMProtocol.EEPROM_LENGTH)
                return HeaderReturnInt(messageHeader);
            else if (messageTypeByte == EEPROMProtocol.EEPROM_DATA && messageSubTypeByte == EEPROMProtocol.EEPROM_READ)
                return HeaderReturnByte(messageHeader);
            else if (messageTypeByte == EEPROMProtocol.EEPROM_DATA && messageSubTypeByte == EEPROMProtocol.EEPROM_GET)
                return HeaderGetMessage(messageHeader);
            else if (messageTypeByte == EEPROMProtocol.EEPROM_DATA &&
                 (messageSubTypeByte == EEPROMProtocol.EEPROM_WRITE || messageSubTypeByte == EEPROMProtocol.EEPROM_UPDATE || messageSubTypeByte == EEPROMProtocol.EEPROM_PUT)
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
            };
            return new FirmataMessage<GenericResponse>(currentState);
        }
        private IFirmataMessage HeaderReturnInt(MessageHeader messageHeader)
        {
            var value = (int)NumberExtensions.decode32BitSignedInteger((byte)messageHeader.MessageBuffer[3], (byte)messageHeader.MessageBuffer[4], (byte)messageHeader.MessageBuffer[5], (byte)messageHeader.MessageBuffer[6], (byte)messageHeader.MessageBuffer[7]);

            var currentState = new GenericResponse<int>
            {
                MessageType = (byte)messageHeader.MessageBuffer[1],
                MessageSubType = (byte)messageHeader.MessageBuffer[2],
                Value = value
            };
            return new FirmataMessage<GenericResponse<int>>(currentState);
        }
        private IFirmataMessage HeaderReturnByte(MessageHeader messageHeader)
        {
            var value = NumberExtensions.decode8BitSignedByte((byte)messageHeader.MessageBuffer[3], (byte)messageHeader.MessageBuffer[4]);

            var currentState = new GenericResponse<byte>
            {
                MessageType = (byte)messageHeader.MessageBuffer[1],
                MessageSubType = (byte)messageHeader.MessageBuffer[2],
                Value = value
            };
            return new FirmataMessage<GenericResponse<byte>>(currentState);
        }
        private IFirmataMessage HeaderGetMessage(MessageHeader messageHeader)
        {
            var value7Bit = messageHeader.MessageBuffer.Skip(3).Take(messageHeader.MessageBufferIndex - 3).Select(f => (byte)f).ToArray();

            var value = ByteArrayExtensions.Decode7Bit(value7Bit);
            var currentState = new GenericResponse<byte[]>
            {
                MessageType = (byte)messageHeader.MessageBuffer[1],
                MessageSubType = (byte)messageHeader.MessageBuffer[2],
                Value = value
            };
            return new FirmataMessage<GenericResponse<byte[]>>(currentState);
        }
    }
}
