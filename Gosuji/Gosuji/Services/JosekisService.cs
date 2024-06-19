using Gosuji.Client.Models.Josekis;
using Gosuji.Client.Services;
using Gosuji.Controllers;
using IGOEnchi.GoGameLogic;
using IGOEnchi.GoGameSgf;
using IGOEnchi.SmartGameLib;
using IGOEnchi.SmartGameLib.models;

namespace Gosuji.Services
{
    public class JosekisService : IServerService, IJosekisService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private Dictionary<int, GoNode> JosekisGoNodes;

        private GoGame baseGame;

        public JosekisService(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;

            JosekisGoNodes = [];

            using FileStream fileStream = File.OpenRead(@"Resources\AI-Josekis-40-0.3-48-48-26-26-20.sgf");

            SGFTree gameTree = SgfReader.LoadFromStream(fileStream);
            baseGame = SgfCompiler.Compile(gameTree);
        }

        public static void CreateEndpoints(WebApplication app)
        {
            RouteGroupBuilder group = app.MapGroup("/api/JosekisService");
            group.MapGet("/AddSession/{sessionId}", (int sessionId, IJosekisService service) => service.AddSession(sessionId));
            group.MapGet("/RemoveSession/{sessionId}", (int sessionId, IJosekisService service) => service.RemoveSession(sessionId));
            group.MapGet("/Current/{sessionId}", (int sessionId, IJosekisService service) => service.Current(sessionId));
            group.MapGet("/ToParent/{sessionId}", (int sessionId, IJosekisService service) => service.ToParent(sessionId));
            group.MapGet("/ToLastBranch/{sessionId}", (int sessionId, IJosekisService service) => service.ToLastBranch(sessionId));
            group.MapGet("/ToFirst/{sessionId}", (int sessionId, IJosekisService service) => service.ToFirst(sessionId));
            group.MapPost("/ToChild/{sessionId}", (int sessionId, JosekisNode childToGo, IJosekisService service) => service.ToChild(sessionId, childToGo));
        }

        public async Task AddSession(int sessionId)
        {
            JosekisGoNodes[sessionId] = baseGame.RootNode;
        }

        public async Task RemoveSession(int sessionId)
        {
            JosekisGoNodes.Remove(sessionId);
        }

        public async Task<JosekisNode> Current(int sessionId)
        {
            GoNode node = JosekisGoNodes[sessionId];
            return node is GoMoveNode moveNode ? JosekisNodeConverter.Convert(moveNode) : JosekisNodeConverter.Convert(node);
        }

        public async Task ToParent(int sessionId)
        {
            GoNode node = JosekisGoNodes[sessionId];
            if (node.ParentNode == null)
            {
                return;
            }

            JosekisGoNodes[sessionId] = node.ParentNode;
        }

        public async Task<int> ToLastBranch(int sessionId)
        {
            GoNode node = JosekisGoNodes[sessionId];

            int returnCount = 0;

            do
            {
                if (node.ParentNode == null)
                {
                    break;
                }

                node = node.ParentNode;
                returnCount++;

            } while (node.ChildNodes.Count < 2);

            JosekisGoNodes[sessionId] = node;

            return returnCount;
        }

        public async Task ToFirst(int sessionId)
        {
            JosekisGoNodes[sessionId] = baseGame.RootNode;
        }

        public async Task<bool> ToChild(int sessionId, JosekisNode childToGo)
        {
            Sanitizer.Sanitize(childToGo);

            foreach (GoNode? childNode in JosekisGoNodes[sessionId].ChildNodes)
            {
                if (childNode is GoMoveNode childMove)
                {
                    if (childToGo.X == childMove.Stone.X && childToGo.Y == childMove.Stone.Y)
                    {
                        JosekisGoNodes[sessionId] = childMove;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
