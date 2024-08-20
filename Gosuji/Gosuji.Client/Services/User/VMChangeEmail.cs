using Gosuji.Client.Resources.Translations;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Services.User
{
    public class VMChangeEmail
    {
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        [MinLength(32)]
        [MaxLength(32)]
        public string BackupCode { get; set; }

        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$")]
        public string NewEmail { get; set; }
    }
}
