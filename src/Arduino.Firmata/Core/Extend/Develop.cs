using Solid.Arduino.Firmata;
using System;
using System.Threading.Tasks;
using static Solid.Arduino.Firmata.MessageHeader;

namespace Arduino.Firmata
{
    public static class Develop
    {
        public static void Write(this ArduinoSession session, byte[] buffer, int offset, int count)
        {
            //var _connection = typeof(ArduinoSession).GetField("_connection", Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Instance).GetValue(session) as ISerialConnection;
            session.Connection.Write(buffer, offset, count);
        }
        /// <inheritdoc cref="IStringProtocol.Write"/>
        public static void Write(this ArduinoSession session, string value)
        {
            if (!string.IsNullOrEmpty(value))
                session.Connection.Write(value);
        }

        /// <inheritdoc cref="IStringProtocol.WriteLine"/>
        public static void WriteLine(this ArduinoSession session, string value)
        {
            session.Connection.WriteLine(value);
        }
        /// <inheritdoc cref="IStringProtocol.ReadLine"/>
        public static string ReadLine(this ArduinoSession session)
        {
            return session.messageHeader. GetStringFromQueue(StringRequest.CreateReadLineRequest());
        }

        /// <inheritdoc cref="IStringProtocol.ReadLineAsync"/>
        public static async Task<string> ReadLineAsync(this ArduinoSession session)
        {
            return await Task.Run(() => session.messageHeader.GetStringFromQueue(StringRequest.CreateReadLineRequest()));
        }

        /// <inheritdoc cref="IStringProtocol.Read"/>
        public static string Read(this ArduinoSession session, int length = 1)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), Messages.ArgumentEx_PositiveValue);

            return session.messageHeader.GetStringFromQueue(StringRequest.CreateReadRequest(length));
        }
        
        /// <inheritdoc cref="IStringProtocol.ReadAsync"/>
        public static async Task<string> ReadAsync(this ArduinoSession session, int length = 1)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), Messages.ArgumentEx_PositiveValue);

            return await Task.Run(() => session.messageHeader.GetStringFromQueue(StringRequest.CreateReadRequest(length)));
        }

        /// <inheritdoc cref="IStringProtocol.ReadTo"/>
        public static string ReadTo(this ArduinoSession session, char terminator = char.MinValue)
        {
            return session.messageHeader.GetStringFromQueue(StringRequest.CreateReadRequest(terminator));
        }

        /// <inheritdoc cref="IStringProtocol.ReadToAsync"/>
        public static async Task<string> ReadToAsync(this ArduinoSession session, char terminator = char.MinValue)
        {
            return await Task.Run(() => session.messageHeader.GetStringFromQueue(StringRequest.CreateReadRequest(terminator)));
        }




        public static void SendSysExCommand(this ArduinoSession session, byte command)
        {
            var message = new[]
            {
                Utility.SysExStart,
                command,
                Utility.SysExEnd
            };

            session.Write(message);
        }


        /// <summary>
        /// Sends a SysEx message.
        /// </summary>
        /// <param name="message"></param>
        public static void SendSysEx(this ArduinoSession session, byte command, byte[] payload)
        {
            if (payload == null || payload.Length == 0)
            {
                session.SendSysExCommand(command);
                return;
            }

            var message = new byte[3 + payload.Length];
            message[0] = Utility.SysExStart;
            message[1] = command;
            Array.Copy(payload, 0, message, 2, payload.Length);
            message[message.Length - 1] = Utility.SysExEnd;

            session.Write(message);
        }
       



        public static void SendCommand(this ArduinoSession session, byte command)
        {
            session.Write(new byte[] { command });
        }
        public static void Write(this ArduinoSession session, byte[] command)
        {
            session.Write(command, 0, command.Length);
        }



    }
}
