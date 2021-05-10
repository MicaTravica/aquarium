namespace Logic.Model.Controller
{
    public interface IController
    {
        void WriteToMotor(bool turnOn);
        void WriteToRedLed(bool turnOn);
    }
}