using System.Device.Gpio;

namespace Logic.Model.Button
{
    public class Button : IButton
    {
        private readonly int _pin;
        private PinValue _value;
        private PinValue _lastValue;

        public Button(int pin)
        {
            _pin = pin;
            _value = PinValue.Low;
            _lastValue = PinValue.Low;
        }

        public int GetPin()
        {
            return _pin;
        }

        public PinValue GetValue()
        {
            return _value;
        }

        public void SetValue(PinValue pinValue)
        {
            _value = pinValue;
        }

        public PinValue GetLastValue()
        {
            return _lastValue;
        }

        public void SetLastValue()
        {
            _lastValue = _value;
        }
        public void ResetLastValue()
        {
            _lastValue = PinValue.Low;
        }   
    }
}