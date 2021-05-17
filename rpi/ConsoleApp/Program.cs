using System;
using Logic.Constants;
using Logic.Model.Pin;
using Logic.Model.Controller;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IPin motor = new Pin(ConstantsRPI.Motor);
            IPin greenLed = new Pin(ConstantsRPI.LedGreen);
            IPin redLed = new Pin(ConstantsRPI.LedRed);
            IPin bobberFishing = new Pin(ConstantsRPI.BobberFishing);
            IPin button = new Pin(ConstantsRPI.Button);

            IController controller = new Controller(motor, greenLed, redLed, bobberFishing, button);
            controller.Start();

            Console.ReadLine();
        }
    }
}