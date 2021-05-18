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

        public void Start()
        {
            _stateMachine.WriteToMotor(true);
            _stateMachine.SetTimer(new EightSecondsPassed(), 7800);
        }

        public void Stop()
        {
            _stateMachine.WriteToMotor(false);
        }
    }
}