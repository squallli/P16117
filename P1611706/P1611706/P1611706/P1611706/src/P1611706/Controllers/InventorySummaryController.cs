using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P1611706.Model;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers
{
    public class InventorySummaryController : BaseController
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            UserModel _UserModel = null;

            if (ViewData["UserModel"] != null)
            {
                _UserModel = (UserModel)ViewData["UserModel"];

                if (_UserModel.ProgramDictionary.ContainsKey("I"))
                    foreach (string progID in _UserModel.ProgramDictionary["I"].ProgID)
                        if (progID.Equals("I001"))//有權限則進入
                            return View();
                return RedirectToAction("Index", "ErrorPage");//沒有權限則進入錯誤畫面
            }

            return RedirectToAction("Index", "Login");
        }
        public IActionResult PalletsDetail(string InventoryDate,string InventoryNo,string PalletsNo)
        {
            ViewData["InventoryDate"] = InventoryDate;
            ViewData["InventoryNo"] = InventoryNo;
            ViewData["PalletsNo"] = PalletsNo;
            return View();
        }
        

    }
}
