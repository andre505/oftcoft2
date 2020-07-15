using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OFTENCOFTAPI.ApplicationCore.Models
{
    public partial class Draws
    {
        public Draws()
        {
            Tickets = new HashSet<Tickets>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Itemid { get; set; }
        public int noofwinners { get; set; }

        public int drawwinners { get; set; }

        public DrawType? DrawType { get; set; }
        public DateTime? Drawdate { get; set; }
        public DrawStatus? Drawstatus { get; set; }
        public DateTime? Datecreated { get; set; }
        public DateTime? Datemodified { get; set; }

        public virtual Items Item { get; set; }
        public virtual ICollection<Tickets> Tickets { get; set; }
    }

    public enum DrawType
    {
        Free,
        Paid
    }
    public enum DrawStatus
    {
        Live,
        Drawn
    }
}
