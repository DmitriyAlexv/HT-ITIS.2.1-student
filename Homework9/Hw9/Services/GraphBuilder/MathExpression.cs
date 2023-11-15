using System.Linq.Expressions;
namespace Hw9.Services.GraphBuilder;

public record MathExpression(Expression LeftExpression, Expression RightExpression);