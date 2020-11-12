using Arduino.Firmata;
using Arduino.Firmata.Extend;
using Arduino.Firmata.Protocol.AccelStepper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Linq
{
    /// <summary>
    /// https://github.com/firmata/protocol/blob/master/accelStepperFirmata.md
    /// </summary>
    public static class AccelStepperProtocol
    {
        public const byte ACCELSTEPPER_DATA = 0x62; // ACCELSTEPPER_DATA(0x62)
        /// <summary>
        /// 步进器配置
        /// 此消息是必需的，必须在发送任何其他消息之前发送。设备号是任意的，但必须是唯一的。
        /// </summary>
        /// <param name="session"></param>
        public static void StepperConfiguration(this ArduinoSession session, int deviceNumber, DeviceConfig deviceConfig)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");
            if (deviceConfig == null)
                throw new ArgumentNullException(nameof(deviceConfig));
            if (deviceConfig.StepOrPin1Number < 0 || deviceConfig.StepOrPin1Number > 127)
                throw new ArgumentOutOfRangeException(nameof(deviceConfig.StepOrPin1Number), "Pin number must be between 0 and 127.");
            if (deviceConfig.DirectionOrPin2Number < 0 || deviceConfig.DirectionOrPin2Number > 127)
                throw new ArgumentOutOfRangeException(nameof(deviceConfig.DirectionOrPin2Number), "Pin number must be between 0 and 127.");
            if (deviceConfig.Pin3Number.HasValue && (deviceConfig.Pin3Number.Value < 0 || deviceConfig.Pin3Number.Value > 127))
                throw new ArgumentOutOfRangeException(nameof(deviceConfig.Pin3Number), "Pin number must be between 0 and 127.");
            if (deviceConfig.Pin4Number.HasValue && (deviceConfig.Pin4Number.Value < 0 || deviceConfig.Pin4Number.Value > 127))
                throw new ArgumentOutOfRangeException(nameof(deviceConfig.Pin4Number), "Pin number must be between 0 and 127.");
            if (deviceConfig.EnablePinNumber.HasValue && (deviceConfig.EnablePinNumber.Value < 0 || deviceConfig.EnablePinNumber.Value > 127))
                throw new ArgumentOutOfRangeException(nameof(deviceConfig.EnablePinNumber), "Pin number must be between 0 and 127.");



            int motorInterface;
            switch (deviceConfig.MotorInterface)
            {
                case DeviceConfig.MotorInterfaceType.Driver:
                    motorInterface = 0b_001_000_0 | (deviceConfig.EnablePinNumber.HasValue ? 0b_1 : 0b_0);
                    break;
                case DeviceConfig.MotorInterfaceType.Full2Wire:
                    motorInterface = 0b_010_000_0 | (deviceConfig.EnablePinNumber.HasValue ? 0b_1 : 0b_0);
                    break;
                case DeviceConfig.MotorInterfaceType.Full3Wire:
                    motorInterface = 0b_011_000_0 | (deviceConfig.EnablePinNumber.HasValue ? 0b_1 : 0b_0);
                    break;
                case DeviceConfig.MotorInterfaceType.Full4Wire:
                    motorInterface = 0b_100_000_0 | (deviceConfig.EnablePinNumber.HasValue ? 0b_1 : 0b_0);
                    break;
                case DeviceConfig.MotorInterfaceType.Half3Wire:
                    motorInterface = 0b_011_001_0 | (deviceConfig.EnablePinNumber.HasValue ? 0b_1 : 0b_0);
                    break;
                case DeviceConfig.MotorInterfaceType.Half4Wire:
                    motorInterface = 0b_100_001_0 | (deviceConfig.EnablePinNumber.HasValue ? 0b_1 : 0b_0);
                    break;
                default:
                    motorInterface = 0b_001_000_0 | (deviceConfig.EnablePinNumber.HasValue ? 0b_1 : 0b_0);
                    break;
            }
            var command = new List<byte>
            {
                Utility.SysExStart,
                ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
                (byte)0x00,//config command(0x00 = config)      //ACCELSTEPPER_CONFIG
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)

                (byte)motorInterface,//interface

                (byte)deviceConfig.StepOrPin1Number, //motorPin1 or stepPin number                (0-127)
                (byte)deviceConfig.DirectionOrPin2Number,//motorPin2 or directionPin number           (0-127)
                //(byte)0,//[when interface >= 0x011] motorPin3(0-127)
                //(byte)0,//[when interface >= 0x100] motorPin4(0-127)
                //(byte)deviceConfig.EnablePinNumber,//[when interface && 0x0000001] enablePin(0-127)

                //(byte)0b_00_0_0_0_0_0,// [optional] pins to invert 


                //Utility.SysExEnd
            };
            if (deviceConfig.MotorInterface == DeviceConfig.MotorInterfaceType.Full3Wire || deviceConfig.MotorInterface == DeviceConfig.MotorInterfaceType.Half3Wire)
                command.Add((byte)deviceConfig.Pin3Number);
            if (deviceConfig.MotorInterface == DeviceConfig.MotorInterfaceType.Full4Wire || deviceConfig.MotorInterface == DeviceConfig.MotorInterfaceType.Half4Wire)
                command.Add((byte)deviceConfig.Pin4Number);
            if (deviceConfig.EnablePinNumber.HasValue)
                command.Add((byte)deviceConfig.EnablePinNumber);

            command.Add((byte)((deviceConfig.InvertEnablePinNumber ? 1 : 0 << 1) | (deviceConfig.InvertPin4Number ? 1 : 0) << 1 | (deviceConfig.InvertPin3Number ? 1 : 0) << 1 | (deviceConfig.InvertDirectionOrPin2Number ? 1 : 0) << 1 | (deviceConfig.InvertStepOrPin1Number ? 1 : 0)));
            command.Add(Utility.SysExEnd);
            session.Write(command.ToArray(), 0, command.Count);
        }


        /// <summary>
        /// 步进归零
        /// accelStepper将存储步进电机的当前绝对位置（以步为单位）。发送清零命令会将位置值重置为零，而无需移动步进器。
        /// </summary>
        /// <param name="session"></param>
        public static void StepperZero(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
                (byte)0x01,//zero command(0x01)     //ACCELSTEPPER_ZERO
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        /// <summary>
        /// 启动步进电机（相对移动）
        /// 移动步骤指定为32位带符号长。
        /// </summary>
        /// <param name="session"></param>
        public static void StepperMove(this ArduinoSession session, int deviceNumber, int stepsNum)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var bytes = stepsNum.encode32BitSignedInteger();

            var command = new[]
           {
                Utility.SysExStart,
                ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
                (byte)0x02,//step command(0x02)     //ACCELSTEPPER_STEP
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                bytes[0], //4  num steps, bits 0-6
                bytes[1], //5  num steps, bits 7-13
                bytes[2], //6  num steps, bits 14-20
                bytes[3], //7  num steps, bits 21-27
                bytes[4], //8  num steps, bits 28-32
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }


        /// <summary>
        /// 启动步进电机（绝对移动）
        /// 根据从零位置开始的步数将步进器移动到所需位置。位置指定为32位带符号的长整数。
        /// </summary>
        /// <param name="session"></param>
        public static void StepperMoveTo(this ArduinoSession session, int deviceNumber, int stepsNum)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var bytes = stepsNum.encode32BitSignedInteger();

            var command = new[]
            {
                Utility.SysExStart,
                ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
                (byte)0x03,//to command(0x03)       //ACCELSTEPPER_TO
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                bytes[0], //4  num steps, bits 0-6
                bytes[1], //5  num steps, bits 7-13
                bytes[2], //6  num steps, bits 14-20
                bytes[3], //7  num steps, bits 21-27
                bytes[4], //8  num steps, bits 28-32
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        /// <summary>
        /// 步进使能
        /// 对于配置了使能引脚的步进电机控制器，使能命令管理控制器是否将电压传递给电机。步进电机闲置时，仍会消耗电压，因此，如果步进电机不需要保持其位置，请启用以节省功率。
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        public static void StepperEnable(this ArduinoSession session, int deviceNumber, bool enable)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
                (byte)0x04,//enable command(0x04)       //ACCELSTEPPER_ENABLE
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                (byte)(enable ? 1 : 0),//device state(HIGH : enabled | LOW : disabled)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        /// <summary>
        /// 步进停止
        /// 停止步进电机。在结果STEPPER_MOVE_COMPLETE被发送到客户端与电机的位置时停止完成注：如果加速设置，停止将不会立竿见影。
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        public static void StepperStop(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
                (byte)0x05,//stop command(0x05)     //ACCELSTEPPER_STOP
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        /// <summary>
        /// 设定加速度
        /// 以steps / sec ^ 2设置加速/减速。使用下面描述的accelStepperFirmata的自定义浮点格式传递accel值。
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        /// <param name="enable"></param>
        public static void StepperSetScceleration(this ArduinoSession session, int deviceNumber, float acceleration)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var bytes = acceleration.encodeCustomFloat();
            var command = new[]
            {
                Utility.SysExStart,
                ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
                (byte)0x08,//set acceleration command(0x08)     //ACCELSTEPPER_SET_ACCELERATION
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                bytes[0],//4  accel, bits 0-6                         (acceleration in steps/sec^2)
                bytes[1],//5  accel, bits 7-13
                bytes[2],//6  accel, bits 14-20
                bytes[3],//7  accel, bits 21-27
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        /// <summary>
        /// 步进设定速度
        /// 如果加速关闭（等于零），则以每秒为单位设置速度。如果加速打开（非零），则设置每秒的最大速度。使用accelStepperFirmata的自定义浮动格式传递速度值，如下所述。
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        /// <param name="enable"></param>
        public static void StepperSetSpeed(this ArduinoSession session, int deviceNumber, float speed)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var bytes = speed.encodeCustomFloat();
            var command = new[]
            {
                Utility.SysExStart,
                ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
                (byte)0x09,//set speed command(0x09)        //ACCELSTEPPER_SET_SPEED
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                bytes[0],//4  accel, bits 0-6                         (acceleration in steps/sec^2)
                bytes[1],//5  accel, bits 7-13
                bytes[2],//6  accel, bits 14-20
                bytes[3],//7  accel, bits 21-27
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        /// <summary>
        /// 当限制销（数字）设置为限制状态时，将无法沿该方向移动
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        /// <param name="speed"></param>
        public static void 步进极限_尚未实现(this ArduinoSession session, int deviceNumber, float speed)
        {
            throw new NotSupportedException();
            //            if (deviceNumber < 0 || deviceNumber > 9)
            //                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            //            var bytes = encodeCustomFloat(speed);
            //            var command = new[]
            //            {
            //                Utility.SysExStart,
            //                ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
            //                (byte)0x07,//stop limit command(0x07)
            //                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
            //// 4  lower limit pin number                  (0-127)
            //// 5  lower limit state                       (0x00 | 0x01)
            //// 6  upper limit pin number                  (0-127)
            //// 7  upper limit state                       (0x00 | 0x01)
            //                Utility.SysExEnd
            //            };
            //            session.Write(command, 0, command.Length);
        }
        public static void Request请求报告步进位置(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
                (byte)0x06,//report position command(0x06)      //ACCELSTEPPER_REPORT_POSITION
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static StepperPosition 请求报告步进位置(this ArduinoSession session, int deviceNumber)
        {
            session.Request请求报告步进位置(deviceNumber);
            return session.GetMessageFromQueue<StepperPosition>().Value;
        }

        /// <summary>
        /// Asynchronously gets the firmware signature of the party system.
        /// </summary>
        /// <returns>The firmware signature</returns>
        public static async Task<StepperPosition> 请求报告步进位置Async(this ArduinoSession session, int deviceNumber)
        {
            session.Request请求报告步进位置(deviceNumber);
            return await Task.Run(() => session.GetMessageFromQueue<StepperPosition>().Value).ConfigureAwait(false);
        }



        //public static void MultiStepperConfiguration(this ArduinoSession session)
        //{
        //}






        ///// <summary>
        ///// 可以将使用上述stepper configuration命令创建的Stepper实例添加到multiStepper组。可以通过单个命令向组发送设备/位置列表，并且将协调它们的移动以同时开始和结束。请注意，multiStepper不支持加速或减速。
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="deviceNumber"></param>
        ///// <param name="stepsNum"></param>
        //public static void MultiStepper_移动(this ArduinoSession session, int groupNumber, int stepsNum)
        //{
        //    if (groupNumber < 0 || groupNumber > 4)
        //        throw new ArgumentOutOfRangeException(nameof(groupNumber), "Group number must be between 0 and 4.");

        //    var bytes = encode32BitSignedInteger(stepsNum);

        //    var command = new[]
        //   {
        //        Utility.SysExStart,
        //        ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
        //        (byte)0x21,//multi to command(0x21)         //MULTISTEPPER_TO
        //        (byte)groupNumber,//group number(0-4)
        //        bytes[0], //4  num steps, bits 0-6
        //        bytes[1], //5  num steps, bits 7-13
        //        bytes[2], //6  num steps, bits 14-20
        //        bytes[3], //7  num steps, bits 21-27
        //        bytes[4], //8  num steps, bits 28-32
        //        Utility.SysExEnd
        //    };
        //    session.Write(command, 0, command.Length);
        //}
        ///// <summary>
        ///// 立即停止组中的所有步进器。
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="groupNumber"></param>
        ///// <param name="stepsNum"></param>
        //public static void MultiStepper_停止(this ArduinoSession session, int groupNumber)
        //{
        //    if (groupNumber < 0 || groupNumber > 4)
        //        throw new ArgumentOutOfRangeException(nameof(groupNumber), "Group number must be between 0 and 4.");

        //    var command = new[]
        //   {
        //        Utility.SysExStart,
        //        ACCELSTEPPER_DATA,//ACCELSTEPPER_DATA(0x62)
        //        (byte)0x23,//multi stop command(0x23)       //MULTISTEPPER_STOP
        //        (byte)groupNumber,//group number(0-4)
        //        Utility.SysExEnd
        //    };
        //    session.Write(command, 0, command.Length);
        //}

        public static AccelStepperEvint EvintAccelStepper(this ArduinoSession session)
        {
            return session.GetEvint(() => new AccelStepperEvint(session));
        }





        public static IObservable<StepperPosition> CreateStepperMoveCompleteMonitor(this ArduinoSession session)
        {
            return new StepperMoveCompleteTracker(session.EvintAccelStepper());
        }
        public static IObservable<StepperPosition> CreateStepperPositionMonitor(this ArduinoSession session)
        {
            return new StepperPositionTracker(session.EvintAccelStepper());
        }
        public static IObservable<int> CreateMultiStepperMoveCompelteMonitor(this ArduinoSession session)
        {
            return new MultiStepperMoveCompelteTracker(session.EvintAccelStepper());
        }
    }
}
