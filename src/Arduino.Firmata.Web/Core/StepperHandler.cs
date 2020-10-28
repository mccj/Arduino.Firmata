using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arduino.Firmata.Protocol.AccelStepper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Arduino.Firmata.Web
{
    public abstract class StepperHandlerBase
    {
        public abstract string PortName { get; }
        public abstract int BaudRate { get; }
        private IDataConnection GetConnection()
        {
            //Console.WriteLine("正在搜索Arduino连接...");
            var connection = new Serial.SerialConnection(PortName, BaudRate);

            //EnhancedSerialConnection.Find();

            if (connection == null)
                //Console.WriteLine("找不到连接。请把您的Arduino板连接到USB端口。");
                throw new Exception("找不到连接。请把您的Arduino板连接到USB端口。");
            //else
            //    Console.WriteLine($"以 {connection.BaudRate} 波特率连接到 {connection.PortName} 端口。");

            return connection;
        }

        private ArduinoSession arduinoSession;
        public ArduinoSession GetArduinoSession()
        {
            if (arduinoSession == null)
            {
                var connection = GetConnection();
                var session = new ArduinoSession(connection);
                var firmware = session.GetFirmware();
                if (firmware.MajorVersion >= 2)
                {
                    //步进电机
                    session.CreateStepperMoveCompleteMonitor().Monitor(f =>
                    {
                        GetStepperStep(f.DeviceNumber).Invoke(f.DeviceNumber);
                    });
                    session.CreateReceivedStringMonitor().Monitor(f =>
                    {
                    });

                    session.StepperConfiguration(0, new DeviceConfig
                    {
                        MotorInterface = DeviceConfig.MotorInterfaceType.Driver,
                        StepOrPin1Number = 26,
                        DirectionOrPin2Number = 28,
                        EnablePinNumber = 24,
                        InvertEnablePinNumber = true
                    });
                    session.StepperEnable(0, true);
                    session.StepperSetSpeed(0, 32767);
                    session.StepperSetScceleration(0, 5000);
                    arduinoSession = session;
                }
                else
                {
                    session.Dispose();
                    throw new Exception("读取设备失败");
                }
            }
            return arduinoSession;
        }


        private System.Collections.Concurrent.ConcurrentDictionary<int, StepperStep> keyValuePairs = new System.Collections.Concurrent.ConcurrentDictionary<int, StepperStep>();
        public StepperStep GetStepperStep(int deviceNumber)
        {
            return keyValuePairs.GetOrAdd(deviceNumber, f => new StepperStep());
        }
    }
    public class StepperStep
    {
        private Action<StepperStep, int> nextStep;

        public StepperStep Clear()
        {
            nextStep = null;
            return this;
        }

        public StepperStep SetStep(Action<StepperStep, int> p)
        {
            nextStep = p;
            return this;
        }

        public void Invoke(int deviceNumber)
        {
            nextStep?.Invoke(this, deviceNumber);
        }
    }

    public class StepperHandler : StepperHandlerBase
    {
        public override string PortName => "COM4";
        public override int BaudRate => 57600;

        private static StepperHandler stepperHandler;
        public static StepperHandler GetStepperHandler()
        {
            if (stepperHandler == null) stepperHandler = new StepperHandler();
            return stepperHandler;
        }

        public void StepperMove(int deviceNumber, int speed, int notSpeed = 10000)
        {
            var session = GetArduinoSession();

            session.StepperEnable(deviceNumber, false);//启动电机
            session.StepperMove(deviceNumber, speed);

            GetStepperStep(deviceNumber)
            .Clear()
            .SetStep((f, n) =>
            {
                session.StepperMove(deviceNumber, speed > 0 ? -notSpeed : notSpeed);
                f.Clear().SetStep((f1, n1) =>
                {
                    session.StepperEnable(deviceNumber, true);//停止电机
                    //var ss1 = session.请求报告步进位置(deviceNumber);
                    session.StepperZero(deviceNumber);//清零
                    //var ss2 = session.请求报告步进位置(deviceNumber);
                });
            });
        }
    }
}
