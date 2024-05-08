namespace GosujiServer.Models
{
    public class Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public float? A { get; set; }

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
        public Color(byte r, byte g, byte b, float a) : this(r, g, b)
        {
            A = a;
        }

        public string ToCSS()
        {
            return "rgb" +
                (A != null ? "a" : "") +
                "(" + R + "," + G + "," + B +
                (A != null ? "," + A : "") +
                ")";
        }
    }
}
