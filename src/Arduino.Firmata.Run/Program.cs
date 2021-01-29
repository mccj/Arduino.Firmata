using Arduino.Firmata;
using Arduino.Firmata.Protocol.AccelStepper;
using Arduino.Firmata.Protocol.EEPROM;
using Arduino.Firmata.Protocol.Firmata;
using Arduino.Firmata.Protocol.NeoPixel;
using Arduino.Firmata.Protocol.Serial;
using Arduino.Firmata.Serial;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Solid.Arduino.Run
{
    class Program
    {
        static void Main(string[] args)
        {
            //var ss1 = NumberExtensions.encode8BitSignedByte(247);
            //var ss2 = NumberExtensions.decode32BitSignedInteger(0x77,0x1,0x0,0x0,0x0);
            //var s=  NumberExtensions.decode32BitSignedInteger(0, 20, 0, 0, 0);
            //var _serial = new System.IO.Ports.SerialPort("COM9", 57600/*, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One*/);

            //_serial.DataReceived += (a, b) =>
            //{
            //    while (_serial.BytesToRead > 0)
            //    {
            //        var bb = _serial.ReadByte();
            //        Debug.Write(string.Format("{0:x2} ", bb));
            //    }
            //};
            ////_serial.ErrorReceived += OnSerialPortErrorReceived;

            //_serial.Open();
            //_serial.Write(new byte[] { 0xf9 }, 0, 1);

            //Console.ReadKey();
            //TcpListener();
            //return;
            //DisplayPortCapabilities();
            //TimeTest();


            //var connection1 = GetTcpConnection();
            //var connection2 = GetTcpConnection();
            //var session1 = new ArduinoSession(connection1);
            //var session2 = new ArduinoSession(connection2);
            //var f1 = session1.GetFirmware();
            //var f2 = session2.GetFirmware();





            var connection = GetSerialConnection();
            //var connection = GetTcpConnection();
            if (connection != null)
            {
                //connection.Open();
                //connection.Close();
                //connection.Open();
                //connection.Close();
                //SimpelTest(connection);
                //StepperTest(connection, session => { session.SetCNCShieldV3Board(); }, 4);
                //StepperTest(connection, session => { session.SetRAMPSBoard(); }, 5);

                //AnalogPinTest(connection);
                //DigitalPinTest(connection);
                //EEPROMTest(connection);
                NeoPixelTest(connection);

                //StepperTest(connection, session =>
                //{
                //    session.StepperConfiguration(0, new DeviceConfig
                //    {
                //        MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                //        StepOrPin1Number = 5,
                //        DirectionOrPin2Number = 6,
                //        //EnablePinNumber = 4,
                //        //InvertEnablePinNumber = false
                //    });
                //    //    session.StepperConfiguration(1, new DeviceConfig
                //    //    {
                //    //        MotorInterface = DeviceConfig.MotorInterfaceType.Driver,

                //    //        StepOrPin1Number = 4,
                //    //        DirectionOrPin2Number = 5,
                //    //        //EnablePinNumber = 8,
                //    //        //InvertEnablePinNumber = false
                //    //    });
                //    //    session.StepperConfiguration(2, new DeviceConfig
                //    //    {
                //    //        MotorInterface = DeviceConfig.MotorInterfaceType.Driver,

                //    //        StepOrPin1Number = 8,
                //    //        DirectionOrPin2Number = 9,
                //    //        //EnablePinNumber = 10,
                //    //        //InvertEnablePinNumber = false
                //    //    });
                //    //    session.StepperConfiguration(3, new DeviceConfig
                //    //    {
                //    //        MotorInterface = DeviceConfig.MotorInterfaceType.Driver,

                //    //        StepOrPin1Number = 10,
                //    //        DirectionOrPin2Number = 11,
                //    //        //EnablePinNumber = 12,
                //    //        InvertStepOrPin1Number = true,
                //    //        //InvertEnablePinNumber = false
                //    //    });
                //    //    session.StepperConfiguration(4, new DeviceConfig
                //    //    {
                //    //        MotorInterface = DeviceConfig.MotorInterfaceType.Driver,

                //    //        StepOrPin1Number = 12,
                //    //        DirectionOrPin2Number = 47,
                //    //        //EnablePinNumber = 12,
                //    //        //InvertEnablePinNumber = false
                //    //    });
                //}, 1);

                //来回移动(connection, session =>
                //{
                //    session.StepperConfiguration(0, new DeviceConfig
                //    {
                //        MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                //        StepOrPin1Number = 27,
                //        DirectionOrPin2Number = 28,
                //        //EnablePinNumber = 4,

                //        InvertEnablePinNumber = true,
                //        //InvertDirectionOrPin2Number=true
                //    });
                //}, 0, 2, false, 3, false, 1000);

                //来回移动(connection, session =>
                //{
                //    session.StepperConfiguration(0, new DeviceConfig
                //    {
                //        MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                //        //StepOrPin1Number = 27,
                //        //DirectionOrPin2Number = 28,
                //        ////EnablePinNumber = 4,

                //        StepOrPin1Number = 4,
                //        DirectionOrPin2Number = 5,
                //        //EnablePinNumber = 8,
                //        InvertEnablePinNumber = true,
                //        //InvertDirectionOrPin2Number=true
                //    });
                //}, 0, 18, true, 19, false, 1000);

                //来回移动(connection, session =>
                //{
                //    session.StepperConfiguration(0, new DeviceConfig
                //    {
                //        MotorInterface = DeviceConfig.MotorInterfaceType.Driver,

                //        StepOrPin1Number = 8,
                //        DirectionOrPin2Number = 9,
                //        //EnablePinNumber = 10,
                //        InvertEnablePinNumber = true,
                //        //InvertDirectionOrPin2Number=true
                //    });
                //}, 0, 18, true, 19, false, 1000);
                //PerformBasicTest(connection);
                //test1(connection);
                //SerialTest(connection);
            }
            Console.WriteLine("Press a key");
            Console.ReadKey(true);
        }

        private static IDataConnection GetSerialConnection()
        {
            Console.WriteLine("正在搜索Arduino连接...");
            //var connection = new SerialConnection("COM9", 57600);
            var connection = new SerialConnection("COM4", 115200);

            //EnhancedSerialConnection.Find();

            if (connection == null)
                Console.WriteLine("找不到连接。请把您的Arduino板连接到USB端口。");
            else
                Console.WriteLine($"以 {connection.BaudRate} 波特率连接到 {connection.PortName} 端口。");

            connection.Open();
            System.Threading.Thread.Sleep(1000 * 4);
            return connection;
        }
        private static IDataConnection GetTcpConnection()
        {
            Console.WriteLine("正在搜索Arduino连接...");
            //var connection = new SerialConnection("COM4", 57600);
            var connection = new global::Arduino.Firmata.Tcp.TcpClientConnection("10.11.201.82", 3030);

            //EnhancedSerialConnection.Find();

            if (connection == null)
                Console.WriteLine("找不到连接。请把您的Arduino板连接到USB端口。");
            else
                Console.WriteLine($"成功连接到 {connection.Name} 端口。");

            return connection;
        }
        private static void TcpListener()
        {
            string ip = "0.0.0.0";
            int port = 30300;

            var tcp = new global::Arduino.Firmata.Tcp.TcpServerConnection()
            {
                AddArduinoSession = (a, d) =>
                {
                    Console.WriteLine("新链接:" + d.Connection);
                },
                RemoveArduinoSession = (a, d, s) =>
                {
                    Console.WriteLine("断开链接:" + d.Connection + s);
                }
            };
            tcp.Start(ip, port);
            Console.WriteLine("按任意建停止");
            Console.ReadKey();
            tcp.Stop();
        }
        private static void EEPROMTest(IDataConnection connection)
        {
            FirmataMessageHeader.RegisterMessage<EEPROMSysExMessage>();
            var session = new ArduinoSession(connection, timeOut: 5000);

            session.CreateReceivedStringMonitor().Monitor(f =>
            {
                Console.WriteLine($"字符串输出 {f}");
            });

            session.ResetBoard();
            var firmware = session.GetFirmware();//获取固件信息
            var protocolVersion = session.GetProtocolVersion();//获取协议信息

            var s1 = session.EEPROM_Length();
            session.EEPROM_Write(247, 247);
            var s2 = session.EEPROM_Read(247);
            session.EEPROM_Update(8, 222);
            var s3 = session.EEPROM_Read(8);
            var bytes = new byte[45/*s1-512*/];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(i % 255);
            }
            session.EEPROM_Put(0, bytes);
            var s4 = session.EEPROM_Get(0,/* bytes.Length*/s1);
        }
        private static void NeoPixelTest(IDataConnection connection)
        {
            FirmataMessageHeader.RegisterMessage<NeoPixelSysExMessage>();
            var session = new ArduinoSession(connection, timeOut: 5000);

            session.CreateReceivedStringMonitor().Monitor(f =>
            {
                Console.WriteLine($"字符串输出 {f}");
            });

            session.ResetBoard();
            var firmware = session.GetFirmware();//获取固件信息
            var protocolVersion = session.GetProtocolVersion();//获取协议信息

            //session.NeoPixelConfiguration(2, 5120, 6);
            //session.NeoPixelConfiguration(1, 64);
            session.NeoPixelConfiguration(1);
            session.NeoPixelSetPin(1, 6);
            session.NeoPixelUpdateLength(1, 4960);
            session.NeoPixelUpdateType(1, NeoPixelType.NEO_GRB | NeoPixelType.NEO_KHZ800);
            session.NeoPixelBegin(1);
            session.NeoPixelClear(1);
            session.NeoPixelSetPixelColor(1, 1, 240, 0, 0);//13
            session.NeoPixelSetPixelColor(1, 2, 0, 247, 0, 10);//15
            session.NeoPixelSetPixelColor(1, 3, System.Drawing.Color.White.ToArgb());//12
            session.NeoPixelFill(1, System.Drawing.Color.DarkBlue, 4, 4);
            session.NeoPixelSetBrightness(1, 240);
            session.NeoPixelShow(1);
            var s1_0 = session.NeoPixelGetBrightness(1);
            session.NeoPixelClear(1);
            var mmm = new[] { 5,6,7,8,9,10,11,12,19,24,25,26,27,28,29,30,31,35,40,41,42,43,44,45,46,47,51,59,60,61,62,63};

            //mmm.AsParallel().ForAll(item =>
            //{
            //    session.NeoPixelSetPixelColor(1, item, System.Drawing.Color.Blue.ToArgb());//12
            //});
            foreach (var item in mmm)
            {
                session.NeoPixelSetPixelColor(1, item, System.Drawing.Color.Blue.ToArgb());//12
            }
            session.NeoPixelSetBrightness(1, 20);
            session.NeoPixelShow(1);

            var s = System.Drawing.Color.Red.ToArgb();
            var s1 = session.NeoPixelGetBrightness(1);
            var s2 = session.NeoPixelCanShow(1);
            var s3 = session.NeoPixelGetPin(1);
            var s4 = session.NeoPixelNumPixels(1);

            var s5 = session.NeoPixelGetPixelColor(1, 2);
            var s6 = session.NeoPixelSine8(1, 5);
            var s7 = session.NeoPixelGamma8(1, 105);
            var s8 = session.NeoPixelGamma32(1, 105);
            var s9 = session.NeoPixelColor(1, 255, 0, 0);
            var s10 = session.NeoPixelColor(1, 255, 0, 0, 5);
            //var s11 = session.NeoPixelColorHSV(1, 5, 5, 5);

            //session.NeoPixelSetPixelColor(0, 8, s9.Value);
            //session.NeoPixelShow(0);
        }
        private static void test1(IDataConnection connection)
        {
            var session = new ArduinoSession(connection, timeOut: 5000);
            session.ResetBoard();
            var firmware = session.GetFirmware();//获取固件信息
            var protocolVersion = session.GetProtocolVersion();//获取协议信息
            var ys = new[] { 27, 28, 4, 5, 8, 9, 10, 11, 12, 47, 48, 49, 24, 23, 38, 14, 15, 16, 17 };
            foreach (var item in ys)
            {
                session.SetDigitalPinMode(item, PinMode.DigitalOutput);//设置引脚模式
                //session.SetDigitalPin(item, true);//设置输出
                session.SetDigitalPin(item, false);//设置输出
            }
            var xs = new[] { 2, 3, 18, 19, 29, 39, 30, 31, 32, 33, 34, 35, 36, 37, 40, 41 };
            foreach (var item in xs)
            {
                session.SetDigitalPinMode(item, PinMode.DigitalInput);//设置引脚模式
                //session.SetDigitalPin(item, true);//设置输出
                //session.SetDigitalPin(item, false);//设置输出
            }
        }
        private static void SerialTest(IDataConnection connection)
        {
            var session = new ArduinoSession(connection, timeOut: 5000);
            session.ResetBoard();
            session.MessageReceived += Session_OnMessageReceived;
            var firmware = session.GetFirmware();//获取固件信息
            var protocolVersion = session.GetProtocolVersion();//获取协议信息

            session.CreateSerialReplyMonitor().Monitor((a) =>
            {
                Console.WriteLine(a.Serial + " " + System.Text.Encoding.UTF8.GetString(a.Data));
            });
            //session.SerialConfig(HW_SERIAL.HW_SERIAL0, 9600);
            //session.SerialWrite(HW_SERIAL.HW_SERIAL0, "123456789123456789");
            //session.SerialRead(HW_SERIAL.HW_SERIAL0);
            //session.SerialListen(SW_SERIAL.SW_SERIAL0);
            //session.SerialConfig(HW_SERIAL.HW_SERIAL1, 9600/*, 16, 17*/);
            //session.SerialWrite(HW_SERIAL.HW_SERIAL1, "123456789123456789");
            //session.SerialRead(HW_SERIAL.HW_SERIAL1);
            //session.SerialListen(SW_SERIAL.SW_SERIAL1);
            //session.SerialConfig(HW_SERIAL.HW_SERIAL2, 9600/*, 0, 1*/);
            //session.SerialWrite(HW_SERIAL.HW_SERIAL2, "123456789123456789");
            //session.SerialRead(HW_SERIAL.HW_SERIAL2);
            //session.SerialListen(SW_SERIAL.SW_SERIAL2);
            session.SerialConfig(HW_SERIAL.HW_SERIAL3, 9600);
            //session.SerialWrite(HW_SERIAL.HW_SERIAL3, "1");
            session.SerialRead(HW_SERIAL.HW_SERIAL3, true, 3);
            session.SerialListen(SW_SERIAL.SW_SERIAL3);


        }
        private static void AnalogPinTest(IDataConnection connection)
        {
            var session = new ArduinoSession(connection, timeOut: 5000);

            //session.ResetBoard();
            //session.MessageReceived += Session_OnMessageReceived;
            var firmware = session.GetFirmware();//获取固件信息
            Console.WriteLine($"固件: {firmware.Name} 版本 {firmware.MajorVersion}.{firmware.MinorVersion}");
            var protocolVersion = session.GetProtocolVersion();//获取协议信息
            Console.WriteLine($"Firmata协议版本 {protocolVersion.Major}.{protocolVersion.Minor}");

            BoardCapability cap = session.GetBoardCapability();//开发板引脚支持的功能
            Console.WriteLine();
            Console.WriteLine("开发板引脚支持的功能:");

            foreach (var pin in cap.Pins)
            {
                Console.WriteLine("引脚(Pin) {0}: 输入(Input): {1}, 输出(Output): {2}, (模拟信号)Analog: {3}, Analog-Res: {4}, 脉冲信号(PWM): {5}, PWM-Res: {6}, 伺服(Servo): {7}, Servo-Res: {8}, 串口(Serial): {9}, (编码器)Encoder: {10}, 输入上拉模式(Input-pullup): {11}",
                    pin.PinNumber,
                    pin.DigitalInput,
                    pin.DigitalOutput,
                    pin.Analog,
                    pin.AnalogResolution,
                    pin.Pwm,
                    pin.PwmResolution,
                    pin.Servo,
                    pin.ServoResolution,
                    pin.Serial,
                    pin.Encoder,
                    pin.InputPullup);
            }
            Console.WriteLine();

            var analogMapping = session.GetBoardAnalogMapping();//模拟信道映射
            Console.WriteLine("模拟信道映射:");

            foreach (var mapping in analogMapping.PinMappings)
            {
                Console.WriteLine("通道 {0} 映射到管脚 {1}.", mapping.Channel, mapping.PinNumber);
            }


            session.ResetBoard();

            //模拟信号控制
            //session.SetDigitalPinMode(13, PinMode.PwmOutput);//设置引脚模式
            //session.SetDigitalPin(13, 0);//设置输出
            //session.SetDigitalPinMode(9, PinMode.PwmOutput);//设置引脚模式
            //session.SetDigitalPin(9, 0);//设置输出


            session.SetDigitalPinMode(0, PinMode.AnalogInput);//设置引脚模式
            session.SetDigitalPinMode(1, PinMode.AnalogInput);//设置引脚模式
            session.SetDigitalPinMode(2, PinMode.AnalogInput);//设置引脚模式

            //session.CreateAnalogStateMonitor().Subscribe(new eeee2("无"));//设置监控调用
            session.CreateAnalogStateChangeMonitor().Monitor(f =>
            {
                Console.WriteLine("通道 {0} 值 {1}", f.Channel, f.Level);
            });//设置数字信号输入监控调用

            session.SetAnalogReportMode(0, true);
            session.SetAnalogReportMode(1, true);
            session.SetAnalogReportMode(2, true);

            ////session.CreateDigitalStateMonitor().PortStateChange(f =>
            ////{
            ////    Console.WriteLine("端口 {0} Pin {1} 的数字电平: {2} 是否第一次初始化 {3}", f.Port, f.PinNumber, f.Value ? 'X' : 'O', f.InitChange);
            ////});//设置数字信号输入监控调用
            //session.CreateDigitalStateChangeMonitor().Monitor(f =>
            //{
            //    Console.WriteLine("端口 {0} Pin {1} 的数字电平: {2} 是否第一次初始化 {3}", f.Port, f.PinNumber, f.Value ? 'X' : 'O', f.InitChange);
            //});//设置数字信号输入监控调用

            //session.SetDigitalReportMode(0, true);//设置监控报告
            //session.SetDigitalReportMode(1, true);//设置监控报告
            //System.Threading.Thread.Sleep(1000);
            //session.CreateDigitalStateMonitor().Subscribe(new eeee1("无"));//设置数字信号输入监控调用


            while (true)
            {
                Console.WriteLine("请输入数字，Q退出");
                var r = Console.ReadLine();
                if (r.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                //if (r.Equals("Z", StringComparison.OrdinalIgnoreCase))
                //{
                //    for (int i = 0; i <= stepperCount; i++)
                //    {
                //        session.StepperZero(i);
                //        session.请求报告步进位置(i);
                //    }
                //}
                //else
                //{
                //    int.TryParse(r, out var n);
                //    for (int i = 0; i <= stepperCount; i++)
                //    {
                //        session.StepperMove(i, n);
                //    }
                //}
            }
        }
        private static void DigitalPinTest(IDataConnection connection)
        {
            var session = new ArduinoSession(connection, timeOut: 5000);

            //session.ResetBoard();
            //session.MessageReceived += Session_OnMessageReceived;
            var firmware = session.GetFirmware();//获取固件信息
            Console.WriteLine($"固件: {firmware.Name} 版本 {firmware.MajorVersion}.{firmware.MinorVersion}");
            var protocolVersion = session.GetProtocolVersion();//获取协议信息
            Console.WriteLine($"Firmata协议版本 {protocolVersion.Major}.{protocolVersion.Minor}");
            session.ResetBoard();

            session.SetDigitalPinMode(8, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(7, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(6, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(5, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(4, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(3, PinMode.DigitalInput);//设置引脚模式


            //session.CreateDigitalStateMonitor().PortStateChange(f =>
            //{
            //    Console.WriteLine("端口 {0} Pin {1} 的数字电平: {2} 是否第一次初始化 {3}", f.Port, f.PinNumber, f.Value ? 'X' : 'O', f.InitChange);
            //});//设置数字信号输入监控调用
            session.CreateDigitalStateChangeMonitor().Monitor(f =>
            {
                Console.WriteLine("端口 {0} Pin {1} 的数字电平: {2} 是否第一次初始化 {3}", f.Port, f.PinNumber, f.Value ? 'X' : 'O', f.InitChange);
            });//设置数字信号输入监控调用

            session.SetDigitalReportMode(0, true);//设置监控报告
            session.SetDigitalReportMode(1, true);//设置监控报告
                                                  //System.Threading.Thread.Sleep(1000);
                                                  //session.CreateDigitalStateMonitor().Subscribe(new eeee1("无"));//设置数字信号输入监控调用


            while (true)
            {
                Console.WriteLine("请输入数字，Q退出");
                var r = Console.ReadLine();
                if (r.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                //if (r.Equals("Z", StringComparison.OrdinalIgnoreCase))
                //{
                //    for (int i = 0; i <= stepperCount; i++)
                //    {
                //        session.StepperZero(i);
                //        session.请求报告步进位置(i);
                //    }
                //}
                //else
                //{
                //    int.TryParse(r, out var n);
                //    for (int i = 0; i <= stepperCount; i++)
                //    {
                //        session.StepperMove(i, n);
                //    }
                //}
            }
        }


        private static void 来回移动(IDataConnection connection, Action<ArduinoSession> action, int stepperNo, int a1, bool b1, int a2, bool b2, int iii)
        {
            iii = int.MaxValue;
            var session = new ArduinoSession(connection, timeOut: 5000);

            session.CreateReceivedStringMonitor().Monitor(f =>
            {
                Console.WriteLine($"字符串输出 {f}");
            });

            //session.ResetBoard();
            //session.MessageReceived += Session_OnMessageReceived;
            var firmware = session.GetFirmware();//获取固件信息
            Console.WriteLine($"固件: {firmware.Name} 版本 {firmware.MajorVersion}.{firmware.MinorVersion}");
            var protocolVersion = session.GetProtocolVersion();//获取协议信息
            Console.WriteLine($"Firmata协议版本 {protocolVersion.Major}.{protocolVersion.Minor}");
            session.ResetBoard();



            session.SetDigitalPinMode(2, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(3, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(18, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(19, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(29, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(39, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(30, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(31, PinMode.DigitalInput);//设置引脚模式


            //session.CreateDigitalStateMonitor().PortStateChange(f =>
            //{
            //    Console.WriteLine("端口 {0} Pin {1} 的数字电平: {2} 是否第一次初始化 {3}", f.Port, f.PinNumber, f.Value ? 'X' : 'O', f.InitChange);
            //});//设置数字信号输入监控调用
            //session.CreateDigitalStateChangeMonitor().Monitor(f =>
            //{
            //    Console.WriteLine("端口 {0} Pin {1} 的数字电平: {2} 是否第一次初始化 {3}", f.Port, f.PinNumber, f.Value ? 'X' : 'O', f.InitChange);
            //});//设置数字信号输入监控调用

            session.SetDigitalReportMode(0, true);//设置监控报告
            session.SetDigitalReportMode(1, true);
            session.SetDigitalReportMode(2, true);
            session.SetDigitalReportMode(3, true);
            session.SetDigitalReportMode(4, true);
            session.SetDigitalReportMode(5, true);
            session.SetDigitalReportMode(6, true);
            session.SetDigitalReportMode(7, true);
            session.SetDigitalReportMode(8, true);


            //var bb1 = true;
            //var bb2 = true;
            ////步进电机
            //session.CreateStepperMoveCompleteMonitor().Monitor(f =>
            //{
            //    System.Threading.Thread.Sleep(1000);
            //    bb2 = !bb2;
            //    session.StepperMove(stepperNo, bb2 ? -iii : iii);
            //    Console.WriteLine("步进电机: {0}, 驱动器编号: {1},驱动器: {2}", f, f.DeviceNumber, f.StepsNum);
            //});

            session.CreateDigitalStateChangeMonitor().Monitor(f =>
            {
                if (f.PinNumber == a1 && f.Value == b1)
                {
                    session.StepperZero(stepperNo);
                    System.Threading.Thread.Sleep(1000);
                    session.StepperMove(stepperNo, iii);
                }
                else if (f.PinNumber == a2 && f.Value == b2)
                {
                    session.StepperZero(stepperNo);
                    System.Threading.Thread.Sleep(1000);
                    session.StepperMove(stepperNo, -iii);
                }
                Console.WriteLine("端口 {0} Pin {1} 的数字电平: {2} 是否第一次初始化 {3}", f.Port, f.PinNumber, f.Value ? 'X' : 'O', f.InitChange);
            });
            //Console.ReadKey();



            //session.CreateStepperMoveCompleteMonitor().Subscribe(new eeee3("步进完成"));
            session.CreateStepperPositionMonitor().Subscribe(new eeee3("步进汇报"));

            //session.SetRAMPSBoard();
            //session.SetCNCShieldV3Board();
            action(session);

            session.StepperEnable(stepperNo, true);
            session.StepperEnable(stepperNo, false);
            //session.StepperSetSpeed(i, 32767);
            //session.StepperSetSpeed(i, 32767/12);
            session.StepperSetSpeed(stepperNo, 32767 / 30);
            session.StepperSetScceleration(stepperNo, 5000 * 2);


            Console.WriteLine("按任意键开始");
            Console.ReadKey();


            session.StepperMove(stepperNo, iii);

            Console.WriteLine("按q键退出");

            while (true)
            {
                var key = Console.ReadKey().KeyChar.ToString();
                if (key.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    session.StepperZero(stepperNo);
                    return;
                }
            }
        }

        private static void StepperTest(IDataConnection connection, Action<ArduinoSession> action, int stepperCount)
        {
            var session = new ArduinoSession(connection, timeOut: 5000);

            session.CreateReceivedStringMonitor().Monitor(f =>
            {
                Console.WriteLine($"字符串输出 {f}");
            });

            //session.ResetBoard();
            //session.MessageReceived += Session_OnMessageReceived;
            var firmware = session.GetFirmware();//获取固件信息
            Console.WriteLine($"固件: {firmware.Name} 版本 {firmware.MajorVersion}.{firmware.MinorVersion}");
            var protocolVersion = session.GetProtocolVersion();//获取协议信息
            Console.WriteLine($"Firmata协议版本 {protocolVersion.Major}.{protocolVersion.Minor}");
            session.ResetBoard();

            //session.CreateDigitalStateChangeMonitor().Monitor(f =>
            //{
            //    Console.WriteLine("端口 {0} Pin {1} 的数字电平: {2} 是否第一次初始化 {3}", f.Port, f.PinNumber, f.Value ? 'X' : 'O', f.InitChange);
            //});//设置数字信号输入监控调用

            //session.SetDigitalPinMode(2, PinMode.DigitalInput);//设置引脚模式
            //session.SetDigitalPinMode(3, PinMode.DigitalInput);//设置引脚模式
            //session.SetDigitalPinMode(18, PinMode.DigitalInput);//设置引脚模式
            //session.SetDigitalPinMode(19, PinMode.DigitalInput);//设置引脚模式
            //session.SetDigitalPinMode(29, PinMode.DigitalInput);//设置引脚模式
            //session.SetDigitalPinMode(39, PinMode.DigitalInput);//设置引脚模式
            //session.SetDigitalPinMode(30, PinMode.DigitalInput);//设置引脚模式
            //session.SetDigitalPinMode(31, PinMode.DigitalInput);//设置引脚模式

            ////session.CreateDigitalStateMonitor().Subscribe(new eeee1("无"));//设置数字信号输入监控调用
            //session.SetDigitalReportMode(0, true);//设置监控报告
            //session.SetDigitalReportMode(1, true);
            //session.SetDigitalReportMode(2, true);
            //session.SetDigitalReportMode(3, true);
            //session.SetDigitalReportMode(4, true);
            //session.SetDigitalReportMode(5, true);
            //session.SetDigitalReportMode(6, true);
            //session.SetDigitalReportMode(7, true);
            //session.SetDigitalReportMode(8, true);


            session.SetDigitalPinMode(6, PinMode.DigitalOutput);//设置引脚模式
            session.SetDigitalPin(6, true);
            session.SetDigitalPin(6, false);

            ////步进电机
            session.CreateReceivedStringMonitor().Monitor(f =>
            {

            });
            session.CreateStepperMoveCompleteMonitor().Subscribe(new eeee3("步进完成"));
            session.CreateStepperPositionMonitor().Subscribe(new eeee3("步进汇报"));

            //session.SetRAMPSBoard();
            //session.SetCNCShieldV3Board();
            action(session);
            for (int i = 0; i < stepperCount; i++)
            {
                session.StepperEnable(i, true);
                session.StepperEnable(i, false);
                //session.StepperSetSpeed(i, 32767);
                //session.StepperSetSpeed(i, 32767/12);
                session.StepperSetSpeed(i, 32767 / 10);
                session.StepperSetScceleration(i, 5000 / 5);
            }


            while (true)
            {
                Console.WriteLine("请输入数字，Q退出");
                var r = Console.ReadLine();
                if (r.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    for (int i = 0; i < stepperCount; i++)
                    {
                        session.StepperEnable(i, true);
                    }
                    return;
                }
                if (r.Equals("Z", StringComparison.OrdinalIgnoreCase))
                {
                    for (int i = 0; i < stepperCount; i++)
                    {
                        session.StepperZero(i);
                        session.请求报告步进位置(i);
                    }
                }
                else
                {
                    int.TryParse(r, out var n);
                    for (int i = 0; i < stepperCount; i++)
                    {
                        session.StepperMove(i, n);
                    }
                }
            }
        }

        private static void PerformBasicTest(IDataConnection connection)
        {
            var session = new ArduinoSession(connection);
            session.ResetBoard();

            session.MessageReceived += Session_OnMessageReceived;
            session.EvintFirmata().DigitalStateReceived += Session_OnDigitalStateReceived;//接收到数字引脚状态
            session.EvintFirmata().AnalogStateReceived += Session_OnAnalogStateReceived;//接收到数字引脚状态

            var firmware = session.GetFirmware();//获取固件信息
            Console.WriteLine($"固件: {firmware.Name} 版本 {firmware.MajorVersion}.{firmware.MinorVersion}");
            var protocolVersion = session.GetProtocolVersion();//获取协议信息
            Console.WriteLine($"Firmata协议版本 {protocolVersion.Major}.{protocolVersion.Minor}");


            //数字信号控制
            //session.SetDigitalPort(1, 0b_1111_1111);//将给定端口的数字输出引脚设置为低或高
            //session.SetDigitalPort(1, 0b_0000_0000);//将给定端口的数字输出引脚设置为低或高
            session.SetDigitalPinMode(13, PinMode.DigitalOutput);//设置引脚模式
            session.SetDigitalPin(13, true);//设置输出
            Console.WriteLine("Command sent: Light on (pin 10)");
            var pinState1_1 = session.GetPinState(13);//获取引脚状态

            Console.WriteLine("Press a key");
            //Console.ReadKey(true);
            session.SetDigitalPin(13, false);//设置输出
            Console.WriteLine("Command sent: Light off");
            var pinState1_2 = session.GetPinState(13);//获取引脚状态

            //模拟信号控制
            session.SetDigitalPinMode(13, PinMode.PwmOutput);//设置引脚模式
            session.SetDigitalPin(13, 255);//设置输出
            Console.WriteLine("Command sent: Light on (pin 10)");
            var pinState2_1 = session.GetPinState(13);//获取引脚状态
            Console.WriteLine("Press a key");
            //Console.ReadKey(true);
            session.SetDigitalPin(13, 0);//设置输出
            Console.WriteLine("Command sent: Light off");
            var pinState2_2 = session.GetPinState(13);//获取引脚状态



            // 信号读取
            //session.SetSamplingInterval(500);
            //数字信号读取

            session.SetDigitalPinMode(2, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(3, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(15, PinMode.DigitalInput);//设置引脚模式
            session.SetDigitalPinMode(14, PinMode.DigitalInput);//设置引脚模式

            session.CreateDigitalStateMonitor().Subscribe(new eeee1("无"));//设置监控调用
            //session.CreateDigitalStateMonitor(0).Subscribe(new eeee("0"));
            //session.CreateDigitalStateMonitor(1).Subscribe(new eeee("1"));

            session.SetDigitalReportMode(0, true);//设置监控报告
            session.SetDigitalReportMode(1, true);

            //模拟信号读取
            session.SetDigitalPinMode(13, PinMode.AnalogInput);//设置引脚模式
            session.SetDigitalPinMode(14, PinMode.AnalogInput);//设置引脚模式
            session.SetDigitalPinMode(15, PinMode.AnalogInput);//设置引脚模式

            session.CreateAnalogStateMonitor().Subscribe(new eeee2("无"));//设置监控调用

            session.SetAnalogReportMode(13, true);
            session.SetAnalogReportMode(14, true);
            session.SetAnalogReportMode(15, true);

            //var sss1 = session.GetBoardAnalogMapping();
            //session.RequestBoardAnalogMapping();

            ////步进电机
            session.CreateStepperMoveCompleteMonitor().Subscribe(new eeee3("步进完成"));
            session.CreateStepperPositionMonitor().Subscribe(new eeee3("步进汇报"));
            session.StepperConfiguration(0, new DeviceConfig
            {
                MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                StepOrPin1Number = 26,
                DirectionOrPin2Number = 28,
                EnablePinNumber = 24,
                InvertEnablePinNumber = true
            });
            session.StepperEnable(0, true);
            session.StepperEnable(0, false);
            session.StepperSetSpeed(0, 32767);
            session.StepperSetScceleration(0, 5000);
            var ss = session.请求报告步进位置(0);
            Console.ReadKey(true);
            session.StepperMove(0, 10000);
            Console.ReadKey(true);
            session.StepperMove(0, -10000);
            Console.ReadKey(true);
            session.StepperEnable(0, true);

            Console.ReadKey(true);


            var cap = session.GetBoardCapability();//开发板引脚支持的功能
            foreach (var pincap in cap.Pins.Where(c => (c.DigitalInput || c.DigitalOutput) && !c.Analog))
            {
                var pinState = session.GetPinState(pincap.PinNumber);//获取数字引脚状态
                Console.WriteLine("引脚(Pin) {0}: 模式(Mode) = {1}, 值(Value) = {2}", pincap.PinNumber, pinState.Mode, pinState.Value);
            }


            ////伺服电机
            //session.SetDigitalPinMode(11, PinMode.ServoControl);
            //session.SetDigitalPin(11, 90);//设置数字引脚
            //Thread.Sleep(500);
            //int hi = 0;

            //for (int a = 0; a <= 179; a += 1)
            //{
            //    session.SetDigitalPin(11, a);
            //    Thread.Sleep(100);
            //    session.SetDigitalPort(1, hi);
            //    hi ^= 4;
            //    Console.Write("{0};", a);
            //}



            Console.ReadKey(true);
        }



        public class eeee1 : IObserver<DigitalPortState>
        {
            public eeee1(string v)
            {
                V = v;
            }

            public string V { get; }

            public void OnCompleted()
            {
                //throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                //throw new NotImplementedException();
            }
            private bool?[] sss = new bool?[1000];
            public void OnNext(DigitalPortState value)
            {
                for (int i = 0; i < 8; i++)
                {
                    var d1 = value.Port * 8 + i;
                    var v = value.IsSet(i);
                    if (sss[d1] != v)
                    {
                        sss[d1] = v;
                        Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(i) ? 'X' : 'O', i, value.Pins);
                    }
                }



                //Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(0) ? 'X' : 'O', 0, value.Pins);
                //Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(1) ? 'X' : 'O', 1, value.Pins);
                //Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(2) ? 'X' : 'O', 2, value.Pins);
                //Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(3) ? 'X' : 'O', 3, value.Pins);
                //Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(4) ? 'X' : 'O', 4, value.Pins);
                //Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(5) ? 'X' : 'O', 5, value.Pins);
                //Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(6) ? 'X' : 'O', 6, value.Pins);
                //Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(7) ? 'X' : 'O', 7, value.Pins);
            }
        }
        public class eeee2 : IObserver<AnalogState>
        {
            public eeee2(string v)
            {
                V = v;
            }

            public string V { get; }

            public void OnCompleted()
            {
                //throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                //throw new NotImplementedException();
            }

            public void OnNext(AnalogState value)
            {
                Console.WriteLine("B_引脚 {0} 的模拟电平: {1}", value.Channel, value.Level);
            }
        }
        public class eeee3 : IObserver<StepperPosition>
        {
            public eeee3(string v)
            {
                V = v;
            }

            public string V { get; }

            public void OnCompleted()
            {
                //throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                //throw new NotImplementedException();
            }

            public void OnNext(StepperPosition value)
            {
                Console.WriteLine("步进电机: {0}, 驱动器编号: {1},驱动器: {2}", V, value.DeviceNumber, value.StepsNum);
            }
        }

        //private static void DisplayPortCapabilities()
        //{
        //    using (var session = new ArduinoSession(new EnhancedSerialConnection("COM5", SerialBaudRate.Bps_57600)))
        //    {
        //        BoardCapability cap = session.GetBoardCapability();
        //        Console.WriteLine();
        //        Console.WriteLine("Board Capability:");

        //        foreach (var pin in cap.Pins)
        //        {
        //            Console.WriteLine("Pin {0}: Input: {1}, Output: {2}, Analog: {3}, Analog-Res: {4}, PWM: {5}, PWM-Res: {6}, Servo: {7}, Servo-Res: {8}, Serial: {9}, Encoder: {10}, Input-pullup: {11}",
        //                pin.PinNumber,
        //                pin.DigitalInput,
        //                pin.DigitalOutput,
        //                pin.Analog,
        //                pin.AnalogResolution,
        //                pin.Pwm,
        //                pin.PwmResolution,
        //                pin.Servo,
        //                pin.ServoResolution,
        //                pin.Serial,
        //                pin.Encoder,
        //                pin.InputPullup);
        //        }
        //    }
        //}

        //private static void TimeTest()
        //{
        //    var session = new ArduinoSession(new SerialConnection("COM5", SerialBaudRate.Bps_57600)) { TimeOut = 1000 };
        //    session.MessageReceived += Session_OnMessageReceived;

        //    var firmata = (II2CProtocol)session;
        //    var x = firmata.GetI2CReply(0x68, 7);

        //    Console.WriteLine();
        //    Console.WriteLine("{0} bytes received.", x.Data.Length);
        //    Console.WriteLine("Starting");
        //    Console.WriteLine("Press a key to abort.");
        //    Console.ReadKey(true);

        //    session.Dispose();
        //}

        static void Session_OnMessageReceived(object sender, FirmataMessageEventArgs eventArgs)
        {
            if (eventArgs.Value.Value is global::Arduino.Firmata.Protocol.String.StringData aaa)
            {

            }
            //string o;

            //switch (eventArgs.Value.Type)
            //{
            //    case MessageType.StringData:
            //        o = ((StringData)eventArgs.Value.Value).Text;
            //        Console.WriteLine("Message {0} received: {1}", eventArgs.Value.Type, o);
            //        break;

            //    default:
            //        o = "?";
            //        break;
            //}

            //Console.WriteLine("Message {0} received: {1}", eventArgs.Value.Type, o);
        }

        static void SimpelTest(IDataConnection connection)
        {
            var session = new ArduinoSession(connection, timeOut: 2500);
            var firmata = session;

            firmata.EvintFirmata().AnalogStateReceived += Session_OnAnalogStateReceived;//接收到模拟引脚状态
            firmata.EvintFirmata().DigitalStateReceived += Session_OnDigitalStateReceived;//接收到数字引脚状态

            Firmware firm = firmata.GetFirmware();//获取固件版本
            Console.WriteLine();
            Console.WriteLine("固件: {0} {1}.{2}", firm.Name, firm.MajorVersion, firm.MinorVersion);
            Console.WriteLine();

            ProtocolVersion version = firmata.GetProtocolVersion();//获取传输协议版本
            Console.WriteLine();
            Console.WriteLine("协议版本: {0}.{1}", version.Major, version.Minor);
            Console.WriteLine();

            BoardCapability cap = firmata.GetBoardCapability();//开发板引脚支持的功能
            Console.WriteLine();
            Console.WriteLine("开发板引脚支持的功能:");

            foreach (var pin in cap.Pins)
            {
                Console.WriteLine("引脚(Pin) {0}: 输入(Input): {1}, 输出(Output): {2}, (模拟信号)Analog: {3}, Analog-Res: {4}, 脉冲信号(PWM): {5}, PWM-Res: {6}, 伺服(Servo): {7}, Servo-Res: {8}, 串口(Serial): {9}, (编码器)Encoder: {10}, 输入上拉模式(Input-pullup): {11}",
                    pin.PinNumber,
                    pin.DigitalInput,
                    pin.DigitalOutput,
                    pin.Analog,
                    pin.AnalogResolution,
                    pin.Pwm,
                    pin.PwmResolution,
                    pin.Servo,
                    pin.ServoResolution,
                    pin.Serial,
                    pin.Encoder,
                    pin.InputPullup);
            }
            Console.WriteLine();

            var analogMapping = firmata.GetBoardAnalogMapping();//模拟信道映射
            Console.WriteLine("模拟信道映射:");

            foreach (var mapping in analogMapping.PinMappings)
            {
                Console.WriteLine("通道 {0} 映射到管脚 {1}.", mapping.Channel, mapping.PinNumber);
            }

            firmata.ResetBoard();//重置开发板

            Console.WriteLine();
            Console.WriteLine("数字端口状态:");

            foreach (var pincap in cap.Pins.Where(c => (c.DigitalInput || c.DigitalOutput) && !c.Analog))
            {
                var pinState = firmata.GetPinState(pincap.PinNumber);//获取数字引脚状态
                Console.WriteLine("引脚(Pin) {0}: 模式(Mode) = {1}, 值(Value) = {2}", pincap.PinNumber, pinState.Mode, pinState.Value);
            }
            Console.WriteLine();

            firmata.SetDigitalPort(0, 0x04);//设置数字端口
            firmata.SetDigitalPort(1, 0xff);
            firmata.SetDigitalPinMode(10, PinMode.DigitalOutput);//设置数字引脚模式
            firmata.SetDigitalPinMode(11, PinMode.ServoControl);
            firmata.SetDigitalPin(11, 90);//设置数字引脚
            Thread.Sleep(500);
            int hi = 0;

            for (int a = 0; a <= 179; a += 1)
            {
                firmata.SetDigitalPin(11, a);
                Thread.Sleep(100);
                firmata.SetDigitalPort(1, hi);
                hi ^= 4;
                Console.Write("{0};", a);
            }
            Console.WriteLine();
            Console.WriteLine();

            firmata.SetDigitalPinMode(3, PinMode.DigitalInput);

            //firmata.SetDigitalPortState(2, 255);//设置数字端口状态
            //firmata.SetDigitalPortState(3, 255);

            firmata.SetSamplingInterval(500);//设置采样间隔
            firmata.SetAnalogReportMode(0, false);//设置模拟报告模式

            Console.WriteLine("设置数字报告模式:");
            firmata.SetDigitalReportMode(0, true);//设置数字报告模式
            //firmata.SetDigitalReportMode(1, true);
            //firmata.SetDigitalReportMode(2, true);
            //firmata.SetDigitalReportMode(3, true);
            Console.WriteLine();

            foreach (var pinCap in cap.Pins.Where(c => (c.DigitalInput || c.DigitalOutput) && !c.Analog))
            {
                PinState state = firmata.GetPinState(pinCap.PinNumber);
                Console.WriteLine("Digital {1} 引脚(pin) {0}: {2}", state.PinNumber, state.Mode, state.Value);
            }
            Console.WriteLine();

            Console.ReadLine();
            //firmata.SetAnalogReportMode(0, false);
            //firmata.SetDigitalReportMode(0, false);
            //firmata.SetDigitalReportMode(1, false);
            //firmata.SetDigitalReportMode(2, false);
            Console.WriteLine("Ready.");
            Console.ReadKey(true);
        }

        static void Session_OnDigitalStateReceived(object sender, FirmataEventArgs<DigitalPortState> eventArgs)
        {
            Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(0) ? 'X' : 'O', 0, eventArgs.Value.Pins);
            Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(1) ? 'X' : 'O', 1, eventArgs.Value.Pins);
            Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(1) ? 'X' : 'O', 1, eventArgs.Value.Pins);
            Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(2) ? 'X' : 'O', 2, eventArgs.Value.Pins);
            Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(3) ? 'X' : 'O', 3, eventArgs.Value.Pins);
            Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(4) ? 'X' : 'O', 4, eventArgs.Value.Pins);
            Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(6) ? 'X' : 'O', 6, eventArgs.Value.Pins);
            Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(7) ? 'X' : 'O', 7, eventArgs.Value.Pins);
            //Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(8) ? 'X' : 'O', 8, eventArgs.Value.Pins);
            //Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(9) ? 'X' : 'O', 9, eventArgs.Value.Pins);
            //Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(10) ? 'X' : 'O', 10, eventArgs.Value.Pins);
            //Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(11) ? 'X' : 'O', 11, eventArgs.Value.Pins);
            //Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(12) ? 'X' : 'O', 12, eventArgs.Value.Pins);
            //Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(13) ? 'X' : 'O', 13, eventArgs.Value.Pins);
            //Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(14) ? 'X' : 'O', 14, eventArgs.Value.Pins);
            //Console.WriteLine("端口 {0} 的数字电平: {1}-{2}-{3}", eventArgs.Value.Port, eventArgs.Value.IsSet(15) ? 'X' : 'O', 15, eventArgs.Value.Pins);
            Console.WriteLine("----------------------");
        }

        static void Session_OnAnalogStateReceived(object sender, FirmataEventArgs<AnalogState> eventArgs)
        {
            Console.WriteLine("引脚 {0} 的模拟电平: {1}", eventArgs.Value.Channel, eventArgs.Value.Level);
        }

    }
}
