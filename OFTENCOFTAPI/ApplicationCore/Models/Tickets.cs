using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OFTENCOFTAPI.ApplicationCore.Models
{
    public partial class Tickets

    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? Drawid { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Emailaddress { get; set; }
        public string Phonenumber { get; set; }
        public string Ticketreference { get; set; }
        public string AccessCode { get; set; }
        public string PaystackReference { get; set; }
        public WinStatus? Winstatus { get; set; }
        public ClaimStatus? Claimstatus { get; set; }
        //new fields
        public int? transactionid { get; set; }
        public DateTime? Datemodified { get; set; }
        public virtual Draws Draw { get; set; }
        public virtual Transaction Transaction { get; set; }
        public ConfirmStatus? ConfirmStatus { get; set; }
    }

    public enum ConfirmStatus
    {
        Pending,
        Confirmed
    }

    public enum WinStatus
    {
      Won,
      NotWon
    }

    public enum ClaimStatus
    {
        Claimed,
        Unclaimed
    }
}
