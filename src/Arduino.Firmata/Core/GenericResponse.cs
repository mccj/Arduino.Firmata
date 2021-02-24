namespace Arduino.Firmata.Protocol.Firmata
{

    public struct GenericResponse
    {
        public byte MessageType { get; set; }
        public byte MessageSubType { get; set; }
    }
    public struct GenericResponse<T>
    {
        public byte MessageType { get; set; }
        public byte MessageSubType { get; set; }
        public T Value { get; set; }
    }
}
