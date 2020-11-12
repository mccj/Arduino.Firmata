namespace Arduino.Firmata.Protocol.Serial
{
    public struct SerialReply
    {
        public HW_SERIAL Serial { get; set; }
        public byte[] Data { get; set; }
    }
}
