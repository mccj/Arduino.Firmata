namespace Arduino.Firmata.Protocol.EEPROM
{
    public struct EEPROM_LengthResponse
    {
        public int Value { get; set; }
    }
    public struct EEPROM_ReadResponse
    {
        public byte Value { get; set; }
    }
    public struct EEPROM_BytesResponse
    {
        public byte[] Value { get; set; }
    }
}
