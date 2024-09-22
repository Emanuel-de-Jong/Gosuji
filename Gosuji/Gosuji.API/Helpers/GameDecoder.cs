using Gosuji.API.Services.TrainerService;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;

namespace Gosuji.API.Helpers
{
    public class GameDecoder
    {
        private MoveTree tree = new();
        private int currentIndex = 0;
        private byte[] data;

        public MoveTree Decode(List<byte> data)
        {
            this.data = data.ToArray();
            //DecodeLoop(tree.RootNode);
            return tree;
        }
    }
}
