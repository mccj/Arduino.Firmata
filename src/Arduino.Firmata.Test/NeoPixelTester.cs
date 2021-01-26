using Arduino.Firmata;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class NeoPixelTester
    {
        [TestMethod]
        public void test1()
        {
            var s1 = (int)(Arduino.Firmata.Protocol.NeoPixel.NeoPixelType.NEO_KHZ400 | Arduino.Firmata.Protocol.NeoPixel.NeoPixelType.NEO_BGR);
            var s2 = ((int)Arduino.Firmata.Protocol.NeoPixel.NeoPixelType.NEO_KHZ400) + ((int)Arduino.Firmata.Protocol.NeoPixel.NeoPixelType.NEO_BGR);
            var dd = s1 == s2;
        }
    }
}
