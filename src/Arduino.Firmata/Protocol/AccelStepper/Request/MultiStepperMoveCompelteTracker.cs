using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Arduino.Firmata
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
