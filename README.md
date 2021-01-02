# Arduino.Firmata

Arduino.Firmata is a client library built on the .NET Framework 4.5 and providing an easy way to interact with Arduino boards.
The library implements the main communication protocols, the first of which is the Firmata protocol.
It aims to make communication with Arduino boards in MS .NET projects easier
through a comprehensive and consistent set of methods and events.

The library supports the following protocols:

1. Serial (ASCII) messaging
2. Firmata
3. I2C (as it has become part of Firmata)

All protocols can be mixed. The library brokers all incoming message types
and directs them to the appropriate requestors (synchronous as well as asynchronous).

Currently [Standard Firmata 2.5](https://github.com/firmata/protocol/blob/master/protocol.md) is supported.
(Extra capabilities of Standard Firmata Plus and Configurable Firmata are currently not supported by this client library.)


https://github.com/firmata/protocol
https://github.com/firmata/ConfigurableFirmata
http://firmatabuilder.com/
