using Arduino.Firmata;
using Arduino.Firmata.Protocol.AccelStepper;
using System;

namespace System.Linq
{
    /// <summary>
    /// RepRap Arduino Mega Pololu Shield
    /// </summary>
    public static class CNCShieldExtensions
    {
        /// <summary>
        /// Converts a <see cref="byte"/> array holding binary coded digits to a readable string.
        /// </summary>
        /// <param name="data">The binary coded digit bytes</param>
        /// <param name="isLittleEndian">Value indicating if the first nibble contains the least significant part</param>
        /// <returns>A string containing numeric data</returns>
        /// <exception cref="ArgumentException">The array contains one or more non-BCD bytes.</exception>
        public static void SetCNCShieldV3Board(this ArduinoSession session)
        {
            //Stepper
            // X
            session.StepperConfiguration(0, new DeviceConfig
            {
                MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                StepOrPin1Number = 2,
                DirectionOrPin2Number = 5,
                EnablePinNumber = 8,
                InvertEnablePinNumber = true
            });
            // Y
            session.StepperConfiguration(1, new DeviceConfig
            {
                MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                StepOrPin1Number = 3,
                DirectionOrPin2Number = 6,
                EnablePinNumber = 8,
                InvertEnablePinNumber = true
            }); 
            // Z
            session.StepperConfiguration(2, new DeviceConfig
            {
                MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                StepOrPin1Number = 4,
                DirectionOrPin2Number = 7,
                EnablePinNumber = 8,
                InvertEnablePinNumber = true
            });
            // A
            session.StepperConfiguration(3, new DeviceConfig
            {
                MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                StepOrPin1Number = 11,
                DirectionOrPin2Number = 13,
                EnablePinNumber = 8,
                InvertEnablePinNumber = true
            });
        }

    }
}
