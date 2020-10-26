using Arduino.Firmata;
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
    /// Signature of event handlers capable of processing analog I/O messages.
    /// </summary>
    /// <param name="sender">The object raising the event</param>
    /// <param name="eventArgs">Event arguments holding a <see cref="AnalogState"/></param>
    public delegate void AnalogStateReceivedHandler(object sender, FirmataEventArgs<AnalogState> eventArgs);

    /// <summary>
    /// Signature of event handlers capable of processing digital I/O messages.
    /// </summary>
    /// <param name="sender">The object raising the event</param>
    /// <param name="eventArgs">Event arguments holding a <see cref="DigitalPortState"/></param>
    public delegate void DigitalStateReceivedHandler(object sender, FirmataEventArgs<DigitalPortState> eventArgs);

    public class FirmataEvint
    {
        private ArduinoSession session;

        public FirmataEvint(ArduinoSession session)
        {
            this.session = session;
        }

        /// <inheritdoc cref="IFirmataProtocol.AnalogStateReceived"/>
        public event AnalogStateReceivedHandler AnalogStateReceived;
        /// <inheritdoc cref="IFirmataProtocol.DigitalStateReceived"/>
        public event DigitalStateReceivedHandler DigitalStateReceived;
        internal void OnAnalogStateReceived(FirmataEventArgs<AnalogState> eventArgs)
        {
            AnalogStateReceived?.Invoke(session, eventArgs);
        }
        internal void OnDigitalStateReceived(FirmataEventArgs<DigitalPortState> eventArgs)
        {
            DigitalStateReceived?.Invoke(session, eventArgs);
        }

    }

    /// <summary>
    /// Contains event data for a <see cref="AnalogStateReceivedHandler"/> and <see cref="DigitalStateReceivedHandler"/> type events.
    /// </summary>
    /// <typeparam name="T">Type of the event data</typeparam>
    /// <remarks>
    /// This class is primarily implemented by the <see cref="IFirmataProtocol.AnalogStateReceived"/> and <see cref="IFirmataProtocol.DigitalStateReceived"/> events.
    /// </remarks>
    /// <seealso cref="AnalogStateReceivedHandler"/>
    /// <seealso cref="DigitalStateReceivedHandler"/>
    public class FirmataEventArgs<T> : EventArgs
        where T : struct
    {
        internal FirmataEventArgs(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the received message.
        /// </summary>
        public T Value { get; private set; }
    }
}
