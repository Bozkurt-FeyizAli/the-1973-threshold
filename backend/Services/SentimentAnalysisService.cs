using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VaderSharp2;

namespace backend.Services
{
    public class SentimentAnalysisService
    {
        public SentimentAnalysisService()
        {
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
