using Gosuji.Client.Components.Shared;
using System.Diagnostics;
using System.Reflection;

namespace Gosuji.Client
{
    public static class G
    {
        public static LogLevel LogLevel =
#if DEBUG
                LogLevel.Trace;
#else
                LogLevel.None;
#endif

        public static CStatusMessage? StatusMessage = null;

        public const string ControllerRateLimitPolicyName = "ControllerRateLimitPolicy";
        public const string RazorRateLimitPolicyName = "RazorRateLimitPolicy";

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

        public static void Log()
        {
            if (LogLevel > LogLevel.Trace) {
                return;
            }

            int frameIndex = 1;
            MethodBase? method = null;
            string? methodName = null;
            string? className = null;

            while (frameIndex < 10) {
                method = new StackFrame(frameIndex).GetMethod();
                if (method == null) {
                    break;
                }

                methodName = method.Name;
                className = method.DeclaringType?.Name;

                if (methodName != "MoveNext" && method.DeclaringType == null ||
                    (methodName != "Start" && method.DeclaringType?.Name != "AsyncMethodBuilderCore")) {
                    break;
                }

                frameIndex++;
            }

            if (method == null) {
                Console.WriteLine($"G.Log MethodBase null ({className}.{methodName} {frameIndex})");
                return;
            }

            if (methodName == "MoveNext" && method.DeclaringType?.DeclaringType != null) {
                methodName = method.DeclaringType.Name.Split('<', '>')[1];
                className = method.DeclaringType.DeclaringType.Name;
            }

            Console.WriteLine($"{className ?? "UnknownClass"}.{methodName ?? "UnknownMethod"}");
        }

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
