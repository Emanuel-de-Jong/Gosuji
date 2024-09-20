using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gosuji.Client.Data
{
    public class EncodedGameData : DbModel
    {
        [StringLength(12)]
        [Key]
        public string Id { get; set; }

        [Required]
        public byte[] Data { get; set; }

        [NotMapped]
        public List<byte> DataList => Data.ToList();

        public EncodedGameData() { }

        public EncodedGameData(string id, List<byte> data)
        {
            Id = id;
            SetData(data);
        }

        public void SetData(List<byte> data)
        {
            Data = data.ToArray();
        }
    }
}
