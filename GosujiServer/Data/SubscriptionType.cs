using System.ComponentModel.DataAnnotations;

namespace GosujiServer.Data
{
    public class SubscriptionType : DbModel
    {
        [Key] public long Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return "{" +
                "\nId: " + Id +
                "\nName: " + Name +
                "\n" + base.ToString() +
                "\n}";
        }
    }
}
