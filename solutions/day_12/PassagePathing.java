import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;

public class PassagePathing {
    private static final String fileName = "C:/Users/david/source/repos/advent-of-code-2021/input/day_12.txt";

    public static void main(String[] args) {
        List<String> lines = new ArrayList<>();
        try {
            lines = Files.readAllLines(Paths.get(fileName), StandardCharsets.UTF_8);
        }
        catch (Exception ex) {
            Logger.getAnonymousLogger().log(Level.SEVERE, "", ex);
            System.exit(-1);
        }

        HashMap<String, List<String>> caveConnectionMap = GetCaveConnectionMap(lines);
        SimulationContext context = new PassagePathing.SimulationContext(caveConnectionMap);

        // Part One
        int partOneResult = context.GetTotalPathCount(false);
        System.out.println(partOneResult);

        // Part Two
        int partTwoResult = context.GetTotalPathCount(true);
        System.out.println(partTwoResult);
    }

    private static HashMap<String, List<String>> GetCaveConnectionMap(List<String> lines) {
        HashMap<String, List<String>> caveConnectionMap = new HashMap<>();

        for (String line : lines) {
            String[] caveConnection = line.trim().split("-");
            AddCaveConnection(caveConnectionMap, caveConnection[0], caveConnection[1]);
            AddCaveConnection(caveConnectionMap, caveConnection[1], caveConnection[0]);
        }

        return caveConnectionMap;
    }

    private static void AddCaveConnection(HashMap<String, List<String>> caveConnectionMap, String startingCave, String endingCave)
    {
        if (!caveConnectionMap.containsKey(startingCave)) {
            caveConnectionMap.put(startingCave, new ArrayList<String>());
        }

        caveConnectionMap.get(startingCave).add(endingCave);
    }

    private static class SimulationContext {
        private static final String startingCave = "start";
        private static final String endingCave = "end";

        private HashMap<String, List<String>> caveConnectionMap = new HashMap<>();

        public SimulationContext(HashMap<String, List<String>> caveConnectionMap) {
            this.caveConnectionMap = caveConnectionMap;
        }

        public int GetTotalPathCount(boolean canVisitSmallCaveTwice) {
            HashSet<String> visitedCaves = new HashSet<>(Arrays.asList(startingCave));

            return canVisitSmallCaveTwice
                ? GetTotalPathCountForCaveForPartTwo(startingCave, visitedCaves, false)
                : GetTotalPathCountForCaveForPartOne(startingCave, visitedCaves);
        }

        private int GetTotalPathCountForCaveForPartOne(String cave, HashSet<String> visitedCaves) {
            if (cave.equals(endingCave)) {
                return 1;
            }

            int totalPaths = 0;
            for (String nextCave : caveConnectionMap.get(cave)) {
                boolean wasVisited = visitedCaves.contains(nextCave);

                if (!wasVisited || IsBigCave(nextCave)) {
                    totalPaths += GetTotalPathCountForCaveForPartOne(nextCave,
                        GetUpdatedVisitedCaves(visitedCaves, nextCave));
                }
            }

            return totalPaths;
        }

        private int GetTotalPathCountForCaveForPartTwo(String cave, HashSet<String> visitedCaves, boolean hasVisitedSmallCaveTwice) {
            if (cave.equals(endingCave)) {
                return 1;
            }

            int totalPaths = 0;
            for (String nextCave : caveConnectionMap.get(cave)) {
                boolean wasVisited = visitedCaves.contains(nextCave);

                boolean isBigCave = IsBigCave(nextCave);
                if (!wasVisited || isBigCave) {
                    totalPaths += GetTotalPathCountForCaveForPartTwo(nextCave,
                        GetUpdatedVisitedCaves(visitedCaves, nextCave),
                        hasVisitedSmallCaveTwice);
                } else if (!isBigCave && !nextCave.equals(startingCave) && !hasVisitedSmallCaveTwice) {
                    totalPaths += GetTotalPathCountForCaveForPartTwo(nextCave, visitedCaves, true);
                }
            }

            return totalPaths;
        }

        private boolean IsBigCave(String cave) {
            return cave.toUpperCase() == cave;
        }

        private HashSet<String> GetUpdatedVisitedCaves(HashSet<String> visitedCaves, String cave) {
            HashSet<String> updatedVisitedCaves = new HashSet<>(visitedCaves);
            updatedVisitedCaves.add(cave);

            return updatedVisitedCaves;
        }
    }
}