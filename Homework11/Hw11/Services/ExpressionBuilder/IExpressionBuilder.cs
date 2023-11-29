using System.Linq.Expressions;

namespace Hw11.Services.ExpressionBuilder;

public interface IExpressionBuilder
{ 
    Task<Expression> CreateExpressionFromString(string? expression);
}