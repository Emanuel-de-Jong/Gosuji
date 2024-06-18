using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class Changelog : DbModel
    {
        [Key] public long Id { get; set; }
        [StringLength(100)]
        public string Version { get; set; }
        [StringLength(250)]
        public string Name { get; set; }
        [StringLength(2500)]
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
