namespace backend.Models;

public class TransitionResponseData
{
    public string DetectedSentiment { get; set; } = string.Empty;
    public string FarewellText { get; set; } = string.Empty;
    public string AudioBase64 { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;
    public long GenerationTimeMs { get; set; }
}

public class TransitionResponse
{
    public string Status { get; set; } = string.Empty;
    public TransitionResponseData? Data { get; set; }
    public string? Message { get; set; }
}
