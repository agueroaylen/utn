using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;


namespace Dsw2025Tpi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly Dsw2025TpiContext _context;

        public OrdersController(Dsw2025TpiContext context)
        {
            _context = context;
        }

        // 1. Crear una nueva orden
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            // Validar que haya productos
            if (order.OrderItems == null || !order.OrderItems.Any())
                return BadRequest("La orden debe tener al menos un producto.");

            decimal total = 0;

            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null || !product.IsActive)
                    return BadRequest($"Producto con ID {item.ProductId} no encontrado.");

                if (product.StockQuantity < item.Quantity)
                    return BadRequest($"Stock insuficiente para el producto: {product.Name}");

                // Fijar valores históricos en el OrderItem
                item.Name = product.Name;
                item.Description = product.Description;
                item.UnitPrice = product.CurrentUnitPrice;

                // Calcular total acumulado
                total += item.UnitPrice * item.Quantity;

                // Descontar stock
                product.StockQuantity -= item.Quantity;
            }

            order.TotalAmount = total;
            order.CreatedAt = DateTime.UtcNow;
            order.Status = "Pending";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        // 2. Obtener todas las órdenes
        [HttpGet]
        public async Task<IActionResult> GetOrders(
     [FromQuery] string? status,
     [FromQuery] Guid? customerId,
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10)
        {
            var query = _context.Orders.AsQueryable();

            // Filtros
            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            // Paginación
            var totalItems = await query.CountAsync();
            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                pageNumber,
                pageSize,
                totalItems,
                data = orders
            };

            return Ok(result);
        }


        // 4. Cambiar estado de la orden
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        // 3. Obtener orden por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

    }
}
