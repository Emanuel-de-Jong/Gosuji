using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class Language : DbModel
    {
        [Key] public long Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(15)]
        public string Short { get; set; }
    }
}
