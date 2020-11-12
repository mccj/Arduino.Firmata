using Arduino.Firmata;
using Arduino.Firmata.Extend;
using Arduino.Firmata.Protocol.Serial;
using System.Linq;

namespace System.Linq
{
    /// <summary>
    /// Defines Servo control related members of the Firmata protocol.
    /// </summary>
    /// <remarks>
    /// This interface is separated from the <see cref="IFirmataProtocol"/> interface, in order to
    /// protect the latter against feature bloat. 
    /// </remarks>
    public static class SerialProtocol
    {
        /// <summary>
        /// Configures the specified hardware or software serial port. RX and TX pins are optional and should only be specified if the platform requires them to be set.
        /// </summary>
        public static void SerialConfig(this ArduinoSession session, HW_SERIAL hw, int baud = 9600, int? rxPin = null, int? txPin = null)
        {
            if (rxPin.HasValue && (rxPin < 0 || rxPin > 127))
                throw new ArgumentOutOfRangeException(nameof(rxPin), Messages.ArgumentEx_PinRange0_127);
            if (txPin.HasValue && (txPin < 0 || txPin > 127))
                throw new ArgumentOutOfRangeException(nameof(txPin), Messages.ArgumentEx_PinRange0_127);
            if (baud >= 4194303)
                throw new ArgumentOutOfRangeException(nameof(baud), "不能大于4194303");


            var bytes = baud.encode32BitSignedInteger();
            var command = new byte[9];
            command[0] = Utility.SysExStart;
            command[1] = (byte)0x60;
            command[2] = (byte)(0x10 | (int)hw);//SERIAL_CONFIG  //OR with port (0x11 = SERIAL_CONFIG | HW_SERIAL1)
            command[3] = bytes[0]; //baud (bits 0 - 6)
            command[4] = bytes[1]; //baud (bits 7 - 13)
            command[5] = bytes[2]; //baud (bits 14 - 20)
            if (rxPin.HasValue && txPin.HasValue)
            {
                command[6] = (byte)rxPin;
                command[7] = (byte)txPin;
                command[8] = Utility.SysExEnd;
            }
            else
                command[6] = Utility.SysExEnd;

            session.Write(command, 0, (rxPin.HasValue && txPin.HasValue) ? 8 : 6);
        }

        public static void SerialWrite(this ArduinoSession session, HW_SERIAL hw, string value)
        {
            var b = value.To14BitIso();

            var command = new byte[b.Length + 4];
            command[0] = Utility.SysExStart;
            command[1] = (byte)0x60;
            command[2] = (byte)(0x20 | (int)hw);//SERIAL_WRITE      //OR with port (0x21 = SERIAL_WRITE | HW_SERIAL1)

            b.CopyTo(command, 3);

            command[command.Length - 1] = Utility.SysExEnd;

            session.Write(command);
        }

        public static void SerialRead(this ArduinoSession session, HW_SERIAL hw, bool continuously = true, int? maxBytesToRead = null)
        {
            var command = new byte[14];
            command[0] = Utility.SysExStart;
            command[1] = (byte)0x60;
            command[2] = (byte)(0x30 | (int)hw);//SERIAL_READ  //OR with port (0x31 = SERIAL_READ | HW_SERIAL1)
            command[3] = continuously ? (byte)0x00 : (byte)0x01; //SERIAL_READ_MODE   0x00 => read continuously, 0x01 => stop reading
            if (maxBytesToRead.HasValue)
            {
                if (maxBytesToRead.Value >= 32767)
                    throw new ArgumentOutOfRangeException(nameof(maxBytesToRead), "不能大于32767");

                var bytes = maxBytesToRead.Value.encode32BitSignedInteger();
                command[4] = bytes[0];
                command[5] = bytes[1];
                command[6] = Utility.SysExEnd;
            }
            else
                command[4] = Utility.SysExEnd;

            session.Write(command, 0, maxBytesToRead.HasValue ? 6 : 4);
        }
        public static void SerialClose(this ArduinoSession session, HW_SERIAL hw)
        {
            var command = new[] {
                Utility.SysExStart,
                (byte)0x60,
                (byte)(0x50 | (int)hw),//SERIAL_CLOSE      // OR with port (0x51 = SERIAL_CLOSE | HW_SERIAL1) 
                Utility.SysExEnd
            };

            session.Write(command);
        }
        public static void SerialFlush(this ArduinoSession session, HW_SERIAL hw)
        {
            var command = new[] {
                Utility.SysExStart,
                (byte)0x60,
                (byte)(0x60 | (int)hw),//SERIAL_FLUSH      OR with port (0x61 = SERIAL_FLUSH | HW_SERIAL1)
                Utility.SysExEnd
            };

            session.Write(command);
        }
        public static void SerialListen(this ArduinoSession session, SW_SERIAL sw)
        {
            var command = new[] {
                Utility.SysExStart,
                (byte)0x60,
                (byte)(0x70 | (int)sw),//OR with port to switch to (0x79 = switch to SW_SERIAL1)
                Utility.SysExEnd
            };

            session.Write(command);
        }


        public static SerialEvint EvintSerial(this ArduinoSession session)
        {
            return session.GetEvint(() => new SerialEvint(session));
        }
        public static IObservable<SerialReply> CreateSerialReplyMonitor(this ArduinoSession session)
        {
            return new SerialReplyTracker(session.EvintSerial());
        }

    }
}
