using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gosuji.Client.Data
{
    public class KataGoVersion : DbModel
    {
        [NotMapped]
        public const string VERSION = "1.16.2";
        // Download: https://media.katagotraining.org/uploaded/networks/models/kata1/kata1-b28c512nbt-s8954935040-d4794564322.bin.gz
        [NotMapped]
        public const string MODEL = "kata1-b28c512nbt-s8954935040-d4794564322";
        [NotMapped]
        public const string BACKEND = "OpenCL"; // OpenCL TensorRT

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Version { get; set; }
        [Required]
        [MaxLength(100)]
        public string Model { get; set; }
        [Required]
        [MaxLength(50_000)]
        public string Config { get; set; }

        public static string GetConfig()
        {
            return File.ReadAllText($"Resources/KataGo/{BACKEND}/default_gtp.cfg");
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
