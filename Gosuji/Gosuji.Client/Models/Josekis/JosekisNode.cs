namespace Gosuji.Client.Models.Josekis
{
    public class JosekisNode
    {
        public int? X { get; set; }
        public int? Y { get; set; }
        public bool IsBlack { get; set; }
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
