using System;
using System.Device.Gpio;
using Logic.Model.Message;

namespace Logic.Model.Sensor
{
    public interface ISensor
    {
        void SetEvent(Action<IMessage> func, IMessage messageFalling, IMessage messageRising);
        PinValue GetValue();
    }
}