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

            using var fileStream = File.OpenRead(@"Resources\AI-Josekis-40-0.3-48-48-26-26-20.sgf");

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

        public int ToLastBranch(int sessionId)
        {
            GoNode node = JosekisGoNodes[sessionId];

            int returnCount = 0;

            do {
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

        public void ToFirst(int sessionId)
        {
            JosekisGoNodes[sessionId] = baseGame.RootNode;
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
