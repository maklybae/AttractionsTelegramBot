﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TelegramBot.UpdateProcessors;

namespace TelegramBot
{
    internal class Program
    {
        private static void Main()
        {
            // Importing access token
            var dotenv = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", ".env");
            DotEnv.Load(dotenv);
            var botToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN") ?? "{UNKNOWN_ACCESS_TOKEN}";

            // Logging init
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.File(Path.Combine("..", "..", "..", "var", "bot.log"))
              .CreateLogger();

            var services = new ServiceCollection();
            services.AddLogging(loggerBuilder => loggerBuilder.AddConsole());
            services.AddLogging(loggerBuilder => loggerBuilder.AddSerilog(dispose: true));
            var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<Program>>();

            //Creating bot.
            var botManager = new BotManager(botToken);
            logger!.LogInformation("Telegram bot managere created");

            // Creating bot sercices for processing messages from user and manage user's states.
            var stateProcessor = new StateProcessor(botManager, logger!);
            var messagesProcessor = new MessagesProcessor(botManager, stateProcessor);
            var callbackQueryProcessor = new CallbackQueryProcessor(botManager, stateProcessor);
            logger!.LogInformation("Messages and CallbackQueeries processsors created");

            Console.ReadLine();
            
            serviceProvider.Dispose();
            logger!.LogInformation("Resources disposed");
        }
    }
}