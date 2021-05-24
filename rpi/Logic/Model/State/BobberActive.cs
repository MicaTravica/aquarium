using Logic.Model.Message;

namespace Logic.Model.State
{
    public class BobberActive : IState
    {
        private readonly StateMachine.StateMachine _stateMachine;

        public BobberActive(StateMachine.StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void TurnOn()
        {
            if (_stateMachine.Cycle >= 3)
            {
                _stateMachine.StopTimer();
                _stateMachine.ChangeState(new BobberAlarm(_stateMachine));
            }
            else
            {
                _stateMachine.WriteToMotor(true);
                _stateMachine.SetTimer(new TurnOff(), 250);
            }
        }

        public void TurnOff()
        {
            _stateMachine.WriteToMotor(false);
            _stateMachine.SetTimer(new TurnOn(), 5000);
            _stateMachine.IncCycle();

        }

        public void ButtonPressed()
        {
            _stateMachine.StopTimer();
            _stateMachine.WriteToMotor(false);
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
        }

        public void BobberIsDown()
        {
            _stateMachine.StopTimer();
            _stateMachine.WriteToMotor(false);
            _stateMachine.ChangeState(new StandBy(_stateMachine));
        }
    }
}