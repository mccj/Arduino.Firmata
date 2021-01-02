using System;

namespace Arduino.Firmata.Protocol.Firmata
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
    public delegate void DigitalStateReceivedChangeHandler(object sender, DigitalPinState eventArgs);

    public class FirmataEvint
    {
        private ArduinoSession session;
        private bool?[] digitalPortState = new bool?[256];
        private long?[] analogPortState = new long?[256];

        public FirmataEvint(ArduinoSession session)
        {
            this.session = session;
        }

        /// <inheritdoc cref="IFirmataProtocol.AnalogStateReceived"/>
        public event AnalogStateReceivedHandler AnalogStateReceived;
        public event AnalogStateReceivedHandler AnalogStateChangeReceived;
        /// <inheritdoc cref="IFirmataProtocol.DigitalStateReceived"/>
        public event DigitalStateReceivedHandler DigitalStateReceived;
        public event DigitalStateReceivedChangeHandler DigitalStateChangeReceived;
        internal void OnAnalogStateReceived(FirmataEventArgs<AnalogState> eventArgs)
        {
            AnalogStateReceived?.Invoke(session, eventArgs);

            var value = eventArgs.Value;
            var initChange = !analogPortState[value.Channel].HasValue;

            if (analogPortState[value.Channel] != value.Level)
            {
                analogPortState[value.Channel] = value.Level;

                AnalogStateChangeReceived?.Invoke(session, eventArgs);
            }
        }
        internal void OnDigitalStateReceived(FirmataEventArgs<DigitalPortState> eventArgs)
        {
            DigitalStateReceived?.Invoke(session, eventArgs);

            var value = eventArgs.Value;
            for (int i = 0; i < 8; i++)
            {
                var portPinNumber = value.Port * 8 + i;
                var currentState = value.IsSet(i);
                var initChange = !digitalPortState[portPinNumber].HasValue;
     
                if (digitalPortState[portPinNumber] != currentState)
                {
                    digitalPortState[portPinNumber] = currentState;

                    var pinState = new DigitalPinState(value.Port, portPinNumber, currentState, initChange);
                    DigitalStateChangeReceived?.Invoke(session,pinState);
                    //Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(i) ? 'X' : 'O', i, value.Pins);
                }
            }
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
    public class DigitalPinState
    {
        public DigitalPinState(int port, int portPinNumber, bool currentState, bool initChange)
        {
            this.Port = port;
            this.PinNumber = portPinNumber;
            this.Value = currentState;
            this.InitChange = initChange;
        }
        public int Port { get; }
        public int PinNumber { get; }
        public bool Value { get; }
        public bool InitChange { get; } = false;
    }
}
