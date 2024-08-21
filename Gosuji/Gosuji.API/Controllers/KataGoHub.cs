using Gosuji.API.Services;
using Gosuji.Client.Models.KataGo;
using Microsoft.AspNetCore.SignalR;

namespace Gosuji.API.Controllers
{
    public class KataGoHub : Hub
    {
        public async Task<string> Test(string input)
        {
            Console.WriteLine(input);
            return "output";
        }
    }
}
