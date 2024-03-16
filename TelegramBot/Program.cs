using Telegram.Bot.Polling;
using TelegramBot.UpdateProcessors;

namespace TelegramBot
{
    internal class Program
    {
        async static Task Main()
        {
           // Importing access token
           //var dotenv = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", ".env");
           // DotEnv.Load(dotenv);
           // var botToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN") ?? "{UNKNOWN_ACCESS_TOKEN}";
            var botToken = "7038009512:AAFtlfzLuU1Gf1HQoGwp1RehA5ZbfBFHVuA";


            // Creating bot

            var botManager = new BotManager(botToken);
            var messagesProcessor = new MessagesProcessor(botManager);
            var callbackQueryProcessor = new CallbackQueryProcessor(botManager);

            while (true) { }
        }
    }
}