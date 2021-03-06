open System
open System.IO

let file_path = "C:/Users/david/source/repos/advent-of-code-2021/input/day_3.txt"
let input = File.ReadAllLines(file_path)

// Part One
let calculateRate(input: string[], useMostCommonBit: bool): int =
    input
    |> Array.map (fun x -> Seq.toArray x)
    |> Array.transpose
    |> Array.map (fun column -> 
        column 
        |> Array.countBy id 
        |> if useMostCommonBit then Array.sortByDescending (fun x -> snd x) 
           else Array.sortBy (fun x -> snd x) 
        |> Array.head 
        |> fst)
    |> fun x -> new string(x)
    |> fun x -> Convert.ToInt32(x, 2)

let gamma = calculateRate(input, true)
let epsilon = calculateRate(input, false)

let partOneResult = gamma * epsilon
printfn "%i" partOneResult

// Part Two
let getBitFilter(input: string[], index: int, useMostCommonBit: bool): char =
    input
    |> Array.map (fun x -> Seq.toArray x)
    |> Array.transpose
    |> fun x -> x[index]
    |> Array.countBy id
    |> if useMostCommonBit then Array.sortByDescending (fun x -> snd x, fst x) 
       else Array.sortBy (fun x -> snd x, fst x)
    |> Array.head
    |> fst

let calculateRating(input: string[], useMostCommonBit: bool): int = 
    let rec filterByBitCriteria(input: string[], index: int, useMostCommonBit: bool): string[] =
        let bitFilter = getBitFilter(input, index, useMostCommonBit)

        input
        |> Array.filter (fun x -> x[index] = bitFilter)
        |> fun x -> if x.Length > 1 then filterByBitCriteria(x, index + 1, useMostCommonBit) else x
    
    filterByBitCriteria(input, 0, useMostCommonBit)
        |> Array.head
        |> fun x -> Convert.ToInt32(x, 2)

let oxygenGeneratorRating = calculateRating(input, true)
let co2GeneratorRating = calculateRating(input, false)

let partTwoResult = oxygenGeneratorRating * co2GeneratorRating
printfn "%i" partTwoResult