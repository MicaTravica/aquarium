using System.Device.Gpio;
using Logic.Model.Common;

namespace Logic.Model.Button
{
    public interface IButton : IPinWithValue
    {
        PinValue GetLastValue();
        void SetLastValue();
        void ResetLastValue();
    }
}