namespace Arduino.Firmata.Protocol.Firmata
{
    internal class DigitalStateChangeTracker : ObservableEventTracker<FirmataEvint, DigitalPinState>
    {
        #region Fields

        private readonly int _port;

        #endregion

        #region Constructors

        internal DigitalStateChangeTracker(FirmataEvint source, int port = -1)
            : base(source)
        {
            _port = port;
            TrackingSource.DigitalStateChangeReceived += Firmata_DigitalStateChangeReceived;
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                TrackingSource.DigitalStateChangeReceived -= Firmata_DigitalStateChangeReceived;
                base.Dispose();
            }
        }

        #endregion

        #region Private Methods

        void Firmata_DigitalStateChangeReceived(object parSender, DigitalPinState parEventArgs)
        {
            if (_port >= 0 && _port != parEventArgs.Port)
                return;

            Observers.ForEach(o => o.OnNext(parEventArgs));
        }

        #endregion
    }
}
