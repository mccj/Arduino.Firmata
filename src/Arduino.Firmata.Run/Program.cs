using Arduino.Firmata;
using Solid.Arduino.Firmata;
using System;
using System.Linq;
using System.Threading;

namespace Solid.Arduino.Run
{
    class Program
    {
        static void Main(string[] args)
        {

            //DisplayPortCapabilities();
            //TimeTest();


            var connection = GetConnection();
            SimpelTest(connection);

            //if (connection != null)
            //    using (var session = new ArduinoSession(connection))
            //        PerformBasicTest(session);

            Console.WriteLine("Press a key");
            Console.ReadKey(true);
        }

        private static IDataConnection GetConnection()
        {
            Console.WriteLine("正在搜索Arduino连接...");
            var connection = new global::Arduino.Firmata.Connection.Serial.SerialConnection("COM4", 57600);

            //EnhancedSerialConnection.Find();

            if (connection == null)
                Console.WriteLine("找不到连接。请把您的Arduino板连接到USB端口。");
            else
                Console.WriteLine($"以 {connection.BaudRate} 波特率连接到 {connection.PortName} 端口。");

            return connection;
        }

        private static void PerformBasicTest(ArduinoSession session)
        {
            //session.ResetBoard();
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

            //////步进电机
            //session.CreateStepperMoveCompleteMonitor().Subscribe(new eeee3("步进完成"));
            //session.CreateStepperPositionMonitor().Subscribe(new eeee3("步进汇报"));
            //session.StepperConfiguration(0, new DeviceConfig
            //{
            //    MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
            //    StepOrPin1Number = 26,
            //    DirectionOrPin2Number = 28,
            //    EnablePinNumber = 24,
            //    InvertEnablePinNumber = true
            //});
            //session.StepperEnable(0, true);
            //session.StepperEnable(0, false);
            //session.StepperSetSpeed(0, 32767);
            //session.StepperSetScceleration(0, 5000);
            //session.请求报告步进位置(0);
            //Console.ReadKey(true);
            //session.StepperMove(0, 10000);
            //Console.ReadKey(true);
            //session.StepperMove(0, -10000);
            //Console.ReadKey(true);
            //session.StepperEnable(0, true);

            //Console.ReadKey(true);


            //var cap = session.GetBoardCapability();//开发板引脚支持的功能
            //foreach (var pincap in cap.Pins.Where(c => (c.DigitalInput || c.DigitalOutput) && !c.Analog))
            //{
            //    var pinState = session.GetPinState(pincap.PinNumber);//获取数字引脚状态
            //    Console.WriteLine("引脚(Pin) {0}: 模式(Mode) = {1}, 值(Value) = {2}", pincap.PinNumber, pinState.Mode, pinState.Value);
            //}


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

            public void OnNext(DigitalPortState value)
            {
                Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(0) ? 'X' : 'O', 0, value.Pins);
                Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(1) ? 'X' : 'O', 1, value.Pins);
                Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(1) ? 'X' : 'O', 1, value.Pins);
                Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(2) ? 'X' : 'O', 2, value.Pins);
                Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(3) ? 'X' : 'O', 3, value.Pins);
                Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(4) ? 'X' : 'O', 4, value.Pins);
                Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(6) ? 'X' : 'O', 6, value.Pins);
                Console.WriteLine("A_端口 {0} 的数字电平: {1}-{2}-{3}", value.Port, value.IsSet(7) ? 'X' : 'O', 7, value.Pins);
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
