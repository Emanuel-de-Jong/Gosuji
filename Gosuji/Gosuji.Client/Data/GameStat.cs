using Gosuji.Client.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gosuji.Client.Data
{
    public class GameStat : DbModel
    {
        public static Color RIGHT_COLOR = new(255, 120, 0);
        public static Color PERFECT_COLOR = new(0, 200, 0);


        [Key]
        public long Id { get; set; }

        [Required]
        [Range(0, 500)]
        public int From { get; set; }
        public int To { get; set; }

        [Required]
        [Range(0, 500)]
        public int Total { get; set; }

        [Required]
        [Range(0, 500)]
        public int Right { get; set; }
        [NotMapped]
        public int RightPercent => (int)((double)Right / Total * 100);

        [Required]
        [Range(0, 500)]
        public int Perfect { get; set; }
        [NotMapped]
        public int PerfectPercent => (int)((double)Perfect / Total * 100);

        public GameStat() { }

        public GameStat(int from, int to)
        {
            From = from;
            To = to;
        }

        public bool Equal(GameStat other)
        {
            return From == other.From &&
                To == other.To &&
                Right == other.Right &&
                Perfect == other.Perfect;
        }
    }
}
