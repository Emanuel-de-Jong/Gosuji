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
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}
