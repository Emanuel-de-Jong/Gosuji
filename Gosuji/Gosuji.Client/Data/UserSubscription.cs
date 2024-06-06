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
        public string UserId { get; set; }
        public ESubscriptionType SubscriptionType { get; set; }
        public long? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        public int Months { get; set; }
        [NotMapped]
        public DateTimeOffset EndDate => CreateDate.AddMonths(Months);

        public override string ToString()
        {
            return "{" +
                "\nId: " + Id +
                "\nSubscriptionType: " + SubscriptionType +
                "\nDiscountId: " + DiscountId +
                (Discount == null ? "" : "\nDiscount: " + Discount) +
                "\nMonths: " + Months +
                "\n" + base.ToString() +
                "\n}";
        }
    }
}
