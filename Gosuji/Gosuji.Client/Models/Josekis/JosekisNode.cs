using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Models.Josekis
{
    public class JosekisNode
    {
        public Move Move { get; set; }
        [MaxLength(1000)]
        public string? Comment { get; set; }
        public List<JosekisLabel>? Labels { get; set; }
        public List<JosekisMark>? Marks { get; set; }

        public JosekisNode()
        {
        }

        public JosekisNode(Move move)
        {
            Move = move;
        }

        public JosekisNode(string comment, List<JosekisLabel> labels, List<JosekisMark> marks)
        {
            Comment = comment;
            Labels = labels;
            Marks = marks;
        }

        public JosekisNode(Move move, string comment, List<JosekisLabel> labels, List<JosekisMark> marks)
            : this(comment, labels, marks)
        {
            Move = move;
        }
    }
}
