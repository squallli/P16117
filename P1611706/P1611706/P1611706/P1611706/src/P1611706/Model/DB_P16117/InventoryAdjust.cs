using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P1611706.Model
{
    public class InventoryAdjust
    {
        public string InventoryNo { get; set; }
        public string BagNo { get; set; }
        public string CaseNo { get; set; }
        public string PalletsNo { get; set; }
        public string Lot { get; set; }
        public string StockNo { get; set; }
        public int OriQty { get; set; }
        public int InventoryQty { get; set; }
        public int AdjQty { get; set; }
        public string InventoryDate { get; set; }
        public string InventoryTime { get; set; }
        public string InventoryMan { get; set; }
    }
}
