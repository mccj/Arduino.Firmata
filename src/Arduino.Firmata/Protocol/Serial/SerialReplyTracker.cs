using Arduino.Firmata.Protocol.Firmata;

namespace Arduino.Firmata.Protocol.Serial
{
    internal class SerialReplyTracker : ObservableEventTracker<SerialEvint, SerialReply>
    {
       #region Constructors

        internal SerialReplyTracker(SerialEvint source): base(source)
        {
            TrackingSource.SerialReplyReceived += Firmata_SerialReplyReceived;
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                TrackingSource.SerialReplyReceived -= Firmata_SerialReplyReceived;
                base.Dispose();
            }
        }

        #endregion

        #region Private Methods

        void Firmata_SerialReplyReceived(object parSender, SerialEventArgs parEventArgs)
        {
           Observers.ForEach(o => o.OnNext(parEventArgs.Value));
        }

        #endregion
    }
}
