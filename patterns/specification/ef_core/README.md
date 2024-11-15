## Specification design pattern - EF Core
This demonstrates how to implement the specification design pattern with EF Core  
Example:
```csharp
var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();

var hasProductionDateBetween = (DateTime from, DateTime to) => new HasProductionDateBetweenSpecification(from, to);
var isInCategory = (string cat) => new InCategorySpecification(cat);
var hasPriceLessThan = (decimal price) => new LessThanPriceSpecification(price);

var eligibleProductSpec =
    (isInCategory("Cat A")
        .And(hasPriceLessThan(300)))
    .Or(
        isInCategory("Cat B")
        .And(hasProductionDateBetween(new DateTime(2024, 06, 01), new DateTime(2024, 06, 01))));

var eligibleProducts = appDbContext.Products.Where(eligibleProductSpec.ToExpression()).ToList();
```