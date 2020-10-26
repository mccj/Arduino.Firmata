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
    public struct StepperPosition
    {
        public int DeviceNumber { get; set; }
        public long StepsNum { get; set; }
    }
}
