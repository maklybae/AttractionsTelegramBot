namespace TelegramBot
{
    internal class Program
    {
        static void Main()
        {
            var dotenv = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", ".env");
            DotEnv.Load(dotenv);

            Console.WriteLine(Environment.GetEnvironmentVariable("ACCESS_TOKEN"));

            
        }
    }
}