using System;
using System.Collections.Generic;
using System.Linq;

class SmokeBasin
{
    private const string _filePath = "C:/Users/david/source/repos/advent-of-code-2021/input/day_9.txt";

    public static void Main(string[] args)
    {
        var input = File.ReadAllLines(_filePath)
            .Select(x => 
                x.Trim()
                 .ToCharArray()
                 .Select(y => Convert.ToInt32(char.GetNumericValue(y)))
                 .ToArray())
            .ToArray();

        int height = input.Count();
        int width = input.First().Count();

        var pointGrid = new Point[height, width];
        var points = new List<Point>(height * width);
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                var point  = new Point(x, y, input[x][y]);

                pointGrid[x, y] = point;
                points.Add(point);
            }
        }

        var simulationContext = new SimulationContext(pointGrid, points);

        // Part One
        int partOneResult = simulationContext.GetLowPoints().Sum(x => x.RiskLevel);

        Console.WriteLine(partOneResult);

        // Part Two
        var partTwoResult = simulationContext.GetBasins()
            .OrderByDescending(x => x.Points.Count())
            .Take(3)
            .Aggregate(1, (result, basin) => result * basin.Points.Count());

        Console.WriteLine(partTwoResult);
    }

    public class SimulationContext
    {
        public Point[,] PointGrid { get; set; }
        public IList<Point> Points { get; set; }

        public int GridHeight => PointGrid.GetLength(0);
        public int GridWidth => PointGrid.GetLength(1);

        public SimulationContext(Point[,] pointGrid, List<Point> points)
        {
            PointGrid = pointGrid;
            Points = points;
        }

        public IEnumerable<Point> GetLowPoints()
        {
            return Points.Where(p => GetSurroundingPoints(p).All(sp => sp.Height > p.Height));
        }

        public IEnumerable<Basin> GetBasins()
        {
            return Points.Select(p =>
                {
                    var basin = new Basin(this);
                    basin.AddPointToBasin(p);
                    return basin;
                })
                .Where(x => x.Points.Any());
        }

        public IEnumerable<Point> GetSurroundingPoints(Point point)
        {
            var surroundingPoints = new List<Point>();

            int previousX = point.X - 1;
            if (previousX >= 0)
            {
                surroundingPoints.Add(PointGrid[previousX, point.Y]);
            }

            int nextX = point.X + 1;
            if (nextX < GridWidth)
            {
                surroundingPoints.Add(PointGrid[nextX, point.Y]);
            }

            int previousY = point.Y - 1;
            if (previousY >= 0)
            {
                surroundingPoints.Add(PointGrid[point.X, previousY]);
            }

            int nextY = point.Y + 1;
            if (nextY < GridHeight)
            {
                surroundingPoints.Add(PointGrid[point.X, nextY]);
            }

            return surroundingPoints;
        }
    }

    public class Basin
    {
        private readonly SimulationContext _context;

        public Basin(SimulationContext context)
        {
            _context = context;
        }

        public IList<Point> Points { get; } = new List<Point>();

        public void AddPointToBasin(Point point)
        {
            if (point.Height == 9 || point.Processed)
            {
                return;
            }

            Points.Add(point);
            point.Processed = true;

            foreach (var surroundingPoint in _context.GetSurroundingPoints(point))
            {
                AddPointToBasin(surroundingPoint);
            }
        }
    }

    public class Point
    {
        public int X { get; }
        public int Y { get; }
        public int Height { get; }

        public int RiskLevel => Height + 1;
        public bool Processed { get; set; } = false;

        public Point(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
        }
    }
}