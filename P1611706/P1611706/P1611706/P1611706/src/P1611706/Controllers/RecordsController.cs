using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P1611706.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Text;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace P1611706.Controllers
{
    public class RecordsController : BaseController
    {
        private P1611706DB _db = null;
		private LeaderDemoDB _Ldb = null;

		public RecordsController(P1611706DB db, LeaderDemoDB Ldb)
        {
            _db = db;
			_Ldb = Ldb;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
			UserModel _UserModel = null;

			if (ViewData["UserModel"] != null)
			{
				_UserModel = (UserModel)ViewData["UserModel"];

				if (_UserModel.ProgramDictionary.ContainsKey("T"))
					foreach (string progID in _UserModel.ProgramDictionary["T"].ProgID)
						if (progID.Equals("T001"))
							return View();//有權限則進入
				return RedirectToAction("Index", "ErrorPage");//沒有權限則進入錯誤畫面
			}

			return RedirectToAction("Index", "Login");
		}

        [HttpGet]
        public List<RecordLabel> GetRecords(string bagNo, string funCode)
        {
            List<RecordLabel> listRecordLabel = null;
        
            

            try
            {
                if (bagNo == null)
                {
                    return null;
                }

                else
                {

                    List<MOCTA> listMOCTA = null;
                    List<INVTBProcess> listINVTBProcess = null;
                    List<PURTG> listPURTG = null;
                    List<COPTG> listCOPTG = null;
                    List<COPTI> listCOPTI = null;
                    List<COPTN> listCOPTN = null;
                    List<CMSMB> listCMSMB = null; 
                    List<CMSMC> listCMSMC = null;
                    List<INVTF> listINVTF = null;
                    List<INVTH> listINVTH = null;
                    List<INVTA> listINVTA = null;
                    if (funCode.Equals("1"))
                    {
                        listRecordLabel = (from a in _db.Records
                                           join b in _db.ProductInfoes on a.ProductNo equals b.ProductNo
                                           where a.BagNo == bagNo
                                           select new RecordLabel
                                           {
                                               FunCode = a.FunCode,
                                               ProductNo = a.ProductNo,
                                               ProductName = b.ProductName,
                                               Lot = a.Lot,
                                               ExpiryDate = a.ExpiryDate,
                                               WorkDate = a.WorkDate,
                                               WorkTime = a.WorkTime,
                                               OrderType = a.OrderType,
                                               OrderNo = a.OrderNo,
                                               WareHouse = a.WarehouseNo,
                                               CarNo = a.CarNo
                                           }).OrderBy(e => e.WorkDate).ToList();
                    }
                    else
                    {
                        listRecordLabel = (from a in _db.Records
                                           join b in _db.ProductInfoes on a.ProductNo equals b.ProductNo
                                           where a.BagNo == bagNo
                                           where a.FunCode == funCode
                                           select new RecordLabel
                                           {
                                               FunCode = a.FunCode,
                                               ProductNo = a.ProductNo,
                                               ProductName = b.ProductName,
                                               Lot = a.Lot,
                                               ExpiryDate = a.ExpiryDate,
                                               WorkDate = a.WorkDate,
                                               WorkTime = a.WorkTime,
                                               OrderType = a.OrderType,
                                               OrderNo = a.OrderNo,
                                               WareHouse = a.WarehouseNo,
                                               CarNo = a.CarNo
                                           }).OrderBy(e => e.WorkDate).ToList();
                    }

                    foreach (RecordLabel recordLabel in listRecordLabel)
                    {
                        listCMSMB = new List<CMSMB>();
                        listCMSMC = new List<CMSMC>();

                        if (recordLabel.FunCode.Equals("2"))//包裝
                        {
                        }
                        else if (recordLabel.FunCode.Equals("3"))//出產入庫
                        {
                            listMOCTA = _Ldb.MOCTA.Where(e => e.TA001 == recordLabel.OrderType).Where(e => e.TA002 == recordLabel.OrderNo)
                                //.Where(d => d.TA003 == recordLabel.WorkDate)
                                .ToList();

                            if (listMOCTA.Count > 0)
                            {
                                recordLabel.Source = listMOCTA[0].TA019;
                                recordLabel.Purpose = listMOCTA[0].TA020;

                                //取得廠別名稱
                                listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listMOCTA[0].TA019).ToList();

                                if (listCMSMB.Count > 0)
                                {
                                    recordLabel.Source = listCMSMB[0].MB002;
                                }

                                //取得入庫庫別名稱
                                listCMSMC = _Ldb.CMSMC.Where(e => e.MC001 == listMOCTA[0].TA020).ToList();

                                if (listCMSMC.Count > 0)
                                {
                                    recordLabel.Purpose = listCMSMC[0].MC002;
                                }

                            }
                        }
                        else if (recordLabel.FunCode.Equals("4"))//轉撥
                        {
                            //listINVTB = _Ldb.INVTB.Where(e => e.TB001 == recordLabel.OrderType).Where(e => e.TB002 == recordLabel.OrderNo).Where(e => e.TB014 == recordLabel.Lot).ToList();
                            listINVTBProcess = (from a in _Ldb.INVTA
                                                from b in _Ldb.INVTB
                                                where a.TA001 == b.TB001 && a.TA002 == b.TB002
                                                where a.TA001 == recordLabel.OrderType
                                                where a.TA002 == recordLabel.OrderNo
                                                //where a.TA003 == recordLabel.WorkDate
                                                select new INVTBProcess
                                                {
                                                    Source = b.TB012,
                                                    Purpose = b.TB013,
                                                }).ToList();

                            if (listINVTBProcess.Count > 0)
                            {

                                recordLabel.Source = listINVTBProcess[0].Source;
                                recordLabel.Purpose = listINVTBProcess[0].Purpose;

                                //取得轉出庫名稱
                                listCMSMC = _Ldb.CMSMC.Where(e => e.MC001 == listINVTBProcess[0].Source).ToList();

                                if (listCMSMC.Count > 0)
                                {
                                    recordLabel.Source = listCMSMC[0].MC002;
                                }

                                listCMSMC.Clear();

                                //取得轉入庫名稱
                                listCMSMC = _Ldb.CMSMC.Where(e => e.MC001 == listINVTBProcess[0].Purpose).ToList();

                                if (listCMSMC.Count > 0)
                                {
                                    recordLabel.Purpose = listCMSMC[0].MC002;
                                }
                            }
                        }
                        else if (recordLabel.FunCode.Equals("5"))//進貨
                        {
                            listPURTG = _Ldb.PURTG.Where(e => e.TG001 == recordLabel.OrderType).Where(e => e.TG002 == recordLabel.OrderNo)
                                //.Where(d => d.TG003 == recordLabel.WorkDate)
                                .ToList();

                            if (listPURTG.Count > 0)
                            {
                                recordLabel.Source = listPURTG[0].TG005;
                                recordLabel.Purpose = listPURTG[0].TG004;

                                //取得供應廠商名稱
                                listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listPURTG[0].TG005).ToList();

                                if (listCMSMB.Count > 0)
                                {
                                    recordLabel.Source = listCMSMB[0].MB002;
                                }

                                listCMSMB.Clear();

                                //取得廠別名稱
                                listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listPURTG[0].TG004).ToList();

                                if (listCMSMB.Count > 0)
                                {
                                    recordLabel.Purpose = listCMSMB[0].MB002;
                                }
                            }
                        }
                        else if (recordLabel.FunCode.Equals("6"))//銷貨
                        {
                            if(recordLabel.OrderType.Substring(1, 2).Equals("23"))
                            {
                                listCOPTG = _Ldb.COPTG.Where(e => e.TG001 == recordLabel.OrderType).Where(e => e.TG002 == recordLabel.OrderNo)
                               //.Where(d => d.TG003 == recordLabel.WorkDate)
                               .ToList();

                                if (listCOPTG.Count > 0)
                                {
                                    recordLabel.Source = listCOPTG[0].TG010;
                                    recordLabel.Purpose = listCOPTG[0].TG007;//客戶名稱

                                    //取得出貨廠別名稱
                                    listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listCOPTG[0].TG010).ToList();

                                    if (listCMSMB.Count > 0)
                                    {
                                        recordLabel.Source = listCMSMB[0].MB002;
                                    }

                                }
                            }
                            else if (recordLabel.OrderType.Substring(1, 2).Equals("13"))
                            {
                                listINVTF = _Ldb.INVTF.Where(e => e.TF001 == recordLabel.OrderType).Where(e => e.TF002 == recordLabel.OrderNo)
                               //.Where(d => d.TG003 == recordLabel.WorkDate)
                               .ToList();

                                if (listINVTF.Count > 0)
                                {
                                    recordLabel.Source = listINVTF[0].TF009;
                                    recordLabel.Purpose = listINVTF[0].TF006;//客戶名稱

                                    //取得出貨廠別名稱
                                    listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listINVTF[0].TF009).ToList();

                                    if (listCMSMB.Count > 0)
                                    {
                                        recordLabel.Source = listCMSMB[0].MB002;
                                    }

                                }
                            }
                            else if (recordLabel.OrderType.Substring(1, 2).Equals("16"))
                            {
                                listINVTH = _Ldb.INVTH.Where(e => e.TH001 == recordLabel.OrderType).Where(e => e.TH002 == recordLabel.OrderNo)
                               //.Where(d => d.TG003 == recordLabel.WorkDate)
                               .ToList();

                                if (listINVTH.Count > 0)
                                {
                                    recordLabel.Source = listINVTH[0].TH009;
                                    recordLabel.Purpose = listINVTH[0].TH006;//客戶名稱

                                    //取得出貨廠別名稱
                                    listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listINVTH[0].TH009).ToList();

                                    if (listCMSMB.Count > 0)
                                    {
                                        recordLabel.Source = listCMSMB[0].MB002;
                                    }

                                }
                            }
                            else if (recordLabel.OrderType.Substring(1, 2).Equals("11"))
                            {
                                listINVTA = _Ldb.INVTA.Where(e => e.TA001 == recordLabel.OrderType).Where(e => e.TA002 == recordLabel.OrderNo)
                               //.Where(d => d.TG003 == recordLabel.WorkDate)
                               .ToList();

                                if (listINVTA.Count > 0)
                                {
                                    recordLabel.Source = listINVTA[0].TA008;
                                    recordLabel.Purpose = listINVTA[0].TA005;//客戶名稱

                                    //取得出貨廠別名稱
                                    listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listINVTA[0].TA008).ToList();

                                    if (listCMSMB.Count > 0)
                                    {
                                        recordLabel.Source = listCMSMB[0].MB002;
                                    }

                                }
                            }

                           
                        }
                        else if (recordLabel.FunCode.Equals("7"))//上車
                        {

                                recordLabel.Source = recordLabel.WareHouse;
                            listCMSMC = _Ldb.CMSMC.Where(x => x.MC001 == recordLabel.WareHouse).ToList();

                            if(listCMSMC.Count > 0)
                            {
                                recordLabel.Source = listCMSMC[0].MC002;
                            }

                                recordLabel.Purpose = recordLabel.CarNo;



                            //listCOPTN = _Ldb.COPTN.Where(e => e.TN001 == recordLabel.OrderType).Where(e => e.TN002 == recordLabel.OrderNo)
                            //    //.Where(d => d.TN003 == recordLabel.WorkDate)
                            //    .ToList();

                            //if (listCOPTN.Count > 0)
                            //{
                            //    recordLabel.Source = listCOPTN[0].TN010;
                            //    recordLabel.Purpose = listCOPTN[0].TN005;

                            //    //取得廠別名稱
                            //    listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listCOPTN[0].TN010).ToList();

                            //    if (listCMSMB.Count > 0)
                            //    {
                            //        recordLabel.Source = listCMSMB[0].MB002;
                            //    }
                            //}
                        }
                        else if (recordLabel.FunCode.Equals("8"))//下車
                        {
                            
                            if (recordLabel.OrderType.Substring(1, 2).Equals("23"))
                            {
                                recordLabel.Source = recordLabel.CarNo;
                                listCOPTG = _Ldb.COPTG.Where(e => e.TG001 == recordLabel.OrderType).Where(e => e.TG002 == recordLabel.OrderNo)
                                    //.Where(d => d.TN003 == recordLabel.WorkDate)
                                    .ToList();

                                if (listCOPTG.Count > 0)
                                {
                                    recordLabel.Purpose = listCOPTG[0].TG007;

                                    ////取得廠別名稱
                                    //listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listCOPTN[0].TN010).ToList();

                                    //if (listCMSMB.Count > 0)
                                    //{
                                    //    recordLabel.Source = listCMSMB[0].MB002;
                                    //}
                                }
                            }
                            else if (recordLabel.OrderType.Substring(1, 2).Equals("13"))
                            {
                                recordLabel.Source = recordLabel.CarNo;
                                listINVTF = _Ldb.INVTF.Where(e => e.TF001 == recordLabel.OrderType).Where(e => e.TF002 == recordLabel.OrderNo)
                                    //.Where(d => d.TN003 == recordLabel.WorkDate)
                                    .ToList();

                                if (listINVTF.Count > 0)
                                {
                                    recordLabel.Purpose = listINVTF[0].TF006;

                                    ////取得廠別名稱
                                    //listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listCOPTN[0].TN010).ToList();

                                    //if (listCMSMB.Count > 0)
                                    //{
                                    //    recordLabel.Source = listCMSMB[0].MB002;
                                    //}
                                }
                            }
                            else if (recordLabel.OrderType.Substring(1, 2).Equals("16"))
                            {
                                recordLabel.Source = recordLabel.CarNo;
                                listINVTH = _Ldb.INVTH.Where(e => e.TH001 == recordLabel.OrderType).Where(e => e.TH002 == recordLabel.OrderNo)
                                    //.Where(d => d.TN003 == recordLabel.WorkDate)
                                    .ToList();

                                if (listINVTH.Count > 0)
                                {
                                    recordLabel.Purpose = listINVTH[0].TH006;

                                    ////取得廠別名稱
                                    //listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listCOPTN[0].TN010).ToList();

                                    //if (listCMSMB.Count > 0)
                                    //{
                                    //    recordLabel.Source = listCMSMB[0].MB002;
                                    //}
                                }
                            }
                            else if (recordLabel.OrderType.Substring(1, 2).Equals("11"))
                            {
                                recordLabel.Source = recordLabel.CarNo;
                                listINVTA = _Ldb.INVTA.Where(e => e.TA001 == recordLabel.OrderType).Where(e => e.TA002 == recordLabel.OrderNo)
                                    //.Where(d => d.TN003 == recordLabel.WorkDate)
                                    .ToList();

                                if (listINVTA.Count > 0)
                                {
                                    recordLabel.Purpose = listINVTA[0].TA005;

                                    ////取得廠別名稱
                                    //listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listCOPTN[0].TN010).ToList();

                                    //if (listCMSMB.Count > 0)
                                    //{
                                    //    recordLabel.Source = listCMSMB[0].MB002;
                                    //}
                                }
                            }
                        }
                        else if (recordLabel.FunCode.Equals("9"))//退貨
                        {
                            if (recordLabel.OrderType.Substring(1, 2).Equals("13"))
                            {
                                listINVTF = _Ldb.INVTF.Where(e => e.TF001 == recordLabel.OrderType).Where(e => e.TF002 == recordLabel.OrderNo)
                                   //.Where(d => d.TI003 == recordLabel.WorkDate)
                                   .ToList();

                                if (listINVTF.Count > 0)
                                {
                                    recordLabel.Source = listINVTF[0].TF006;//客戶名稱
                                    recordLabel.Purpose = listINVTF[0].TF009;

                                    //取得廠別名稱
                                    listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listINVTF[0].TF009).ToList();

                                    if (listCMSMB.Count > 0)
                                    {
                                        recordLabel.Purpose = listCMSMB[0].MB002;
                                    }
                                }

                            }
                            else if (recordLabel.OrderType.Substring(1, 2).Equals("16"))
                            {
                                listINVTH = _Ldb.INVTH.Where(e => e.TH001 == recordLabel.OrderType).Where(e => e.TH002 == recordLabel.OrderNo)
                                   //.Where(d => d.TI003 == recordLabel.WorkDate)
                                   .ToList();

                                if (listINVTH.Count > 0)
                                {
                                    recordLabel.Source = listINVTH[0].TH006;//客戶名稱
                                    recordLabel.Purpose = listINVTH[0].TH009;

                                    //取得廠別名稱
                                    listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listINVTH[0].TH009).ToList();

                                    if (listCMSMB.Count > 0)
                                    {
                                        recordLabel.Purpose = listCMSMB[0].MB002;
                                    }
                                }

                            }
                            else
                            {
                                listCOPTI = _Ldb.COPTI.Where(e => e.TI001 == recordLabel.OrderType).Where(e => e.TI002 == recordLabel.OrderNo)
                                   //.Where(d => d.TI003 == recordLabel.WorkDate)
                                   .ToList();

                                if (listCOPTI.Count > 0)
                                {
                                    recordLabel.Source = listCOPTI[0].TI004;//客戶名稱
                                    recordLabel.Purpose = listCOPTI[0].TI007;

                                    //取得廠別名稱
                                    listCMSMB = _Ldb.CMSMB.Where(e => e.MB001 == listCOPTI[0].TI007).ToList();

                                    if (listCMSMB.Count > 0)
                                    {
                                        recordLabel.Purpose = listCMSMB[0].MB002;
                                    }
                                }

                            }



                                
                        }
                    }


                }
                
            }
            catch (Exception ex)
            {
                RecordLabel exRecordLabel = new RecordLabel();
                exRecordLabel.ProductNo = ex.Message;
                listRecordLabel.Clear();
                listRecordLabel.Add(exRecordLabel);


            }
            finally
            {
            }

            return listRecordLabel;
        }


    }

		
    
}
