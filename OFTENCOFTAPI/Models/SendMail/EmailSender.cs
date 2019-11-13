using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Models.SendMail

{
    public class EmailSender
    {
        //static void main (string[] args)
        //{
        //    string to = "tonidavis01@gmail.com";
        //    string subject = "Ticket Purchase Successful";
        //    string body = "< !DOCTYPE html >< html > < head >< style ></ style ></ head >"+
        //" < body >"+
        // "< h1 style = 'font-family: Arial, Helvetica, sans-serif; color: rgb(19, 30, 56)' > Congratulations </ h1 >"+
        //  "< p style = 'font-family: Arial, Helvetica, sans-serif; color:rgb(51, 50, 50)' > You have successfully entered into the National Giveaways Draw</ p >"+
        //      " < p style = 'font-family: Arial, Helvetica, sans-serif; color:rgb(64, 63, 65)'> Your ticket numbers are as follows:</ p >"+
        //            "< h4 style = 'font-family: Arial, Helvetica, sans-serif;' >  </ h4 ></ body ></ html >";

        //    string body2 ="<strong>and easy to do anywhere, even with C#</strong>";

        //    Execute2("tonidavis@gmail.com", subject, "", body).Wait();
        //}

    //static async Task Execute()
    //{
    //    var apiKey = Environment.GetEnvironmentVariable("oftcoftkey");
    //    var client = new SendGridClient(apiKey);
    //    var from = new EmailAddress("test@example.com", "Example User");
    //    var subject = "Sending with SendGrid is Fun";
    //    var to = new EmailAddress("tonidavis01@gmail.com", "Example User");
    //    var plainTextContent = "and easy to do anywhere, even with C#";
    //    var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
    //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
    //    var response = await client.SendEmailAsync(msg);
    //}

    public async Task Execute2(string to, string subject, string plaintext, string Html)
        {
            var apiKey = Environment.GetEnvironmentVariable("oftcoftkey");
            var client = new SendGridClient(apiKey);
            var emailfrom = new EmailAddress("noreply@nationalgiveawaycom", "National Giveaway");
            var emailsubject = subject;
            var emailto = new EmailAddress("tonidavis01@gmail.com", "Yo");
            var plainTextContent = plaintext;
            //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var htmlContent = Html;
            var msg = MailHelper.CreateSingleEmail(emailfrom, emailto, emailsubject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}