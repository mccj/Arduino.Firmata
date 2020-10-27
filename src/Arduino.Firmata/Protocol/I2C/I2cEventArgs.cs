using System;

namespace Arduino.Firmata.Protocol.I2C
{
    /// <summary>
    /// Event arguments passed to a <see cref="I2CReplyReceivedHandler"/> type event.
    /// </summary>
    public class I2CEventArgs : EventArgs
    {
        internal I2CEventArgs(I2CReply value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the I2C message value being received.
        /// </summary>
        public I2CReply Value { get; private set; }

    }
}
