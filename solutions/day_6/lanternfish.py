from collections import Counter

def get_next_population_map(population_map: dict[int, int]) -> dict[int, int]:
    next_population_map: dict[int, int] = {}
    for countdown, population in population_map.items():
        if countdown == 0:
            next_population_map[6] = next_population_map.get(6, 0) + population
            next_population_map[8] = next_population_map.get(8, 0) + population
        else:
            next_population_map[countdown - 1] = next_population_map.get(countdown - 1, 0) + population

    return next_population_map

def get_final_population(population_map: dict[int, int], days: int) -> int:
    if (days > 0):
        next_population_map = get_next_population_map(population_map)
        return get_final_population(next_population_map, days - 1)

    return sum(population_map.values())

with open('C:/Users/david/source/repos/advent-of-code-2021/input/day_6.txt') as file:
    initial_population_map: dict[int, int] = Counter([int(countdown) for countdown in file.readline().split(',')])

# Part One
final_population: int = get_final_population(initial_population_map, days=80)
print(final_population)

# Part Two
final_population: int = get_final_population(initial_population_map, days=256)
print(final_population)