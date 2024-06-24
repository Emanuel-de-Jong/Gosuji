using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class DbModel : IDbModel
    {
        [Required]
        [CustomPersonalData]
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
        [CustomPersonalData]
        public DateTimeOffset? ModifyDate { get; set; }

        public override string ToString()
        {
            return "CreateDate: " + CreateDate +
                (ModifyDate == null ? "" : "\nModifyDate: " + ModifyDate);
        }

        public void Update(DbModel newModel)
        {
            CreateDate = newModel.CreateDate;
            ModifyDate = newModel.ModifyDate;
        }
    }
}
