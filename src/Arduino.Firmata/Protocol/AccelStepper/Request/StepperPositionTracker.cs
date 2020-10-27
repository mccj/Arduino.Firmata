using Arduino.Firmata.Protocol.Firmata;

namespace Arduino.Firmata.Protocol.AccelStepper
{
    internal class StepperPositionTracker : ObservableEventTracker<AccelStepperEvint, StepperPosition>
    {
       #region Constructors

        internal StepperPositionTracker(AccelStepperEvint source): base(source)
        {
            TrackingSource.StepperPositionReceived += Firmata_StepperPositionReceived;
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                TrackingSource.StepperMoveCompleteReceived -= Firmata_StepperPositionReceived;
                base.Dispose();
            }
        }

        #endregion

        #region Private Methods

        void Firmata_StepperPositionReceived(object parSender, FirmataEventArgs<StepperPosition> parEventArgs)
        {
           Observers.ForEach(o => o.OnNext(parEventArgs.Value));
        }

        #endregion
    }
}
