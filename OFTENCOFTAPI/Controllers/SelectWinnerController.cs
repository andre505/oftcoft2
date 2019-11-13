using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OFTENCOFTAPI.Models;

namespace OFTENCOFTAPI.Controllers
{
    [Route("api/selectwinner")]
    [ApiController]
    public class SelectWinnerController : ControllerBase
    {
        private readonly TicketsController _tController;
        private readonly DrawsController _dController;
     
        private readonly OFTENCOFTDBContext _context;

        public SelectWinnerController(OFTENCOFTDBContext context, TicketsController tcontroller, DrawsController drawsController )
        {
            _context = context;
            _tController = tcontroller;
            _dController = drawsController;
        }

        [HttpGet("getrange/{drawid}")]
        public async Task<ActionResult> GetRange(int drawid)

        {
            //get tickets where win status for ticket is not null 
            var drawDetails = await _context.Tickets.Where(s => s.Drawid == drawid && s.ConfirmStatus == ConfirmStatus.Confirmed && s.Winstatus!=WinStatus.Won).ToListAsync();
            int drawdetailscount = drawDetails.Count();
            //get minimum id
            if (drawDetails == null)
            {
                var data = new
                {
                    MinimumValue = "0 - No entries for this draw",
                    MaximumValue = "0 - No Entries for this draw"
                };
                return new JsonResult(data);
            }

            else
            {
                //int minid = drawDetails.Min(p => p.Id);
                int minid = 1;
                int maxid = drawdetailscount;
                var data = new
                {

                    MinimumValue = minid,
                    MaximumValue = maxid,
                    iddraw = drawid
                };
                return new JsonResult(data);

            }
        }

        [HttpGet("pick/{ticketid}")]
        [HttpPost("pick/{ticketid}")]
        public async Task<ActionResult> GetWinner(int goognumber, int iddraw )

        {
            
            //get generic drawdetails
            var ticketDetails = await _context.Tickets.Where(s => s.Drawid == iddraw).ToListAsync();
            //var did = ticketDetails.Drawid;
            var draw = await _context.Draws.FindAsync(iddraw);
            //get status of winners
            //if (draw.) 

            if (draw.Drawstatus == DrawStatus.Drawn)
            {
                var data = new
                {
                    status = "fail",
                    winningTicket = "Winners have been selected for this draw",
                    message = "Winners have been selected for this draw"
                };
                return new JsonResult(data);

            }
            else
            {
                var winningrecord = ticketDetails[goognumber - 1];
                string winner = winningrecord.Ticketreference.ToString();
                var data = new
                {
                    status = "success",
                    winningTicket = winner,
                    message = "Winner Found"
                };
                winningrecord.Winstatus = WinStatus.Won;
                draw.drawwinners += 1;
                var updateticketstatus = _tController.PutTickets(goognumber, winningrecord);
                var updatedrawstatus = _dController.PutDraws(Convert.ToInt32(iddraw), draw);

                if (draw.noofwinners == draw.drawwinners)
                {
                    draw.Drawstatus = DrawStatus.Drawn;
                }
                else
                {

                }
                return new JsonResult(data);


            }

        }
        // PUT: api/SelectWinner/5
        [HttpPut("{id}")]
        public void Put(int id, Tickets tickets)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
