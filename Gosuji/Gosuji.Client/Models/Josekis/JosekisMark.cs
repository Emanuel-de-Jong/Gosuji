namespace Gosuji.Client.Models.Josekis
{
    public class JosekisMark
    {
        public int X { get; set; }

        public int Y { get; set; }

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
