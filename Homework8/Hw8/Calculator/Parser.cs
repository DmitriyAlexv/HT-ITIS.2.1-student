using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace Hw8.Calculator;

public class Parser: IParser
{
    public (double, Operation, double) ParseCalcArguments(string val1, string op, string val2)
    {
        if (!Double.TryParse(val1, NumberStyles.Any, CultureInfo.InvariantCulture, out var value1) ||
            !Double.TryParse(val2, NumberStyles.Any, CultureInfo.InvariantCulture, out var value2))
        {
            throw new ArgumentException(Messages.InvalidNumberMessage);
        }

        var operation = ParseOperation(op);
        if (operation == Operation.Invalid)
        {
            throw new InvalidOperationException(Messages.InvalidOperationMessage);
        }

        return (value1, operation, value2);
    }

    private Operation ParseOperation(string op)
    {
        return op.ToLower() switch
        {
            "plus" => Operation.Plus,
            "minus" => Operation.Minus,
            "multiply" => Operation.Multiply,
            "divide" => Operation.Divide,
            _ => Operation.Invalid
        };
    }
}