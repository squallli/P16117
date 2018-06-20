using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P1611706.Model
{
    public class OrderLog
	{
        public string LogDate { get; set; }
        public string LogTime { get; set; }
		public string OrderType { get; set; }
		public string OrderNo { get; set; }
		public int WorkType { get; set; }
		public string ProductNo { get; set; }
		public int OriQty { get; set; }
		public int RealQty { get; set; }
	}
}
