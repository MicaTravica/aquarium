using Logic.Model.Message;

namespace Logic.Model.State
{
    public class StandBy : IState
    {
        private readonly StateMachine.StateMachine _stateMachine;

        public StandBy(StateMachine.StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void TurnOn()
        {
            _stateMachine.WriteToRedLed(true);
            _stateMachine.SetTimer(new TurnOff(), 250);
        }

        public void TurnOff()
        {
            _stateMachine.WriteToRedLed(false);
            _stateMachine.SetTimer(new TurnOn(), 5000);
        }

        public void ButtonPressed()
        {
            _stateMachine.StopTimer();
            _stateMachine.WriteToRedLed(false);
            _stateMachine.NotHolding();
            _stateMachine.ChangeState(new MotorActive(_stateMachine));   
        }

        public void ButtonReleased()
        {
        }

        public void ButtonHold()
        {
        }

        public void BobberIsUp()
        {
            _stateMachine.StopTimer();
            _stateMachine.ResetCycle();
            _stateMachine.WriteToRedLed(false);
            _stateMachine.ChangeState(new BobberActive(_stateMachine));
        }

        public void BobberIsDown()
        {
        }
        
    }
}
