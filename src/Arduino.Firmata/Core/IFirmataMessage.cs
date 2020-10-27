using System;

namespace Arduino.Firmata
{
    /// <summary>
    /// Represents a Firmata message received from an Arduino or Arduino compatible system.
    /// </summary>
    public interface IFirmataMessage
    {
        /// <summary>
        ///     Gets the time of the delivered message.
        /// </summary>
        DateTime Time { get; }

        string Name { get; }
        /// <summary>
        ///     Gets the specific value delivered by the message.
        /// </summary>
        public ValueType Value { get; }

    }
}