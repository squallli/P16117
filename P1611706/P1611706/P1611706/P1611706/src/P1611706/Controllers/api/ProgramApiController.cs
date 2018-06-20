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
    public class ProgramApiController : Controller
    {
        private P1611706DB _db = null;

        public ProgramApiController(P1611706DB db)
        {
            _db = db;
        }
        // GET: api/values
        [HttpGet]
        public JsonResult Get(int draw, int start)
        {
            var filteredData = _db.tbProgram.AsQueryable();
             
            
            List<tbProgram> rows = null;
            int recordsTotal = filteredData.Count();

            rows = filteredData.OrderBy(e => e.Power).Skip(start).Take(15).ToList();

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
        public void Post([FromBody]string productNo)
        {
            
        }
        
        
    }
}
