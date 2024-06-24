namespace Gosuji.Components.Account
{
    public class PersonalData
    {
        public Dictionary<string, string> User { get; set; } = [];
        public List<Dictionary<string, string>> Activities { get; set; } = [];
        public List<Dictionary<string, string>> Feedbacks { get; set; } = [];
    }
}
