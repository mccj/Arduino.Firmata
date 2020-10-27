using Arduino.Firmata;
using Arduino.Firmata.Extend;
using System.Linq;

namespace System.Linq
{
    /// <summary>
    /// https://github.com/firmata/protocol/blob/master/encoder.md
    /// </summary>
    public static class EncoderProtocol
    {
        public const byte ENCODER_DATA = 0x61; // ENCODER_DATA（0x61）
        public static void 附加编码器(this ArduinoSession session, int encoderNumber,int encoderPinA, int encoderPinB)
        {
            if (encoderNumber < 0 || encoderNumber > 5)
                throw new ArgumentOutOfRangeException(nameof(encoderNumber), "Encoder number must be between 0 and 5.");
            if (encoderPinA < 0 || encoderPinA > 127)
                throw new ArgumentOutOfRangeException(nameof(encoderPinA), "Pin number must be between 0 and 127.");
            if (encoderPinB < 0 || encoderPinB > 127)
                throw new ArgumentOutOfRangeException(nameof(encoderPinB), "Pin number must be between 0 and 127.");

            var command = new[]
            {
                Utility.SysExStart,
                ENCODER_DATA,//ENCODER_DATA（0x61）
                (byte)0x00,//ENCODER_ATTACH（0x00）
                (byte)encoderNumber,//encoder #                  ([0 - MAX_ENCODERS-1])
                (byte)encoderPinA,//4 pin A #(first pin) 
                (byte)encoderPinB,//5 pin B #(second pin)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
    }
}
