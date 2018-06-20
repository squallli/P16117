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
	public class ProductVendorApiController : Controller
	{
		private P1611706DB _db = null;

		public ProductVendorApiController(P1611706DB db)
		{
			_db = db;
		}

		// GET: api/values
		[HttpGet]
		public JsonResult Get(int draw, int start)
		{
			List<ProductVendor> rows = null;
			List<VendorProducts> vrows = null;
			var VendorProductsfilteredData = _db.VendorProducts.AsQueryable();
			var filteredData = (from p in _db.ProductInfoes
								select new ProductVendor
								{
									ProductNo = p.ProductNo,
									ProductName = p.ProductName,
									Spec = p.Spec,
									Unit = p.Unit,
									Capcity = p.Capcity,
									EffectiveMonth = p.EffectiveMonth,
									EffectiveDay = p.EffectiveDay,
									Barcode = p.Barcode,
									Manufacture = false
								}
				).AsQueryable();

			string parameter = Request.Query["search[value]"].FirstOrDefault();

			VendorCondition condition = null;
			if (parameter != "")
			{
				condition = (VendorCondition)JsonConvert.DeserializeObject(parameter, typeof(VendorCondition));
			}

			int recordsTotal = filteredData.Count();

			rows = filteredData.OrderBy(e => e.ProductNo).ToList();


			if (condition != null)
			{
				//根據代工廠Table(VendorProducts)，給予ProductVendor.Manufacture是否為true，true代表該代工廠有製造此商品
				VendorProductsfilteredData = VendorProductsfilteredData.Where(e => e.VendorNo == condition.VendorNo);
				vrows = VendorProductsfilteredData.OrderBy(e => e.ProductNo).ToList();

				foreach (VendorProducts vendorProducts in vrows)
				{
					foreach (ProductVendor productVendor in rows)
					{
						if (productVendor.ProductNo.Equals(vendorProducts.ProductNo))
						{
							productVendor.Manufacture = true;
						}
					}
				}
			}

            rows = rows.AsQueryable().OrderByDescending(x => x.Manufacture).ToList();


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

					List<VendorProducts> listVendorProducts = _db.VendorProducts.Where(x => x.VendorNo == vendorNo).ToList();
					foreach (VendorProducts vendorProducts in listVendorProducts)
					{
						_db.VendorProducts.Remove(vendorProducts);
					}

					_db.SaveChanges();
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
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}

	}
}
