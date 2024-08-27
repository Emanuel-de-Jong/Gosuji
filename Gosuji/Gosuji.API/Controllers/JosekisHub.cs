using Gosuji.API.Helpers;
using Gosuji.API.Services;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models.Josekis;
using IGOEnchi.GoGameLogic;
using IGOEnchi.GoGameSgf;
using IGOEnchi.SmartGameLib;
using IGOEnchi.SmartGameLib.models;

namespace Gosuji.API.Controllers
{
    public class JosekisHub : CustomHubBase
    {
        private static readonly string SESSION_UNKNOWN_ERR = "SessionId unknown.";

        private static Dictionary<int, GoNode> josekisGoNodes = [];
        private static GoGame baseGame;
        private static Random random = new();

        private SanitizeService sanitizeService;

        public JosekisHub(SanitizeService _sanitizeService)
        {
            sanitizeService = _sanitizeService;

            if (baseGame == null)
            {
                using FileStream fileStream = System.IO.File.OpenRead(@"Resources\AI-Josekis-40-0.3-48-48-26-26-20.sgf");
                SGFTree gameTree = SgfReader.LoadFromStream(fileStream);
                baseGame = SgfCompiler.Compile(gameTree);
            }
        }

        public async Task<HubResponse> StartSession()
        {
            int sessionId = random.Next(100_000_000, 999_999_999);
            josekisGoNodes[sessionId] = baseGame.RootNode;
            return OkData(sessionId);
        }

        public async Task<HubResponse> StopSession(int sessionId)
        {
            josekisGoNodes.Remove(sessionId);
            return Ok;
        }

        public async Task<HubResponse> Current(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return BadRequest(SESSION_UNKNOWN_ERR);
            }

            GoNode node = josekisGoNodes[sessionId];
            JosekisNode result = node is GoMoveNode moveNode ? JosekisNodeConverter.Convert(moveNode) : JosekisNodeConverter.Convert(node);
            return OkData(result);
        }

        public async Task<HubResponse> ToParent(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return BadRequest(SESSION_UNKNOWN_ERR);
            }

            GoNode node = josekisGoNodes[sessionId];
            if (node.ParentNode == null)
            {
                return Ok;
            }

            josekisGoNodes[sessionId] = node.ParentNode;
            return Ok;
        }

        public async Task<HubResponse> ToLastBranch(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return BadRequest(SESSION_UNKNOWN_ERR);
            }

            GoNode node = josekisGoNodes[sessionId];

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

            josekisGoNodes[sessionId] = node;

            return OkData(returnCount);
        }

        public async Task<HubResponse> ToFirst(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return BadRequest(SESSION_UNKNOWN_ERR);
            }

            josekisGoNodes[sessionId] = baseGame.RootNode;
            return Ok;
        }

        public async Task<HubResponse> ToChild(int sessionId, JosekisNode childToGo)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return BadRequest(SESSION_UNKNOWN_ERR);
            }

            foreach (GoNode? childNode in josekisGoNodes[sessionId].ChildNodes)
            {
                if (childNode is GoMoveNode childMove)
                {
                    if (childToGo.X == childMove.Stone.X && childToGo.Y == childMove.Stone.Y)
                    {
                        josekisGoNodes[sessionId] = childMove;
                        return OkData(true);
                    }
                }
            }

            return OkData(false);
        }
    }
}
