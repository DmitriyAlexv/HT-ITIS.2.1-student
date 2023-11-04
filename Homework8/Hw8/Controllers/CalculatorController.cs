using System.Diagnostics.CodeAnalysis;
using Hw8.Calculator;
using Microsoft.AspNetCore.Mvc;

namespace Hw8.Controllers;

public class CalculatorController : Controller
{
    public ActionResult<double> Calculate([FromServices] ICalculator calculator,
        string val1,
        string operation,
        string val2)
    {
        try
        {
            var calcArgs = Parser.ParseCalcArguments(val1, operation, val2);
            return calcArgs switch
            {
                (var value1, Operation.Plus, var value2) => calculator.Plus(value1, value2),
                (var value1, Operation.Minus, var value2) => calculator.Minus(value1, value2),
                (var value1, Operation.Multiply, var value2) => calculator.Multiply(value1, value2),
                (var value1, Operation.Divide, 0d) => this.Content(Messages.DivisionByZeroMessage),
                _ => calculator.Divide(calcArgs.Item1, calcArgs.Item3)
            };
        }
        catch (ArgumentException)
        {
            return this.Content(Messages.InvalidNumberMessage);
        }
        catch
        {
            return this.Content(Messages.InvalidOperationMessage);
        }
    }
    
    [ExcludeFromCodeCoverage]
    public IActionResult Index()
    {
        return Content(
            "Заполните val1, operation(plus, minus, multiply, divide) и val2 здесь '/calculator/calculate?val1= &operation= &val2= '\n" +
            "и добавьте её в адресную строку.");
    }
}