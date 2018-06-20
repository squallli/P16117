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
    public class PackingListsApiController : BaseController
    {
        private P1611706DB _db = null;

        public PackingListsApiController(P1611706DB db)
        {
            _db = db;
        }

        // GET: api/values
        [HttpGet]
        public JsonResult Get(int draw, int start)
        {
            UserModel _UserModel = null;
            List<PackingPallets> rows = new List<PackingPallets>();
            List<PackingPallets> rowsResult = new List<PackingPallets>();
            List<PackingPallets> rowsPageResult = new List<PackingPallets>();
            string[] packingTimeRangeList;
            string packingTimeS = "";
            string packingTimeE = "";
            string parameter = Request.Query["search[value]"].FirstOrDefault();

            PackingListsCondition condition = null;

            if (ViewData["UserModel"] != null)
            {
                _UserModel = (UserModel)ViewData["UserModel"];
                tbEmpGroup GroupId = _db.tbEmpGroup.Where(e => e.EmployeeNo == _UserModel.EmployeeNo).FirstOrDefault();
                var tmp = (from a in _db.PackingLists
                           orderby a.ProductNo
                           orderby a.PackingTime
                           group a by new { a.CompanyCode, a.ProductNo, a.PalletsNo, a.Lot, a.CaseNo, PackingTime = a.PackingTime.ToString("yyyyMMdd") } into g
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
                           }).Where(c => c.CompanyCode == GroupId.GroupID);

                if (parameter != "")
                {
                    condition = (PackingListsCondition)JsonConvert.DeserializeObject(parameter, typeof(PackingListsCondition));
                }

                //如果有搜尋條件
                if (condition != null)
                {
                    if (condition.PackingTime.ToString() != "")
                    {
                        packingTimeS = condition.PackingTime.Substring(0, 10).Replace("/", "").Trim();
                        packingTimeE = condition.PackingTime.Substring(10).Replace("/", "").Trim();
                        tmp = tmp.Where(e => Convert.ToInt32(e.PackingTime) >= Convert.ToInt32(packingTimeS) && Convert.ToInt32(e.PackingTime) <= Convert.ToInt32(packingTimeE));
                    }

                    if (condition.ProductName != "")
                    {
                        tmp = tmp.Where(e => e.ProductName.Contains(condition.ProductName));
                    }
                }

                rows = tmp.OrderBy(e => e.CompanyCode).OrderBy(e => e.PackingTime).OrderBy(e => e.ProductName).OrderBy(e => e.Lot).ToList();

                int count = 0;
                PackingPallets newPackingPallets = new PackingPallets();
                foreach (PackingPallets packingPallets in rows)
                {
                    if (count == 0)
                    {
                        newPackingPallets = new PackingPallets();
                        newPackingPallets.CompanyCode = packingPallets.CompanyCode;
                        newPackingPallets.PackingTime = packingPallets.PackingTime;
                        newPackingPallets.ProductName = packingPallets.ProductName;
                        newPackingPallets.ProductNo = packingPallets.ProductNo;
                        newPackingPallets.Lot = packingPallets.Lot;
                        newPackingPallets.PalletsNo = "1";
                        newPackingPallets.Quantity = packingPallets.Quantity;

                        count++;
                        continue;
                    }

                    if (newPackingPallets.CompanyCode.Equals(packingPallets.CompanyCode) &&
                        newPackingPallets.PackingTime.Equals(packingPallets.PackingTime) &&
                        newPackingPallets.ProductName.Equals(packingPallets.ProductName) &&
                        newPackingPallets.Lot.Equals(packingPallets.Lot))
                    {
                        newPackingPallets.PalletsNo = (Convert.ToInt32(newPackingPallets.PalletsNo) + 1).ToString();
                        newPackingPallets.Quantity = newPackingPallets.Quantity + packingPallets.Quantity;
                    }
                    else
                    {
                        rowsResult.Add(newPackingPallets);

                        newPackingPallets = new PackingPallets();
                        newPackingPallets.CompanyCode = packingPallets.CompanyCode;
                        newPackingPallets.PackingTime = packingPallets.PackingTime;
                        newPackingPallets.ProductName = packingPallets.ProductName;
                        newPackingPallets.ProductNo = packingPallets.ProductNo;
                        newPackingPallets.Lot = packingPallets.Lot;
                        newPackingPallets.PalletsNo = "1";
                        newPackingPallets.Quantity = packingPallets.Quantity;
                    }
                }
                if (count != 0)
                    rowsResult.Add(newPackingPallets);


                //資料總數
                int recordsTotal = rowsResult.Count();

                //取得當頁資料
                rowsPageResult = rowsResult.Skip(start).Take(10).ToList(); ;

                var renderModel = new DataTablesRenderModel
                {
                    draw = draw,
                    data = rowsPageResult,
                    length = rowsPageResult.Count(),
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
        public string[] Post()
        {
            var filteredData = _db.ProductInfoes.Select(x => x.ProductName + "(" + x.Spec + ")").ToArray();
            return filteredData;
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
