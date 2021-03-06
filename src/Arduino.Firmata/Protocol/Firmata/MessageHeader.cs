﻿using Arduino.Firmata.Extend;
using Arduino.Firmata.Protocol.AccelStepper;
using Arduino.Firmata.Protocol.I2C;
using Arduino.Firmata.Protocol.Serial;
using Arduino.Firmata.Protocol.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Arduino.Firmata.Protocol.Firmata
{
    /// <summary>
    /// Defines a serial port connection.
    /// </summary>
    /// <seealso href="http://arduino.cc/en/Reference/Serial">Serial reference for Arduino</seealso>
    public class FirmataMessageHeader //: IDisposable
    {
        private static List<ISysExMessage> sysExMessage = new List<ISysExMessage> {
                new SysExMessage(),
                new I2CSysExMessage(),
                new StringSysExMessage(),
                new AccelStepperSysExMessage(),
                new SerialSysExMessage()
                };

        public static void RegisterMessage(ISysExMessage sysEx)
        {
            if (!sysExMessage.Contains(sysEx))
                sysExMessage.Add(sysEx);
        }
        public static void RegisterMessage<T>() where T : ISysExMessage
        {
            var obj = System.Activator.CreateInstance<T>();
            RegisterMessage(obj);
        }

        private enum MessageHeader1
        {
            AnalogState = 0xE0, // 224
            DigitalState = 0x90, // 144
            SystemExtension = 0xF0,
            ProtocolVersion = 0xF9
        }
        private MessageHeader _messageHeader;
        public void Header(int serialByte, MessageHeader messageHeader)
        {
            _messageHeader = messageHeader;

            var header = (MessageHeader1)(serialByte & 0xF0);

            switch (header)
            {
                case MessageHeader1.AnalogState:
                    messageHeader._processMessage = ProcessAnalogStateMessage;
                    break;

                case MessageHeader1.DigitalState:
                    messageHeader._processMessage = ProcessDigitalStateMessage;
                    break;

                case MessageHeader1.SystemExtension:
                    header = (MessageHeader1)serialByte;

                    switch (header)
                    {
                        case MessageHeader1.SystemExtension:
                            messageHeader._processMessage = ProcessSysExMessage;
                            break;

                        case MessageHeader1.ProtocolVersion:
                            messageHeader._processMessage = ProcessProtocolVersionMessage;
                            break;

                        //case MessageHeader1.SetPinMode:
                        //case MessageHeader1.SystemReset:
                        default:
                            // 0xF? command not supported.
                            throw new NotImplementedException(string.Format(global::Arduino.Firmata.Messages.NotImplementedEx_Command, serialByte));
                            //                    // 0xF? command not supported.
                            //                    //throw new NotImplementedException(string.Format(Messages.NotImplementedEx_Command, serialByte));
                            //                    //_messageBuffer[0] = 0;
                            //                    //_messageBufferIndex = 0;

                            //                    Console.WriteLine($"\r\n\r\n------------------\r\nUnknown sysex command {serialByte}\r\n\r\n------------------\r\n");
                            //                    // Stream is most likely out of sync or the baudrate is incorrect.
                            //                    // Don't throw an exception here, as we're in the middle of handling an event and
                            //                    // have no way of catching an exception, other than a global unhandled exception handler.
                            //                    // Just skip these bytes, until sync is found when a new message starts.
                            //                    return;
                    }
                    break;

                default:
                    // Command not supported.
                    throw new NotImplementedException(string.Format(global::Arduino.Firmata.Messages.NotImplementedEx_Command, serialByte));
                    //            // Stream is most likely out of sync or the baudrate is incorrect.
                    //            // Don't throw an exception here, as we're in the middle of handling an event from the serial port and
                    //            // have no way of catching an exception, other than a global unhandled exception handler.
                    //            // Just skip these bytes, until sync is found when a new message starts.
                    //            //_messageBuffer[0] = 0;
                    //            //_messageBufferIndex = 0;

                    //            Console.WriteLine($"\r\n\r\n------------------\r\nCommand not supported {serialByte}\r\n\r\n------------------\r\n");

            }
#if DEBUG
            //Debug.Write(messageHeader._processMessage);
#endif
        }
        private void ProcessAnalogStateMessage(int messageByte)
        {
            if (_messageHeader.MessageBufferIndex < 2)
            {
                _messageHeader.WriteMessageByte(messageByte);
            }
            else
            {
                var currentState = new AnalogState
                {
                    Channel = _messageHeader.MessageBuffer[0] & 0x0F,
                    Level = (_messageHeader.MessageBuffer[1] | (messageByte << 7))
                };
                _messageHeader._processMessage = null;

                _messageHeader._arduinoSession.EvintFirmata().OnAnalogStateReceived(new FirmataEventArgs<AnalogState>(currentState));
                //if (AnalogStateReceived != null)
                //    AnalogStateReceived(this, new FirmataEventArgs<AnalogState>(currentState));

                _messageHeader._arduinoSession.OnMessageReceivedHandler(new FirmataMessageEventArgs(new FirmataMessage<AnalogState>(currentState)));
                //if (MessageReceived != null)
                //    MessageReceived(this, new FirmataMessageEventArgs(new FirmataMessage(currentState, MessageType.AnalogState)));

            }
        }

        private void ProcessDigitalStateMessage(int messageByte)
        {
            if (_messageHeader.MessageBufferIndex < 2)
            {
                _messageHeader.WriteMessageByte(messageByte);
            }
            else
            {
                var currentState = new DigitalPortState
                {
                    Port = _messageHeader.MessageBuffer[0] & 0x0F,
                    Pins = _messageHeader.MessageBuffer[1] | (messageByte << 7)
                };
                _messageHeader._processMessage = null;

                _messageHeader._arduinoSession.EvintFirmata().OnDigitalStateReceived(new FirmataEventArgs<DigitalPortState>(currentState));
                //if (DigitalStateReceived != null)
                //    DigitalStateReceived(this, new FirmataEventArgs<DigitalPortState>(currentState));

                _messageHeader._arduinoSession.OnMessageReceivedHandler(new FirmataMessageEventArgs(new FirmataMessage<DigitalPortState>(currentState)));
                //if (MessageReceived != null)
                //    MessageReceived(this, new FirmataMessageEventArgs(new FirmataMessage(currentState, MessageType.DigitalPortState)));

            }
        }

        private void ProcessProtocolVersionMessage(int messageByte)
        {
            if (_messageHeader.MessageBufferIndex < 2)
            {
                _messageHeader.WriteMessageByte(messageByte);
            }
            else
            {
                var version = new ProtocolVersion
                {
                    Major = _messageHeader.MessageBuffer[1],
                    Minor = messageByte
                };
                DeliverMessage(new FirmataMessage<ProtocolVersion>(version));
            }
        }

        private void ProcessSysExMessage(int messageByte)
        {
            if (messageByte != Utility.SysExEnd)
            {
                _messageHeader.WriteMessageByte(messageByte);
                return;
            }

            foreach (var item in sysExMessage)
            {
                if (item.CanHeader((byte)_messageHeader.MessageBuffer[1]))
                {
                    DeliverMessage(item.Header(_messageHeader));
                    return;
                }
            }
            throw new NotImplementedException();


            //switch (_messageBuffer[1])
            //{
            //    case 0x6A: // AnalogMappingResponse
            //        DeliverMessage(CreateAnalogMappingResponse());
            //        return;

            //    case 0x6C: // CapabilityResponse
            //        DeliverMessage(CreateCapabilityResponse());
            //        return;

            //    case 0x6E: // PinStateResponse
            //        DeliverMessage(CreatePinStateResponse());
            //        return;

            //    case 0x71: // StringData
            //        DeliverMessage(CreateStringDataMessage());
            //        return;

            //    case 0x77: // I2cReply
            //        DeliverMessage(CreateI2CReply());
            //        return;

            //    case 0x79: // FirmwareResponse
            //        DeliverMessage(CreateFirmwareResponse());
            //        return;

            //    case 0x62: // 步进电机
            //        DeliverMessage(CreateAccelStepperResponse());
            //        return;

            //    default: // Unknown or unsupported message
            //        throw new NotImplementedException();
            //}
        }


        private void DeliverMessage(IFirmataMessage message)
        {
            _messageHeader._processMessage = null;

            lock (_messageHeader._receivedMessageList)
            {
                if (_messageHeader._receivedMessageList.Count >= MessageHeader.MaxQueuelength)
                    throw new OverflowException(global::Arduino.Firmata.Messages.OverflowEx_MsgBufferFull);

                // Remove all unprocessed and timed-out messages.
                while (_messageHeader._receivedMessageList.Count > 0 &&
                    ((DateTime.UtcNow - _messageHeader._receivedMessageList.First.Value.Time).TotalMilliseconds > _messageHeader.TimeOut))
                {
                    _messageHeader._receivedMessageList.RemoveFirst();
                }

                _messageHeader._receivedMessageList.AddLast(message);
                Monitor.PulseAll(_messageHeader._receivedMessageList);
            }

            _messageHeader._arduinoSession.OnMessageReceivedHandler(new FirmataMessageEventArgs(message));
            //if (MessageReceived != null && message.Type != MessageType.I2CReply)
            //    MessageReceived(this, new FirmataMessageEventArgs(message));
        }
    }

}
