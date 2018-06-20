﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P1611706.Model;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers
{
	public class OrderLogController : BaseController
	{
		private P1611706DB _db = null;

		public OrderLogController(P1611706DB db)
		{
			_db = db;
		}
		// GET: /<controller>/
		public IActionResult Index()
		{
			UserModel _UserModel = null;

			if (ViewData["UserModel"] != null)
			{
				_UserModel = (UserModel)ViewData["UserModel"];

				if (_UserModel.ProgramDictionary.ContainsKey("L"))
					foreach (string progID in _UserModel.ProgramDictionary["L"].ProgID)
						if (progID.Equals("L001"))
							return View();//有權限則進入
				return RedirectToAction("Index", "ErrorPage");//沒有權限則進入錯誤畫面
			}

			return RedirectToAction("Index", "Login");
		}
		
	}
}