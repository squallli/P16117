using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P1611706.Model;
using SmartNetInventory.DataTable;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers.api
{
    [Route("api/[controller]")]
    public class EmployeeApiController : Controller
    {
        private P1611706DB _db = null;

        public EmployeeApiController(P1611706DB db)
        {
            _db = db;
        }
        // GET: api/values
        [HttpGet]
        public JsonResult Get(int draw, int start)
        {
            var employeeNo = "";
            var countEmpNo = "";
            int count = 0;
            int index = 0;
            int empCount = 0;
            string empNo = "";
            List<Employee> rows = new List<Employee>();
            List<string> rowsCount = new List<string>();
            List<Employee> tbrows = new List<Employee>();
            string parameter = Request.Query["search[value]"].FirstOrDefault();

            EmployeeCondition condition = null;

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
                       select new Employee
                       {
                           EmployeeNo = x.EmployeeNo,
                           EmployeeName = x.EmployeeName,
                           GroupName = (y == null ? String.Empty : y.GroupName)
                       }).AsQueryable();

            if (parameter != "")
            {
                condition = (EmployeeCondition)JsonConvert.DeserializeObject(parameter, typeof(EmployeeCondition));
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

            //計算搜尋員工總數
            foreach (Employee emp in rows)
            {
                if (!empNo.Equals(emp.EmployeeNo))
                {
                    empNo = emp.EmployeeNo;
                    empCount++;
                }
            }

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

            Employee Employee = new Employee();
            for (int i = 0; i < rows.Count; i++)
            {
                if (!rows[i].EmployeeNo.Equals(countEmpNo))
                {
                    countEmpNo = rows[i].EmployeeNo;
                    count++;
                }

                if (count > (start + 10))
                    break;

                if (count > start)
                {

                    if (!rows[i].EmployeeNo.Equals(employeeNo))
                    {
                        if (!employeeNo.Equals(""))
                            tbrows.Add(Employee);
                        Employee = new Employee();
                        employeeNo = rows[i].EmployeeNo;
                        Employee.EmployeeNo = employeeNo;
                        Employee.EmployeeName = rows[i].EmployeeName;
                        Employee.GroupName = rows[i].GroupName;
                    }
                    else
                    {
                        Employee.GroupName = Employee.GroupName + "," + rows[i].GroupName;
                    }

                }
            }
            if (!countEmpNo.Equals(""))
                tbrows.Add(Employee);

            int recordsTotal = counttmp.Count();

            //rows = filteredData.OrderBy(e => e.GroupID).Skip(start).Take(10).ToList();


            var renderModel = new DataTablesRenderModel
            {
                draw = draw,
                data = tbrows,
                length = tbrows.Count(),
                recordsFiltered = empCount,
                recordsTotal = empCount
            };
            return Json(renderModel);

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // PUT api/values
        [HttpPut]
        public string Put([FromBody]tbEmployee employee)
        {
            try
            {
                tbEmployee tbEmployee = _db.tbEmployee.Where(b => b.EmployeeNo == employee.EmployeeNo).First();
                tbEmployee.EmployeeName = employee.EmployeeName;


                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }


        // POST api/values
        [HttpPost("{Add}")]
        public string Post([FromBody]tbEmployee emp, string Add)
        {
            try
            {
                tbEmpGroup tbEmpG = new tbEmpGroup();
                if (Add.Equals("Add"))
                {

                    SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256
                    byte[] source = Encoding.Default.GetBytes(emp.PassWord);//將字串轉為Byte[]
                    byte[] crypto = sha256.ComputeHash(source);//進行SHA256加密
                    string result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串

                    emp.PassWord = result;

                    _db.tbEmployee.Add(emp);
                    tbEmpG.EmployeeNo = emp.EmployeeNo;
                    tbEmpG.GroupID = "";
                    _db.tbEmpGroup.Add(tbEmpG);
                    _db.SaveChanges();
                }
                else
                    return "error";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }


        // POST api/values
        [HttpPost]
        public string Post([FromBody]EmployeePassword employeePassword)
        {
            try
            {
                var filteredData = _db.tbEmployee.AsQueryable();
                List<tbEmployee> tbEmployee = null;
                SHA256 sha256;//建立一個SHA256
                byte[] source;//將字串轉為Byte[]
                byte[] crypto;//進行SHA256加密
                string result;//把加密後的字串從Byte[]轉為字串


                filteredData = filteredData.Where(b => b.EmployeeNo == employeePassword.EmployeeNo);

                tbEmployee = filteredData.ToList();

                if (tbEmployee.Count == 1)
                {
                    sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256
                    source = Encoding.Default.GetBytes(employeePassword.NewPassword);//將字串轉為Byte[]
                    crypto = sha256.ComputeHash(source);//進行SHA256加密
                    result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串

                    tbEmployee[0].PassWord = result;

                    _db.SaveChanges();
                }
                else
                    return "無法取得該員工資訊!!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }

        [HttpDelete]
        public string Delete([FromBody]string value)
        {
            try
            {

                tbEmployee _tbEmployee = _db.tbEmployee.Where(b => b.EmployeeNo == value).First();
                _db.tbEmployee.Remove(_tbEmployee);
                _db.SaveChanges();


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }



        //計算年資
        public string CalculateSeniority(DateTime self, DateTime target)
        {
            int years, months, days;
            try
            {

                // 將年轉換成月份以便用來計算
                months = 12 * (self.Year - target.Year) + (self.Month - target.Month);

                // 如果天數要相減的量不夠時要向月份借天數補滿該月再來相減
                if (self.Day < target.Day)
                {
                    months--;
                    days = DateTime.DaysInMonth(target.Year, target.Month) - target.Day + self.Day;
                }
                else
                {
                    days = self.Day - target.Day;
                }

                // 天數計算完成後將月份轉成年
                years = months / 12;
                months = months % 12;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return years.ToString() + "年 " + (months * 30 + days).ToString() + "天";
        }

    }
}
