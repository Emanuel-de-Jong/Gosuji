using System.Text;
using System.Reflection;

namespace Gosuji.Client.Helpers
{
    public class DebugHelper
    {
        public static string ToString<T>(T obj) where T : class
        {
            StringBuilder result = new();
            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object? value = property.GetValue(obj);
                if (value != null)
                {
                    result.AppendLine($"{property.Name}: {value}");
                }
            }

            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                object? value = field.GetValue(obj);
                if (value != null)
                {
                    result.AppendLine($"{field.Name}: {value}");
                }
            }

            return result.ToString();
        }
    }
}
