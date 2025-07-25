﻿using Gosuji.Client;
using Gosuji.Client.Data;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using System.Diagnostics;
using System.Text;

namespace Gosuji.API.Helpers
{
    public class KataGo
    {
        // Change numSearchThreads depending on backend als IsLowComputeHost.
        public static Dictionary<string, Dictionary<bool, int>> SEARCH_THREAD_DEFAULTS = new() {
            { "OpenCL", new() {
                { false, 20 },
                { true, 8 }
            }},
            { "TensorRT", new() {
                { false, 24 },
                { true, 12 }
            }}
        };

        public bool IsPaused { get; set; } = false;
        public int TotalVisits { get; set; } = 0;
        public DateTimeOffset LastStartTime { get; set; }

        public Process process;
        public StreamReader reader;
        public StreamReader errorReader;
        public StreamWriter writer;

        private bool isStopped = false;
        private int boardsize = 19;
        private int handicap = 0;

        private int lastMaxVisits;

        private void WaitForGTPReady()
        {
            string line;
            do
            {
                line = ReadError();
                Console.WriteLine(line);
            } while (!line.Contains("GTP ready"));
        }

        private void SetSearchThreads()
        {
            int searchThreads = SEARCH_THREAD_DEFAULTS[KataGoVersion.BACKEND][G.IsLowComputeHost];
            Write("kata-set-param numSearchThreads " + searchThreads);
            ClearReader();
        }

        public async Task Start()
        {
            if (isStopped)
            {
                return;
            }

            LastStartTime = DateTimeOffset.UtcNow;

            lastMaxVisits = 0;

            process = new Process();
            process.StartInfo.FileName = $"Resources/KataGo/{KataGoVersion.BACKEND}/katago.exe";
            process.StartInfo.Arguments = $"gtp -model Resources/KataGo/Models/{KataGoVersion.MODEL}.bin.gz";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;

            await Task.Run(() => process.Start());

            reader = process.StandardOutput;
            errorReader = process.StandardError;
            writer = process.StandardInput;

            await Task.Run(WaitForGTPReady);

            SetSearchThreads();
        }

        public void Stop()
        {
            isStopped = true;
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
            if (this.boardsize == boardsize)
            {
                return;
            }

            this.boardsize = boardsize;
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
            if (this.handicap == handicap)
            {
                return;
            }

            this.handicap = handicap;
            Write("fixed_handicap " + handicap);
            ClearReader();
        }

        public MoveSuggestionList ParseAnalysis(string[] analysis, EMoveColor color)
        {
            MoveSuggestionList suggestionList = new();
            MoveSuggestion? suggestion = null;
            for (int i = 0; i < analysis.Length; i++)
            {
                string element = analysis[i];
                if (element == "move")
                {
                    suggestion.Coord ??= Move.CoordFromKataGo(analysis[i + 1], boardsize);
                }
                else if (element == "visits")
                {
                    suggestion?.SetVisits(analysis[i + 1]);
                }
                else if (element == "winrate")
                {
                    suggestion?.SetWinrate(analysis[i + 1], color);
                }
                else if (element == "scoreLead")
                {
                    suggestion?.SetScoreLead(analysis[i + 1], color);
                }
                else if (element == "pv")
                {
                    bool isPassed = false;
                    while (analysis.Length - 1 >= i + 1 && analysis[i + 1] != "info")
                    {
                        if (!isPassed)
                        {
                            Coord coord = Move.CoordFromKataGo(analysis[i + 1], boardsize);
                            if (Move.IsPass(coord))
                            {
                                isPassed = true;
                            }
                            else
                            {
                                suggestion?.Continuation.Add(coord);

                                if (suggestion?.Continuation.Count >= MoveSuggestion.MAX_CONTINUATION_SIZE)
                                {
                                    break;
                                }
                            }
                        }

                        i++;
                    }
                }

                if (element == "info" || i == analysis.Length - 1)
                {
                    if (suggestion != null)
                    {
                        if (!suggestionList.Add(suggestion))
                        {
                            break;
                        }
                    }
                    suggestion = new MoveSuggestion();
                }
            }

            return suggestionList;
        }

        public MoveSuggestion AnalyzeMove(Move move)
        {
            int maxVisits = 100;
            if (lastMaxVisits != maxVisits)
            {
                lastMaxVisits = maxVisits;
                Write("kata-set-param maxVisits " + maxVisits);
                ClearReader();
            }

            KataGoMove kataGoMove = move.ToKataGo(boardsize);
            Write("kata-genmove_analyze " + kataGoMove.Color + " allow " + kataGoMove.Color + " " + kataGoMove.Coord + " 1");
            TotalVisits += maxVisits;

            Read(); // Ignore '= '
            string[] analysis = Read().Split(" ");
            ClearReader();

            Write("undo");
            ClearReader();

            MoveSuggestion suggestion = ParseAnalysis(analysis, move.Color.Value).Suggestions.FirstOrDefault();

            return suggestion;
        }

        public MoveSuggestionList Analyze(EMoveColor color, int maxVisits, double minVisitsPerc, double maxVisitDiffPerc, int moveOptions)
        {
            if (lastMaxVisits != maxVisits)
            {
                lastMaxVisits = maxVisits;
                Write("kata-set-param maxVisits " + maxVisits);
                ClearReader();
            }

            Write("kata-genmove_analyze " + Move.ColorToKataGo(color));
            TotalVisits += maxVisits;

            Read(); // Ignore '= '
            string[] analysis = Read().Split(" ");
            ClearReader();

            Write("undo");
            ClearReader();

            MoveSuggestionList suggestionList = ParseAnalysis(analysis, color);
            suggestionList.Visits = maxVisits;

            suggestionList.Filter(color, minVisitsPerc, maxVisitDiffPerc, moveOptions);

            return suggestionList;
        }

        public void Play(Move move)
        {
            Write("play " + move.ColorToKataGo() + " " + move.CoordToKataGo(boardsize));
            ClearReader();
        }

        public void PlayRange(Move[] moves)
        {
            foreach (Move move in moves)
            {
                Play(move);
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
