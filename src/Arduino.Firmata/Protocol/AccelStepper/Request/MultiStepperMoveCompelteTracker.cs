using Arduino.Firmata.Protocol.Firmata;

namespace Arduino.Firmata.Protocol.AccelStepper
{
    internal class MultiStepperMoveCompelteTracker : ObservableEventTracker<AccelStepperEvint, int>
    {
       #region Constructors

        internal MultiStepperMoveCompelteTracker(AccelStepperEvint source): base(source)
        {
            TrackingSource.MultiStepperMoveCompelteReceived += Firmata_MultiStepperMoveCompelteReceived;
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                TrackingSource.MultiStepperMoveCompelteReceived -= Firmata_MultiStepperMoveCompelteReceived;
                base.Dispose();
            }
        }

        #endregion

        #region Private Methods

        void Firmata_MultiStepperMoveCompelteReceived(object parSender, FirmataEventArgs<int> parEventArgs)
        {
           Observers.ForEach(o => o.OnNext(parEventArgs.Value));
        }

        #endregion
    }
}
