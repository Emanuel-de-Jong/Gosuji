using Gosuji.Client.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Services.User
{
    public class VMUpdatePrivacy
    {
        [MinLength(2)]
        [MaxLength(30)]
        [RegularExpression(@"^(?!\s*$).+$")] // Not just whitespace
        public string? UserName { get; set; }

        [MaxLength(50)]
        [RegularExpression(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$")]
        public string? Email { get; set; }

        [MinLength(6)]
        [MaxLength(50)]
        [NotEqual(nameof(CurrentPassword))]
        [RegularExpression(@"^(?=.*\d)(?=.*[@#$%^&+=!]).*$")] // At least one digit and special character
        public string? NewPassword { get; set; }

        [Required]
        [MaxLength(50)]
        public string CurrentPassword { get; set; }
    }
}
