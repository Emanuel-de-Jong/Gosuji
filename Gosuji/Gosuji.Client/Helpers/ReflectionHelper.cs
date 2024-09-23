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

        public static bool Equals(object? obj1, object? obj2)
        {
            if (obj1 == null && obj2 == null)
            {
                return true;
            }

            if (obj1 == null || obj2 == null)
            {
                return false;
            }

            if (obj1.GetType() != obj2.GetType())
            {
                return false;
            }

            Type type = obj1.GetType();

            Dictionary<string, FieldInfo> fields = FieldCache.GetOrAdd(type, t => t
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .ToDictionary(f => f.Name));
            foreach (FieldInfo field in fields.Values)
            {
                object? value1 = field.GetValue(obj1);
                object? value2 = field.GetValue(obj2);

                if (!AreEqual(value1, value2))
                {
                    return false;
                }
            }

            Dictionary<string, PropertyInfo> properties = PropertyCache.GetOrAdd(type, t => t
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name));
            foreach (PropertyInfo property in properties.Values)
            {
                object? value1 = property.GetValue(obj1);
                object? value2 = property.GetValue(obj2);

                if (!AreEqual(value1, value2))
                {
                    return false;
                }
            }

            return true;
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

            Type type1 = value1.GetType();
            Type type2 = value2.GetType();

            if (type1 != type2)
            {
                return false;
            }

            if (type1.IsPrimitive || type1 == typeof(string) || type1.IsValueType)
            {
                return value1.Equals(value2);
            }

            return Equals(value1, value2);
        }

        public static bool CompareAndLogDifferences(object? obj1, object? obj2)
        {
            if (obj1 == null && obj2 == null)
            {
                return true;
            }

            if (obj1 == null || obj2 == null)
            {
                Console.WriteLine($"One of the objects is null. obj1: {obj1}, obj2: {obj2}");
                return false;
            }

            if (obj1.GetType() != obj2.GetType())
            {
                Console.WriteLine($"Objects are of different types. obj1 type: {obj1.GetType().Name}, obj2 type: {obj2.GetType().Name}");
                return false;
            }

            Type type = obj1.GetType();
            bool isEqual = true;

            Dictionary<string, FieldInfo> fields = GetFields(type);
            foreach (FieldInfo field in fields.Values)
            {
                object? value1 = field.GetValue(obj1);
                object? value2 = field.GetValue(obj2);

                if (!AreEqualWithLogging(field.Name, value1, value2, field.FieldType, type.Name))
                {
                    isEqual = false;
                }
            }

            Dictionary<string, PropertyInfo> properties = GetProperties(type);
            foreach (PropertyInfo property in properties.Values)
            {
                object? value1 = property.GetValue(obj1);
                object? value2 = property.GetValue(obj2);

                if (!AreEqualWithLogging(property.Name, value1, value2, property.PropertyType, type.Name))
                {
                    isEqual = false;
                }
            }

            return isEqual;
        }

        private static bool AreEqualWithLogging(string memberName, object? value1, object? value2, Type memberType, string className)
        {
            if (value1 == null && value2 == null)
            {
                return true;
            }

            if (value1 == null || value2 == null)
            {
                Console.WriteLine($"{className}.{memberName} differs. Value1: {value1 ?? "null"}, Value2: {value2 ?? "null"}");
                return false;
            }

            if (value1.GetType() != value2.GetType())
            {
                Console.WriteLine($"{className}.{memberName} differs. Different types: {value1.GetType().Name} vs {value2.GetType().Name}");
                return false;
            }

            if (value1.GetType().IsPrimitive || value1 is string || value1 is ValueType)
            {
                if (!value1.Equals(value2))
                {
                    Console.WriteLine($"{className}.{memberName} differs. Value1: {value1}, Value2: {value2}");
                    return false;
                }
            }
            else
            {
                if (!CompareAndLogDifferences(value1, value2))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
