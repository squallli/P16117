using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P1611706.Model;
using SmartNetInventory.DataTable;
using System.Net;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers.api
{
    [Route("api/[controller]")]
    public class InventoryPalletsDetailApiController : Controller
    {
        private P1611706DB _db = null;
        public InventoryPalletsDetailApiController(P1611706DB db)
        {
            _db = db;
        }
        // GET: api/values
        [HttpGet]
        public JsonResult Get(int draw, int start)
        {
            List<PalletsDetail> rows = new List<PalletsDetail>();
            InventoryPalletsCondition  condition = null;
            string parameter = Request.Query["search[value]"].FirstOrDefault();
            if (parameter != "")
            {
                condition = (InventoryPalletsCondition)JsonConvert.DeserializeObject(parameter, typeof(InventoryPalletsCondition));
            }
            var Should = (from a in _db.vRecords
                          join b in (from w in _db.InventoryBody.Where(e => e.InventoryNo == condition.InventoryNo)
                                     group w by new { w.StockNo } into Stock
                                     select new
                                     {
                                         Stock.Key.StockNo
                                     })
                          on a.WarehouseNo equals b.StockNo

                          join c in _db.PackingLists.Where(e => e.PalletsNo == condition.PalletsNo)
                          on a.BagNo equals c.BagNo
                          group a by new { c.CaseNo, c.PalletsNo,a.WarehouseNo } into S
                          select new
                          {
                              S.Key.CaseNo,
                              QTY = S.Count().ToString()
                          }).ToList();

            var Actual = (from a in _db.InventoryBody.Where(e => e.InventoryNo == condition.InventoryNo && e.PalletsNo==condition.PalletsNo)
                          group a by new { a.PalletsNo, a.CaseNo,a.StockNo } into ActBag
                          select new
                          {
                              ActBag.Key.CaseNo,
                              ActualQty = ActBag.Count().ToString()
                          }).ToList();
            //Full Join 應盤實盤 Linq需兩次Join後再union，不可先裝進Model
            var leftJoin = (from a in Should
                            join b in Actual
                            on a.CaseNo equals b.CaseNo into ps
                            from g in ps.DefaultIfEmpty()
                            select new 
                            {
                                CaseNo = a.CaseNo,
                                Qty = a.QTY,
                                ActualQty = g == null ? "0" : g.ActualQty
                            });
            var rightJoin = (from a in Actual
                             join b in Should
                             on a.CaseNo equals b.CaseNo into ps
                             from g in ps.DefaultIfEmpty()
                             select new 
                             {
                                 CaseNo = a.CaseNo,
                                 Qty = g == null ? "0" : g.QTY,
                                 ActualQty = a.ActualQty
                             });
            var full = leftJoin.Union(rightJoin);
            
            IEnumerable<PalletsDetail> tmp = (from a in full
                       select new PalletsDetail
                       {
                           CaseNo = a.CaseNo,
                           Qty = a.Qty,
                           ActualQty = a.ActualQty
                       });
            //資料總數
            int recordsTotal = tmp.Count();
            //取得當頁資料
            rows = tmp.OrderBy(e => e.CaseNo).OrderBy(e => e.ActualQty == e.Qty).Skip(start).Take(10).ToList();

            foreach(PalletsDetail rowResult in rows)
            {
                if (rowResult.ActualQty != rowResult.Qty)
                {
                    rowResult.ActualQty = "<span style='color:red'>" + rowResult.ActualQty + "</span>";
                }
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
        

        // POST api/values
        [HttpPost]
        public bool Post(string InventoryNo)
        {
            InventoryHead inventoryHead = _db.InventoryHead.Where(e => e.InventoryNo == InventoryNo).FirstOrDefault();
            if(inventoryHead.IsSummary == "0" || inventoryHead.IsSummary == "N")
            {
                return true;
            }
            else
            {
                return false;
            }
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
