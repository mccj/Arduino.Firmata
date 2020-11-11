namespace Arduino.Firmata.Protocol.Firmata
{
    /// <summary>
    /// Represents the Firmata communication protocol version.
    /// </summary>
    public struct ProtocolVersion
    {
        /// <summary>
        /// Gets or sets the major version number.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Gets or sets the minor version number.
        /// </summary>
        public int Minor { get; set; }
        public override string ToString()
        {
            return $"V{Major}.{Minor}";
        }
    }
}
