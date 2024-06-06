using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public enum ESubscriptionType
    {
        Free = 1,
        Level1 = 2,
        Level2 = 3,
        Level3 = 4,
    }

    public class UserSubscription : DbModel
    {
        [Key] public long Id { get; set; }
        public string UserId { get; set; }
        public ESubscriptionType SubscriptionType { get; set; }
        public long? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        public int Months { get; set; }

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
