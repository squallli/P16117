using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using P1611706.Model;
using SmartNetInventory.DataTable;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers.api
{
    [Route("api/[controller]")]
    public class OrderLogApiController : Controller
    {
		private P1611706DB _db = null;

		public OrderLogApiController(P1611706DB db)
		{
			_db = db;
		}

		// GET: api/values
		[HttpGet]
		public JsonResult Get(int draw, int start)
		{
			string logDateS;
			string logDateE;
			var filteredData = (from a in _db.OrderLog
								select new OrderLogTable
								{
									LogDate = a.LogDate,
									LogTime = a.LogTime,
									OrderType = a.OrderType,
									OrderNo = a.OrderNo,
									WorkType = a.WorkType.ToString(),
									ProductNo = a.ProductNo,
									OriQty = a.OriQty,
									RealQty = a.RealQty
								}).AsQueryable();

			string parameter = Request.Query["search[value]"].FirstOrDefault();

			OrderLogCondition condition = null;
			if (parameter != "")
			{
				condition = (OrderLogCondition)JsonConvert.DeserializeObject(parameter, typeof(OrderLogCondition));
			}

			List<OrderLogTable> rows = null;
			if (condition != null)
			{
				if (condition.LogDate != "")
				{
                    logDateS = condition.LogDate.Substring(0, 10).Replace("/", "").Trim();
                    logDateE = condition.LogDate.Substring(10).Replace("/", "").Trim();

                    filteredData = filteredData.Where(e => Convert.ToInt32(e.LogDate) >= Convert.ToInt32(logDateS) && Convert.ToInt32(e.LogDate) <= Convert.ToInt32(logDateE));
				}

				if (condition.OrderType != "")
					filteredData = filteredData.Where(e => e.OrderType.Contains(condition.OrderType));

				if (condition.OrderNo != "")
					filteredData = filteredData.Where(e => e.OrderNo.Contains(condition.OrderNo));

				if (condition.ProductNo != "")
					filteredData = filteredData.Where(e => e.ProductNo.Contains(condition.ProductNo));
			}

			int recordsTotal = filteredData.Count();

			rows = filteredData.OrderBy(e => e.LogDate).Skip(start).Take(10).ToList();
			
			foreach (OrderLogTable orderlog in rows)
			{
				if (orderlog.WorkType.Equals("2"))
					orderlog.WorkType = "包裝";
				if (orderlog.WorkType.Equals("3"))
					orderlog.WorkType = "出產入庫";
				if (orderlog.WorkType.Equals("4"))
					orderlog.WorkType = "轉撥";
				if (orderlog.WorkType.Equals("5"))
					orderlog.WorkType = "進貨";
				if (orderlog.WorkType.Equals("6"))
					orderlog.WorkType = "銷貨";
				if (orderlog.WorkType.Equals("7"))
					orderlog.WorkType = "上車";
				if (orderlog.WorkType.Equals("8"))
					orderlog.WorkType = "下車";
				if (orderlog.WorkType.Equals("9"))
					orderlog.WorkType = "退貨";
			}

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
