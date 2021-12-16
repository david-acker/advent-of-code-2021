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
    |> List.collect id

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

        if stepsRemaining - 1 > 0
        then growPolymer(newPolymer, stepsRemaining - 1)
        else newPolymer
    
    growPolymer(polymerTemplate, steps)

let getElementCountDifference(polymerTemplate: string, pairRules: list<string * string>, steps: int): int =
    growPolymerBySteps(polymerTemplate, pairRules, steps).ToCharArray()
    |> Array.countBy id
    |> Array.sortByDescending (fun x -> snd x)
    |> fun x ->
        (x |> Array.head |> snd) -
        (x |> Array.last |> snd)

// Input Data
let file_path = "C:/Users/david/source/repos/advent-of-code-2021/input/day_14.txt"
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

// Part Two (started over since the last solution was too inefficient)
let rules: Map<char * char, char> =
    input[1].Split("\r\n")
    |> Array.map (fun x ->
        x.Split(" -> ")
        |> fun x -> ((x[0][0], x[0][1]), char x[1]))
    |> Map.ofArray

let initialPairMap: Map<char * char, bigint> = 
    [|for i in 0..(polymerTemplate.Length - 2) -> (polymerTemplate[i], polymerTemplate[i + 1])|]
    |> Array.groupBy id
    |> Array.map (fun (x, y) -> (x, bigint y.Length))
    |> Map.ofArray

let getChangesForRule(initialPairMap: Map<char * char, bigint>, pair: char * char, element: char): Map<char * char, bigint> =
    if initialPairMap.ContainsKey pair
    then 
        Map [((fst pair, element), initialPairMap.[pair]); ((element, snd pair), initialPairMap.[pair])]
        |> fun map -> 
            if map.ContainsKey pair
            then Map.add pair (map.[pair] - initialPairMap.[pair]) map
            else Map.add pair -initialPairMap.[pair] map
    else Map.empty

let getMergedValue(key: char * char, mapA: Map<char * char, bigint>, mapB: Map<char * char, bigint>): bigint =
    let valueA: bigint = if mapA.ContainsKey key then mapA.[key] else bigint 0
    let valueB: bigint = if mapB.ContainsKey key then mapB.[key] else bigint 0
    valueA + valueB

let getMergedMap(mapA: Map<char * char, bigint>, mapB: Map<char * char, bigint>): Map<char * char, bigint> =
    Set.union (set mapA.Keys) (set mapB.Keys)
    |> Set.toArray
    |> Array.map (fun key -> (key, getMergedValue(key, mapA, mapB)))
    |> Map.ofArray

let getNextPairMap(pairMap: Map<char * char, bigint>, rules: Map<char * char, char>): Map<char * char, bigint> = 
    rules
    |> Map.toArray
    |> Array.map (fun (pair, element) -> getChangesForRule(pairMap, pair, element))
    |> Array.fold (fun acc map -> getMergedMap(acc, map)) Map.empty
    |> fun deltaMap -> getMergedMap(pairMap, deltaMap)

let getPairMap(initialPairMap: Map<char * char, bigint>, rules: Map<char * char, char>, steps: int): Map<char * char, bigint> =
    let rec getNext(pairMap: Map<char * char, bigint>, remainingSteps: int): Map<char * char, bigint> =
        if remainingSteps > 0
        then getNext(getNextPairMap(pairMap, rules), remainingSteps - 1)
        else pairMap

    getNext(initialPairMap, steps)

let stepCount: int = 40
let lastElement: char = char polymerTemplate[(polymerTemplate.Length - 1)..]
let partTwoResult =
    getPairMap(initialPairMap, rules, stepCount)
    |> Map.toArray
    |> Array.groupBy (fun x -> fst (fst x))
    |> Array.map (fun (x, y) ->
        (x, y 
            |> Array.map snd
            |> Array.reduce (fun acc i -> acc + i)))
    |> Map.ofArray
    |> fun x -> (Map.add lastElement (x.[lastElement] + bigint 1) x)
    |> Map.toArray
    |> Array.sortByDescending (fun x -> snd x)
    |> fun x -> 
        (x |> Array.head |> snd) -
        (x |> Array.last |> snd)

printfn "%O" partTwoResult