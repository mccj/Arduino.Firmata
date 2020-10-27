namespace Arduino.Firmata.Protocol.I2C
{
    internal class I2CReplyTracker : ObservableEventTracker<I2CEvint, I2CReply>
    {
        #region Constructors

        internal I2CReplyTracker(I2CEvint ii2C): base(ii2C)
        {
            TrackingSource.I2CReplyReceived += I2CReplyReceived;
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                TrackingSource.I2CReplyReceived -= I2CReplyReceived;
                base.Dispose();
            }
        }

        #endregion

        #region Private Methods

        private void I2CReplyReceived(object parSender, I2CEventArgs parEventArgs)
        {
            Observers.ForEach(o => o.OnNext(parEventArgs.Value));
        }

        #endregion
    }
}
