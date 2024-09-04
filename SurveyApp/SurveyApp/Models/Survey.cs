namespace SurveyApp.Models
{
    public class Survey
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string ImagePath { get; set; } = "";
    }
}
