using Telegram.Bot.Types;

namespace TelegramBot.EventArguments;

internal class MessageReceivedEventArgs : EventArgs 
{
    private readonly Message _message;

    private MessageReceivedEventArgs() => _message = new();

    public MessageReceivedEventArgs(Message message)
    {
        _message = message;
    }

    public Message ReceivedMessage => _message;

}
