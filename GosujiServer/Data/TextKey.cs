using System.ComponentModel.DataAnnotations;

namespace GosujiServer.Data
{
    public class TextKey : DbModel
    {
        [Key] public long Id { get; set; }
        public string Key { get; set; }
    }
}
