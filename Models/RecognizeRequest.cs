
namespace SpeechApp.API.Models
{
    public class RecognizeRequest
    {
        public IFormFile File { get; set; }
        public string Language { get; set; }
    }
}
