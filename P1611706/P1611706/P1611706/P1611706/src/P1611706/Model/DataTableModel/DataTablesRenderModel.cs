using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartNetInventory.DataTable
{
    public class DataTablesRenderModel
    {
        public int draw { set; get; }

        public int length { set; get; }

        public int recordsTotal {set;get;}

        public int recordsFiltered { get; set; }

        public object data { set; get; }
    }
}
