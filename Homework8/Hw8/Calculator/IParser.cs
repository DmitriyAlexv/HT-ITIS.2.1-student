namespace Hw8.Calculator;

public interface IParser
{
    (double, Operation, double) ParseCalcArguments(string val1, string op, string val2);
}