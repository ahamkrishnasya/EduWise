namespace EduWise.Models
{
    public class UserProgress
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalAttempts { get; set; }
    }
}