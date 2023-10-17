open System
open System.Net.Http
open Microsoft.FSharp.Control

let calculateAsync (query:string) =
    async{
        use client = new HttpClient()
        let! response = client.GetAsync(query) |> Async.AwaitTask
        let! result = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return result
    }

[<EntryPoint>]
let runConsoleCalc _=
    async {
        printfn "Enter an value1 to calculate:"
        let val1 = Console.ReadLine()
        printfn "Enter an operation to calculate:"
        let operation = Console.ReadLine()
        printfn "Enter an value2 to calculate:"
        let val2 = Console.ReadLine()
        let query = $"https://localhost:59540/calculate?value1={val1}&operation={operation}&value2={val2}"
        let! result = calculateAsync query
        printfn $"Result: {result}"
    } |> Async.RunSynchronously
    0
