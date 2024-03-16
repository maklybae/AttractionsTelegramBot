using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using TelegramBot.EventArguments;

namespace TelegramBot;

internal class BotManager
{
    private readonly TelegramBotClient _client;

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public event EventHandler<CallbackQueryReceivedEventArgs>? CallbackQueryReceived;

    public BotManager(string accessToken)
    {
        _client = new(accessToken);
        StartReceiving();
    }

    public TelegramBotClient Client => _client;



    private void StartReceiving()
    {
        using var cts = new CancellationTokenSource();

        _client.StartReceiving(updateHandler: HandleUpdateAsync,
                               pollingErrorHandler: HandlePollingErrorAsync,
                               receiverOptions: new ReceiverOptions
                               {
                                   AllowedUpdates = Array.Empty<UpdateType>()
                               },
                               cancellationToken: cts.Token);

    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => OnMessageReceive(update.Message!),
            UpdateType.CallbackQuery => OnCallbackQuery( update.CallbackQuery!),
            _ => UnknownUpdateHandlerAsync(update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandlePollingErrorAsync(botClient, exception, cancellationToken);
        }
    }

    private Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        Console.WriteLine("Callback query received");
        CallbackQueryReceived?.Invoke(this, new CallbackQueryReceivedEventArgs(callbackQuery));
        return Task.CompletedTask;        
    }

    private Task OnMessageReceive(Message message)
    {
        Console.WriteLine("Message received");
        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        return Task.CompletedTask;
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
