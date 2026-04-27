using System.Text;
using System.Text.Json;

namespace backend.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GenerateFarewellTextAsync(string userBurden)
    {
        var apiKey = _configuration["AI_APIs:Gemini:ApiKey"];
        var modelId = _configuration["AI_APIs:Gemini:ModelId"];
        
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelId}:generateContent?key={apiKey}";

        var prompt = $"Kullanıcının yükü: '{userBurden}'. Bob Dylan'ın Knockin' on Heaven's Door şarkısındaki 1973 yılı felsefesiyle, barışçıl ve karşı kültür bir tonda bu yüke veda etmesini sağlayan kısa (en fazla 2-3 cümle), şiirsel bir mektup/yanıt yaz. 70'ler FM radyo sunucusu gibi, rahat ve umut verici bir sesleniş olsun.";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            
            var generatedText = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return generatedText ?? "Radyo frekansı zayıfladı dostum... Tekrar dener misin?";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gemini API Error: {ex.Message}");
            throw new Exception("Metin üretilirken bir hata oluştu.", ex);
        }
    }
}
