using Gosuji.Client.Data;

namespace Gosuji.Data
{
    public class TextValue : DbModel
    {
        public long LanguageId { get; set; }
        public Language? Language { get; set; }
        public long TextKeyId { get; set; }
        public TextKey? TextKey { get; set; }
        public string Value { get; set; }
    }
}
