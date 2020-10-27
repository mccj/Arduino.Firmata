namespace Arduino.Firmata.Protocol.String
{
    internal class ReceivedStringTracker : ObservableEventTracker<StringEvint, string>
    {
        #region Constructors

        internal ReceivedStringTracker(StringEvint source)
            : base(source)
        {
            TrackingSource.StringReceived += TrackingSource_StringReceived;
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                TrackingSource.StringReceived -= TrackingSource_StringReceived;
                base.Dispose();
            }
        }

        #endregion

        #region Private Methods

        void TrackingSource_StringReceived(object parSender, StringEventArgs parEventArgs)
        {
            Observers.ForEach(o => o.OnNext(parEventArgs.Text));
        }

        #endregion
    }
}
