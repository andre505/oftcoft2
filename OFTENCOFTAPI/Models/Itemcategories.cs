using System;
using System.Collections.Generic;

namespace OFTENCOFTAPI.Models
{
    public partial class Itemcategories
    {
        public Itemcategories()
        {
            Items = new HashSet<Items>();
        }

        public int Id { get; set; }
        public string Categoryname { get; set; }
        public string Categorydescription { get; set; }
        public DateTime? Datecreated { get; set; }
        public DateTime? Datemodified { get; set; }

        public virtual ICollection<Items> Items { get; set; }
    }
}
