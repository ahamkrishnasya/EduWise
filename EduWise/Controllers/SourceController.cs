using Microsoft.AspNetCore.Mvc;

namespace EduWise.Controllers
{
    public class SourceController : Controller
    {
        public IActionResult Index()
        {
            // Later you can list all available sources (PDFs, YouTube, etc.)
            return View();
        }

        [HttpGet]
        public IActionResult SelectSource()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SelectSource(string selectedSource)
        {
            ViewBag.Message = $"Selected Source: {selectedSource}";
            return View("Index");
        }
    }
}
