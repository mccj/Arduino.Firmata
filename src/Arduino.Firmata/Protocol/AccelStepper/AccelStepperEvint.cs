using Arduino.Firmata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Solid.Arduino.Firmata
{
    /// <summary>
    /// Signature of event handlers capable of processing analog I/O messages.
    /// </summary>
    /// <param name="sender">The object raising the event</param>
    /// <param name="eventArgs">Event arguments holding a <see cref="AnalogState"/></param>
    public delegate void StepperPositionReceivedHandler(object sender, FirmataEventArgs<StepperPosition> eventArgs);

    /// <summary>
    /// Signature of event handlers capable of processing digital I/O messages.
    /// </summary>
    /// <param name="sender">The object raising the event</param>
    /// <param name="eventArgs">Event arguments holding a <see cref="DigitalPortState"/></param>
    public delegate void MultiStepperMoveCompelteReceivedHandler(object sender, FirmataEventArgs<int> eventArgs);

    public class AccelStepperEvint
    {
        private ArduinoSession session;

        public AccelStepperEvint(ArduinoSession session)
        {
            this.session = session;
        }

        public event StepperPositionReceivedHandler StepperPositionReceived;
        public event StepperPositionReceivedHandler StepperMoveCompleteReceived;
        public event MultiStepperMoveCompelteReceivedHandler MultiStepperMoveCompelteReceived;
        internal void OnStepperPositionReceived(FirmataEventArgs<StepperPosition> eventArgs)
        {//StepperPosition
            StepperPositionReceived?.Invoke(session, eventArgs);
        }
        internal void OnStepperMoveCompleteReceived(FirmataEventArgs<StepperPosition> eventArgs)
        {//StepperMoveComplete
            StepperMoveCompleteReceived?.Invoke(session, eventArgs);
        }
        internal void OnMultiStepperMoveCompelteReceived(FirmataEventArgs<int> eventArgs)
        {//MultiStepperMoveCompelte
            MultiStepperMoveCompelteReceived?.Invoke(session, eventArgs);
        }

    }
}
