using System.Text;
using System.Reflection;
using System.Collections.Concurrent;

namespace Gosuji.Client.Helpers
{
    public class ReflectionHelper
    {
        public static ConcurrentDictionary<Type, FieldInfo[]> FieldCache { get; } = new();
        public static ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache { get; } = new();

        public static T DeepClone<T>(T obj) where T : new()
        {
            Type type = typeof(T);
            T clone = new();

            FieldInfo[] fields = FieldCache.GetOrAdd(type, t => t.GetFields(BindingFlags.Public | BindingFlags.Instance));
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                {
                    object fieldValue = field.GetValue(obj);
                    field.SetValue(clone, fieldValue);
                }
            }

            PropertyInfo[] properties = PropertyCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite &&
                    (property.PropertyType.IsValueType || property.PropertyType == typeof(string)))
                {
                    object propertyValue = property.GetValue(obj);
                    property.SetValue(clone, propertyValue);
                }
            }

            return clone;
        }

        public static string ToString<T>(T obj)
        {
            Type type = typeof(T);
            StringBuilder result = new();

            FieldInfo[] fields = FieldCache.GetOrAdd(type, t => t.GetFields(BindingFlags.Public | BindingFlags.Instance));
            foreach (FieldInfo field in fields)
            {
                object? value = field.GetValue(obj);
                if (value != null)
                {
                    result.AppendLine($"{field.Name}: {value}");
                }
            }

            PropertyInfo[] properties = PropertyCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            foreach (PropertyInfo property in properties)
            {
                object? value = property.GetValue(obj);
                if (value != null)
                {
                    result.AppendLine($"{property.Name}: {value}");
                }
            }

            return result.ToString();
        }
    }
}
