using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OFTENCOFTAPI.Models;
using OFTENCOFTAPI.Models.ViewModels;

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
            if (drawdetailscount == 0)
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

        //[HttpGet("pick/{ticketid}")]
        [HttpPost("pick")]
        public async Task<ActionResult> GetWinner([FromBody] SelectWinnerRequest winnerRequest)
        {
            //validate first
            Logger.LogInfo("Ticket Enquiry:::" + winnerRequest.googrand);
            //var drawDetails = await _context.Tickets.Where(s => s.Drawid == winnerRequest.drawid && s.ConfirmStatus == ConfirmStatus.Confirmed && s.Winstatus != WinStatus.Won).OrderBy(s => Guid.NewGuid()).ToListAsync();
            var drawDetails = await _context.Tickets.Where(s => s.Drawid == winnerRequest.drawid && s.ConfirmStatus == ConfirmStatus.Confirmed && s.Winstatus != WinStatus.Won).ToListAsync();

            int drawdetailscount = drawDetails.Count();
            int minid = 1;
            int maxid = drawdetailscount;

            if ((winnerRequest.googrand < minid) || winnerRequest.googrand > maxid)
            {
                //return invalid random number
                var data = new
                {
                    status = "fail",
                    winningTicket = "Please enter the correct generated ticket number between the minimum and maximum generated values",
                    message = "Please enter the correct generated ticket number between the minimum and maximum generated values"
                };
                return new JsonResult(data);
            }
            else
            {

                //get generic drawdetails
                             // var ticketDetails = await _context.Tickets.Where(s => s.Drawid == winnerRequest.drawid).ToListAsync();
                //var did = ticketDetails.Drawid;
                var draw = await _context.Draws.FindAsync(winnerRequest.drawid);
                //get status of winners
                //if (draw.) 

                if (draw.Drawstatus == DrawStatus.Drawn)
                {
                    var data = new
                    {
                        status = "fail",
                        winningTicket = "Winners have already been selected for this draw",
                        message = "Winners have already been selected for this draw"
                    };
                    return new JsonResult(data);

                }
                else
                {
                    var winningrecord = drawDetails[winnerRequest.googrand - 1];
                    string winner = winningrecord.Ticketreference.ToString();
                    var data = new
                    {
                        status = "success",
                        winningTicket = winner,
                        message = "Winner Found"
                    };
                    winningrecord.Winstatus = WinStatus.Won;
                    draw.noofwinners += 1;
                    if (draw.noofwinners == draw.drawwinners)
                    {
                        draw.Drawstatus = DrawStatus.Drawn;
                    }
                    else
                    {

                    }

                    await _tController.PutTickets(winningrecord.Id, winningrecord);
                    await _dController.PutDraws(Convert.ToInt32(winnerRequest.drawid), draw);

                   
                    return new JsonResult(data);
                }
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
