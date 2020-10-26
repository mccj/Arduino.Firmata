using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Arduino.Firmata.Connection.Serial;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Solid.Arduino;

namespace Solid.Arduino.Test
{
    [TestClass]
    public class EnhancedSerialConnectionTester
    {
        [TestMethod]
        public void EnhancedSerialConnection_Constructor_WithoutParameters()
        {
            var connection = new SerialConnection();
            //Assert.AreEqual(100, connection.ReadTimeout);
            //Assert.AreEqual(100, connection.WriteTimeout);
            Assert.AreEqual(115200, connection.BaudRate);
        }

        [TestMethod]
        public void EnhancedSerialConnection_Constructor_WithParameters()
        {
            var connection = new SerialConnection("COM1", SerialBaudRate.Bps_115200);
            //Assert.AreEqual(100, connection.ReadTimeout);
            //Assert.AreEqual(100, connection.WriteTimeout);
            Assert.AreEqual(115200, connection.BaudRate);
        }

        [TestMethod]
        public void EnhancedSerialConnection_OpenAndClose()
        {
            var connection = new SerialConnection();
            connection.Open();
            connection.Close();
        }

        [TestMethod]
        public void EnhancedSerialConnection_OpenAndDoubleClose()
        {
            var connection = new SerialConnection();
            connection.Open();
            connection.Close();
            connection.Close();
        }
    }
}
