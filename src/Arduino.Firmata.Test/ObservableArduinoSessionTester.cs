using System;
using System.Linq;
using Arduino.Firmata;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Solid.Arduino;

namespace Solid.Arduino.Test
{
    [TestClass]
    public class ObservableArduinoSessionTester
    {
        [TestMethod]
        public void TestMethod1()
        {
            var x = new Firmata.ArduinoSession(new MockSerialConnection());

            var tracker = x.CreateAnalogStateMonitor();
            
        }
    }
}
