using System.ComponentModel.DataAnnotations;

namespace GosujiServer.Data
{
    public class KataGoVersion : DbModel
    {
        public const string VERSION = "1.14.1";
        public const string MODEL = "kata1-b28c512nbt-s6797380608-d4265362003";

        [Key] public long Id { get; set; }
        public string Version { get; set; } = VERSION;
        public string Model { get; set; } = MODEL;
        public string Config { get; set; } = GetConfig();

        public static string GetConfig()
        {
            return File.ReadAllText(@"Resources\KataGo\default_gtp.cfg");
        }

    }
}
