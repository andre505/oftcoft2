using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OFTENCOFTAPI.Models
{
    public partial class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public string customercode { get; set; }
        public string Pspaymentreference { get; set; }
        //new fields
        public string Email { get; set; }
        public string TicketReferences { get; set; }
        public string customerid { get; set; }
        public string cardlast4 { get; set; }
        public string cardexpmonth { get; set; }
        public string cardexpyear { get; set; }
        public string cardchannel { get; set; }
        public string cardtype { get; set; }
        public string countrycode { get; set; }
        public string IPAddress { get; set; }
        public string Location { get; set; }
        public string Bank { get; set; }
        public TransactionStatus? TransactionStatus { get; set; }

        public decimal? Totalamount { get; set; }
        public DateTime? Paymentdate { get; set; }

        public DateTime? DateModified { get; set; }


        public virtual ICollection<Tickets> Tickets { get; set; }
    }
    public enum TransactionStatus
    {
        Confirmed,
        Pending
    }
}
