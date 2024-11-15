using App;
using App.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static App.ProductSpecifications;

var serviceProvider = CreateServiceProvider();

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
foreach (var product in eligibleProducts)
{
    Console.WriteLine(product.Name);
}

return;

static ServiceProvider CreateServiceProvider()
{
    var services = new ServiceCollection();
    services.AddDbContext<AppDbContext>(opt =>
    {
        opt.UseSqlite("Data Source=app.db");
    });

    // create service provider
    var serviceProvider = services.BuildServiceProvider();
    
    // run db migration
    using var scope = serviceProvider.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    return serviceProvider;
}