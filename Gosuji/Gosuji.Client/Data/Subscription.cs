using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gosuji.Client.Data
{
    public enum ESubscriptionType
    {
        LEVEL_1 = 1,
        LEVEL_2 = 2,
        LEVEL_3 = 3,
    }

    public class Subscription : DbModel
    {
        [Key] public long Id { get; set; }
        [Required]
        [StringLength(36)]
        public string UserId { get; set; }
        [Required]
        [CustomPersonalData]
        public ESubscriptionType SubscriptionType { get; set; }
        public long? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        [Required]
        [CustomPersonalData]
        public int Months { get; set; }
        [NotMapped]
        [CustomPersonalData]
        public DateTimeOffset EndDate => CreateDate.AddMonths(Months);
    }
}
