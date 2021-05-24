namespace Logic.Model.State
{
    public interface IState
    {
        void TurnOn();
        void TurnOff();
        void ButtonPressed();
        void ButtonReleased();
        void ButtonHold();
        void BobberIsUp();
        void BobberIsDown();

    }
}