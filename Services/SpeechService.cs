using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;

namespace SpeechApp.API.Services
{
    public class SpeechService
    {
        private readonly IConfiguration _configuration;

        public SpeechService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> RecognizeSpeechAsync(Stream audioStream, string language)
        {
            var speechKey = _configuration["Azure:SpeechKey"];
            var speechRegion = _configuration["Azure:SpeechRegion"];

            var config = SpeechConfig.FromSubscription(speechKey, speechRegion);
            config.SpeechRecognitionLanguage = language;

            var audioFormat = AudioStreamFormat.GetWaveFormatPCM(44100, 16, 1);
            using var pushStream = AudioInputStream.CreatePushStream(audioFormat);
            using var audioConfig = AudioConfig.FromStreamInput(pushStream);
            using var recognizer = new SpeechRecognizer(config, audioConfig);

            // 🔄 Пишем аудиопоток в Azure
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = await audioStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                pushStream.Write(buffer, bytesRead);
            }
            pushStream.Close();

            // 🎙️ Распознаём
            var result = await recognizer.RecognizeOnceAsync();

            // 🪵 Логируем всё, что пришло от Azure
            Console.WriteLine($"[Azure Result] Reason: {result.Reason}");
            Console.WriteLine($"[Azure Result] Text: {result.Text}");
            Console.WriteLine($"[Azure Result] Json: {result.Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult)}");

            if (result.Reason == ResultReason.RecognizedSpeech)
                return result.Text;

            if (result.Reason == ResultReason.NoMatch)
                return "❌ Azure не смог распознать речь (NoMatch). Проверь формат, язык и наличие голоса.";

            return $"❌ Ошибка: {result.Reason}";
        }
    }
}
// using Microsoft.CognitiveServices.Speech;
// using Microsoft.CognitiveServices.Speech.Audio;
// using Microsoft.Extensions.Configuration;

// namespace SpeechApp.API.Services
// {
//     public class SpeechService
//     {
//         private readonly IConfiguration _configuration;

//         public SpeechService(IConfiguration configuration)
//         {
//             _configuration = configuration;
//         }

//         public async Task<string> RecognizeSpeechAsync(Stream audioStream, string language)
//         {
//             var speechKey = _configuration["Azure:SpeechKey"];
//             var speechRegion = _configuration["Azure:SpeechRegion"];

//             var config = SpeechConfig.FromSubscription(speechKey, speechRegion);
//             config.SpeechRecognitionLanguage = language;

//             var audioFormat = AudioStreamFormat.GetWaveFormatPCM(44100, 16, 1);
//             using var pushStream = AudioInputStream.CreatePushStream(audioFormat);
//             using var audioConfig = AudioConfig.FromStreamInput(pushStream);
//             using var recognizer = new SpeechRecognizer(config, audioConfig);

//             byte[] buffer = new byte[4096];
//             int bytesRead;
//             while ((bytesRead = await audioStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
//             {
//                 pushStream.Write(buffer, bytesRead);
//             }
//             pushStream.Close();

//             var result = await recognizer.RecognizeOnceAsync();

//             return result.Reason == ResultReason.RecognizedSpeech
//                 ? result.Text
//                 : $"Ошибка: {result.Reason}";
//         }
//     }
// }
