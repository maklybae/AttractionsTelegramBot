using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Models.DataFormatProcessors;
using System.Threading.Channels;
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
            // var botToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN") ?? "{UNKNOWN_ACCESS_TOKEN


            var botToken = "7038009512:AAFtlfzLuU1Gf1HQoGwp1RehA5ZbfBFHVuA";


            //Creating bot

            var botManager = new BotManager(botToken);
            var messagesProcessor = new MessagesProcessor(botManager);
            var callbackQueryProcessor = new CallbackQueryProcessor(botManager);

            Console.ReadLine();

            //FileStream fs = new FileStream("D:\\Downloads\\attraction-TC (1).csv", FileMode.Open);
            //var res = new CSVProcessing().Read(fs);
            //foreach (var item in new Selector(res, new[] { ("Location", "ТЦ «АВИАПАРК»"), ("Name", "ПОЖАРНАЯ МАШИНА") }).Select())
            //{
            //    await Console.Out.WriteLineAsync($"{item.Name} {item.Location}");
            //}


        }
    }
}