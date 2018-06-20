using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartNetInventory.Models
{
    public class DataTablesRequestParameters
    {
        public int draw { set; get; }

        public int? start { set; get; }

        //public int? lenght { set; get; }

        //public int CurrentPageNumber { set; get; }
        //public int CurrentPageNumber
        //{
        //    get
        //    {
        //        if(!start.HasValue || !lenght.HasValue)
        //        {
        //            throw new ArgumentNullException();
        //        }
        //        return (int)(start / lenght);
        //    }
        //}
    }
}
