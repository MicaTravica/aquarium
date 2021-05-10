using System.Device.Gpio;
using Logic.Model.BobberFishing;
using Logic.Model.Button;
using Logic.Model.GreenLed;
using Logic.Model.Motor;
using Logic.Model.RedLed;

namespace Logic.Model.Controller
{
    public class Controller : IController
    {
        private GpioController _controller;
        private IMotor _motor;
        private IGreenLed _greenLed;
        private IRedLed _redLed;

        public Controller(IMotor motor, IGreenLed greenLed, IRedLed redLed, IBobberFishing bobberFishing,
            IButton button)
        {
            _motor = motor;
            _greenLed = greenLed;
            _redLed = redLed;
            _controller = new GpioController();
            _controller.OpenPin(_motor.GetPin(), PinMode.Output);
            _controller.OpenPin(_greenLed.GetPin(), PinMode.Output);
            _controller.OpenPin(_redLed.GetPin(), PinMode.Output);
            _controller.OpenPin(bobberFishing.GetPin(), PinMode.Input);
            _controller.OpenPin(button.GetPin(), PinMode.Input);
            _controller.RegisterCallbackForPinValueChangedEvent(bobberFishing.GetPin(), PinEventTypes.Rising,
                (sender, args) => { bobberFishing.SetValue(PinValue.High); });
            _controller.RegisterCallbackForPinValueChangedEvent(button.GetPin(), PinEventTypes.Rising,
                (sender, args) => { button.SetValue(PinValue.High); });
            _controller.RegisterCallbackForPinValueChangedEvent(bobberFishing.GetPin(), PinEventTypes.Falling,
                (sender, args) => { bobberFishing.SetValue(PinValue.Low); });
            _controller.RegisterCallbackForPinValueChangedEvent(button.GetPin(), PinEventTypes.Falling,
                (sender, args) => { button.SetValue(PinValue.Low); });
        }

        public void WriteToMotor(bool turnOn)
        {
            _controller.Write(_motor.GetPin(), turnOn ? PinValue.High : PinValue.Low);
            _controller.Write(_greenLed.GetPin(), turnOn ? PinValue.High : PinValue.Low);
            _motor.SetValue(turnOn ? PinValue.High : PinValue.Low);
        }

        public void WriteToRedLed(bool turnOn)
        {
            _controller.Write(_redLed.GetPin(), turnOn ? PinValue.High : PinValue.Low);
            _redLed.SetValue(turnOn ? PinValue.High : PinValue.Low);
        }
    }
}