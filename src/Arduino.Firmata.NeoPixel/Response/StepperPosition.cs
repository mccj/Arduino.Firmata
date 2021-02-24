namespace Arduino.Firmata.Protocol.NeoPixel
{
    public struct GenericResponse
    {
        public byte MessageType { get; set; }
        public byte MessageSubType { get; set; }
        public int DeviceNumber { get; set; }
    }
    public struct GenericResponse<T>
    {
        public byte MessageType { get; set; }
        public byte MessageSubType { get; set; }
        public int DeviceNumber { get; set; }
        public T Value { get; set; }
    }
    //public struct CanShow
    //{
    //    public int DeviceNumber { get; set; }
    //    public bool Value { get; set; }
    //}
    //public struct Brightness
    //{
    //    public int DeviceNumber { get; set; }
    //    public byte Value { get; set; }
    //}
    //public struct Pin
    //{
    //    public int DeviceNumber { get; set; }
    //    public byte Value { get; set; }
    //}
    //public struct NumPixels
    //{
    //    public int DeviceNumber { get; set; }
    //    public int Value { get; set; }
    //}
    //public struct GetPixelColor
    //{
    //    public int DeviceNumber { get; set; }
    //    public int Value { get; set; }
    //}
    //public struct Sine8
    //{
    //    public int DeviceNumber { get; set; }
    //    public byte Value { get; set; }
    //} 
    //public struct Gamma8
    //{
    //    public int DeviceNumber { get; set; }
    //    public byte Value { get; set; }
    //}  
    //public struct Gamma32
    //{
    //    public int DeviceNumber { get; set; }
    //    public int Value { get; set; }
    //}  
    //public struct PixelColor
    //{
    //    public int DeviceNumber { get; set; }
    //    public int Value { get; set; }
    //}  
    //public struct ColorHSV
    //{
    //    public int DeviceNumber { get; set; }
    //    public int Value { get; set; }
    //}
}
