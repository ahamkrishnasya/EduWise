using Microsoft.EntityFrameworkCore;
using EduWise.Models;

namespace EduWise.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PdfFile> PdfFiles { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<UserProgress> UserProgress { get; set; }
    }
}
