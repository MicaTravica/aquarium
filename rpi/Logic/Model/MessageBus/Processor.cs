using System;
using System.Collections.Concurrent;
using System.Threading;
using Logic.Model.Message;

namespace Logic.Model.MessageBus
{
    public sealed class Processor : IMessageBus
    {
        private Action<IMessage> _messageProcessor = _ => { };
        private readonly BlockingCollection<IMessage> _queue = new();
        private readonly Thread _workerThread;

        public Processor()
        {
            _workerThread = new Thread(Worker);
        }

        private void Worker()
        {
            while (!_queue.IsCompleted)
            {
                if (_queue.Count > 0)
                {
                    var message = _queue.Take();
                    _messageProcessor(message);
                }
            }
        }

        public void StartProcessing(Action<IMessage> messageProcessor)
        {
            _messageProcessor = messageProcessor;
            _workerThread.Start();
        }

        public void StopProcessing()
        {
            _queue.CompleteAdding();
            _workerThread.Join();
        }

        public void Enqueue(IMessage message)
        {
            if (!_queue.IsAddingCompleted)
            {
                _queue.Add(message);
            }
        }
    }
}