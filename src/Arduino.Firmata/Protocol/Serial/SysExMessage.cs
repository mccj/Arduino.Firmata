using System;
using System.Linq;

namespace Arduino.Firmata.Protocol.Serial
{
    /// <summary>
    /// Defines a serial port connection.
    /// </summary>
    /// <seealso href="http://arduino.cc/en/Reference/Serial">Serial reference for Arduino</seealso>
    public class SerialSysExMessage : ISysExMessage
    {
        public bool CanHeader(byte messageByte)
        {
            var ss = new[] {
                0x60
            };
            return ss.Contains(messageByte);
        }

        public IFirmataMessage Header(MessageHeader messageHeader)
        {
            var messageByte = (byte)messageHeader.MessageBuffer[1];
            switch (messageByte)
            {
                case 0x60:
                    return CreateSerialReply(messageHeader);
                default:
                    throw new NotImplementedException();
            }
        }



        private IFirmataMessage CreateSerialReply(MessageHeader messageHeader)
        {
            var serial = (HW_SERIAL)(messageHeader.MessageBuffer[2] & 0b_111111);
            var data = new byte[(messageHeader.MessageBufferIndex - 3) / 2];

            for (int x = 0; x < data.Length; x++)
            {
                data[x] = (byte)(messageHeader.MessageBuffer[x + 3] | (messageHeader.MessageBuffer[x + 4] << 7));
            }

            //var builder = new System.Text.StringBuilder(messageHeader.MessageBufferIndex);
            //for (int x = 3; x < messageHeader.MessageBufferIndex; x += 2)
            //{
            //    builder.Append((char)(messageHeader.MessageBuffer[x] | (messageHeader.MessageBuffer[x + 1] << 7)));
            //}
            //reply.Data = data;
            var reply = new SerialReply
            {
                Serial = serial,
                Data = data
            };


            //if (I2CReplyReceived != null)
            //    I2CReplyReceived(this, new I2CEventArgs(reply));
            messageHeader._arduinoSession.EvintSerial().OnSerialReplyReceived(new SerialEventArgs(reply));

            return new FirmataMessage<SerialReply>(reply);
        }
    }
}
