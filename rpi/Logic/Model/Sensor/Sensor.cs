using System;
using System.Device.Gpio;
using System.Timers;
using Logic.Model.Message;

namespace Logic.Model.Sensor
{
    public class Sensor : ISensor
    {
        private readonly GpioController _controller;
        private readonly int _pin;

        public Sensor(GpioController controller, int pin)
        {
            _controller = controller;
            _pin = pin;
            _controller.OpenPin(_pin, PinMode.Input);
        }

        public void SetEvent(Action<IMessage> func, IMessage messageFalling, IMessage messageRising)
        {
            _controller.RegisterCallbackForPinValueChangedEvent(_pin, PinEventTypes.Falling | PinEventTypes.Rising,
                (_, args) => { SetTimer(args.ChangeType, func, messageFalling, messageRising); });
        }

        private void SetTimer(PinEventTypes type, Action<IMessage> func, IMessage messageFalling, IMessage messageRising)
        {
            PinValue typeValue = type == PinEventTypes.Rising ? 1 : 0;
            Timer aTimer = new Timer(100);
            int sum = 0;
            aTimer.Elapsed += (_, _) =>
            {
                PinValue pinValue = GetValue();
                if (sum < 3 && typeValue != pinValue)
                {
                    sum += 1;
                    aTimer.Start();
                }

                if (typeValue == pinValue)
                {
                    if (type == PinEventTypes.Falling)
                    {
                        func(messageFalling);
                    }
                    else if (type == PinEventTypes.Rising)
                    {
                        func(messageRising);
                    }
                }
            };
            aTimer.Enabled = true;
            aTimer.AutoReset = false;
            aTimer.Start();
        }

        public PinValue GetValue()
        {
            return _controller.Read(_pin);
        }
    }
}