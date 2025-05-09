using Microsoft.EntityFrameworkCore;
using ProductManagementApp.API.Data;
using ProductManagementApp.API.Models;
using ProductManagementApp.API.Repositories.Interfaces;

namespace ProductManagementApp.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string? name, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            return await query.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);

        public async Task AddAsync(Product product)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Product product)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Product product)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
