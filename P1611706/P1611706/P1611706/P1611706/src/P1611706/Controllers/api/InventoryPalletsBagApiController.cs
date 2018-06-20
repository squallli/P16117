using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P1611706.Model;
using SmartNetInventory.DataTable;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers.api
{
    [Route("api/[controller]")]
    public class InventoryPalletsBagApiController : Controller
    {
        private P1611706DB _db = null;
        public InventoryPalletsBagApiController(P1611706DB db)
        {
            _db = db;
        }
        // GET: api/values
        [HttpGet]
        public JsonResult Get(int draw, int start)
        {
            List<InventoryPalletsBag> rows = new List<InventoryPalletsBag>();

            string condition = Request.Query["search[value]"].FirstOrDefault();
            int bagIndex = 0;
            List<InventoryPalletsBag> tmp = (from a in _db.InventoryBody.Where(e => e.CaseNo == condition).AsEnumerable()
                                             select new InventoryPalletsBag
                                             {
                                                 Item = 0,
                                                 Bag = a.BagNo
                                             }).OrderBy(e => e.Bag).ToList();
            
            //資料總數
            int recordsTotal = tmp.Count();
            //取得當頁資料
            //計算Item
            foreach (InventoryPalletsBag bag in tmp)
            {
                bagIndex++;
                bag.Item = bagIndex;
            }
            rows = tmp.Skip(start).Take(10).ToList();

            
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
        public string Post(string InventoryDate, string InventoryNo,string PalletsNo, string CaseNo, string BagNo)
        {
            try
            {
                IEnumerable<InventoryBody> InventoryBag = _db.InventoryBody.Where(e => e.InventoryNo == InventoryNo && e.BagNo == BagNo).AsEnumerable() ;
                if(InventoryBag.Count() > 0)
                {
                    return "新增失敗，已盤過此袋號!";
                }
                PackingLists PackingLists = _db.PackingLists.Where(e => e.BagNo == BagNo).FirstOrDefault();
                if(PackingLists == null)
                {
                    return "新增失敗，不合法的袋子!";
                }
                PackingLists = _db.PackingLists.Where(e => e.PalletsNo == PalletsNo && e.CaseNo == CaseNo).First();
                InventoryBody InventoryPallets = _db.InventoryBody.Where(e => e.InventoryNo == InventoryNo).FirstOrDefault();
                if (true)
                {
                    InventoryBody Add_Bag = new InventoryBody();
                    Add_Bag.InventoryNo = InventoryNo;
                    Add_Bag.BagNo = BagNo;
                    Add_Bag.CaseNo = CaseNo;
                    Add_Bag.PalletsNo = PalletsNo;
                    Add_Bag.Lot = PackingLists.Lot;
                    Add_Bag.StockNo = InventoryPallets.StockNo;
                    Add_Bag.InventoryDate = DateTime.Now.ToString("yyyyMMdd");
                    Add_Bag.InventoryTime = DateTime.Now.ToString("hhmmss");
                    Add_Bag.InventoryMan = InventoryPallets.InventoryMan;

                    _db.InventoryBody.Add(Add_Bag);
                    _db.SaveChanges();
                }
                
                return "success";

            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete]
        public string  Delete(string InventoryNo, string CaseNo, string BagNo)
        {
            try
            {
                InventoryBody Del_InventoryBody = _db.InventoryBody.Where(e => e.InventoryNo == InventoryNo && e.CaseNo == CaseNo && e.BagNo == BagNo).First();
                _db.InventoryBody.Remove(Del_InventoryBody);
                _db.SaveChanges();
                return "success";


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
