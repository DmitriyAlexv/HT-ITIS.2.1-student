using Hw10.DbModels;
using Hw10.Dto;
using Hw10.Services.MathCalculator;
using Microsoft.EntityFrameworkCore;

namespace Hw10.Services.CachedCalculator;

public class MathCachedCalculatorService : IMathCalculatorService
{
	private readonly ApplicationContext _dbContext;
	private readonly IMathCalculatorService _simpleCalculator;

	public MathCachedCalculatorService(ApplicationContext dbContext, IMathCalculatorService simpleCalculator)
	{
		_dbContext = dbContext;
		_simpleCalculator = simpleCalculator;
	}

	public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
	{
		
		if (await _dbContext.SolvingExpressions.AnyAsync(solvingExpression => solvingExpression.Expression == expression))
		{
			await Task.Delay(1000);
			var cachedResult = await _dbContext.SolvingExpressions
				.FirstAsync(solvingExpression => solvingExpression.Expression == expression);
			return new CalculationMathExpressionResultDto(cachedResult.Result);
		}
		var result = await _simpleCalculator.CalculateMathExpressionAsync(expression);
		if (!result.IsSuccess)
			return result;
		
		await _dbContext.SolvingExpressions.AddAsync(new SolvingExpression() { Expression = expression!, Result = result.Result });
		await _dbContext.SaveChangesAsync();
		return result;
	}
}