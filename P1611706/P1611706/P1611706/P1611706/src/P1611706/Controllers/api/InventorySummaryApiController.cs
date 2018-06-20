using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P1611706.Model;
using SmartNetInventory.DataTable;
using System.Net;
using Newtonsoft.Json;
using System.Transactions;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers.api
{
    [Route("api/[controller]")]
    public class InventorySummaryApiController : Controller
    {
        private P1611706DB _db = null;
        public InventorySummaryApiController(P1611706DB db)
        {
            _db = db;
        }
        // GET: api/values
        [HttpGet]
        public JsonResult Get(int draw, int start)
        {
            List<InventoryLists> rows = new List<InventoryLists>();
            string InventoryNo = Request.Query["search[value]"].FirstOrDefault();
            var Should = (from a in _db.vRecords
                          join b in (from w in _db.InventoryBody.Where(e => e.InventoryNo == InventoryNo)
                                     group w by new { w.StockNo } into Stock
                                     select new
                                     {
                                         Stock.Key.StockNo
                                     })
                                     on a.WarehouseNo equals b.StockNo
                          join c in _db.PackingLists
                          on a.BagNo equals c.BagNo
                          group a by new { a.ProductNo, c.CaseNo, c.PalletsNo,a.WarehouseNo } into Bag
                          group Bag by new { Bag.Key.ProductNo, Bag.Key.PalletsNo,Bag.Key.WarehouseNo } into Case
                          select new
                          {
                              Case.Key.ProductNo,
                              PalletsNo = Case.Key.PalletsNo,
                              Qty = Case.Count().ToString(),
                          }).ToList();
            var Actual = (from a in _db.InventoryBody.Where(e => e.InventoryNo == InventoryNo)
                          group a by new { a.PalletsNo, a.CaseNo,a.StockNo } into ActBag
                          group ActBag by new { ActBag.Key.PalletsNo,ActBag.Key.StockNo } into ActCase
                          select new
                          {
                              ActCase.Key.PalletsNo,
                              ActualQty = ActCase.Count().ToString()
                          }).ToList();

            //Full Join 應盤實盤 Linq需兩次Join後再union，不可先裝進Model
            var leftJoin = (from a in Should
                            join b in Actual
                            on a.PalletsNo equals b.PalletsNo into ps
                            from k in ps.DefaultIfEmpty()
                            select new 
                            {

                                ProductNo = a.ProductNo,
                                PalletsNo = a.PalletsNo,
                                Qty = a.Qty,
                                ActualQty = k == null ? "0" : k.ActualQty
                            });

            var rightJoin = (from a in Actual
                             join b in Should
                             on a.PalletsNo equals b.PalletsNo into ps
                             from k in ps.DefaultIfEmpty()
                             select new
                             {

                                 ProductNo = k == null ? "" : k.ProductNo,
                                 PalletsNo = a.PalletsNo,
                                 Qty = k == null ?　"0" : k.Qty,
                                 ActualQty = a.ActualQty
                             });

             var fullJoin = leftJoin.Union(rightJoin);

            IEnumerable<InventoryLists> tmp = (from a in fullJoin
                                               from b in _db.ProductInfoes.Where(e => e.ProductNo == a.ProductNo)
                                               select new InventoryLists
                                               {
                                                   ProductName = b.ProductName + "(" + b.Spec + ")",
                                                   PalletsNo = a.PalletsNo,
                                                   Qty = a.Qty,
                                                   ActualQty = a.ActualQty
                                               });

            rows = tmp.OrderBy(e => e.Qty == e.ActualQty).ToList();
            //資料總數
            int recordsTotal = rows.Count();
            //取得當頁資料



            rows = tmp.OrderBy(e => e.Qty == e.ActualQty).Skip(start).Take(10).ToList();
                 
            foreach(InventoryLists rowResult in rows)
            {
                if(rowResult.ActualQty != rowResult.Qty)
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

        // GET api/values/5
        [HttpGet("{date}")]
        public List<string> Get(string date)
        {
            List<string> listInventoryNo = (from a in _db.InventoryHead
                                   where a.InventoryDate == date
                                   select a.InventoryNo
                                  ).ToList();
            
            return listInventoryNo;
        }

        // POST api/values
        [HttpPost]
        public string Post(string InventoryDate, string InventoryNo)
        {
            try
            {
                InventoryHead Inventory = _db.InventoryHead.Where(e => e.InventoryNo == InventoryNo && e.InventoryDate == InventoryDate).FirstOrDefault();
                string InventoryMan = (from a in _db.InventoryBody.Where(e => e.InventoryNo == InventoryNo) select new { a.InventoryMan }).First().InventoryMan;
                if (Inventory.IsSummary == "N" || Inventory.IsSummary == "0")
                {

                    //應盤項目
                    var Should = (from a in _db.vRecords
                                  join b in (from w in _db.InventoryBody.Where(e => e.InventoryNo == InventoryNo)
                                             group w by new { w.StockNo } into Stock
                                             select new
                                             {
                                                 Stock.Key.StockNo
                                             })
                                             on a.WarehouseNo equals b.StockNo
                                  join c in _db.PackingLists
                                  on a.BagNo equals c.BagNo
                                  select new
                                  {
                                      a.BagNo,
                                      c.CaseNo,
                                      c.PalletsNo,
                                      a.Lot,
                                      StockNo = a.WarehouseNo
                                  }).Distinct().ToList();
                    //實盤項目
                    var Actual = (from a in _db.InventoryBody.Where(e => e.InventoryNo == InventoryNo)

                                  select new
                                  {
                                      a.BagNo,
                                      a.CaseNo,
                                      a.PalletsNo,
                                      a.Lot,
                                      a.StockNo
                                  }).ToList();

                    var Loss = Should.Except(Actual);
                    List<InventoryAdjust> LossAdj = (from a in Loss
                                                     select new InventoryAdjust
                                                     {
                                                         InventoryNo = InventoryNo,
                                                         BagNo = a.BagNo,
                                                         CaseNo = a.CaseNo,
                                                         PalletsNo = a.PalletsNo,
                                                         Lot = a.Lot,
                                                         StockNo = a.StockNo,
                                                         OriQty = 1,
                                                         InventoryQty = 0,
                                                         AdjQty = 1,
                                                         InventoryDate = Inventory.InventoryDate,
                                                         InventoryTime = Inventory.InventoryTime,
                                                         InventoryMan = InventoryMan
                                                     }).ToList();

                    var Surplus = Actual.Except(Should);
                    List<InventoryAdjust> SurplusAdj = (from b in Surplus
                                                        select new InventoryAdjust
                                                        {
                                                            InventoryNo = InventoryNo,
                                                            BagNo = b.BagNo,
                                                            CaseNo = b.CaseNo,
                                                            PalletsNo = b.PalletsNo,
                                                            Lot = b.Lot,
                                                            StockNo = b.StockNo,
                                                            OriQty = 0,
                                                            InventoryQty = 1,
                                                            AdjQty = 1,
                                                            InventoryDate = Inventory.InventoryDate,
                                                            InventoryTime = Inventory.InventoryTime,
                                                            InventoryMan = InventoryMan,
                                                        }).ToList();
                    using (var dbContextTransaction = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (InventoryAdjust Insert_loss in LossAdj)
                            {
                                _db.InventoryAdjust.Add(Insert_loss);
                            }
                            foreach (InventoryAdjust Insert_loss in SurplusAdj)
                            {
                                _db.InventoryAdjust.Add(Insert_loss);
                            }
                            Inventory.IsSummary = "Y";
                            _db.SaveChanges();
                            dbContextTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            return ex.Message;
                        }
                    }
                }
                else
                {
                    return "盤點底稿-" + InventoryNo + "已彙總，無法再彙總!";
                }
            }
            catch(Exception ex)
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
