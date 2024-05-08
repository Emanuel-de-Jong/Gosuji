namespace GosujiServer
{
    public static class G
    {
        public static bool Log =
#if DEBUG
                true;
#else
                false;
#endif

        public static bool LogRouting = false;

        //public static string ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=aspnet-GosujiServer-24E974ED-5CCD-44B7-9AE4-ECBD51EEE5E9;Persist Security Info=True;User ID=sa;Password=@Password1;MultipleActiveResultSets=True";
        public static string ConnectionString = "Data Source=gosuji.db";

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
