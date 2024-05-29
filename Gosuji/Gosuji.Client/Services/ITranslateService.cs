namespace Gosuji.Client.Services
{
    public interface ITranslateService
    {
        bool IsInitialized { get; }

        Task Init();
        string? Get(string? userId, string key);
    }
}
