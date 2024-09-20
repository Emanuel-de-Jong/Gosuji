using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class Changelog : DbModel
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(100)]
        public string? Version { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        [MaxLength(2500)]
        public string? Description { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; }
    }
}
