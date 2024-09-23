using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace Gosuji.Client.Helpers
{
    public class ReflectionHelper
    {
        public static ConcurrentDictionary<Type, Dictionary<string, FieldInfo>> FieldCache { get; } = new();
        public static ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> PropertyCache { get; } = new();

        public static Dictionary<string, FieldInfo> GetFields(Type type)
        {
            return FieldCache.GetOrAdd(type, t => t
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => !f.Name.Contains("k__BackingField"))
                .ToDictionary(f => f.Name));
        }

        public static Dictionary<string, PropertyInfo> GetProperties(Type type)
        {
            return PropertyCache.GetOrAdd(type, t => t
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name));
        }

        public static bool SetProperty<T>(T obj, string name, string? value)
        {
            Type type = typeof(T);

            PropertyInfo? property = GetProperties(type).GetValueOrDefault(name);
            if (property == null)
            {
                return false;
            }

            Type propertyType = property.PropertyType;
            Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            object? convertedValue;
            if (value == null)
            {
                if (Nullable.GetUnderlyingType(propertyType) != null)
                {
                    convertedValue = null;
                }
                else
                {
                    return false;
                }
            }
            else if (underlyingType.IsEnum)
            {
                if (int.TryParse(value, out int enumValue))
                {
                    convertedValue = Enum.ToObject(underlyingType, enumValue);
                }
                else
                {
                    convertedValue = Enum.Parse(underlyingType, value);
                }
            }
            else
            {
                convertedValue = Convert.ChangeType(value, underlyingType);
            }

            property.SetValue(obj, convertedValue);
            return true;
        }

        public static T DeepClone<T>(T obj) where T : new()
        {
            Type type = typeof(T);
            T clone = new();

            Dictionary<string, FieldInfo> fields = GetFields(type);
            foreach (FieldInfo field in fields.Values)
            {
                if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                {
                    object fieldValue = field.GetValue(obj);
                    field.SetValue(clone, fieldValue);
                }
            }

            Dictionary<string, PropertyInfo> properties = GetProperties(type);
            foreach (PropertyInfo property in properties.Values)
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

            Dictionary<string, FieldInfo> fields = GetFields(type);
            foreach (FieldInfo field in fields.Values)
            {
                object? value = field.GetValue(obj);
                if (value != null)
                {
                    result.AppendLine($"{field.Name}: {value}");
                }
            }

            Dictionary<string, PropertyInfo> properties = GetProperties(type);
            foreach (PropertyInfo property in properties.Values)
            {
                object? value = property.GetValue(obj);
                if (value != null)
                {
                    result.AppendLine($"{property.Name}: {value}");
                }
            }

            return result.ToString();
        }

        public static bool CompareDifferences<T>(T obj1, T obj2)
        {
            List<string> differences = [];
            bool areEqual = CompareEqual(obj1, obj2, differences, typeof(T).Name);

            foreach (string difference in differences)
            {
                Console.WriteLine(difference);
            }

            return areEqual;
        }

        public static bool CompareEqual(object obj1, object obj2, List<string>? differences, string path)
        {
            if (obj1 == null && obj2 == null)
            {
                return true;
            }

            if (obj1 == null || obj2 == null)
            {
                if (differences != null)
                {
                    differences.Add($"{path} 1=[{obj1}] 2=[{obj2}]");
                }
                return false;
            }

            Type type = obj1.GetType();
            bool areEqual = true;

            IEnumerable<MemberInfo> members = GetFields(type).Values.Cast<MemberInfo>()
                .Concat(GetProperties(type).Values.Cast<MemberInfo>());

            foreach (MemberInfo? member in members)
            {
                Type memberType;
                object? value1;
                object? value2;

                if (member is FieldInfo field)
                {
                    memberType = field.FieldType;
                    value1 = field.GetValue(obj1);
                    value2 = field.GetValue(obj2);
                }
                else if (member is PropertyInfo property)
                {
                    memberType = property.PropertyType;
                    value1 = property.GetValue(obj1);
                    value2 = property.GetValue(obj2);
                }
                else
                {
                    continue;
                }

                if (memberType.IsClass && memberType != typeof(string))
                {
                    if (!CompareEqual(value1, value2, differences, $"{path}.{member.Name}"))
                    {
                        areEqual = false;
                    }
                }
                else if (!AreEqual(value1, value2))
                {
                    if (differences != null)
                    {
                        differences.Add($"{path}.{member.Name} 1=[{value1}] 2=[{value2}]");
                    }
                    areEqual = false;
                }
            }

            return areEqual;
        }

        private static bool AreEqual(object? value1, object? value2)
        {
            if (value1 == null && value2 == null)
            {
                return true;
            }

            if (value1 == null || value2 == null)
            {
                return false;
            }

            return value1.Equals(value2);
        }
    }
}
