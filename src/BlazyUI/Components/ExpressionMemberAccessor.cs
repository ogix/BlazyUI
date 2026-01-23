using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazyUI.Components;

/// <summary>
/// Provides methods for extracting metadata from member access expressions.
/// Based on ASP.NET Core's ExpressionMemberAccessor implementation.
/// </summary>
internal static class ExpressionMemberAccessor
{
    private static readonly ConcurrentDictionary<Expression, MemberInfo> MemberInfoCache = new();
    private static readonly ConcurrentDictionary<MemberInfo, string> DisplayNameCache = new();

    private static MemberInfo GetMemberInfo<TValue>(Expression<Func<TValue>> accessor)
    {
        ArgumentNullException.ThrowIfNull(accessor);

        return MemberInfoCache.GetOrAdd(accessor, static expr =>
        {
            var lambdaExpression = (LambdaExpression)expr;
            var accessorBody = lambdaExpression.Body;

            if (accessorBody is UnaryExpression unaryExpression
                && unaryExpression.NodeType == ExpressionType.Convert
                && unaryExpression.Type == typeof(object))
            {
                accessorBody = unaryExpression.Operand;
            }

            if (accessorBody is not MemberExpression memberExpression)
            {
                throw new ArgumentException(
                    $"The provided expression contains a {accessorBody.GetType().Name} which is not supported. " +
                    $"Only simple member accessors (fields, properties) of an object are supported.");
            }

            return memberExpression.Member;
        });
    }

    public static string GetDisplayName(MemberInfo member)
    {
        ArgumentNullException.ThrowIfNull(member);

        return DisplayNameCache.GetOrAdd(member, static m =>
        {
            var displayAttribute = m.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute is not null)
            {
                var name = displayAttribute.GetName();
                if (name is not null)
                {
                    return name;
                }
            }

            var displayNameAttribute = m.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttribute?.DisplayName is not null)
            {
                return displayNameAttribute.DisplayName;
            }

            return m.Name;
        });
    }

    public static string GetDisplayName<TValue>(Expression<Func<TValue>> accessor)
    {
        ArgumentNullException.ThrowIfNull(accessor);
        var member = GetMemberInfo(accessor);
        return GetDisplayName(member);
    }
}
