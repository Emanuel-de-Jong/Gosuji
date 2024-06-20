using Gosuji.Client.Models.Josekis;
using Gosuji.Client.Services;
using Gosuji.Helpers;
using Gosuji.Services;
using IGOEnchi.GoGameLogic;
using IGOEnchi.GoGameSgf;
using IGOEnchi.SmartGameLib;
using IGOEnchi.SmartGameLib.models;
using Microsoft.AspNetCore.Mvc;

namespace Gosuji.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class JosekisController : ControllerBase
    {
        private SanitizeService sanitizeService;

        private static Dictionary<int, GoNode> josekisGoNodes = [];
        private static GoGame baseGame;

        public JosekisController(SanitizeService _sanitizeService)
        {
            sanitizeService = _sanitizeService;

            if (baseGame == null)
            {
                using FileStream fileStream = System.IO.File.OpenRead(@"Resources\AI-Josekis-40-0.3-48-48-26-26-20.sgf");
                SGFTree gameTree = SgfReader.LoadFromStream(fileStream);
                baseGame = SgfCompiler.Compile(gameTree);
            }
        }

        [HttpGet("{sessionId}")]
        public async Task AddSession(int sessionId)
        {
            josekisGoNodes[sessionId] = baseGame.RootNode;
        }

        [HttpGet("{sessionId}")]
        public async Task RemoveSession(int sessionId)
        {
            josekisGoNodes.Remove(sessionId);
        }

        [HttpGet("{sessionId}")]
        public async Task<JosekisNode> Current(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return null;
            }

            GoNode node = josekisGoNodes[sessionId];
            return node is GoMoveNode moveNode ? JosekisNodeConverter.Convert(moveNode) : JosekisNodeConverter.Convert(node);
        }

        [HttpGet("{sessionId}")]
        public async Task ToParent(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return;
            }

            GoNode node = josekisGoNodes[sessionId];
            if (node.ParentNode == null)
            {
                return;
            }

            josekisGoNodes[sessionId] = node.ParentNode;
        }

        [HttpGet("{sessionId}")]
        public async Task<int> ToLastBranch(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return 0;
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

            return returnCount;
        }

        [HttpGet("{sessionId}")]
        public async Task ToFirst(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return;
            }

            josekisGoNodes[sessionId] = baseGame.RootNode;
        }

        [HttpPost("{sessionId}")]
        public async Task<bool> ToChild(int sessionId, JosekisNode childToGo)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return false;
            }

            sanitizeService.Sanitize(childToGo);

            foreach (GoNode? childNode in josekisGoNodes[sessionId].ChildNodes)
            {
                if (childNode is GoMoveNode childMove)
                {
                    if (childToGo.X == childMove.Stone.X && childToGo.Y == childMove.Stone.Y)
                    {
                        josekisGoNodes[sessionId] = childMove;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
