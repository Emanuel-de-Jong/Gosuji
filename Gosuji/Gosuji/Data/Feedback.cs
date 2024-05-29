using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Data
{
    public class Feedback : DbModel
    {
        [Key] public long Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public long FeedbackTypeId { get; set; }
        public FeedbackType? FeedbackType { get; set; }
        public string? Subject { get; set; }
        public string Message { get; set; }
    }
}
