using System.Diagnostics.CodeAnalysis;
using Hw10.Dto;
using Hw10.Services;

namespace Cached.Services.CachedCalculator;

public class MathCachedCalculatorService : IMathCalculatorService
{
	private readonly Dictionary<string, double> _cachedResults = new();
	private readonly IMathCalculatorService _simpleCalculator;

	public MathCachedCalculatorService(IMathCalculatorService simpleCalculator)
	{
		_simpleCalculator = simpleCalculator;
	}

	[ExcludeFromCodeCoverage]
	public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
	{
		
		if (_cachedResults.ContainsKey(expression!))
		{
			await Task.Delay(1000);
			var cachedResult = _cachedResults[expression!];
			return new CalculationMathExpressionResultDto(cachedResult);
		}
		var result = await _simpleCalculator.CalculateMathExpressionAsync(expression);
		if (!result.IsSuccess)
			return result;
		
		_cachedResults.Add(expression!, result.Result);
		return result;
	}
}