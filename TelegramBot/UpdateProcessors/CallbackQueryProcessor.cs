using TelegramBot.EventArguments;

namespace TelegramBot.UpdateProcessors;

internal class CallbackQueryProcessor
{
    private readonly StateProcessor _stateProcessor;

    public CallbackQueryProcessor(BotManager botManager, StateProcessor stateProcessor)
    {
        botManager.CallbackQueryReceived += ProcessCallbackQuery;
        _stateProcessor = stateProcessor;
    }

    public async void ProcessCallbackQuery(object? sender, CallbackQueryReceivedEventArgs args)
    {
        var message = args.ReceivedCallback.Message;
        await Console.Out.WriteLineAsync($"Callback \"{message!.Text}\" in processing");
        await _stateProcessor.ProcessRequest(message!, args.ReceivedCallback);
    }
}
