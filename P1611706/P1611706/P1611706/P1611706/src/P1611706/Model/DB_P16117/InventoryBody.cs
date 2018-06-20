using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace P1611706.Model
{
    public class InventoryBody
    {
        
        public string InventoryNo { get; set; }
        public string BagNo { get; set; }
        public string CaseNo { get; set; }
        public string PalletsNo { get; set; }
        public string Lot { get; set; }
        public string StockNo { get; set; }
        public string InventoryDate { get; set; }
        public string InventoryTime { get; set; }
        public string InventoryMan { get; set; }
    }
}
