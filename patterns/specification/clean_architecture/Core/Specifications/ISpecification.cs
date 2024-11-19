using System.Linq.Expressions;

namespace App.Core.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> ToExpression();
}