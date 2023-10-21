module Hw6.CalculatorHandler
open Giraffe
open Hw6.MaybeBuilder
open Hw6.Calculator
open Hw6.Arguments
let calculatorHandler: HttpHandler =
    fun next ctx ->
        let result = maybe{
            let! args = ctx.TryBindQueryString<arguments>()
            let! calcResult = calculate args
            
            return calcResult
        }
        
        match result with
        | Ok ok -> (setStatusCode 200 >=> text (ok.ToString())) next ctx
        | Error error -> (setStatusCode 400 >=> text error) next ctx