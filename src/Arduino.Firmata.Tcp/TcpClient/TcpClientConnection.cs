using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Arduino.Firmata.Tcp
{
    public class TcpClientConnection : TcpClientConnectionBase
    {
        #region Fields
        private readonly EndPoint _endPoint;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of <see cref="MicrosoftSerialConnection"/> class on the given serial port and at the given baud rate.
        /// </summary>
        /// <param name="hostNameOrAddress">A string that contains an IP address in dotted-quad notation for IPv4 and in colon-hexadecimal notation for IPv6.</param>
        /// <param name="port">The port number associated with the address, or 0 to specify any available port. port is in host order.</param>
        public TcpClientConnection(string hostNameOrAddress, int port = 3030)
        {
            var addresses = Dns.GetHostAddresses(hostNameOrAddress);
            _endPoint = new IPEndPoint(addresses[0], port);
        }
        public TcpClientConnection(IPEndPoint iPEndPoint)
        {
            _endPoint = iPEndPoint;
        }

        #endregion
        #region Public Methods & Properties
        public override string Name => _endPoint?.ToString();
        public override void Open()
        {
            if (IsOpen)
                return;

            var _tcpClient = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _tcpClient.Connect(_endPoint);
                this.SetSocket(_tcpClient);
            }
            catch (UnauthorizedAccessException)
            {
                // Connection closure has probably not yet been finalized.
                // Wait 250 ms and try again once.
                Thread.Sleep(250);
                _tcpClient.Connect(_endPoint);
                this.SetSocket(_tcpClient);
            }
        }
        #endregion
        public override string ToString()
        {
            return $"{ nameof(TcpServerClientConnection) } {this.Name}";
        }
    }
}
