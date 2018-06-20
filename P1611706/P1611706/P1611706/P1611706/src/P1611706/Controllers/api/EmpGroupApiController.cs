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
    public class EmpGroupApiController : Controller
    {
        private P1611706DB _db = null;

        public EmpGroupApiController(P1611706DB db)
        {
            _db = db;
        }
        // GET: api/values
        [HttpGet]
        public JsonResult Get(int draw, int start)
        {
            //List<Records> tmp = (from a in _db.ProductInfoes
            //                     join b in _db.Records
            //                     on a.ProductNo equals b.ProductNo
            //                     where a.ProductNo == "dddd"
            //                     select new Records
            //                     {
            //                         UserId = b.UserId,
            //                         Unit = b.Unit,
            //                         ProductNo = a.ProductNo
            //                     }).ToList();
            
            var employeeNo = "";
            var countEmpNo = "";
            int count = 0;
            int index = 0;
            List<EmpGroup> rows = new List<EmpGroup>();
            List<string> rowsCount = new List<string>();
            List<EmpGroup> tbrows = new List<EmpGroup>();
            string parameter = Request.Query["search[value]"].FirstOrDefault();

            EmpGroupCondition condition = null;

            var counttmp = _db.tbEmployee.Select(m => m.EmployeeNo).Distinct().AsQueryable();
			
			var tmp = (from a in _db.tbEmployee
					   from b in _db.tbEmpGroup
					   .Where(f => f.EmployeeNo == a.EmployeeNo)
					   select new
					   {
						   a.EmployeeNo,
						   a.EmployeeName,
						   b.GroupID
					   } into x
					   join c in _db.tbGroup on x.GroupID equals c.GroupID into gj
					   from y in gj.DefaultIfEmpty()
					   select new EmpGroup
					   {
						   EmployeeNo = x.EmployeeNo,
						   EmployeeName = x.EmployeeName,
						   GroupName = (y == null ? String.Empty : y.GroupName)
					   }).AsQueryable();



			//var tmp = (from a in _db.tbEmployee
			//           join b in _db.tbEmpGroup
			//           on a.EmployeeNo equals b.EmployeeNo into list1
			//           from l1 in list1.DefaultIfEmpty()
			//           join c in _db.tbGroup
			//                      on l1.GroupID equals c.GroupID into list12
			//           from l2 in list12.DefaultIfEmpty()
			//           select new EmpGroup
			//           {
			//               EmployeeNo = a.EmployeeNo,
			//               EmployeeName = a.EmployeeName,
			//               EmployeeEName = a.EmployeeEName,
			//               GroupName = l2.GroupName
			//           }).AsQueryable();
			//    var tmp = (from a in _db.tbEmployee
			//               from OD in _db.tbEmpGroup
			//.Where(OD => OD.EmployeeNo == a.EmployeeNo).DefaultIfEmpty()

			//               join c in _db.tbGroup
			//                          on OD.GroupID equals c.GroupID
			//               select new EmpGroup
			//               {
			//                   EmployeeNo = a.EmployeeNo,
			//                   EmployeeName = a.EmployeeName,
			//                   EmployeeEName = a.EmployeeEName,
			//                   GroupName = (OD == null ? String.Empty : c.GroupName)
			//               }).AsQueryable();

			if (parameter != "")
            {
                condition = (EmpGroupCondition)JsonConvert.DeserializeObject(parameter, typeof(EmpGroupCondition));
            }

            if (condition != null)
            {
                if (condition.EmployeeNo != "")
                    tmp = tmp.Where(e => e.EmployeeNo.Contains(condition.EmployeeNo));

                if (condition.EmployeeName != "")
                {
                    tmp = tmp.Where(e => e.EmployeeName.Contains(condition.EmployeeName));
                }
            }
            
            rows = tmp.OrderBy(e => e.EmployeeNo).ToList();

            //for (int i = rows.Count - 1; i >= 0; i--)
            //{

            //    if (!rows[i].EmployeeNo.Equals(employeeNo))
            //    {
            //        index = i;
            //        employeeNo = rows[i].EmployeeNo;
            //    }
            //    else
            //    {
            //        rows[index].GroupName = rows[index].GroupName + "," + rows[i].GroupName;
            //        rows.RemoveAt(i);
            //    }
            //}

            EmpGroup EmpGroup = new EmpGroup();
            for (int i=0; i<rows.Count; i++)
            {
                if (!rows[i].EmployeeNo.Equals(countEmpNo))
                {
                    countEmpNo = rows[i].EmployeeNo;
                    count++;
                }

                if (count > (start + 10))
                    break;

                if (count>start)
                {

                    if (!rows[i].EmployeeNo.Equals(employeeNo))
                    {
                        if (!employeeNo.Equals(""))
                            tbrows.Add(EmpGroup);
                        EmpGroup = new EmpGroup();
                        employeeNo = rows[i].EmployeeNo;
                        EmpGroup.EmployeeNo = employeeNo;
                        EmpGroup.EmployeeName = rows[i].EmployeeName;
                        EmpGroup.GroupName = rows[i].GroupName;
                    }
                    else
                    {
                        EmpGroup.GroupName = EmpGroup.GroupName + "," + rows[i].GroupName;
                    }

                }
            }
            if(rows.Count != 0)
                tbrows.Add(EmpGroup);

            int recordsTotal = counttmp.Count();

            //rows = filteredData.OrderBy(e => e.GroupID).Skip(start).Take(10).ToList();

           
            var renderModel = new DataTablesRenderModel
            {
                draw = draw,
                data = tbrows,
                length = tbrows.Count(),
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
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values
        [HttpPut]
        public string Put([FromBody]tbEmpGroup empGroup)
        {
            try
            {
                string[] arrGroupID = empGroup.GroupID.Split(','); //更改過後的GroupID
                List<string> listDGroupID = new List<string>(); //需要刪除的GroupID
                List<string> listAGroupID = new List<string>(); //需要新增的GroupID
                
                bool flag = true;

                List<tbEmpGroup> listTbEmpGroup = _db.tbEmpGroup.Where(b => b.EmployeeNo == empGroup.EmployeeNo).ToList();
                
                //紀錄刪除項目
                foreach(tbEmpGroup mtbEmpGroup in listTbEmpGroup)
                {
                    flag = true;
                    for (int i = 0; i < arrGroupID.Length; i++)
                    {
                        if (mtbEmpGroup.GroupID.Equals(arrGroupID[i]))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag == true)
                        listDGroupID.Add(mtbEmpGroup.GroupID);
                }

                //紀錄新增項目
                for (int i = 0; i < arrGroupID.Length; i++)
                {
                    flag = true;
                    foreach (tbEmpGroup mtbEmpGroup in listTbEmpGroup)
                    {
                        if (arrGroupID[i].Equals(mtbEmpGroup.GroupID))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if(flag == true)
                        listAGroupID.Add(arrGroupID[i]);
                }

                //資料庫刪除
                foreach(string groupID in listDGroupID)
                {
                    tbEmpGroup tbEmpGroup = _db.tbEmpGroup.Where(b => b.EmployeeNo == empGroup.EmployeeNo).Where(c => c.GroupID == groupID).First();
                    _db.tbEmpGroup.Remove(tbEmpGroup);
                    
                }

                //資料庫新增
                foreach (string groupID in listAGroupID)
                {
                    tbEmpGroup tbEmpGroup = new tbEmpGroup();
                    tbEmpGroup.EmployeeNo = empGroup.EmployeeNo;
                    tbEmpGroup.GroupID = groupID;
                    _db.tbEmpGroup.Add(tbEmpGroup);

                    _db.SaveChanges();
                }

                

                
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

    }
}
