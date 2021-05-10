using System.Device.Gpio;

namespace Logic.Model.RedLed
{
    public class RedLed : IRedLed
    {
        private readonly int _pin;
        private PinValue _value;

        public RedLed(int pin)
        {
            _pin = pin;
            _value = PinValue.Low;
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
    }
}