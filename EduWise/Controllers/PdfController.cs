using EduWise.Data;
using EduWise.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduWise.Controllers
{
    public class PdfController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public PdfController(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        public IActionResult Index()
        {
            var pdfs = _context.PdfFiles.ToList();
            return View(pdfs);
        }

        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select a PDF.";
                return View();
            }

            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _context.PdfFiles.Add(new PdfFile
            {
                FileName = fileName,
                FilePath = "/uploads/" + fileName
            });
            await _context.SaveChangesAsync();

            ViewBag.Message = "File uploaded successfully!";
            return View();
        }
    }
}
