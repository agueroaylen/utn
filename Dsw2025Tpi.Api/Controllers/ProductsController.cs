using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;

namespace Dsw2025Tpi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        // POST: Crear producto
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // GET: Obtener todos los productos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            if (!products.Any()) return NoContent();
            return Ok(products);
        }

        // GET: Obtener producto por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            return Ok(product); // Si no existe, lanza excepción
        }

        // PUT: Actualizar producto
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated); // Si no existe, lanza excepción
        }

        // PATCH: Inhabilitar producto
        [HttpPatch("{id}")]
        public async Task<IActionResult> Disable(Guid id)
        {
            await _service.DisableAsync(id); // Si no existe, lanza excepción
            return NoContent();
        }
    }
}
