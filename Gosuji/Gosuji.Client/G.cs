using Gosuji.Client.Components.Shared;

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

        public static CStatusMessage StatusMessage;

        public const string ControllerRateLimitPolicyName = "ControllerRateLimitPolicy";
        public const string RazorRateLimitPolicyName = "RazorRateLimitPolicy";

        public const string LangLocalStorageName = "lang";
        public static string[] SupportedLangs = { "en", "zh", "ko", "ja" };

        public const string ReturnUriName = "ReturnUri";

        public static Dictionary<string, string> CSSLibUrls = new()
        {
            { "DataTables", "https://cdn.datatables.net/v/bs5/dt-2.0.8/sl-2.0.3/datatables.min.css" },
        };

        public static Dictionary<string, string> JSLibUrls = new()
        {
            { "JQuery", "https://code.jquery.com/jquery-3.7.0.min.js" },
            { "DataTables", "https://cdn.datatables.net/v/bs5/dt-2.0.8/sl-2.0.3/datatables.min.js" },
            { "Moment", "https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.30.1/moment.min.js" },
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
