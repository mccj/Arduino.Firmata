using Solid.Arduino.Firmata;
using System;
using System.Threading.Tasks;

namespace Arduino.Firmata
{
    /// <summary>
    /// Defines a comprehensive set of members supporting the Firmata Protocol.
    /// Currently version 2.3 is supported.
    /// </summary>
    /// <seealso href="https://github.com/firmata/arduino">Firmata project on GitHub</seealso>
    /// <seealso href="https://github.com/firmata/protocol">Firmata protocol details</seealso>
    /// <seealso href="http://arduino.cc/en/reference/firmata">Firmata reference for Arduino</seealso>
    public static class FirmataProtocol
    {
        //private const byte SysExStart = 0xF0;
        //private const byte SysExEnd = 0xF7;
        private const byte AnalogMessage = 0xE0;
        private const byte DigitalMessage = 0x90;
        private const byte ReportAnalogPin = 0xC0;
        private const byte ReportDigitalPort = 0xD0;
        private const byte SetPinModeIO = 0xF4;
        private const byte SetDigitalPinValue = 0xF5;
        private const byte ProtocolVersion = 0xF9;
        private const byte SystemReset = 0xFF;
        //private const byte SysExString = 0x71;
        //private const byte SysExFirmware = 0x79;



        //private const byte RESERVED = 0x01-0x0F; // IDs 0x01 - 0x0F are reserved for user defined commands
        private const byte EXTENDED_ID = 0x00; // A value of 0x00 indicates the next 2 bytes define the extended ID
        private const byte ANALOG_MAPPING_QUERY = 0x69; // ask for mapping of analog to pin numbers
        internal const byte ANALOG_MAPPING_RESPONSE = 0x6A; // reply with mapping info
        private const byte CAPABILITY_QUERY = 0x6B; // ask for supported modes and resolution of all pins
        internal const byte CAPABILITY_RESPONSE = 0x6C; // reply with supported modes and resolution
        private const byte PIN_STATE_QUERY = 0x6D; // ask for a pin's current mode and state (different than value)
        internal const byte PIN_STATE_RESPONSE = 0x6E; // reply with a pin's current mode and state (different than value)
        private const byte EXTENDED_ANALOG = 0x6F; // analog write (PWM, Servo, etc) to any pin
        private const byte STRING_DATA = 0x71; // a string message with 14-bits per char
        internal const byte REPORT_FIRMWARE = 0x79; // report name and version of the firmware
        private const byte SAMPLING_INTERVAL = 0x7A; // the interval at which analog input is sampled (default = 19ms)
        private const byte SYSEX_NON_REALTIME = 0x7E; // MIDI Reserved for non-realtime messages
        private const byte SYSEX_REALTIME = 0X7F; // MIDI Reserved for realtime messages


        ///// <summary>
        ///// Event, raised for every SysEx (0xF0) and ProtocolVersion (0xF9) message not handled by an <see cref="IFirmataProtocol"/>'s Get method.
        ///// </summary>
        ///// <remarks>
        ///// When e.g. method <see cref="RequestBoardCapability"/> is invoked, the party system's response message raises this event.
        ///// However, when method <see cref="GetBoardCapability"/> or <see cref="GetBoardCapabilityAsync"/> is invoked, the response is returned
        ///// to the respective method and event <see cref="MessageReceived"/> is not raised.
        ///// 
        ///// This event is not raised for either analog or digital I/O messages.
        ///// </remarks>
        //event MessageReceivedHandler MessageReceived;

        ///// <summary>
        ///// Event, raised when an analog state message (command 0xE0) is received.
        ///// </summary>
        ///// <remarks>
        ///// The frequency at which analog state messages are being sent by the party system can be set with method <see cref="SetSamplingInterval"/>.
        ///// </remarks>
        //event AnalogStateReceivedHandler AnalogStateReceived;

        ///// <summary>
        ///// Event, raised when a digital I/O message (command 0x90) is received.
        ///// </summary>
        ///// <remarks>
        ///// Please note that the StandardFirmata implementation for Arduino only sends updates of digital port states if necessary.
        ///// When none of a port's digital input pins have changed state since a previous polling cycle, no Firmata.sendDigitalPort message
        ///// is sent.
        ///// <para>
        ///// Also, calling method <see cref="SetDigitalReportMode"/> does not guarantee this event will receive a (first) Firmata.sendDigitalPort message.
        ///// Use method <see cref="GetPinState"/> or <see cref="GetPinStateAsync"/> inquiring the current pin states.
        ///// </para>
        ///// </remarks>
        //event DigitalStateReceivedHandler DigitalStateReceived;

        /// <summary>
        /// Creates an observable object tracking <see cref="AnalogState"/> messages.
        /// </summary>
        /// <returns>An <see cref="IObservable{AnalogState}"/> interface</returns>
        public static IObservable<AnalogState> CreateAnalogStateMonitor(this ArduinoSession session)
        {
            return new AnalogStateTracker(session.EvintFirmata());
        }
        /// <summary>
        /// Creates an observable object tracking <see cref="AnalogState" /> messages for a specific channel.
        /// </summary>
        /// <param name="channel">The channel to track</param>
        /// <returns>
        /// An <see cref="IObservable{AnalogState}" /> interface
        /// </returns>
        public static IObservable<AnalogState> CreateAnalogStateMonitor(this ArduinoSession session, int channel)
        {
            if (channel < 0 || channel > 15)
                throw new ArgumentOutOfRangeException(nameof(channel), Messages.ArgumentEx_ChannelRange0_15);

            return new AnalogStateTracker(session.EvintFirmata(), channel);
        }
        /// <summary>
        /// Creates an observable object tracking <see cref="DigitalPortState"/> messages.
        /// </summary>
        /// <returns>An <see cref="IObservable{DigitalPortState}"/> interface</returns>
        public static IObservable<DigitalPortState> CreateDigitalStateMonitor(this ArduinoSession session)
        {
            return new DigitalStateTracker(session.EvintFirmata());
        }
        /// <summary>
        /// Creates an observable object tracking <see cref="DigitalPortState" /> messages for a specific port.
        /// </summary>
        /// <param name="port">The port to track</param>
        /// <returns>
        /// An <see cref="IObservable{DigitalPortState}" /> interface
        /// </returns>
        public static IObservable<DigitalPortState> CreateDigitalStateMonitor(this ArduinoSession session, int port)
        {
            if (port < 0 || port > 15)
                throw new ArgumentOutOfRangeException(nameof(port), Messages.ArgumentEx_PortRange0_15);

            return new DigitalStateTracker(session.EvintFirmata(), port);
        }

        /// <summary>
        /// Sends a message string.
        /// </summary>
        /// <param name="data">The message string</param>
        public static void SendStringData(this ArduinoSession session, string data)
        {
            if (data == null)
                data = string.Empty;

            byte[] command = new byte[data.Length * 2 + 3];
            command[0] = Utility.SysExStart;
            command[1] = STRING_DATA;

            for (int x = 0; x < data.Length; x++)
            {
                short c = Convert.ToInt16(data[x]);
                command[x * 2 + 2] = (byte)(c & 0x7F);
                command[x * 2 + 3] = (byte)((c >> 7) & 0x7F);
            }

            command[command.Length - 1] = Utility.SysExEnd;

            session.Write(command, 0, command.Length);
        }
        /// <summary>
        /// Enables or disables analog sampling reporting.
        /// </summary>
        /// <param name="channel">The channel attached to the analog pin</param>
        /// <param name="enable"><c>True</c> if enabled, otherwise <c>false</c></param>
        /// <remarks>
        /// When enabled, the party system is expected to return analog I/O messages (0xE0)
        /// for the given channel. The frequency at which these messages are returned can
        /// be controlled by method <see cref="SetSamplingInterval"/>.
        /// </remarks>
        public static void SetAnalogReportMode(this ArduinoSession session, int channel, bool enable)
        {
            if (channel < 0 || channel > 15)
                throw new ArgumentOutOfRangeException(nameof(channel), Messages.ArgumentEx_ChannelRange0_15);

            session.Write(new[] { (byte)(ReportAnalogPin | channel), (byte)(enable ? 1 : 0) }, 0, 2);
        }
        /// <summary>
        /// Sets the digital output pins of a given port LOW or HIGH.
        /// </summary>
        /// <param name="portNumber">The 0-based port number</param>
        /// <param name="pins">Binary value for the port's pins (0 to 7)</param>
        /// <remarks>
        /// A binary 1 sets the digital output pin HIGH (+5 or +3.3 volts).
        /// A binary 0 sets the digital output pin LOW.
        /// <para>
        /// The Arduino operates with 8-bit ports, so only bits 0 to 7 of the pins parameter are mapped.
        /// Higher bits are ignored.
        /// </para>
        /// <example>
        /// For port 0 bit 2 maps to the Arduino Uno's pin 2.
        /// For port 1 bit 2 maps to pin 10.
        /// 
        /// The complete mapping of port 1 of the Arduino Uno looks like this:
        /// <list type="">
        /// <item>bit 0: pin 8</item>
        /// <item>bit 1: pin 9</item>
        /// <item>bit 2: pin 10</item>
        /// <item>bit 3: pin 11</item>
        /// <item>bit 4: pin 12</item>
        /// <item>bit 5: pin 13</item>
        /// <item>bit 6: not mapped</item>
        /// <item>bit 7: not mapped</item>
        /// </list> 
        /// </example>
        /// </remarks>
        public static void SetDigitalPort(this ArduinoSession session, int portNumber, int pins)
        {
            if (portNumber < 0 || portNumber > 15)
                throw new ArgumentOutOfRangeException(nameof(portNumber), Messages.ArgumentEx_PortRange0_15);

            if (pins < 0 || pins > 0xFF)
                throw new ArgumentOutOfRangeException(nameof(pins), Messages.ArgumentEx_ValueRange0_255);

            session.Write(new[] { (byte)(DigitalMessage | portNumber), (byte)(pins & 0x7F), (byte)((pins >> 7) & 0x03) }, 0, 3);
        }

        /// <summary>
        /// Enables or disables digital input pin reporting for the given port.
        /// </summary>
        /// <param name="portNumber">The number of the port</param>
        /// <param name="enable"><c>true</c> if enabled, otherwise <c>false</c></param>
        /// <remarks>
        /// When enabled, the party system is expected to return digital I/O messages (0x90)
        /// for the given port.
        /// <para>
        /// Note: as for Firmata version 2.3 digital I/O messages are only returned when
        /// at least one digital input pin's state has changed from high to low or vice versa.
        /// </para>
        /// </remarks>
        public static void SetDigitalReportMode(this ArduinoSession session, int portNumber, bool enable)
        {
            if (portNumber < 0 || portNumber > 15)
                throw new ArgumentOutOfRangeException(nameof(portNumber), Messages.ArgumentEx_PortRange0_15);

            session.Write(new[] { (byte)(ReportDigitalPort | portNumber), (byte)(enable ? 1 : 0) }, 0, 2);
        }
        /// <summary>
        /// Sets a pin's mode (digital input/digital output/analog/PWM/servo etc.).
        /// </summary>
        /// <param name="pinNumber">The number of the pin</param>
        /// <param name="mode">The pin's mode</param>
        public static void SetDigitalPinMode(this ArduinoSession session, int pinNumber, PinMode mode)
        {
            if (pinNumber < 0 || pinNumber > 127)
                throw new ArgumentOutOfRangeException(nameof(pinNumber), Messages.ArgumentEx_PinRange0_127);

            session.Write(new byte[] { SetPinModeIO, (byte)pinNumber, (byte)mode });
        }
        /// <summary>
        /// Sets the frequency at which analog samples must be reported.
        /// </summary>
        /// <param name="milliseconds">The sampling interval in milliseconds</param>
        public static void SetSamplingInterval(this ArduinoSession session, int milliseconds)
        {
            if (milliseconds < 0 || milliseconds > 0x3FFF)
                throw new ArgumentOutOfRangeException(nameof(milliseconds), Messages.ArgumentEx_SamplingInterval);

            var command = new[]
            {
                 Utility.SysExStart,
                SAMPLING_INTERVAL,
                (byte)(milliseconds & 0x7F),
                (byte)((milliseconds >> 7) & 0x7F),
                Utility.SysExEnd
            };
            session.Write(command, 0, 5);
        }
        /// <summary>
        /// Sets an analog value on a PWM or Servo enabled analog output pin.
        /// </summary>
        /// <param name="pinNumber">The pin number.</param>
        /// <param name="value">The value</param>
        public static void SetDigitalPin(this ArduinoSession session, int pinNumber, long value)
        {
            if (pinNumber < 0 || pinNumber > 127)
                throw new ArgumentOutOfRangeException(nameof(pinNumber), Messages.ArgumentEx_PinRange0_127);

            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), Messages.ArgumentEx_NoNegativeValue);

            byte[] message;

            if (pinNumber < 16 && value < 0x4000)
            {
                // Send value in a conventional Analog Message.
                message = new[] {
                    (byte)(AnalogMessage | pinNumber),
                    (byte)(value & 0x7F),
                    (byte)((value >> 7) & 0x7F)
                };
                session.Write(message);
                return;
            }

            // Send long value in an Extended Analog Message.
            message = new byte[14];
            message[0] = Utility.SysExStart;
            message[1] = EXTENDED_ANALOG;
            message[2] = (byte)pinNumber;
            int index = 3;

            do
            {
                message[index] = (byte)(value & 0x7F);
                value >>= 7;
                index++;
            } while (value > 0 || index < 5);

            message[index] = Utility.SysExEnd;
            session.Write(message, 0, index + 1);
        }

        /// <summary>
        /// Sets a HI or LO value on a digital output pin.
        /// </summary>
        /// <param name="pinNumber">The pin number</param>
        /// <param name="value">The value (<c>false</c> = Low, <c>true</c> = High)</param>
        public static void SetDigitalPin(this ArduinoSession session, int pinNumber, bool value)
        {
            if (pinNumber < 0 || pinNumber > 127)
                throw new ArgumentOutOfRangeException(nameof(pinNumber), Messages.ArgumentEx_PinRange0_127);

            session.Write(new[] { SetDigitalPinValue, (byte)pinNumber, (byte)(value ? 1 : 0) });
        }

        /// <summary>
        /// Sends a reset message to the party system.
        /// </summary>
        public static void ResetBoard(this ArduinoSession session)
        {
            session.SendCommand(SystemReset);
        }
        #region GetProtocolVersion
        /// <summary>
        /// Requests the party system to send a protocol version message.
        /// </summary>
        /// <remarks>
        /// The party system is expected to return a single protocol version message (0xF9).
        /// This message triggers the <see cref="MessageReceived"/> event. The protocol version
        /// is passed in the <see cref="FirmataMessageEventArgs"/> in a <see cref="ProtocolVersion"/> object.
        /// </remarks>
        internal static void RequestProtocolVersion(this ArduinoSession session)
        {
            //Console.WriteLine($"\r\n{_stopWatch.ElapsedMilliseconds}: RequestProtocolVersion()");
            session.SendCommand(ProtocolVersion);
        }

        /// <summary>
        /// Gets the protocol version implemented on the party system.
        /// </summary>
        /// <returns>The implemented protocol version</returns>
        public static ProtocolVersion GetProtocolVersion(this ArduinoSession session)
        {
            session.RequestProtocolVersion();
            //return (ProtocolVersion)((FirmataMessage)session.GetMessageFromQueue(new FirmataMessage(MessageType.ProtocolVersion))).Value;
            return session.GetMessageFromQueue<ProtocolVersion>().Value;
        }

        /// <summary>
        /// Asynchronously gets the protocol version implemented on the party system.
        /// </summary>
        /// <returns>The implemented protocol version</returns>
        public static async Task<ProtocolVersion> GetProtocolVersionAsync(this ArduinoSession session)
        {
            session.RequestProtocolVersion();
            //return await Task.Run(() =>
            //    (ProtocolVersion)((FirmataMessage)session.GetMessageFromQueue(new FirmataMessage(MessageType.ProtocolVersion))).Value);
            return await Task.Run(() => session.GetMessageFromQueue<ProtocolVersion>().Value).ConfigureAwait(false);
        }
        #endregion
        #region GetFirmware
        /// <summary>
        /// Requests the party system to send a firmware message.
        /// </summary>
        /// <remarks>
        /// The party system is expected to return a single SYSEX REPORT_FIRMWARE message.
        /// This message triggers the <see cref="MessageReceived"/> event. The firmware signature
        /// is passed in the <see cref="FirmataMessageEventArgs"/> in a <see cref="Firmware"/> object.
        /// </remarks>
        internal static void RequestFirmware(this ArduinoSession session)
        {
            //Console.WriteLine($"\r\n{_stopWatch.ElapsedMilliseconds}: RequestFirmware()");
            session.SendSysExCommand(REPORT_FIRMWARE);
        }

        /// <summary>
        /// Gets the firmware signature of the party system.
        /// </summary>
        /// <returns>The firmware signature</returns>
        public static Firmware GetFirmware(this ArduinoSession session)
        {
            session.RequestFirmware();
            //return (Firmware)((FirmataMessage)session.GetMessageFromQueue(new FirmataMessage(MessageType.FirmwareResponse))).Value;
            return session.GetMessageFromQueue<Firmware>().Value;
        }

        /// <summary>
        /// Asynchronously gets the firmware signature of the party system.
        /// </summary>
        /// <returns>The firmware signature</returns>
        public static async Task<Firmware> GetFirmwareAsync(this ArduinoSession session)
        {
            session.RequestFirmware();
            //return await Task.Run(() =>
            //    (Firmware)((FirmataMessage)session.GetMessageFromQueue(new FirmataMessage(MessageType.FirmwareResponse))).Value);
            return await Task.Run(() => session.GetMessageFromQueue<Firmware>().Value).ConfigureAwait(false);
        }
        #endregion
        #region GetBoardCapability
        /// <summary>
        /// Requests the party system to send a summary of its capabilities.
        /// </summary>
        /// <remarks>
        /// The party system is expected to return a single SYSEX CAPABILITY_RESPONSE message.
        /// This message triggers the <see cref="MessageReceived"/> event. The capabilities
        /// are passed in the <see cref="FirmataMessageEventArgs"/> in a <see cref="BoardCapability"/> object.
        /// </remarks>
        internal static void RequestBoardCapability(this ArduinoSession session)
        {
            session.SendSysExCommand(CAPABILITY_QUERY);
        }
        /// <summary>
        /// Gets a summary of the party system's capabilities.
        /// </summary>
        /// <returns>The system's capabilities</returns>
        public static BoardCapability GetBoardCapability(this ArduinoSession session)
        {
            session.RequestBoardCapability();
            //return (BoardCapability)((FirmataMessage)session.GetMessageFromQueue(new FirmataMessage(MessageType.CapabilityResponse))).Value;
            return session.GetMessageFromQueue<BoardCapability>().Value;
        }

        /// <summary>
        /// Asynchronously gets a summary of the party system's capabilities.
        /// </summary>
        /// <returns>The system's capabilities</returns>
        public static async Task<BoardCapability> GetBoardCapabilityAsync(this ArduinoSession session)
        {
            session.RequestBoardCapability();
            //return await Task.Run(() =>
            //    (BoardCapability)((FirmataMessage)session.GetMessageFromQueue(new FirmataMessage(MessageType.CapabilityResponse))).Value);
            return await Task.Run(() => session.GetMessageFromQueue<BoardCapability>().Value).ConfigureAwait(false);
        }
        #endregion
        #region GetBoardAnalogMapping
        /// <summary>
        /// Requests the party system to send the channel-to-pin mappings of its analog lines.
        /// </summary>
        /// <remarks>
        /// The party system is expected to return a single SYSEX ANALOG_MAPPING_RESPONSE message.
        /// This message triggers the <see cref="MessageReceived"/> event. The analog mappings are
        /// passed in the <see cref="FirmataMessageEventArgs"/> in a <see cref="BoardAnalogMapping"/> object.
        /// </remarks>
        internal static void RequestBoardAnalogMapping(this ArduinoSession session)
        {
            session.SendSysExCommand(ANALOG_MAPPING_QUERY);
        }
        /// <summary>
        /// Gets the channel-to-pin mappings of the party system's analog lines.
        /// </summary>
        /// <returns>The channel-to-pin mappings</returns>
        public static BoardAnalogMapping GetBoardAnalogMapping(this ArduinoSession session)
        {
            session.RequestBoardAnalogMapping();
            //return (BoardAnalogMapping)((FirmataMessage)session.GetMessageFromQueue(new FirmataMessage(MessageType.AnalogMappingResponse))).Value;
            return session.GetMessageFromQueue<BoardAnalogMapping>().Value;
        }
        /// <summary>
        /// Asynchronously gets the channel-to-pin mappings of the party system's analog lines.
        /// </summary>
        /// <returns>The channel-to-pin mappings</returns>
        public static async Task<BoardAnalogMapping> GetBoardAnalogMappingAsync(this ArduinoSession session)
        {
            session.RequestBoardAnalogMapping();
            //return await Task.Run(() =>
            //    (BoardAnalogMapping)((FirmataMessage)session.GetMessageFromQueue(new FirmataMessage(MessageType.AnalogMappingResponse))).Value);
            return await Task.Run(() => session.GetMessageFromQueue<BoardAnalogMapping>().Value).ConfigureAwait(false);
        }
        #endregion
        #region GetPinState
        /// <summary>
        /// Requests the party system to send the state of a given pin.
        /// </summary>
        /// <param name="pinNumber">The pin number</param>
        /// <remarks>
        /// The party system is expected to return a single SYSEX PINSTATE_RESPONSE message.
        /// This message triggers the <see cref="MessageReceived"/> event. The pin state
        /// is passed in the <see cref="FirmataMessageEventArgs"/> in a <see cref="PinState"/> object.
        /// </remarks>
        internal static void RequestPinState(this ArduinoSession session, int pinNumber)
        {
            if (pinNumber < 0 || pinNumber > 127)
                throw new ArgumentOutOfRangeException(nameof(pinNumber), Messages.ArgumentEx_PinRange0_127);

            var command = new[]
            {
                Utility.SysExStart,
                PIN_STATE_QUERY,
                (byte)pinNumber,
                Utility.SysExEnd
            };
            session.Write(command);
        }
        /// <summary>
        /// Gets a pin's mode (digital input/output, analog etc.) and actual value.
        /// </summary>
        /// <param name="pinNumber">The pin number</param>
        /// <returns>The pin's state</returns>
        public static PinState GetPinState(this ArduinoSession session, int pinNumber)
        {
            session.RequestPinState(pinNumber);
            //return (PinState)((FirmataMessage)session.GetMessageFromQueue(new FirmataMessage(MessageType.PinStateResponse))).Value;
            return session.GetMessageFromQueue<PinState>().Value;
        }

        /// <summary>
        /// Asynchronously gets a pin's mode (digital input/output, analog etc.) and actual value.
        /// </summary>
        /// <param name="pinNumber">The pin number</param>
        /// <returns>The pin's state</returns>
        public static async Task<PinState> GetPinStateAsync(this ArduinoSession session, int pinNumber)
        {
            session.RequestPinState(pinNumber);
            //return await Task.Run
            //(
            //    () =>
            //    (PinState)((FirmataMessage)session.GetMessageFromQueue(new FirmataMessage(MessageType.PinStateResponse))).Value
            //);
            return await Task.Run(() => session.GetMessageFromQueue<PinState>().Value).ConfigureAwait(false);
        }
        #endregion

        public static FirmataEvint EvintFirmata(this ArduinoSession session)
        {
            return session.GetEvint(()=>new FirmataEvint(session));
        }
    }

    /// <summary>
    /// The modes a pin can be in or can be set to.
    /// </summary>
    public enum PinMode
    {
        Undefined = -1,
        DigitalInput = 0,
        DigitalOutput = 1,
        AnalogInput = 2,
        PwmOutput = 3,
        ServoControl = 4,
        I2C = 6,
        OneWire = 7,
        StepperControl = 8,
        Encoder = 9,
        Serial = 10,
        InputPullup = 11
    }
}
