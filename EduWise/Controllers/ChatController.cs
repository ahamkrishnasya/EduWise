using Microsoft.AspNetCore.Mvc;

namespace EduWise.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ask(string userMessage)
        {
            // Mock response (you can replace with AI later)
            string response = userMessage.Contains("Newton") ? "Newton's laws explain motion and force." : "I'll get back to you with more details!";
            ViewBag.Response = response;
            ViewBag.UserMessage = userMessage;
            return View("Index");
        }
    }
}
