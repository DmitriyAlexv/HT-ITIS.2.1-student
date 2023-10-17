module Hw6.Calculator

open Hw6.Arguments
open Hw6.CalculatorOperation

let calculate (args: arguments): Result<string, string> =
    match args.operation with
    | CalculatorOperation.Plus -> Ok ((args.value1 + args.value2).ToString())
    | CalculatorOperation.Minus -> Ok ((args.value1 - args.value2).ToString())
    | CalculatorOperation.Multiply -> Ok ((args.value1 * args.value2).ToString())
    | CalculatorOperation.Divide ->
        if (args.value2 = 0m) then Ok "DivideByZero"
        else Ok ((args.value1 / args.value2).ToString())
    | _ -> Error "Invalid operation"
    
