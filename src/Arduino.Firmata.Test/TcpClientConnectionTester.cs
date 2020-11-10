using Arduino.Firmata.Serial;
using Arduino.Firmata.Tcp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Solid.Arduino.Test
{
    [TestClass]
    public class TcpClientConnectionTester
    {
        private string ip = "10.11.201.82";
        [TestMethod]
        public void TcpClientConnection_Constructor_WithoutParameters()
        {
            var connection = new TcpClientConnection(ip);
            //Assert.AreEqual(100, connection.ReadTimeout);
            //Assert.AreEqual(100, connection.WriteTimeout);
            Assert.AreEqual(ip + ":3030", connection.Name);
        }

        [TestMethod]
        public void TcpClientConnection_Constructor_WithParameters()
        {
            var connection = new TcpClientConnection(ip, 3030);
            //Assert.AreEqual(100, connection.ReadTimeout);
            //Assert.AreEqual(100, connection.WriteTimeout);
            Assert.AreEqual(ip + ":3030", connection.Name);
        }

        [TestMethod]
        public void TcpClientConnection_OpenAndClose()
        {
            var connection = new TcpClientConnection(ip);
            connection.Open();
            connection.Close();
            connection.Open();
            connection.Close();
            connection.Dispose();
        }

        [TestMethod]
        public void TcpClientConnection_OpenAndDoubleClose()
        {
            var connection = new TcpClientConnection(ip);
            connection.Open();
            connection.Close();
            connection.Open();
            connection.Close();
            connection.Close();
            connection.Dispose();
        }
    }
}
