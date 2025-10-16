
using Microsoft.AspNetCore.Mvc;
using SpeechApp.API.Models;
using SpeechApp.API.Services;

namespace SpeechApp.API.Controllers
{
    [ApiController]
    [Route("api/speech")]
    public class SpeechController : ControllerBase
    {
        private readonly SpeechService _speechService;
        private readonly TranslationService _translationService;

        public SpeechController(SpeechService speechService, TranslationService translationService)
        {
            _speechService = speechService;
            _translationService = translationService;
        }

        [HttpPost("recognize")]
        public async Task<IActionResult> Recognize([FromForm] IFormFile file, [FromForm] string language)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Audio file is required.");

            using var stream = file.OpenReadStream();
            var result = await _speechService.RecognizeSpeechAsync(stream, language);
            return Ok(new { text = result });
        }

        [HttpPost("translate")]
        public async Task<IActionResult> Translate([FromBody] TranslateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text) || string.IsNullOrWhiteSpace(request.To))
                return BadRequest("Text and target language are required.");

            var result = await _translationService.TranslateTextAsync(request.Text, request.To);
            return Ok(new { translated = result });
        }

        [HttpGet("download")]
        public IActionResult Download([FromQuery] string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return File(bytes, "text/plain", "result.txt");
        }
    }
}
