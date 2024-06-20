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
        private static readonly string SESSION_UNKNOWN_ERR = "SessionId unknown.";

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
        public async Task<ActionResult> AddSession(int sessionId)
        {
            josekisGoNodes[sessionId] = baseGame.RootNode;
            return Ok();
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult> RemoveSession(int sessionId)
        {
            josekisGoNodes.Remove(sessionId);
            return Ok();
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult<JosekisNode>> Current(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return BadRequest(SESSION_UNKNOWN_ERR);
            }

            GoNode node = josekisGoNodes[sessionId];
            JosekisNode result = node is GoMoveNode moveNode ? JosekisNodeConverter.Convert(moveNode) : JosekisNodeConverter.Convert(node);
            return Ok(result);
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult> ToParent(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return BadRequest(SESSION_UNKNOWN_ERR);
            }

            GoNode node = josekisGoNodes[sessionId];
            if (node.ParentNode == null)
            {
                return Ok();
            }

            josekisGoNodes[sessionId] = node.ParentNode;
            return Ok();
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult<int>> ToLastBranch(int sessionId)
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

            return Ok(returnCount);
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult> ToFirst(int sessionId)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return BadRequest(SESSION_UNKNOWN_ERR);
            }

            josekisGoNodes[sessionId] = baseGame.RootNode;
            return Ok();
        }

        [HttpPost("{sessionId}")]
        public async Task<ActionResult<bool>> ToChild(int sessionId, JosekisNode childToGo)
        {
            if (!josekisGoNodes.ContainsKey(sessionId))
            {
                return BadRequest(SESSION_UNKNOWN_ERR);
            }

            sanitizeService.Sanitize(childToGo);

            foreach (GoNode? childNode in josekisGoNodes[sessionId].ChildNodes)
            {
                if (childNode is GoMoveNode childMove)
                {
                    if (childToGo.X == childMove.Stone.X && childToGo.Y == childMove.Stone.Y)
                    {
                        josekisGoNodes[sessionId] = childMove;
                        return Ok(true);
                    }
                }
            }

            return Ok(false);
        }
    }
}
