using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace Arduino.Firmata.Tcp
{
    public class TcpServerConnection
    {
        private Socket socket;
        private System.Threading.Tasks.Task _task;
        private Dictionary<string, ArduinoSession> dic = new Dictionary<string, ArduinoSession>();
        public void Start(string ip = "0.0.0.0", int port = 30300, int backlog = 10)
        {
            if (socket.IsBound) throw new Exception("已经运行");

            var endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endpoint);
            //同一个时间点过来10个客户端，排队
            socket.Listen(backlog);

            _task = System.Threading.Tasks.Task.Run(() =>
            {
                while (socket.IsBound)
                {
                    try
                    {
                        //创建通信用的Socket
                        Socket tSocket = socket.Accept();


                        string point = tSocket.RemoteEndPoint.ToString();
                        var connection = new TcpServerClientConnection(tSocket);
                        var session = new ArduinoSession(connection, timeOut: 5000);

                        //var ss = session.GetFirmware();

                        dic.Add(point, session);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            });
        }
        public void Stop()
        {
            socket?.Close();
            socket?.Dispose();
            //_task.Dispose();
            _task = null;
            dic.Clear();
        }

        public bool IsRuning => socket?.IsBound ?? false;
    }
}
