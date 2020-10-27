using Arduino.Firmata.Protocol.Firmata;

namespace Arduino.Firmata.Protocol.AccelStepper
{
    internal class StepperMoveCompleteTracker : ObservableEventTracker<AccelStepperEvint, StepperPosition>
    {
       #region Constructors

        internal StepperMoveCompleteTracker(AccelStepperEvint source): base(source)
        {
            TrackingSource.StepperMoveCompleteReceived += Firmata_StepperMoveCompleteReceived;
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                TrackingSource.StepperMoveCompleteReceived -= Firmata_StepperMoveCompleteReceived;
                base.Dispose();
            }
        }

        #endregion

        #region Private Methods

        void Firmata_StepperMoveCompleteReceived(object parSender, FirmataEventArgs<StepperPosition> parEventArgs)
        {
           Observers.ForEach(o => o.OnNext(parEventArgs.Value));
        }

        #endregion
    }
}
