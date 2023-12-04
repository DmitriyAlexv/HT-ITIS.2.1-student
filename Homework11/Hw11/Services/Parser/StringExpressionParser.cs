using System.Net;
using System.Runtime.CompilerServices;
using Hw11.Dto;
using Hw11.ErrorMessages;
using Hw11.Exceptions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Hw11.Services.Parser;

public class StringExpressionParser : IStringExpressionParser
{
    private static readonly HashSet<char> _digits = new() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private static readonly HashSet<char> _operations = new() { '+', '-', '*', '/' };
    private static readonly HashSet<char> _brackets = new() { '(', ')' };
    
    public async Task<List<string>> ValidateExpression(string? expression)
    {
        var mathExpression = DefineSymbols(expression);

        var specialTask = new Task<List<string>>(() => JoinDigits(mathExpression));
        var tasks = new List<Task<List<string>>>()
        {
            new (() => IsFirstSymbolAnOperation(mathExpression)),
            new (() => IsLastSymbolAnOperation(mathExpression)),
            new (() => IsCorrectCountOfBrackets(mathExpression)),
            new (() => IsDigitBeforeRightBrackets(mathExpression)),
            new (() => IsTwoOperationInRow(mathExpression)),
            new (() => IsCorrectOperationAfterLeftBrackets(mathExpression)),
            specialTask,
        };
        tasks.ForEach(task => task.Start());
        await Task.WhenAll(tasks);
        return specialTask.Result;
    }
    
    private List<string> DefineSymbols(string? expression)
    {
        if (string.IsNullOrEmpty(expression)) 
            throw new Exception(MathErrorMessager.EmptyString);

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
                    throw new InvalidSymbolException(MathErrorMessager.UnknownCharacterMessage(symbol));
            }
        }
        return result;
    }

    private List<string> IsFirstSymbolAnOperation(List<string> mathExpression) =>
        _operations.Contains(mathExpression[0][0]) ? 
            throw new InvalidSyntaxException(MathErrorMessager.StartingWithOperation) : 
            mathExpression;
    
    private List<string> IsLastSymbolAnOperation(List<string> mathExpression) =>
        _operations.Contains(mathExpression[^1][^1]) ? 
            throw new InvalidSyntaxException(MathErrorMessager.EndingWithOperation) : 
            mathExpression;

    private List<string> IsCorrectCountOfBrackets(List<string> mathExpression) =>
        mathExpression.Count(x => x == ")") == mathExpression.Count(x => x == "(")
            ? mathExpression
            : throw new InvalidSyntaxException(MathErrorMessager.IncorrectBracketsNumber);

    private List<string> IsTwoOperationInRow(List<string> mathExpression)
    {
        for (var i = 0; i < mathExpression.Count - 1; i++)
        {
            if (_operations.Contains(mathExpression[i][0]) && _operations.Contains(mathExpression[i + 1][0]))
                throw new InvalidSyntaxException(MathErrorMessager.TwoOperationInRowMessage(mathExpression[i], mathExpression[i + 1]));
        }

        return mathExpression;
    }
    
    private List<string> IsCorrectOperationAfterLeftBrackets(List<string> mathExpression)
    {
        for (var i = 0; i < mathExpression.Count - 1; i++)
        {
            if (mathExpression[i] == "(" && mathExpression[i + 1] != "-" && _operations.Contains(mathExpression[i + 1][0]))
                throw new InvalidSyntaxException(MathErrorMessager.InvalidOperatorAfterParenthesisMessage(mathExpression[i + 1]));
        }

        return mathExpression;
    }
    
    private List<string> IsDigitBeforeRightBrackets(List<string> mathExpression)
    {
        for (var i = 1; i < mathExpression.Count; i++)
        {
            if (mathExpression[i] == ")" && !_digits.Contains(mathExpression[i - 1][0]))
                throw new InvalidSyntaxException(MathErrorMessager.OperationBeforeParenthesisMessage(mathExpression[i - 1]));
        }

        return mathExpression;
    }

    private List<string> JoinDigits(List<string> mathExpression)
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
                    throw new InvalidNumberException(MathErrorMessager.NotNumberMessage(result[^1]));
                i = j - 1;
            }
            else
            {
                result.Add(mathExpression[i]);
            }
        }

        return result;
    }
}