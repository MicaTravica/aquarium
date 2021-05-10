using System.Device.Gpio;

namespace Logic.Model.Common
{
    public interface IPinWithValue: IPinBase
    {
        PinValue GetValue();
        void SetValue(PinValue pinValue);
    }
}