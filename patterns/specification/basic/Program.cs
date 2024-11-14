using App.Specifications;
using static App.Models;
using static App.ProductSpecifications;

IList<Product> products = [
    new("Product 1", 100, "Cat A", DateTimeOffset.Now.AddDays(-100)),
    new("Product 2", 200, "Cat A", DateTimeOffset.Now.AddDays(-90)),
    new("Product 3", 300, "Cat A", DateTimeOffset.Now.AddDays(-80)),
    new("Product 4", 400, "Cat B", DateTimeOffset.Now.AddDays(-70)),
    new("Product 5", 500, "Cat B", DateTimeOffset.Now.AddDays(-60)),
    new("Product 6", 600, "Cat B", DateTimeOffset.Now.AddDays(-50)),
];

var isInCategory = (string cat) => new InCategorySpecification(cat);
var hasPriceLessThan = (decimal price) => new LessThanPriceSpecification(price);
var hasProductionDateBefore = (int days) => new LessThanProductDateSpecification(DateTimeOffset.Now.AddDays(-days));


var eligibleProductSpecification =
(
    isInCategory("Cat A")
        .And(hasPriceLessThan(300))
        .And(hasProductionDateBefore(90))
)
.Or
(
    isInCategory("Cat A")
        .Not()
        .And(hasProductionDateBefore(50))
);

var eligibleProducts = products.Where(p => eligibleProductSpecification.IsSatisfiedBy(p));
foreach (var product in eligibleProducts)
{
    Console.WriteLine(product.Name);
}