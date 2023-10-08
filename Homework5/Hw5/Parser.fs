module Hw5.Parser

open System
open System.Globalization
open Hw5.Calculator
open Hw5.MaybeBuilder
open Microsoft.FSharp.Core

let isArgLengthSupported (args:string[]): Result<string array,Message> =
    if args.Length = 3 then Ok args
    else Error Message.WrongArgLength
    
[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isOperationSupported (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    match operation with
    | Calculator.plus -> Ok (arg1, CalculatorOperation.Plus, arg2)
    | Calculator.minus -> Ok (arg1, CalculatorOperation.Minus, arg2)
    | Calculator.multiply -> Ok (arg1, CalculatorOperation.Multiply, arg2)
    | Calculator.divide -> Ok (arg1, CalculatorOperation.Divide, arg2)
    | operation -> Error Message.WrongArgFormatOperation
    
let parseArgs (args: string[]): Result<('a * string * 'b), Message> when 'a: struct and 'b: struct=
    let culture = System.Globalization.CultureInfo("en-US")
    let numberStyle = NumberStyles.Any
    let parseMethodA = typeof<'a>.GetMethod("TryParse", [| typeof<string> ;typeof<NumberStyles>;typeof<IFormatProvider>; typeof<'a>.MakeByRefType()|])
    let parseMethodB = typeof<'b>.GetMethod("TryParse", [| typeof<string> ;typeof<NumberStyles>;typeof<IFormatProvider>; typeof<'a>.MakeByRefType()|])
    let argsArr1: obj[] = [|args[0];numberStyle;culture;null|]
    let argsArr2: obj[] = [|args[2];numberStyle;culture;null|]
    let isSuccessVal1 = parseMethodA.Invoke(null, argsArr1) |> unbox<bool> 
    let isSuccessVal2 = parseMethodB.Invoke(null, argsArr2) |> unbox<bool>
    if not (isSuccessVal1 && isSuccessVal2) then Error Message.WrongArgFormat
    else
    let val1 = argsArr1[3]|> unbox<'a>
    let val2 = argsArr2[3]|> unbox<'b>
    Ok (val1, args[1], val2)


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