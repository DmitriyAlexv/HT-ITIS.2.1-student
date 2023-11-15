using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Hw9.Dto;
using Hw9.ErrorMessages;
using Hw9.Services.GraphBuilder;

namespace Hw9.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    private readonly ExpressionBuilder.ExpressionBuilder _expressionBuilder = new();
    private readonly GraphBuilder.GraphBuilder _graphBuilder = new();
    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        var buildResult = await _expressionBuilder.CreateExpressionFromString(expression);
        if (!buildResult.IsSuccess)
            return new CalculationMathExpressionResultDto(buildResult.ErrorMessage!);

        var graph = _graphBuilder.BuildGraph(buildResult.MathExpression!);
        
        var result = await CalculateAsync(buildResult.MathExpression!, graph);
        
        return  Double.IsInfinity(result) ? 
            new CalculationMathExpressionResultDto(MathErrorMessager.DivisionByZero) :
            new CalculationMathExpressionResultDto(result);
    }
    private static async Task<double> CalculateAsync(Expression current,
        IReadOnlyDictionary<Expression, MathExpression> dependencies)
    {
        if (!dependencies.ContainsKey(current))
        {
            return double.Parse(current.ToString());
        }
        
        await Task.Delay(1000);
        var left = Task.Run(() => 
            CalculateAsync(dependencies[current].LeftExpression, dependencies));
        var right = Task.Run(() => 
            CalculateAsync(dependencies[current].RightExpression, dependencies));

        var results = await Task.WhenAll(left, right);
        return CalculateExpression(results[0], current.NodeType, results[1]);
    }
    
    [ExcludeFromCodeCoverage]
    private static double CalculateExpression(double value1, ExpressionType expressionType, double value2) =>
        expressionType switch
        {
            ExpressionType.Add => value1 + value2,
            ExpressionType.Subtract => value1 - value2,
            ExpressionType.Divide => value2 == 0 ? double.PositiveInfinity : value1 / value2,
            ExpressionType.Multiply => value1 * value2,
            _ => 0
        };
}