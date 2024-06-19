using Ganss.Xss;
using Gosuji.Client.Data;
using Gosuji.Client.Models.Josekis;
using Gosuji.Client.Models.KataGo;
using System.Reflection;

namespace Gosuji.Controllers
{
    public class Sanitizer
    {
        private static HtmlSanitizer htmlSanitizer = new();
        private static Dictionary<Type, List<FieldInfo>> fieldsToSanitize = new();
        private static Dictionary<Type, List<PropertyInfo>> propertiesToSanitize = new();

        public static void Init()
        {
            Type[] typesPosted = {
                // DataService
                typeof(TrainerSettingConfig),
                typeof(GameStat),
                typeof(Game),
                typeof(Feedback),
                typeof(SettingConfig),

                // JosekisService
                typeof(JosekisNode),

                // KataGoService
                typeof(Moves),
            };

            foreach (Type type in typesPosted)
            {
                fieldsToSanitize.Add(type, new());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(string))
                    {
                        fieldsToSanitize[type].Add(field);
                    }
                }

                propertiesToSanitize.Add(type, new());
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        propertiesToSanitize[type].Add(property);
                    }
                }
            }
        }

        public static string Sanitize(string str)
        {
            return htmlSanitizer.Sanitize(str);
        }

        public static void Sanitize<T>(T obj)
        {
            Type type = typeof(T);

            foreach (var field in fieldsToSanitize[type])
            {
                string value = (string)field.GetValue(obj);
                if (value != null)
                {
                    field.SetValue(obj, htmlSanitizer.Sanitize(value));
                }
            }

            foreach (var property in propertiesToSanitize[type])
            {
                string value = (string)property.GetValue(obj);
                if (value != null)
                {
                    property.SetValue(obj, htmlSanitizer.Sanitize(value));
                }
            }
        }
    }
}
