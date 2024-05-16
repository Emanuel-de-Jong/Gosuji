using GosujiServer.Models;
using IGOEnchi.GoGameLogic;
using IGOEnchi.GoGameSgf;
using IGOEnchi.SmartGameLib;

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

        public JosekisNode? Current()
        {
            GoNode node = GetSessionNode();
            if (node is GoMoveNode)
            {
                return new JosekisNode((GoMoveNode)node);
            }

            return null;
        }

        public List<JosekisNode> Children()
        {
            List<JosekisNode> children = new();

            foreach (var childNode in GetSessionNode().ChildNodes)
            {
                if (childNode is GoMoveNode)
                {
                    children.Add(new JosekisNode((GoMoveNode)childNode));
                }
            }

            return children;
        }

        public void ToParent()
        {
            GoNode node = GetSessionNode();
            if (node.ParentNode == null)
            {
                return;
            }

            JosekiNodes[GetSessionId()] = node.ParentNode;
        }

        public bool ToChild(JosekisNode childToGo)
        {
            foreach (var childNode in GetSessionNode().ChildNodes)
            {
                if (childNode is GoMoveNode)
                {
                    GoMoveNode childMove = (GoMoveNode)childNode;
                    if (childToGo.Compare(childMove))
                    {
                        JosekiNodes[GetSessionId()] = childMove;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
