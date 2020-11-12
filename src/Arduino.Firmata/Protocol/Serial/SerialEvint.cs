namespace Arduino.Firmata.Protocol.Serial
{
    /// <summary>
    /// Signature of event handlers capable of processing Serial_REPLY messages.
    /// </summary>
    /// <param name="sender">The object raising the event</param>
    /// <param name="eventArgs">Event arguments holding an <see cref="SerialReply"/></param>
    public delegate void SerialReplyReceivedHandler(object sender, SerialEventArgs eventArgs);

    public class SerialEvint
    {
        private ArduinoSession session;

        public SerialEvint(ArduinoSession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Event, raised for every SYSEX Serial message not handled by an <see cref="ISerialProtocol"/>'s Get method.
        /// </summary>
        /// <remarks>
        /// When e.g. methods <see cref="ReadSerialOnce(int,int)"/> and <see cref="ReadSerialContinuous(int,int)"/> are invoked,
        /// the party system's response messages raise this event.
        /// However, when method <see cref="GetSerialReply(int,int)"/> or <see cref="GetSerialReplyAsync(int,int)"/> is invoked,
        /// the response received is returned to the method that issued the command and event <see cref="SerialReplyReceived"/> is not raised.
        /// </remarks>
        public event SerialReplyReceivedHandler SerialReplyReceived;
        internal void OnSerialReplyReceived(SerialEventArgs eventArgs)
        {
            SerialReplyReceived?.Invoke(session, eventArgs);
        }
    }
}
