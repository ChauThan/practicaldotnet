using System.Linq.Expressions;

namespace App.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> ToExpression();
}