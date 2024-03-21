using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TelegramBot.UpdateProcessors;

namespace TelegramBot
{
    /// <summary>
    /// Main entry point for the Telegram bot application.
    /// </summary>
    internal class Program
    {
        private const string BotToken = "7164117434:AAGy3cWQ4FgIuc27ue1koMa5H7nLJRKNCIw";

        /// <summary>
        /// Main method that initializes and runs the Telegram bot application.
        /// </summary>
        private static void Main()
        {
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
            var botManager = new BotManager(BotToken);
            logger!.LogInformation("Telegram bot managere created");

            // Creating bot sercices for processing messages from user and manage user's states.
            var stateProcessor = new StateProcessor(botManager, logger!);
            var messagesProcessor = new MessagesProcessor(botManager, stateProcessor);
            var callbackQueryProcessor = new CallbackQueryProcessor(botManager, stateProcessor);
            logger!.LogInformation("Messages and CallbackQueeries processsors created");

            // Keep app's running
            try
            {
                while (true) { }
            }
            finally 
            {
                serviceProvider.Dispose();
                logger!.LogInformation("Resources disposed");
            }            
        }
    }
}