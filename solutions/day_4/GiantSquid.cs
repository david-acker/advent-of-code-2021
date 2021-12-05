using System.Collections.Generic;
using System.IO;
using System.Linq;

class GiantSquidBingo
{
    private const string _inputPath = "C:/Users/david/source/repos/advent-of-code-2021/input/day_4.txt";

    public static void Main(string[] args)
    {
        var input = File.ReadAllText(_inputPath)
            .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var numbers = input.First().Split(',').Select(int.Parse).ToList();

        // Part One
        var partOneBoards = input.Skip(1).Select(x => new Board(x)).ToList();
        int winningBoardScore = GetFirstWinningBoardScore(partOneBoards, numbers);
        Console.WriteLine($"Score of first winning board: {winningBoardScore}");

        // Part Two
        var partTwoBoards = input.Skip(1).Select(x => new Board(x)).ToList();
        int lastWinningBoardScore = GetLastWinningBoardScore(partTwoBoards, numbers);
        Console.WriteLine($"Score of last winning board: {lastWinningBoardScore}");
    }

    private static int GetFirstWinningBoardScore(List<Board> boards, List<int> numbers)
    {
        foreach (int number in numbers)
        {
            var winningBoard = boards.Where(b => b.Mark(number)).FirstOrDefault(b => b.IsWinner);

            if (winningBoard != null)
            {
                return winningBoard.Score;
            }
        }

        return default;
    }

    private static int GetLastWinningBoardScore(List<Board> boards, List<int> numbers)
    {
        foreach (int number in numbers)
        {
            var winningBoardsForNumber = boards.Where(b => b.Mark(number) && b.IsWinner);
        
            if (boards.Count() == 1 && winningBoardsForNumber.Any())
            {
                return winningBoardsForNumber.Single().Score;
            }

            boards.RemoveAll(b => winningBoardsForNumber.Contains(b));
        }

        return default;
    }

    public class Board
    {
        public class BoardNumber
        {
            public int Number { get; set; }
            public bool IsMarked { get; set; }

            public BoardNumber(int number)
            {
                Number = number;
            }
        }

        private BoardNumber[][] _rows { get; set; } = new BoardNumber[5][];
        private BoardNumber[][] _columns { get; set; } = new BoardNumber[5][];
        private List<BoardNumber> _numbers { get; set; } = new List<BoardNumber>(25);

        public bool IsWinner => _rows.Any(row => row.All(num => num.IsMarked))
            || _columns.Any(col => col.All(num => num.IsMarked));

        private int _lastNumberMarked { get; set; } = 0;
        public int Score => _numbers.Where(num => !num.IsMarked).Sum(num => num.Number) * _lastNumberMarked;

        public Board(string input)
        {
            _rows = input.Split(Environment.NewLine)
                .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Select(x => x.Trim()).Select(int.Parse).Select(x => new BoardNumber(x)).ToArray())
                .ToArray();

            // Transpose board number rows in columns
            for (int x = 0; x < 5; x++)
            {
                _columns[x] = new BoardNumber[5];

                for (int y = 0; y < 5; y++)
                {
                    _columns[x][y] = _rows[y][x];
                }
            }

            _numbers = _rows.SelectMany(x => x).ToList();
        }

        public bool Mark(int number)
        {
            var boardNumber = _numbers.FirstOrDefault(x => x.Number == number);
            if (boardNumber != null)
            {
                _lastNumberMarked = number;
                boardNumber.IsMarked = true;
                
                return true;
            }
            return false;
        }
    }
}