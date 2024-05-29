using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class Changelog : DbModel
    {
        [Key] public long Id { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
