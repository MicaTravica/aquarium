namespace ConsoleApp
{
    public interface IMessageBus
    {
        void Enqueue(IMessage message);
    }
}