using Microsoft.AspNetCore.Mvc;
using SpeechApp.API.Services;

namespace SpeechApp.API.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly UploadService _uploadService;
        private readonly SpeechService _speechService;

        public UploadController(UploadService uploadService, SpeechService speechService)
        {
            _uploadService = uploadService;
            _speechService = speechService;
        }

        [HttpPost("audio")]
        public async Task<IActionResult> UploadAudio([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не найден.");

            using var stream = file.OpenReadStream();
            var url = await _uploadService.UploadAsync(stream, file.FileName);

            return Ok(new { url });
        }

        [HttpPost("audio-with-recognition")]
        public async Task<IActionResult> UploadAndRecognizeAudio([FromForm] IFormFile file, [FromForm] string language)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не найден.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            // 🎙 Распознаём текст
            var recognizedText = await _speechService.RecognizeSpeechAsync(stream, language);

            // ☁ Загружаем файл в Azure Blob
            stream.Position = 0; // сброс позиции перед повторным чтением
            var url = await _uploadService.UploadAsync(stream, file.FileName);

            return Ok(new
            {
                text = recognizedText,
                url
            });
        }
    }
}