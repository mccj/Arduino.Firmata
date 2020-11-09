using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
#if NETSTANDARD
using SerialPort = RJCP.IO.Ports.SerialPortStream;
using SerialDataReceivedEventArgs = RJCP.IO.Ports.SerialDataReceivedEventArgs;
using SerialErrorReceivedEventArgs = RJCP.IO.Ports.SerialErrorReceivedEventArgs;
#else
using SerialPort = System.IO.Ports.SerialPort;
using SerialDataReceivedEventArgs = System.IO.Ports.SerialDataReceivedEventArgs;
using SerialErrorReceivedEventArgs = System.IO.Ports.SerialErrorReceivedEventArgs;
#endif

namespace Arduino.Firmata.Serial
{
    public class SerialConnection : IDataConnection
    {
        #region Fields
        private readonly SerialPort _serial;

        private const int DefaultTimeoutMs = 100;
        private bool _isDisposed;

        private static readonly SerialBaudRate[] PopularBaudRates =
        {
            SerialBaudRate.Bps_9600,
            SerialBaudRate.Bps_57600,
            SerialBaudRate.Bps_115200
        };

        private static readonly SerialBaudRate[] OtherBaudRates =
        {
            SerialBaudRate.Bps_28800,
            SerialBaudRate.Bps_14400,
            SerialBaudRate.Bps_38400,
            SerialBaudRate.Bps_31250,
            SerialBaudRate.Bps_4800,
            SerialBaudRate.Bps_2400
        };

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of <see cref="EnhancedSerialConnection"/> class using the highest serial port available at 115,200 bits per second.
        /// </summary>
        public SerialConnection() : this(GetLastPortName(), (int)SerialBaudRate.Bps_115200) { }

        /// <summary>
        /// Initializes a new instance of <see cref="EnhancedSerialConnection"/> class on the given serial port and at the given baud rate.
        /// </summary>
        /// <param name="portName">The port name (e.g. 'COM3')</param>
        /// <param name="baudRate">The baud rate</param>
        public SerialConnection(string portName, SerialBaudRate baudRate) : this(portName, (int)baudRate) { }

        /// <summary>
        /// Initializes a new instance of <see cref="MicrosoftSerialConnection"/> class on the given serial port and at the given baud rate.
        /// </summary>
        /// <param name="portName">The port name (e.g. 'COM3')</param>
        /// <param name="baudRate">The baud rate</param>
        public SerialConnection(string portName, int baudRate)
        {
#if NETSTANDARD
            _serial = new SerialPort(portName, baudRate, 8, RJCP.IO.Ports.Parity.None, RJCP.IO.Ports.StopBits.One);
#else
            _serial = new SerialPort(portName, baudRate, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
#endif
            _serial.ReadTimeout = DefaultTimeoutMs;
            _serial.WriteTimeout = DefaultTimeoutMs;

            _serial.DataReceived += OnSerialPortDataReceived;
            _serial.ErrorReceived += OnSerialPortErrorReceived;

        }

        #endregion

        #region Public Methods & Properties

        public int InfiniteTimeout => SerialPort.InfiniteTimeout;
        public event DataReceivedEventHandler DataReceived;
        public string Name => _serial.PortName;
        public string PortName => this.Name;
        public int BaudRate => _serial.BaudRate;
        public bool IsOpen => _serial.IsOpen;
        public string NewLine { get => _serial.NewLine; set => _serial.NewLine = value; }
        public int BytesToRead => _serial.BytesToRead;


        /// <inheritdoc cref="SerialPort" />
        public void Open()
        {
            if (IsOpen)
                return;

            try
            {
                _serial.Open();

                // My observation on the Raspberry Pi was, that the serial port (Arduino connected on /dev/ttyUSB0) already had data
                // in its buffer, even though the connected Arduino hadn't sent anything.
                // The data in the buffer appeared to be from a previous run of the program.
                // So apparently the buffer is remembered even across consecutive runs of the program and after the serial port is closed.
                // This means that the serial port OS driver is at fault here, as imo it should have cleared 
                // its buffer after a Close or directly when Opened.
                // NOPE: RawDump on Rpi shows that data is correctly received, so no buffers are "remembered".

#if NETSTANDARD
                _serial.Flush();
#endif
                _serial.DiscardOutBuffer();
                _serial.DiscardInBuffer();
            }
            catch (UnauthorizedAccessException)
            {
                // Connection closure has probably not yet been finalized.
                // Wait 250 ms and try again once.
                Thread.Sleep(250);
                _serial.Open();
            }
        }

        /// <inheritdoc cref="SerialPort.Close"/>
        public void Close()
        {
            if (!IsOpen)
                return;

            Thread.Sleep(250);
#if NETSTANDARD
            _serial.Flush();
#endif
            _serial.DiscardInBuffer();
            _serial.Close();
        }

        /// <inheritdoc cref="SerialPort.Dispose"/>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _serial.DataReceived -= OnSerialPortDataReceived;
            _serial.ErrorReceived -= OnSerialPortErrorReceived;

            _serial.Dispose();
            GC.SuppressFinalize(this);
        }

        public int ReadByte()
        {
            return _serial.ReadByte();
        }

        public void Write(string text)
        {
            _serial.Write(text);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _serial.Write(buffer, offset, count);
        }

        public void WriteLine(string text)
        {
            _serial.WriteLine(text);
        }

        public static string[] GetPortNames() => SerialPort.GetPortNames();
        /// <summary>
        /// Finds a serial connection to a device supporting the Firmata protocol.
        /// </summary>
        /// <returns>A <see cref="IDataConnection"/> instance or <c>null</c> if no connection is found</returns>
        /// <remarks>
        /// <para>
        /// This method searches all available serial ports until it finds a working serial connection.
        /// For every available serial port an attempt is made to open a connection at a range of common baudrates.
        /// The connection is tested by issueing an <see cref="IFirmataProtocol.GetFirmware()"/> command.
        /// (I.e. a Firmata SysEx Firmware query (0xF0 0x79 0xF7).)
        /// </para>
        /// <para>
        /// The connected device is expected to respond by sending the version number of the supported protocol.
        /// When a major version of 2 or higher is received, the connection is regarded to be valid.
        /// </para>
        /// </remarks>
        /// <seealso cref="IFirmataProtocol"/>
        /// <seealso href="http://www.firmata.org/wiki/Protocol#Query_Firmware_Name_and_Version">Query Firmware Name and Version</seealso>
        public static IDataConnection Find()
        {
            Func<ArduinoSession, bool> isAvailableFunc = session =>
            {
                var firmware = session.GetFirmware();
                return firmware.MajorVersion >= 2;
            };

            string[] portNames = GetPortNames();
            var connection = FindConnection(isAvailableFunc, portNames, PopularBaudRates);
            return connection ?? FindConnection(isAvailableFunc, portNames, OtherBaudRates);
        }

        /// <summary>
        /// Finds a serial connection to a device supporting plain serial communications.
        /// </summary>
        /// <param name="query">The query text used to inquire the connection</param>
        /// <param name="expectedReply">The reply text the connected device is expected to respond with</param>
        /// <returns>A <see cref="IDataConnection"/> instance or <c>null</c> if no connection is found</returns>
        /// <remarks>
        /// <para>
        /// This method searches all available serial ports until it finds a working serial connection.
        /// For every available serial port an attempt is made to open a connection at a range of common baudrates.
        /// The connection is tested by sending the query string passed to this method.
        /// </para>
        /// <para>
        /// The connected device is expected to respond by sending the reply string passed to this method.
        /// When the string received is equal to the expected reply string, the connection is regarded to be valid.
        /// </para>
        /// </remarks>
        /// <example>
        /// The Arduino sketch below can be used to demonstrate this method.
        /// Upload the sketch to your Arduino device.
        /// <code lang="Arduino Sketch">
        /// char query[] = "Hello?";
        /// char reply[] = "Arduino!";
        ///
        /// void setup()
        /// {
        ///   Serial.begin(9600);
        ///   while (!Serial) {}
        /// }
        ///
        /// void loop()
        /// {
        ///   if (Serial.find(query))
        ///   {
        ///     Serial.println(reply);
        ///   }
        ///   else
        ///   {
        ///     Serial.println("Listening...");
        ///     Serial.flush();
        ///   }
        ///
        ///   delay(25);
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="IStringProtocol"/>
        public static IDataConnection Find(string query, string expectedReply)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException(Messages.ArgumentEx_NotNullOrEmpty, "query");

            if (string.IsNullOrEmpty(expectedReply))
                throw new ArgumentException(Messages.ArgumentEx_NotNullOrEmpty, "expectedReply");

            Func<ArduinoSession, bool> isAvailableFunc = session =>
            {
                session.Write(query);
                return session.Read(expectedReply.Length) == expectedReply;
            };

            string[] portNames = GetPortNames();
            var connection = FindConnection(isAvailableFunc, portNames, PopularBaudRates);
            return connection ?? FindConnection(isAvailableFunc, portNames, OtherBaudRates);
        }
        #endregion

        #region Private Methods

        private void OnSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(sender/*, new SerialDataReceivedEventArgs((SerialData)e.EventType)*/);
        }
        private void OnSerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new Exception("串口异常");
        }
        private static IDataConnection FindConnection(Func<ArduinoSession, bool> isDeviceAvailable, string[] portNames, SerialBaudRate[] baudRates)
        {
            bool found = false;

            for (int x = portNames.Length - 1; x >= 0; x--)
            {
                foreach (var rate in baudRates)
                {
                    try
                    {
                        using (var connection = new SerialConnection(portNames[x], rate))
                        {
                            using (var session = new ArduinoSession(connection, 100))
                            {
                                Debug.WriteLine("{0}:{1}; ", portNames[x], (int)rate);

                                if (isDeviceAvailable(session))
                                    found = true;
                            }
                        }

                        if (found)
                            return new SerialConnection(portNames[x], rate);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Port is not available.
                        Debug.WriteLine("{0} NOT AVAILABLE; ", portNames[x]);
                        break;
                    }
                    catch (TimeoutException)
                    {
                        // Baudrate or protocol error.
                    }
                    catch (IOException ex)
                    {
                        Debug.WriteLine($"HResult 0x{ex.HResult:X} - {ex.Message}");
                    }
                }
            }
            return null;
        }
        private static string GetLastPortName()
        {
            return (from p in SerialConnection.GetPortNames()
                    where (p.StartsWith(@"/dev/ttyUSB") || p.StartsWith(@"/dev/ttyAMA") || p.StartsWith(@"/dev/ttyACM") || p.StartsWith("COM"))
                    orderby p descending
                    select p).First();
        }
        #endregion
    }
}
