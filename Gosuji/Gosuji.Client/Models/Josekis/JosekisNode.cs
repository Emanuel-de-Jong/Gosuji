using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Models.Josekis
{
    public class JosekisNode
    {
        [Range(0, 20)]
        public int? X { get; set; }
        [Range(0, 20)]
        public int? Y { get; set; }
        public bool IsBlack { get; set; }
        [MaxLength(1000)]
        public string? Comment { get; set; }
        public List<JosekisLabel>? Labels { get; set; }
        public List<JosekisMark>? Marks { get; set; }

        public JosekisNode()
        {
        }

        public JosekisNode(int x, int y)
        {
            X = x;
            Y = y;
        }

        public JosekisNode(string comment, List<JosekisLabel> labels, List<JosekisMark> marks)
        {
            Comment = comment;
            Labels = labels;
            Marks = marks;
        }

        public JosekisNode(int x, int y, bool isBlack, string comment, List<JosekisLabel> labels, List<JosekisMark> marks)
            : this(comment, labels, marks)
        {
            X = x;
            Y = y;
            IsBlack = isBlack;
        }
    }
}
