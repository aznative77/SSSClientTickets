using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly SssclientContext _context;

        public TicketsController(SssclientContext context)
        {
            _context = context;
        }

        [HttpGet("customers-by-client")]
        public async Task<ActionResult<IEnumerable<object>>> GetCustomersByClient(int clientId)
        {
            var customers = await _context.Customers
                .Where(c => c.ClientRec == clientId)
                .OrderBy(c => c.CustomerName)
                .Select(c => new { value = c.CustomerRec.ToString(), text = c.CustomerName })
                .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("client-hourly-rate")]
        public async Task<ActionResult<object>> GetClientHourlyRate(int clientId)
        {
            var client = await _context.Clients
                .Where(c => c.ClientRec == clientId)
                .Select(c => new { c.HourlyRate })
                .FirstOrDefaultAsync();

            if (client == null)
                return NotFound();

            return Ok(new { hourlyRate = client.HourlyRate });
        }
    }
}
