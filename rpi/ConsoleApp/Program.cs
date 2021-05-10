using System;
using System.Device.Gpio;
using System.Threading;
using Logic.Constants;
using Logic.Model.BobberFishing;
using Logic.Model.Button;
using Logic.Model.Common;
using Logic.Model.Controller;
using Logic.Model.GreenLed;
using Logic.Model.Motor;
using Logic.Model.RedLed;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IMotor motor = new Motor(ConstantsRPI.Motor);
            IGreenLed greenLed = new GreenLed(ConstantsRPI.LedGreen);
            IRedLed redLed = new RedLed(ConstantsRPI.LedRed);
            IBobberFishing bobberFishing = new BobberFishing(ConstantsRPI.BobberFishing);
            IButton button = new Button(ConstantsRPI.Button);
            
            IController controller = new Controller(motor, greenLed, redLed, bobberFishing, button);

            State state = State.StandBy;
            DateTime timeNow;
            DateTime lastActivatedTime = DateTime.Now.AddSeconds(-5);

            while (true)
            {

                if (bobberFishing.GetValue() == PinValue.High && bobberFishing.GetCount() < 3)
                {
                    state = State.BobberActive;
                }
                else if (button.GetValue() == PinValue.High && state == State.BobberAlarm)
                {
                    state = State.StandBy;
                    bobberFishing.ResetCount();
                    lastActivatedTime = DateTime.Now.AddSeconds(-5);
                    controller.WriteToRedLed(false);
                    Thread.Sleep(200);
                }
                else if ((button.GetValue() == PinValue.High && button.GetLastValue() == PinValue.High) ||
                         (button.GetValue() == PinValue.Low && button.GetLastValue() == PinValue.High &&
                          state == State.MotorHold))
                {
                    state = State.MotorHold;
                    button.SetLastValue();
                }
                else if (button.GetValue() == PinValue.High || motor.GetValue() == PinValue.High)
                {
                    state = State.MotorActive;
                    button.SetLastValue();
                }
                else if (bobberFishing.GetValue() == PinValue.Low && bobberFishing.GetCount() < 3)
                {
                    state = State.StandBy;
                    bobberFishing.ResetCount();
                }
                else if (bobberFishing.GetCount() >= 3 && button.GetValue() == PinValue.Low)
                {
                    state = State.BobberAlarm;
                }

                if (state == State.StandBy)
                {
                    timeNow = DateTime.Now;
                    if ((timeNow - lastActivatedTime).TotalSeconds >= 5)
                    {
                        controller.WriteToRedLed(true);
                        Thread.Sleep(250);
                        controller.WriteToRedLed(false);
                        lastActivatedTime = DateTime.Now;
                    }

                }
                else if (state == State.BobberActive)
                {
                    bobberFishing.IncCount();
                    controller.WriteToMotor(true);
                    Thread.Sleep(250);
                    controller.WriteToMotor(false);
                    Thread.Sleep(5000);
                }
                else if (state == State.BobberAlarm)
                {
                    timeNow = DateTime.Now;
                    if (redLed.GetValue() == PinValue.Low && (timeNow - lastActivatedTime).TotalSeconds >= 1)
                    {
                        controller.WriteToRedLed(true);
                        lastActivatedTime = DateTime.Now;
                    }
                    else if (redLed.GetValue() == PinValue.High && (timeNow - lastActivatedTime).TotalSeconds >= 0.5)
                    {
                        controller.WriteToRedLed(false);
                        lastActivatedTime = DateTime.Now;
                    }
                }
                else if (state == State.MotorActive)
                {
                    timeNow = DateTime.Now;
                    if (motor.GetValue() == PinValue.Low)
                    {
                        controller.WriteToMotor(true);
                        lastActivatedTime = DateTime.Now;
                        Thread.Sleep(200);
                    }
                    else if (motor.GetValue() == PinValue.High &&
                             ((timeNow - lastActivatedTime).TotalSeconds >= 8 || button.GetValue() == PinValue.High))
                    {
                        controller.WriteToMotor(false);
                        lastActivatedTime = DateTime.Now.AddSeconds(-5);
                        button.ResetLastValue();
                        Thread.Sleep(200);
                    }
                }
                else
                {
                    if (button.GetLastValue() == PinValue.Low)
                    {
                        controller.WriteToMotor(false);
                        lastActivatedTime = DateTime.Now.AddSeconds(-5);
                    }
                }
            }
        }
    }
}