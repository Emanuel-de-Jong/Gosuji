using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    // A UserState always has 1 User. A User always has 1 UserState.
    public class UserState : DbModel
    {
        [Key]
        public string Id { get; set; } // Same as User.Id

        public long? LastPresetId { get; set; }
        public Preset? LastPreset { get; set; }
    }
}
