using System.Device.Gpio;
using Logic.Model.Common;

namespace Logic.Model.BobberFishing
{
    public interface IBobberFishing : IPinWithValue
    {
        int GetCount();
        void IncCount();
        void ResetCount();
    }
}