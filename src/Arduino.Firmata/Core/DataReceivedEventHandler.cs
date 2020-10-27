namespace Arduino.Firmata
{
    /// <summary>
    /// Represents the method that will handle the <see cref="IDataConnection.DataReceived" /> event of a <see cref="IDataConnection" /> object.
    /// </summary>
    /// <param name="sender">The sender of the event, which is the <see cref="IDataConnection" /> object. </param>
    /// <param name="e">A <see cref="T:System.IO.Ports.SerialDataReceivedEventArgs" /> object that contains the event data. </param>
    public delegate void DataReceivedEventHandler(object sender/*, DataReceivedEventArgs e*/);
}