using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OFTENCOFTAPI.ApplicationCore.Models;
using OFTENCOFTAPI.ApplicationCore.Models.ViewModels;
using OFTENCOFTAPI.ApplicationCore.Models.ViewModels.QuickType;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using OFTENCOFTAPI.Services;
using Microsoft.Extensions.Logging;
using OFTENCOFTAPI.Data.Models;

namespace OFTENCOFTAPI.Controllers
{
    // Added this attribute to exclude this controller from being documented by swagger
    // as swagger as issues with documenting it. I'll be fixing the issue later, but for now,
    //I'll just exclude it..
    [ApiExplorerSettings(IgnoreApi = true)]
    
    
    [Route("api/makepayment")]
    [ApiController]
    public class MakePaymentController : Controller
    {
        private readonly OFTENCOFTDBContext _context;
        private readonly TicketsController _tController;
        private readonly ILogger<MakePaymentController> _logger;
        private readonly IEmailService _emailService;



        public MakePaymentController(OFTENCOFTDBContext context, TicketsController tController, ILogger<MakePaymentController> logger, IEmailService emailService)
        {
            _context = context;
            _tController = tController;
            _logger = logger;
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("initialize")]
        public async Task<ActionResult> InitializeTransaction([FromBody] TicketRequest ticketRequest)
        {
            PayStackResponse paystackresponse = new PayStackResponse();
            PayRequest payRequest = new PayRequest();
            var builder = new ConfigurationBuilder()
                                   .SetBasePath(Directory.GetCurrentDirectory())
                                   .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string secretkey = configuration["paystacksk"];
            string endpoint = configuration["paystackitendpoint"];
            string twilsid = configuration["twiliosid"];
            string twiltoken = configuration["twilioauthtoken"];
            string twilphone = configuration["twiliophone"];

            //fetchdraw

            //fetch amount
            //var amount = _context.Items
            //              .Where(u => u.Itemname == ticketRequest.itemname)
            //              .Select(u => u.Ticketamount)
            //              .SingleOrDefault();
            DateTime curentdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
            //get item
            var item = await _context.Items.Where(u => u.Itemname == ticketRequest.itemname).FirstOrDefaultAsync();
            //get draw for item
            var draw = await _context.Draws.Where(u => u.Itemid == item.Id).FirstOrDefaultAsync();
            if (draw.Drawdate < curentdate)
            {
                paystackresponse.Status = "fail";
                paystackresponse.Message = "Oopsy, you are a tiny bit late to this party. Please ensure to enter earlier for any of our Giveaways. All our giveaways end at 19:30";
                paystackresponse.AuthorizationUrl = "null";
                paystackresponse.AccessCode = "null";
                paystackresponse.Reference = "null";
            }
            else
            {

            
                if (draw.DrawType == DrawType.Paid)
                {
                    var totalamount = Convert.ToDecimal(item.Ticketamount) * Convert.ToDecimal(ticketRequest.quantity);
                    //paystack request params
                    payRequest.email = ticketRequest.email;
                    payRequest.amount = totalamount.ToString().Replace(".", "");
                    //payRequest.amount += "00";

                    DateTime curdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
                    //DateTime nigerianTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(curdate, "W. Central Africa Standard Time");

                    HttpClient client = CreateWebRequest();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + secretkey);
                    var resp = await client.PostAsJsonAsync(endpoint, payRequest);

                    if (!resp.IsSuccessStatusCode)
                    {
                        Logger.LogInfo(" Customer Enquiry - error saving response:::" + resp.RequestMessage.ToString());
                        paystackresponse.Status = "fail";
                        paystackresponse.Message = "A Network Error Occurred";
                        paystackresponse.AuthorizationUrl = "null";
                        paystackresponse.AccessCode = "null";
                        paystackresponse.Reference = "null";
                    }
                    else
                    {
                        var res = await resp.Content.ReadAsStringAsync();
                        var result = (TopLevel)JsonConvert.DeserializeObject(res, typeof(TopLevel));
                        //paystackresponse.Status = result.Status.ToString();
                        paystackresponse.Status = "success";
                        paystackresponse.Message = result.Message;
                        paystackresponse.AuthorizationUrl = result.Data.AuthorizationUrl.ToString();
                        paystackresponse.AccessCode = result.Data.AccessCode;
                        paystackresponse.Reference = result.Data.Reference;

                        //saveticket in database
                        //if (paystackresponse.Status == "Success")
                        //{
                        //var tickets = new Tickets();
                        //tickets.Drawid = draw.Id;
                        //tickets.Firstname = ticketRequest.firstname;
                        //tickets.Lastname = ticketRequest.lastname;
                        //tickets.Emailaddress = ticketRequest.email;
                        //tickets.Phonenumber = ticketRequest.phonenumber;


                        //Create tickets * quantity (Validate Method actually creates the ticket references)
                        IList<Tickets> newcustomer = new List<Tickets>();
                        // add to context
                        for (int i = 0; i < Convert.ToInt32(ticketRequest.quantity); i++)
                        {
                            Tickets ticket = new Tickets
                            {
                                Firstname = ticketRequest.firstname,
                                Lastname = ticketRequest.lastname,
                                Emailaddress = ticketRequest.email,
                                Phonenumber = ticketRequest.phonenumber,
                                Drawid = draw.Id,
                                AccessCode = paystackresponse.AccessCode,
                                PaystackReference = paystackresponse.Reference,
                                ConfirmStatus = ConfirmStatus.Pending,
                                Datemodified = curdate,
                                Winstatus = WinStatus.NotWon
                            };
                            newcustomer.Add(ticket);
                        };
                        _context.Tickets.AddRange(newcustomer);

                      //}
                    
                    }     
                    //END PAID
                    await _context.SaveChangesAsync();
                }
                //draw type is free
                else
                {
                    DateTime curdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));

                    //var potentialrecord = await _context.Tickets.Where(x => x.Drawid == draw.Id && x.Firstname == ticketRequest.firstname && x.Lastname == ticketRequest.lastname && x.Phonenumber == ticketRequest.phonenumber).FirstOrDefaultAsync();
                    var potentialrecord = await _context.Tickets.Where(x => x.Drawid == draw.Id && (x.Phonenumber == ticketRequest.phonenumber || x.Emailaddress == ticketRequest.email)).FirstOrDefaultAsync();
                    if (potentialrecord == null)  

                    {
                        //generate ticket ref
                        Random generator = new Random();
                        String r2, r;
                        r2 = generator.Next(0, 99).ToString("D2");
                        r = generator.Next(0, 999999).ToString("D6");
                        var ticketRef = ticketRequest.firstname.Substring(0, 1).ToUpper() + ticketRequest.lastname.Substring(0, 1).ToUpper() + r2 + DateTime.Now.ToString("ss") + r;
                        Tickets ticket = new Tickets
                        {
                            Firstname = ticketRequest.firstname,
                            Lastname = ticketRequest.lastname,
                            Emailaddress = ticketRequest.email,
                            Phonenumber = ticketRequest.phonenumber,
                            Drawid = draw.Id,
                            AccessCode = paystackresponse.AccessCode,
                            PaystackReference = paystackresponse.Reference,
                            ConfirmStatus = ConfirmStatus.Confirmed,
                            Datemodified = curdate,
                            Ticketreference = ticketRef,
                            Winstatus = WinStatus.NotWon
                        };
                        _context.Tickets.Add(ticket);
                        await _context.SaveChangesAsync();
                        //
                        paystackresponse.Status = "success";
                        paystackresponse.Message = "Free ticket successfully procured";
                        paystackresponse.AccessCode = "Free";

                        //send email
                        string sqlFormattedDate = draw.Drawdate.HasValue
                     ? draw.Drawdate.Value.ToString("dd-MMMM-yyyy")
                     : "<not available>";
                        //List<string> stringofticketids = new List<string>();
                        StringBuilder ticketrows = new StringBuilder();
                        //int i = 1;
                        //send

                        ticketrows.Append("<tr><td>1</td> <td>"+ticketRef+ "</td></tr>");
                    

                        var subject = "Ticket Request Successful";
                        //string ticketss = String.Join(",", stringofticketids);
                        var body = "";
                        string body2 = @"<!DOCTYPE html>
                            <html>
                            <head>
                            <style>
                            </style>
                            </head>
                            <body>
                            <img style='display:block;' align='right' src='https://www.dropbox.com/s/0p1flnq0voo7hn9/oftcoftlogosmall.jpg?raw=1' alt = 'felt lucky'></a>" +
                                "<h1 style = 'font-family: Arial, sans-serif; font-size: 250%; color:#9370DB;'> Congratulations!!!" + ticket.Firstname + "</h1>" +
                                 "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> You have successfully entered into the National Giveaways Draw. Find draw details and ticket reference(s) below</p>" +
                                 "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Draw Date: " + sqlFormattedDate + "</p>" +
                                 "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Draw Date: " + item.Itemdescription + "</p>" +
                                 "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Ticket Reference(s)</p>" +
                                 "<table style='border:1px solid #d9d9d9;width:50%;font-family:Gill Sans, sans-serif;text-align:left; font-size: 130%; color:#666666;'>" +
                                 "<tr style='background-color:#595959; color:#FFFFFF'><td>S/N</td><td>Ticket Reference</td></tr> " +

                                 ticketrows +
                                 //
                                 "</table>" +
                                 "<p></p>" +
                                 "<a href='https://www.nationalgiveaway.com'><img style='display:block; width:100%;height:100%;' src='https://www.dropbox.com/s/medm6f3npfr4gh5/freegift.jpg?raw=1' alt = 'feeling lucky'></a>" +
                                 "</body>" +
                                 "</html>";
                        //EmailSender sender = new EmailSender();
                        try
                        {
                            //await sender.Execute2(ticket.Emailaddress, subject, body, body2);
                            await _emailService.ExecuteAsync(ticket.Emailaddress, subject, body, body2);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }

                        //end send email
                    }
                    else
                    {
                        paystackresponse.Status = "fail";
                        paystackresponse.Message = "You have already entered this giveaway";
                    }

                }
            }
            //string JSONresult = JsonConvert.SerializeObject(paystackresponse);               

            return new JsonResult(paystackresponse);
        }
        

        //Paystack calls this
        [HttpPost("validate")]
        public async Task<ActionResult<ChargeSuccessResponse>> HandleCallback()
        {
            var builder = new ConfigurationBuilder()
                                   .SetBasePath(Directory.GetCurrentDirectory())
                                   .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string secretkey = configuration["paystacksk"];
            SendSms sendsms = new SendSms();
            ChargeSuccessResponse paystackchargesuccessresponse = new ChargeSuccessResponse();
            string answer;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                answer = await reader.ReadToEndAsync();
            }

            var jsonanswer = (HookResponse)JsonConvert.DeserializeObject(answer, typeof(HookResponse));
            var amt = jsonanswer.Data.Amount.ToString();
            var normalizedamount = amt.Insert(amt.Length - 2, ".");
            DateTime curdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
            //compare signature  
            //var signature = jsonanswer.Data.Authorization.Signature;
            //byte[] secretkeybytes = Encoding.UTF8.GetBytes(secretkey);
            //var mysignature = HashToString(answer, secretkeybytes);
            paystackchargesuccessresponse.Event = jsonanswer.Event;
            paystackchargesuccessresponse.Status = jsonanswer.Data.Status;
            paystackchargesuccessresponse.Reference = jsonanswer.Data.Reference;
            paystackchargesuccessresponse.Amount = normalizedamount;
            paystackchargesuccessresponse.GatewayResponse = jsonanswer.Data.GatewayResponse;
            paystackchargesuccessresponse.IPAddress = jsonanswer.Data.IpAddress;
            paystackchargesuccessresponse.AuthorizationCode = jsonanswer.Data.Authorization.AuthorizationCode;
            paystackchargesuccessresponse.Signature = jsonanswer.Data.Authorization.Signature;
            paystackchargesuccessresponse.Fees = jsonanswer.Data.Fees.ToString();
            paystackchargesuccessresponse.CustomerID = jsonanswer.Data.Customer.Id.ToString();
            paystackchargesuccessresponse.TimeofPayment = jsonanswer.Data.PaidAt.ToString();
            paystackchargesuccessresponse.Last4 = jsonanswer.Data.Authorization.Last4.ToString();
            paystackchargesuccessresponse.ExpiryMonth = jsonanswer.Data.Authorization.ExpMonth.ToString();
            paystackchargesuccessresponse.ExpiryYear = jsonanswer.Data.Authorization.ExpYear.ToString();
            paystackchargesuccessresponse.Bank = jsonanswer.Data.Authorization.Bank;
            paystackchargesuccessresponse.CardType = jsonanswer.Data.Authorization.CardType;
            paystackchargesuccessresponse.Channel = jsonanswer.Data.Authorization.Channel;
            paystackchargesuccessresponse.CustomerCode = jsonanswer.Data.Customer.CustomerCode;
            paystackchargesuccessresponse.CustomerID = jsonanswer.Data.Customer.Id.ToString();
            paystackchargesuccessresponse.CustomerEmail = jsonanswer.Data.Customer.Email;
            paystackchargesuccessresponse.CountryCode = jsonanswer.Data.Authorization.CountryCode;


            if (paystackchargesuccessresponse.Status == "success")
            {
                //save transaction to Transaction table
                DateTime dateTime = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
                DateTime paymentdate = DateTime.Parse(paystackchargesuccessresponse.TimeofPayment);
                Transaction transaction = new Transaction()
                {
                    TransactionStatus = TransactionStatus.Confirmed,
                    Pspaymentreference = paystackchargesuccessresponse.Reference,
                    IPAddress = paystackchargesuccessresponse.IPAddress,
                    Bank = paystackchargesuccessresponse.Bank,
                    Email = paystackchargesuccessresponse.CustomerEmail,
                    Totalamount = Convert.ToDecimal(paystackchargesuccessresponse.Amount),
                    customercode = paystackchargesuccessresponse.CustomerCode,
                    customerid = paystackchargesuccessresponse.CustomerID,
                    cardchannel = paystackchargesuccessresponse.Channel,
                    cardtype = paystackchargesuccessresponse.CardType,
                    cardlast4 = paystackchargesuccessresponse.Last4,
                    cardexpmonth = paystackchargesuccessresponse.ExpiryMonth,
                    cardexpyear = paystackchargesuccessresponse.ExpiryYear,
                    countrycode = paystackchargesuccessresponse.CountryCode,
                    DateModified = dateTime,
                    Paymentdate = paymentdate,

                };
                _context.Transaction.Add(transaction);

                await _context.SaveChangesAsync();

                //update ticket table with references
                var ticketdetails = await _context.Tickets.Where(s => s.PaystackReference == paystackchargesuccessresponse.Reference).ToListAsync();
                Random generator = new Random();
                String r2;
                String r;


                var tran = await _context.Transaction.Where(s => s.Pspaymentreference == paystackchargesuccessresponse.Reference).FirstOrDefaultAsync();
                foreach (var e in ticketdetails)
                {

                    r2 = generator.Next(0, 99).ToString("D2");
                    r = generator.Next(0, 999999).ToString("D6");
                    var ticketRef = e.Firstname.Substring(0, 1).ToUpper() + e.Lastname.Substring(0, 1).ToUpper() + r2 + DateTime.Now.ToString("ss") + r;
                    e.Ticketreference = ticketRef;
                    e.ConfirmStatus = ConfirmStatus.Confirmed;
                    e.transactionid = tran.Id;
                    e.Datemodified = curdate;
                    await _tController.PutTickets(e.Id, e);
                }
                //Send email to Customers after getting all ticket references
                    //ticket details
                var ticketdetails2 = await _context.Tickets.Where(s => s.PaystackReference == paystackchargesuccessresponse.Reference).ToListAsync();

                    //draw details

                 var drawdetails = await (from p in _context.Draws
                                          join e in _context.Items
                                          on p.Itemid equals e.Id
                                          where p.Id == ticketdetails2[0].Drawid
                                          select new
                                          {
                                              id = p.Id,
                                              itemid = p.Itemid,
                                              drawdate = p.Drawdate,
                                              itemname = e.Itemdescription
                                          }).FirstOrDefaultAsync();
                //normalizedate
                string sqlFormattedDate = drawdetails.drawdate.HasValue
                  ? drawdetails.drawdate.Value.ToString("dd-MMMM-yyyy")
                  : "<not available>";

                //string[] ticket = new string[ticketdetails.Count]; //List<string> TicketReferencesList = new List<string>();//foreach (var e in ticketdetails2)//{ //TicketReferencesList.Add(e.Ticketreference);//}//string[] trs = TicketReferencesList.ToArray();//send email to customer//get params
                
                List<string> stringofticketids = new List<string>();
                StringBuilder ticketrows = new StringBuilder();
                int i=1;
                //send
                foreach (var p in ticketdetails2)
                {
                    stringofticketids.Add(p.Ticketreference);
                    ticketrows.Append("<tr><td style='padding: 0.25rem; text-align: left; border: 1px solid #ccc;'>" + i++.ToString()+ "</td> <td style ='padding: 0.25rem; text-align: left; border: 1px solid #ccc;'>" + p.Ticketreference +"</td></tr>");
                }

                var subject = "Ticket Request Successful";
                string ticketss = String.Join(",", stringofticketids);
                var body = "";
                string body2 = @"<!DOCTYPE html>
                        <html>
                        <head>
                        <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.3/css/bootstrap.min.css' integrity='sha384-Zug+QiDoJOrZ5t4lssLdxGhVrurbmBWopoEl+M6BdEfwnCJZtKxi1KgxUyJq13dy' crossorigin='anonymous'>
                        </head>
                        <body style='font: normal medium/1.4 sans-serif;'>
                        <img style='display:block;width:10%;height:10%;margin-left: auto;margin-right: auto;' src='https://www.dropbox.com/s/0p1flnq0voo7hn9/oftcoftlogosmall.jpg?raw=1' alt = 'felt lucky'></a>" +
                        "<h1 style = 'font-family: Arial, sans-serif; font-size: 185%; color:#000000;'> Congratulations!!!" + ticketdetails2[0].Firstname + "</h1>" +
                         "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> You have successfully entered into the National Giveaways Draw. Find draw details and ticket reference(s) below</p>" +
                         "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Draw Date: " + sqlFormattedDate +"</p>" +
                         "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Draw Date: " + drawdetails.itemname + "</p>" +
                         "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Ticket Reference(s)</p>" +
                         "<table style='border-collapse: collapse; width: 100%;'>" +
                         "<tr style='background-color:#595959; color:#FFFFFF'><th style='padding: 0.25rem; text-align: left; border: 1px solid #ccc;'>S/N</th><th style='padding: 0.25rem; text-align: left; border: 1px solid #ccc;'>Ticket Reference</th></tr>" +
                         //"<tr><td>1</td> <td> ABC12344674HH </td> </tr>" +
                         //"<tr><td>2</td> <td> AHDN3J32K2K22 </td> </tr>" +
                         //"<tr><td>3</td><td> AHDN3J32K2K22 </td> </tr>" +
                         ticketrows+
                         //
                         "</table>" +
                         "<p></p>" +
                         "<a href='https://www.nationalgiveaway.com'><img style='display:block; width:100%;height:100%;' src='https://www.dropbox.com/s/medm6f3npfr4gh5/freegift.jpg?raw=1' alt = 'feeling lucky'></a>" +
                         "</body>" +
                         "</html>";
                //EmailSender sender = new EmailSender();
                try
                {

                    await _emailService.ExecuteAsync(ticketdetails2[0].Emailaddress, subject, body, body2);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                //send text message.
                string phone = ticketdetails2[0].Phonenumber.Substring(1, 10);
                string completephone = "+234" + phone;
                string smsbody = "Hi " + ticketdetails2[0].Firstname + ", you have successfully purchased a ticket for the following item: " + drawdetails.itemname + ". Draw Date: " + drawdetails.drawdate + ". Tickets: " + ticketss;
                try
                {
                    await sendsms.SendSmsMessage(completephone, smsbody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }//end send sms

            }
           // DateTime curdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
            //DateTime nigerianTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(curdate, "W. Central Africa Standard Time");s        
            return paystackchargesuccessresponse;
        }
        //public static async Task<HttpResponseMessage> Run(HttpRequestMessage req)
        //{

        //    dynamic body = await req.Content.ReadAsStringAsync();
        //    log.Info(body);
        //}


        [HttpGet("ticketdetails/{psref}")]
        public async Task<ActionResult<IEnumerable<Draws>>> GetTicketDetails(string psref)
        //public async Task<ActionResult> GetLiveDraws()
        {
            //return await _context.Draws.Where(s => s.Drawstatus == "live").ToListAsync()
            DateTime curdate = DateTime.Parse(DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));
            //DateTime nigerianTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(curdate, "W. Central Africa Standard Time");
            //return await _context.Draws.Include("Items").Where(s => s.Drawdate < curdate && s.Drawstatus == "live").ToListAsync();

            try
            {
                //var ticketts = await (from p in _context.Tickets
                //                      join e in _context.Items
                //                      on p.Itemid equals e.Id
                //                      where p.Drawstatus == DrawStatus.Live && p.Drawdate < curdate
                //                      select new
                //                      {
                //                          id = p.Id,
                //                          itemid = p.Itemid,
                //                          drawdate = p.Drawdate,
                //                          drawstatus = p.Drawstatus,
                //                          itemname = e.Itemname
                //                      }).ToListAsync();

                var ticketts = await (from tr in _context.Transaction
                                      join ti in _context.Tickets on tr.Id equals ti.transactionid
                                      join dr in _context.Draws on ti.Drawid equals dr.Id
                                      join it in _context.Items on dr.Itemid equals it.Id

                                      where tr.Pspaymentreference == psref

                                      select new
                                      {
                                          id = tr.Id,
                                          drawdate = dr.Drawdate,
                                          itemname = it.Itemdescription

                                      }).FirstOrDefaultAsync();



                if (ticketts == null)
                {
                    var data = new
                    {
                        status = "fail",
                        message = "Your payment is processing, you will receive an email with your ticket details once payment has been confirmed"
                    };
                    return new JsonResult(data);

                }
                else
                {
                    var ticketdetails2 = await _context.Tickets.Where(s => s.transactionid == ticketts.id).ToListAsync();
                    List<string> stringoftickets = new List<string>();

                    int i = 1;
                    //send
                    foreach (var p in ticketdetails2)
                    {
                        stringoftickets.Add(p.Ticketreference);
                    }
                    var ticketsstring = string.Join(",", stringoftickets);

                    var data = new
                    {
                        status = "success",
                        tickets = ticketsstring,
                        itemname = ticketts.itemname,
                        drawdate = ticketts.drawdate
                    };
                    //send 


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

        [HttpPost]
        [Route("verify")]
        public async Task<string> PaystackHook()
        {
            string answer;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                answer = await reader.ReadToEndAsync();
            }
            return answer;
        }

        public static HttpClient CreateWebRequest()
        {

            var builder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string paysendpoint = configuration["paystackitendpoint"];

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(paysendpoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));


            return client;
        }

        public static string HashToString(string message, byte[] key)

        {

            byte[] b = new HMACSHA512(key).ComputeHash(Encoding.UTF8.GetBytes(message));

            return Convert.ToBase64String(b);

        }

        public static string GenerateSHA512String(string inputString)
        {
            SHA512 sha512 = SHA512Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}