using Arduino.Firmata.Extend;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arduino.Firmata.Protocol.EEPROM
{
    public static class EEPROMProtocol
    {
        public const byte EEPROM_DATA = 0x64;

        public const byte EEPROM_LENGTH = 0x00;
        public const byte EEPROM_READ = 0x01;
        public const byte EEPROM_WRITE = 0x02;
        public const byte EEPROM_UPDATE = 0x03;
        public const byte EEPROM_PUT = 0x04;
        public const byte EEPROM_GET = 0x05;

        public static int EEPROM_Length(this ArduinoSession session)
        {
            var command = new[]
            {
                Utility.SysExStart,
                EEPROM_DATA,
                EEPROM_LENGTH,
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);

            return session.GetMessageFromQueue<EEPROM_LengthResponse>().Value.Value;
        }
        public static byte EEPROM_Read(this ArduinoSession session, int index)
        {
            var bytes = index.encode32BitSignedInteger();

            var command = new[]
            {
                Utility.SysExStart,
                EEPROM_DATA,
                EEPROM_READ,
                bytes[0], //4  num steps, bits 0-6
                bytes[1], //5  num steps, bits 7-13
                bytes[2], //6  num steps, bits 14-20
                bytes[3], //7  num steps, bits 21-27
                bytes[4], //8  num steps, bits 28-32
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);

            return session.GetMessageFromQueue<EEPROM_ReadResponse>().Value.Value;
        }

        public static void EEPROM_Write(this ArduinoSession session, int index, byte value)
        {
            var indexBytes = index.encode32BitSignedInteger();
            var valueBytes = value.encode8BitSignedByte();
            var command = new[]
            {
                Utility.SysExStart,
                EEPROM_DATA,
                EEPROM_WRITE,
                indexBytes[0],
                indexBytes[1],
                indexBytes[2],
                indexBytes[3],
                indexBytes[4],
                valueBytes[0],
                valueBytes[1],
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static void EEPROM_Update(this ArduinoSession session, int index, byte value)
        {
            var indexBytes = index.encode32BitSignedInteger();
            var valueBytes = value.encode8BitSignedByte();
            var command = new[]
            {
                Utility.SysExStart,
                EEPROM_DATA,
                EEPROM_UPDATE,
                indexBytes[0],
                indexBytes[1],
                indexBytes[2],
                indexBytes[3],
                indexBytes[4],
                valueBytes[0],
                valueBytes[1],
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static void EEPROM_Put(this ArduinoSession session, int index, byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(bytes), "bytes 不能为空且长度必须大于0");

            var indexBytes = index.encode32BitSignedInteger();
            var encoder7Bytes = ByteArrayExtensions.Encoder7Bit(bytes);
            var command = new List<byte>
            {
                Utility.SysExStart,
                EEPROM_DATA,
                EEPROM_PUT,
                indexBytes[0], //4  num steps, bits 0-6
                indexBytes[1], //5  num steps, bits 7-13
                indexBytes[2], //6  num steps, bits 14-20
                indexBytes[3], //7  num steps, bits 21-27
                indexBytes[4], //8  num steps, bits 28-32
                //(byte)white,
                //Utility.SysExEnd
            };
            command.AddRange(encoder7Bytes);

            command.Add(Utility.SysExEnd);

            session.Write(command.ToArray(), 0, command.Count);
        }
        public static byte[] EEPROM_Get(this ArduinoSession session, int index, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "length 必须大于0");

            var indexBytes = index.encode32BitSignedInteger();
            var lengthBytes = length.encode32BitSignedInteger();
            var command = new[]
            {
                Utility.SysExStart,
                EEPROM_DATA,
                EEPROM_GET,
                indexBytes[0], //4  num steps, bits 0-6
                indexBytes[1], //5  num steps, bits 7-13
                indexBytes[2], //6  num steps, bits 14-20
                indexBytes[3], //7  num steps, bits 21-27
                indexBytes[4], //8  num steps, bits 28-32
                lengthBytes[0], //4  num steps, bits 0-6
                lengthBytes[1], //5  num steps, bits 7-13
                lengthBytes[2], //6  num steps, bits 14-20
                lengthBytes[3], //7  num steps, bits 21-27
                lengthBytes[4], //8  num steps, bits 28-32
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
            return session.GetMessageFromQueue<EEPROM_BytesResponse>().Value.Value;
        }
    }
}
