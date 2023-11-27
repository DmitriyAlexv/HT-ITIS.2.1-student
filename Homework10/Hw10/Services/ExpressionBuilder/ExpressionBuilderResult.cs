using System.Linq.Expressions;

namespace Hw10.Services.ExpressionBuilder;

public class ExpressionBuilderResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; set; }
    public Expression? MathExpression { get; set; }
    
    public ExpressionBuilderResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
        IsSuccess = false;
    }

    public ExpressionBuilderResult(Expression mathExpression)
    {
        MathExpression = mathExpression;
        IsSuccess = true;
    }
}