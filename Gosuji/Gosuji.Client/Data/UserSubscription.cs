using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gosuji.Client.Data
{
    public enum ESubscriptionType
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
    }

    public class UserSubscription : DbModel
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
