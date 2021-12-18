using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Chiton
{
    private const string _filePath = "C:/Users/david/source/repos/advent-of-code-2021/input/day_15.txt";

    public static void Main(string[] args)
    {
        // Part One
        Node[][] nodeGrid = File.ReadAllLines(_filePath)
            .Select((row, x) => row.Trim()
                .Select((risk, y) => 
                    new Node
                    {
                        X = x,
                        Y = y,
                        Risk = (int)Char.GetNumericValue(risk)
                    })
                .ToArray())
            .ToArray();

        var context = new SimulationContext(nodeGrid);

        int partOneResult = context.GetLowestRiskPath();
        Console.WriteLine(partOneResult);

        // Part Two
        int originalHeight = nodeGrid.Length;
        int originalWidth = nodeGrid[0].Length;

        int newHeight = originalHeight * 5;
        int newWidth = originalWidth * 5;

        Node[][] expandedNodeGrid = new Node[newHeight][];

        for (int x = 0; x < newHeight; x++)
        {
            expandedNodeGrid[x] = new Node[newWidth];
            for (int y = 0; y < newWidth; y++)
            {
                Node originalNode = nodeGrid[x % originalHeight][y % originalWidth];

                int delta = (x / originalHeight) + (y / originalWidth);
                int newRisk = ((originalNode.Risk + delta - 1) % 9) + 1;

                expandedNodeGrid[x][y] = new Node
                {
                    X = x,
                    Y = y,
                    Risk = newRisk
                };
            }
        }

        context = new SimulationContext(expandedNodeGrid);

        int partTwoResult = context.GetLowestRiskPath();
        Console.WriteLine(partTwoResult);
    }

    public class SimulationContext
    {
        public IList<Node> Nodes { get; set; }
        public Node[][] NodeGrid { get; set; }

        public int Height => NodeGrid.Length;
        public int Width => NodeGrid.First().Length;

        public SimulationContext(Node[][] nodeGrid)
        {
            NodeGrid = nodeGrid;
            Nodes = NodeGrid.SelectMany(x => x).ToList();
        }

        // Find lowest risk path with Dijkstra's algorithm.
        public int GetLowestRiskPath()
        {
            var queue = new PriorityQueue<Node, int>();

            Node startingNode = NodeGrid[0][0];
            Node endingNode = NodeGrid[Height - 1][Width - 1];

            queue.Enqueue(startingNode, 0);

            var lowestRiskMap = Nodes.ToDictionary(x => x, y => int.MaxValue);
            lowestRiskMap[startingNode] = 0;

            while (queue.Count > 0)
            {
                Node currentNode = queue.Dequeue();
                currentNode.Visited = true;

                foreach (Node neighbor in GetAdjacentNodes(currentNode))
                {
                    int oldRisk = lowestRiskMap[neighbor];
                    int newRisk = lowestRiskMap[currentNode] + neighbor.Risk;

                    if (newRisk < oldRisk)
                    {
                        queue.Enqueue(neighbor, newRisk);
                        lowestRiskMap[neighbor] = newRisk;
                    }
                }
            }

            return lowestRiskMap[endingNode];
        }

        private IEnumerable<Node> GetAdjacentNodes(Node node)
        {
            var adjacentNodes = new List<Node>();

            for (int xShift = -1; xShift <= 1; xShift++)
            {
                for (int yShift = -1; yShift <= 1; yShift++)
                {
                    if ((xShift == 0 && yShift == 0)
                        || (xShift != 0 && yShift != 0))
                    {
                        continue;
                    }

                    int newX = node.X + xShift;
                    int newY = node.Y + yShift;

                    if ((newX >= 0 && newX < Height) &&
                        (newY >= 0 && newY < Width))
                    {
                        adjacentNodes.Add(NodeGrid[newX][newY]);
                    }
                }
            }

            return adjacentNodes;
        }
    }

    public class Node
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Risk { get; init; }
        public bool Visited { get; set; }
    }
}