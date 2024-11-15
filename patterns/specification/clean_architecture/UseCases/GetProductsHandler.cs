using App.Core;

namespace App.UseCases;

public class GetProductsHandler(IProductRepository productRepository)
{
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await productRepository.FindAsync();
    }
}