﻿using System;

namespace Arduino.Firmata
{
    /// <summary>
    /// Defines a serial port connection.
    /// </summary>
    /// <seealso href="http://arduino.cc/en/Reference/Serial">Serial reference for Arduino</seealso>
    public interface IDataConnection : IDisposable
    {
        /// <summary>Indicates that no time-out should occur.</summary>
        int InfiniteTimeout { get; }

        /// <summary>
        ///  Represents the method that will handle the data received event of a <see cref="IDataConnection"/> object.
        /// </summary>
        event DataReceivedEventHandler DataReceived;

        /// <summary>
        /// Name of the connected device, e.g. "COM1".
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating the open or closed status of the <see cref="IDataConnection"/> object.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets or sets the value used to interpret the end of strings received and sent
        /// using <see cref="StringProtocol.ReadLine"/> and <see cref="StringProtocol.WriteLine"/> methods.
        /// </summary>
        /// <remarks>
        /// The default is a line feed, (<see cref="Environment.NewLine"/>).
        /// </remarks>
        string NewLine { get; set; }

        /// <summary>
        /// Gets the number of bytes of data in the receive buffer.
        /// </summary>
        int BytesToRead { get; }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Reads a byte from the underlying serial input data stream.
        /// </summary>
        /// <returns>
        /// The byte, cast to an Int32, or -1 if the end of the stream has been read.
        /// </returns>
        int ReadByte();

        /// <summary>
        /// Writes a string to the serial output data stream.
        /// </summary>
        /// <param name="text">A string to be written</param>
        void Write(string text);

        /// <summary>
        /// Writes a specified number of bytes to the serial output stream using data from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array that contains the data to write</param>
        /// <param name="offset">The zero-based byte offset in the array at which to begin copying bytes</param>
        /// <param name="count">The number of bytes to write</param>
        void Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// Writes the specified string and the <see cref="SerialPort.NewLine"/> value to the serial output stream.
        /// </summary>
        /// <param name="text">The string to write</param>
        void WriteLine(string text);
    }
}
