using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Models.Josekis
{
    public class JosekisLabel
    {
        [Required]
        [Range(0, 18)]
        public int X { get; set; }
        [Required]
        [Range(0, 18)]
        public int Y { get; set; }
        [Required]
        [MaxLength(3)]
        [MinLength(1)]
        public string Text { get; set; }

        public JosekisLabel()
        {
        }

        public JosekisLabel(int x, int y, string text)
        {
            X = x;
            Y = y;
            Text = text;
        }
    }
}
