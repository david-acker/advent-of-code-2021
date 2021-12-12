use std::fs;

fn main() {
    let file_name = r"C:\Users\david\source\repos\advent-of-code-2021\input\day_7.txt";
    let starting_positions: Vec<u32> =
        fs::read_to_string(&file_name)
            .unwrap()
            .lines()
            .next()
            .unwrap()
            .split(",")
            .map(|x| x.parse::<u32>().unwrap())
            .collect();

    // Part One
    let cheapest_fixed_fuel_cost = get_cheapest_fuel_cost(&starting_positions, get_fixed_fuel_cost);
    println!("{:?}", cheapest_fixed_fuel_cost);

    // Part Two
    let cheapest_increasing_fuel_cost = get_cheapest_fuel_cost(&starting_positions, get_increasing_fuel_cost);
    println!("{:?}", cheapest_increasing_fuel_cost);
}

fn get_cheapest_fuel_cost(positions: &[u32], get_fuel_cost: fn(u32, u32) -> u32) -> u32 {
    let min: u32 = *positions.iter().min().unwrap();
    let max: u32 = *positions.iter().max().unwrap();

    let mut cheapest_fuel_cost = u32::MAX;
    for end_position in min..=max {
        let fuel_cost = get_total_fuel_cost(
            positions,
            end_position,
            get_fuel_cost
        );

        if fuel_cost < cheapest_fuel_cost {
            cheapest_fuel_cost = fuel_cost;
        }
    }

    return cheapest_fuel_cost;
}

fn get_total_fuel_cost(positions: &[u32], end_position: u32, calculate_fuel_cost: fn(u32, u32) -> u32) -> u32 {
    let mut total_cost = 0;
    for p in positions {
        total_cost += calculate_fuel_cost(*p, end_position);
    }

    return total_cost;
}

fn get_fixed_fuel_cost(start: u32, end: u32) -> u32 {
    return get_distance(start, end);
}

fn get_increasing_fuel_cost(start: u32, end: u32) -> u32 {
    let distance: u32 = get_distance(start, end);
    return (distance * (distance + 1)) / 2;
}

fn get_distance(start_position: u32, final_position: u32) -> u32 {
    return ((final_position as i32) - (start_position as i32)).abs() as u32;
}