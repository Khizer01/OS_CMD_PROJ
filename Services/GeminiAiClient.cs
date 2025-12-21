using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OS_CMD_PROJECT.Services
{
    public class GeminiClient : IAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiClient()
        {
            _httpClient = new HttpClient();
            _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            if (string.IsNullOrEmpty(_apiKey))
                throw new Exception("GEMINI_API_KEY is not set");
        }

        public async Task<string> QueryAsync(string prompt)
        {
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);

            var response = await _httpClient.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                return $"[AI ERROR] HTTP {(int)response.StatusCode}";
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var parsed = JObject.Parse(responseJson);

            return (string)parsed["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]
                   ?? "[AI ERROR] Empty response";
        }
    }
}
