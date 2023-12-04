namespace Hw11.Services.Parser;

public interface IStringExpressionParser
{
    Task<List<string>> ValidateExpression(string? expression);
}