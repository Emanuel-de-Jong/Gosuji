using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public enum ELanguage
    {
        en,
        zh,
        ko,
        ja
    }

    public class Language : DbModel
    {
        [MaxLength(15)]
        [CustomPersonalData]
        [Key] public string Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
