using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P1611706.Model;

using Microsoft.AspNetCore.Http;


namespace P1611706.Controllers
{
    public class MainController : BaseController
    {
        
        private P1611706DB _db = null;
        public MainController(P1611706DB db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            
            return View();
        }

        

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
