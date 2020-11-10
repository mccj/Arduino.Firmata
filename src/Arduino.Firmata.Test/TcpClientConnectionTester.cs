using Arduino.Firmata.Serial;
using Arduino.Firmata.Tcp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Solid.Arduino.Test
{
    [TestClass]
    public class TcpClientConnectionTester
    {
        [TestMethod]
        public void TcpClientConnection_Constructor_WithoutParameters()
        {
            var connection = new TcpClientConnection("10.11.201.235");
            //Assert.AreEqual(100, connection.ReadTimeout);
            //Assert.AreEqual(100, connection.WriteTimeout);
            Assert.AreEqual("10.11.201.235:3030", connection.Name);
        }

        [TestMethod]
        public void TcpClientConnection_Constructor_WithParameters()
        {
            var connection = new TcpClientConnection("10.11.201.235", 3030);
            //Assert.AreEqual(100, connection.ReadTimeout);
            //Assert.AreEqual(100, connection.WriteTimeout);
            Assert.AreEqual("10.11.201.235:3030", connection.Name);
        }

        [TestMethod]
        public void TcpClientConnection_OpenAndClose()
        {
            var connection = new TcpClientConnection("10.11.201.235");
            connection.Open();
            connection.Close();
            connection.Open();
            connection.Close();
        }

        [TestMethod]
        public void TcpClientConnection_OpenAndDoubleClose()
        {
            var connection = new TcpClientConnection("10.11.201.235");
            connection.Open();
            connection.Close();
            connection.Open();
            connection.Close();
            connection.Close();
        }
    }
}
