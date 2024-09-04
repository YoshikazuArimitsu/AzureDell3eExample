using System.ClientModel;
using System.Drawing;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Images;

namespace AzureDell3eExample.Services
{
    public class Dell3eConfig
    {
        public GeneratedImageQuality quality { get; set; } = GeneratedImageQuality.Standard;
        public GeneratedImageSize size { get; set; } = GeneratedImageSize.W1024xH1024;

        public string endpoint { get; set; } = "";
        public string apikey { get; set; } = "";
    }
    public class GenerateImageDell3eService : IGenerateImageService
    {


        private ILogger<GenerateImageDell3eService> Logger { get; set; }
        private Dell3eConfig Config { get; set; }

        public GenerateImageDell3eService(ILogger<GenerateImageDell3eService> logger, IOptions<Dell3eConfig> config)
        {
            Logger = logger;
            Config = config.Value;
        }

        public async Task<Uri> GenerateAsync(string prompt)
        {
            AzureKeyCredential credential = new AzureKeyCredential(Config.apikey);
            AzureOpenAIClient azureClient = new AzureOpenAIClient(new Uri(Config.endpoint), credential);
            ImageClient client = azureClient.GetImageClient("dall-e-3");

            ClientResult<GeneratedImage> imageResult = await client.GenerateImageAsync(prompt, new()
            {
                Quality = Config.quality,
                Size = Config.size,
                ResponseFormat = GeneratedImageFormat.Uri
            });

            return imageResult.Value.ImageUri;
        }
    }
}

