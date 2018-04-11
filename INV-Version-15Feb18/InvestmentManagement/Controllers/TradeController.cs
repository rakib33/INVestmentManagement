using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using InvestmentManagement.InvestmentManagement.Models;
using System.IO;
using System.Xml.Linq;
using System.Data.EntityClient;
using InvestmentManagement.App_Code;

namespace InvestmentManagement.Controllers
{
    public class TradeController : Controller
    {
        //
        // GET: /Trade/

        CommonFunction oCommonFunction = new CommonFunction();
        [HttpGet]
        public ActionResult ListTrade(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 5000, string reference = null, DateTime? tradingDate = null, string stockExchangeSA = null, string brokerSA = null, string instrument = null, bool chkSale = false, bool chkBuy = false)
        {
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
                double netseleamount = 0;
                //Drop Down List for Broker
                ViewBag.brokerList = new SelectList(new Entities(Session["Connection"] as EntityConnection).BROKERs.OrderBy(t=>t.NAME), "REFERENCE", "NAME");
                //Drop Down List for Instrument
                ViewBag.instrumentList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.OrderBy(t=>t.SHORTNAME), "REFERENCE", "SHORTNAME");

                

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<TRADE> gridModels = new GridModel<TRADE>();
                List<TRADE> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;

                //Paging direction
                switch (PagingType)
                {
                    case "Next":
                        Session["pageNo"] = (int)Session["pageNo"] + 1;
                        break;
                    case "Prev":
                        Session["pageNo"] = (int)Session["pageNo"] - 1;
                        break;
                    default:
                        Session["pageNo"] = 1;
                        ViewBag.Prev = "disabled";
                        ViewBag.PrevNotActive = "not-active";
                        break;
                }


                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).TRADEs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).TRADEs.AsNoTracking().Where(w => w.FILENAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();




                if (TempData["TranDate"] != null)
                {
                    DateTime importDate = DateTime.Parse(TempData["TranDate"].ToString());
                    models = models.Where(p => p.TRANSACTIONDATE == importDate).ToList();
                }


                
                if (tradingDate != null)
                {
                   // models = models.Where(p => p.TRANSACTIONDATE == tradingDate).ToList();
                    models = new Entities(Session["Connection"] as EntityConnection).TRADEs.AsNoTracking().Where(w => w.TRANSACTIONDATE == tradingDate).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();

                }
                else if (TempData["TranDate"] == null)
                {
                    models = models.Where(p => p.TRANSACTIONDATE == DateTime.Now).ToList();
                }



                if (string.IsNullOrEmpty(stockExchangeSA) != true)
                {
                    models = models.Where(p => p.STOCKEXCHANGE == stockExchangeSA).ToList();
                }

                if (string.IsNullOrEmpty(brokerSA) != true)
                {
                    models = models.Where(p => p.BROKER_REFERENCE == brokerSA).ToList();
                }


                if (string.IsNullOrEmpty(instrument) != true)
                {
                    //INSTRUMENT oINSTRUMENT=new Entities().INSTRUMENTs.Where(p=>p.SHORTNAME==instrument).FirstOrDefault();

                    models = models.Where(p => p.INSTRUMENT_REFERENCE == instrument).ToList();
                }

                //if (string.IsNullOrEmpty(investorAccount) != true)
                //{
                //    INVESTOR oINVESTOR = new INVESTOR();
                //    oINVESTOR = new Entities().INVESTORs.Where(x => x.ACCOUNTNUMBER == investorAccount).FirstOrDefault();
                //    models = models.Where(p => p.INVESTORACREF == oINVESTOR.REFERENCE).ToList();
                //}

                if (chkSale != false)
                {

                    models = models.Where(p => p.TRANSACTIONTYPE == "S").ToList();

                    //
                  // netseleamount =Convert.ToDouble(models.Sum(t => t.NETAMOUNT).Value);
                }
                if (chkBuy != false)
                {
                    models = models.Where(p => p.TRANSACTIONTYPE == "B").ToList();
                }


                gridModels.DataModel = models.OrderBy(t => t.TRANSACTIONTIME).ToList();
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }



                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
               
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }
                return PartialView("ListTrade", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        /// <summary>
        /// SHARE that buy sell in spot market those are matured 1 days later by checking holiday.
        /// IN this case instrument catagory sattlement days does not do any effect for spot market matured date.
        /// Implement: Trade Table contains a field COMPSPOTID with value Y or N
        /// if Y then it is soptmarket.then sattlement days =1
        /// </summary>
   
        [HttpPost]
        public ActionResult ImportTrade(string stockExchange = null, string broker = null)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
              //  return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };
            }
            int i = 0;
            string lines;
            Entities db = new Entities(Session["Connection"] as EntityConnection);
            int flag = 0;

            int row = 0;
            double NetSell = 0;
            double NetCommision = 0;
            double NetAmount = 0;
       

            if (stockExchange != null && !string.IsNullOrEmpty(stockExchange) && broker !=null && !string.IsNullOrEmpty(broker))
            {
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader oTradeFileList = new StreamReader(file.InputStream);
                    try
                    {
                        while ((lines = oTradeFileList.ReadLine()) != null)
                        {
                            string[] words = lines.Split('~');

                            string date = words[7];
                            DateTime oTransactionDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);

                            //check data of this date in trade table is this file already uploaded if record found delete all. added 15-12-16 after discussed with asif vai via Skype
                            if (flag == 0)
                            {
                                List<TRADE> listTrade = db.TRADEs.Where(t => t.TRANSACTIONDATE == oTransactionDate).ToList();

                                foreach (var data in listTrade)
                                {
                                    TRADE oldTrade = new TRADE();
                                    oldTrade = data;
                                    db.TRADEs.Remove(oldTrade);
                                    db.SaveChanges();
                                }

                                try
                                {
                                    db.SaveChanges();
                                    flag++;
                                }
                                catch (Exception ex)
                                {
                                    return new JsonResult { Data ="Previous data remove error:"+ ex.Message };
                                }
                            }
                            TRADE oTrade = new TRADE();

                            oTrade.CREATEDBY = Session["UserId"].ToString();
                            oTrade.CREATEDDATE = DateTime.Today;
                            oTrade.LASTUPDATED = DateTime.Today;
                            oTrade.LASTUPDATEDBY = Session["UserId"].ToString();
                            oTrade.ORDERNO = words[0].ToLower();
                            INSTRUMENT instrument = new INSTRUMENT();
                            oTrade.REFERENCE = Guid.NewGuid().ToString();
                            BROKER oBroker = new BROKER();
                            oBroker = db.BROKERs.Where(bkr => bkr.REFERENCE == broker).FirstOrDefault();
                            if (oBroker != null)
                                oTrade.BROKER_REFERENCE = oBroker.REFERENCE;

                            oTrade.STOCKEXCHANGE = stockExchange;

                            string shortName = words[1].ToString();
                            string ISIS = words[2].ToString();
                            
                          //  instrument = db.INSTRUMENTs.Where(instru => instru.SHORTNAME == shortName).FirstOrDefault(); //26-APR-17

                            instrument = db.INSTRUMENTs.Where(instru => instru.ISIN == ISIS).FirstOrDefault();  //add 26-APR-17

                            if (instrument == null)
                            {
                                oTrade.ISEXCEPTION = "Y";
                                oTrade.EXCEPTIONDETAILS = "Instrument is not found.";
                                oTrade.MEMO = shortName;
                             
                                return new JsonResult { Data = "Instrument " + shortName + " is not found.Please add this instrument." };
   
                            }
                            else if (instrument.STATUS == "Closed")
                            {
                             return new JsonResult { Data = "Instrument " + shortName + " is Closed.Please Active this instrument." };
                            }
                            else
                            {
                                oTrade.INSTRUMENT_REFERENCE = instrument.REFERENCE;
                            }

                            //if (oTrade.INSTRUMENT == null)
                            oTrade.MEMO = words[1].ToString();

                            oTrade.INSTRUMENTCATEGORY = words[17].ToString();                        //instrument.INSTRUMENTCATEGORY.DESCRIPTION;
                            oTrade.TRADER = words[3].ToString();
                            oTrade.TRANSACTIONTYPE = words[4].ToString();
                            oTrade.SHAREQUANTITY = Convert.ToDecimal(words[5].ToString());
                            oTrade.RATE = Convert.ToDecimal(words[6].ToString());
                            //string date = words[7];
                            string time = words[8];
                            //DateTime oTransactionDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);
                            oTrade.TRANSACTIONDATE = oTransactionDate;
                            oTrade.TRANSACTIONTIME = DateTime.ParseExact(time, "HH:mm:ss", null);
                            oTrade.MARKET = words[9].ToString();
                            oTrade.FILETYPE = words[10].ToString();
                            oTrade.HOWLATYPE = words[11].ToString();
                            oTrade.FOREIGNFLAG = words[12].ToString();
                            oTrade.INVESTORACREF = words[13].ToString();
                            oTrade.CONTRACTNO = words[15].ToString();
                            oTrade.COMPSPOTID = words[16].ToString();

                            string MarketGroup = words[17].ToString();
                           
                            //MaturedDate Calculation added by rakibul <15-Dec-2016>
                            //take sattlementDays from InstrumentCatagory
                            try
                            {
                                if (oTrade.COMPSPOTID == "Y") //Spot Market trade so matured date will be 1 days later
                                {
                                    oTrade.MATUREDDATE = oTrade.TRANSACTIONTYPE == "B" ? GetSettlementDate(oTrade.TRANSACTIONDATE.Value, 1) : oTransactionDate;

                                }
                                else
                                {
                                    var sattlement = db.INSTRUMENTCATEGORies.Where(t => t.CODE == MarketGroup && t.STOCKEXCHANGE == stockExchange).SingleOrDefault(); //&& t.STATUS == "Active"
                                    if (sattlement.STATUS == "Active")
                                        oTrade.MATUREDDATE = oTrade.TRANSACTIONTYPE == "B" ? GetSettlementDate(oTrade.TRANSACTIONDATE.Value, Convert.ToInt32(sattlement.SETTLEMENTDAYS)) : oTransactionDate;
                                    else
                                        return new JsonResult { Data = "INSTRUMENTCATEGOR Market " + MarketGroup + " Status is Closed." };
                                }
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Sattlement Days Market Group:" + MarketGroup + " Error:" + ex.Message };
                            }
                            oTrade.BRANCHREF = Guid.NewGuid().ToString();
                            oTrade.TOTALAMOUNT = oTrade.RATE * oTrade.SHAREQUANTITY;
                            oTrade.COMMISSION = oTrade.TOTALAMOUNT * (oBroker.COMMISSIONRATE / 100);

                            if (oTrade.COMMISSION < oBroker.MINIMUMAMOUNT)
                            {
                                oTrade.COMMISSION = oBroker.MINIMUMAMOUNT;
                            }

                            oTrade.NETAMOUNT = 0;
                            oTrade.NETAMOUNT = oTrade.TRANSACTIONTYPE == "B" ? oTrade.TOTALAMOUNT + oTrade.COMMISSION : oTrade.TOTALAMOUNT - oTrade.COMMISSION;
                            oTrade.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";
                            oTrade.FILENAME = fileName;
                            oTrade.ISDEALER = "N";

                            // add hawla laga Charge 14-March-17
                            try
                            {
                                var getCharge = db.TRADINGCHARGEs.Where(t => t.MARKET == oTrade.MARKET).SingleOrDefault();
                                if (getCharge != null)
                                {
                                    oTrade.HOWLA = getCharge.TRANSACTIONFEE.HasValue ? getCharge.TRANSACTIONFEE.Value : 0;

                                    if (getCharge.CHARGERATE.HasValue)
                                        oTrade.LAGA = oTrade.TOTALAMOUNT * getCharge.CHARGERATE.Value / 100;
                                    if (getCharge.TAX.HasValue)
                                        oTrade.TAX = oTrade.TOTALAMOUNT * getCharge.TAX.Value / 100;
                                }
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Trading Charge Error:" + ex.Message };
                            }

                            oCommonFunction.CustomObjectNullValidation<TRADE>(ref oTrade);
                            db.TRADEs.Add(oTrade);
                            db.SaveChanges();
                            row++;
                        }
                        db.SaveChanges();
                    }
                    catch (NullReferenceException mx)
                    {
                        string ms = mx.Message;
                        return new JsonResult { Data ="Value not found:"+ mx.Message };
                    }
                    catch (Exception ex)
                    {
                        //Exception should be catch(DbEntityValidationException dbEx)
                        //foreach (var validationErrors in dbEx.EntityValidationErrors)
                        //{
                        //    foreach (var validationError in validationErrors.ValidationErrors)
                        //    {
                        //        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        //    }
                        //}
                        return new JsonResult { Data = ex.Message };
                    }

                }

                TempData["TranDate"] = TempData["TranDate"];
                return new JsonResult { Data = "Upload Successful" };
            }
            else {
                return new JsonResult { Data = "Please Select a Stock Exchage and Broker." };
            }
        }

            


        public DateTime GetSettlementDate(DateTime startDate, int settlementDays)
        {

            Entities db = new Entities(Session["Connection"] as EntityConnection);          

            List<HOLIDAYCALENDER> holidayList = db.HOLIDAYCALENDERs.ToList();
            WEEKEND weekened = db.WEEKENDs.FirstOrDefault();

            DateTime settlementDate = startDate;
            try
            {
                while (settlementDays > 0)
                {
                    settlementDate = settlementDate.AddDays(1);
                    if (!IsHoliday(settlementDate,weekened.WEEKEND_ONE,weekened.WEEKEND_TWO,holidayList))
                        settlementDays--;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return settlementDate;
        }

        public bool IsHoliday(DateTime date, string Weekend1, string Weekend2, List<HOLIDAYCALENDER> Holidays)
        {
            try
            {
                string dayOfTheDate = date.DayOfWeek.ToString();

                if (dayOfTheDate.ToLower() == Weekend1.ToLower() || dayOfTheDate.ToLower() == Weekend2.ToLower())
                    return true;

                var getHolidayDate = from holiday in Holidays
                                     where holiday.HOLIDAY == date
                                     select holiday;

                if (getHolidayDate != null && getHolidayDate.Count() > 0)
                    return true;
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }
   }
}