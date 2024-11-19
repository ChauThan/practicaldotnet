using System.Linq.Expressions;

namespace App.Core.Specifications;

public class AndSpecification<T>(
    ISpecification<T> left, 
    ISpecification<T> right) 
    : ISpecification<T>
{
    public Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = left.ToExpression(); 
        var rightExpression = right.ToExpression(); 
        var parameter = Expression.Parameter(typeof(T)); 
        
        var combined = Expression.AndAlso( 
            Expression.Invoke(leftExpression, parameter), 
            Expression.Invoke(rightExpression, parameter) ); 
        
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}