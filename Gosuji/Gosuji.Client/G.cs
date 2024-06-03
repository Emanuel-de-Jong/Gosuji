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

        public static Dictionary<string, string> CSSLibUrls = new Dictionary<string, string>()
        {
            { "DataTablesBootstrap", "https://cdn.datatables.net/1.13.1/css/dataTables.bootstrap5.min.css" },
        };

        public static Dictionary<string, string> JSLibUrls = new Dictionary<string, string>()
        {
            { "JQuery", "https://code.jquery.com/jquery-3.6.0.min.js" },
            { "DataTables", "https://cdn.datatables.net/1.13.1/js/jquery.dataTables.min.js" },
            { "DataTablesBootstrap", "https://cdn.datatables.net/1.13.1/js/dataTables.bootstrap5.min.js" },
            { "ChartJS", "https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.9.1/chart.min.js" },
        };

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
