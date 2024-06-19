using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Models.Josekis
{
    public class JosekisMark
    {
        [Required]
        [Range(0, 18)]
        public int X { get; set; }
        [Required]
        [Range(0, 18)]
        public int Y { get; set; }
        [Required]
        public JosekisMarkType MarkType { get; set; }

        public JosekisMark()
        {
        }

        public JosekisMark(int x, int y, JosekisMarkType markType)
        {
            X = x;
            Y = y;
            MarkType = markType;
        }
    }
}
