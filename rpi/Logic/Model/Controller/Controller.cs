using System;
using System.Device.Gpio;
using System.Threading;
using Logic.Model.Common;
using Logic.Model.Pin;
using Timer = System.Timers.Timer;

namespace Logic.Model.Controller
{
    public class Controller : IController
    {
        private readonly GpioController _controller;
        private readonly IPin _motor;
        private readonly IPin _greenLed;
        private readonly IPin _redLed;
        private readonly IPin _bobberFishing;
        private readonly IPin _button;

        public Controller(IPin motor, IPin greenLed, IPin redLed, IPin bobberFishing, IPin button)
        {
            _motor = motor;
            _greenLed = greenLed;
            _redLed = redLed;
            _bobberFishing = bobberFishing;
            _button = button;

            _controller = new GpioController();
            _controller.OpenPin(_motor.GetPin(), PinMode.Output);
            _controller.OpenPin(_greenLed.GetPin(), PinMode.Output);
            _controller.OpenPin(_redLed.GetPin(), PinMode.Output);
            _controller.OpenPin(_bobberFishing.GetPin(), PinMode.Input);
            _controller.OpenPin(_button.GetPin(), PinMode.Input);

        }

        public void SetBobberEvent(Action<PinEventTypes> func)
        {
            _controller.RegisterCallbackForPinValueChangedEvent(_bobberFishing.GetPin(),
                PinEventTypes.Falling | PinEventTypes.Rising,
                (_, args) => { SetTimer(args.ChangeType, func, _bobberFishing.GetPin()); });
        }

        public void SetButtonEvent(Action<PinEventTypes> func)
        {
            _controller.RegisterCallbackForPinValueChangedEvent(_button.GetPin(),
                PinEventTypes.Falling | PinEventTypes.Rising,
                (_, args) => { SetTimer(args.ChangeType, func, _button.GetPin()); });
        }

        private void SetTimer(PinEventTypes type, Action<PinEventTypes> func, int pin)
        {
            PinValue typeValue = type == PinEventTypes.Rising ? 1 : 0;
            Timer aTimer = new Timer(100);
            int sum = 0;
            aTimer.Elapsed += (_, _) =>
            {
                PinValue pinValue = _controller.Read(pin);
                if (sum < 3 && typeValue != pinValue)
                {
                    sum += 1;
                    aTimer.Start();
                }

                if (typeValue == pinValue)
                {
                    func(type);
                }

            };
            aTimer.Enabled = true;
            aTimer.AutoReset = false;
            aTimer.Start();
        }

        public void WriteToMotor(bool turnOn)
        {
            _controller.Write(_motor.GetPin(), turnOn ? PinValue.High : PinValue.Low);
            _controller.Write(_greenLed.GetPin(), turnOn ? PinValue.High : PinValue.Low);
        }

        public void WriteToRedLed(bool turnOn)
        {
            _controller.Write(_redLed.GetPin(), turnOn ? PinValue.High : PinValue.Low);
        }

        public PinValue ReadBobberValue()
        {
            return _controller.Read(_bobberFishing.GetPin());
        }
    }
}