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
        
        public void Start()
        {
            _stateMachine.WriteToMotor(true);
            _stateMachine.SetTimer(new ButtonHold(), 400);
        }

        public void Stop()
        {
            _stateMachine.WriteToMotor(false);
        }
    }
}