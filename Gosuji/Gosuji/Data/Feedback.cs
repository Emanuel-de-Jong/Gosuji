using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Data
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
        public string UserId { get; set; }
        public EFeedbackType FeedbackType { get; set; }
        public string? Subject { get; set; }
        public string Message { get; set; }
    }
}
