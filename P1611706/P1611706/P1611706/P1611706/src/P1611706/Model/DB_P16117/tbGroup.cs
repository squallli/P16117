using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P1611706.Model
{
    public class tbGroup
    {
        [Key]
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public int Power { get; set; }
        
    }
}
