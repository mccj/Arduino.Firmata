using Arduino.Firmata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Arduino.Firmata.Servo
{
    /// <summary>
    /// Defines Servo control related members of the Firmata protocol.
    /// </summary>
    /// <remarks>
    /// This interface is separated from the <see cref="IFirmataProtocol"/> interface, in order to
    /// protect the latter against feature bloat. 
    /// </remarks>
    public static class ServoProtocol
    {
        /// <summary>
        /// Configures the minimum and maximum pulse length for a servo pin.
        /// </summary>
        /// <param name="pinNumber">The pin number</param>
        /// <param name="minPulse">Minimum pulse length</param>
        /// <param name="maxPulse">Maximum pulse length</param>
        public static void ConfigureServo(this ArduinoSession session, int pinNumber, int minPulse, int maxPulse)
        {
            if (pinNumber < 0 || pinNumber > 127)
                throw new ArgumentOutOfRangeException(nameof(pinNumber), Messages.ArgumentEx_PinRange0_127);

            if (minPulse < 0 || minPulse > 0x3FFF)
                throw new ArgumentOutOfRangeException(nameof(minPulse), Messages.ArgumentEx_MinPulseWidth);

            if (maxPulse < 0 || maxPulse > 0x3FFF)
                throw new ArgumentOutOfRangeException(nameof(maxPulse), Messages.ArgumentEx_MaxPulseWidth);

            if (minPulse > maxPulse)
                throw new ArgumentException(Messages.ArgumentEx_MinMaxPulse);

            var command = new[]
            {
                Utility.SysExStart,
                (byte)0x70,
                (byte)pinNumber,
                (byte)(minPulse & 0x7F),
                (byte)((minPulse >> 7) & 0x7F),
                (byte)(maxPulse & 0x7F),
                (byte)((maxPulse >> 7) & 0x7F),
                Utility.SysExEnd
            };
            session.Write(command);
        }
    }
}
