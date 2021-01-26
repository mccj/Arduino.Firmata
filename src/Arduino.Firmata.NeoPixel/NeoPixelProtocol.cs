using Arduino.Firmata.Extend;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arduino.Firmata.Protocol.NeoPixel
{
    /*
  Adafruit_NeoPixel(uint16_t n, uint16_t pin=6, neoPixelType type=NEO_GRB + NEO_KHZ800);
  Adafruit_NeoPixel(void);

  void              begin(void);
  void              show(void);
  void              setPin(uint16_t p);
  void              setPixelColor(uint16_t n, uint8_t r, uint8_t g, uint8_t b);
  void              setPixelColor(uint16_t n, uint8_t r, uint8_t g, uint8_t b, uint8_t w);
  void              setPixelColor(uint16_t n, uint32_t c);
  void              fill(uint32_t c=0, uint16_t first=0, uint16_t count=0);
  void              setBrightness(uint8_t);
  void              clear(void);
  void              updateLength(uint16_t n);
  void              updateType(neoPixelType t);
  /*!
    @brief   Check whether a call to show() will start sending data
             immediately or will 'block' for a required interval. NeoPixels
             require a short quiet time (about 300 microseconds) after the
             last bit is received before the data 'latches' and new data can
             start being received. Usually one's sketch is implicitly using
             this time to generate a new frame of animation...but if it
             finishes very quickly, this function could be used to see if
             there's some idle time available for some low-priority
             concurrent task.
    @return  1 or true if show() will start sending immediately, 0 or false
             if show() would block (meaning some idle time is available).
  *-/
  bool canShow(void) {
    if (endTime > micros()) {
      endTime = micros();
    }
    return (micros() - endTime) >= 300L;
  }
  /*!
    @brief   Get a pointer directly to the NeoPixel data buffer in RAM.
             Pixel data is stored in a device-native format (a la the NEO_*
             constants) and is not translated here. Applications that access
             this buffer will need to be aware of the specific data format
             and handle colors appropriately.
    @return  Pointer to NeoPixel buffer (uint8_t* array).
    @note    This is for high-performance applications where calling
             setPixelColor() on every single pixel would be too slow (e.g.
             POV or light-painting projects). There is no bounds checking
             on the array, creating tremendous potential for mayhem if one
             writes past the ends of the buffer. Great power, great
             responsibility and all that.
  *-/
  uint8_t          *getPixels(void) const { return pixels; };
  uint8_t           getBrightness(void) const;
  /*!
    @brief   Retrieve the pin number used for NeoPixel data output.
    @return  Arduino pin number (-1 if not set).
  *-/
  int16_t           getPin(void) const { return pin; };
  /*!
    @brief   Return the number of pixels in an Adafruit_NeoPixel strip object.
    @return  Pixel count (0 if not set).
  *-/
  uint16_t          numPixels(void) const { return numLEDs; }
  uint32_t          getPixelColor(uint16_t n) const;
  /*!
    @brief   An 8-bit integer sine wave function, not directly compatible
             with standard trigonometric units like radians or degrees.
    @param   x  Input angle, 0-255; 256 would loop back to zero, completing
                the circle (equivalent to 360 degrees or 2 pi radians).
                One can therefore use an unsigned 8-bit variable and simply
                add or subtract, allowing it to overflow/underflow and it
                still does the expected contiguous thing.
    @return  Sine result, 0 to 255, or -128 to +127 if type-converted to
             a signed int8_t, but you'll most likely want unsigned as this
             output is often used for pixel brightness in animation effects.
  *-/
  static uint8_t    sine8(uint8_t x) {
    return pgm_read_byte(&_NeoPixelSineTable[x]); // 0-255 in, 0-255 out
  }
  /*!
    @brief   An 8-bit gamma-correction function for basic pixel brightness
             adjustment. Makes color transitions appear more perceptially
             correct.
    @param   x  Input brightness, 0 (minimum or off/black) to 255 (maximum).
    @return  Gamma-adjusted brightness, can then be passed to one of the
             setPixelColor() functions. This uses a fixed gamma correction
             exponent of 2.6, which seems reasonably okay for average
             NeoPixels in average tasks. If you need finer control you'll
             need to provide your own gamma-correction function instead.
  *-/
  static uint8_t    gamma8(uint8_t x) {
    return pgm_read_byte(&_NeoPixelGammaTable[x]); // 0-255 in, 0-255 out
  }
  /*!
    @brief   Convert separate red, green and blue values into a single
             "packed" 32-bit RGB color.
    @param   r  Red brightness, 0 to 255.
    @param   g  Green brightness, 0 to 255.
    @param   b  Blue brightness, 0 to 255.
    @return  32-bit packed RGB value, which can then be assigned to a
             variable for later use or passed to the setPixelColor()
             function. Packed RGB format is predictable, regardless of
             LED strand color order.
  *-/
  static uint32_t   Color(uint8_t r, uint8_t g, uint8_t b) {
    return ((uint32_t)r << 16) | ((uint32_t)g <<  8) | b;
  }
  /*!
    @brief   Convert separate red, green, blue and white values into a
             single "packed" 32-bit WRGB color.
    @param   r  Red brightness, 0 to 255.
    @param   g  Green brightness, 0 to 255.
    @param   b  Blue brightness, 0 to 255.
    @param   w  White brightness, 0 to 255.
    @return  32-bit packed WRGB value, which can then be assigned to a
             variable for later use or passed to the setPixelColor()
             function. Packed WRGB format is predictable, regardless of
             LED strand color order.
  *-/
  static uint32_t   Color(uint8_t r, uint8_t g, uint8_t b, uint8_t w) {
    return ((uint32_t)w << 24) | ((uint32_t)r << 16) | ((uint32_t)g <<  8) | b;
  }
  static uint32_t   ColorHSV(uint16_t hue, uint8_t sat=255, uint8_t val=255);
  /*!
    @brief   A gamma-correction function for 32-bit packed RGB or WRGB
             colors. Makes color transitions appear more perceptially
             correct.
    @param   x  32-bit packed RGB or WRGB color.
    @return  Gamma-adjusted packed color, can then be passed in one of the
             setPixelColor() functions. Like gamma8(), this uses a fixed
             gamma correction exponent of 2.6, which seems reasonably okay
             for average NeoPixels in average tasks. If you need finer
             control you'll need to provide your own gamma-correction
             function instead.
  *-/
  static uint32_t   gamma32(uint32_t x);



















begin			KEYWORD2
show			KEYWORD2
setPin			KEYWORD2
setPixelColor		KEYWORD2
fill			KEYWORD2
setBrightness		KEYWORD2
clear			KEYWORD2
updateLength		KEYWORD2
updateType		KEYWORD2
canShow			KEYWORD2
getPixels		KEYWORD2
getBrightness		KEYWORD2
getPin			KEYWORD2
numPixels		KEYWORD2
getPixelColor		KEYWORD2
sine8			KEYWORD2
gamma8			KEYWORD2
Color			KEYWORD2
ColorHSV		KEYWORD2
gamma32			KEYWORD2
     */
    /// <summary>
    /// 
    /// </summary>
    public static class NeoPixelProtocol
    {
        public const byte NEOPIXEL_DATA = 0x63;

        public const byte NEOPIXEL_CONFIG = 0x00;
        public const byte NEOPIXEL_BEGIN = 0x01;
        public const byte NEOPIXEL_SHOW = 0x02;
        public const byte NEOPIXEL_CLEAR = 0x03;
        public const byte NEOPIXEL_SET_PIXEL_COLOR = 0x04;
        public const byte NEOPIXEL_FILL = 0x05;
        public const byte NEOPIXEL_UPDATE_LENGTH = 0x06;
        public const byte NEOPIXEL_UPDATE_TYPE = 0x07;
        public const byte NEOPIXEL_SET_PIN = 0x08;
        public const byte NEOPIXEL_SET_BRIGHTNESS = 0x09;

        public const byte NEOPIXEL_REPORT_CAN_SHOW = 0x0a;
        public const byte NEOPIXEL_REPORT_COMPLETE1 = 0x0b;
        public const byte NEOPIXEL_REPORT_GET_BRIGHTNESS = 0x0c;
        public const byte NEOPIXEL_REPORT_GET_PIN = 0x0d;
        public const byte NEOPIXEL_REPORT_COMPLETE4 = 0x0e;
        public const byte NEOPIXEL_REPORT_GET_PIXEL_COLOR = 0x0f;
        public const byte NEOPIXEL_REPORT_SINE8 = 0x10;
        public const byte NEOPIXEL_REPORT_GAMMA8 = 0x11;
        public const byte NEOPIXEL_REPORT_GAMMA32 = 0x12;
        public const byte NEOPIXEL_REPORT_COLOR = 0x13;
        public const byte NEOPIXEL_REPORT_COLORHSV = 0x14;

        public static void NeoPixelConfiguration(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_CONFIG,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };

            session.Write(command, 0, command.Length);
        }
        public static void NeoPixelConfiguration(this ArduinoSession session, int deviceNumber, int ledCount, int pinNumber = 6, NeoPixelType ledType = NeoPixelType.NEO_GRB | NeoPixelType.NEO_KHZ800)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");
            if (pinNumber < 0 || pinNumber > 127)
                throw new ArgumentOutOfRangeException(nameof(pinNumber), "Pin number must be between 0 and 127.");
            if (ledCount < 0)
                throw new ArgumentOutOfRangeException(nameof(ledCount), "ledCount must be between 0.");

            var bytes = ledCount.encode32BitSignedInteger();
            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_CONFIG,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                bytes[0], //4  num steps, bits 0-6
                bytes[1], //5  num steps, bits 7-13
                bytes[2], //6  num steps, bits 14-20
                bytes[3], //7  num steps, bits 21-27
                bytes[4], //8  num steps, bits 28-32
                (byte)pinNumber,
                (byte)ledType,
                Utility.SysExEnd
            };

            session.Write(command, 0, command.Length);
        }

        public static void NeoPixelBegin(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_BEGIN,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static void NeoPixelShow(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_SHOW,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static void NeoPixelClear(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_CLEAR,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static void NeoPixelUpdateLength(this ArduinoSession session, int deviceNumber, int ledCount)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");
            if (ledCount < 0)
                throw new ArgumentOutOfRangeException(nameof(ledCount), "ledCount must be between 0.");

            var bytes = ledCount.encode32BitSignedInteger();
            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_UPDATE_LENGTH,
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
        public static void NeoPixelUpdateType(this ArduinoSession session, int deviceNumber, NeoPixelType ledType)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_UPDATE_TYPE,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                (byte)ledType,
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static void NeoPixelSetPin(this ArduinoSession session, int deviceNumber, int pinNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");
            if (pinNumber < 0 || pinNumber > 127)
                throw new ArgumentOutOfRangeException(nameof(pinNumber), "Pin number must be between 0 and 127.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_SET_PIN,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                (byte)pinNumber,
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static void NeoPixelSetPixelColor(this ArduinoSession session, int deviceNumber, int n, byte red, byte green, byte blue, byte? white = null)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var bytes = n.encode32BitSignedInteger();
            var command = new List<byte>
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_SET_PIXEL_COLOR,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                bytes[0], //4  num steps, bits 0-6
                bytes[1], //5  num steps, bits 7-13
                bytes[2], //6  num steps, bits 14-20
                bytes[3], //7  num steps, bits 21-27
                bytes[4], //8  num steps, bits 28-32
                (byte)red,
                (byte)green,
                (byte)blue,
                //(byte)white,
                //Utility.SysExEnd
            };
            if (white.HasValue)
                command.Add((byte)white.Value);

            command.Add(Utility.SysExEnd);

            session.Write(command.ToArray(), 0, command.Count);
        }
        public static void NeoPixelSetPixelColor(this ArduinoSession session, int deviceNumber, int n, System.Drawing.Color color)
        {
            NeoPixelSetPixelColor(session, deviceNumber, n, color.ToArgb());
        }
        public static void NeoPixelSetPixelColor(this ArduinoSession session, int deviceNumber, int n, int color)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var bytes = n.encode32BitSignedInteger();
            var colorBytes = color.encode32BitSignedInteger();
            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_SET_PIXEL_COLOR,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                bytes[0], //4  num steps, bits 0-6
                bytes[1], //5  num steps, bits 7-13
                bytes[2], //6  num steps, bits 14-20
                bytes[3], //7  num steps, bits 21-27
                bytes[4], //8  num steps, bits 28-32
                colorBytes[0], //4  num steps, bits 0-6
                colorBytes[1], //5  num steps, bits 7-13
                colorBytes[2], //6  num steps, bits 14-20
                colorBytes[3], //7  num steps, bits 21-27
                colorBytes[4], //8  num steps, bits 28-32
                Utility.SysExEnd
            };


            session.Write(command, 0, command.Length);
        }
        public static void NeoPixelFill(this ArduinoSession session, int deviceNumber, System.Drawing.Color color, int first, int count)
        {
            NeoPixelFill(session, deviceNumber, color.ToArgb(), first, count);
        }
        public static void NeoPixelFill(this ArduinoSession session, int deviceNumber, int color, int first, int count)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var colorBytes = color.encode32BitSignedInteger();
            var firstBytes = first.encode32BitSignedInteger();
            var countBytes = count.encode32BitSignedInteger();
            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_FILL,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                colorBytes[0], //4  num steps, bits 0-6
                colorBytes[1], //5  num steps, bits 7-13
                colorBytes[2], //6  num steps, bits 14-20
                colorBytes[3], //7  num steps, bits 21-27
                colorBytes[4], //8  num steps, bits 28-32
                firstBytes[0], //4  num steps, bits 0-6
                firstBytes[1], //5  num steps, bits 7-13
                firstBytes[2], //6  num steps, bits 14-20
                firstBytes[3], //7  num steps, bits 21-27
                firstBytes[4], //8  num steps, bits 28-32
                countBytes[0], //4  num steps, bits 0-6
                countBytes[1], //5  num steps, bits 7-13
                countBytes[2], //6  num steps, bits 14-20
                countBytes[3], //7  num steps, bits 21-27
                countBytes[4], //8  num steps, bits 28-32

                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static void NeoPixelSetBrightness(this ArduinoSession session, int deviceNumber, byte b)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_SET_BRIGHTNESS,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                b,
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
        }
        public static CanShow NeoPixelCanShow(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_REPORT_CAN_SHOW,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
            return session.GetMessageFromQueue<CanShow>().Value;
        }
        //public static byte NeoPixelGetPixels(this ArduinoSession session, int deviceNumber)
        //{
        //    if (deviceNumber < 0 || deviceNumber > 9)
        //        throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

        //    var command = new[]
        //    {
        //        Utility.SysExStart,
        //        NEOPIXEL_DATA,
        //        NEOPIXEL_REPORT_COMPLETE1,
        //        (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
        //        Utility.SysExEnd
        //    };
        //    session.Write(command, 0, command.Length);
        //    return session.GetMessageFromQueue<StepperPosition>().Value;
        //}
        public static Brightness NeoPixelGetBrightness(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_REPORT_GET_BRIGHTNESS,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
            return session.GetMessageFromQueue<Brightness>().Value;
        }
        public static Pin NeoPixelGetPin(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_REPORT_GET_PIN,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
            return session.GetMessageFromQueue<Pin>().Value;
        }
        /// <summary>
        /// 貌似存在问题
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        /// <returns></returns>
        public static NumPixels NeoPixelNumPixels(this ArduinoSession session, int deviceNumber)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_REPORT_COMPLETE4,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);
            return session.GetMessageFromQueue<NumPixels>().Value;
        }
        /// <summary>
        /// 貌似存在问题
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static GetPixelColor NeoPixelGetPixelColor(this ArduinoSession session, int deviceNumber, int n)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var bytes = n.encode32BitSignedInteger();
            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_REPORT_GET_PIXEL_COLOR,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                bytes[0], //4  num steps, bits 0-6
                bytes[1], //5  num steps, bits 7-13
                bytes[2], //6  num steps, bits 14-20
                bytes[3], //7  num steps, bits 21-27
                bytes[4], //8  num steps, bits 28-32
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);

            return session.GetMessageFromQueue<GetPixelColor>().Value;
        }
        public static Sine8 NeoPixelSine8(this ArduinoSession session, int deviceNumber, byte x)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_REPORT_SINE8,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                x,
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);

            return session.GetMessageFromQueue<Sine8>().Value;
        }
        /// <summary>
        /// 貌似存在问题
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Gamma8 NeoPixelGamma8(this ArduinoSession session, int deviceNumber, byte x)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new[]
            {
                Utility.SysExStart,
                NEOPIXEL_DATA,
                NEOPIXEL_REPORT_GAMMA8,
                (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
                x,
                Utility.SysExEnd
            };
            session.Write(command, 0, command.Length);

            return session.GetMessageFromQueue<Gamma8>().Value;
        }
        /// <summary>
        /// 貌似存在问题
        /// A gamma-correction function for 32-bit packed RGB or WRGB
        /// colors. Makes color transitions appear more perceptially
        /// correct.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        /// <param name="x">32-bit packed RGB or WRGB color.</param>
        /// <returns>
        ///   Gamma-adjusted packed color, can then be passed in one of the
        ///   setPixelColor() functions. Like gamma8(), this uses a fixed
        ///   gamma correction exponent of 2.6, which seems reasonably okay
        ///   for average NeoPixels in average tasks. If you need finer
        ///   control you'll need to provide your own gamma-correction
        ///   function instead.
        /// </returns>
        public static Gamma32 NeoPixelGamma32(this ArduinoSession session, int deviceNumber, int x)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var bytes = x.encode32BitSignedInteger();
            var command = new[]
            {
              Utility.SysExStart,
              NEOPIXEL_DATA,
              NEOPIXEL_REPORT_GAMMA32,
              (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
              bytes[0], //4  num steps, bits 0-6
              bytes[1], //5  num steps, bits 7-13
              bytes[2], //6  num steps, bits 14-20
              bytes[3], //7  num steps, bits 21-27
              bytes[4], //8  num steps, bits 28-32
              Utility.SysExEnd
          };
            session.Write(command, 0, command.Length);

            return session.GetMessageFromQueue<Gamma32>().Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="white"></param>
        /// <returns></returns>
        public static PixelColor NeoPixelColor(this ArduinoSession session, int deviceNumber, byte red, byte green, byte blue, byte? white = null)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var command = new List<byte>
          {
              Utility.SysExStart,
              NEOPIXEL_DATA,
              NEOPIXEL_REPORT_COLOR,
              (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
              (byte)red,
              (byte)green,
              (byte)blue,
              //(byte)white,
              //Utility.SysExEnd
          };
            if (white.HasValue)
                command.Add((byte)white.Value);

            command.Add(Utility.SysExEnd);

            session.Write(command.ToArray(), 0, command.Count);
            return session.GetMessageFromQueue<PixelColor>().Value;
        }
        /// <summary>
        /// 貌似存在问题
        /// </summary>
        /// <param name="session"></param>
        /// <param name="deviceNumber"></param>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ColorHSV NeoPixelColorHSV(this ArduinoSession session, int deviceNumber, int hue, byte sat, byte val)
        {
            if (deviceNumber < 0 || deviceNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(deviceNumber), "Device number must be between 0 and 9.");

            var bytes = hue.encode32BitSignedInteger();
            var command = new[]
            {
              Utility.SysExStart,
              NEOPIXEL_DATA,
              NEOPIXEL_REPORT_COLORHSV,
              (byte)deviceNumber,//device number(0-9) (Supports up to 10 motors)
              bytes[0], //4  num steps, bits 0-6
              bytes[1], //5  num steps, bits 7-13
              bytes[2], //6  num steps, bits 14-20
              bytes[3], //7  num steps, bits 21-27
              bytes[4], //8  num steps, bits 28-32
              sat,
              val,
              Utility.SysExEnd
          };
            session.Write(command, 0, command.Length);

            return session.GetMessageFromQueue<ColorHSV>().Value;
        }
    }


    public enum NeoPixelType : uint
    {
        NEO_KHZ800 = 0x0000,
        NEO_KHZ400 = 0x0100,

        NEO_RGB = ((0 << 6) | (0 << 4) | (1 << 2) | (2)),
        NEO_RBG = ((0 << 6) | (0 << 4) | (2 << 2) | (1)),
        NEO_GRB = ((1 << 6) | (1 << 4) | (0 << 2) | (2)),
        NEO_GBR = ((2 << 6) | (2 << 4) | (0 << 2) | (1)),
        NEO_BRG = ((1 << 6) | (1 << 4) | (2 << 2) | (0)),
        NEO_BGR = ((2 << 6) | (2 << 4) | (1 << 2) | (0)),

        NEO_WRGB = ((0 << 6) | (1 << 4) | (2 << 2) | (3)),
        NEO_WRBG = ((0 << 6) | (1 << 4) | (3 << 2) | (2)),
        NEO_WGRB = ((0 << 6) | (2 << 4) | (1 << 2) | (3)),
        NEO_WGBR = ((0 << 6) | (3 << 4) | (1 << 2) | (2)),
        NEO_WBRG = ((0 << 6) | (2 << 4) | (3 << 2) | (1)),
        NEO_WBGR = ((0 << 6) | (3 << 4) | (2 << 2) | (1)),

        NEO_RWGB = ((1 << 6) | (0 << 4) | (2 << 2) | (3)),
        NEO_RWBG = ((1 << 6) | (0 << 4) | (3 << 2) | (2)),
        NEO_RGWB = ((2 << 6) | (0 << 4) | (1 << 2) | (3)),
        NEO_RGBW = ((3 << 6) | (0 << 4) | (1 << 2) | (2)),
        NEO_RBWG = ((2 << 6) | (0 << 4) | (3 << 2) | (1)),
        NEO_RBGW = ((3 << 6) | (0 << 4) | (2 << 2) | (1)),

        NEO_GWRB = ((1 << 6) | (2 << 4) | (0 << 2) | (3)),
        NEO_GWBR = ((1 << 6) | (3 << 4) | (0 << 2) | (2)),
        NEO_GRWB = ((2 << 6) | (1 << 4) | (0 << 2) | (3)),
        NEO_GRBW = ((3 << 6) | (1 << 4) | (0 << 2) | (2)),
        NEO_GBWR = ((2 << 6) | (3 << 4) | (0 << 2) | (1)),
        NEO_GBRW = ((3 << 6) | (2 << 4) | (0 << 2) | (1)),

        NEO_BWRG = ((1 << 6) | (2 << 4) | (3 << 2) | (0)),
        NEO_BWGR = ((1 << 6) | (3 << 4) | (2 << 2) | (0)),
        NEO_BRWG = ((2 << 6) | (1 << 4) | (3 << 2) | (0)),
        NEO_BRGW = ((3 << 6) | (1 << 4) | (2 << 2) | (0)),
        NEO_BGWR = ((2 << 6) | (3 << 4) | (1 << 2) | (0)),
        NEO_BGRW = ((3 << 6) | (2 << 4) | (1 << 2) | (0))
    }

    /*

     // RGB NeoPixel permutations; white and red offsets are always same
  // Offset:         W        R        G        B
  #define NEO_RGB  ((0<<6) | (0<<4) | (1<<2) | (2)) ///< Transmit as R,G,B
  #define NEO_RBG  ((0<<6) | (0<<4) | (2<<2) | (1)) ///< Transmit as R,B,G
  #define NEO_GRB  ((1<<6) | (1<<4) | (0<<2) | (2)) ///< Transmit as G,R,B
  #define NEO_GBR  ((2<<6) | (2<<4) | (0<<2) | (1)) ///< Transmit as G,B,R
  #define NEO_BRG  ((1<<6) | (1<<4) | (2<<2) | (0)) ///< Transmit as B,R,G
  #define NEO_BGR  ((2<<6) | (2<<4) | (1<<2) | (0)) ///< Transmit as B,G,R

  // RGBW NeoPixel permutations; all 4 offsets are distinct
  // Offset:         W          R          G          B
  #define NEO_WRGB ((0<<6) | (1<<4) | (2<<2) | (3)) ///< Transmit as W,R,G,B
  #define NEO_WRBG ((0<<6) | (1<<4) | (3<<2) | (2)) ///< Transmit as W,R,B,G
  #define NEO_WGRB ((0<<6) | (2<<4) | (1<<2) | (3)) ///< Transmit as W,G,R,B
  #define NEO_WGBR ((0<<6) | (3<<4) | (1<<2) | (2)) ///< Transmit as W,G,B,R
  #define NEO_WBRG ((0<<6) | (2<<4) | (3<<2) | (1)) ///< Transmit as W,B,R,G
  #define NEO_WBGR ((0<<6) | (3<<4) | (2<<2) | (1)) ///< Transmit as W,B,G,R

  #define NEO_RWGB ((1<<6) | (0<<4) | (2<<2) | (3)) ///< Transmit as R,W,G,B
  #define NEO_RWBG ((1<<6) | (0<<4) | (3<<2) | (2)) ///< Transmit as R,W,B,G
  #define NEO_RGWB ((2<<6) | (0<<4) | (1<<2) | (3)) ///< Transmit as R,G,W,B
  #define NEO_RGBW ((3<<6) | (0<<4) | (1<<2) | (2)) ///< Transmit as R,G,B,W
  #define NEO_RBWG ((2<<6) | (0<<4) | (3<<2) | (1)) ///< Transmit as R,B,W,G
  #define NEO_RBGW ((3<<6) | (0<<4) | (2<<2) | (1)) ///< Transmit as R,B,G,W

  #define NEO_GWRB ((1<<6) | (2<<4) | (0<<2) | (3)) ///< Transmit as G,W,R,B
  #define NEO_GWBR ((1<<6) | (3<<4) | (0<<2) | (2)) ///< Transmit as G,W,B,R
  #define NEO_GRWB ((2<<6) | (1<<4) | (0<<2) | (3)) ///< Transmit as G,R,W,B
  #define NEO_GRBW ((3<<6) | (1<<4) | (0<<2) | (2)) ///< Transmit as G,R,B,W
  #define NEO_GBWR ((2<<6) | (3<<4) | (0<<2) | (1)) ///< Transmit as G,B,W,R
  #define NEO_GBRW ((3<<6) | (2<<4) | (0<<2) | (1)) ///< Transmit as G,B,R,W

  #define NEO_BWRG ((1<<6) | (2<<4) | (3<<2) | (0)) ///< Transmit as B,W,R,G
  #define NEO_BWGR ((1<<6) | (3<<4) | (2<<2) | (0)) ///< Transmit as B,W,G,R
  #define NEO_BRWG ((2<<6) | (1<<4) | (3<<2) | (0)) ///< Transmit as B,R,W,G
  #define NEO_BRGW ((3<<6) | (1<<4) | (2<<2) | (0)) ///< Transmit as B,R,G,W
  #define NEO_BGWR ((2<<6) | (3<<4) | (1<<2) | (0)) ///< Transmit as B,G,W,R
  #define NEO_BGRW ((3<<6) | (2<<4) | (1<<2) | (0)) ///< Transmit as B,G,R,W

  // Add NEO_KHZ400 to the color order value to indicate a 400 KHz device.
  // All but the earliest v1 NeoPixels expect an 800 KHz data stream, this is
  // the default if unspecified. Because flash space is very limited on ATtiny
  // devices (e.g. Trinket, Gemma), v1 NeoPixels aren't handled by default on
  // those chips, though it can be enabled by removing the ifndef/endif below,
  // but code will be bigger. Conversely, can disable the NEO_KHZ400 line on
  // other MCUs to remove v1 support and save a little space.

  #define NEO_KHZ800 0x0000 ///< 800 KHz data transmission
  #ifndef __AVR_ATtiny85__
  #define NEO_KHZ400 0x0100 ///< 400 KHz data transmission
  #endif

     */
}
