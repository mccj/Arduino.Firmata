using Arduino.Firmata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Solid.Arduino.Firmata
{
    public class DeviceConfig
    {
        //public DeviceConfig()
        //{
        //}
        //public DeviceConfig(MotorInterfaceType motorInterface,int stepOrPin1Number,int directionOrPin2Number)
        //{
        //    MotorInterface = motorInterface;
        //    StepOrPin1Number = stepOrPin1Number;
        //    DirectionOrPin2Number = directionOrPin2Number;
        //}
        public MotorInterfaceType MotorInterface { get; set; }
        public int StepOrPin1Number { get; set; }
        public int DirectionOrPin2Number { get; set; }
        public int? Pin3Number { get; set; }
        public int? Pin4Number { get; set; }
        public int? EnablePinNumber { get; set; }
        public bool InvertStepOrPin1Number { get; set; }
        public bool InvertDirectionOrPin2Number { get; set; }
        public bool InvertPin3Number { get; set; }
        public bool InvertPin4Number { get; set; }
        public bool InvertEnablePinNumber { get; set; }

        public enum MotorInterfaceType
        {
            Driver,//步进驱动器，需要2个驱动器引脚
            Full2Wire,//2线步进电机，需要2个电机引脚
            Full3Wire,//3线步进，如硬盘驱动器主轴，需要3个电机引脚
            Full4Wire,//4线全步进电机，需要4个电机引脚
            Half3Wire,//3线半步进，如硬盘驱动器主轴，需要3个电机引脚
            Half4Wire//4线半步进电机，需要4个电机引脚
        }
    }
}
