using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OFTENCOFTAPI.Models;
using OFTENCOFTAPI.Models.SendMail;
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
        private readonly ILogger<SelectWinnerController> _logger;

        public SelectWinnerController(OFTENCOFTDBContext context, TicketsController tcontroller, DrawsController drawsController, ILogger<SelectWinnerController> logger )
        {
            _context = context;
            _tController = tcontroller;
            _dController = drawsController;
            _logger = logger;
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
            _logger.LogInformation("Someone just tried to check a ticket");
            //Logger.LogInfo("Ticket Enquiry:::" + winnerRequest.googrand);
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
                var item = await _context.Items.FindAsync(draw.Itemid);

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
                    //send mail
                    // <body style='background-image: url(https://www.dropbox.com/s/labh6yvg1n4t1p7/ba-colorful-row-of-firework-shells-pretty-animated-gif-pic.gif?raw=1);'><p></p>" +
                    //"<img style = 'display:inline-block;width:100%;height:600px;'align='right'src ='https://www.dropbox.com/s/labh6yvg1n4t1p7/ba-colorful-row-of-firework-shells-pretty-animated-gif-pic.gif?raw=1'alt='fireworks'></a>" +

                    string sqlFormattedDate = draw.Drawdate.HasValue
                    ? draw.Drawdate.Value.ToString("dd-MMMM-yyyy")
                    : "<not available>";
                    var subject = "Ticket Request Successful";
                    var body = "";
                    string body2 = @"<!DOCTYPE html>
                        <html>
                        <head>
                        <style>
                        </style>
                        </head>
                        <body>" +
                            "<img style='display:block;width:10%;height:10%;margin-left: auto;margin-right: auto;'src='https://www.dropbox.com/s/0p1flnq0voo7hn9/oftcoftlogosmall.jpg?raw=1'alt='felt lucky'></a>" +
                            "<p></p>"+
                            "<h1 style = 'font-family: Arial, sans-serif; font-size: 185%; color:#000000	;'> Congratulations " + winningrecord.Firstname + "!!!</h1>" +
                             "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> You are the brand new winner of our giveaway with the following details</p>" +
                             "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Draw Date: " + sqlFormattedDate + "</p>" +
                             "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Item: " + item.Itemdescription + "</p>" +
                             "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Please call 08012345678 to claim your prize</p>" +
                             "<p></p>" +
                             "<a href='https://www.nationalgiveaway.com'><img style='display:block; width:100%;height:100%;' src='https://www.dropbox.com/s/medm6f3npfr4gh5/freegift.jpg?raw=1' alt = 'feeling lucky'></a>" +
                             "<img style='display:block;width:10%;height:10%;margin-left: auto;margin-right: auto;' src='https://www.dropbox.com/s/0p1flnq0voo7hn9/oftcoftlogosmall.jpg?raw=1'alt='felt lucky'></a>" +
                             "</body>" +
                             "</html>";
                    EmailSender sender = new EmailSender();
                    try
                    {
                        await sender.Execute2("tonidavis01@gmail.com", subject, body, body2);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                    //end send mail
                    //send text message
                    SendSms sendsms = new SendSms();
                    string phone = winningrecord.Phonenumber.Substring(1, 10);
                    string completephone = "+234" + phone;
                    string smsbody = "Congratulations" + winningrecord.Firstname + ", You are a winner of the following giveaway: " + item.Itemdescription + ". Draw Date: " + sqlFormattedDate + ". Please call 08012345678 to claim your prize ";
                    try
                    {
                        await sendsms.SendSmsMessage(completephone, smsbody);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                    //end send sms
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
