namespace Arduino.Firmata
{
    /// <summary>
    /// Defines a serial port connection.
    /// </summary>
    /// <seealso href="http://arduino.cc/en/Reference/Serial">Serial reference for Arduino</seealso>
    public interface ISysExMessage
    {
        bool CanHeader(byte messageByte);
        IFirmataMessage Header(MessageHeader messageHeader);
    }
}
