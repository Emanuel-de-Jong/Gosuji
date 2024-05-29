using System.ComponentModel.DataAnnotations;

namespace GosujiServer.Data
{
    public class Language : DbModel
    {
        [Key] public long Id { get; set; }
        public string Name { get; set; }
        public string Short { get; set; }
        public string Flag { get; set; }
    }
}
