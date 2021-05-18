using System;
using Logic.Model.Message;
using Logic.Model.MessageBus;
using Logic.Model.StateMachine;

namespace ConsoleApp
{ 
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
}