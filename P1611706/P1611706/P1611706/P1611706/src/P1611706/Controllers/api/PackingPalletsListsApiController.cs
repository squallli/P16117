using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using P1611706.Model;
using SmartNetInventory.DataTable;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers.api
{
	[Route("api/[controller]")]
    public class PackingPalletsListsApiController : BaseController
    {
		private P1611706DB _db = null;

		public PackingPalletsListsApiController(P1611706DB db)
		{
			_db = db;
		}

		// GET: api/values
		[HttpGet]
		public JsonResult Get(int draw, int start)
		{
			List<PackingPallets> rows = new List<PackingPallets>();
			string[] packingTimeRangeList;
			string packingTimeS = "";
			string packingTimeE = "";
			string parameter = Request.Query["search[value]"].FirstOrDefault();


			PackingPalletsListsCondition condition = null;

			var tmp = (from a in _db.PackingLists
							  orderby a.ProductNo
							  orderby a.PackingTime
					   group a by new { a.CompanyCode, a.ProductNo, a.PalletsNo, a.Lot, a.CaseNo, PackingTime=a.PackingTime.ToString("yyyyMMdd")} into g
							   group g by new { g.Key.CompanyCode, g.Key.ProductNo, g.Key.PalletsNo, g.Key.Lot, g.Key.PackingTime } into k
                       from b in _db.ProductInfoes.Where(e => e.ProductNo == k.Key.ProductNo)
                       select new PackingPallets
					   {
						   CompanyCode = k.Key.CompanyCode,
						   PackingTime = k.Key.PackingTime,
						   ProductName = b.ProductName + "(" + b.Spec + ")",
                           ProductNo = k.Key.ProductNo,
                           PalletsNo = k.Key.PalletsNo,
						   Lot = k.Key.Lot,
						   Quantity = k.Count()
					   });

			if (parameter != "")
			{
				condition = (PackingPalletsListsCondition)JsonConvert.DeserializeObject(parameter, typeof(PackingPalletsListsCondition));
			}

            //如果有搜尋條件
            if (condition != null)
            {

                if (condition.CompanyCode != "")
                {
                    tmp = tmp.Where(e => e.CompanyCode == condition.CompanyCode);
                }

                if (condition.PackingTime != "")
                {
                    tmp = tmp.Where(e => e.PackingTime == condition.PackingTime);
                }

                if (condition.ProductName != "")
                {
                    tmp = tmp.Where(e => e.ProductNo == condition.ProductName);
                }

                if (condition.Lot != "")
                {
                    tmp = tmp.Where(e => e.Lot == condition.Lot);
                }
            }


            //資料總數
            int recordsTotal = rows.Count();

			//取得當頁資料
			rows = tmp.OrderBy(x => x.PalletsNo).OrderBy(x => x.Quantity).Skip(start).Take(10).ToList();
			
			var renderModel = new DataTablesRenderModel
			{
				draw = draw,
				data = rows,
				length = rows.Count(),
				recordsFiltered = recordsTotal,
				recordsTotal = recordsTotal
			};
			return Json(renderModel);
		}

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
			return "value";
        }

		
		// POST api/values
		[HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
