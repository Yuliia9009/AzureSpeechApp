
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace SpeechApp.API.Services
{
    public class TranslationService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public TranslationService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<string> TranslateTextAsync(string text, string toLanguage)
        {
            var key = _configuration["Azure:TranslatorKey"];
            var endpoint = _configuration["Azure:TranslatorEndpoint"];
            var region = _configuration["Azure:TranslatorRegion"];
            var route = $"/translate?api-version=3.0&to={toLanguage}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", region);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var body = new object[] { new { Text = text } };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint + route, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JArray.Parse(responseBody);
            return result[0]["translations"][0]["text"]?.ToString();
        }
    }
}
