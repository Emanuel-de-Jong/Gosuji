using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class Language : DbModel
    {
        [Key] public long Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(10)]
        public string Short { get; set; }
    }
}
