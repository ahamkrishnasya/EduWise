namespace EduWise.Models
{   public class QuizAttempt
    {
        public int Id { get; set; }
        public int PdfFileId { get; set; }
        public int Score { get; set; }
        public DateTime AttemptedOn { get; set; } = DateTime.Now;
    }
}