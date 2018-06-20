using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P1611706.Model
{
    public class tbProgram
    {
        [Key]
        public string ProgID { get; set; }
        public string ProgName { get; set; }
        public int Power { get; set; }
        public string Url { get; set; }
        public string FlagType { get; set; }

    }
}
