using Telegram.Bot.Types;

namespace TelegramBot.EventArguments;

/// <summary>
/// Represents event arguments for when a message is received.
/// </summary>
internal class MessageReceivedEventArgs : EventArgs 
{
    private readonly Message _message;

    /// <summary>
    /// Initializes a new instance of the MessageReceivedEventArgs class.
    /// </summary>
    private MessageReceivedEventArgs() => _message = new();

    /// <summary>
    /// Initializes a new instance of the MessageReceivedEventArgs class with the specified message.
    /// </summary>
    /// <param name="message">The received message.</param>
    public MessageReceivedEventArgs(Message message)
    {
        _message = message;
    }

    /// <summary>
    /// Gets the received message.
    /// </summary>
    public Message ReceivedMessage => _message;

}
