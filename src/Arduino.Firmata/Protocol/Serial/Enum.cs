namespace Arduino.Firmata.Protocol.Serial
{
    public enum HW_SERIAL
    {
        HW_SERIAL0 = 0x00,//(for using Serial when another transport is used for the Firmata Stream)
        HW_SERIAL1 = 0x01,
        HW_SERIAL2 = 0x02,
        HW_SERIAL3 = 0x03
    }
    public enum SW_SERIAL
    {
        SW_SERIAL0 = 0x08,
        SW_SERIAL1 = 0x09,
        SW_SERIAL2 = 0x0A,
        SW_SERIAL3 = 0x0B
    }
}
