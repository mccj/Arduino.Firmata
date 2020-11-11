using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Arduino.Firmata.Tcp
{
    public class TcpServerClientConnection : TcpClientConnectionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of <see cref="MicrosoftSerialConnection"/> class on the given serial port and at the given baud rate.
        /// </summary>
        protected internal TcpServerClientConnection(Socket socket)
        {
            try
            {
                this.SetSocket(socket);
            }
            catch (UnauthorizedAccessException)
            {
                // Connection closure has probably not yet been finalized.
                // Wait 250 ms and try again once.
                Thread.Sleep(250);
                this.SetSocket(socket);
            }

            //DataReceived += OnSerialPortDataReceived;
            //ErrorReceived += OnSerialPortErrorReceived;
        }

        #endregion
        #region Public Methods & Properties
        public override void Open()
        {
            throw new Exception("无需 Open");
        }
        public override string ToString()
        {
            return $"{ nameof(TcpServerClientConnection) } {this.Name}";
        }
        #endregion
    }
}
