using DataManager;
using DataManager.Mapping;
using DataManager.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.EventArguments;

namespace TelegramBot.UpdateProcessors;

internal class CallbackQueryProcessor
{
    private readonly StateProcessor _stateProcessor;
    public CallbackQueryProcessor(BotManager botManager)
    {
        botManager.CallbackQueryReceived += ProcessCallbackQuery;
        _stateProcessor = new(botManager);
    }

    public async void ProcessCallbackQuery(object? sender, CallbackQueryReceivedEventArgs args)
    {
        var message = args.ReceivedCallback.Message;
        await Console.Out.WriteLineAsync($"Callback \"{message!.Text}\" in processing");
        await _stateProcessor.ProcessRequest(message!, args.ReceivedCallback);
    }
}
