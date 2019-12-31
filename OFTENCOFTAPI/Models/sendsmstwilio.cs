using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace OFTENCOFTAPI.Models
{
    public class SendSms
    {

        private readonly ITwilioRestClient _client;

        public SendSms()
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string _twiltoken = Environment.GetEnvironmentVariable("twilsms");
            string _twilsid = configuration["twiliosid"];
            //string _twiltoken = configuration["twilioauthtoken"];
            string _twilphone = configuration["twiliophone"];
            _client = new TwilioRestClient(_twilsid, _twiltoken);
        }

        public SendSms(ITwilioRestClient client)
        {
            _client = client;
        }

        public async Task SendSmsMessage(string phoneNumber, string message)
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            string _twilphone = configuration["twiliophone"];
            var to = new PhoneNumber(phoneNumber);
            MessageResource.Create(
                to,
                from: new PhoneNumber(_twilphone),
                body: message,
                client: _client);
        }
    }
}
