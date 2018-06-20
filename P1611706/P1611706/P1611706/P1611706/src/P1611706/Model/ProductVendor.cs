using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P1611706.Model
{
    public class ProductVendor
	{
        [Key]
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string Spec { get; set; }
        public string Unit { get; set; }
        public int? Capcity { get; set; }
		public int? EffectiveMonth { get; set; }
		public int? EffectiveDay { get; set; }
		public string Barcode { get; set; }
		public bool Manufacture { get; set; }
	}
}
