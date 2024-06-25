using Gosuji.Client.Data.Attributes;
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
        [StringLength(36)]
        public string? UserId { get; set; }
        [Required]
        [CustomPersonalData]
        public EFeedbackType FeedbackType { get; set; }
        [Required]
        [MaxLength(250)]
        [MinLength(3)]
        [CustomPersonalData]
        public string Subject { get; set; }
        [MaxLength(1000)]
        [CustomPersonalData]
        public string? Message { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsResolved { get; set; } = false;
    }
}
