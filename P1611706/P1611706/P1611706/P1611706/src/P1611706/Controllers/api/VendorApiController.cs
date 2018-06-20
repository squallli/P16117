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
    public class VendorApiController : Controller
    {
        private P1611706DB _db = null;
		private LeaderDemoDB _Ldb = null;

		public VendorApiController(P1611706DB db)
        {
            _db = db;

		}
        // GET: api/values
        [HttpGet]
        public JsonResult Get(int draw, int start)
        {
			//var filteredData = (from v in _db.VendorProducts
			//					join c in _Ldb.CMSMB on v.VendorNo equals c.MB001
			//					select new Vendor {
			//						VendorNo = v.VendorNo,
			//						VendorName = c.MB001
			//					}).AsQueryable();  //get three results




			var filteredData = (from v in _db.VendorProducts
								select new Vendor {
									VendorNo = v.VendorNo
								}).Distinct().AsQueryable();

            string parameter = Request.Query["search[value]"].FirstOrDefault();

            VendorCondition condition = null;
            if (parameter != "")
            {
                condition = (VendorCondition)JsonConvert.DeserializeObject(parameter, typeof(VendorCondition));
            }

            List<Vendor> rows = null;
            if (condition != null)
            {
                if (condition.VendorNo != "")
                    filteredData = filteredData.Where(e => e.VendorNo.Contains(condition.VendorNo));
				

            }

			//var filteredData2 = filteredData.GroupBy(x => x.v.VendorNo)
			//				   .Select(grp => grp.First())
			//				   .ToList();  //get one result

			
			int recordsTotal = filteredData.Count();

			//rows = filteredData.GroupBy(x => x.VendorNo)
			//				   .Select(grp => grp.First()).OrderBy(e => e.VendorNo).Skip(start).Take(15).ToList();

			rows = filteredData.OrderBy(e => e.VendorNo).Skip(start).Take(15).ToList();

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
        public string Post([FromBody]List<ProductVendorSettle> listProductVendorSettl)
        {
            try
            {
                string vendorNo = "";
                if (listProductVendorSettl.Count > 0)
                {
                    vendorNo = listProductVendorSettl[0].VendorNo;

                    IQueryable<VendorProducts> VendorProducts = _db.VendorProducts.Where(x => x.VendorNo == vendorNo).AsQueryable();
                    if (VendorProducts.Count() > 0)
                    {
                        return "已存在的代工廠編號!!";
                    }
                }
                else
                    return "接收設定資料失敗!!";

                foreach (ProductVendorSettle productVendorSettle in listProductVendorSettl)
                {
                    if (productVendorSettle.Manufacture.Equals("1"))
                    {
                        VendorProducts insertVendorProducts = new VendorProducts();
                        insertVendorProducts.VendorNo = vendorNo;
                        insertVendorProducts.ProductNo = productVendorSettle.ProductNo;
                        insertVendorProducts.ProductName = productVendorSettle.ProductName;
                        insertVendorProducts.spec = productVendorSettle.Spec;


                        _db.VendorProducts.Add(insertVendorProducts);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete]
        public string Delete(string VendorNo)
        {
            try
            {
                List<VendorProducts> listVendorProducts = _db.VendorProducts.Where(x => x.VendorNo == VendorNo).ToList();
                foreach (VendorProducts vendorProducts in listVendorProducts)
                {
                    _db.VendorProducts.Remove(vendorProducts);
                }

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }
    }
}
