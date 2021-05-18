namespace Logic.Model.Message
{
    public sealed class StartProcessing : IMessage
    {
    }

    public sealed class TurnOn: IMessage
    {
    }

    public sealed class TurnOff: IMessage
    {
    }

    public sealed class ButtonPressed : IMessage
    {
    }

    public sealed class ButtonReleased : IMessage
    {
    }
    
    public sealed class ButtonHold : IMessage
    {
    }

    public sealed class BobberIsUpActive : IMessage
    {
    }

    public sealed class BobberIsUpPause : IMessage
    {
    }

    public sealed class BobberIsDown : IMessage
    {
    }

    public sealed class EightSecondsPassed : IMessage
    {
    }
}