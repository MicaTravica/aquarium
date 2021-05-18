using System.Device.Gpio;

namespace Logic.Model.OutputPin
{
    public class OutputPin : IOutputPin
    {
        private readonly GpioController _controller;
        private readonly int _pin;

        public OutputPin(GpioController controller, int pin)
        {
            _controller = controller;
            _pin = pin;
            _controller.OpenPin(_pin, PinMode.Output);
        }

        public void WriteToPin(bool isOn)
        {
            _controller.Write(_pin, isOn ? PinValue.High : PinValue.Low);
        }
    }
}