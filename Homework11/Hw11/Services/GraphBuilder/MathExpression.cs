using System.Linq.Expressions;
namespace Hw11.Services.GraphBuilder;

public record MathExpression(Expression LeftExpression, Expression RightExpression);