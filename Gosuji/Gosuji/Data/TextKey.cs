using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Data
{
    public class TextKey : DbModel
    {
        [Key] public long Id { get; set; }
        public string Key { get; set; }

        public TextKey(string key)
        {
            Key = key;
        }
    }
}
