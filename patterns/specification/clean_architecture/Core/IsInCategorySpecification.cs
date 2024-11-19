using System.Linq.Expressions;
using App.Core.Specifications;

namespace App.Core;

public class IsInCategorySpecification(string category) : ISpecification<Product>
{
    public Expression<Func<Product, bool>> ToExpression()
    {
        return product => product.Category == category;
    }
}