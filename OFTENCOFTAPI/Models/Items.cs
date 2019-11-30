using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OFTENCOFTAPI.Models
{
    public partial class Items
    {
        public Items()
        {
            Draws = new HashSet<Draws>();
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Itemname { get; set; }
        public string Itemdescription { get; set; }
        public int Categoryid { get; set; }
        public decimal? Ticketamount { get; set; }
        public DateTime? Datecreated { get; set; }
        public DateTime? Datemodified { get; set; }

        public virtual Itemcategories Category { get; set; }
        public virtual ICollection<Draws> Draws { get; set; }
    }
}
