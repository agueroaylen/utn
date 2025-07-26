using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Data;                  // Tu DbContext real
using Dsw2025Tpi.Domain.Entities;       // Tu modelo Product
using Microsoft.AspNetCore.Authorization;


namespace Dsw2025Tpi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly Dsw2025TpiContext _context;

        public ProductsController(Dsw2025TpiContext context)
        {
            _context = context;
        }

        // 1. Crear un producto
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Sku) || string.IsNullOrEmpty(product.Name))
                return BadRequest("SKU y Nombre son obligatorios.");

            if (product.CurrentUnitPrice <= 0)
                return BadRequest("El precio debe ser mayor a cero.");

            if (product.StockQuantity < 0)
                return BadRequest("El stock no puede ser negativo.");

            var exists = await _context.Products.AnyAsync(p => p.Sku == product.Sku);
            if (exists)
                return BadRequest("Ya existe un producto con ese SKU.");

            product.IsActive = true;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // 2. Obtener todos los productos
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products
                                         .Where(p => p.IsActive)
                                         .ToListAsync();

            if (products.Count == 0)
                return NoContent();

            return Ok(products);
        }

        // 3. Obtener producto por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null || !product.IsActive)
                return NotFound();

            return Ok(product);
        }

        // 4. Actualizar un producto
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, Product updated)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null || !product.IsActive)
                return NotFound();

            if (string.IsNullOrEmpty(updated.Sku) || string.IsNullOrEmpty(updated.Name))
                return BadRequest("SKU y Nombre son obligatorios.");

            if (updated.CurrentUnitPrice <= 0)
                return BadRequest("El precio debe ser mayor a cero.");

            if (updated.StockQuantity < 0)
                return BadRequest("El stock no puede ser negativo.");

            product.Sku = updated.Sku;
            product.InternalCode = updated.InternalCode;
            product.Name = updated.Name;
            product.Description = updated.Description;
            product.CurrentUnitPrice = updated.CurrentUnitPrice;
            product.StockQuantity = updated.StockQuantity;

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // 5. Inhabilitar un producto
        [HttpPatch("{id}")]
        public async Task<IActionResult> DisableProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            product.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
