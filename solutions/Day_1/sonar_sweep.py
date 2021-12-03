import os

input_path = f'{os.getcwd()}/input/day_1.txt'

# Part One
with open(input_path) as file:
    measurements = [int(line) for line in file.readlines()]

    count = sum([current > last for current, last in zip(measurements[1:], measurements)])

    print(count)

# Part Two
with open(input_path) as file:
    measurements = [int(line) for line in file.readlines()]

    count = 0
    window_size = 3
    for i in range(len(measurements) - window_size):
        previous_window = measurements[i:(i + window_size)]
        current_window = measurements[(i + 1):(i + window_size + 1)]

        if (sum(current_window) > sum(previous_window)):
            count += 1
    
    print(count)

    