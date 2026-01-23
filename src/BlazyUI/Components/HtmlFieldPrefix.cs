using System.Linq.Expressions;

namespace BlazyUI.Components;

/// <summary>
/// Represents an HTML field prefix for nested form scenarios.
/// Based on ASP.NET Core's HtmlFieldPrefix implementation.
/// </summary>
internal class HtmlFieldPrefix(LambdaExpression initial)
{
    private readonly LambdaExpression[] _rest = [];

    internal HtmlFieldPrefix(LambdaExpression expression, params LambdaExpression[] rest)
        : this(expression)
    {
        _rest = rest;
    }

    public HtmlFieldPrefix Combine(LambdaExpression other)
    {
        var restLength = _rest?.Length ?? 0;
        var length = restLength + 1;
        var expressions = new LambdaExpression[length];
        for (var i = 0; i < restLength; i++)
        {
            expressions[i] = _rest![i];
        }

        expressions[length - 1] = other;

        return new HtmlFieldPrefix(initial, expressions);
    }

    public string GetFieldName(LambdaExpression expression)
    {
        var prefix = ExpressionFormatter.FormatLambda(initial);
        var restLength = _rest?.Length ?? 0;
        for (var i = 0; i < restLength; i++)
        {
            prefix = ExpressionFormatter.FormatLambda(_rest![i], prefix);
        }

        return ExpressionFormatter.FormatLambda(expression, prefix);
    }
}
