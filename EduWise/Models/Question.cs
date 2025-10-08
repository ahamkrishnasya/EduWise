namespace EduWise.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Type { get; set; }  // MCQ, SAQ, LAQ
        public string Text { get; set; }
        public string OptionsJson { get; set; } // Store options if MCQ
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public int PdfFileId { get; set; }
    }
}