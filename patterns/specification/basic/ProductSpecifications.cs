using App.Specifications;

namespace App;

public static class ProductSpecifications
{
    public class LessThanProductDateSpecification(DateTimeOffset date) : ISpecification<Models.Product>
    {
        public bool IsSatisfiedBy(Models.Product item) => item.ProductionDate.Date < date.Date;
    }

    public class InCategorySpecification(string category) : ISpecification<Models.Product>
    {
        public bool IsSatisfiedBy(Models.Product item) => item.Category == category;
    }

    public class LessThanPriceSpecification(decimal price) : ISpecification<Models.Product>
    {
        public bool IsSatisfiedBy(Models.Product item) => item.Price < price;
    }
}