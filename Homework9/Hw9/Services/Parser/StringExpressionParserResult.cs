namespace Hw9.Services.Parser;

public class StringExpressionParserResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; set; }
    public List<string>? MathExpression { get; set; }
    
    public StringExpressionParserResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
        IsSuccess = false;
    }

    public StringExpressionParserResult(List<string> mathExpression)
    {
        MathExpression = mathExpression;
        IsSuccess = true;
    }
}