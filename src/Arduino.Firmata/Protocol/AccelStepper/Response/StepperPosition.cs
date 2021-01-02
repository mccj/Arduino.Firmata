namespace Arduino.Firmata.Protocol.AccelStepper
{
    public struct StepperPosition
    {
        public int DeviceNumber { get; set; }
        public long StepsNum { get; set; }
        public override string ToString()
        {
            return $"Device {DeviceNumber} Steps = {StepsNum}";
        }
    }
}
