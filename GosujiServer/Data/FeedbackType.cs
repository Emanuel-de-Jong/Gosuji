using System.ComponentModel.DataAnnotations;

namespace GosujiServer.Data
{
    public class FeedbackType : DbModel
    {
        [Key] public long Id { get; set; }
        public string Name { get; set; }
    }
}
