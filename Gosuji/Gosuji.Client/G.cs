namespace Gosuji.Client
{
    public static class G
    {
        public static bool Log =
#if DEBUG
                true;
#else
                false;
#endif

        public static string ColorNumToName(int colorNum)
        {
            return colorNum == -1 ? "B" : "W";
        }

        public static string ColorNumToFullName(int colorNum)
        {
            return colorNum == -1 ? "Black" : "White";
        }
    }
}
