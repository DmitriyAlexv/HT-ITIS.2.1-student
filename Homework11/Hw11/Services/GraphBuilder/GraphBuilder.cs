using System.Linq.Expressions;

namespace Hw11.Services.GraphBuilder;

public class GraphBuilder : IGraphBuilder
{
    private readonly Dictionary<Expression, MathExpression> _dependencies = new();

    public Dictionary<Expression, MathExpression> 
        BuildGraph(Expression expression)
    {
        Visit((dynamic)expression);
        return _dependencies;
    }

    private void Visit(ConstantExpression node){}
        
    private void Visit(BinaryExpression node)
    {
        _dependencies.Add(node, new MathExpression(node.Left, node.Right));
        Visit((dynamic)node.Left);
        Visit((dynamic)node.Right);
    }
}