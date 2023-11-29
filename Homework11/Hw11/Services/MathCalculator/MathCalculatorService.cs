using System.Linq.Expressions;
using Hw11.Services.GraphBuilder;
using System.Diagnostics.CodeAnalysis;
using Hw11.ErrorMessages;
using Hw11.Services.ExpressionBuilder;

namespace Hw11.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    private readonly IExpressionBuilder _expressionBuilder;
    private readonly IGraphBuilder _graphBuilder;

    public MathCalculatorService(IExpressionBuilder expressionBuilder, IGraphBuilder graphBuilder)
    {
        _expressionBuilder = expressionBuilder;
        _graphBuilder = graphBuilder;
    }
    
    public async Task<double> CalculateMathExpressionAsync(string? expression)
    {
        var buildResult = await _expressionBuilder.CreateExpressionFromString(expression);

        var graph = _graphBuilder.BuildGraph(buildResult);
        
        var result = await CalculateAsync(buildResult, graph);
        
        return  Double.IsInfinity(result) ? 
            throw new DivideByZeroException(MathErrorMessager.DivisionByZero):
            result;
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