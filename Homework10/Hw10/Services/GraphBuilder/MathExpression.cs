using System.Linq.Expressions;
namespace Hw10.Services.GraphBuilder;

public record MathExpression(Expression LeftExpression, Expression RightExpression);