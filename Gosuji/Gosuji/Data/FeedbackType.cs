using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Data
{
    public class FeedbackType : DbModel
    {
        [Key] public long Id { get; set; }
        public string Name { get; set; }
    }
}
