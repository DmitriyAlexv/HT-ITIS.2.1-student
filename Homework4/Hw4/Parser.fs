module Hw4.Parser

open System
open Hw4.Calculator


type CalcOptions = {
    arg1: float
    arg2: float
    operation: CalculatorOperation
}

let isArgLengthSupported (args : string[]) =
    args.Length = 3

let parseOperation (arg : string) =
    match arg with
    | "+" -> CalculatorOperation.Plus
    | "-" -> CalculatorOperation.Minus
    | "*" -> CalculatorOperation.Multiply
    | "/" -> CalculatorOperation.Divide
    | arg -> CalculatorOperation.Undefined
    
let parseCalcArguments(args : string[]) =
    let isSuccessVal1, val1 = Double.TryParse(args[0])
    let isSuccessVal2, val2 = Double.TryParse(args[2])
    let operation = parseOperation(args[1])
    if not (isSuccessVal1 && isSuccessVal2 && isArgLengthSupported(args)) then invalidArg "" ""
    elif operation = CalculatorOperation.Undefined then invalidOp ""
    else {arg1 =  val1; arg2 = val2; operation = operation}