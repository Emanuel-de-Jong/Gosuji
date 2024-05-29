using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class KataGoVersion : DbModel
    {
        public const string VERSION = "1.14.1";
        public const string MODEL = "kata1-b28c512nbt-s6797380608-d4265362003";

        [Key] public long Id { get; set; }
        public string Version { get; set; }
        public string Model { get; set; }
        public string Config { get; set; }

        public static string GetConfig()
        {
            return File.ReadAllText(@"Resources\KataGo\default_gtp.cfg");
        }

        public static KataGoVersion GetCurrent()
        {
            return new KataGoVersion()
            {
                Version = VERSION,
                Model = MODEL,
                Config = GetConfig()
            };
        }
    }
}
