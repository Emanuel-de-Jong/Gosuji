namespace Gosuji.Client.Models.Josekis
{
    public class JosekisLabel
    {
        public int X { get; set; }

        public int Y { get; set; }

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
