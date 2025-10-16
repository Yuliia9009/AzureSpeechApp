using SpeechApp.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Azure services
builder.Services.AddSingleton<SpeechService>();
builder.Services.AddSingleton<TranslationService>();
builder.Services.AddSingleton<BlobStorageService>();
builder.Services.AddSingleton<UploadService>();

// Enable configuration access
builder.Services.AddSingleton(builder.Configuration);

// CORS (для фронтенда)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // для wwwroot
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();