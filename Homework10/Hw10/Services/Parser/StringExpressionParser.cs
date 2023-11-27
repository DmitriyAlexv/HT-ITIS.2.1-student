using Hw10.ErrorMessages;

namespace Hw10.Services.Parser;

public class StringExpressionParser
{
    private static readonly HashSet<char> _digits = new() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private static readonly HashSet<char> _operations = new() { '+', '-', '*', '/' };
    private static readonly HashSet<char> _brackets = new() { '(', ')' };
    
    public async Task<StringExpressionParserResult> ValidateExpression(string? expression)
    {
        var mathExpression = DefineSymbols(expression);
        if (!mathExpression.IsSuccess)
        {
            return new StringExpressionParserResult(mathExpression.ErrorMessage!);
        }

        var specialTask = new Task<StringExpressionParserResult>(() => JoinDigits(mathExpression.MathExpression!));
        var tasks = new List<Task<StringExpressionParserResult>>()
        {
            new Task<StringExpressionParserResult>(() => IsFirstSymbolAnOperation(mathExpression.MathExpression!)),
            new Task<StringExpressionParserResult>(() => IsLastSymbolAnOperation(mathExpression.MathExpression!)),
            new Task<StringExpressionParserResult>(() => IsCorrectCountOfBrackets(mathExpression.MathExpression!)),
            new Task<StringExpressionParserResult>(() => IsDigitBeforeRightBrackets(mathExpression.MathExpression!)),
            new Task<StringExpressionParserResult>(() => IsTwoOperationInRow(mathExpression.MathExpression!)),
            new Task<StringExpressionParserResult>(() => IsCorrectOperationAfterLeftBrackets(mathExpression.MathExpression!)),
            specialTask,
        };
        tasks.ForEach(task => task.Start());
        var wrongResult = (await Task.WhenAll(tasks)).FirstOrDefault(result => !result.IsSuccess);
        return wrongResult == null ?
            new StringExpressionParserResult(specialTask.Result.MathExpression!) :
            new StringExpressionParserResult(wrongResult.ErrorMessage!);
    }
    
    private StringExpressionParserResult DefineSymbols(string? expression)
    {
        if (string.IsNullOrEmpty(expression))
            return new StringExpressionParserResult(MathErrorMessager.EmptyString);

        var result = new List<string>();
        foreach (var symbol in expression)
        {
            switch (symbol)
            {
                case var bracket when _brackets.Contains(symbol):
                case var digit when _digits.Contains(symbol):
                case var operation when _operations.Contains(symbol):
                    result.Add(symbol.ToString());
                    break;
                case '.':
                    result.Add(".");
                    break;
                case ' ':
                    break;
                default:
                    return new StringExpressionParserResult(MathErrorMessager.UnknownCharacterMessage(symbol));
            }
        }
        return new StringExpressionParserResult(result);
    }

    private StringExpressionParserResult IsFirstSymbolAnOperation(List<string> mathExpression) =>
        _operations.Contains(mathExpression[0][0]) ? 
            new StringExpressionParserResult(MathErrorMessager.StartingWithOperation) : 
            new StringExpressionParserResult(mathExpression);
    
    private StringExpressionParserResult IsLastSymbolAnOperation(List<string> mathExpression) =>
        _operations.Contains(mathExpression[^1][^1]) ? 
            new StringExpressionParserResult(MathErrorMessager.EndingWithOperation) : 
            new StringExpressionParserResult(mathExpression);

    private StringExpressionParserResult IsCorrectCountOfBrackets(List<string> mathExpression) =>
        mathExpression.Count(x => x == ")") == mathExpression.Count(x => x == "(")
            ? new StringExpressionParserResult(mathExpression)
            : new StringExpressionParserResult(MathErrorMessager.IncorrectBracketsNumber);

    private StringExpressionParserResult IsTwoOperationInRow(List<string> mathExpression)
    {
        for (var i = 0; i < mathExpression.Count - 1; i++)
        {
            if (_operations.Contains(mathExpression[i][0]) && _operations.Contains(mathExpression[i + 1][0]))
                return new StringExpressionParserResult(MathErrorMessager.TwoOperationInRowMessage(mathExpression[i], mathExpression[i + 1]));
        }

        return new StringExpressionParserResult(mathExpression);
    }
    
    private StringExpressionParserResult IsCorrectOperationAfterLeftBrackets(List<string> mathExpression)
    {
        for (var i = 0; i < mathExpression.Count - 1; i++)
        {
            if (mathExpression[i] == "(" && mathExpression[i + 1] != "-" && _operations.Contains(mathExpression[i + 1][0]))
                return new StringExpressionParserResult(MathErrorMessager.InvalidOperatorAfterParenthesisMessage(mathExpression[i + 1]));
        }

        return new StringExpressionParserResult(mathExpression);
    }
    
    private StringExpressionParserResult IsDigitBeforeRightBrackets(List<string> mathExpression)
    {
        for (var i = 1; i < mathExpression.Count; i++)
        {
            if (mathExpression[i] == ")" && !_digits.Contains(mathExpression[i - 1][0]))
                return new StringExpressionParserResult(MathErrorMessager.OperationBeforeParenthesisMessage(mathExpression[i - 1]));
        }

        return new StringExpressionParserResult(mathExpression);
    }

    private StringExpressionParserResult JoinDigits(List<string> mathExpression)
    {
        var result = new List<string>();
        for (var i = 0; i < mathExpression.Count; i++)
        {
            if (_digits.Contains(mathExpression[i][0]))
            {
                var j = i + 1;
                for (; j < mathExpression.Count; j++)
                {
                    if (!_digits.Contains(mathExpression[j][0]) && mathExpression[j] != ".")
                    {
                        break;
                    }
                }
                result.Add(mathExpression.GetRange(i, j - i).Aggregate("", (x,y) => $"{x}{y}"));
                if (result[^1].Count(x => x == '.') > 1 || result[^1][^1] == '.')
                    return new StringExpressionParserResult(MathErrorMessager.NotNumberMessage(result[^1]));
                i = j - 1;
            }
            else
            {
                result.Add(mathExpression[i]);
            }
        }

        return new StringExpressionParserResult(result);
    }
}