using Logic.Model.Message;

namespace Logic.Model.State
{
    public class MotorActive : IState
    {
        private readonly StateMachine.StateMachine _stateMachine;

        public MotorActive(StateMachine.StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void TurnOn()
        {
            _stateMachine.WriteToMotor(true);
            _stateMachine.SetTimer(new ButtonHold(), 400);
        }

        public void TurnOff()
        {

        }

        public void ButtonPressed()
        {

        }

        public void ButtonReleased()
        {
            if (_stateMachine.Hold)
            {
                _stateMachine.WriteToMotor(false);
                _stateMachine.ChangeState(_stateMachine.CheckBobber());
            }
            else
            {
                _stateMachine.StopTimer();
                _stateMachine.ChangeState(new MotorActiveEightSeconds(_stateMachine));
            }
        }

        public void ButtonHold()
        {
            _stateMachine.StopTimer();
            _stateMachine.Holding();
        }

        public void BobberIsUp()
        {
        }

        public void BobberIsDown()
        {
        }

    }
}