using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace EventConsumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var eventConsumer = new Program();
            var currentDirectory = eventConsumer.GetProjectDirectory();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            AppConfiguration.Load(configuration);
            var signalRUrl = AppConfiguration.signalRHubUrl;
            var signalRClient = new SignalRClient<string>(signalRUrl);
            await signalRClient.StartAsync();
            var receivedMessage = await signalRClient.GetReceivedMessageAsync();

            eventConsumer.PrintMessage(receivedMessage);
        }
        public void PrintMessage(string message)
        {
            Console.WriteLine($"Received message: { message}");
        }
        public string GetProjectDirectory()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            while (!File.Exists(Path.Combine(currentDirectory, "appsettings.json")))
            {
                DirectoryInfo parentDirectory = Directory.GetParent(currentDirectory);
                if (parentDirectory is null)
                {
                    throw new Exception("appsettings.json not found in the directory tree.");
                }
                currentDirectory = parentDirectory.FullName;
            }
            return currentDirectory;
        }
        public void LoggerConfiguration()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            Log.Logger = new LoggerConfiguration()
                        .WriteTo.File(Path.Combine(currentDirectory, "Information.log"),
                         restrictedToMinimumLevel: LogEventLevel.Information,
                         rollingInterval: RollingInterval.Day,
                         rollOnFileSizeLimit: true)
                        .WriteTo.File(Path.Combine(currentDirectory, "Error.log"),
                         restrictedToMinimumLevel: LogEventLevel.Error,
                         rollingInterval: RollingInterval.Day,
                         rollOnFileSizeLimit: true)
                         .CreateLogger();
        }
    }
}