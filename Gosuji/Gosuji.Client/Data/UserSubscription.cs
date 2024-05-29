using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class UserSubscription : DbModel
    {
        [Key] public long Id { get; set; }
        public string UserId { get; set; }
        public long SubscriptionTypeId { get; set; }
        public SubscriptionType? SubscriptionType { get; set; }
        public long? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        public int Months { get; set; }
        public DateTimeOffset? StartDate { get; set; }

        public override string ToString()
        {
            return "{" +
                "\nId: " + Id +
                "\nSubscriptionTypeId: " + SubscriptionTypeId +
                (SubscriptionType == null ? "" : "\nSubscriptionType: " + SubscriptionType) +
                "\nDiscountId: " + DiscountId +
                (Discount == null ? "" : "\nDiscount: " + Discount) +
                "\nMonths: " + Months +
                (StartDate == null ? "" : "\nStartDate: " + StartDate) +
                "\n" + base.ToString() +
                "\n}";
        }
    }
}
