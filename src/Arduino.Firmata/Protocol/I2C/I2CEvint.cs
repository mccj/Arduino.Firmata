namespace Arduino.Firmata.Protocol.I2C
{
    /// <summary>
    /// Signature of event handlers capable of processing I2C_REPLY messages.
    /// </summary>
    /// <param name="sender">The object raising the event</param>
    /// <param name="eventArgs">Event arguments holding an <see cref="I2CReply"/></param>
    public delegate void I2CReplyReceivedHandler(object sender, I2CEventArgs eventArgs);

    public class I2CEvint
    {
        private ArduinoSession session;

        public I2CEvint(ArduinoSession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Event, raised for every SYSEX I2C message not handled by an <see cref="II2CProtocol"/>'s Get method.
        /// </summary>
        /// <remarks>
        /// When e.g. methods <see cref="ReadI2COnce(int,int)"/> and <see cref="ReadI2CContinuous(int,int)"/> are invoked,
        /// the party system's response messages raise this event.
        /// However, when method <see cref="GetI2CReply(int,int)"/> or <see cref="GetI2CReplyAsync(int,int)"/> is invoked,
        /// the response received is returned to the method that issued the command and event <see cref="I2CReplyReceived"/> is not raised.
        /// </remarks>
        public event I2CReplyReceivedHandler I2CReplyReceived;
        internal void OnI2CReplyReceived(I2CEventArgs eventArgs)
        {
            I2CReplyReceived?.Invoke(session, eventArgs);
        }
    }
}
