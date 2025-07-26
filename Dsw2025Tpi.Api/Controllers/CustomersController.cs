using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Dsw2025Tpi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly Dsw2025TpiContext _context;

        public CustomersController(Dsw2025TpiContext context)
        {
            _context = context;
        }

        // GET: /api/customers
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetCustomers()
        {
            return Ok(_context.Customers.ToList());
        }
    }
}
