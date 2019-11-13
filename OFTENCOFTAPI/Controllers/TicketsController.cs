using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OFTENCOFTAPI.Models;

namespace OFTENCOFTAPI.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly OFTENCOFTDBContext _context;

        public TicketsController(OFTENCOFTDBContext context)
        {
            _context = context;

        }

        // GET: api/Tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tickets>>> GetTickets()
        {
            return await _context.Tickets.ToListAsync();
        }

        // GET: api/Tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tickets>> GetTickets(int id)
        {
            var tickets = await _context.Tickets.FindAsync(id);

            if (tickets == null)
            {
                return NotFound();
            }

            return tickets;
        }

        // PUT: api/Tickets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTickets(int id, Tickets tickets)
        {
            if (id != tickets.Id)
            {
                return BadRequest();
            }

            _context.Entry(tickets).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tickets
        [HttpPost]
        public async Task<ActionResult<Tickets>> PostTickets(Tickets tickets)
        {

            tickets.Datemodified = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));

            Random generator = new Random();
            String r = generator.Next(0, 999999).ToString("D6");
            String r2 = generator.Next(0, 99).ToString("D2");

            //var ticketRef = tickets.Firstname.Substring(0, 1).ToUpper() + tickets.Lastname.Substring(0, 1).ToUpper() + DateTime.Now.ToString("ddMMyyHHmmss") + r;
            var ticketRef = tickets.Firstname.Substring(0, 1).ToUpper() + tickets.Lastname.Substring(0, 1).ToUpper() + r2+ DateTime.Now.ToString("ss") + r;

            tickets.Ticketreference = ticketRef;
            tickets.ConfirmStatus = ConfirmStatus.Pending;
            _context.Tickets.Add(tickets);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TicketsExists(tickets.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTickets", new { id = tickets.Id }, tickets);
        }

        // DELETE: api/Tickets/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Tickets>> DeleteTickets(int id)
        {
            var tickets = await _context.Tickets.FindAsync(id);
            if (tickets == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(tickets);
            await _context.SaveChangesAsync();

            return tickets;
        }

        private bool TicketsExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
