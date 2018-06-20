using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class PackingCaseApiController : BaseController
    {
		private P1611706DB _db = null;

		public PackingCaseApiController(P1611706DB db)
		{
			_db = db;
		}

		// GET: api/values
		[HttpGet]
		public JsonResult Get(int draw, int start)
		{
			int boxIndex = 0;
			UserModel _UserModel = null;
			List<PackingCase> rows = new List<PackingCase>();
			string parameter = Request.Query["search[value]"].FirstOrDefault();

			PackingCaseCondition condition = null;

			string palletsNo = "";
			string packingTime = "";

			if (parameter != "")
			{
					condition = (PackingCaseCondition)JsonConvert.DeserializeObject(parameter, typeof(PackingCaseCondition));
					palletsNo = condition.PalletsNo;
					packingTime = condition.PackingTime;
			}

			if (ViewData["UserModel"] != null)
			{
				_UserModel = (UserModel)ViewData["UserModel"];

							  //where a.CompanyCode == _UserModel.EmployeeNo 
				var tmp = from a in _db.PackingLists
						  where a.PalletsNo == palletsNo
						  where a.PackingTime.ToString("yyyyMMdd") == packingTime
						  group a by a.CaseNo into g
						  select new PackingCase
						  {
							  Item = 0,
							  CaseNo = g.Key,
							  Quantity = g.Count()
						  };

				//資料總數
				int recordsTotal = tmp.Count();

				//取得當頁資料
				rows = tmp.Skip(start).Take(10).ToList(); ;

				//計算Item
				boxIndex = start;
				foreach (PackingCase box in rows)
				{
					boxIndex++;
					box.Item = boxIndex;
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
			else
				return null;
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
