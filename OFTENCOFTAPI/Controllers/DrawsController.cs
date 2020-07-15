using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OFTENCOFTAPI.ApplicationCore.Models;
using OFTENCOFTAPI.Data.Models;

namespace OFTENCOFTAPI.Controllers
{
    [Route("api/draws")]
    [ApiController]
    public class DrawsController : ControllerBase
    {
        private readonly OFTENCOFTDBContext _context;
        private readonly ILogger<DrawsController> _logger;

        public DrawsController(OFTENCOFTDBContext context, ILogger<DrawsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Draws
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Draws>>> GetDraws()
        {
            return await _context.Draws.ToListAsync();
        }

        [HttpGet("livedraws")]
        public async Task<ActionResult<IEnumerable<Draws>>> GetLiveDraws()
        //public async Task<ActionResult> GetLiveDraws()
        {
            //return await _context.Draws.Where(s => s.Drawstatus == "live").ToListAsync()
            DateTime curdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
            //DateTime nigerianTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(curdate, "W. Central Africa Standard Time");
            //return await _context.Draws.Include("Items").Where(s => s.Drawdate < curdate && s.Drawstatus == "live").ToListAsync();

            try
            {
                var draww = await (from p in _context.Draws
                                   join e in _context.Items
                                   on p.Itemid equals e.Id
                                   where p.Drawstatus == DrawStatus.Live && p.Drawdate < curdate
                                   select new
                                   {
                                       id = p.Id,
                                       itemid = p.Itemid,
                                       drawdate = p.Drawdate,
                                       drawstatus = p.Drawstatus,
                                       itemname = e.Itemname
                                   }).ToListAsync();

                if (draww == null)
                {
                    var data = new
                    {
                        status = "fail",
                        message = "There are currently no live draws"
                    };
                    return new JsonResult(data);

                }
                else
                {
                    var data = new
                    {
                        status = "success",
                        draws = draww
                    };

                    return new JsonResult(data);
                }
            }
            catch (Exception ex)
            {
                var data = new
                {
                    status = "fail",
                    message = "An error occurred while trying to access Oftcoft API Draws. Please check your connection or try again later",
                    exception = ex.Message
                };
                _logger.LogError(ex, data.message);
                return new JsonResult(data);

            }
        }

        // GET: api/Draws/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Draws>> GetDraws(int id)
        {
            var draws = await _context.Draws.FindAsync(id);

            if (draws == null)
            {
                return NotFound();
            }

            return draws;
        }

        // PUT: api/Draws/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDraws(int id, Draws draws)
        {
            if (id != draws.Id)
            {
                return BadRequest();
            }

            _context.Entry(draws).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DrawsExists(id))
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

        // POST: api/Draws
        [HttpPost]
        public async Task<ActionResult> PostDraws(Draws draws)
        {

            draws.Datecreated = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            draws.Datemodified = draws.Datecreated;
            draws.Drawdate = Convert.ToDateTime(draws.Drawdate);
            draws.Drawstatus = DrawStatus.Live;
            _context.Draws.Add(draws);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (DrawsExists(draws.Id))
                {
                    var data = new
                    {
                        status = "fail",
                        message = "A draw with the specified ID already exists"
                    };

                    _logger.LogError(ex, data.message, ex.Message);
                    return new JsonResult(data);
                }
                else
                {
                    //throw;
                    var data = new
                    {
                        status = "fail",
                        message = "Draw with ID" + draws.Id + " already exists",
                        exception = ex.Message
                    };

                    _logger.LogError(ex, data.message, ex.Message);
                    return new JsonResult(data);
                }
            }
            _logger.LogInformation("New draw with draw ID" + draws.Id + " created successfully");
            return CreatedAtAction("GetDraws", new { id = draws.Id }, draws);
        }

        // DELETE: api/Draws/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Draws>> DeleteDraws(int id)
        {
            var draws = await _context.Draws.FindAsync(id);
            if (draws == null)
            {
                return NotFound();
            }

            _context.Draws.Remove(draws);
            await _context.SaveChangesAsync();

            return draws;
        }

        private bool DrawsExists(int id)
        {
            return _context.Draws.Any(e => e.Id == id);
        }
    }
}
