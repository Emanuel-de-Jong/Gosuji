﻿using Gosuji.Client;
using Gosuji.Client.Models;
using IGOEnchi.GoGameLogic;

namespace Gosuji.API.Models.GoGameExtensions
{
    public static class StoneExtensions
    {
        public static string Print(this Stone stone)
        {
            string output = G.ColorToName(Move.ColorFromIGOEnchi(stone.IsBlack));
            if (stone.X == 20)
            {
                output += "(PASS)";
            }
            else
            {
                output += "(" + stone.X + "," + stone.Y + ")";
            }

            return output;
        }
    }
}
