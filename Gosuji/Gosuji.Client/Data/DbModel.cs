using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class DbModel : IDbModel
    {
        [Required]
        [CustomPersonalData]
        public DateTimeOffset CreateDate { get; set; }
        [Required]
        [CustomPersonalData]
        public DateTimeOffset ModifyDate { get; set; }

        public DbModel()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            CreateDate = now;
            ModifyDate = now;
        }
    }
}
