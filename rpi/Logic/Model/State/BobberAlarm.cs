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
        public void Start()
        {
            _stateMachine.WriteToRedLed(true);
            _stateMachine.SetTimer(new TurnOff(), 500);
        }

        public void Stop()
        {
            _stateMachine.WriteToRedLed(false);
            _stateMachine.SetTimer(new TurnOn(), 1000);
        }
    }
}