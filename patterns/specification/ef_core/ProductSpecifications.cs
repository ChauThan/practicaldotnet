using System.Linq.Expressions;
using App.Specifications;

namespace App;

public static class ProductSpecifications
{
    public class HasProductionDateBetweenSpecification(DateTime from, DateTime to) : ISpecification<Models.Product>
    {
        public Expression<Func<Models.Product, bool>> ToExpression()
        {
            return product => product.ProductionDate >= from
                              && product.ProductionDate <= to;
        }
    }
    public class InCategorySpecification(string category) : ISpecification<Models.Product>
    {
        public Expression<Func<Models.Product, bool>> ToExpression()
        {
            return product => product.Category == category;
        }
    }

    public class LessThanPriceSpecification(decimal price) : ISpecification<Models.Product>
    {
        public Expression<Func<Models.Product, bool>> ToExpression()
        {
            return product => product.Price < price;
        }
    }
}