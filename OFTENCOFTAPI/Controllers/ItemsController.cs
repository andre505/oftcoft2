using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OFTENCOFTAPI.Models;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace OFTENCOFTAPI.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly OFTENCOFTDBContext _context;
        private readonly ILogger<ItemsController> _logger;



        public ItemsController(OFTENCOFTDBContext context, ILogger<ItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("fetchitemname/{initialname}")]
        public async Task<ActionResult<IEnumerable<Itemcategories>>> GetItemName(string initialname)
        {
            //List<Itemcategories> itemcategories = new List<Itemcategories>();
            var item = await _context.Items.Select(c => new { c.Id, c.Itemname}).FirstOrDefaultAsync();
            if (item == null)
            {
                int i = 0;
                i++;
                var nulldata = new
                {
                    status = "success",
                    message = "No Item Categories Found",
                    itemname = initialname + i.ToString()
                };
                return new JsonResult(nulldata);
            }

            var data = item.Itemname.Split("-");
            int number = Convert.ToInt32(data[0]);
            number++;
            string finalname = initialname + number.ToString();
            var data2 = new
            {
                status = "success",
                cats = finalname
            };
            return new JsonResult(data2);

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
            catch (DbUpdateException ex)
            {
                if (ItemsExists(items.Id))
                {
                    //return Conflict();
                    var data = new
                    {
                        status = "fail",
                        message = "Item with ID" + items.Id+" already exists",
                        exception = ex.Message
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
                        message = "An error occurred",
                        exception = ex.Message
                    };

                    _logger.LogError(ex, data.message, ex.Message);
                    return new JsonResult(data);
                }
            }
            _logger.LogInformation("New Item" + items.Itemname + " created successfully");
            return CreatedAtAction("GetItems", new { id = items.Id }, items);
        }

        [HttpPost("createitemdraw")]
        public async Task<ActionResult> CreateItemDraw([FromBody] ItemDraw itemdraw)
        {
            List<int> itemids = new List<int>();
            int newitemid = 1;
            //if (!(items.Ticketamount.ToString().Contains("."))) { 
            decimal amount = Math.Round(Convert.ToDecimal(itemdraw.amount), 2);

            DateTime curdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
            //normalize drawdate to 7.30PM - next four lines
            string stringeddatetime = itemdraw.drawdate.ToString();
            string[] splitdate = stringeddatetime.Split("T");
            string completeddatetime = splitdate[0] + "T18:30:00.000Z";
            DateTime drawdatetime = DateTime.Parse(completeddatetime);
            string newitemname;
            var draww = new Draws();
            Items itemm = new Items();
            var itemcheck = await _context.Items.Where(x => x.Itemname.Contains(itemdraw.itemname)).FirstOrDefaultAsync();
            if (itemcheck == null)
            {
                //create new item in the format "iphone11-1(for item name)"
                newitemname = itemdraw.itemname.ToLower() + "-" + newitemid.ToString();
                itemm.Categoryid = itemdraw.catid;
                itemm.Itemdescription = itemdraw.itemdescription;
                itemm.Itemname = newitemname;
                itemm.Ticketamount = amount;
                itemm.Datecreated = curdate;
                itemm.Datemodified = curdate;
            }

            else {
                // retrieve record that has the highest number attached to it.
                 var itemcheck2= await _context.Items.Where(x => x.Itemname.Contains(itemdraw.itemname)).ToListAsync();
                 foreach (var i in itemcheck2)
                 {
                        string[] splititems = i.Itemname.Split('-');
                        itemids.Add(Convert.ToInt32(splititems[1]));
                 }

                newitemid = itemids.Max() + 1;
                newitemname = itemdraw.itemname.ToLower() + "-" +newitemid.ToString();
                itemm.Categoryid = itemdraw.catid;
                itemm.Itemdescription = itemdraw.itemdescription;
                itemm.Itemname = newitemname;
                itemm.Ticketamount = amount;
                itemm.Datecreated = curdate;
                itemm.Datemodified = curdate;
            }
        
           
            _context.Items.Add(itemm);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ItemsExists(itemm.Id))
                {
                    var data = new
                    {
                        status = "fail",
                        message = "An item already exists with the same ID",
                        exception = ex.Message
                    };
                    _logger.LogError(ex, data.message);
                    return new JsonResult(data);
                }
                else
                {
                    var data = new
                    {
                        status = "fail",
                        message = "An error occured while attempting to save new item",
                        exception = ex.Message
                    };
                    _logger.LogError(ex, data.message);
                    return new JsonResult(data);
                    //throw;
                }
            }
            //if get here, then new item create successfully
            _logger.LogInformation("New item with item name" + itemm.Itemname +" created successfully");

            //get newly created item
            var createditem = await _context.Items.Where(x => x.Itemname == itemm.Itemname).FirstOrDefaultAsync();
            //save draw
            draww.DrawType = itemdraw.drawtype;
            //draww.Drawdate = DateTime.Parse(itemdraw.drawdate);
            draww.Drawdate = drawdatetime.AddDays(1);
            draww.Drawstatus = DrawStatus.Live;
            draww.drawwinners = itemdraw.qty;
            draww.Itemid = createditem.Id;
            draww.Datemodified = curdate;
            draww.Datecreated = curdate;

            //add draw
            _context.Draws.Add(draww);
            try
            {

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ItemsExists(draww.Id))
                {
                    var data = new
                    {
                        status = "fail",
                        message = "A draw already exists with the same ID",
                        exception = ex.Message
                    };
                    _logger.LogError(ex, data.message, ex.Message);
                    return new JsonResult(data);
                }
                else
                {
                    var data = new
                    {
                        status = "fail",
                        message = "An error occured while attempting to save new item",
                        exception = ex.Message
                    };
                    _logger.LogError(ex, data.message, ex.Message);
                    return new JsonResult(data);
                }
            }
            _logger.LogInformation("Draw with draw ID" + draww.Id + "created successfully");
            var data2 = new
            {
                status = "Saved! Copy the name shown below into the next field, then click 'Create Giveaway' ",
                message = "Item Created Successfully",
                dbitemname = newitemname
            };
            return new JsonResult(data2);
        
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
