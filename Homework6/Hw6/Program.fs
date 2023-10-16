module Hw6.App

open System
open System.Globalization
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

let getResult (val1:string) (operation:string) (val2:string)=
    let culture = System.Globalization.CultureInfo("en-US")
    let numberStyle = NumberStyles.Any
    let isSuc1, v1 = Double.TryParse(val1, numberStyle ,culture)
    let isSuc2, v2 = Double.TryParse(val2, numberStyle ,culture)
    match (isSuc1, isSuc2) with
    | (false, _) -> Error $"Could not parse value '{val1}'"
    | (_, false) -> Error $"Could not parse value '{val2}'"
    | _ ->
        match operation with
        | "Plus" -> Ok (v1 + v2)
        | "Minus" -> Ok (v1 - v2)
        | "Multiply" ->Ok (v1 * v2)
        | "Divide" -> if v2 = 0 then Error "DivideByZero" else Ok (v1 / v2)
        | _ -> Error $"Could not parse value '{operation}'"
        
let calculatorHandler: HttpHandler =
    fun next ctx ->
        let (val1, operation, val2)=
            match (ctx.GetQueryStringValue "value1",ctx.GetQueryStringValue "operation",ctx.GetQueryStringValue "value2") with
            | (Ok v1, Ok op, Ok v2) -> (v1, op, v2)
            | _ -> Exception() |> raise
        let result:Result<Double,string> = getResult val1 operation val2
        
        match result with
        | Ok ok -> (setStatusCode 200 >=> text (ok.ToString())) next ctx
        | Error error -> ((if error = "DivideByZero" then setStatusCode 200
                          else setStatusCode 400) >=> text error) next ctx

let webApp =
    choose [
        GET >=> choose [
             route "/" >=> text "Use //calculate?value1=<VAL1>&operation=<OPERATION>&value2=<VAL2>"
        ]
        GET >=> choose [
             route "/calculate" >=> calculatorHandler
        ]
        setStatusCode 404 >=> text "Not Found" 
    ]
    
type Startup() =
    member _.ConfigureServices (services : IServiceCollection) =
        services.AddGiraffe() |> ignore

    member _.Configure (app : IApplicationBuilder) (_ : IHostEnvironment) (_ : ILoggerFactory) =
        app.UseGiraffe webApp
        
[<EntryPoint>]
let main _ =
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun whBuilder -> whBuilder.UseStartup<Startup>() |> ignore)
        .Build()
        .Run()
    0