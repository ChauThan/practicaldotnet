using App.Core;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure;

public class ProductRepository(
    AppDbContext appDbContext,
    IMapper mapper) : IProductRepository
{
    public async Task<IEnumerable<Product>> FindAsync()
    {
        var products= await appDbContext.Products.ToListAsync();
        
        return mapper.Map<List<Product>>(products);
    }
}