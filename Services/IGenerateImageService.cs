using System.Drawing;

namespace AzureDell3eExample.Services;
public interface IGenerateImageService {
    public Task<Uri> GenerateAsync(string paragraph);
}