namespace Arduino.Firmata.Protocol.String
{
    /// <summary>
    /// Represents a string exchanged with the Firmata SYSEX STRING_DATA command.
    /// </summary>
    public struct StringData
    {
        /// <summary>
        /// Gets or sets the string.
        /// </summary>
        public string Text { get; set; }
    }
}
