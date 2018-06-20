using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P1611706.Model
{
    public class PackingLists
	{
        [Key]
        public string BagNo { get; set; }
        public string CaseNo { get; set; }
        public string PalletsNo { get; set; }
		public string CompanyCode { get; set; }
		public string Lot { get; set; }
		public string ProductNo { get; set; }
		public string SerialNo { get; set; }
		public DateTime PackingTime { get; set; }

	}
}
