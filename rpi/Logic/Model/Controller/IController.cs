using System;
using System.Device.Gpio;
using Logic.Model.Common;

namespace Logic.Model.Controller
{
    public interface IController
    {
        void SetBobberEvent(Action<PinEventTypes> func);
        void SetButtonEvent(Action<PinEventTypes> func);
        void WriteToMotor(bool turnOn);
        void WriteToRedLed(bool turnOn);
        PinValue ReadBobberValue();
    }
}