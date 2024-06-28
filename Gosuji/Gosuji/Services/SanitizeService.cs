using Ganss.Xss;
using Gosuji.Client.Data;
using Gosuji.Client.Helpers;
using Gosuji.Client.Models.Josekis;
using Gosuji.Client.Models.KataGo;
using System.Reflection;

namespace Gosuji.Services
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

            FieldInfo[] fields = ReflectionHelper.FieldCache.GetOrAdd(type, t => t.GetFields(BindingFlags.Public | BindingFlags.Instance));
            foreach (FieldInfo field in fields)
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

            PropertyInfo[] properties = ReflectionHelper.PropertyCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            foreach (PropertyInfo property in properties)
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
