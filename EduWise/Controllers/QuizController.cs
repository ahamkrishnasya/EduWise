using EduWise.Data;
using EduWise.Models;
using Microsoft.AspNetCore.Mvc;

namespace YourApp.Controllers
{
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var quizzes = _context.Questions.ToList();
            return View(quizzes);
        }

        [HttpGet]
        public IActionResult Generate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(int pdfId)
        {
            // Mock quiz generation (you can replace this later with AI)
            //var questions = new List<Question>
            //{
            //    new Question { PdfId = pdfId, QuestionType="MCQ", Text="What is gravity?", Answer="Force of attraction between masses", Explanation="Newton's universal law", SourcePage=5 },
            //    new Question { PdfId = pdfId, QuestionType="SAQ", Text="Define velocity.", Answer="Rate of change of displacement", Explanation="Physics definition", SourcePage=2 }
            //};

            _context.Questions.AddRange();
            _context.SaveChanges();

            ViewBag.Message = "Quiz generated successfully!";
            return View();
        }

        [HttpGet]
        public IActionResult Attempt(int quizId)
        {
            var questions = _context.Questions.Where(q => q.Id == quizId).ToList();
            return View(questions);
        }

        [HttpPost]
        public IActionResult Submit(Dictionary<int, string> answers)
        {
            int score = 0;
            foreach (var item in answers)
            {
                var q = _context.Questions.Find(item.Key);
                if (q != null && q.CorrectAnswer.Equals(item.Value, StringComparison.OrdinalIgnoreCase))
                    score++;
            }

            var attempt = new QuizAttempt
            {
                PdfFileId = 1, // mock
                Score = score,
                //AttemptDetailsJson = System.Text.Json.JsonSerializer.Serialize(answers)
            };

            _context.QuizAttempts.Add(attempt);
            _context.SaveChanges();

            ViewBag.Score = score;
            return View("Result");
        }
    }
}
