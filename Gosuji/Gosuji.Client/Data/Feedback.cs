using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public enum EFeedbackType
    {
        SUPPORT = 1,
        SUGGESTION = 2,
        REPORT_BUG = 3,
        OTHER = 4,
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
