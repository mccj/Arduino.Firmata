namespace Arduino.Firmata.Protocol.Firmata
{
    internal class AnalogStateChangeTracker : ObservableEventTracker<FirmataEvint, AnalogState>
    {
        #region Fields

        private readonly int _channel;

        #endregion

        #region Constructors

        internal AnalogStateChangeTracker(FirmataEvint source, int channel = -1): base(source)
        {
            _channel = channel;
            TrackingSource.AnalogStateChangeReceived += Firmata_AnalogStateChangeReceived;
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                TrackingSource.AnalogStateChangeReceived -= Firmata_AnalogStateChangeReceived;
                base.Dispose();
            }
        }

        #endregion

        #region Private Methods

        void Firmata_AnalogStateChangeReceived(object parSender, FirmataEventArgs<AnalogState> parEventArgs)
        {
            if (_channel >= 0 && _channel != parEventArgs.Value.Channel)
                return;

            Observers.ForEach(o => o.OnNext(parEventArgs.Value));
        }

        #endregion
    }
}
