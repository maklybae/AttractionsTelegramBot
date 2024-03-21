using Telegram.Bot.Types;

namespace TelegramBot.EventArguments;

/// <summary>
/// Represents event arguments for when a callback query is received.
/// </summary>
internal class CallbackQueryReceivedEventArgs
{
    private readonly CallbackQuery _callback;

    /// <summary>
    /// Initializes a new instance of the CallbackQueryReceivedEventArgs class.
    /// </summary>
    private CallbackQueryReceivedEventArgs() => _callback = new();

    /// <summary>
    /// Initializes a new instance of the CallbackQueryReceivedEventArgs class with the specified callback query.
    /// </summary>
    /// <param name="callback">The received callback query.</param>
    public CallbackQueryReceivedEventArgs(CallbackQuery callback)
    {
        _callback = callback;
    }

    /// <summary>
    /// Gets the received callback query.
    /// </summary>
    public CallbackQuery ReceivedCallback => _callback;
}
