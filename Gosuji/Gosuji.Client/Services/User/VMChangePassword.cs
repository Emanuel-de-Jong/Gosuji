using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Services.User
{
    public class VMChangePassword
    {
        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        [RegularExpression(@"^(?=.*\d)(?=.*[@#$%^&+=!]).*$")]
        public string NewPassword { get; set; }
    }
}
