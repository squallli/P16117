using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P1611706.Model;
using SmartNetInventory.DataTable;
using System.Net;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers.api
{
    [Route("api/[controller]")]
    public class GroupApiController : Controller
    {
        private P1611706DB _db = null;

        public GroupApiController(P1611706DB db)
        {
            _db = db;
        }
        // GET: api/values
        [HttpGet]
        public JsonResult Get(int draw, int start)
        {
            var filteredData = _db.tbGroup.AsQueryable();
             
            string parameter = Request.Query["search[value]"].FirstOrDefault();

            GroupCondition condition = null;
            if (parameter != "")
            {
                condition = (GroupCondition)JsonConvert.DeserializeObject(parameter, typeof(GroupCondition));
            }

            List<tbGroup> rows = null;
            if (condition != null)
            {
                if (condition.GroupID != "")
                    filteredData = filteredData.Where(e => e.GroupID.Contains(condition.GroupID));
                
                if (condition.GroupName != "")
                    filteredData = filteredData.Where(e => e.GroupName.Contains(condition.GroupName));
            }

            int recordsTotal = filteredData.Count();

            rows = filteredData.OrderBy(e => e.GroupID).Skip(start).Take(10).ToList();

            var renderModel = new DataTablesRenderModel
            {
                draw = draw,
                data = rows,
                length = rows.Count(),
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal
            };
            return Json(renderModel);
            ////////////////


            //var filteredData = (from p in _db.ProductInfoes

            //                    select new
            //                    {
            //                        p.ProductNo,
            //                        p.ProductName,
            //                        p.Spec,
            //                        p.Unit,
            //                        p.Capcity
            //                    }).AsQueryable();

            //int recordsTotal = _db.ProductInfoes.Count();
            //var rows = filteredData.OrderBy(e => e.ProductNo).ToList();
            //var rows = filteredData.OrderBy(e => e.ProductNo).Skip(start).Take(10).ToList();
            //rows = filteredData.OrderBy(e => e.ProductNo).Where(s => s.ProductNo == searchProductNo).Skip(start).Take(10).ToList();
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
        public string Post([FromBody]tbGroup group)
        {
            try
            {
                tbGroup tbGroup = _db.tbGroup.Where(x => x.GroupID == group.GroupID).FirstOrDefault();

                if(tbGroup != null)
                {
                    return "已存在此群組ID!!";
                }



                _db.tbGroup.Add(group);

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }

        // PUT api/values
        [HttpPut]
        public string Put([FromBody]tbGroup group)
        {
            try
            {
                tbGroup tbGroup = _db.tbGroup.Where(b => b.GroupID == group.GroupID).First();
                tbGroup.GroupName = group.GroupName;
                tbGroup.Power = group.Power;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }

        // DELETE api/values
        [HttpDelete]
        public string Delete([FromBody]string value)
        {
            try
            {
                //var tmp = from a in _db.ProductInfoes
                //          join b in _db.Records
                //          on a.ProductNo equals b.ProductNo
                //          where a.ProductNo == "dddd"
                //          select new { a.ProductNo, a.Unit, b.WorkDate, b.UserId };

                tbGroup tbGroup = _db.tbGroup.Where(b => b.GroupID == value).First();
                _db.tbGroup.Remove(tbGroup);
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
