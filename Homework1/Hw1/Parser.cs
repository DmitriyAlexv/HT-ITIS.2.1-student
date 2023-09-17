namespace Hw1;

public static class Parser
{
    public static void ParseCalcArguments(string[] args,
        out double val1,
        out CalculatorOperation operation,
        out double val2)
    {
        if (!IsArgLengthSupported(args) || !double.TryParse(args[0], out val1) || !double.TryParse(args[2], out val2))
        {
            throw new ArgumentException("Invalid arguments for calculating");
        }
        operation = ParseOperation(args[1]);
        if (operation == CalculatorOperation.Undefined)
        {
            throw new InvalidOperationException("Invalid operation");
        }
    }

    private static bool IsArgLengthSupported(string[] args) => args.Length == 3;

    private static CalculatorOperation ParseOperation(string arg)
    {
        switch (arg)
        {
            case "+":
                return CalculatorOperation.Plus;
            case "-":
                return CalculatorOperation.Minus;
            case "*":
                return CalculatorOperation.Multiply;
            case "/":
                return CalculatorOperation.Divide;
            default:
                return CalculatorOperation.Undefined;
        }
    }
}