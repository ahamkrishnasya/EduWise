using System;

namespace EduWise.Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }

        public int PdfFileId { get; set; }

        public int Score { get; set; }

        // Store user answers as JSON
        public string AttemptDetailsJson { get; set; }

        // Timestamp of attempt
        public DateTime AttemptedAt { get; set; } = DateTime.Now;
    }
}
