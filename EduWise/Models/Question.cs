namespace EduWise.Models
{
    public class Question
    {
        public int Id { get; set; }

        // Use consistent property names as in controller
        public string QuestionType { get; set; }  // MCQ, SAQ, LAQ
        public string Text { get; set; }

        // Store options as JSON for MCQs
        public string OptionsJson { get; set; }

        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }

        // Reference to the PDF this question is from
        public int PdfId { get; set; }  // renamed from PdfFileId to match controller
        public int SourcePage { get; set; } // optional: which page the question came from
    }
}
