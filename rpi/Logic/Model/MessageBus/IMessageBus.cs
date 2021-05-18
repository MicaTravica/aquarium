using Logic.Model.Message;

namespace Logic.Model.MessageBus
{
    public interface IMessageBus
    {
        void Enqueue(IMessage message);
    }
}