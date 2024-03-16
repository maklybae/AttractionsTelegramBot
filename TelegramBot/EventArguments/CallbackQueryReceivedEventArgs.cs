using Telegram.Bot.Types;

namespace TelegramBot.EventArguments;

internal class CallbackQueryReceivedEventArgs
{
    private readonly CallbackQuery _callback;

    private CallbackQueryReceivedEventArgs() => _callback = new();

    public CallbackQueryReceivedEventArgs(CallbackQuery callback)
    {
        _callback = callback;
    }

    public CallbackQuery ReceivedCallback => _callback;
}
