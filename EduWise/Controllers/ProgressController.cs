using EduWise.Data;
using Microsoft.AspNetCore.Mvc;
using EduWise.Models;

namespace EduWise.Controllers
{
    public class ProgressController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgressController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var progressList = _context.UserProgress.ToList();
            return View(progressList);
        }

        [HttpPost]
        public IActionResult Update(string topic, int correct, int total)
        {
            var progress = _context.UserProgress.FirstOrDefault(p => p.Topic == topic);
            if (progress == null)
            {
                progress = new UserProgress { Topic = topic, CorrectAnswers = correct, TotalAttempts = total };
                _context.UserProgress.Add(progress);
            }
            else
            {
                progress.CorrectAnswers += correct;
                progress.TotalAttempts += total;
            }

            _context.SaveChanges();
            ViewBag.Message = "Progress updated!";
            return RedirectToAction("Index");
        }
    }
}
