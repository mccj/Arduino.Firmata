using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Arduino.Firmata.Tcp
{
    public class TcpConnection : IDataConnection
    {
        #region Fields
        private readonly TcpClient _tcpClient;
        private readonly IPEndPoint _endPoint;
        private /*readonly*/ NetworkStream _networkStream;
        private /*readonly*/ System.Threading.Tasks.Task _task;

        private const int DefaultTimeoutMs = 100;
        private bool _isDisposed;

        #endregion

        #region Constructors
        public TcpConnection(string ipString, int port) : this(new IPEndPoint(IPAddress.Parse(ipString), port)) { }
        /// <summary>
        /// Initializes a new instance of <see cref="MicrosoftSerialConnection"/> class on the given serial port and at the given baud rate.
        /// </summary>
        /// <param name="ipString">A string that contains an IP address in dotted-quad notation for IPv4 and in colon-hexadecimal notation for IPv6.</param>
        /// <param name="port">The port number associated with the address, or 0 to specify any available port. port is in host order.</param>
        public TcpConnection(IPEndPoint iPEndPoint)
        {
            _tcpClient = new TcpClient();
            _endPoint = iPEndPoint;

            _tcpClient.ReceiveTimeout = DefaultTimeoutMs;
            _tcpClient.SendTimeout = DefaultTimeoutMs;

            //_networkStream = _tcpClient.GetStream();
            //_task = System.Threading.Tasks.Task.Run(() =>
            //{
            //    while (true)
            //    {

            //    }
            //});
            //DataReceived += OnSerialPortDataReceived;
            //ErrorReceived += OnSerialPortErrorReceived;
        }

        #endregion
        #region Public Methods & Properties
        public int InfiniteTimeout => DefaultTimeoutMs;
        public event DataReceivedEventHandler DataReceived;
        public string Name => _endPoint.ToString();
        //public string PortName => this.Name;
        //public int BaudRate => _endPoint.Port;
        public bool IsOpen => _tcpClient.Connected;
        public string NewLine { get; set; } = "\r";
        public int BytesToRead => _networkStream?.DataAvailable == true ? 1 : 0;
        //public bool CanRead => _networkStream.CanRead;

        public void Open()
        {
            if (IsOpen)
                return;

            try
            {
                _tcpClient.Connect(_endPoint);

                // My observation on the Raspberry Pi was, that the serial port (Arduino connected on /dev/ttyUSB0) already had data
                // in its buffer, even though the connected Arduino hadn't sent anything.
                // The data in the buffer appeared to be from a previous run of the program.
                // So apparently the buffer is remembered even across consecutive runs of the program and after the serial port is closed.
                // This means that the serial port OS driver is at fault here, as imo it should have cleared 
                // its buffer after a Close or directly when Opened.
                // NOPE: RawDump on Rpi shows that data is correctly received, so no buffers are "remembered".

                _networkStream?.Flush();

                _networkStream = _tcpClient.GetStream();

                _task = System.Threading.Tasks.Task.Run(() =>
                {
                    while (_tcpClient.Connected)
                    {
                        OnTcpDataReceived();
                    }
                });
            }
            catch (UnauthorizedAccessException)
            {
                // Connection closure has probably not yet been finalized.
                // Wait 250 ms and try again once.
                Thread.Sleep(250);
                _tcpClient.Connect(_endPoint);
            }
        }

        public void Close()
        {
            if (!IsOpen)
                return;

            Thread.Sleep(250);
            _networkStream?.Flush();
            _networkStream.Close();
            _networkStream.Dispose();
            _tcpClient.Close();

            //_task?.Dispose();
            _task = null;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            //DataReceived -= OnSerialPortDataReceived;
            //_tcpClient.ErrorReceived -= OnSerialPortErrorReceived;

            _tcpClient.Dispose();

            _networkStream.Dispose();

            _task?.Dispose();
            _task = null;

            GC.SuppressFinalize(this);
        }

        public int ReadByte()
        {
            if (_tcpClient.Connected)
                return _networkStream?.ReadByte() ?? 0;
            else
                return 0;
        }

        public void Write(string text)
        {
            if (_tcpClient.Connected)
            {
                var bytes = System.Text.Encoding.ASCII.GetBytes(text);
                _networkStream?.Write(bytes, 0, bytes.Length);
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (_tcpClient.Connected)
                _networkStream?.Write(buffer, offset, count);
        }

        public void WriteLine(string text)
        {
            this.WriteLine(text + this.NewLine);
        }

        //public static string[] GetPortNames() => SerialPort.GetPortNames();
        ///// <summary>
        ///// Finds a serial connection to a device supporting the Firmata protocol.
        ///// </summary>
        ///// <returns>A <see cref="IDataConnection"/> instance or <c>null</c> if no connection is found</returns>
        ///// <remarks>
        ///// <para>
        ///// This method searches all available serial ports until it finds a working serial connection.
        ///// For every available serial port an attempt is made to open a connection at a range of common baudrates.
        ///// The connection is tested by issueing an <see cref="IFirmataProtocol.GetFirmware()"/> command.
        ///// (I.e. a Firmata SysEx Firmware query (0xF0 0x79 0xF7).)
        ///// </para>
        ///// <para>
        ///// The connected device is expected to respond by sending the version number of the supported protocol.
        ///// When a major version of 2 or higher is received, the connection is regarded to be valid.
        ///// </para>
        ///// </remarks>
        ///// <seealso cref="IFirmataProtocol"/>
        ///// <seealso href="http://www.firmata.org/wiki/Protocol#Query_Firmware_Name_and_Version">Query Firmware Name and Version</seealso>
        //public static IDataConnection Find()
        //{
        //    Func<ArduinoSession, bool> isAvailableFunc = session =>
        //    {
        //        var firmware = session.GetFirmware();
        //        return firmware.MajorVersion >= 2;
        //    };

        //    string[] portNames = GetPortNames();
        //    var connection = FindConnection(isAvailableFunc, portNames, PopularBaudRates);
        //    return connection ?? FindConnection(isAvailableFunc, portNames, OtherBaudRates);
        //}

        ///// <summary>
        ///// Finds a serial connection to a device supporting plain serial communications.
        ///// </summary>
        ///// <param name="query">The query text used to inquire the connection</param>
        ///// <param name="expectedReply">The reply text the connected device is expected to respond with</param>
        ///// <returns>A <see cref="IDataConnection"/> instance or <c>null</c> if no connection is found</returns>
        ///// <remarks>
        ///// <para>
        ///// This method searches all available serial ports until it finds a working serial connection.
        ///// For every available serial port an attempt is made to open a connection at a range of common baudrates.
        ///// The connection is tested by sending the query string passed to this method.
        ///// </para>
        ///// <para>
        ///// The connected device is expected to respond by sending the reply string passed to this method.
        ///// When the string received is equal to the expected reply string, the connection is regarded to be valid.
        ///// </para>
        ///// </remarks>
        ///// <example>
        ///// The Arduino sketch below can be used to demonstrate this method.
        ///// Upload the sketch to your Arduino device.
        ///// <code lang="Arduino Sketch">
        ///// char query[] = "Hello?";
        ///// char reply[] = "Arduino!";
        /////
        ///// void setup()
        ///// {
        /////   Serial.begin(9600);
        /////   while (!Serial) {}
        ///// }
        /////
        ///// void loop()
        ///// {
        /////   if (Serial.find(query))
        /////   {
        /////     Serial.println(reply);
        /////   }
        /////   else
        /////   {
        /////     Serial.println("Listening...");
        /////     Serial.flush();
        /////   }
        /////
        /////   delay(25);
        ///// }
        ///// </code>
        ///// </example>
        ///// <seealso cref="IStringProtocol"/>
        //public static IDataConnection Find(string query, string expectedReply)
        //{
        //    if (string.IsNullOrEmpty(query))
        //        throw new ArgumentException(Messages.ArgumentEx_NotNullOrEmpty, "query");

        //    if (string.IsNullOrEmpty(expectedReply))
        //        throw new ArgumentException(Messages.ArgumentEx_NotNullOrEmpty, "expectedReply");

        //    Func<ArduinoSession, bool> isAvailableFunc = session =>
        //    {
        //        session.Write(query);
        //        return session.Read(expectedReply.Length) == expectedReply;
        //    };

        //    string[] portNames = GetPortNames();
        //    var connection = FindConnection(isAvailableFunc, portNames, PopularBaudRates);
        //    return connection ?? FindConnection(isAvailableFunc, portNames, OtherBaudRates);
        //}
        #endregion

        #region Private Methods

        private void OnTcpDataReceived()
        {
            DataReceived?.Invoke(this/*, new SerialDataReceivedEventArgs((SerialData)e.EventType)*/);
        }
        //private void OnSerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        //{
        //    throw new Exception("串口异常");
        //}
        //private static IDataConnection FindConnection(Func<ArduinoSession, bool> isDeviceAvailable, string[] portNames, SerialBaudRate[] baudRates)
        //{
        //    bool found = false;

        //    for (int x = portNames.Length - 1; x >= 0; x--)
        //    {
        //        foreach (var rate in baudRates)
        //        {
        //            try
        //            {
        //                using (var connection = new SerialConnection(portNames[x], rate))
        //                {
        //                    using (var session = new ArduinoSession(connection, 100))
        //                    {
        //                        Debug.WriteLine("{0}:{1}; ", portNames[x], (int)rate);

        //                        if (isDeviceAvailable(session))
        //                            found = true;
        //                    }
        //                }

        //                if (found)
        //                    return new SerialConnection(portNames[x], rate);
        //            }
        //            catch (UnauthorizedAccessException)
        //            {
        //                // Port is not available.
        //                Debug.WriteLine("{0} NOT AVAILABLE; ", portNames[x]);
        //                break;
        //            }
        //            catch (TimeoutException)
        //            {
        //                // Baudrate or protocol error.
        //            }
        //            catch (IOException ex)
        //            {
        //                Debug.WriteLine($"HResult 0x{ex.HResult:X} - {ex.Message}");
        //            }
        //        }
        //    }
        //    return null;
        //}
        //private static string GetLastPortName()
        //{
        //    return (from p in SerialConnection.GetPortNames()
        //            where (p.StartsWith(@"/dev/ttyUSB") || p.StartsWith(@"/dev/ttyAMA") || p.StartsWith(@"/dev/ttyACM") || p.StartsWith("COM"))
        //            orderby p descending
        //            select p).First();
        //}
        #endregion
    }
}
