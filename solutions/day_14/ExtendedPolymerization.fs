open System.IO

let getInsertionIndices(polymer: string, pair: string): list<int> = 
    let rec getInsertionIndex(polymer: string, pair: string, indices: list<int>): list<int> = 
        let lastMatchIndex: int = 
            if Seq.isEmpty indices
            then 0
            else (List.max indices) + 1
        
        let matchIndex: int = polymer.IndexOf(pair, lastMatchIndex)

        if matchIndex = -1 
        then indices
        else getInsertionIndex(polymer, pair, List.append indices [matchIndex])
    
    getInsertionIndex(polymer, pair, [])

let getInstructions(polymer: string, pairRules: list<string * string>): list<int * string> =
    pairRules
    |> List.map (fun (pair, element) -> 
        getInsertionIndices(polymer, pair)
        |> List.map (fun index -> (polymer.Length - index - 1, element)))
    |> List.collect (fun x -> x)

let insertElement(polymer: string, reverseIndex: int, element: string): string =
    let insertionIndex: int = polymer.Length - reverseIndex - 1

    polymer[0..insertionIndex] + element + polymer[(insertionIndex + 1)..]

let applyInstructions(polymer: string, instructions: list<int * string>) = 
    instructions
    |> List.sortByDescending (fun (reverseIndex, _) -> reverseIndex)
    |> List.fold (fun polymer (reverseIndex, element) -> insertElement(polymer, reverseIndex, element)) polymer

let growPolymerBySteps(polymerTemplate: string, pairRules: list<string * string>, steps: int): string =
    let rec growPolymer(polymer: string, stepsRemaining: int) = 
        
        let instructions: list<int * string> = getInstructions(polymer, pairRules)
        let newPolymer: string = applyInstructions(polymer, instructions)

        printfn "%i" (stepsRemaining - 1)

        if stepsRemaining - 1 > 0
        then growPolymer(newPolymer, stepsRemaining - 1)
        else newPolymer
    
    growPolymer(polymerTemplate, steps)

let getElementCountDifference(polymerTemplate: string, pairRules: list<string * string>, steps: int): int =
    growPolymerBySteps(polymerTemplate, pairRules, steps).ToCharArray()
    |> Array.countBy id
    |> fun x -> (
        (x |> Array.sortByDescending (fun x -> snd x) |> Array.head |> snd) - 
        (x |> Array.sortBy (fun x -> snd x) |> Array.head |> snd))

// Input Data
let file_path = "C:/Users/david/source/repos/advent-of-code-2021/input/day_14_test.txt"
let input: string[] = File.ReadAllText(file_path).Split "\r\n\r\n"

let polymerTemplate: string = input[0].Trim()
let pairRules: list<string * string> =
    input[1].Split("\r\n")
    |> Array.map (fun x -> 
        x.Split(" -> ") 
        |> (fun x -> (x[0], x[1])))
    |> Array.toList

// Part One
let partOneResult: int = getElementCountDifference(polymerTemplate, pairRules, 10);
printfn "%i" partOneResult