using Arduino.Firmata;
using Solid.Arduino.Firmata;
using System;
using System.Threading.Tasks;
using static Solid.Arduino.Firmata.MessageHeader;

namespace Solid.Arduino
{
    /// <summary>
    /// Defines members for sending and receiving ASCII string messages.
    /// </summary>
    public static class StringProtocol
    {
        /// <summary>
        /// Creates an observable object tracking received ASCII <see cref="System.String"/> messages.
        /// </summary>
        /// <returns>An <see cref="IObservable{String}"/> interface</returns>
        public static IObservable<string> CreateReceivedStringMonitor(this ArduinoSession session)
        {
            return new ReceivedStringTracker(session.EvintString());
        }

        /// <summary>
        /// Gets or sets the value used to interpret the end of strings received and sent.
        /// </summary>
        public static string NewLine(this ArduinoSession session) => session.Connection.NewLine;
        public static string NewLine(this ArduinoSession session, string value) => session.Connection.NewLine = value;

        /// <summary>
        /// Writes a string to the serial output data stream.
        /// </summary>
        /// <param name="value">A string to be written</param>
        public static void Write(this ArduinoSession session, string value = null)
        {
            if (!string.IsNullOrEmpty(value))
                session.Connection.Write(value);
        }

        /// <summary>
        /// Writes the specified string and the <see cref="SerialPort.NewLine"/> value to the serial output stream.
        /// </summary>
        /// <param name="value">The string to write</param>
        public static void WriteLine(this ArduinoSession session, string value = null)
        {
            session.Connection.WriteLine(value);
        }

        /// <summary>
        /// Reads a string up to the next <see cref="NewLine"/> character.
        /// </summary>
        /// <returns>The string read</returns>
        public static string ReadLine(this ArduinoSession session)
        {
            return session.messageHeader.GetStringFromQueue(StringRequest.CreateReadLineRequest());
        }

        /// <summary>
        /// Reads a string asynchronous up to the next <see cref="NewLine"/> character.
        /// </summary>
        /// <returns>An awaitable <see cref="Task{String}"/> returning the string read</returns>
        public static async Task<string> ReadLineAsync(this ArduinoSession session)
        {
            return await Task.Run(() => session.messageHeader.GetStringFromQueue(StringRequest.CreateReadLineRequest()));
        }

        /// <summary>
        /// Reads a specified number of characters.
        /// </summary>
        /// <param name="length">The number of characters to be read (default is 1)</param>
        /// <returns>The string read</returns>
        public static string Read(this ArduinoSession session, int length = 1)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), Messages.ArgumentEx_PositiveValue);

            return session.messageHeader.GetStringFromQueue(StringRequest.CreateReadRequest(length));
        }
        /// <summary>
        /// Reads a specified number of characters asynchronous.
        /// </summary>
        /// <param name="length">The number of characters to be read (default is 1)</param>
        /// <returns>An awaitable <see cref="Task{String}"/> returning the string read</returns>
        public static async Task<string> ReadAsync(this ArduinoSession session, int length = 1)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), Messages.ArgumentEx_PositiveValue);

            return await Task.Run(() => session.messageHeader.GetStringFromQueue(StringRequest.CreateReadRequest(length)));
        }

        /// <summary>
        /// Reads a string up to the first terminating character.
        /// </summary>
        /// <param name="terminator">The character identifying the end of the string</param>
        /// <returns>The string read</returns>
        public static string ReadTo(this ArduinoSession session, char terminator)
        {
            return session.messageHeader.GetStringFromQueue(StringRequest.CreateReadRequest(terminator));
        }

        /// <summary>
        /// Reads a string asynchronous up to the first terminating character.
        /// </summary>
        /// <param name="terminator">The character identifying the end of the string</param>
        /// <returns>An awaitable <see cref="Task{String}"/> returning the string read</returns>
        public static async Task<string> ReadToAsync(this ArduinoSession session, char terminator)
        {
            return await Task.Run(() => session.messageHeader.GetStringFromQueue(StringRequest.CreateReadRequest(terminator)));
        }


        public static StringEvint EvintString(this ArduinoSession session)
        {
            return session.GetEvint(() => new StringEvint(session));
        }

    }
}
