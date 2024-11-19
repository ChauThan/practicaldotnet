using App.Core;

namespace App.UseCases;

public class GetProductsHandler(IProductRepository productRepository)
{
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await productRepository.FindAsync(new AllProductsSpecification());
    }

    public async Task<IEnumerable<Product>> GetProductsByCategory(string category)
    {
        return await productRepository.FindAsync(new IsInCategorySpecification(category));
    }
}