using System.Linq.Expressions;

namespace App.Core.Specifications;

public class OrSpecification<T>(
    ISpecification<T> left, 
    ISpecification<T> right) 
    : ISpecification<T>
{
    public bool IsSatisfiedBy(T item) => left.IsSatisfiedBy(item) || right.IsSatisfiedBy(item);
    public Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = left.ToExpression(); 
        var rightExpression = right.ToExpression(); 
        var parameter = Expression.Parameter(typeof(T));
        
        var combined = Expression.OrElse( 
            Expression.Invoke(leftExpression, parameter), 
            Expression.Invoke(rightExpression, parameter) ); 
        
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}