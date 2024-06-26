using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class UserState : DbModel
    {
        [Key] public string Id { get; set; }

        [Required]
        public long LastPresetId { get; set; }
        public Preset LastPreset { get; set; }
    }
}
