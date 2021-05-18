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

            _bobber.SetEvent(_messageBus.Enqueue, new BobberIsDown(), new BobberIsUpActive());
            _button.SetEvent(_messageBus.Enqueue, new ButtonReleased(), new ButtonPressed());
        }

        public void ProcessSingle(IMessage message)
        {
            switch (message)
            {
                case BobberIsDown:
                    if (_state is not BobberAlarm)
                    {
                        StopTimer();
                        TurnOffAll();
                        _state = new StandBy(this);
                        _state.Start();
                    }
                    break;
                case BobberIsUpActive:
                    if (_state is not BobberAlarm)
                    {
                        if (_state is not BobberActive)
                        {
                            StopTimer();
                            ResetCycle();
                            TurnOffAll();
                            _state = new BobberActive(this);
                        }

                        if (_cycle >= 3)
                        {
                            StopTimer();
                            _state = new BobberAlarm(this);
                        }

                        _state.Start();
                    }
                    break;
                case BobberIsUpPause:
                    _state.Stop();
                    IncCycle();
                    break;
                case ButtonHold:
                    StopTimer();
                    _hold = true;
                    break;
                case ButtonPressed:
                    StopTimer();
                    TurnOffAll();
                    if (_state is BobberAlarm || _state is MotorActiveEightSeconds)
                    {
                        Console.WriteLine(_state);
                        Console.WriteLine(_timer);
                        _state = CheckBobber();
                    }
                    else
                    {
                        _hold = false;
                        _state = new MotorActive(this);
                    }
                    _state.Start();
                    break;
                case ButtonReleased:
                    if (_state is MotorActive && _hold)
                    {
                        _state.Stop();
                        _state = CheckBobber();
                    }else if (_state is MotorActive && !_hold)
                    {
                        StopTimer();
                        _state.Stop();
                        _state = new MotorActiveEightSeconds(this);
                    }
                    _state.Start();
                    break;
                case EightSecondsPassed:
                    _state.Stop();
                    StopTimer();
                    _state = CheckBobber();
                    _state.Start();
                    break;
                case StartProcessing:
                    _state = new StandBy(this);
                    _state.Start();
                    break;
                case TurnOff:
                    _state.Stop();
                    break;
                case TurnOn:
                    _state.Start();
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

        public void TurnOffAll()
        {
            WriteToMotor(false);
            WriteToRedLed(false);
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

        private void StopTimer()
        {
            _timer.Dispose();
        }

        private void IncCycle()
        {
            _cycle += 1;
        }

        private void ResetCycle()
        {
            _cycle = 0;
        }

        private IState CheckBobber()
        {
            if (_bobber.GetValue() == 1)
            {
                ResetCycle();
                return new BobberActive(this);
            }
            return new StandBy(this);
        }
    }
}