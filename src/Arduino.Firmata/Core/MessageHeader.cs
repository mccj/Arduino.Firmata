using Arduino.Firmata.Protocol.Firmata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Arduino.Firmata
{
    /// <summary>
    /// Defines a serial port connection.
    /// </summary>
    /// <seealso href="http://arduino.cc/en/Reference/Serial">Serial reference for Arduino</seealso>
    public class MessageHeader : IDisposable
    {
        #region Type declarations

        public delegate void ProcessMessageHandler(int messageByte);
        internal enum StringReadMode
        {
            ReadLine,
            ReadToTerminator,
            ReadBlock
        }

        internal class StringRequest
        {
            private readonly StringReadMode _mode;
            private readonly int _blockLength;
            private readonly char _terminator;

            public static StringRequest CreateReadLineRequest()
            {
                return new StringRequest(StringReadMode.ReadLine, '\\', 0);
            }

            public static StringRequest CreateReadRequest(int blockLength)
            {
                return new StringRequest(StringReadMode.ReadBlock, '\\', blockLength);
            }

            public static StringRequest CreateReadRequest(char terminator)
            {
                return new StringRequest(StringReadMode.ReadToTerminator, terminator, 0);
            }

            private StringRequest(StringReadMode mode, char terminator, int blockLength)
            {
                _mode = mode;
                _blockLength = blockLength;
                _terminator = terminator;
            }

            public char Terminator { get { return _terminator; } }
            public int BlockLength { get { return _blockLength; } }
            public StringReadMode Mode { get { return _mode; } }
        }
        #endregion

        #region Fields
        public ArduinoSession _arduinoSession;


        private const int Buffersize = 2048;
        public const int MaxQueuelength = 100;

        protected readonly IDataConnection _connection;
        private readonly bool _gotOpenConnection;
        public readonly LinkedList<IFirmataMessage> _receivedMessageList = new LinkedList<IFirmataMessage>();
        private readonly Queue<string> _receivedStringQueue = new Queue<string>();
        //private ConcurrentQueue<IFirmataMessage> _awaitedMessagesQueue = new ConcurrentQueue<IFirmataMessage>();
        private ConcurrentQueue<StringRequest> _awaitedStringsQueue = new ConcurrentQueue<StringRequest>();
        private StringRequest _currentStringRequest;

        private int _messageTimeout = -1;
        public ProcessMessageHandler _processMessage;
        public int _messageBufferIndex, _stringBufferIndex;
        public readonly int[] _messageBuffer = new int[Buffersize];
        private readonly char[] _stringBuffer = new char[Buffersize];
        #endregion
        #region Constructors
        public MessageHeader(ArduinoSession arduinoSession, IDataConnection connection)
        {
            _arduinoSession = arduinoSession;

            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            _connection = connection;
            _gotOpenConnection = connection.IsOpen;

            if (!connection.IsOpen)
                connection.Open();

            _connection.DataReceived += SerialDataReceived;
        }
        #endregion
        public IDataConnection Connection => _connection;
        /// <summary>
        /// Gets or sets the number of milliseconds before a time-out occurs when a read operation does not finish.
        /// </summary>
        /// <remarks>
        /// The default is a <see cref="SerialPort.InfiniteTimeout"/> value (-1).
        /// </remarks>
        public int TimeOut
        {
            get => _messageTimeout;
            set
            {
                if (value < _connection.InfiniteTimeout)
                    throw new ArgumentOutOfRangeException();

                _messageTimeout = value;
            }
        }
        #region IDisposable

        public void Dispose()
        {
            if (!_gotOpenConnection)
                _connection.Close();

            GC.SuppressFinalize(this);
        }

        #endregion

        public void SerialDataReceived(object sender/*, SerialDataReceivedEventArgs e*/)
        {
            while (_connection.IsOpen && _connection.BytesToRead > 0)
            {
                int serialByte = _connection.ReadByte();

#if DEBUG
                if (_messageBufferIndex > 0 && _messageBufferIndex % 8 == 0)
                    Debug.WriteLine(string.Empty);

                Debug.Write(string.Format("{0:x2} ", serialByte));
#endif

                if (_processMessage != null)
                {
                    _processMessage(serialByte);
                }
                else
                {
                    if ((serialByte & 0x80) != 0)
                    {
                        // Process Firmata command byte.
                        ProcessCommand(serialByte);
                    }
                    else
                    {
                        // Process ASCII character.
                        ProcessAsciiString(serialByte);
                    }
                }
            }
        }

        //public void AddToMessageBuffer(int dataByte)
        //{
        //    if (_messageBufferIndex == MessageHeader.BufferSize)
        //        throw new OverflowException(Messages.OverflowEx_CmdBufferFull);

        //    _messageBuffer[_messageBufferIndex] = dataByte;
        //    _messageBufferIndex++;
        //}
        //public IFirmataMessage WaitForMessageFromQueue(Func<IFirmataMessage, bool> messagePredicate, int timeOutInMs)
        //{
        //    var lockTaken = false;

        //    try
        //    {
        //        var stopwatch = Stopwatch.StartNew();
        //        var remainingTime = timeOutInMs;

        //        Monitor.TryEnter(_receivedMessageList, remainingTime, ref lockTaken);
        //        while (lockTaken)
        //        {
        //            if (_receivedMessageList.Count > 0)
        //            {
        //                var message = (from firmataMessage in _receivedMessageList
        //                               where messagePredicate(firmataMessage)
        //                               select firmataMessage).FirstOrDefault();

        //                if (message != null)
        //                {
        //                    _receivedMessageList.Remove(message);
        //                    Monitor.PulseAll(_receivedMessageList);
        //                    return message;
        //                }
        //            }

        //            remainingTime = (int)(timeOutInMs - stopwatch.ElapsedMilliseconds);
        //            if (remainingTime > 0 || timeOutInMs == -1)
        //            {
        //                lockTaken = Monitor.Wait(_receivedMessageList, remainingTime);
        //            }
        //            else
        //            {
        //                Monitor.PulseAll(_receivedMessageList);
        //                return null;
        //            }
        //        }

        //        Monitor.PulseAll(_receivedMessageList);
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Monitor.PulseAll(_receivedMessageList);
        //        return null;
        //    }
        //    finally
        //    {
        //        if (lockTaken)
        //        {
        //            Monitor.Exit(_receivedMessageList);
        //        }
        //    }
        //}

        private void ProcessAsciiString(int serialByte)
        {
            if (_stringBufferIndex == Buffersize)
                throw new OverflowException(Messages.OverflowEx_StringBufferFull);

            char c = Convert.ToChar(serialByte);
            _stringBuffer[_stringBufferIndex] = c;
            _stringBufferIndex++;

            if (_currentStringRequest == null)
            {
                if (!_awaitedStringsQueue.TryDequeue(out _currentStringRequest))
                {
                    // No pending Read/ReadLine/ReadTo requests.
                    // Handle StringReceived event.
                    if (c == _connection.NewLine[_connection.NewLine.Length - 1]
                        || serialByte == 0x1A
                        || serialByte == 0x00) // NewLine, EOF or terminating 0-byte?
                    {
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //if (StringReceived != null)
                        //    StringReceived(this, new StringEventArgs(new string(_stringBuffer, 0, _stringBufferIndex - 1)));

                        _stringBufferIndex = 0;
                    }
                    return;
                }
            }

            switch (_currentStringRequest.Mode)
            {
                case StringReadMode.ReadLine:
                    if (c == _connection.NewLine[0] || serialByte == 0x1A)
                        EnqueueReceivedString(new string(_stringBuffer, 0, _stringBufferIndex - 1));
                    else if (c == '\n') // Ignore linefeed, just in case cr+lf pair was expected.
                        _stringBufferIndex--;
                    break;

                case StringReadMode.ReadBlock:
                    if (_stringBufferIndex == _currentStringRequest.BlockLength)
                        EnqueueReceivedString(new string(_stringBuffer, 0, _stringBufferIndex));
                    break;

                case StringReadMode.ReadToTerminator:
                    if (c == _currentStringRequest.Terminator)
                        EnqueueReceivedString(new string(_stringBuffer, 0, _stringBufferIndex - 1));
                    break;
            }
        }

        /// <summary>
        /// Closes and reopens the underlying connection and clears all buffers and queues.
        /// </summary>
        public void Clear()
        {
            lock (_receivedMessageList)
            {
                _connection.Close();
                _receivedMessageList.Clear();
                _processMessage = null;
                //_awaitedMessagesQueue = new ConcurrentQueue<IFirmataMessage>();
                _awaitedStringsQueue = new ConcurrentQueue<StringRequest>();
                _connection.Open();
            }
        }

        private void EnqueueReceivedString(string value)
        {
            bool lockTaken = false;

            try
            {
                Monitor.TryEnter(_receivedStringQueue, _messageTimeout, ref lockTaken);

                if (!lockTaken)
                    throw new TimeoutException();

                if (_receivedStringQueue.Count >= MaxQueuelength)
                    throw new OverflowException(Messages.OverflowEx_StringBufferFull);

                _receivedStringQueue.Enqueue(value);
                Monitor.PulseAll(_receivedStringQueue);
                _currentStringRequest = null;
                _stringBufferIndex = 0;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_receivedStringQueue);
            }
        }

        private void ProcessCommand(int serialByte)
        {
            _messageBuffer[0] = serialByte;
            _messageBufferIndex = 1;
            FirmataMessageHeader.Header(serialByte, this);


            //MessageHeader header = (MessageHeader)(serialByte & 0xF0);

            //            switch (header)
            //            {
            //                case MessageHeader.AnalogState:
            //                    _processMessage = ProcessAnalogStateMessage;
            //                    break;

            //                case MessageHeader.DigitalState:
            //                    _processMessage = ProcessDigitalStateMessage;
            //                    break;

            //                case MessageHeader.SystemExtension:
            //                    header = (MessageHeader)serialByte;

            //                    switch (header)
            //                    {
            //                        case MessageHeader.SystemExtension:
            //                            _processMessage = ProcessSysExMessage;
            //                            break;

            //                        case MessageHeader.ProtocolVersion:
            //                            _processMessage = ProcessProtocolVersionMessage;
            //                            break;

            //                        //case MessageHeader.SetPinMode:
            //                        //case MessageHeader.SystemReset:
            //                        default:
            //                            // 0xF? command not supported.
            //                            throw new NotImplementedException(string.Format(Messages.NotImplementedEx_Command, serialByte));
            //                    }
            //                    break;

            //                default:
            //                    // Command not supported.
            //                    throw new NotImplementedException(string.Format(Messages.NotImplementedEx_Command, serialByte));
            //            }
        }
        public FirmataMessage<T> GetMessageFromQueue<T>() where T : struct
        {
            var message = GetMessageFromQueue(firmataMessage => firmataMessage.GetType() == typeof(FirmataMessage<T>));
            if (message is FirmataMessage<T> result)
                return result;

            throw new TimeoutException(string.Format(Messages.TimeoutEx_WaitMessage, typeof(T).Name));
        }
        public IFirmataMessage GetMessageFromQueue(Func<IFirmataMessage, bool> messagePredicate)
        {
            //_awaitedMessagesQueue.Enqueue(awaitedMessage);
            bool lockTaken = false;

            try
            {
                Monitor.TryEnter(_receivedMessageList, _messageTimeout, ref lockTaken);

                while (lockTaken)
                {
                    if (_receivedMessageList.Count > 0)
                    {
                        var message = (from firmataMessage in _receivedMessageList
                                           //where firmataMessage.Type == awaitedMessage.Type
                                       where messagePredicate(firmataMessage)
                                       select firmataMessage).FirstOrDefault();
                        if (message != null)
                        {
                            //if (_receivedMessageQueue.Count > 0
                            //    && _receivedMessageQueue.Select( fm => fm.Type == awaitedMessage.Type).First()) // .Find(FirmataMessage =>) .First().Type == awaitedMessage.Type)
                            //{
                            //FirmataMessage message = _receivedMessageQueue.First.Value;
                            //_receivedMessageQueue.RemoveFirst();
                            _receivedMessageList.Remove(message);
                            Monitor.PulseAll(_receivedMessageList);
                            return message;
                        }
                    }

                    lockTaken = Monitor.Wait(_receivedMessageList, _messageTimeout);
                }

                return null;
                //throw new TimeoutException(string.Format(Messages.TimeoutEx_WaitMessage, awaitedMessage.Type));
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_receivedMessageList);
                }
            }
        }
        public void WriteMessageByte(int dataByte)
        {
            if (_messageBufferIndex == Buffersize)
                throw new OverflowException(Messages.OverflowEx_CmdBufferFull);

            _messageBuffer[_messageBufferIndex] = dataByte;
            _messageBufferIndex++;
        }
        internal string GetStringFromQueue(StringRequest request)
        {
            _awaitedStringsQueue.Enqueue(request);
            bool lockTaken = false;

            try
            {
                Monitor.TryEnter(_receivedStringQueue, _messageTimeout, ref lockTaken);

                while (lockTaken)
                {
                    if (_receivedStringQueue.Count > 0)
                    {
                        string message = _receivedStringQueue.Dequeue();
                        Monitor.PulseAll(_receivedStringQueue);
                        return message;
                    }

                    lockTaken = Monitor.Wait(_receivedStringQueue, _messageTimeout);
                }

                throw new TimeoutException(string.Format(Messages.TimeoutEx_WaitStringRequest, request.Mode));
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_receivedStringQueue);
                }
            }
        }

    }
}
