using Ganss.Xss;
using Gosuji.Client.Data;
using Gosuji.Client.Models.Josekis;
using Gosuji.Client.Models.KataGo;
using System.Reflection;

namespace Gosuji.Services
{
    public class SanitizeService
    {
        private HtmlSanitizer htmlSanitizer = new();
        private Dictionary<Type, List<FieldInfo>> fieldsToSanitize = new();
        private Dictionary<Type, List<PropertyInfo>> propertiesToSanitize = new();

        public SanitizeService()
        {
            Type[] typesPosted = {
                // DataController
                typeof(TrainerSettingConfig),
                typeof(GameStat),
                typeof(Game),
                typeof(Feedback),
                typeof(SettingConfig),

                // JosekisController
                typeof(JosekisNode),

                // KataGoController
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

        public string Sanitize(string str)
        {
            return htmlSanitizer.Sanitize(str);
        }

        public void Sanitize<T>(T obj)
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
