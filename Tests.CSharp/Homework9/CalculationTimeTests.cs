using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Testing;
using Tests.RunLogic.Attributes;

namespace Tests.CSharp.Homework9;

public class CalculationTimeTests : IClassFixture<WebApplicationFactory<Hw9.Program>>
{
    private readonly HttpClient _client;

    public CalculationTimeTests(WebApplicationFactory<Hw9.Program> fixture)
    {
        _client = fixture.CreateClient();
    }

    [HomeworkTheory(Homeworks.HomeWork9)]
    // уменьшил на 1 тыс, так как можно параллельно выполнить 1 и 3 +
    [InlineData("2 + 3 + 4 + 6", 1990, 3000)]
    // увеличил на 1 тыс, так как у меня 2 ядра (1 - на первую скобку, 2 -  на вторую, а считать внутренности уже синхронно)
    [InlineData("(2 * 3 + 3 * 3) * (5 / 5 + 6 / 6)", 3990, 6000)]
    [InlineData("(2 + 3) / 12 * 7 + 8 * 9", 3990, 5000)]
    private async Task CalculatorController_ParallelTest(string expression, long minExpectedTime, long maxExpectedTime)
    {
        var executionTime = await GetRequestExecutionTime(expression);

        Assert.True(executionTime >= minExpectedTime,
            UserMessagerForTest.WaitingTimeIsLess(minExpectedTime, executionTime));
        Assert.True(executionTime <= maxExpectedTime,
            UserMessagerForTest.WaitingTimeIsMore(maxExpectedTime, executionTime));
    }

    private async Task<long> GetRequestExecutionTime(string expression)
    {
        var watch = Stopwatch.StartNew();
        var response = await _client.PostCalculateExpressionAsync(expression);
        watch.Stop();
        response.EnsureSuccessStatusCode();
        return watch.ElapsedMilliseconds;
    }
}