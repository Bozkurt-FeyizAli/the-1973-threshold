using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VaderSharp2;

namespace backend.Services
{
    public class SentimentAnalysisService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        // Dependency injection requires this constructor due to builder.Services.AddHttpClient<SentimentAnalysisService>()
        public SentimentAnalysisService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public Task<string> AnalyzeSentimentAsync(string userBurden)
        {
            var analyzer = new SentimentIntensityAnalyzer();
            var results = analyzer.PolarityScores(userBurden);
            
            // Compound değeri -1 (Çok Negatif) ile +1 (Çok Pozitif) arasındadır.
            if (results.Compound <= -0.05) return Task.FromResult("NEGATIVE");
            if (results.Compound >= 0.05) return Task.FromResult("POSITIVE");
            return Task.FromResult("NEUTRAL");
        }
    }
}
