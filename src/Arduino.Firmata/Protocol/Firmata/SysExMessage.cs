using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arduino.Firmata.Protocol.Firmata
{
    /// <summary>
    /// Defines a serial port connection.
    /// </summary>
    /// <seealso href="http://arduino.cc/en/Reference/Serial">Serial reference for Arduino</seealso>
    public class SysExMessage : ISysExMessage
    {
        public bool CanHeader(byte messageByte)
        {
            var ss = new[] {
                FirmataProtocol.REPORT_FIRMWARE,//FirmwareResponse
                FirmataProtocol.PIN_STATE_RESPONSE,//PinStateResponse
                FirmataProtocol.CAPABILITY_RESPONSE,//CapabilityResponse
                FirmataProtocol.ANALOG_MAPPING_RESPONSE//AnalogMappingResponse
            };
            return ss.Contains(messageByte);
        }

        public IFirmataMessage Header(MessageHeader messageHeader)
        {
            var messageByte = (byte)messageHeader._messageBuffer[1];
            switch (messageByte)
            {
                case FirmataProtocol.REPORT_FIRMWARE:
                    return CreateFirmwareResponse(messageHeader);
                case FirmataProtocol.PIN_STATE_RESPONSE:
                    return CreatePinStateResponse(messageHeader);
                case FirmataProtocol.CAPABILITY_RESPONSE:
                    return CreateCapabilityResponse(messageHeader);
                case FirmataProtocol.ANALOG_MAPPING_RESPONSE:
                    return CreateAnalogMappingResponse(messageHeader);
                default:
                    throw new NotImplementedException();
            }
        }
        private IFirmataMessage CreateFirmwareResponse(MessageHeader messageHeader)
        {
            var firmware = new Firmware
            {
                MajorVersion = messageHeader._messageBuffer[2],
                MinorVersion = messageHeader._messageBuffer[3]
            };

            var builder = new StringBuilder(messageHeader._messageBufferIndex);

            for (int x = 4; x < messageHeader._messageBufferIndex; x += 2)
            {
                builder.Append((char)(messageHeader._messageBuffer[x] | (messageHeader._messageBuffer[x + 1] << 7)));
            }

            firmware.Name = builder.ToString();
            return new FirmataMessage<Firmware>(firmware);
        }
        private IFirmataMessage CreatePinStateResponse(MessageHeader messageHeader)
        {
            if (messageHeader._messageBufferIndex < 5)
                throw new InvalidOperationException(Messages.InvalidOpEx_PinNotSupported);

            int value = 0;

            for (int x = messageHeader._messageBufferIndex - 1; x > 3; x--)
            {
                value = (value << 7) | messageHeader._messageBuffer[x];
            }

            var pinState = new PinState
            {
                PinNumber = messageHeader._messageBuffer[2],
                Mode = (PinMode)messageHeader._messageBuffer[3],
                Value = value
            };
            return new FirmataMessage<PinState>(pinState);
        }
        private IFirmataMessage CreateAnalogMappingResponse(MessageHeader messageHeader)
        {
            var pins = new List<AnalogPinMapping>(8);

            for (int x = 2; x < messageHeader._messageBufferIndex; x++)
            {
                if (messageHeader._messageBuffer[x] != 0x7F)
                {
                    pins.Add
                    (
                        new AnalogPinMapping
                        {
                            PinNumber = x - 2,
                            Channel = messageHeader._messageBuffer[x]
                        }
                    );
                }
            }

            var board = new BoardAnalogMapping { PinMappings = pins.ToArray() };
            return new FirmataMessage<BoardAnalogMapping>(board);
        }
        private IFirmataMessage CreateCapabilityResponse(MessageHeader messageHeader)
        {
            var pins = new List<PinCapability>(12);
            int pinIndex = 0;
            int x = 2;

            while (x < messageHeader._messageBufferIndex)
            {
                if (messageHeader._messageBuffer[x] != 127)
                {
                    var capability = new PinCapability { PinNumber = pinIndex };

                    while (x < messageHeader._messageBufferIndex && messageHeader._messageBuffer[x] != 127)
                    {
                        PinMode pinMode = (PinMode)messageHeader._messageBuffer[x];
                        bool isCapable = (messageHeader._messageBuffer[x + 1] != 0);

                        switch (pinMode)
                        {
                            case PinMode.AnalogInput:
                                capability.Analog = true;
                                capability.AnalogResolution = messageHeader._messageBuffer[x + 1];
                                break;

                            case PinMode.DigitalInput:
                                capability.DigitalInput = true;
                                break;

                            case PinMode.DigitalOutput:
                                capability.DigitalOutput = true;
                                break;

                            case PinMode.PwmOutput:
                                capability.Pwm = true;
                                capability.PwmResolution = messageHeader._messageBuffer[x + 1];
                                break;

                            case PinMode.ServoControl:
                                capability.Servo = true;
                                capability.ServoResolution = messageHeader._messageBuffer[x + 1];
                                break;

                            case PinMode.I2C:
                                capability.I2C = true;
                                break;

                            case PinMode.OneWire:
                                capability.OneWire = true;
                                break;

                            case PinMode.StepperControl:
                                capability.StepperControl = true;
                                capability.MaxStepNumber = messageHeader._messageBuffer[x + 1];
                                break;

                            case PinMode.Encoder:
                                capability.Encoder = true;
                                break;

                            case PinMode.Serial:
                                capability.Serial = true;
                                break;

                            case PinMode.InputPullup:
                                capability.InputPullup = true;
                                break;

                            default:
                                throw new NotImplementedException();
                        }

                        x += 2;
                    }

                    pins.Add(capability);
                }

                pinIndex++;
                x++;
            }

            return new FirmataMessage<BoardCapability>(new BoardCapability { Pins = pins.ToArray() });
        }










        //private FirmataMessage<Firmware> CreateFirmwareResponse(MessageHeader messageHeader)
        //{
        //    var builder = new StringBuilder(messageHeader._messageBufferIndex);

        //    for (int x = 4; x < messageHeader._messageBufferIndex; x += 2)
        //        builder.Append((char)(messageHeader._messageBuffer[x] | (messageHeader._messageBuffer[x + 1] << 7)));

        //    return new FirmataMessage<Firmware>(new Firmware
        //    {
        //        MajorVersion = messageHeader._messageBuffer[2],
        //        MinorVersion = messageHeader._messageBuffer[3],
        //        Name = builder.ToString()
        //    });
        //}
        //private FirmataMessage<PinState> CreatePinStateResponse(MessageHeader messageHeader)
        //{
        //    if (messageHeader._messageBufferIndex < 5)
        //        throw new InvalidOperationException(Messages.InvalidOpEx_PinNotSupported);

        //    int value = 0;

        //    for (int x = messageHeader._messageBufferIndex - 1; x > 3; x--)
        //        value = (value << 7) | messageHeader._messageBuffer[x];

        //    return new FirmataMessage<PinState>(new PinState
        //    {
        //        PinNumber = messageHeader._messageBuffer[2],
        //        Mode = (PinMode)messageHeader._messageBuffer[3],
        //        Value = value
        //    });
        //}
        //private FirmataMessage<BoardCapability> CreateCapabilityResponse(MessageHeader messageHeader)
        //{
        //    var pins = new List<PinCapability>(12);
        //    int pinIndex = 0;
        //    int x = 2;

        //    while (x < messageHeader._messageBufferIndex)
        //    {
        //        if (messageHeader._messageBuffer[x] != 127)
        //        {
        //            var capability = new PinCapability { PinNumber = pinIndex };

        //            while (x < messageHeader._messageBufferIndex && messageHeader._messageBuffer[x] != 127)
        //            {
        //                PinMode pinMode = (PinMode)messageHeader._messageBuffer[x];
        //                bool isCapable = (messageHeader._messageBuffer[x + 1] != 0);

        //                switch (pinMode)
        //                {
        //                    case PinMode.AnalogInput:
        //                        capability.Analog = true;
        //                        capability.AnalogResolution = messageHeader._messageBuffer[x + 1];
        //                        break;

        //                    case PinMode.DigitalInput:
        //                        capability.DigitalInput = true;
        //                        break;

        //                    case PinMode.DigitalOutput:
        //                        capability.DigitalOutput = true;
        //                        break;

        //                    case PinMode.PwmOutput:
        //                        capability.Pwm = true;
        //                        capability.PwmResolution = messageHeader._messageBuffer[x + 1];
        //                        break;

        //                    case PinMode.ServoControl:
        //                        capability.Servo = true;
        //                        capability.ServoResolution = messageHeader._messageBuffer[x + 1];
        //                        break;

        //                    case PinMode.I2C:
        //                        capability.I2C = true;
        //                        break;

        //                    case PinMode.OneWire:
        //                        capability.OneWire = true;
        //                        break;

        //                    case PinMode.StepperControl:
        //                        capability.StepperControl = true;
        //                        capability.MaxStepNumber = messageHeader._messageBuffer[x + 1];
        //                        break;

        //                    case PinMode.Encoder:
        //                        capability.Encoder = true;
        //                        break;

        //                    case PinMode.Serial:
        //                        capability.Serial = true;
        //                        break;

        //                    case PinMode.InputPullup:
        //                        capability.InputPullup = true;
        //                        break;

        //                    default:
        //                        throw new NotImplementedException();
        //                }

        //                x += 2;
        //            }

        //            pins.Add(capability);
        //        }

        //        pinIndex++;
        //        x++;
        //    }

        //    return new FirmataMessage<BoardCapability>(new BoardCapability { Pins = pins.ToArray() });
        //}
        //private FirmataMessage<BoardAnalogMapping> CreateAnalogMappingResponse(MessageHeader messageHeader)
        //{
        //    var pins = new List<AnalogPinMapping>(8);

        //    for (int x = 2; x < messageHeader._messageBufferIndex; x++)
        //    {
        //        if (messageHeader._messageBuffer[x] != 0x7F)
        //        {
        //            pins.Add
        //            (
        //                new AnalogPinMapping
        //                {
        //                    PinNumber = x - 2,
        //                    Channel = messageHeader._messageBuffer[x]
        //                }
        //            );
        //        }
        //    }

        //    return new FirmataMessage<BoardAnalogMapping>(new BoardAnalogMapping { PinMappings = pins.ToArray() });
        //}

    }
}
