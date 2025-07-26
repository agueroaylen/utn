using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Application.Services
{
    public class OrderService
    {
        private readonly Dsw2025TpiContext _context;

        public OrderService(Dsw2025TpiContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(CreateOrderDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
                throw new NotFoundException("Cliente no encontrado");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = dto.CustomerId,
                ShippingAddress = dto.ShippingAddress,
                BillingAddress = dto.BillingAddress,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var item in dto.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || !product.IsActive)
                    throw new NotFoundException($"Producto no válido: {item.ProductId}");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Stock insuficiente para el producto {product.Name}");

                decimal subtotal = item.Quantity * product.CurrentUnitPrice;
                total += subtotal;

                product.StockQuantity -= item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Quantity = item.Quantity,
                    UnitPrice = product.CurrentUnitPrice,
                    Subtotal = subtotal
                });
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetAllAsync(string? status, Guid? customerId)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            return await query.ToListAsync();
        }

        public async Task<Order> UpdateStatusAsync(Guid id, string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                throw new NotFoundException("Orden no encontrada");

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return order;
        }
    }
}
