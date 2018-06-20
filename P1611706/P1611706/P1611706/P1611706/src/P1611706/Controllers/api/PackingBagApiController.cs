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
    public class PackingBagApiController : BaseController
    {
		private P1611706DB _db = null;

		public PackingBagApiController(P1611706DB db)
		{
			_db = db;
		}

		// GET: api/values
		[HttpGet]
		public JsonResult Get(int draw, int start)
		{
			int bagIndex = 0;
			UserModel _UserModel = null;
			List<PackingBag> rows = new List<PackingBag>();
			string parameter = Request.Query["search[value]"].FirstOrDefault();

			PackingBagCondition condition = null;

			string palletsNo = "";
			string packingTime = "";
			string caseNo = "";

			if (parameter != "")
			{
				condition = (PackingBagCondition)JsonConvert.DeserializeObject(parameter, typeof(PackingBagCondition));
				palletsNo = condition.PalletsNo;
				packingTime = condition.PackingTime;
				caseNo = condition.CaseNo;
			}

			if (ViewData["UserModel"] != null)
			{
				_UserModel = (UserModel)ViewData["UserModel"];

							  //where a.CompanyCode == _UserModel.EmployeeNo 
				var tmp = from a in _db.PackingLists
						  where a.PalletsNo == palletsNo
						  where a.PackingTime.ToString("yyyyMMdd") == packingTime
						  where a.CaseNo == caseNo
						  select new PackingBag
						  {
							  Item = 0,
							  BagNo = a.BagNo
						  };

				//資料總數
				int recordsTotal = tmp.Count();

				//取得當頁資料
				rows = tmp.OrderBy(b => b.BagNo).ToList(); ;

				//計算Item
				foreach (PackingBag bag in rows)
				{
					bagIndex++;
					bag.Item = bagIndex;
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
