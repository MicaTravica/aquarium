using System;
using System.Device.Gpio;
using System.Threading;
using Logic.Model.Common;
using Logic.Model.Controller;
using Logic.Model.Pin;
using Timer = System.Timers.Timer;

namespace Logic.Model.Aquarium
{
    public class Aquarium : IAquarium
    {
        private Thread _thread;
        private State _state;
        private DateTime _timeStamp;
        private readonly IController _controller;

        public Aquarium(IController controller)
        {
            _controller = controller;
        }

        public void Start()
        {
            _state = State.StandBy;
            _controller.SetBobberEvent(BobberActivity);
            _controller.SetButtonEvent(ButtonActivity);
            _thread = new Thread(StandByState);
            _thread.Start();
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
            if (_controller.ReadBobberValue() == PinValue.High)
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

        private void StandByState()
        {
            DateTime lastActivatedTime = DateTime.Now.AddSeconds(-5);
            while (_state == State.StandBy)
            {
                DateTime timeNow = DateTime.Now;
                if (!((timeNow - lastActivatedTime).TotalSeconds >= 5)) continue;
                _controller.WriteToRedLed(true);
                Thread.Sleep(250);
                _controller.WriteToRedLed(false);
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
                _controller.WriteToMotor(true);
                Thread.Sleep(250);
                _controller.WriteToMotor(false);
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
                    _controller.WriteToRedLed(true);
                    lastActivatedTime = DateTime.Now;
                    turnOn = true;
                }
                else if (turnOn && (timeNow - lastActivatedTime).TotalSeconds >= 0.5)
                {
                    _controller.WriteToRedLed(false);
                    lastActivatedTime = DateTime.Now;
                    turnOn = false;
                }
            }
            _controller.WriteToRedLed(false);
        }

        private void MotorStates()
        {
            _controller.WriteToMotor(true);
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

            _controller.WriteToMotor(false);
            if (active8Seconds)
            {
                _state = State.StandBy;
                StandByState();
            }
        }
    }
}