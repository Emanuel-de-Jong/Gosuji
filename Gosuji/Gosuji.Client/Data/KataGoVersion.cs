using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Gosuji.Client.Data
{
    public class KataGoVersion : DbModel
    {
        [NotMapped]
        public const string VERSION = "1.14.1";
        [NotMapped]
        public const string MODEL = "kata1-b28c512nbt-s6797380608-d4265362003";

        [Key] public long Id { get; set; }
        [StringLength(50)]
        public string Version { get; set; }
        [StringLength(100)]
        public string Model { get; set; }
        [StringLength(150)]
        public string Config { get; set; }

        public static string GetConfig()
        {
            string config = File.ReadAllText(@"Resources\KataGo\default_gtp.cfg");

            using MD5 hasher = MD5.Create();
            byte[] bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(config));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
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
