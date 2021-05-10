using System.Device.Gpio;

namespace Logic.Model.BobberFishing
{
    public class BobberFishing : IBobberFishing
    {
        private readonly int _pin;
        private PinValue _value;
        private int _count;

        public BobberFishing(int pin)
        {
            _pin = pin;
            _value = PinValue.Low;
            _count = 0;
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

        public int GetCount()
        {
            return _count;
        }

        public void IncCount()
        {
            _count += 1;
        }

        public void ResetCount()
        {
            _count = 0;
        }
    }
}