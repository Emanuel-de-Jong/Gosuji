namespace Gosuji.API.Services
{
    public class TrainerInstance
    {
        public string ConnectionId { get; set; }

        public TrainerInstance(string connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}
