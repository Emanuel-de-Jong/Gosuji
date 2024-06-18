using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public enum EFeedbackType
    {
        Support = 1,
        Suggestion = 2,
        ReportBug = 3,
        Other = 4,
    }

    public class Feedback : DbModel
    {
        [Key] public long Id { get; set; }
        [StringLength(100)]
        public string UserId { get; set; }
        public EFeedbackType FeedbackType { get; set; }
        [StringLength(250)]
        public string Subject { get; set; }
        [StringLength(1000)]
        public string? Message { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsResolved { get; set; } = false;
    }
}
