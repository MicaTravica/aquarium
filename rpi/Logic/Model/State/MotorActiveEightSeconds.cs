using Logic.Model.Message;

namespace Logic.Model.State
{
    public class MotorActiveEightSeconds : IState
    {
        private readonly StateMachine.StateMachine _stateMachine;

        public MotorActiveEightSeconds(StateMachine.StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void TurnOn()
        {
            _stateMachine.WriteToMotor(true);
            _stateMachine.SetTimer(new TurnOff(), 7800);
        }

        public void TurnOff()
        {
            _stateMachine.WriteToMotor(false);
            _stateMachine.StopTimer();
            _stateMachine.ChangeState(_stateMachine.CheckBobber());
        }

        public void ButtonPressed()
        {
            _stateMachine.StopTimer();
            _stateMachine.WriteToMotor(false);
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