using EduWise.Data;
using EduWise.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduWise.Controllers
{
    public class SourceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;


        public SourceController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: /Source/
        public IActionResult Index()
        {
            var pdfs = _context.PdfFiles.ToList();
            return View(pdfs);
        }

        // POST: /Source/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a PDF file.";
                return RedirectToAction(nameof(Index));
            }

            // Ensure uploads directory exists
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Save file
            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save record in DB
            var pdf = new PdfFile
            {
                FileName = file.FileName,
                FilePath = $"/uploads/{fileName}",
                UploadedAt = DateTime.Now
            };
            _context.PdfFiles.Add(pdf);
            await _context.SaveChangesAsync();

            TempData["Success"] = "PDF uploaded successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Source/ViewPdf/{id}
        public IActionResult ViewPdf(int id)
        {
            var pdf = _context.PdfFiles.FirstOrDefault(x => x.Id == id);
            if (pdf == null)
                return NotFound();

            return View("ChatView", pdf);
        }
    }
}
