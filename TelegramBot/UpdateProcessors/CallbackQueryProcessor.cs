using TelegramBot.EventArguments;

namespace TelegramBot.UpdateProcessors;

/// <summary>
/// Represents a processor for handling callback queries.
/// </summary>
internal class CallbackQueryProcessor
{
    private readonly StateProcessor _stateProcessor;

    /// <summary>
    /// Initializes a new instance of the CallbackQueryProcessor class with the specified bot manager and state processor.
    /// </summary>
    /// <param name="botManager">The bot manager responsible for handling bot events.</param>
    /// <param name="stateProcessor">The state processor responsible for processing bot states.</param>
    public CallbackQueryProcessor(BotManager botManager, StateProcessor stateProcessor)
    {
        botManager.CallbackQueryReceived += ProcessCallbackQuery;
        _stateProcessor = stateProcessor;
    }

    /// <summary>
    /// Processes the callback query when received.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="args">The event arguments containing the received callback query.</param>
    public async void ProcessCallbackQuery(object? sender, CallbackQueryReceivedEventArgs args)
    {
        var message = args.ReceivedCallback.Message;
        await _stateProcessor.ProcessRequest(message!, args.ReceivedCallback);
    }
}
