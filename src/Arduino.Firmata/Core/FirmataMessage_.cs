using System;

namespace Solid.Arduino.Firmata
{
    /// <inheritdoc />
    public class FirmataMessage<T> : IFirmataMessage
        where T : struct
    {
        /// <summary>
        ///     Initializes a new <see cref="FirmataMessage{T}" /> instance.
        /// </summary>
        /// <param name="value"></param>
        public FirmataMessage(T value) : this(value, DateTime.UtcNow) { }
        public FirmataMessage(T value, DateTime time)
        {
            Value = value;
            Time = time;
        }
        public string Name => typeof(T).Name;

        /// <summary>
        ///     Gets the specific value delivered by the message.
        /// </summary>
        public T Value { get; }

        /// <inheritdoc />
        public DateTime Time { get; }

        ValueType IFirmataMessage.Value => Value;
    }
}