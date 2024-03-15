using Models;

namespace TelegramBot
{
    internal class Program
    {
        static void Main()
        {
            var dotenv = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", ".env");
            DotEnv.Load(dotenv);

            Console.WriteLine(Environment.GetEnvironmentVariable("ACCESS_TOKEN"));

            var processor = new CSVProcessing();
            processor.Read(new FileStream("D:\\Downloads\\attraction-TC (1).csv", FileMode.Open));
            
        }
    }
}