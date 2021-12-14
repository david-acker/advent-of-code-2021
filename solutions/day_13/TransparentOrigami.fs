open System.IO

let file_path = "C:/Users/david/source/repos/advent-of-code-2021/input/day_13.txt"
let input: string[] = File.ReadAllText(file_path).Split "\r\n\r\n"

let coordinates = 
    input[0].Split "\r\n"
    |> Array.map (fun x -> 
        x.Split ','
        |> Array.map (fun x -> int x))

let getDimensionSize(index: int): int = 
    coordinates
    |> Array.map (fun x -> x[index])
    |> Array.maxBy (fun x -> x)
    |> fun x -> x + 1

let folds =
    input[1].Split "\r\n"
    |> Array.map (fun x ->
        x.Split ' '
        |> Array.last
        |> fun x -> x.Split '=')
    |> array2D

let height: int = getDimensionSize(0)
let width: int = getDimensionSize(1)

let mutable paper: bool[,] = Array2D.init height width (fun _ _ -> false)
for x: int[] in coordinates do
    paper[x[0], x[1]] <- true

let getFoldedRow(input: bool[,], index: int): bool[] =
    let topRow: bool[] = input.[index, *]
    let bottomRow: bool[] = input[input.[*, 0].Length - index - 1, *]

    Array.zip topRow bottomRow
        |> Array.map (fun (x: bool, y: bool) -> x || y)

let getFoldedColumn(input: bool[,], index: int): bool[] =
    let leftColumn: bool[] = input.[*, index]
    let rightColumn: bool[] = input.[*, input.[0, *].Length - index - 1]

    Array.zip leftColumn rightColumn
        |> Array.map (fun (x: bool, y: bool) -> x || y)

let applyFolds(paper: bool[,], folds: string[,]): bool[][] =
    let rec applyFold(paper: bool[,], folds: string[,]): bool[][] =
        let currentFold: string[] = folds[0, *]

        let foldType: string = currentFold |> Array.head |> fun x -> x.Trim()
        let foldIndex: int = currentFold |> Array.last |> int

        let foldedPaper: bool[][] = 
            if foldType = "x" 
            then [|for i in 0..(foldIndex - 1) -> getFoldedRow(paper, i)|]
            else [|for i in 0..(foldIndex - 1) -> getFoldedColumn(paper, i)|] |> Array.transpose

        if folds[1.., *].Length > 0 then applyFold(array2D foldedPaper, folds[1.., *]) else foldedPaper
    
    applyFold(paper, folds)

// Part One
let countAfterFirstFold =
    applyFolds(paper, folds[..0, *])
    |> Array.map (fun x -> 
        x
        |> Array.sumBy (fun x -> if x then 1 else 0))
    |> Array.fold (+) 0

printfn "%i" countAfterFirstFold

// Part Two
let paperAfterAllFolds: bool[][] = applyFolds(paper, folds) |> Array.transpose
for row in paperAfterAllFolds do
    let line: string =
        row
        |> Array.map (fun x -> if x then "X" else "-")
        |> String.concat ""
    printfn "%s" line