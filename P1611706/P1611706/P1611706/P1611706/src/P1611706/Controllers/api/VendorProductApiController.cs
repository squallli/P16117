using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P1611706.Model;
using SmartNetInventory.DataTable;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers.api
{
    [Route("api/[controller]")]
    public class VendorProductApiController : Controller
    {
        private P1611706DB _db = null;

		public VendorProductApiController(P1611706DB db)
        {
            _db = db;
        }
        // GET: api/values
        [HttpGet]
        public JsonResult Get(int draw, int start)
        {
			var filteredData = _db.VendorProducts.AsQueryable();

            string parameter = Request.Query["search[value]"].FirstOrDefault();

            VendorProductCondition condition = null;
            if (parameter != "")
            {
                condition = (VendorProductCondition)JsonConvert.DeserializeObject(parameter, typeof(VendorProductCondition));
            }

            List<VendorProducts> rows = null;
            if (condition != null)
			{
				filteredData = filteredData.Where(e => e.VendorNo == condition.VendorNo);

				if (condition.ProductNo != "")
                    filteredData = filteredData.Where(e => e.ProductNo.Contains(condition.ProductNo));

                if (condition.ProductName != "")
                    filteredData = filteredData.Where(e => e.ProductName.Contains(condition.ProductName));

            }

            int recordsTotal = filteredData.Count();

            rows = filteredData.OrderBy(e => e.VendorNo).Skip(start).Take(10).ToList();

            var renderModel = new DataTablesRenderModel
            {
                draw = draw,
                data = rows,
                length = rows.Count(),
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal
            };
            return Json(renderModel);





            //var filteredData = (from p in _db.VendorProducts

            //                    select new
            //                    {
            //                        p.VendorNo,
            //                        p.ProductNo,
            //                        p.ProductName,
            //                        p.spec
            //                    }).AsQueryable();

            //int recordsTotal = _db.VendorProducts.Count();
            ////var rows = filteredData.OrderBy(e => e.ProductNo).ToList();
            //var rows = filteredData.OrderBy(e => e.VendorNo).OrderBy(s => s.ProductNo).Skip(start).Take(10).ToList();
            //var renderModel = new DataTablesRenderModel
            //{
            //    draw = draw,
            //    data = rows,
            //    length = rows.Count(),
            //    recordsFiltered = recordsTotal,
            //    recordsTotal = recordsTotal
            //};
            //return Json(renderModel);
            
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
