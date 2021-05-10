namespace Logic.Model.GreenLed
{
    public class GreenLed : IGreenLed
    {
        private readonly int _pin;

        public GreenLed(int pin)
        {
            _pin = pin;
        }

        public int GetPin()
        {
            return _pin;
        }
    }
}