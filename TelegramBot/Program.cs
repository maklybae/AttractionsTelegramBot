using Microsoft.Extensions.DependencyInjection;
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
            var botToken = "7038009512:AAFtlfzLuU1Gf1HQoGwp1RehA5ZbfBFHVuA";

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