using Gosuji.Client.Resources.Translations;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Services.User
{
    public class VMDeletePersonalData
    {
        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}
