using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P1611706.Model
{
    public class VendorProducts
    {
        
        public string VendorNo { get; set; }
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string spec { get; set; }
    }
}
