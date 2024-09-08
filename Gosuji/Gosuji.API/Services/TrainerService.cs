
namespace Gosuji.API.Services
{
    public class TrainerService : IAsyncDisposable
    {
        public string ConnectionId { get; set; }

        public TrainerService(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public async ValueTask DisposeAsync()
        {

        }
    }
}
