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
    [Route("api/items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly OFTENCOFTDBContext _context;

        public ItemsController(OFTENCOFTDBContext context)
        {
            _context = context;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Items>>> GetItems()
        {
            return await _context.Items.ToListAsync();
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Items>> GetItems(int id)
        {
            var items = await _context.Items.FindAsync(id);

            if (items == null)
            {
                return NotFound();
            }

            return items;
        }

        // PUT: api/Items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItems(int id, Items items)
        {
            if (id != items.Id)
            {
                return BadRequest();
            }

            _context.Entry(items).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemsExists(id))
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

        // POST: api/Items
        [HttpPost]
        public async Task<ActionResult<Items>> PostItems(Items items)
        {
            //if (!(items.Ticketamount.ToString().Contains("."))) { 
            decimal amount = Math.Round(Convert.ToDecimal(items.Ticketamount), 2);
            //}
            DateTime curdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
            //DateTime nigerianTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(curdate, "W. Central Africa Standard Time");

            items.Ticketamount = amount;
            items.Datecreated = curdate;
            items.Datemodified = items.Datecreated;

            _context.Items.Add(items);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ItemsExists(items.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetItems", new { id = items.Id }, items);
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Items>> DeleteItems(int id)
        {
            var items = await _context.Items.FindAsync(id);
            if (items == null)
            {
                return NotFound();
            }

            _context.Items.Remove(items);
            await _context.SaveChangesAsync();

            return items;
        }

        private bool ItemsExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
