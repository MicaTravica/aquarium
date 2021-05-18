using System;

namespace ConsoleApp
{
    public sealed class StartProcessing : IMessage {}

    public sealed class ButtonPressed : IMessage
    {
    }

    public sealed class ButtonReleased : IMessage
    {
    }

    public sealed class EightSecondsPassed : IMessage
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            var processor = new Processor();
            var stateMachine = new StateMachine(processor);
            
            processor.StartProcessing(stateMachine.ProcessSingle);
            Console.ReadLine();
            processor.StopProcessing();
        }
    }

    internal sealed class StateMachine
    {
        private readonly IMessageBus _messageBus;

        public StateMachine(IMessageBus messageBus)
        {
            _messageBus = messageBus;
            _messageBus.Enqueue(new StartProcessing());
        }
        
        public void ProcessSingle(IMessage message)
        {
            switch (message)
            {
                case ButtonPressed buttonPressed:
                    break;
                case ButtonReleased buttonReleased:
                    break;
                case EightSecondsPassed eightSecondsPassed:
                    break;
                case StartProcessing startProcessing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message));
            }
        }
    }
}