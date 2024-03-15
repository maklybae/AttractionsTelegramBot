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
            var data = processor.Read(new FileStream("D:\\Downloads\\attraction-TC (1).csv", FileMode.Open));
            var fstream = new FileStream("test.csv", FileMode.Create);
            using (var stream = processor.Write(data))
            {
                stream.WriteTo(fstream);
            }
            fstream.Close();

            
        }
    }
}