using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class Language : DbModel
    {
        [Key] public long Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(15)]
        [CustomPersonalData]
        public string Short { get; set; }
    }
}
