using System.Threading;
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

        public void Start()
        {
            _stateMachine.WriteToMotor(true);
            _stateMachine.SetTimer(new BobberIsUpPause(), 250);
        }

        public void Stop()
        {
            _stateMachine.WriteToMotor(false);
            _stateMachine.SetTimer(new BobberIsUpActive(), 5000);
        }
    }
}