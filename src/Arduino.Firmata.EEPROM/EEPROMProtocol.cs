using Arduino.Firmata.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        private static void Request_EEPROM_Length(this ArduinoSession session)
        {
            var command = new[]
            {
                Utility.SysExStart,
                EEPROM_DATA,
                EEPROM_LENGTH,
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static int EEPROM_Length(this ArduinoSession session)
        {
            Request_EEPROM_Length(session);
            return session.GetMessageFromQueue<int>(EEPROM_DATA, EEPROM_LENGTH).Value.Value;
        }
        public static async Task<int> EEPROM_LengthAsync(this ArduinoSession session)
        {
            Request_EEPROM_Length(session);
            return await Task.Run(() => session.GetMessageFromQueue<int>(EEPROM_DATA, EEPROM_LENGTH).Value.Value).ConfigureAwait(false);
        }
        private static void Request_EEPROM_Read(this ArduinoSession session, int index)
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
        }
        public static byte EEPROM_Read(this ArduinoSession session, int index)
        {
            Request_EEPROM_Read(session, index);
            return session.GetMessageFromQueue<byte>(EEPROM_DATA, EEPROM_READ).Value.Value;
        }
        public static async Task<byte> EEPROM_ReadAsync(this ArduinoSession session, int index)
        {
            Request_EEPROM_Read(session, index);
            return await Task.Run(() => session.GetMessageFromQueue<byte>(EEPROM_DATA, EEPROM_READ).Value.Value).ConfigureAwait(false);
        }
        public static void Request_EEPROM_Write(this ArduinoSession session, int index, byte value)
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

        public static void EEPROM_Write(this ArduinoSession session, int index, byte value)
        {
            Request_EEPROM_Write(session, index, value);
            session.GetMessageFromQueue(EEPROM_DATA, EEPROM_WRITE);
        }
        public static async Task EEPROM_WriteAsync(this ArduinoSession session, int index, byte value)
        {
            Request_EEPROM_Write(session, index, value);
            await Task.Run(() => session.GetMessageFromQueue(EEPROM_DATA, EEPROM_WRITE)).ConfigureAwait(false);
        }
        public static void Request_EEPROM_Update(this ArduinoSession session, int index, byte value)
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
        public static void EEPROM_Update(this ArduinoSession session, int index, byte value)
        {
            Request_EEPROM_Update(session, index, value);
            session.GetMessageFromQueue(EEPROM_DATA, EEPROM_UPDATE);
        }
        public static async Task EEPROM_UpdateAsync(this ArduinoSession session, int index, byte value)
        {
            Request_EEPROM_Update(session, index, value);
            await Task.Run(() => session.GetMessageFromQueue(EEPROM_DATA, EEPROM_UPDATE)).ConfigureAwait(false);
        }
        public static void Request_EEPROM_Put(this ArduinoSession session, int index, byte[] bytes)
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
        public static void EEPROM_Put(this ArduinoSession session, int index, byte[] bytes)
        {
            Request_EEPROM_Put(session, index, bytes);
            session.GetMessageFromQueue(EEPROM_DATA, EEPROM_PUT);
        }
        public static async Task EEPROM_PutAsync(this ArduinoSession session, int index, byte[] bytes)
        {
            Request_EEPROM_Put(session, index, bytes);
            await Task.Run(() => session.GetMessageFromQueue(EEPROM_DATA, EEPROM_PUT)).ConfigureAwait(false);
        }
        public static void Request_EEPROM_Get(this ArduinoSession session, int index, int length)
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
        }
        public static byte[] EEPROM_Get(this ArduinoSession session, int index, int length)
        {
            Request_EEPROM_Get(session, index, length);
            return session.GetMessageFromQueue<byte[]>(EEPROM_DATA, EEPROM_GET).Value.Value;
        }
        public static async Task<byte[]> EEPROM_GetAsync(this ArduinoSession session, int index, int length)
        {
            Request_EEPROM_Get(session, index, length);
            return await Task.Run(() => session.GetMessageFromQueue<byte[]>(EEPROM_DATA, EEPROM_GET).Value.Value).ConfigureAwait(false);
        }
    }
}
