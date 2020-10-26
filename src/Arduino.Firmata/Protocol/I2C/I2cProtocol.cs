using System;
using System.Threading.Tasks;
using Arduino.Firmata;
using Solid.Arduino.Firmata;
using Solid.Arduino.I2C;

namespace Solid.Arduino
{
    /// <summary>
    /// Defines a comprehensive set of members supporting the I2C Protocol.
    /// </summary>
    /// <seealso href="http://www.i2c-bus.org/">I2C bus website by telos Systementwicklung GmbH</seealso>
    /// <seealso href="http://www.arduino.cc/en/Reference/Wire">Arduino Wire reference</seealso>
    /// <seealso href="http://playground.arduino.cc/Main/I2cScanner">I2C Scanner sample sketch for Arduino</seealso>
    public static class I2CProtocol
    {
        /// <summary>
        /// Creates an observable object tracking <see cref="I2CReply"/> messages.
        /// </summary>
        /// <returns>An <see cref="IObservable{I2cReply}"/> interface</returns>
        public static IObservable<I2CReply> CreateI2CReplyMonitor(this ArduinoSession session)
        {
            return new I2CReplyTracker(session.EvintI2C());
        }

        /// <summary>
        /// Sets the frequency at which data is read in the continuous mode.
        /// </summary>
        /// <param name="microseconds">The interval, expressed in microseconds</param>
        public static void SetI2CReadInterval(this ArduinoSession session, int microseconds)
        {
            if (microseconds < 0 || microseconds > 0x3FFF)
                throw new ArgumentOutOfRangeException(nameof(microseconds), Messages.ArgumentEx_I2cInterval);

            var command = new[]
            {
                    Utility.SysExStart,
                    (byte)0x78,
                    (byte)(microseconds & 0x7F),
                    (byte)((microseconds >> 7) & 0x7F),
                    Utility.SysExEnd
                };
            session.Write(command, 0, 5);
        }

        /// <summary>
        /// Writes an arbitrary array of bytes to the given memory address.
        /// </summary>
        /// <param name="slaveAddress">The slave's target address</param>
        /// <param name="data">The data array</param>
        public static void WriteI2C(this ArduinoSession session, int slaveAddress, params byte[] data)
        {
            if (slaveAddress < 0 || slaveAddress > 0x3FF)
                throw new ArgumentOutOfRangeException(nameof(slaveAddress), Messages.ArgumentEx_I2cAddressRange);

            byte[] command = new byte[data.Length * 2 + 5];
            command[0] = Utility.SysExStart;
            command[1] = 0x76;
            command[2] = (byte)(slaveAddress & 0x7F);
            command[3] = (byte)(slaveAddress < 0x80 ? 0 : ((slaveAddress >> 7) & 0x07) | 0x20);

            for (int x = 0; x < data.Length; x++)
            {
                command[x * 2 + 4] = (byte)(data[x] & 0x7F);
                command[x * 2 + 5] = (byte)((data[x] >> 7) & 0x7F);
            }

            command[command.Length - 1] = Utility.SysExEnd;

            session.Write(command, 0, command.Length);
        }

        /// <summary>
        /// Requests the party system to send bytes read from the given memory address.
        /// </summary>
        /// <param name="slaveAddress">The slave's memory address</param>
        /// <param name="bytesToRead">Number of bytes to read</param>
        /// <remarks>
        /// The party system is expected to return a single I2C_REPLY message.
        /// This message triggers the <see cref="I2CReplyReceived"/> event. The data
        /// are passed in the <see cref="FirmataEventArgs{T}"/> in an <see cref="I2CReply"/> object.
        /// </remarks>
        public static void ReadI2COnce(this ArduinoSession session, int slaveAddress, int bytesToRead)
        {
            session.I2CRead(false, slaveAddress, -1, bytesToRead);
        }

        /// <summary>
        /// Requests the party system to send bytes read from the given memory address and register.
        /// </summary>
        /// <param name="slaveAddress">The slave's memory address</param>
        /// <param name="slaveRegister">The slave's register</param>
        /// <param name="bytesToRead">Number of bytes to read</param>
        public static void ReadI2COnce(this ArduinoSession session, int slaveAddress, int slaveRegister, int bytesToRead)
        {
            session.I2CSlaveRead(false, slaveAddress, slaveRegister, bytesToRead);
        }

        /// <summary>
        /// Requests the party system to repeatedly send bytes read from the given memory address.
        /// </summary>
        /// <param name="slaveAddress">The slave's address</param>
        /// <param name="bytesToRead">Number of bytes to read</param>
        /// <remarks>
        /// The party system is expected to return a continuous stream of I2C_REPLY messages at
        /// an interval which can be set using the <see cref="SetI2CReadInterval"/> method.
        /// Received I2C_REPLY messages trigger the <see cref="I2CReplyReceived"/> event. The data
        /// are served in the <see cref="I2CEventArgs"/>'s Value property as an <see cref="I2CReply"/> object.
        /// <para>
        /// The party system can be stopped sending I2C_REPLY messages by issuing a <see cref="StopI2CReading"/> command.
        /// </para>
        /// </remarks>
        public static void ReadI2CContinuous(this ArduinoSession session, int slaveAddress, int bytesToRead)
        {
            I2CRead(session, true, slaveAddress, -1, bytesToRead);
        }

        /// <summary>
        /// Requests the party system to repeatedly send bytes read from the given memory address and register.
        /// </summary>
        /// <param name="slaveAddress">The slave's memory address</param>
        /// <param name="slaveRegister">The slave's register</param>
        /// <param name="bytesToRead">Number of bytes to read</param>
        public static void ReadI2CContinuous(this ArduinoSession session, int slaveAddress, int slaveRegister, int bytesToRead)
        {
            I2CSlaveRead(session, true, slaveAddress, slaveRegister, bytesToRead);
        }

        /// <summary>
        /// Commands the party system to stop sending I2C_REPLY messages.
        /// </summary>
        public static void StopI2CReading(this ArduinoSession session)
        {
            byte[] command = new byte[5];
            command[0] = Utility.SysExStart;
            command[1] = 0x76;
            command[2] = 0x00;
            command[3] = 0x18;
            command[4] = Utility.SysExEnd;

            session.Write(command, 0, command.Length);
        }

        /// <summary>
        /// Gets byte data from the party system, read from the given memory address.
        /// </summary>
        /// <param name="slaveAddress">The slave's memory address</param>
        /// <param name="bytesToRead">Number of bytes to read</param>
        /// <returns>An <see cref="I2CReply"/> object holding the data read</returns>
        public static I2CReply GetI2CReply(this ArduinoSession session, int slaveAddress, int bytesToRead)
        {
            session.ReadI2COnce(slaveAddress, bytesToRead);
            //_awaitedMessagesQueue.Enqueue(new FirmataMessage(MessageType.I2CReply));
            //return (I2CReply)((FirmataMessage)GetMessageFromQueue(new FirmataMessage(MessageType.I2CReply))).Value;
            return session.GetMessageFromQueue<I2CReply>().Value;
        }

        /// <summary>
        /// Asynchronously gets byte data from the party system, read from the given memory address.
        /// </summary>
        /// <param name="slaveAddress">The slave's memory address</param>
        /// <param name="bytesToRead">Number of bytes to read</param>
        /// <returns>An awaitable <see cref="Task{I2cReply}"/> holding the data read</returns>
        public static async Task<I2CReply> GetI2CReplyAsync(this ArduinoSession session, int slaveAddress, int bytesToRead)
        {
            session.ReadI2COnce(slaveAddress, bytesToRead);
            //_awaitedMessagesQueue.Enqueue(new FirmataMessage(MessageType.I2CReply));
            //return await Task.Run(() =>
            //    (I2CReply)((FirmataMessage)GetMessageFromQueue(new FirmataMessage(MessageType.I2CReply))).Value);
            return await Task.Run(() => session.GetMessageFromQueue<I2CReply>().Value).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets byte data from the party system, read from the given memory address and register.
        /// </summary>
        /// <param name="slaveAddress">The slave's memory address and register</param>
        /// <param name="slaveRegister">The slave's register</param>
        /// <param name="bytesToRead">Number of bytes to read</param>
        /// <returns>An <see cref="I2CReply"/> object holding the data read</returns>
        public static I2CReply GetI2CReply(this ArduinoSession session, int slaveAddress, int slaveRegister, int bytesToRead)
        {
            session.ReadI2COnce(slaveAddress, slaveRegister, bytesToRead);
            //_awaitedMessagesQueue.Enqueue(new FirmataMessage(MessageType.I2CReply));
            //return (I2CReply)((FirmataMessage)GetMessageFromQueue(new FirmataMessage(MessageType.I2CReply))).Value;
            return session.GetMessageFromQueue<I2CReply>().Value;
        }

        /// <summary>
        /// Asynchronously gets byte data from the party system, read from the given memory address and register.
        /// </summary>
        /// <param name="slaveAddress">The slave's memory address</param>
        /// <param name="slaveRegister">The slave's register</param>
        /// <param name="bytesToRead">Number of bytes to read</param>
        /// <returns>An awaitable <see cref="Task{I2cReply}"/> holding the data read</returns>
        public static async Task<I2CReply> GetI2CReplyAsync(this ArduinoSession session, int slaveAddress, int slaveRegister, int bytesToRead)
        {
            session.ReadI2COnce(slaveAddress, slaveRegister, bytesToRead);
            //_awaitedMessagesQueue.Enqueue(new FirmataMessage(MessageType.I2CReply));
            //return await Task.Run(() =>
            //    (I2CReply)((FirmataMessage)GetMessageFromQueue(new FirmataMessage(MessageType.I2CReply))).Value);
            return await Task.Run(() => session.GetMessageFromQueue<I2CReply>().Value).ConfigureAwait(false);
        }

        public static I2CEvint EvintI2C(this ArduinoSession session)
        {
            return session.GetEvint(() => new I2CEvint(session));
        }


















        private static void I2CRead(this ArduinoSession session, bool continuous, int slaveAddress, int slaveRegister = -1, int bytesToRead = 0)
        {
            if (slaveAddress < 0 || slaveAddress > 0x3FF)
                throw new ArgumentOutOfRangeException(nameof(slaveAddress), Messages.ArgumentEx_I2cAddressRange);

            if (bytesToRead < 0 || bytesToRead > 0x3FFF)
                throw new ArgumentOutOfRangeException(nameof(bytesToRead), Messages.ArgumentEx_ValueRange0_16383);

            byte[] command = new byte[(slaveRegister == -1 ? 7 : 9)];
            command[0] = Utility.SysExStart;
            command[1] = 0x76;
            command[2] = (byte)(slaveAddress & 0x7F);
            command[3] = (byte)(((slaveAddress >> 7) & 0x07) | (slaveAddress < 128 ? (continuous ? 0x10 : 0x08) : (continuous ? 0x30 : 0x28)));

            if (slaveRegister != -1)
            {
                command[4] = (byte)(slaveRegister & 0x7F);
                command[5] = (byte)(slaveRegister >> 7);
                command[6] = (byte)(bytesToRead & 0x7F);
                command[7] = (byte)(bytesToRead >> 7);
            }
            else
            {
                command[4] = (byte)(bytesToRead & 0x7F);
                command[5] = (byte)(bytesToRead >> 7);
            }

            command[command.Length - 1] = Utility.SysExEnd;

            session.Write(command, 0, command.Length);
        }
        private static void I2CSlaveRead(this ArduinoSession session, bool continuous, int slaveAddress, int slaveRegister = -1, int bytesToRead = 0)
        {
            if (slaveRegister < 0 || slaveRegister > 0x3FFF)
                throw new ArgumentOutOfRangeException(nameof(slaveRegister), Messages.ArgumentEx_ValueRange0_16383);

            I2CRead(session, continuous, slaveAddress, slaveRegister, bytesToRead);
        }
    }
}
