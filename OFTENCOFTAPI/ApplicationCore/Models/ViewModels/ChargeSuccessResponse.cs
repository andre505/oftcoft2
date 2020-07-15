using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.Models.ViewModels
{
    public class ChargeSuccessResponse
    {
        public string Event { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string Amount { get; set; }
        public string GatewayResponse { get; set; }
        public string IPAddress { get; set; }
        public string AuthorizationCode { get; set; }
        public string Signature { get; set; }
        public string Fees { get; set; }
        public string CustomerID { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCode { get; set; }
        public string TimeofPayment { get; set; }
        public string Last4 { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Channel { get; set; }
        public string CardType { get; set; }
        public string Bank { get; set; }
        public string CountryCode { get; set; }


    }
}
