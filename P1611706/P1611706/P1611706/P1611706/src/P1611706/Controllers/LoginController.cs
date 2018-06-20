using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


using P1611706.Model;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.IO;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers
{
    public class LoginController : Controller
    {
        private readonly LdapString _ldap;
        private P1611706DB _db = null;

        public LoginController(IOptions<LdapString> config, P1611706DB db)
        {
            _ldap = config.Value;
            _db = db;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
			ViewBag.Error = TempData["error"];
			return View();
        }

        //public IActionResult doLogin(string userName, string password)
        //{
        //    LdapConnection ldapConn = new LdapConnection();
        //    JsonObjModel JsonObj = new JsonObjModel();
        //    UserModel User = new UserModel();


        //    try
        //    {
        //        //Connect function will create a socket connection to the server
        //        //ldapConn.Connect("192.168.0.5", 389);

        //        //Bind function will Bind the user object Credentials to the Server
        //        //ldapConn.Bind("regalscan\\" + userName, password);

        //        User.UserName = userName;
        //        JsonObj.Query = User;

        //        string JsonData = JsonConvert.SerializeObject(JsonObj.Query);


        //        //HttpContext.Session.SetString("User", JsonData);

        //        HttpContext.Session.SetString("User", userName);

        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }

        //    return Redirect("/Main/Index");
        //    //return RedirectToAction("Index", "Main");
        //}

        public IActionResult check(string employeeNo, string password)
        {
            LdapConnection ldapConn = new LdapConnection();
            JsonObjModel JsonObj = new JsonObjModel();
            UserModel User = new UserModel();

            try
            {
                //Connect function will create a socket connection to the server
                //ldapConn.Connect("192.168.0.5", 389);


                //Bind function will Bind the user object Credentials to the Server
                //ldapConn.Bind("regalscan\\" + userName, password);
                
                var filteredData = _db.tbEmployee.AsQueryable();
                var filteredProgData = _db.tbProgram.AsQueryable();
                List<tbEmployee> listTbEmployee = null;
                List<tbGroup> listTbGroup = new List<tbGroup>();
                List<tbProgram> listTbProgram = null;
                string power = "";
                string tbGroupPower = "";
                string progMainID = "";//紀錄作業父節點
				int powerLength = 0;
                

                SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256
                byte[] source = Encoding.Default.GetBytes(password);//將字串轉為Byte[]
                byte[] crypto = sha256.ComputeHash(source);//進行SHA256加密
                string result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串

                filteredData = filteredData.Where(e => e.EmployeeNo == employeeNo).Where(p => p.PassWord == result);
                filteredProgData = filteredProgData.Where(e => e.ProgID != "");

                listTbEmployee = filteredData.ToList();
                listTbProgram = filteredProgData.OrderBy(e => e.Power).ToList();

                var tmp = (from b in _db.tbEmpGroup
                           join c in _db.tbGroup
                                      on b.GroupID equals c.GroupID
                           where b.EmployeeNo == employeeNo
                           select new tbGroup
                           {
                               GroupID = c.GroupID,
                               GroupName = c.GroupName,
                               Power = c.Power
                           }).AsQueryable();
                listTbGroup = tmp.OrderByDescending(e => e.Power).ToList();

                //作業權限
                User.ProgramDictionary = new Dictionary<string, Model.Program>();
                Model.Program program = new Model.Program();
                if (listTbGroup.Count > 0)
                {
                    tbGroupPower = Convert.ToString(listTbGroup[0].Power, 2);
                    power = power.PadLeft(tbGroupPower.Length, '0');
					powerLength = power.Length;

					foreach (tbGroup tbGroup in listTbGroup)
                    {
                        tbGroupPower = Convert.ToString(tbGroup.Power, 2);
                        for (int i = 0; i < tbGroupPower.Length; i++)
                        {
                            if (tbGroupPower[i].ToString().Equals("1"))
                            {
                                power = power.Remove(powerLength- tbGroupPower.Length+i, 1);
                                power = power.Insert(powerLength - tbGroupPower.Length + i, "1");
                            }
                        }
                    }
                    for (int i = 0; i < power.Length; i++)
                    {
                        if (power[power.Length-1-i].ToString().Equals("1"))
                        {
                            if (!progMainID.Equals(listTbProgram[i].ProgID.ToString().Substring(0,1)))
                            {
                                if (!progMainID.Equals(""))
                                    User.ProgramDictionary.Add(progMainID, program);
                                program = new Model.Program();
                                progMainID = listTbProgram[i].ProgID.ToString().Substring(0,1);

                                program.ProgID = new List<string>();
                                program.ProgName = new List<string>();
                                program.ProgURL = new List<string>();
                                program.ProgID.Add(listTbProgram[i].ProgID.ToString());
                                program.ProgName.Add(listTbProgram[i].ProgName.ToString());
                                program.ProgURL.Add(listTbProgram[i].Url.ToString());

                            }

                            else
                            {
                                program.ProgID.Add(listTbProgram[i].ProgID.ToString());
                                program.ProgName.Add(listTbProgram[i].ProgName.ToString());
                                program.ProgURL.Add(listTbProgram[i].Url.ToString());
                            }


                        }
                    }
                    if (!progMainID.Equals(""))
                        User.ProgramDictionary.Add(progMainID, program);
                }


                if (listTbEmployee.Count == 1)
                {
                    User.EmployeeNo = listTbEmployee[0].EmployeeNo;
                    User.UserName = listTbEmployee[0].EmployeeName;

                    //tbEmployee[0].

                    JsonObj.Query = User;

                    string JsonData = JsonConvert.SerializeObject(JsonObj.Query);


                    HttpContext.Session.SetString("User", JsonData);

                    return RedirectToAction("Index", "Main");

                    //HttpContext.Session.SetString("User", userName);
                }
                //else
                //    return Content("error");
            }
            catch(Exception ex)
            {
				//return Content("error");

				TempData["error"] = ex.Message;
				ViewBag.Error = TempData["error"];
				return RedirectToAction("Index");

			}
			TempData["error"] = "帳號或密碼錯誤";
			ViewBag.Error = TempData["error"];
			return RedirectToAction("Index");
			//return Content("success");

		}

        private bool LdapConn_UserDefinedServerCertValidationDelegate(System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] certificateErrors)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 密碼加密
        /// </summary>
        /// <param name="text">密碼字串</param>
        /// <param name="keyString">加密鑰匙</param>
        /// <returns></returns>
        public static string EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }
        
        /// <summary>
        /// 密碼解密
        /// </summary>
        /// <param name="cipherText">加密密碼字串</param>
        /// <param name="keyString">加密鑰匙</param>
        /// <returns></returns>
        public static string DecryptString(string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
    }
}
