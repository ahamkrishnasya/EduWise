using EduWise.Data;
using EduWise.Models;
using EduWise.Services; // for GeminiService
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EduWise.Controllers
{
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeminiService _geminiService;

        public QuizController(ApplicationDbContext context, GeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        // List all quizzes (questions)
        public IActionResult Index()
        {
            var quizzes = _context.Questions.ToList();
            return View(quizzes);
        }

        // Display PDF selection for quiz generation
        [HttpGet]
        public IActionResult Generate()
        {
            var pdfs = _context.PdfFiles.ToList();
            return View(pdfs);
        }

        // Generate quiz from selected PDF using AI
        [HttpPost]
        public async Task<IActionResult> Generate(int pdfId)
        {
            var pdf = _context.PdfFiles.Find(pdfId);
            if (pdf == null)
            {
                ViewBag.Message = "PDF not found!";
                return View();
            }

            // Extract text from PDF (you can implement a proper PDF text extraction method)
            string pdfText = System.IO.File.ReadAllText(pdf.FilePath); // Replace with proper extraction

            // Generate questions using Gemini AI
            string prompt = $"Generate 5 MCQs, 3 SAQs, 2 LAQs from the following text. Include question, options, correct answer, explanation: {pdfText}";
            var aiResponse = await _geminiService.GetResponseAsync(prompt);

            // Here, parse AI response JSON (assuming AI returns structured JSON as described earlier)
            var questions = ParseAiResponseToQuestions(aiResponse, pdfId);

            if (questions.Any())
            {
                _context.Questions.AddRange(questions);
                _context.SaveChanges();
            }

            ViewBag.Message = $"Quiz generated successfully! {questions.Count} questions added.";
            return View();
        }

        // Display quiz attempt page
        [HttpGet]
        public IActionResult Attempt(int pdfId)
        {
            var questions = _context.Questions.Where(q => q.PdfId == pdfId).ToList();
            return View(questions);
        }

        // Submit answers and score
        [HttpPost]
        public IActionResult Submit(Dictionary<int, string> answers, int pdfId)
        {
            int score = 0;
            var questions = _context.Questions.Where(q => q.PdfId == pdfId).ToList();

            foreach (var q in questions)
            {
                if (answers.ContainsKey(q.Id) && !string.IsNullOrWhiteSpace(answers[q.Id]))
                {
                    if (q.CorrectAnswer.Equals(answers[q.Id], StringComparison.OrdinalIgnoreCase))
                        score++;
                }
            }

            var attempt = new QuizAttempt
            {
                PdfFileId = pdfId,
                Score = score,
                AttemptedAt = DateTime.Now,
                AttemptDetailsJson = System.Text.Json.JsonSerializer.Serialize(answers)
            };

            _context.QuizAttempts.Add(attempt);
            _context.SaveChanges();

            ViewBag.Score = score;
            ViewBag.Total = questions.Count;
            ViewBag.Explanations = questions.ToDictionary(q => q.Id, q => q.Explanation);

            return View("Result");
        }

        // Helper method: parse AI response to List<Question>
        private List<Question> ParseAiResponseToQuestions(string aiJson, int pdfId)
        {
            var questions = new List<Question>();

            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(aiJson);
                var candidates = doc.RootElement.GetProperty("candidates");

                foreach (var candidate in candidates.EnumerateArray())
                {
                    var parts = candidate.GetProperty("content").GetProperty("parts");
                    foreach (var part in parts.EnumerateArray())
                    {
                        // Assuming each part.text contains a JSON object for one question
                        var text = part.GetProperty("text").GetString();
                        if (string.IsNullOrWhiteSpace(text)) continue;

                        // Parse each question JSON
                        var questionJson = System.Text.Json.JsonDocument.Parse(text);
                        var qElem = questionJson.RootElement;

                        questions.Add(new Question
                        {
                            PdfId = pdfId,
                            QuestionType = qElem.GetProperty("type").GetString(),
                            Text = qElem.GetProperty("question").GetString(),
                            OptionsJson = qElem.TryGetProperty("options", out var opts) ? opts.GetRawText() : null,
                            CorrectAnswer = qElem.GetProperty("answer").GetString(),
                            Explanation = qElem.GetProperty("explanation").GetString(),
                            SourcePage = qElem.TryGetProperty("sourcePage", out var page) ? page.GetInt32() : 0
                        });
                    }
                }
            }
            catch
            {
                // Handle parse errors (maybe log)
            }

            return questions;
        }
    }
}
