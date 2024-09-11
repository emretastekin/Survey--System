using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Models
{
    public class Survey
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]    
        public string Surname { get; set; } = "";

        [EmailAddress]
        public string Email { get; set; } = "";

        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = "";

        [StringLength(200)]
        public string? Answer1 { get; set; }

        public string? ImagePath { get; set; }

        public int? Rating { get; set; }

        public bool? YesNoAnswer { get; set; }
    
    }
}
