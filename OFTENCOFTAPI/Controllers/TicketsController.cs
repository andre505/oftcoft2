using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using OFTENCOFTAPI.Models;
using OFTENCOFTAPI.Models.SendMail;

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
            var ticketRef = tickets.Firstname.Substring(0, 1).ToUpper() + tickets.Lastname.Substring(0, 1).ToUpper() + r2 + DateTime.Now.ToString("ss") + r;

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

        [HttpPost("checkticket")]
        public async Task<ActionResult> CheckTicket([FromBody] Tickets ticket)
        {
            var ticketdetails = await _context.Tickets.Where(s => s.Ticketreference == ticket.Ticketreference.ToUpper()).FirstOrDefaultAsync();
            
            //get minimum id
            

            if (ticketdetails == null)
            {
                var data = new
                {
                    status = "success",
                    winnerdetails = "incorrect",
                    message = "Ticket Reference does not exist. Please confirm your ticket number and try again",
                };
                return new JsonResult(data);
               
            }
            else
            {
                //item
                var draw = await _context.Draws.FindAsync(ticketdetails.Drawid);
                DateTime curdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
                var item = await _context.Items.FindAsync(draw.Itemid);
                //formated draw date
                string sqlFormattedDate = draw.Drawdate.HasValue
                   ? draw.Drawdate.Value.ToString("yyyy-MMMM-dd")
                   : "<not available>";

                string itemdesc = item.Itemdescription;

                if (draw.Drawdate > curdate)
                {
                    var data = new
                    {
                        status = "success",
                        winnerdetails = "incorrect",
                        message = "The draw allocated to this ticket reference has not taken place, please check again on" + sqlFormattedDate + " at 9PM.  #What you believe is what you get",
                    };
                    return new JsonResult(data);
                }

                else if (ticketdetails.Winstatus != WinStatus.Won)
                {

                    var data = new
                    {
                        status = "success",
                        winnerdetails = "incorrect",
                        message = "The ticket reference isn't a winner on this ocassion",
                    };
                    return new JsonResult(data);

                }

                else //ticket.winstatus is winstatus.won
                {
                    //string drawdate = draw.Drawdate.ToString("yyyy-MMMM-dd HH:mm:ss");
                   if ((ticket.Emailaddress.ToLower() != ticketdetails.Emailaddress.ToLower()) || (ticket.Firstname.ToUpper() != ticketdetails.Firstname.ToUpper()))
                   {
                        var data = new
                        {
                            status = "success",
                            winnerdetails = "incomplete",
                            message = "Ticket Reference is a Winner",
                            description = "Giveaway Description: " + itemdesc,
                            datewon = "Date Won: " + sqlFormattedDate,
                            personalstatus = "Personal details do not match the winner's details. Please enter correct details if you are the winner"
                        };
                        return new JsonResult(data);
                   }
                   else
                   {
                        var data = new
                        {
                            status = "success",
                            winnerdetails = "correct",
                            message = "Congratulations! Your ticket is a winning ticket. Here are the details:" + " Draw Date: " + sqlFormattedDate + ". Description: " + itemdesc + ". Please check your email for instructions",

                        };
                        //send email to winner
                        var subject = "Ticket Request Successful";
                        var body = "";
                        string body2 = @"<!DOCTYPE html>
                        <html>
                        <head>
                        <style>
                        </style>
                        </head>
                        <body>
                        <img style='display:block;' align='right' src='https://www.dropbox.com/s/0p1flnq0voo7hn9/oftcoftlogosmall.jpg?raw=1' alt = 'felt lucky'></a>" +
                                "<h1 style = 'font-family: Arial, sans-serif; font-size: 250%; color:#9370DB;'> Congratulations!!!</h1>" +
                                 "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> You are the lucky winner of our giveaway with items below</p>" +
                                 "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Draw Date: " + sqlFormattedDate + "</p>" +
                                 "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Item Description:" + itemdesc + "</p>" +
                                 "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Please call 07065024754 to claim your prize</p>" +
                                 "<p></p>" +
                                 "<a href='https://www.nationalgiveaway.com'><img style='display:block; width:100%;height:100%;' src='https://www.dropbox.com/s/medm6f3npfr4gh5/freegift.jpg?raw=1' alt = 'feeling lucky'></a>" +
                                 "</body>" +
                                 "</html>";
                        EmailSender sender = new EmailSender();
                        await sender.Execute2(ticketdetails.Emailaddress, subject, body, body2);
                        //
                        return new JsonResult(data);
                   }                    
                }
            }
        }

    }
}
