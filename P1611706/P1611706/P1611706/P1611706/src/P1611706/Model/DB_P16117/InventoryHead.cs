using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace P1611706.Model
{
    public class InventoryHead
    {
        [Key]
        public string InventoryNo { get; set; }
        public string InventoryDate { get; set; }
        public string InventoryTime { get; set; }
        public string IsSummary { get; set; }
        public string IsAdjust { get; set; }
    }
}
