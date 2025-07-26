using Dsw2025Tpi.Data;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Application.Services
{
    public class ProductService
    {
        private readonly Dsw2025TpiContext _context;

        public ProductService(Dsw2025TpiContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateAsync(ProductCreateDto dto)
        {
            var exists = await _context.Products.AnyAsync(p => p.Sku == dto.Sku);
            if (exists)
                throw new Exception("Ya existe un producto con ese SKU.");

            var product = new Product
            {
                Sku = dto.Sku,
                InternalCode = dto.InternalCode,
                Name = dto.Name,
                Description = dto.Description,
                CurrentUnitPrice = dto.CurrentUnitPrice,
                StockQuantity = dto.StockQuantity,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
            if (product == null)
                throw new NotFoundException("Producto no encontrado");

            return product;
        }

        public async Task<Product> UpdateAsync(Guid id, ProductUpdateDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive)
                throw new NotFoundException("Producto no encontrado para actualizar");

            product.Sku = dto.Sku;
            product.InternalCode = dto.InternalCode;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.CurrentUnitPrice = dto.CurrentUnitPrice;
            product.StockQuantity = dto.StockQuantity;

            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DisableAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new NotFoundException("Producto no encontrado para desactivar");

            product.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
