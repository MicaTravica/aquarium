using System;
using System.Device.Gpio;
using System.Threading;
using Logic.Model.Common;
using Logic.Model.Pin;
using Timer = System.Timers.Timer;

namespace Logic.Model.Controller
{
    public class Controller : IController
    {
        private Thread _thread;
        private State _state;
        private DateTime _timeStamp;
        private readonly GpioController _controller;
        private readonly IPin _motor;
        private readonly IPin _greenLed;
        private readonly IPin _redLed;
        private readonly IPin _bobberFishing;
        private readonly IPin _button;

        public Controller(IPin motor, IPin greenLed, IPin redLed, IPin bobberFishing, IPin button)
        {
            _state = State.StandBy;
            _motor = motor;
            _greenLed = greenLed;
            _redLed = redLed;
            _bobberFishing = bobberFishing;
            _button = button;
            
            _controller = new GpioController();
            _controller.OpenPin(_motor.GetPin(), PinMode.Output);
            _controller.OpenPin(_greenLed.GetPin(), PinMode.Output);
            _controller.OpenPin(_redLed.GetPin(), PinMode.Output);
            _controller.OpenPin(_bobberFishing.GetPin(), PinMode.Input);
            _controller.OpenPin(_button.GetPin(), PinMode.Input);
            
            _controller.RegisterCallbackForPinValueChangedEvent(bobberFishing.GetPin(),
                PinEventTypes.Falling | PinEventTypes.Rising,
                (_, args) => { SetTimer(args.ChangeType, BobberActivity, _bobberFishing.GetPin()); });
            _controller.RegisterCallbackForPinValueChangedEvent(button.GetPin(),
                PinEventTypes.Falling | PinEventTypes.Rising,
                (_, args) => { SetTimer(args.ChangeType, ButtonActivity, _button.GetPin()); });
        }

        public void Start()
        {
            _thread = new Thread(StandByState);
            _thread.Start();
        }

        private void SetTimer(PinEventTypes type, Action<PinEventTypes> func, int pin)
        {
            PinValue typeValue = type == PinEventTypes.Rising ? 1 : 0;
            Timer aTimer = new Timer(100);
            int sum = 0;
            aTimer.Elapsed += (_, _) =>
            {
                PinValue pinValue = _controller.Read(pin);
                if (sum < 3 && typeValue != pinValue)
                {
                    sum += 1;
                    aTimer.Start();
                }

                if (typeValue == pinValue)
                {
                    func(type);
                }

            };
            aTimer.Enabled = true;
            aTimer.AutoReset = false;
            aTimer.Start();
        }
        private void ChangeThread(ThreadStart job)
        {
            _thread.Join();
            _thread = new Thread(job);
            _thread.Start();
        }
        private void BobberActivity(PinEventTypes type)
        {
            if (type == PinEventTypes.Rising && _state != State.BobberActive && _state != State.BobberAlarm)
            {
                _state = State.BobberActive;
                ChangeThread(BobberActiveState);
            }
            else if (type == PinEventTypes.Falling && _state != State.StandBy && _state != State.BobberAlarm)
            {
                _state = State.StandBy;
                ChangeThread(StandByState);
            }
        }

        private void ButtonActivity(PinEventTypes type)
        {
            if (_state == State.BobberAlarm)
            {
                CheckBobber();
            }
            else if (type == PinEventTypes.Rising && (_state == State.StandBy || _state == State.BobberActive ||
                                                      _state == State.MotorActive8Seconds))
            {
                if (_state == State.StandBy || _state == State.BobberActive)
                {
                    _timeStamp = DateTime.Now;
                    _state = State.MotorActive;
                    ChangeThread(MotorStates);
                }
                else if (_state == State.MotorActive8Seconds)
                {
                    CheckBobber();
                }
            }
            else if (type == PinEventTypes.Falling && _state == State.MotorActive)
            {
                if ((DateTime.Now - _timeStamp).TotalMilliseconds <= 400)
                {
                    _state = State.MotorActive8Seconds;
                }
                else
                {
                    CheckBobber();
                }
            }
        }

        private void CheckBobber()
        {
            if (_controller.Read(_bobberFishing.GetPin()) == PinValue.High)
            {
                _state = State.BobberActive;
                ChangeThread(BobberActiveState);
            }
            else
            {
                _state = State.StandBy;
                ChangeThread(StandByState);
            }
        }

        private void WriteToMotor(bool turnOn)
        {
            _controller.Write(_motor.GetPin(), turnOn ? PinValue.High : PinValue.Low);
            _controller.Write(_greenLed.GetPin(), turnOn ? PinValue.High : PinValue.Low);
        }

        private void WriteToRedLed(bool turnOn)
        {
            _controller.Write(_redLed.GetPin(), turnOn ? PinValue.High : PinValue.Low);
        }

        private void StandByState()
        {
            DateTime lastActivatedTime = DateTime.Now.AddSeconds(-5);
            while (_state == State.StandBy)
            {
                DateTime timeNow = DateTime.Now;
                if (!((timeNow - lastActivatedTime).TotalSeconds >= 5)) continue;
                WriteToRedLed(true);
                Thread.Sleep(250);
                WriteToRedLed(false);
                lastActivatedTime = DateTime.Now;
            }
        }

        private void BobberActiveState()
        {
            int count = 0;
            DateTime lastActivatedTime = DateTime.Now.AddSeconds(-5);
            while (_state == State.BobberActive)
            {
                DateTime timeNow = DateTime.Now;
                if (!((timeNow - lastActivatedTime).TotalSeconds >= 5)) continue;
                count += 1;
                if (count > 3) break;
                WriteToMotor(true);
                Thread.Sleep(250);
                WriteToMotor(false);
                lastActivatedTime = DateTime.Now;

            }

            if (count > 3 && _state == State.BobberActive)
            {
                BobberAlarmState();
            }
        }

        private void BobberAlarmState()
        {
            _state = State.BobberAlarm;
            DateTime lastActivatedTime = DateTime.Now.AddSeconds(-1);
            bool turnOn = false;
            while (_state == State.BobberAlarm)
            {
                DateTime timeNow = DateTime.Now;
                if (!turnOn && (timeNow - lastActivatedTime).TotalSeconds >= 1)
                {
                    WriteToRedLed(true);
                    lastActivatedTime = DateTime.Now;
                    turnOn = true;
                }
                else if (turnOn && (timeNow - lastActivatedTime).TotalSeconds >= 0.5)
                {
                    WriteToRedLed(false);
                    lastActivatedTime = DateTime.Now;
                    turnOn = false;
                }
            }
            WriteToRedLed(false);
        }

        private void MotorStates()
        {
            WriteToMotor(true);
            DateTime lastActivatedTime = DateTime.Now;
            bool active8Seconds = false;
            while (_state == State.MotorActive || _state == State.MotorActive8Seconds)
            {
                if (( DateTime.Now - lastActivatedTime).TotalSeconds >= 8 && _state == State.MotorActive8Seconds)
                {
                    active8Seconds = true;
                    break;
                }
            }

            WriteToMotor(false);
            if (active8Seconds)
            {
                _state = State.StandBy;
                StandByState();
            }
        }
    }
}