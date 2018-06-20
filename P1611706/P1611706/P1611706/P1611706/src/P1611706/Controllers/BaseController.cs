using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using P1611706.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P1611706.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            
        }

        /// <summary> 執行前先讀取資料 </summary>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                //直接跳至Login畫面
                filterContext.Result = new RedirectToActionResult("Index", "Login", null);
                HttpContext.Session.Clear();
                return;



                //HttpCookie Cookie = Request.Cookies["EIPLoginUserInfo"];
                //if (Cookie != null)
                //{
                //    //**自動登入機制**


                //    //從cookie取得json
                //    EIPLoginUserInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModel>(HttpUtility.UrlDecode(Cookie.Value));



                //    if (EIPLoginUserInfo.Version == BaseController.Version)
                //    {
                //        //版本號相同 允許登入
                //        //再次存Session
                //        string JsonData = JsonConvert.SerializeObject(EIPLoginUserInfo);
                //        Session["EIPLoginUserInfo"] = JsonData;
                //    }
                //    else
                //    {
                //        //版本號不同 不允許登入 
                //        EIPLoginUserInfo = null;

                //        //註銷Cookie
                //        var userCookie = new HttpCookie("EIPLoginUserInfo");
                //        userCookie.Expires = DateTime.Now.Date.AddDays(-365);
                //        HttpContext.Response.Cookies.Add(userCookie);
                //    }
                //}
            }
            else
            {
                //若有登入則 賦予ViewData["UserModel"]
                ViewData["UserModel"] = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModel>(HttpContext.Session.GetString("User"));

				
			}
            //base.OnActionExecuting(filterContext);
        }
    }
    
}
