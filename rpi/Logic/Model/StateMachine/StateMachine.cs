using System;
using System.Device.Gpio;
using System.Threading;
using Logic.Constants;
using Logic.Model.Message;
using Logic.Model.MessageBus;
using Logic.Model.OutputPin;
using Logic.Model.Sensor;
using Logic.Model.State;

namespace Logic.Model.StateMachine
{
    public sealed class StateMachine
    {
        private IState _state;
        private readonly IMessageBus _messageBus;
        private readonly ISensor _bobber;
        private readonly ISensor _button;
        private readonly IOutputPin _ledRed;
        private readonly IOutputPin _ledGreen;
        private readonly IOutputPin _motor;
        private Timer _timer;
        private int _cycle;
        private bool _hold;

        public int Cycle => _cycle;

        public bool Hold => _hold;

        public StateMachine(IMessageBus messageBus)
        {
            _messageBus = messageBus;
            _messageBus.Enqueue(new StartProcessing());

            GpioController controller = new GpioController();
            _bobber = new Sensor.Sensor(controller, ConstantsRPI.BobberFishing);
            _button = new Sensor.Sensor(controller, ConstantsRPI.Button);
            _ledRed = new OutputPin.OutputPin(controller, ConstantsRPI.LedRed);
            _ledGreen = new OutputPin.OutputPin(controller, ConstantsRPI.LedGreen);
            _motor = new OutputPin.OutputPin(controller, ConstantsRPI.Motor);

            _bobber.SetEvent(_messageBus.Enqueue, new BobberIsDown(), new BobberIsUp());
            _button.SetEvent(_messageBus.Enqueue, new ButtonReleased(), new ButtonPressed());
        }

        public void ChangeState(IState state)
        {
            _state = state;
            _state.TurnOn();
        }
        public void ProcessSingle(IMessage message)
        {
            switch (message)
            {
                case StartProcessing:
                    _state = new StandBy(this);
                    _state.TurnOn();
                    break;
                case TurnOn:
                    _state.TurnOn();
                    break;
                case TurnOff:
                    _state.TurnOff();
                    break;
                case ButtonPressed:
                    _state.ButtonPressed();
                    break;
                case ButtonReleased:
                    _state.ButtonReleased();
                    break;
                case ButtonHold:
                    _state.ButtonHold();
                    break;
                case BobberIsUp:
                    _state.BobberIsUp();
                    break;
                case BobberIsDown:
                    _state.BobberIsDown();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message));
            }
        }

        public void WriteToMotor(bool turnOn)
        {
            _motor.WriteToPin(turnOn);
            _ledGreen.WriteToPin(turnOn);
        }

        public void WriteToRedLed(bool turnOn)
        {
            _ledRed.WriteToPin(turnOn);
        }

        public void SetTimer(IMessage message, int milliseconds)
        {
            _timer = new Timer(CallbackToTimer, message, TimeSpan.FromMilliseconds(milliseconds),
                TimeSpan.FromDays(10));
        }

        private void CallbackToTimer(object message)
        {
            _messageBus.Enqueue((IMessage) message);
        }

        public void StopTimer()
        {
            _timer.Dispose();
        }

        public void IncCycle()
        {
            _cycle += 1;
        }

        public void ResetCycle()
        {
            _cycle = 0;
        }

        public IState CheckBobber()
        {
            if (_bobber.GetValue() != 1) return new StandBy(this);
            ResetCycle();
            return new BobberActive(this);
        }

        public void Holding()
        {
            _hold = true;
        }

        public void NotHolding()
        {
            _hold = false;
        }
    }
}