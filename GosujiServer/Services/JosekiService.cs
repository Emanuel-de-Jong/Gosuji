using GosujiServer.Data;
using IGOEnchi.GoGameLogic;
using IGOEnchi.GoGameSgf;
using IGOEnchi.SmartGameLib;
using System.IO;

namespace GosujiServer.Services
{
    public class JosekiService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private Dictionary<string, GoNode> JosekiNodes;

        private GoGame baseGame;

        public JosekiService(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;

            JosekiNodes = new();

            using var fileStream = File.OpenRead(@"Resources\AI-Josekis-True-0.3-15-15-8-8-6.sgf");

            var gameTree = SgfReader.LoadFromStream(fileStream);
            baseGame = SgfCompiler.Compile(gameTree);
        }

        private string GetSessionId()
        {
            return httpContextAccessor.HttpContext.Session.Id;
        }

        private GoNode GetSessionNode()
        {
            return JosekiNodes[GetSessionId()];
        }

        public void AddSession()
        {
            JosekiNodes[GetSessionId()] = baseGame.RootNode;
        }

        public void RemoveSession()
        {
            JosekiNodes.Remove(GetSessionId());
        }

        public Stone? CurrentStone()
        {
            GoNode node = GetSessionNode();
            if (node is GoMoveNode)
            {
                return ((GoMoveNode)node).Stone;
            }

            return null;
        }

        public List<Stone> ChildStones()
        {
            List<Stone> childStones = new();

            foreach (var childNode in GetSessionNode().ChildNodes)
            {
                if (childNode is GoMoveNode)
                {
                    childStones.Add(((GoMoveNode)childNode).Stone);
                }
            }

            return childStones;
        }

        public void ToChild(Stone childStoneToGo)
        {
            foreach (var childNode in GetSessionNode().ChildNodes)
            {
                if (childNode is GoMoveNode)
                {
                    GoMoveNode childMove = (GoMoveNode)childNode;
                    if (childMove.Stone.AtPlaceOf(childStoneToGo))
                    {
                        JosekiNodes[GetSessionId()] = childMove;
                        break;
                    }
                }
            }
        }
    }
}
