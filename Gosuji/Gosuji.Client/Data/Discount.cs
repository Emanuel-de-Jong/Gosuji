using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class Discount : DbModel
    {
        [Key] public long Id { get; set; }
        [MaxLength(100)]
        public string? Code { get; set; }
        [Required]
        public float Percent { get; set; }
        public DateTimeOffset? ExpireDate { get; set; }
    }
}
