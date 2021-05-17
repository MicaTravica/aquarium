namespace Logic.Model.Pin
{
    public class Pin : IPin
    {
        private readonly int _pin;
        public Pin(int pin)
        {
            _pin = pin;
        }

        public int GetPin()
        {
            return _pin;
        }
    }
}