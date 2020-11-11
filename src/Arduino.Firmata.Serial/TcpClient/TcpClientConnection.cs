using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Arduino.Firmata.Tcp
{
    public abstract class TcpClientConnectionBase : IDataConnection
    {
        #region Fields
        private Socket _tcpClient;
        private /*readonly*/ NetworkStream _networkStream;
        private /*readonly*/ System.Threading.Tasks.Task _task;

        public const int DefaultTimeoutMs = 100;
        private bool _isDisposed;

        #endregion
        #region Constructors
        public TcpClientConnectionBase()
        {
            //DataReceived += OnSerialPortDataReceived;
            //ErrorReceived += OnSerialPortErrorReceived;
        }

        #endregion

        #region Public Methods & Properties
        public int InfiniteTimeout => DefaultTimeoutMs;
        public event DataReceivedEventHandler DataReceived;
        public virtual string Name => _tcpClient?.RemoteEndPoint?.ToString();
        //public string PortName => this.Name;
        //public int BaudRate => _endPoint.Port;
        public bool IsOpen => _tcpClient?.Connected ?? false;
        public string NewLine { get; set; } = "\r";
        public int BytesToRead => _tcpClient?.Available ?? 0;// _networkStream?.DataAvailable == true ? 1 : 0;
        //public bool CanRead => _networkStream.CanRead;

        protected internal void SetSocket(Socket socket)
        {
            this.Close();
            this.Dispose();

            _tcpClient = socket;
            _tcpClient.ReceiveTimeout = DefaultTimeoutMs;
            _tcpClient.SendTimeout = DefaultTimeoutMs;

            // My observation on the Raspberry Pi was, that the serial port (Arduino connected on /dev/ttyUSB0) already had data
            // in its buffer, even though the connected Arduino hadn't sent anything.
            // The data in the buffer appeared to be from a previous run of the program.
            // So apparently the buffer is remembered even across consecutive runs of the program and after the serial port is closed.
            // This means that the serial port OS driver is at fault here, as imo it should have cleared 
            // its buffer after a Close or directly when Opened.
            // NOPE: RawDump on Rpi shows that data is correctly received, so no buffers are "remembered".

            _networkStream?.Flush();

            _networkStream = new NetworkStream(_tcpClient);

            _task = System.Threading.Tasks.Task.Run(() =>
            {
                while (_tcpClient.Connected/* && _tcpClient.Poll(-1, SelectMode.SelectRead)*/)
                {
                    try
                    {
                        OnTcpDataReceived();
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            });
        }
        public abstract void Open();
        //{
        //    if (IsOpen)
        //        return;

        //    try
        //    {
        //        _tcpClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
        //        _tcpClient.ReceiveTimeout = DefaultTimeoutMs;
        //        _tcpClient.SendTimeout = DefaultTimeoutMs;

        //        //_tcpClient.ReceiveAsync

        //        _tcpClient.Connect(_endPoint);


        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        // Connection closure has probably not yet been finalized.
        //        // Wait 250 ms and try again once.
        //        Thread.Sleep(250);
        //        _tcpClient.Connect(_endPoint);
        //    }
        //}

        public void Close()
        {
            if (!IsOpen)
                return;

            Thread.Sleep(250);
            _networkStream?.Flush();
            _networkStream?.Close();
            _networkStream?.Dispose();
            _tcpClient?.Close();

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

            _networkStream?.Dispose();
            _tcpClient?.Dispose();

            _task?.Dispose();
            _task = null;

            GC.SuppressFinalize(this);
        }

        public int ReadByte()
        {
            if (_tcpClient?.Connected == true)
                return _networkStream?.ReadByte() ?? 0;
            else
                return 0;
        }

        public void Write(string text)
        {
            if (_tcpClient?.Connected == true)
            {
                var bytes = System.Text.Encoding.ASCII.GetBytes(text);
                _networkStream?.Write(bytes, 0, bytes.Length);
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (_tcpClient?.Connected == true)
                _networkStream?.Write(buffer, offset, count);
        }

        public void WriteLine(string text)
        {
            this.WriteLine(text + this.NewLine);
        }
        #endregion

        #region Private Methods

        private void OnTcpDataReceived()
        {
            DataReceived?.Invoke(this/*, new SerialDataReceivedEventArgs((SerialData)e.EventType)*/);
        }
        #endregion
    }
}
