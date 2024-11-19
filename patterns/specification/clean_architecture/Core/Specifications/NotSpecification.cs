using System.Linq.Expressions;

namespace App.Core.Specifications;

public class NotSpecification<T>(ISpecification<T> spec) : ISpecification<T>
{
    public Expression<Func<T, bool>> ToExpression()
    {
        var expression = spec.ToExpression();
        var parameter = Expression.Parameter(typeof(T));

        var notExpression = Expression.Not(Expression.Invoke(expression, parameter));
        
        return Expression.Lambda<Func<T, bool>>(notExpression, parameter);
    }
}