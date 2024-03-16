using Models;
using Models.DataFormatProcessors;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace TelegramBot
{
    internal class Program
    {
        async static Task Main()
        {
            // Importing access token
            var dotenv = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", ".env");
            DotEnv.Load(dotenv);
            var botToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN") ?? "{UNKNOWN_ACCESS_TOKEN}";

            // Preparing for creating bot
            using CancellationTokenSource cts = new();
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };

            // Creating bot
            var bot = new TelegramBotClient(Environment.GetEnvironmentVariable("ACCESS_TOKEN") ?? "{UNKNOWN_ACCESS_TOKEN}");
            var handlerManager = new BotHandlerManager();

            bot = new TelegramBotClient(botToken);
            bot.StartReceiving(handlerManager.HandleUpdateAsync,
                               handlerManager.HandleErrorAsync,
                               receiverOptions,
                               cts.Token);

            var botUser = await bot.GetMeAsync();

            Console.ReadLine();
        }
    }
}