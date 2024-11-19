using App.Core;
using App.Core.Specifications;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure;

public class ProductRepository(
    AppDbContext appDbContext,
    IMapper mapper) : IProductRepository
{
    public async Task<IEnumerable<Product>> FindAsync(ISpecification<Product> specification)
    {
        var entitySpec = new SpecificationConverter(mapper).Convert(specification);
        
        var products= await appDbContext.Products.Where(entitySpec.ToExpression()).ToListAsync();
        
        return mapper.Map<List<Product>>(products);
    }
}