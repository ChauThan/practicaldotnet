using App.Core.Specifications;

namespace App.Core;

public interface IProductRepository
{
    Task<IEnumerable<Product>> FindAsync(ISpecification<Product> specification);
}