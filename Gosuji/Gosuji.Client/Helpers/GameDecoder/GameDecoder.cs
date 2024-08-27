using Gosuji.Client.Models;

namespace Gosuji.Client.Helpers.GameDecoder
{
    public class GameDecoder
    {
        public static short EncodeYIndicator = -1;
        public static short SuggestionsEncodeAnalyzeMoveIndicator = -2;


        private static Dictionary<short, Dictionary<short, T>> Decode<T>(byte[] bytes, Func<int, (T, int)> dataDecoder)
        {
            int i = 0;
            short y = 0;
            Dictionary<short, Dictionary<short, T>> result = [];
            while (i < bytes.Length)
            {
                short x = GetInt16(bytes, i);
                i += 2;

                if (x == EncodeYIndicator)
                {
                    y = GetInt16(bytes, i);
                    i += 2;

                    result[y] = [];
                }
                else
                {
                    (T, int) data = dataDecoder(i);
                    i = data.Item2;
                    result[y][x] = data.Item1;
                }
            }

            return result;
        }

        public static PlayerResultTree DecodePlayerResults(byte[] bytes)
        {
            try
            {
                PlayerResultTree rootNode = new();

                int i = 0;
                short y = 0;
                PlayerResultTree node = rootNode;
                while (i < bytes.Length)
                {
                    short x = GetInt16(bytes, i);
                    i += 2;

                    if (x == EncodeYIndicator)
                    {
                        y = GetInt16(bytes, i);
                        i += 2;

                        short nodeY = GetInt16(bytes, i);
                        i += 2;

                        short nodeX = GetInt16(bytes, i);
                        i += 2;

                        node = rootNode.Nodes[nodeY][nodeX];
                    }
                    else
                    {
                        node = node.Add((EPlayerResult)bytes[i++], x, y);
                    }
                }

                return rootNode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new();
            }
        }

        public static Dictionary<short, Dictionary<short, SuggestionList>> DecodeSuggestions(byte[] bytes)
        {
            try
            {
                return Decode(bytes, (i) =>
                {
                    int suggestionLength = GetInt16(bytes, i);
                    i += 2;

                    SuggestionList suggestionList = new();

                    (Suggestion, int) data;
                    if (suggestionLength == SuggestionsEncodeAnalyzeMoveIndicator)
                    {
                        data = DecodeSuggestion(bytes, i);
                        i = data.Item2;
                        suggestionList.AnalyzeMoveSuggestion = data.Item1;

                        suggestionLength = GetInt16(bytes, i);
                        i += 2;
                    }

                    for (int j = 0; j < suggestionLength; j++)
                    {
                        data = DecodeSuggestion(bytes, i);
                        i = data.Item2;
                        suggestionList.Add(data.Item1);
                    }

                    return (suggestionList, i);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return [];
            }
        }

        private static (Suggestion, int) DecodeSuggestion(byte[] bytes, int i)
        {
            int coordX = bytes[i++];
            int coordY = bytes[i++];

            int visits = GetInt32(bytes, i);
            i += 4;

            int winrate = GetInt32(bytes, i);
            i += 4;
            int scoreLead = GetInt32(bytes, i);
            i += 4;

            Suggestion suggestion = new()
            {
                Coord = new Coord() { X = coordX, Y = coordY },
                Visits = visits,
                Score = new Score() { Winrate = winrate, ScoreLead = scoreLead },
            };

            return (suggestion, i);
        }

        public static Dictionary<short, Dictionary<short, EMoveType>> DecodeMoveTypes(byte[] bytes)
        {
            try
            {
                return Decode(bytes, (i) =>
                {
                    EMoveType moveType = (EMoveType)bytes[i++];
                    return (moveType, i);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return [];
            }
        }

        public static Dictionary<short, Dictionary<short, Coord>> DecodeChosenNotPlayedCoords(byte[] bytes)
        {
            try
            {
                return Decode(bytes, (i) =>
                {
                    int coordX = bytes[i++];
                    int coordY = bytes[i++];

                    return (new Coord() { X = coordX, Y = coordY }, i);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return [];
            }
        }


        private static short GetInt16(byte[] bytes, int i)
        {
            byte[] tempBytes = new byte[] { bytes[i], bytes[++i] };
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(tempBytes);
            }
            return BitConverter.ToInt16(tempBytes, 0);
        }

        private static int GetInt32(byte[] bytes, int i)
        {
            byte[] tempBytes = new byte[] { bytes[i], bytes[++i], bytes[++i], bytes[++i] };
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(tempBytes);
            }
            return BitConverter.ToInt32(tempBytes, 0);
        }
    }
}
