using IGOEnchi.GoGameLogic;
using IGOEnchi.GoGameSgf;
using IGOEnchi.SmartGameLib;
using System.IO;

namespace GosujiServer.Services
{
    public class JosekiService
    {
        private GoGame game;

        public JosekiService()
        {
            using var fileStream = File.OpenRead(@"Resources\AI-Josekis-True-0.3-15-15-8-8-6.sgf");

            var gameTree = SgfReader.LoadFromStream(fileStream);
            game = SgfCompiler.Compile(gameTree);
        }
    }
}
