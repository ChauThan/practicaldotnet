using System.Linq.Expressions;
using App.Core.Specifications;

namespace App.Core;

public class AllProductsSpecification : ISpecification<Product>
{
    public Expression<Func<Product, bool>> ToExpression()
    {
        return x => true;
    }
}