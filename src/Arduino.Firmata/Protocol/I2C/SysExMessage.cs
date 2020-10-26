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
    public class I2CSysExMessage : ISysExMessage
    {
        public bool CanHeader(byte messageByte)
        {
            var ss = new[] {
                0x77
            };
            return ss.Contains(messageByte);
        }

        public IFirmataMessage Header(MessageHeader messageHeader)
        {
            var messageByte = (byte)messageHeader._messageBuffer[1];
            switch (messageByte)
            {
                case 0x77:
                    return CreateI2CReply(messageHeader);
                default:
                    throw new NotImplementedException();
            }
        }



        private IFirmataMessage CreateI2CReply(MessageHeader messageHeader)
        {
            var reply = new I2CReply
            {
                Address = messageHeader._messageBuffer[2] | (messageHeader._messageBuffer[3] << 7),
                Register = messageHeader._messageBuffer[4] | (messageHeader._messageBuffer[5] << 7)
            };

            var data = new byte[(messageHeader._messageBufferIndex - 5) / 2];

            for (int x = 0; x < data.Length; x++)
            {
                data[x] = (byte)(messageHeader._messageBuffer[x * 2 + 6] | messageHeader._messageBuffer[x * 2 + 7] << 7);
            }

            reply.Data = data;

            //if (I2CReplyReceived != null)
            //    I2CReplyReceived(this, new I2CEventArgs(reply));
            messageHeader._arduinoSession.EvintI2C().OnI2CReplyReceived(new I2CEventArgs(reply));

            return new FirmataMessage<I2CReply>(reply);
        }
    }
}
