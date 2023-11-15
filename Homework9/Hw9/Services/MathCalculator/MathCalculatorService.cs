using System.Linq.Expressions;
using Hw9.Dto;
using Hw9.ErrorMessages;

namespace Hw9.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    private readonly ExpressionBuilder.ExpressionBuilder _expressionBuilder = new();
    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        var buildResult = await _expressionBuilder.CreateExpressionFromString(expression);
        if (!buildResult.IsSuccess)
            return new CalculationMathExpressionResultDto(buildResult.ErrorMessage!);

        var mathResult = ((Func<double>)Expression.Lambda(buildResult.MathExpression!).Compile()).Invoke();
        
        return  Double.IsInfinity(mathResult) ? 
            new CalculationMathExpressionResultDto(MathErrorMessager.DivisionByZero) :
            new CalculationMathExpressionResultDto(mathResult);
    }
}