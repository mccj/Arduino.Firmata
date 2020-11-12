using System;

namespace Arduino.Firmata.Protocol.Serial
{
    /// <summary>
    /// Event arguments passed to a <see cref="I2CReplyReceivedHandler"/> type event.
    /// </summary>
    public class SerialEventArgs : EventArgs
    {
        internal SerialEventArgs(SerialReply value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the I2C message value being received.
        /// </summary>
        public SerialReply Value { get; private set; }

    }
}
