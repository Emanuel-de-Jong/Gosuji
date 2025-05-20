using Gosuji.Client.Components.Shared;
using Gosuji.Client.Models;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gosuji.Client
{
    public static class G
    {
        public static bool IsStandalone = true;

        public static LogLevel LogLevel =
#if DEBUG
                LogLevel.Trace;
#else
                LogLevel.None;
#endif

        public static bool IsFirstPage = true;

        public static CStatusMessage? StatusMessage = null;

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

        public static JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };

        public static void Log()
        {
            if (LogLevel > LogLevel.Trace)
            {
                return;
            }

            int frameIndex = 1;
            MethodBase? method = null;
            string? methodName = null;
            string? className = null;

            while (frameIndex < 10)
            {
                method = new StackFrame(frameIndex).GetMethod();
                if (method == null)
                {
                    break;
                }

                methodName = method.Name;
                className = method.DeclaringType?.Name;

                if (methodName != "MoveNext" && method.DeclaringType == null ||
                    (methodName != "Start" && method.DeclaringType?.Name != "AsyncMethodBuilderCore"))
                {
                    break;
                }

                frameIndex++;
            }

            if (method == null)
            {
                Console.WriteLine($"G.Log MethodBase null ({className}.{methodName} {frameIndex})");
                return;
            }

            if (methodName == "MoveNext" && method.DeclaringType?.DeclaringType != null)
            {
                methodName = method.DeclaringType.Name.Split('<', '>')[1];
                className = method.DeclaringType.DeclaringType.Name;
            }

            Console.WriteLine($"{className ?? "UnknownClass"}.{methodName ?? "UnknownMethod"}");
        }

        public static string CapSentence(string sentence)
        {
            sentence = sentence.Replace("_", " ").ToLower();
            sentence = char.ToUpper(sentence[0]) + sentence[1..];
            return sentence;
        }

        public static string ColorToName(EMoveColor color)
        {
            return color == EMoveColor.BLACK ? "B" : "W";
        }

        public static string ColorToFullName(EMoveColor color)
        {
            return color == EMoveColor.BLACK ? "Black" : "White";
        }
    }
}
