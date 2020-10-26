using System;

namespace Solid.Arduino.Firmata
{
    /// <summary>
    /// Event arguments passed to a <see cref="MessageReceivedHandler"/> type event.
    /// </summary>
    /// <see cref="MessageReceivedHandler"/>
    public class FirmataMessageEventArgs : EventArgs
    {
        internal FirmataMessageEventArgs(IFirmataMessage value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the received message.
        /// </summary>
        public IFirmataMessage Value { get; }
    }
}
