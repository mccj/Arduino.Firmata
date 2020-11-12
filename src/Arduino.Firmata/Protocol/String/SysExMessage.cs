using System;
using System.Linq;
using System.Text;

namespace Arduino.Firmata.Protocol.String
{
    /// <summary>
    /// Defines a serial port connection.
    /// </summary>
    /// <seealso href="http://arduino.cc/en/Reference/Serial">Serial reference for Arduino</seealso>
    public class StringSysExMessage : ISysExMessage
    {
        public bool CanHeader(byte messageByte)
        {
            var ss = new[] {
                0x71
            };
            return ss.Contains(messageByte);
        }

        public IFirmataMessage Header(MessageHeader messageHeader)
        {
            var messageByte = (byte)messageHeader.MessageBuffer[1];
            switch (messageByte)
            {
                case 0x71:
                    return CreateStringDataMessage(messageHeader);
                default:
                    throw new NotImplementedException();
            }
        }



        private IFirmataMessage CreateStringDataMessage(MessageHeader messageHeader)
        {
            var builder = new StringBuilder(messageHeader.MessageBufferIndex >> 1);

            for (int x = 2; x < messageHeader.MessageBufferIndex; x += 2)
            {
                builder.Append((char)(messageHeader.MessageBuffer[x] | (messageHeader.MessageBuffer[x + 1] << 7)));
            }

            var stringData = new StringData
            {
                Text = builder.ToString()
            };

            return new FirmataMessage<StringData>(stringData);
        }
    }
}
