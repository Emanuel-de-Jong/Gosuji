using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Models.KataGo
{
    public class Moves
    {
        [Required]
        public Move[] moves { get; set; } = Array.Empty<Move>();
    }
}
