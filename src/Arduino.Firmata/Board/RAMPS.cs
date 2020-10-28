using Arduino.Firmata;
using Arduino.Firmata.Protocol.AccelStepper;
using System;

namespace System.Linq
{
    /// <summary>
    /// RepRap Arduino Mega Pololu Shield
    /// </summary>
    public static class RAMPSExtensions
    {
        /// <summary>
        /// Converts a <see cref="byte"/> array holding binary coded digits to a readable string.
        /// </summary>
        /// <param name="data">The binary coded digit bytes</param>
        /// <param name="isLittleEndian">Value indicating if the first nibble contains the least significant part</param>
        /// <returns>A string containing numeric data</returns>
        /// <exception cref="ArgumentException">The array contains one or more non-BCD bytes.</exception>
        public static void SetRAMPSBoard(this ArduinoSession session)
        {
            //Stepper
            // X
            session.StepperConfiguration(0, new DeviceConfig
            {
                MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                StepOrPin1Number = 54,
                DirectionOrPin2Number = 55,
                EnablePinNumber = 38,
                InvertEnablePinNumber = true
            });
            // Y
            session.StepperConfiguration(1, new DeviceConfig
            {
                MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                StepOrPin1Number = 60,
                DirectionOrPin2Number = 61,
                EnablePinNumber = 56,
                InvertEnablePinNumber = true
            }); 
            // Z
            session.StepperConfiguration(2, new DeviceConfig
            {
                MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                StepOrPin1Number = 46,
                DirectionOrPin2Number = 48,
                EnablePinNumber = 62,
                InvertEnablePinNumber = true
            });
            // E0
            session.StepperConfiguration(3, new DeviceConfig
            {
                MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                StepOrPin1Number = 26,
                DirectionOrPin2Number = 28,
                EnablePinNumber = 24,
                InvertEnablePinNumber = true
            });
            // E1
            session.StepperConfiguration(4, new DeviceConfig
            {
                MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                StepOrPin1Number = 36,
                DirectionOrPin2Number = 34,
                EnablePinNumber = 30,
                InvertEnablePinNumber = true
            });
        }

    }
}
