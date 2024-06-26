using System.Linq.Expressions;
using Hw9.ErrorMessages;
using Hw9.Services.Parser;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hw9.Services.ExpressionBuilder;

public class ExpressionBuilder
{
    private readonly StringExpressionParser _stringExpressionParser = new();
    public async Task<ExpressionBuilderResult> CreateExpressionFromString(string? expression)
    {
        var stringExpression = await _stringExpressionParser.ValidateExpression(expression);
        
        if (!stringExpression.IsSuccess)
            return new ExpressionBuilderResult(stringExpression.ErrorMessage!);
        
        var mathExpression = new List<string>(); 
        mathExpression.Add("(");
        mathExpression.AddRange(stringExpression.MathExpression!);
        mathExpression.Add(")");
        
        var mathExpressionWithPriority = PrioritizeByBrackets(mathExpression);
       
        return CreateExpressionFromPriorityList(
            mathExpressionWithPriority,
            mathExpressionWithPriority.Select<(string,int),(Expression,int)>(x => (null!, x.Item2)).ToList(),
            mathExpressionWithPriority.Max(x => x.Priority),
            new List<Task<ExpressionBuilderResult>>()
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

    private ExpressionBuilderResult CreateExpressionFromPriorityList(
        List<(string MathType, int Priority)> mathTypesWithPriority,
        List<(Expression MathType, int Priority)> expressionsWithPriority, 
        int currentPriority, 
        List<Task<ExpressionBuilderResult>> tasks, 
        bool isTaskCreated = false)
    {
        var currentLeft = mathTypesWithPriority.FindIndex(x => x.Priority == currentPriority && x.MathType == "(");
        var currentRight = mathTypesWithPriority.FindIndex(x => x.Priority == currentPriority && x.MathType == ")");
        if (currentLeft == -1 && currentRight == -1)
        {
            if (tasks.Count != 0)
            {
                tasks.ForEach(task => task.Start());
                var res = Task.WhenAll(tasks).Result.FirstOrDefault(x => !x.IsSuccess)!;
                tasks.Clear();
                return res;
            }
            return currentPriority == 1 ?                                                                                    
                new ExpressionBuilderResult(expressionsWithPriority.First().MathType) :                                      
                CreateExpressionFromPriorityList(mathTypesWithPriority, expressionsWithPriority, currentPriority - 1, tasks);   
        }
        
        var currentBracketMathType = mathTypesWithPriority
            .Where(x => x.Priority == currentPriority)
            .Select(x => x.MathType)
            .Skip(1).SkipLast(1)
            .ToList();
        var currentBracketExpression = expressionsWithPriority
            .Where(x => x.Priority == currentPriority)
            .Select(x => x.MathType)
            .Skip(1).SkipLast(1)
            .ToList();
        if (currentBracketMathType.First() == "-")
        {
            currentBracketMathType[1] = $"-{currentBracketMathType[1]}";
            currentBracketMathType.RemoveAt(0);
            currentBracketExpression.RemoveAt(0);
        }

        var task = new Task<ExpressionBuilderResult>(() => CreateExpressionFromBrackets(
            currentBracketMathType,
            currentBracketExpression,
            new HashSet<string>() { "*", "/" },
            new List<Task<ExpressionBuilderResult>>()
        ));
        tasks.Add(task);
        
        expressionsWithPriority[currentLeft] = (null! ,currentPriority - 1);
        mathTypesWithPriority[currentLeft] = ("?", currentPriority - 1);                            
        expressionsWithPriority.RemoveRange(currentLeft + 1, currentRight - currentLeft);           
        mathTypesWithPriority.RemoveRange(currentLeft + 1, currentRight - currentLeft);
        
        var result = CreateExpressionFromPriorityList(mathTypesWithPriority, expressionsWithPriority, currentPriority, tasks, true); 
        
        if (!task.Result.IsSuccess)
            return result;
        
        expressionsWithPriority[currentLeft] = (task.Result.MathExpression! ,currentPriority - 1);
         
        return isTaskCreated ? 
            result : 
            CreateExpressionFromPriorityList(mathTypesWithPriority, expressionsWithPriority, currentPriority, tasks);
    }

    private ExpressionBuilderResult CreateExpressionFromBrackets(
        List<string> mathTypes,
        List<Expression> expressions,
        HashSet<string> currentOps,
        List<Task<ExpressionBuilderResult>> tasks,
        int previousIndex = 0)
    {
        var operationIndex = previousIndex >= mathTypes.Count ? -1 : mathTypes.FindIndex(previousIndex, currentOps.Contains);

        if (operationIndex == -1)
        {
            if (tasks.Count != 0)
            {
                tasks.ForEach(task => task.Start());
                var res =  Task.WhenAll(tasks).Result.FirstOrDefault(x => !x.IsSuccess)!;
                tasks.Clear();
                return res;
            }
            return currentOps.Contains("+") ? 
                mathTypes.First() != "?" ?
                    CreateExpressionFromTwoValues("1", "*", mathTypes.First()) :
                    new ExpressionBuilderResult(expressions.First()) 
                : CreateExpressionFromBrackets(mathTypes, expressions, new HashSet<string>() { "+", "-" }, tasks);
        }

        
        var currentOp = mathTypes[operationIndex];
        
        var val1String = mathTypes[operationIndex - 1];
        var val2String = mathTypes[operationIndex + 1];
        var val1Expression = expressions[operationIndex - 1];
        var val2Expression = expressions[operationIndex + 1];
        
        var task = new Task<ExpressionBuilderResult>(() => 
            val1String == "?" ? 
                val2String == "?" ?
                    CreateExpressionFromTwoValues(val1Expression, currentOp, val2Expression)
                    : CreateExpressionFromTwoValues(val1Expression, currentOp, val2String)
                : val2String == "?" ? 
                    CreateExpressionFromTwoValues(val1String, currentOp, val2Expression)
                    : CreateExpressionFromTwoValues(val1String, currentOp, val2String));
       tasks.Add(task);
       
       expressions[operationIndex - 1] = null!;
       mathTypes[operationIndex - 1] = "?";
       expressions.RemoveRange(operationIndex, 2);
       mathTypes.RemoveRange(operationIndex, 2);

       var result = CreateExpressionFromBrackets(mathTypes, expressions, currentOps, tasks, operationIndex + 1);
       
        if (!task.Result.IsSuccess)
            return result;
        
        expressions[operationIndex - 1] = task.Result.MathExpression!;
        return previousIndex != 0 ?
            result:
            CreateExpressionFromBrackets(mathTypes, expressions, currentOps, tasks);
        
    }
    
    private ExpressionBuilderResult CreateExpressionFromTwoValues<T, V>(T val1, string operation, V val2)
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
            "+" => new ExpressionBuilderResult(Expression.Add(const1 ?? expr1!, const2 ?? expr2!)),
            "-" => new ExpressionBuilderResult(Expression.Subtract(const1 ?? expr1!, const2 ?? expr2!)),
            "*" => new ExpressionBuilderResult(Expression.Multiply(const1 ?? expr1!, const2 ?? expr2!)),
            _ =>  const2 != null && val2 as string == "0" ?
                new ExpressionBuilderResult(MathErrorMessager.DivisionByZero) :
                new ExpressionBuilderResult(Expression.Divide(const1 ?? expr1!, const2 ?? expr2!))
        };
    }
}