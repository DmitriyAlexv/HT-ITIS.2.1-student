using System.Diagnostics.CodeAnalysis;
using Hw8.Calculator;
using Microsoft.AspNetCore.Mvc;

namespace Hw8.Controllers;

public class CalculatorController : Controller
{
    private readonly ICalculator _calculator;
    private readonly IParser _parser;

    public CalculatorController(ICalculator calculator, IParser parser)
    {
        _calculator = calculator;
        _parser = parser;
    }
    public ActionResult<double> Calculate(
        string val1,
        string operation,
        string val2)
    {
            var calcArgs = _parser.ParseCalcArguments(val1, operation, val2);
            return calcArgs switch
            {
                (var value1, Operation.Plus, var value2) => _calculator.Plus(value1, value2),
                (var value1, Operation.Minus, var value2) => _calculator.Minus(value1, value2),
                (var value1, Operation.Multiply, var value2) => _calculator.Multiply(value1, value2),
                _ => _calculator.Divide(calcArgs.Item1, calcArgs.Item3)
            };
    }
    
    [ExcludeFromCodeCoverage]
    public IActionResult Index()
    {
        return Content(
            "Заполните val1, operation(plus, minus, multiply, divide) и val2 здесь '/calculator/calculate?val1= &operation= &val2= '\n" +
            "и добавьте её в адресную строку.");
    }
}