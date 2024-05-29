using Gosuji.Client.Models.Josekis;

namespace Gosuji.Client.Services
{
    public interface IJosekisService
    {
        Task AddSession(int sessionId);
        Task RemoveSession(int sessionId);
        Task<JosekisNode> Current(int sessionId);
        Task ToParent(int sessionId);
        Task<int> ToLastBranch(int sessionId);
        Task ToFirst(int sessionId);
        Task<bool> ToChild(int sessionId, JosekisNode childToGo);
    }
}
