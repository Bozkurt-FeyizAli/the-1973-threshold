using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure CORS for Angular Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://feyizalibozkurt.dev", "https://www.feyizalibozkurt.dev") // Assuming Angular runs on 4200
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Register HttpClients and Services
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddHttpClient<ElevenLabsAudioService>();
builder.Services.AddSingleton<SentimentAnalysisService>();
builder.Services.AddHttpClient<ImageGenerationService>();

// Swagger config needs to be before builder.Build()
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAngularApp");


app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
