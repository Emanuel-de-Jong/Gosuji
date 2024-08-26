using Gosuji.Client.Data;
using Gosuji.Client.Models.KataGo;
using System.Diagnostics;
using System.Text;

namespace Gosuji.API.Helpers
{
    public class KataGo
    {
        public int TotalVisits { get; set; } = 0;
        public DateTimeOffset LastStartTime { get; set; }

        public Process process;
        public StreamReader reader;
        public StreamReader errorReader;
        public StreamWriter writer;

        private bool stopped = false;

        private int lastMaxVisits;

        public async Task Start()
        {
            if (stopped)
            {
                return;
            }

            LastStartTime = DateTimeOffset.UtcNow;

            lastMaxVisits = 0;

            process = new Process();
            process.StartInfo.FileName = @"Resources\KataGo\katago.exe";
            process.StartInfo.Arguments = @"gtp -model Resources\KataGo\" + KataGoVersion.MODEL + ".bin.gz";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;

            await Task.Run(() => process.Start());

            reader = process.StandardOutput;
            errorReader = process.StandardError;
            writer = process.StandardInput;

            await Task.Run(WaitForGTPReady);
        }

        private void WaitForGTPReady()
        {
            string line;
            do
            {
                line = ReadError();
                Console.WriteLine(line);
            } while (!line.Contains("GTP ready"));
        }

        public void Stop()
        {
            stopped = true;
            process.Dispose();
        }

        public void ClearBoard()
        {
            Write("clear_board");
            ClearReader();
            Write("clear_cache");
            ClearReader();
        }

        public async Task Restart()
        {
            if (process != null)
            {
                Write("quit");
            }

            await Start();
        }

        public void SetBoardsize(int boardsize)
        {
            Write("boardsize " + boardsize);
            ClearReader();
        }

        public void SetRuleset(string ruleset)
        {
            Write("kata-set-rules " + ruleset);
            ClearReader();
        }

        public void SetKomi(double komi)
        {
            Write("komi " + komi);
            ClearReader();
        }

        public void SetHandicap(int handicap)
        {
            Write("fixed_handicap " + handicap);
            ClearReader();
        }

        public MoveSuggestion AnalyzeMove(string color, string coord)
        {
            int maxVisits = 100;
            if (lastMaxVisits != maxVisits)
            {
                lastMaxVisits = maxVisits;
                Write("kata-set-param maxVisits " + maxVisits);
                ClearReader();
            }

            Write("kata-genmove_analyze " + color + " allow " + color + " " + coord + " 1");
            TotalVisits += maxVisits;

            Read(); // Ignore '= '
            string[] analysis = Read().Split(" ");
            ClearReader();

            Write("undo");
            ClearReader();

            MoveSuggestion suggestion = new();
            suggestion.SetMove(color, coord);
            for (int i = 0; i < analysis.Length; i++)
            {
                string element = analysis[i];
                if (element == "visits")
                {
                    suggestion?.SetVisits(analysis[i + 1]);
                }
                else if (element == "winrate")
                {
                    suggestion?.SetWinrate(analysis[i + 1]);
                }
                else if (element == "scoreLead")
                {
                    suggestion?.SetScoreLead(analysis[i + 1]);
                }
            }

            return suggestion;
        }

        public List<MoveSuggestion> Analyze(string color, int maxVisits, double minVisitsPerc, double maxVisitDiffPerc)
        {
            if (lastMaxVisits != maxVisits)
            {
                lastMaxVisits = maxVisits;
                Write("kata-set-param maxVisits " + maxVisits);
                ClearReader();
            }

            Write("kata-genmove_analyze " + color);
            TotalVisits += maxVisits;

            Read(); // Ignore '= '
            string[] analysis = Read().Split(" ");
            ClearReader();

            Write("undo");
            ClearReader();

            List<MoveSuggestion> suggestions = [];
            MoveSuggestion? suggestion = null;
            for (int i = 0; i < analysis.Length; i++)
            {
                string element = analysis[i];
                if (element == "move")
                {
                    suggestion?.SetMove(color, analysis[i + 1]);
                }
                else if (element == "visits")
                {
                    suggestion?.SetVisits(analysis[i + 1]);
                }
                else if (element == "winrate")
                {
                    suggestion?.SetWinrate(analysis[i + 1]);
                }
                else if (element == "scoreLead")
                {
                    suggestion?.SetScoreLead(analysis[i + 1]);
                }

                if (element == "info" || i == analysis.Length - 1)
                {
                    if (suggestion != null)
                    {
                        suggestions.Add(suggestion);
                    }
                    suggestion = new MoveSuggestion();
                }
            }

            int highestVisits = 0;
            foreach (MoveSuggestion moveSuggestion in suggestions)
            {
                if (highestVisits < moveSuggestion.visits)
                {
                    highestVisits = moveSuggestion.visits;
                }
            }
            int maxVisitDiff = (int)Math.Round(maxVisitDiffPerc / 100.0 * Math.Max(maxVisits, highestVisits));
            int minVisits = (int)Math.Round(minVisitsPerc / 100.0 * maxVisits);

            List<MoveSuggestion> filteredSuggestions = [];
            int lastSuggestionVisits = int.MaxValue;
            foreach (MoveSuggestion moveSuggestion in suggestions)
            {
                if (filteredSuggestions.Count > 0 &&
                        filteredSuggestions[^1].move.coord != "pass" &&
                        (moveSuggestion.visits < minVisits ||
                        lastSuggestionVisits - moveSuggestion.visits > maxVisitDiff))
                {
                    break;
                }
                filteredSuggestions.Add(moveSuggestion);
                if (lastSuggestionVisits > moveSuggestion.visits)
                {
                    lastSuggestionVisits = moveSuggestion.visits;
                }
            }

            return filteredSuggestions;
        }

        public void Play(string color, string coord)
        {
            Write("play " + color + " " + coord);
            ClearReader();
        }

        public void PlayRange(Moves moves)
        {
            foreach (Move move in moves.moves)
            {
                Play(move.color, move.coord);
            }
        }

        public string SGF(bool shouldWriteFile)
        {
            Write("printsgf");
            string sgfStr = Read()[2..];
            ClearReader();

            if (shouldWriteFile)
            {
                StreamWriter sgfWriter = new(File.Create("SGFs\\" +
                    DateTime.Now.ToString("dd-MM_HH-mm-ss") +
                    ".sgf"));
                sgfWriter.Write(sgfStr);
                sgfWriter.Close();
            }

            return sgfStr;
        }

        private string ShowBoard()
        {
            Write("showboard");
            StringBuilder stringBuilder = new();
            string line = "";
            while ((line = Read()) != "")
            {
                stringBuilder.AppendLine(line);
            }
            return stringBuilder.ToString();
        }

        private string Read()
        {
            string? line;
            while ((line = reader.ReadLine()) == null) { }
            return line;
        }

        private string ReadError()
        {
            string? line;
            while ((line = errorReader.ReadLine()) == null) { }
            return line;
        }

        private void ClearReader()
        {
            while (reader.ReadLine() != "") { }
        }

        private void Write(string command)
        {
            writer.WriteLine(command);
            writer.Flush();
        }
    }
}
