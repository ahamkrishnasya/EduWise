using Microsoft.AspNetCore.Mvc;
using EduWise.Services;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;

namespace EduWise.Controllers
{
    public class ChatController : Controller
    {
        private readonly GeminiService _gemini;

        public ChatController(GeminiService gemini)
        {
            _gemini = gemini;
        }

        [HttpPost]
        public async Task<IActionResult> Send(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Json(new { reply = "Please type something!" });

            try
            {
                var reply = await _gemini.GetResponseAsync(message); // already parsed text
                return Json(new { reply });
            }
            catch (System.Exception ex)
            {
                return Json(new { reply = $"AI Error: {ex.Message}" });
            }
        }

    }
}
