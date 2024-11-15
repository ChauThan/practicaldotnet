namespace App.Core;

public interface IProductRepository
{
    Task<IEnumerable<Product>> FindAsync();
}