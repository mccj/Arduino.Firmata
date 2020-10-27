using System.Linq;

namespace Arduino.Firmata.Protocol.String
{
    /// <summary>
    /// Signature of event handlers capable of processing received strings.
    /// </summary>
    /// <param name="sender">The object raising the event</param>
    /// <param name="eventArgs">Event arguments holding a <see cref="string"/> message</param>
    public delegate void StringReceivedHandler(object sender, StringEventArgs eventArgs);

    public class StringEvint
    {
        private ArduinoSession session;

        public StringEvint(ArduinoSession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Event, raised for every ASCII stringmessage not handled by an <see cref="StringProtocol"/>'s
        /// Read, ReadAsync, ReadLine, ReadLineAsync, ReadTo or ReadToAsync method
        /// </summary>
        /// <remarks>
        /// Any spontaneous received string message, terminated with a newline or eof character raises this event.
        /// </remarks>
        public event StringReceivedHandler StringReceived;
        internal void OnStringReceived(StringEventArgs eventArgs)
        {
            StringReceived?.Invoke(session, eventArgs);
        }
    }
}
