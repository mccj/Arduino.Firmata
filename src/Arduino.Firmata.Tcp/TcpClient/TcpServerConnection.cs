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
        private System.Threading.Tasks.Task _task2;
        private System.Collections.Concurrent.ConcurrentDictionary<string, ArduinoSession> dic = new System.Collections.Concurrent.ConcurrentDictionary<string, ArduinoSession>();
        public void Start(string ip = "0.0.0.0", int port = 30300, int backlog = 10)
        {
            if (socket?.IsBound == true) throw new Exception("已经运行");

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


                        var connection = new TcpServerClientConnection(tSocket);
                        var session = new ArduinoSession(connection, timeOut: 5000);

                        //var ss = session.GetFirmware();

                        dic.AddOrUpdate(connection.Name, f => session, (f1, f2) => session);

                        System.Threading.Tasks.Task.Run(() =>
                        {
                            try
                            {
                                AddArduinoSession?.Invoke(this, session);
                            }
                            catch (Exception) { }
                        });
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            });
            _task2 = System.Threading.Tasks.Task.Run(async () =>
            {
                while (socket.IsBound)
                {
                    foreach (var item in dic.Values)
                    {
                        if (item.Connection.IsOpen)
                        {
                            try
                            {
                                _ = await item.GetProtocolVersionAsync();
                            }
                            catch (Exception ex)
                            {
                                item.Connection.Close();
                                dic.TryRemove(item.Connection.Name, out var _);
                                try
                                {
                                    RemoveArduinoSession?.Invoke(this, item, "心跳超时");
                                }
                                catch (Exception) { }
                            }
                        }
                        else
                        {
                            item.Connection.Close();
                            dic.TryRemove(item.Connection.Name, out var _);
                            try
                            {
                                RemoveArduinoSession?.Invoke(this, item, "连接断开");
                            }
                            catch (Exception) { }
                        }
                    }
                    System.Threading.Thread.Sleep(TimeSpan);
                }
            });
        }
        /// <summary>
        /// 心跳时间
        /// </summary>
        public TimeSpan TimeSpan { get; set; } = new TimeSpan(0, 0, 1);
        public void Stop()
        {
            socket?.Close();
            socket?.Dispose();
            //_task.Dispose();
            _task = null;
            _task2 = null;
            dic.Clear();
        }

        public bool IsRuning => socket?.IsBound ?? false;
        public ArduinoSession[] GetArduinoSessions()
        {
            return dic.Values.ToArray();
        }
        public Action<object, ArduinoSession> AddArduinoSession { private get; set; }
        public Action<object, ArduinoSession, string> RemoveArduinoSession { private get; set; }
    }
}
