using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace EduWise.Services
{
    public class GeminiService
    {
        private readonly IHttpClientFactory _factory;
        private readonly string _apiKey;

        public GeminiService(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
            _apiKey = config["GeminiApiKey"];
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            var client = _factory.CreateClient();
            var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

            var body = new
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

            var json = JsonSerializer.Serialize(body);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-goog-api-key", _apiKey);

            var resp = await client.PostAsync(url, content);
            var respText = await resp.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(respText);
                var candidates = doc.RootElement.GetProperty("candidates");

                // Get all text parts from candidates
                var generatedText = string.Join("\n",
                    candidates.EnumerateArray()
                              .SelectMany(c => c.GetProperty("content")
                                                .GetProperty("parts")
                                                .EnumerateArray()
                                                .Select(p => p.GetProperty("text").GetString()))
                );

                return generatedText.Trim();
            }
            catch
            {
                // Return raw response if parsing fails
                return "AI Error: " + respText;
            }
        }
    }
}
