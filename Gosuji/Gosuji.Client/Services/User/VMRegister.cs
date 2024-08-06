using Gosuji.Client.Resources.Translations;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Services.User
{
    public class VMRegister
    {
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$")]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        [RegularExpression(@"^(?=.*\d)(?=.*[@#$%^&+=!]).*$")]
        public string Password { get; set; }

        public bool IsGetChangelogEmail { get; set; } = false;

        [Required]
        [MinLength(4)]
        [MaxLength(7)]
        public string Language { get; set; }
    }
}
