using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using P1611706.Model;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers
{
    public class PackingListsEController : BaseController
    {
		private P1611706DB _db = null;

		private readonly IHostingEnvironment _hostingEnvironment;
		public PackingListsEController(IHostingEnvironment hostingEnvironment, P1611706DB db)
		{
			_hostingEnvironment = hostingEnvironment;
			_db = db;
		}


		// GET: /<controller>/
		public IActionResult Index()
		{
			UserModel _UserModel = null;

			if (ViewData["UserModel"] != null)
			{
				_UserModel = (UserModel)ViewData["UserModel"];

				if (_UserModel.ProgramDictionary.ContainsKey("R"))
					foreach (string progID in _UserModel.ProgramDictionary["R"].ProgID)
						if (progID.Equals("R002"))
							return View();//有權限則進入
				return RedirectToAction("Index", "ErrorPage");//沒有權限則進入錯誤畫面
			}

			return RedirectToAction("Index", "Login");
		}

		public IActionResult Case()
		{
			return View();
		}

		public IActionResult Pallets()
		{
			return View();
		}

		[HttpGet]
		public ActionResult DownLoad()
		{
			if (ViewData["UserModel"] != null)
			{
				UserModel _UserModel = (UserModel)ViewData["UserModel"];
				string path = _hostingEnvironment.WebRootPath + "\\Excel";
				byte[] fileBytes = System.IO.File.ReadAllBytes(path + "\\" + _UserModel.EmployeeNo + "-產品報表.xlsx");

				return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, _UserModel.EmployeeNo + "-產品報表.xlsx");
			}
			return null;
		}

		[HttpGet]
		public IActionResult Export(string productName, string packingTime)
		{
			try
			{
				if (ViewData["UserModel"] != null)
				{
					UserModel _UserModel = null;
					FileInfo file;
					string path = _hostingEnvironment.WebRootPath;
					string sFileName;

					if (!Directory.Exists(path + "\\Excel"))
						Directory.CreateDirectory(path + "\\Excel");
					path = path + "\\Excel";

					_UserModel = (UserModel)ViewData["UserModel"];
					sFileName = @"" + _UserModel.EmployeeNo + "-產品報表.xlsx";


					file = new FileInfo(Path.Combine(path, sFileName));

					if (productName == null)
						productName = "";
					if (packingTime == null)
						packingTime = "";

					if (file.Exists)
					{
						file.Delete();
						file = new FileInfo(Path.Combine(path, sFileName));
					}

					var exportSpource = this.GetExportData(productName, packingTime);
					DataTable dt = JsonConvert.DeserializeObject<DataTable>(exportSpource.ToString());

					//如果沒有資料就reurn
					if (dt.Rows.Count == 0)
						return Content("noData");

					using (ExcelPackage package = new ExcelPackage(file))
					{
						ExcelWorksheet ws = package.Workbook.Worksheets.Add("Product");
						ws.Cells["A1"].LoadFromDataTable(dt, true);

						//設定寬度
						ws.Column(1).Width = 48;
						ws.Column(2).Width = 46;
						ws.Column(3).Width = 15;
						ws.Column(4).Width = 11;
						ws.Column(5).Width = 23;
						ws.Column(6).Width = 23;
						ws.Column(7).Width = 17;
						ws.Column(8).Width = 20;
						ws.Column(9).Width = 10;
						package.Save();
					}
				}
				else
				{
					return Content("error");
				}
			}
			catch (Exception ex)
			{
				return Content(ex.Message);
			}
			return Content("success");
		}

		private JArray GetExportData(string productName, string packingTime)
		{
			string[] packingTimeRangeList;
			string packingTimeS = "";
			string packingTimeE = "";
			DateTime dateTimeS;
			DateTime dateTimeE;

			UserModel _UserModel = null;

			var query = (from a in _db.PackingLists
						 join b in _db.ProductInfoes
					   on a.ProductNo equals b.ProductNo
						 select new
						 {
							 a.BagNo,
							 a.CaseNo,
							 a.CompanyCode,
							 a.Lot,
							 a.PackingTime,
							 a.PalletsNo,
							 a.ProductNo,
							 b.ProductName,
							 a.SerialNo
						 });

			if (!productName.Equals(""))
				query = query.Where(p => p.ProductName.Contains(productName));
			if (!packingTime.Equals(""))
			{
				packingTimeRangeList = packingTime.Split('-');
				packingTimeRangeList[0] = packingTimeRangeList[0].Trim();
				packingTimeRangeList[1] = packingTimeRangeList[1].Trim();
				dateTimeS = Convert.ToDateTime(packingTimeRangeList[0]);
				dateTimeE = Convert.ToDateTime(packingTimeRangeList[1]);
				dateTimeE = dateTimeE.AddDays(1);
				query = query.Where(e => e.PackingTime >= dateTimeS && e.PackingTime <= dateTimeE);
			}

			query = query.OrderBy(x => x.PackingTime);

			JArray jObjects = new JArray();

			foreach (var item in query)
			{
				var jo = new JObject();
				jo.Add("BagNo", item.BagNo);
				jo.Add("CaseNo", item.CaseNo);
				jo.Add("CompanyCode", item.CompanyCode);
				jo.Add("Lot", item.Lot);
				jo.Add("PackingTime", item.PackingTime.ToString());
				jo.Add("PalletsNo", item.PalletsNo);
				jo.Add("ProductNo", item.ProductNo);
				jo.Add("ProductName", item.ProductName);
				jo.Add("SerialNo", item.SerialNo);
				jObjects.Add(jo);
			}
			return jObjects;
		}
	}
}
