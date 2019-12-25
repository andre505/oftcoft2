﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Models
{
    public partial class Logs
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string Application { get; set; }
		public DateTime? Logged { get; set; }
		public string Level { get; set; }
		public string Message { get; set; }
		public string Logger { get; set; }
		public string Callsite { get; set; }
		public string Exception { get; set; }
	}
}
