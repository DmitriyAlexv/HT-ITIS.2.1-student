using System.Linq.Expressions;
using Hw11.ErrorMessages;
using Hw11.Services.Parser;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hw11.Services.ExpressionBuilder;

public class ExpressionBuilder : IExpressionBuilder
{
    private readonly IStringExpressionParser _stringExpressionParser;

    public ExpressionBuilder(IStringExpressionParser stringExpressionParser)
    {
        _stringExpressionParser = stringExpressionParser;
    }
    
    public async Task<Expression> CreateExpressionFromString(string? expression)
    {
        var stringExpression = await _stringExpressionParser.ValidateExpression(expression);
        
        var mathExpression = new List<string>(); 
        mathExpression.Add("(");
        mathExpression.AddRange(stringExpression);
        mathExpression.Add(")");
        
        var mathExpressionWithPriority = PrioritizeByBrackets(mathExpression);
       
        return CreateExpressionFromPriorityList(
            mathExpressionWithPriority,
            mathExpressionWithPriority.Select<(string,int),(Expression,int)>(x => (null!, x.Item2)).ToList(),
            mathExpressionWithPriority.Max(x => x.Priority),
            new List<Task<Expression>>()
        );
    }

    private List<(string MathType, int Priority)> PrioritizeByBrackets(List<string> mathExpression)
    {
        var result = new List<(string mathType, int priority)>();

        var currentPriority = 1;
        foreach (var mathType in mathExpression)
        {
            if (mathType == "(")
                currentPriority++;
            result.Add((mathType, currentPriority));
            if (mathType == ")")
                currentPriority--;
        }

        return result;
    }

    private Expression CreateExpressionFromPriorityList(
        List<(string MathType, int Priority)> mathTypesWithPriority,
        List<(Expression MathType, int Priority)> expressionsWithPriority, 
        int currentPriority, 
        List<Task<Expression>> tasks, 
        bool isTaskCreated = false)
    {
        var currentLeft = mathTypesWithPriority.FindIndex(x => x.Priority == currentPriority && x.MathType == "(");
        var currentRight = mathTypesWithPriority.FindIndex(x => x.Priority == currentPriority && x.MathType == ")");
        if (currentLeft == -1 && currentRight == -1)
        {
            if (tasks.Count == 0)
                return currentPriority == 1
                    ? expressionsWithPriority.First().MathType
                    : CreateExpressionFromPriorityList(mathTypesWithPriority, expressionsWithPriority, currentPriority - 1, tasks);
            tasks.ForEach(task => task.Start());
            var res = Task.WhenAll(tasks).Result.FirstOrDefault()!;
            tasks.Clear();
            return res;
        }
        var task = CreateTaskToCreateExpressionFromBrackets(mathTypesWithPriority, expressionsWithPriority, currentPriority);
        tasks.Add(task);
        ReduceBracketExpression(mathTypesWithPriority, expressionsWithPriority, currentPriority, currentLeft, currentRight);
        
        var result = CreateExpressionFromPriorityList(mathTypesWithPriority, expressionsWithPriority, currentPriority, tasks, true); 
        expressionsWithPriority[currentLeft] = (task.Result ,currentPriority - 1);
        return isTaskCreated ? result : CreateExpressionFromPriorityList(mathTypesWithPriority, expressionsWithPriority, currentPriority, tasks);
    }

    private List<T> SelectCurrentBracket<T>(List<(T MathType, int Priority)> expressionsWithPriority, int currentPriority)
    {
        return expressionsWithPriority
            .Where(x => x.Priority == currentPriority)
            .Select(x => x.MathType)
            .Skip(1).SkipLast(1)
            .ToList();
    }

    private void ReduceBracketExpression(
        List<(string MathType, int Priority)> mathTypesWithPriority,
        List<(Expression MathType, int Priority)> expressionsWithPriority, 
        int currentPriority,
        int currentLeft,
        int currentRight)
    {
        expressionsWithPriority[currentLeft] = (null! ,currentPriority - 1);
        mathTypesWithPriority[currentLeft] = ("?", currentPriority - 1);                            
        expressionsWithPriority.RemoveRange(currentLeft + 1, currentRight - currentLeft);           
        mathTypesWithPriority.RemoveRange(currentLeft + 1, currentRight - currentLeft);
    }

    private Task<Expression> CreateTaskToCreateExpressionFromBrackets(        
        List<(string MathType, int Priority)> mathTypesWithPriority,
        List<(Expression MathType, int Priority)> expressionsWithPriority, 
        int currentPriority)
    {
        var currentBracketMathType = SelectCurrentBracket(mathTypesWithPriority, currentPriority);
        var currentBracketExpression = SelectCurrentBracket(expressionsWithPriority, currentPriority);
        if (currentBracketMathType.First() == "-")
        {
            currentBracketMathType[1] = $"-{currentBracketMathType[1]}";
            currentBracketMathType.RemoveAt(0);
            currentBracketExpression.RemoveAt(0);
        }
        return new Task<Expression>(() => CreateExpressionFromBrackets(
            currentBracketMathType,
            currentBracketExpression,
            new HashSet<string>() { "*", "/" },
            new List<Task<Expression>>()
        ));
    }
    
    private Expression CreateExpressionFromBrackets(
        List<string> mathTypes,
        List<Expression> expressions,
        HashSet<string> currentOps,
        List<Task<Expression>> tasks,
        int previousIndex = 0)
    {
        var operationIndex = previousIndex >= mathTypes.Count ? -1 : mathTypes.FindIndex(previousIndex, currentOps.Contains);
        if (operationIndex == -1)
        {
            if (tasks.Count == 0)
                return currentOps.Contains("+")
                    ? mathTypes.First() != "?"
                        ? CreateExpressionFromTwoValues("1", "*", mathTypes.First()) 
                        : expressions.First()
                    : CreateExpressionFromBrackets(mathTypes, expressions, new HashSet<string>() { "+", "-" }, tasks);
            tasks.ForEach(task => task.Start());
            var res =  Task.WhenAll(tasks).Result.FirstOrDefault()!;
            tasks.Clear();
            return res;
        }
        var task = CreateTaskToCreateExpressionFromTwoValues(operationIndex, expressions, mathTypes);
        tasks.Add(task);
        
        ReduceSimpleExpression(operationIndex, expressions, mathTypes);
           
        var result = CreateExpressionFromBrackets(mathTypes, expressions, currentOps, tasks, operationIndex + 1);
        expressions[operationIndex - 1] = task.Result;
        return previousIndex != 0 ? result : CreateExpressionFromBrackets(mathTypes, expressions, currentOps, tasks);
    }

    private void ReduceSimpleExpression(int operationIndex, List<Expression> expressions, List<string> mathTypes)
    {
        expressions[operationIndex - 1] = null!;
        mathTypes[operationIndex - 1] = "?";
        expressions.RemoveRange(operationIndex, 2);
        mathTypes.RemoveRange(operationIndex, 2);
    }

    private Task<Expression> CreateTaskToCreateExpressionFromTwoValues(int operationIndex, List<Expression> expressions, List<string> mathTypes)
    {
        var currentOp = mathTypes[operationIndex];
        
        var val1String = mathTypes[operationIndex - 1];
        var val2String = mathTypes[operationIndex + 1];
        var val1Expression = expressions[operationIndex - 1];
        var val2Expression = expressions[operationIndex + 1];
        
        return new Task<Expression>(() => 
            val1String == "?" ? 
                val2String == "?" ?
                    CreateExpressionFromTwoValues(val1Expression, currentOp, val2Expression)
                    : CreateExpressionFromTwoValues(val1Expression, currentOp, val2String)
                : val2String == "?" ? 
                    CreateExpressionFromTwoValues(val1String, currentOp, val2Expression)
                    : CreateExpressionFromTwoValues(val1String, currentOp, val2String));
    }
    
    private Expression CreateExpressionFromTwoValues<T, V>(T val1, string operation, V val2)
    {
        ConstantExpression? const1 = null;
        ConstantExpression? const2 = null;
        Expression? expr1 = null;
        Expression? expr2 = null;
        if (typeof(T) == typeof(string))
            const1 = Expression.Constant(Double.Parse((val1 as string)!));
        else
            expr1 = val1 as Expression;
        if (typeof(V) == typeof(string))
            const2 = Expression.Constant(Double.Parse((val2 as string)!));
        else
            expr2 = val2 as Expression;
        return operation switch
        {
            "+" => Expression.Add(const1 ?? expr1!, const2 ?? expr2!),
            "-" => Expression.Subtract(const1 ?? expr1!, const2 ?? expr2!),
            "*" => Expression.Multiply(const1 ?? expr1!, const2 ?? expr2!),
            _ => Expression.Divide(const1 ?? expr1!, const2 ?? expr2!)
        };
    }
}