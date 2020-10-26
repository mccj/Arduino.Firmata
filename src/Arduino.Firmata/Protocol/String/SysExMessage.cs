using Arduino.Firmata;
using Solid.Arduino.I2C;
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
            var messageByte = (byte)messageHeader._messageBuffer[1];
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
            var builder = new StringBuilder(messageHeader._messageBufferIndex >> 1);

            for (int x = 2; x < messageHeader._messageBufferIndex; x += 2)
            {
                builder.Append((char)(messageHeader._messageBuffer[x] | (messageHeader._messageBuffer[x + 1] << 7)));
            }

            var stringData = new StringData
            {
                Text = builder.ToString()
            };

            return new FirmataMessage<StringData>(stringData);
        }
    }
}
