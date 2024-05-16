using GosujiServer.Models;
using IGOEnchi.GoGameLogic;
using IGOEnchi.GoGameSgf;
using IGOEnchi.SmartGameLib;

namespace GosujiServer.Services
{
    public class JosekisService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private Dictionary<int, GoNode> JosekisGoNodes;

        private GoGame baseGame;

        public JosekisService(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;

            JosekisGoNodes = new();

            using var fileStream = File.OpenRead(@"Resources\AI-Josekis-True-0.3-15-15-8-8-6.sgf");

            var gameTree = SgfReader.LoadFromStream(fileStream);
            baseGame = SgfCompiler.Compile(gameTree);
        }

        public void AddSession(int sessionId)
        {
            JosekisGoNodes[sessionId] = baseGame.RootNode;
        }

        public void RemoveSession(int sessionId)
        {
            JosekisGoNodes.Remove(sessionId);
        }

        public JosekisNode Current(int sessionId)
        {
            GoNode node = JosekisGoNodes[sessionId];
            if (node is GoMoveNode)
            {
                return new JosekisNode((GoMoveNode)node);
            }

            return new JosekisNode(node);
        }

        public List<JosekisNode> Children(int sessionId)
        {
            List<JosekisNode> children = new();

            foreach (var childNode in JosekisGoNodes[sessionId].ChildNodes)
            {
                if (childNode is GoMoveNode)
                {
                    children.Add(new JosekisNode((GoMoveNode)childNode));
                }
            }

            return children;
        }

        public void ToParent(int sessionId)
        {
            GoNode node = JosekisGoNodes[sessionId];
            if (node.ParentNode == null)
            {
                return;
            }

            JosekisGoNodes[sessionId] = node.ParentNode;
        }

        public bool ToChild(int sessionId, JosekisNode childToGo)
        {
            foreach (var childNode in JosekisGoNodes[sessionId].ChildNodes)
            {
                if (childNode is GoMoveNode)
                {
                    GoMoveNode childMove = (GoMoveNode)childNode;
                    if (childToGo.Compare(childMove))
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
