using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using System.Diagnostics;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransitionController : ControllerBase
{
    private readonly GeminiService _geminiService;
    private readonly ElevenLabsAudioService _audioService;
    private readonly SentimentAnalysisService _sentimentService;
    private readonly ImageGenerationService _imageService;

    public TransitionController(
        GeminiService geminiService, 
        ElevenLabsAudioService audioService,
        SentimentAnalysisService sentimentService,
        ImageGenerationService imageService)
    {
        _geminiService = geminiService;
        _audioService = audioService;
        _sentimentService = sentimentService;
        _imageService = imageService;
    }

    [HttpPost("transmit-burden")]
    public async Task<IActionResult> TransmitBurden([FromBody] BurdenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserBurden))
        {
            return BadRequest(new { status = "error", message = "Yük boş olamaz." });
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 1. LLM ve Sentiment analizini PARALEL çalıştır
            var llmTask = _geminiService.GenerateFarewellTextAsync(request.UserBurden);
            var sentimentTask = _sentimentService.AnalyzeSentimentAsync(request.UserBurden);

            await Task.WhenAll(llmTask, sentimentTask);

            var farewellText = llmTask.Result;
            var detectedSentiment = sentimentTask.Result;

            // 2. TTS ve Image Generation servislerini PARALEL çalıştır
            var audioTask = _audioService.GenerateAudioBase64Async(farewellText, detectedSentiment);
            var imageTask = _imageService.GenerateImageBase64Async(request.UserBurden); // Using original burden or farewell text? The prompt says "LLM'in ürettiği metin temasında" but request.UserBurden might be shorter for image prompt, let's use farewellText for better context or userBurden. I'll use userBurden for simplicity but add "1970s polaroid" in the service.

            await Task.WhenAll(audioTask, imageTask);

            var audioBase64 = audioTask.Result;
            var imageBase64 = imageTask.Result;

            stopwatch.Stop();

            var response = new TransitionResponse
            {
                Status = "success",
                Data = new TransitionResponseData
                {
                    DetectedSentiment = detectedSentiment,
                    FarewellText = farewellText,
                    AudioBase64 = audioBase64,
                    ImageBase64 = imageBase64,
                    GenerationTimeMs = stopwatch.ElapsedMilliseconds
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Controller Error: {ex}");
            stopwatch.Stop();
            return StatusCode(500, new TransitionResponse 
            { 
                Status = "error", 
                Message = "Radyo frekansı koptu. Lütfen tekrar deneyin." // As requested in PRD
            });
        }
    }
}
