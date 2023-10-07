module Hw5.Parser

open System
open Hw5.Calculator
open Hw5.MaybeBuilder
open Microsoft.FSharp.Core

let isArgLengthSupported (args:string[]): Result<string array,Message> =
    if args.Length = 3 then Ok args
    else Error Message.WrongArgLength
    
[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isOperationSupported (arg1, operation:CalculatorOperation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    let _operation = Convert.ToInt32(operation)
    if 0 <= _operation && _operation <= 3 then Ok (arg1, operation, arg2)
    else Error Message.WrongArgFormatOperation

let parseArgs (args: string[]): Result<('a * CalculatorOperation * 'b), Message> when 'a: struct and 'b: struct=
    let f = typeof<'a>
    let parseMethodA = typeof<'a>.GetMethod("TryParse", [| typeof<string> ;typeof<'a>.MakeByRefType()|])
    let parseMethodB = typeof<'b>.GetMethod("TryParse", [| typeof<string> ;typeof<'a>.MakeByRefType()|])
    let argsArr1: obj[] = [|args[0];null|]
    let argsArr2: obj[] = [|args[2];null|]
    let isSuccessVal1 = parseMethodA.Invoke(null, argsArr1) |> unbox<bool> 
    let isSuccessVal2 = parseMethodB.Invoke(null, argsArr2) |> unbox<bool>
    if not (isSuccessVal1 && isSuccessVal2) then Error Message.WrongArgFormat
    else
    let val1 = argsArr1[1]|> unbox<'a>
    let val2 = argsArr2[1]|> unbox<'b>
    match args[1] with
    | Calculator.plus -> Ok (val1, CalculatorOperation.Plus, val2)
    | Calculator.minus -> Ok (val1, CalculatorOperation.Minus, val2)
    | Calculator.multiply -> Ok (val1, CalculatorOperation.Multiply, val2)
    | Calculator.divide -> Ok (val1, CalculatorOperation.Divide, val2)
    | _ -> Ok (val1, enum<CalculatorOperation>(4), val2)


[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isDividingByZero (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    if operation = CalculatorOperation.Divide && arg2.ToString() = "0" then Error Message.DivideByZero
    else Ok (arg1, operation, arg2)
    
let parseCalcArguments (args: string[]): Result<('a * CalculatorOperation * 'b), Message> =
    maybe{
        let! argsAfterLengthCheck = isArgLengthSupported args
        let! argsAfterParse = parseArgs argsAfterLengthCheck
        let! argsAfterOpCheck = isOperationSupported argsAfterParse
        let! argsAfterZeroCheck = isDividingByZero argsAfterOpCheck
        
        return argsAfterZeroCheck
    }