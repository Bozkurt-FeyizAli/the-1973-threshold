using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace backend.Services
{
    public class ImageGenerationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ImageGenerationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateImageBase64Async(string prompt)
        {
            var apiKey = _configuration["AI_APIs:HuggingFace:ApiKey"];
            var modelUrl = _configuration["AI_APIs:HuggingFace:ImageModelUrl"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(modelUrl))
            {
                throw new Exception("Hugging Face Image API config eksik.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, modelUrl);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Headers.Add("Accept", "image/jpeg");

            // Append 1970s polaroid style to prompt
            var styledPrompt = $"1970s vintage polaroid photo, moody lighting, analog photography, {prompt}";

            var payload = new { inputs = styledPrompt };
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Image Generation API Hatası: {response.StatusCode} - {errorMsg}");
                return string.Empty; // Return empty to not break the frontend
            }

            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            return Convert.ToBase64String(imageBytes);
        }
    }
}
