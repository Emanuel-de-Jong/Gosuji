using Ganss.Xss;
using Gosuji.Client.Helpers;
using System.Reflection;

namespace Gosuji.API.Services
{
    public class SanitizeService
    {
        private HtmlSanitizer htmlSanitizer = new();

        public string Sanitize(string str)
        {
            return htmlSanitizer.Sanitize(str);
        }

        public void Sanitize<T>(T obj)
        {
            Type type = typeof(T);

            Dictionary<string, FieldInfo> fields = ReflectionHelper.GetFields(type);
            foreach (FieldInfo field in fields.Values)
            {
                if (field.FieldType == typeof(string))
                {
                    string? value = (string?)field.GetValue(obj);
                    if (value != null)
                    {
                        field.SetValue(obj, htmlSanitizer.Sanitize(value));
                    }
                }
            }

            Dictionary<string, PropertyInfo> properties = ReflectionHelper.GetProperties(type);
            foreach (PropertyInfo property in properties.Values)
            {
                if (property.CanWrite &&
                    property.PropertyType == typeof(string))
                {
                    string? value = (string?)property.GetValue(obj);
                    if (value != null)
                    {
                        property.SetValue(obj, htmlSanitizer.Sanitize(value));
                    }
                }
            }
        }
    }
}
