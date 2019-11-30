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
    [Route("api/itemcats")]
    [ApiController]
    public class ItemcategoriesController : ControllerBase
    {
        private readonly OFTENCOFTDBContext _context;

        public ItemcategoriesController(OFTENCOFTDBContext context)
        {
            _context = context;
            //if (_context.Itemcategories.Count() == 0)
            //{
            //    IList<Itemcategories> itemcats = new List<Itemcategories>() {
            //    new Itemcategories() { Categoryname = "Electronics", Categorydescription = "Mobiles Phones, Laptops, TVs, etc" },
            //    new Itemcategories() { Categoryname = "Automobiles", Categorydescription = "Cars, motorcycles, tricycles" },
            //    new Itemcategories() { Categoryname = "Real Estate", Categorydescription = "Duplexes, flats, etc." },
            //    new Itemcategories() { Categoryname = "Holidays", Categorydescription = "Duplexes, flats, etc. " }
            //    };

            //    context.Itemcategories.AddRange(itemcats);
            //    context.SaveChanges();
            //}
        }

        // GET: api/Itemcategories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<Itemcategories>>> GetItemcategories()
        {
            //List<Itemcategories> itemcategories = new List<Itemcategories>();
            var dbcats = await _context.Itemcategories.Select(c => new { c.Id, c.Categoryname }).ToListAsync();
            if (dbcats == null)
            {
                var data = new
                {
                    status = "fail",
                    Message = "No Item Categories Found"
                };
                return new JsonResult(data);
            }
            var data2 = new
            {
                status = "success",
                cats = dbcats
            };
            return new JsonResult(data2);

        }

        // GET: api/Itemcategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Itemcategories>> GetItemcategories(int id)
        {
            var itemcategories = await _context.Itemcategories.FindAsync(id);

            if (itemcategories == null)
            {
                return NotFound();
            }

            return itemcategories;
        }

        // PUT: api/Itemcategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemcategories(int id, Itemcategories itemcategories)
        {
            if (id != itemcategories.Id)
            {
                return BadRequest();
            }

            _context.Entry(itemcategories).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemcategoriesExists(id))
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

        // POST: api/Itemcategories
        [HttpPost]
        public async Task<ActionResult<Itemcategories>> PostItemcategories(Itemcategories itemcategories)
        {

            //string createddate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime curdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
            //DateTime nigerianTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(curdate, "W. Central Africa Standard Time");

            itemcategories.Datecreated = curdate;
            itemcategories.Datemodified = itemcategories.Datecreated;

            _context.Itemcategories.Add(itemcategories);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ItemcategoriesExists(itemcategories.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetItemcategories", new { id = itemcategories.Id }, itemcategories);
        }

        // DELETE: api/Itemcategories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Itemcategories>> DeleteItemcategories(int id)
        {
            var itemcategories = await _context.Itemcategories.FindAsync(id);
            if (itemcategories == null)
            {
                return NotFound();
            }

            _context.Itemcategories.Remove(itemcategories);
            await _context.SaveChangesAsync();

            return itemcategories;
        }

        private bool ItemcategoriesExists(int id)
        {
            return _context.Itemcategories.Any(e => e.Id == id);
        }
    }
}
