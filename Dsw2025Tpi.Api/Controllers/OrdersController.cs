using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;



namespace Dsw2025Tpi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;

        public OrdersController(OrderService service)
        {
            _service = service;
        }

        // POST: Crear orden
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        // GET: Todas las órdenes (con filtros)
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] string? status, [FromQuery] Guid? customerId)
        {
            var orders = await _service.GetAllAsync(status, customerId);
            if (!orders.Any()) return NoContent();
            return Ok(orders);
        }

        // GET: Orden por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        // PUT: Cambiar estado
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateStatusAsync(id, dto.NewStatus);
            return Ok(updated);
        }
    }
}
