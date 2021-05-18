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

        public void Start()
        {
            _stateMachine.WriteToRedLed(true);
            _stateMachine.SetTimer(new TurnOff(), 250);
        }

        public void Stop()
        {
            _stateMachine.WriteToRedLed(false);
            _stateMachine.SetTimer(new TurnOn(), 5000);
        }
    }
}
