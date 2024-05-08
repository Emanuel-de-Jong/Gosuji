using System.ComponentModel.DataAnnotations;

namespace GosujiServer.Data
{
    public class Discount : DbModel
    {
        [Key] public long Id { get; set; }
        public string Code { get; set; }
        public float Percent { get; set; }
        public DateTimeOffset ExpireDate { get; set; }
    }
}
