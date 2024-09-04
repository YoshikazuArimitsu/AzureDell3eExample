using System.Net;
using System.Security.Cryptography;
using AzureDell3eExample.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommandLine;

namespace AzureDell3eExample
{
    public class Options
    {
        [Option('p', "Prompt", Required = true, HelpText = "プロンプト", Default = "柴犬")]
        public string Prompt { get; set; } = "柴犬";

        [Option('o', "Output", Required = false, HelpText = "出力ファイル名", Default = "image.png")]
        public string Output { get; set; } = "image.png";
    }


    class Program
    {
        public static IConfiguration? Configuration { get; set; }

        static void Main(string[] args)
        {
            var env = System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", true)
                .AddEnvironmentVariables()
                .Build();

            var serviceProvider = new ServiceCollection();
            serviceProvider.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });

            serviceProvider.Configure<Dell3eConfig>(Configuration.GetSection("Dell3e"));
            serviceProvider.AddSingleton<IGenerateImageService, GenerateImageDell3eService>();

            var provider = serviceProvider.BuildServiceProvider();

            Options options = new Options();
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => options = o);

            var service = provider.GetService<IGenerateImageService>();
            var uri = service?.GenerateAsync(options.Prompt).Result;

            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync(uri).Result;

                using (var rs = response.Content.ReadAsStream())
                {
                    using (var ws = File.Create(options.Output))
                    {
                        rs.CopyTo(ws);
                    }
                }
            }
        }
    }
}