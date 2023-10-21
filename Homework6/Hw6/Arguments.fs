module Hw6.Arguments

open System
open Hw6.CalculatorOperation

[<CLIMutable>]
type arguments = {
    value1: Decimal
    operation: CalculatorOperation
    value2: Decimal
}