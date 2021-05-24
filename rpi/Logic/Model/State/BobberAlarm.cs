using Logic.Model.Message;

namespace Logic.Model.State
{
    public class BobberAlarm : IState
    {
        private readonly StateMachine.StateMachine _stateMachine;

        public BobberAlarm(StateMachine.StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void TurnOn()
        {
            _stateMachine.WriteToRedLed(true);
            _stateMachine.SetTimer(new TurnOff(), 500);
        }

        public void TurnOff()
        {
            _stateMachine.WriteToRedLed(false);
            _stateMachine.SetTimer(new TurnOn(), 1000);
        }

        public void ButtonPressed()
        {
            _stateMachine.StopTimer();
            _stateMachine.WriteToRedLed(false);
            _stateMachine.ChangeState(_stateMachine.CheckBobber());
        }

        public void ButtonReleased()
        {
            
        }

        public void ButtonHold()
        {
          
        }

        public void BobberIsUp()
        {
       
        }

        public void BobberIsDown()
        {
            
        }
    }
}