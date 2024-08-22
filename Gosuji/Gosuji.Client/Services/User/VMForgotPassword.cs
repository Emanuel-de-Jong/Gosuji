using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Services.User
{
    public class VMForgotPassword
    {
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$")]
        public string Email { get; set; }
    }
}
