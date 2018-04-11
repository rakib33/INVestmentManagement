using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using InvestmentManagement.App_Code;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using System.Data.EntityClient;
using System.Data;

namespace InvestmentManagement.Controllers
{
    public class ScriptTransferController : Controller
    {
       
        CommonFunction oCommonFunction = new CommonFunction();
        Variable _var = new Variable();

        #region ScriptTransfer

        public ActionResult ListScriptTransfer(string sortdir, string sort, int? page, int? rows, DateTime? CreatedDate, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 9999,string TRANSACTIONTYPE=null)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            try
            {
                string IsExist = Convert.ToString(TempData["message"]);

                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<SCRIPTTRANSFER> gridModels = new GridModel<SCRIPTTRANSFER>();
                List<SCRIPTTRANSFER> models = new List<SCRIPTTRANSFER>();

                //grid settings                
                gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;

                if (CreatedDate == null)
                    CreatedDate = DateTime.Now;
                //int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;
                //DateTime dt = DateTime.Parse("12-MAY-15");
                if (TRANSACTIONTYPE == null && string.IsNullOrEmpty(TRANSACTIONTYPE))
                {
                    models = new Entities(Session["Connection"] as EntityConnection).SCRIPTTRANSFERs.Where(t => t.ENTRYDATE <= CreatedDate && (t.TRANSACTIONTYPE == "D" || t.TRANSACTIONTYPE == "R")).OrderByDescending(t => t.ENTRYDATE).Take(gridModels.RowsPerPage).ToList(); 
                }
                else if(TRANSACTIONTYPE=="F") //Fraction Received then filter by Record date
                {
                    models = new Entities(Session["Connection"] as EntityConnection).SCRIPTTRANSFERs.Where(t => t.RECORDDATE <= CreatedDate && t.TRANSACTIONTYPE == TRANSACTIONTYPE).OrderByDescending(t => t.RECORDDATE).Take(gridModels.RowsPerPage).ToList();  
                }
                else
                {
                    models = new Entities(Session["Connection"] as EntityConnection).SCRIPTTRANSFERs.Where(t => t.ENTRYDATE <= CreatedDate && t.TRANSACTIONTYPE == TRANSACTIONTYPE).OrderByDescending(t => t.ENTRYDATE).Take(gridModels.RowsPerPage).ToList(); 
                }

                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
              

                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                if (!string.IsNullOrEmpty(IsExist) && IsExist != null)
                    ViewBag.HtmlStr = IsExist;

                return PartialView("ListScriptTransfer", gridModels);                

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult AddScriptTransfer(string Option)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                ViewBag.InstrumentList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(p => p.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(p => p.SHORTNAME).ToList(), "REFERENCE", "SHORTNAME");
                
                //ViewBag.InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();

                ViewBag.transactionDate = DateTime.Today;

                ViewBag.Option = Option;

                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddScriptTransfer(SCRIPTTRANSFER OScript, string Option)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    
                    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                    {
                        return RedirectToAction("LogOut", "Home");
                    }
                     
                     Entities db = new Entities(Session["Connection"] as EntityConnection);

                     var investor = db.INVESTORs.Where(t => t.ACCOUNTNUMBER == OScript.ACCOUNTNUMBER).SingleOrDefault();
                     
                     if(investor==null)
                            return RedirectToAction("Index", "ErrorPage", new { message="Can not find any Investor of Account Number "+OScript.ACCOUNTNUMBER+"." });

                        var Instrument = db.INSTRUMENTs.Where(t => t.REFERENCE == OScript.INSTRUMENTACREF).SingleOrDefault();

                        //check if an exixting record already have on same entry date  and same Instrument for Trade Order and Fraction or all
                     
                            var IsExists = db.SCRIPTTRANSFERs.Where(t => t.ENTRYDATE == OScript.ENTRYDATE && t.INSTRUMENTACREF == OScript.INSTRUMENTACREF && t.TRANSACTIONTYPE == OScript.TRANSACTIONTYPE).SingleOrDefault();

                            if (Option == "Fraction")
                            {
                                //03-May-17 teliphonic conversation with Imtiaz vai Fraction have Entry option with same Entry/record date
                                IsExists = null;
                            }

                            if (IsExists != null)
                            {       
                                TempData["message"]= "<span style='color:red'>Record already exixts</span>";
                            }
                            else
                            {

                                SCRIPTTRANSFER scriptObj = new SCRIPTTRANSFER();

                                scriptObj.REFERENCE = Guid.NewGuid().ToString();
                                //Investor Account Number
                                scriptObj.ACCOUNTNUMBER = OScript.ACCOUNTNUMBER;
                                scriptObj.INSTRUMENT = Instrument;
                                scriptObj.SHAREQTY = OScript.SHAREQTY;
                                scriptObj.RATE = OScript.RATE;
                                scriptObj.HOWLANUMBER = OScript.HOWLANUMBER;
                                scriptObj.TRANSACTIONTYPE = OScript.TRANSACTIONTYPE;
                                scriptObj.CERTIFICATENO = OScript.CERTIFICATENO;
                                scriptObj.CREATEDBY = Session["UserId"].ToString();
                                scriptObj.CREATEDDATE = DateTime.Now;
                                scriptObj.SHAREQTY = OScript.SHAREQTY;
                                scriptObj.ENTRYDATE = OScript.ENTRYDATE;

                                //For Trade Order
                                scriptObj.TOTAL = OScript.TOTAL;
                                scriptObj.LOWERLIMIT = OScript.LOWERLIMIT;
                                scriptObj.UPPERLIMIT = OScript.UPPERLIMIT;
                                scriptObj.MAXIMUMQTY = OScript.MAXIMUMQTY;
                                scriptObj.ISFORGOTTRADEORDER = OScript.ISFORGOTTRADEORDER;

                                //For Fraction Received
                                scriptObj.DESCRIPTION = OScript.DESCRIPTION;
                                scriptObj.RECORDDATE = OScript.RECORDDATE;

                                scriptObj.STATUS = ConstantVariable.STATUS_PENDING; //added 11-Jun-17

                                oCommonFunction.CustomObjectNullValidation<SCRIPTTRANSFER>(ref scriptObj);
                                db.SCRIPTTRANSFERs.Add(scriptObj);
                                db.SaveChanges();
                    
                        } 
                }
                catch (Exception ex)
                {
                    //   throw ex;
                    string message = ex.Message;
                    return RedirectToAction("Index", "ErrorPage", new { message });

                }
            }

            ViewBag.Option = Option;
            if (Option == "TradeOrder")
            {
                return RedirectToAction("ListTradeOrder", "ScriptTransfer");
            }
            else if (Option == "Fraction")
            {
                return RedirectToAction("ListFractionReceived", "ScriptTransfer");
            }
            else
            {
                return RedirectToAction("ListScriptTransfer", "ScriptTransfer");
            }
          

        }

        [HttpGet]
        public ActionResult EditScriptTransfer(string Ref, string Option)        
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                SCRIPTTRANSFER oScript = new SCRIPTTRANSFER();
                Entities db = new Entities(Session["Connection"] as EntityConnection);

                oScript = db.SCRIPTTRANSFERs.Where(i => i.REFERENCE == Ref).SingleOrDefault();
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                ViewBag.InstrumentList = new SelectList(db.INSTRUMENTs.Where(p => p.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(p => p.SHORTNAME).ToList(), "REFERENCE", "SHORTNAME", oScript.INSTRUMENTACREF);
                 
                ViewBag.Option = Option;
                return PartialView("EditScriptTransfer", oScript);
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        [HttpPost]
        public ActionResult EditScriptTransfer(SCRIPTTRANSFER oScript, string Option)
        {
            try
            {               
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                SCRIPTTRANSFER oScriptUpdate = new SCRIPTTRANSFER();
                Entities db = new Entities(Session["Connection"] as EntityConnection);

                var investor = db.INVESTORs.Where(t => t.ACCOUNTNUMBER == oScript.ACCOUNTNUMBER).SingleOrDefault();
                if (investor == null)
                    return RedirectToAction("Index", "ErrorPage", new { message = "Can not find any Investor of Account Number " + oScript.ACCOUNTNUMBER + "." });
                             

                oScriptUpdate = db.SCRIPTTRANSFERs.Where(i => i.REFERENCE == oScript.REFERENCE).SingleOrDefault();

                var Instrument = db.INSTRUMENTs.Where(t => t.REFERENCE == oScript.INSTRUMENTACREF).SingleOrDefault();

                oScriptUpdate.ACCOUNTNUMBER = oScript.ACCOUNTNUMBER;
                oScriptUpdate.INSTRUMENT = Instrument;            
                
                oScriptUpdate.RATE = oScript.RATE;
                oScriptUpdate.HOWLANUMBER = oScript.HOWLANUMBER;
                oScriptUpdate.TRANSACTIONTYPE = oScript.TRANSACTIONTYPE;
                oScriptUpdate.CERTIFICATENO = oScript.CERTIFICATENO;
                oScriptUpdate.SHAREQTY = oScript.SHAREQTY;
                oScriptUpdate.ENTRYDATE = oScript.ENTRYDATE;

                oScriptUpdate.LASTUPDATED = DateTime.Now;
                oScriptUpdate.LASTUPDATEDBY = Session["UserId"].ToString();

                //For Trade Order Update
                oScriptUpdate.TOTAL = oScript.TOTAL;
                oScriptUpdate.LOWERLIMIT = oScript.LOWERLIMIT;
                oScriptUpdate.UPPERLIMIT = oScript.UPPERLIMIT;
                oScriptUpdate.MAXIMUMQTY = oScript.MAXIMUMQTY;
                oScriptUpdate.ISFORGOTTRADEORDER = oScript.ISFORGOTTRADEORDER;

                //For Fraction Received
                oScriptUpdate.DESCRIPTION = oScript.DESCRIPTION;
                oScriptUpdate.RECORDDATE = oScript.RECORDDATE;

                oCommonFunction.CustomObjectNullValidation<SCRIPTTRANSFER>(ref oScriptUpdate);
                db.Entry(oScriptUpdate).State = EntityState.Modified;
                db.SaveChanges();

                
                ViewBag.Option = Option;
                if (Option == "TradeOrder")
                {
                   return RedirectToAction("ListTradeOrder", "ScriptTransfer");
                }
                else if (Option == "Fraction")
                {
                   return RedirectToAction("ListFractionReceived", "ScriptTransfer");
                }
                else
                {
                    return RedirectToAction("ListScriptTransfer", "ScriptTransfer");
                }
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }

        }


        #endregion

        public ActionResult Approve(string Ref, string Option)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                SCRIPTTRANSFER oScriptUpdate = new SCRIPTTRANSFER();
                Entities db = new Entities(Session["Connection"] as EntityConnection);             


                oScriptUpdate = db.SCRIPTTRANSFERs.Where(i => i.REFERENCE == Ref).SingleOrDefault();
                oScriptUpdate.STATUS = ConstantVariable.STATUS_APPROVED;

                oScriptUpdate.LASTUPDATED = DateTime.Now;
                oScriptUpdate.LASTUPDATEDBY = Session["UserId"].ToString();

                

                oCommonFunction.CustomObjectNullValidation<SCRIPTTRANSFER>(ref oScriptUpdate);
                db.Entry(oScriptUpdate).State = EntityState.Modified;
                db.SaveChanges();


                ViewBag.Option = Option;

                return RedirectToAction("ListTradeOrder", "ScriptTransfer");

                //if (Option == "TradeOrder")
                //{
                //    return RedirectToAction("ListTradeOrder", "ScriptTransfer");
                //}
                //else if (Option == "Fraction")
                //{
                //    return RedirectToAction("ListFractionReceived", "ScriptTransfer");
                //}
                //else
                //{
                //    return RedirectToAction("ListScriptTransfer", "ScriptTransfer");
                //}
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }

        
        
        }



        /// <summary>
        /// Imtiaz vai first make Trade Order and then upload current date Trade or market price
        /// because he have to trade upon free/matured balance final Quantity of previous day.
        /// such: an instrument has 500 share and today he sold 500 all. So he first make trade order and then after trade he upload trade and market file
        /// because when he upload trade file today this sold instrument free balance will 0(zero).though he alredy trade them.
        /// so trade order must be create before trade of previous portfolio.
        /// BUT BUT if IPO Free balace comes with instant maturity,that means comes today matured today and need to sold today. Then
        /// we have to take market price first.
        /// </summary>
      
        public ActionResult TradeOrderCalculate(string instrumentRef,double Qty,string tranType,DateTime? Date,string forgot)
        {
            string MissingBusinessDate = "";
            try
            {
               
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    _var.ShortName = db.INSTRUMENTs.Where(t => t.REFERENCE == instrumentRef).SingleOrDefault().SHORTNAME;
                    _var.Flag = 0;

                    //Case 1. check given Qty with Free balance of this TradeCode of given Entry Date not previous Date that was before according imtiaz mailed
                    //Conversation subject:	IPO Free share trade order error dated: 07-JUN-17,11-JUn-17
                    //Date = Date.Value.AddDays(-1);

                    //take market price of an instrument 

                    if (forgot == "true")
                    {
                        Date = Date.Value.AddDays(-1);
                        MissingBusinessDate = Date.Value.ToString("dd-MMM-yy");
                    }

                    _var.MarketPrice = new Portfolio().GetClosingPrice(_var.ShortName, Date.Value.ToString("dd-MMM-yy"));

                    if (_var.MarketPrice == -1)
                    {
                        _var.message = "Market Price not found.";
                        return Json(new { _var.message, _var.MarketPrice, _var.Total }, JsonRequestBehavior.AllowGet);
                    }

                   if (tranType == "S")
                    {
                        // Case 1.1.this is for IPO.Some Free Bonus matured Instantly (mailed by ATM dated: 07-JUN-17,11-JUn-17) so need to check currentdate
                       var getPortFolio =new Portfolio().GetPortfolioFn("00001",Date.Value.ToString("dd-MMM-yy"),_var.ShortName,null).SingleOrDefault();
                                            
                       if (getPortFolio != null)
                          {
                                                        
                              _var.MaturedBalance =Convert.ToDouble(getPortFolio.MaturedBalance);   //getPortFolio.MaturedBalance;
                                               
                            
                            if (Qty > _var.MaturedBalance) //For Sell given Quantity must <= Matured Balance
                            {
                                _var.message = "The given Share Quantity of Instrument " + _var.ShortName + "  is exceeds its Matured Balance " + getPortFolio.MaturedBalance + " of given Business Date " + Date.Value.ToString("dd-MMM-yy");
                            }
                            else
                            {
                                _var.Total = Qty * _var.MarketPrice;
                                _var.message = "1";
                            }
                         }
                       else
                       {
                        _var.message = "No record found of instrument "+ _var.ShortName+". Please add price file and trade file of instrument "+ _var.ShortName;
                       }

                     }
                  else if (tranType == "B")
                    {                    
                      
                            _var.Total = Qty * _var.MarketPrice;
                            _var.message = "1";
                     
                    }
                  else
                  _var.message = "For Trade Order transaction Type must Buy Order or Sale Order.";
                  
                }
            }
            catch (Exception ex)
            {
                _var.message = ex.Message;
            }
            return Json(new { _var.message, _var.MarketPrice, _var.Total, MissingBusinessDate }, JsonRequestBehavior.AllowGet);
        }
        
        #region FractionReceived

        public ActionResult ListFractionReceived(string sortdir, string sort, int? page, int? rows, DateTime? CreatedDate, DateTime? ToDate, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 9999)
        {
            try
            {
                string IsExist = Convert.ToString(TempData["message"]);

                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<SCRIPTTRANSFER> gridModels = new GridModel<SCRIPTTRANSFER>();
                List<SCRIPTTRANSFER> models = new List<SCRIPTTRANSFER>();

                //grid settings                
                gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;
                           

                models = new Entities(Session["Connection"] as EntityConnection).SCRIPTTRANSFERs.Where(t => t.TRANSACTIONTYPE == "F").OrderByDescending(t => t.RECORDDATE).Take(gridModels.RowsPerPage).ToList();
                
                if (CreatedDate !=null)
                models = models.Where(t => t.RECORDDATE >= CreatedDate).OrderByDescending(t => t.RECORDDATE).ToList();

                if(ToDate !=null)
                models = models.Where(t => t.RECORDDATE <= ToDate).OrderByDescending(t => t.RECORDDATE).ToList();
              
                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                

                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                if (!string.IsNullOrEmpty(IsExist) && IsExist != null)
                    ViewBag.HtmlStr = IsExist;

                return PartialView("ListFractionReceived", gridModels);

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        #endregion
        
        #region TradeOrder

        public ActionResult ListTradeOrder(string sortdir, string sort, int? page, int? rows, string CreatedDate,string ToDate, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage=15, string TRANSACTIONTYPE = null)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
                string IsExist= Convert.ToString(TempData["message"]);
              
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                //ViewBag.currentRowPerPage = currentRowPerPage;
                GridModel<SCRIPTTRANSFER> gridModels = new GridModel<SCRIPTTRANSFER>();
                List<SCRIPTTRANSFER> models = new List<SCRIPTTRANSFER>();

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

                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);

                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }

                ViewBag.CurrentFilter = filterstring;
                ViewBag.CreatedDate = CreatedDate;
                ViewBag.ToDate = ToDate;

                DateTime fromDate;
                DateTime toDate;
                if (CreatedDate != null && !string.IsNullOrEmpty(CreatedDate) && ToDate != null && !string.IsNullOrEmpty(ToDate))
                {
                    fromDate = Convert.ToDateTime(CreatedDate);
                    toDate = Convert.ToDateTime(ToDate);

                    models = new Entities(Session["Connection"] as EntityConnection).SCRIPTTRANSFERs.Where(t => t.ENTRYDATE >= fromDate && t.ENTRYDATE <= toDate && (t.TRANSACTIONTYPE == "B" || t.TRANSACTIONTYPE == "S")).OrderByDescending(t => t.ENTRYDATE).ThenBy(t=>t.TRANSACTIONTYPE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                }
                else
                {
                    //get max EntryDate                  
                   
                    models = new Entities(Session["Connection"] as EntityConnection).SCRIPTTRANSFERs.Where(t => t.TRANSACTIONTYPE == "B" || t.TRANSACTIONTYPE == "S").OrderByDescending(t => t.ENTRYDATE).ThenBy(t=>t.TRANSACTIONTYPE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                }

                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
               
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                if (!string.IsNullOrEmpty(IsExist) && IsExist != null)
                    ViewBag.HtmlStr = IsExist;

                return PartialView("ListTradeOrder", gridModels);

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        public ActionResult EditTradeOrder(string Ref)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            SCRIPTTRANSFER model = new Entities(Session["Connection"] as EntityConnection).SCRIPTTRANSFERs.Where(t => t.REFERENCE == Ref).SingleOrDefault();

            return PartialView("EditTradeOrder", model);
        }

        [HttpPost]
        public ActionResult EditTradeOrder(SCRIPTTRANSFER model)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                SCRIPTTRANSFER result = db.SCRIPTTRANSFERs.Where(t => t.REFERENCE == model.REFERENCE).SingleOrDefault();
                result.TRANSACTIONTYPE = model.TRANSACTIONTYPE;

                result.LASTUPDATED = DateTime.Now;
                result.LASTUPDATEDBY = Session["UserId"].ToString();

                oCommonFunction.CustomObjectNullValidation<SCRIPTTRANSFER>(ref result);
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ListTradeOrder", "ScriptTransfer");
            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
           
        }

        #endregion


        #region Extra Cash Dividend S

        //public ActionResult ExtraCashDividendList(string sortdir, string sort, int? page, int? rows, DateTime? FromDate,DateTime? ToDate, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 9999)
        //{
        //    try
        //    {
        //        if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
        //        {
        //            return RedirectToAction("LogOut", "Home");
        //        }
        //        string IsExist = Convert.ToString(TempData["message"]);

        //        if (oCommonFunction.CheckSession() == true)
        //        {
        //            return RedirectToAction("LogOut", "Home");
        //        }

        //        //currentRowPerPage=@ViewBag.currentRowPerPage
        //        GridModel<SCRIPTTRANSFER> gridModels = new GridModel<SCRIPTTRANSFER>();
        //        List<SCRIPTTRANSFER> models = new List<SCRIPTTRANSFER>();

        //        //grid settings                
        //        gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
        //        ViewBag.currentRowPerPage = gridModels.RowsPerPage;

        //      //  int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
        //        if (filterstring == null)
        //        {
        //            filterstring = currentFilter;
        //        }


        //        ViewBag.CurrentFilter = filterstring;
        //        //DateTime dt = DateTime.Parse("12-MAY-15");
        //        //for extraCash Dividend Entry Date is CashReceivedDate

        //        models = new Entities(Session["Connection"] as EntityConnection).SCRIPTTRANSFERs.Include("INSTRUMENT").OrderByDescending(t => t.ENTRYDATE).Take(gridModels.RowsPerPage).ToList();
               
        //        if(FromDate !=null && ToDate !=null)
        //            models = new Entities(Session["Connection"] as EntityConnection).SCRIPTTRANSFERs.Include("INSTRUMENT").Where(t => t.ENTRYDATE >= FromDate && t.ENTRYDATE <= ToDate).OrderByDescending(t => t.ENTRYDATE).Take(gridModels.RowsPerPage).ToList();
                             

        //        ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());


        //        if (!string.IsNullOrEmpty(lblbreadcum))
        //        {
        //            Session["currentPage"] = lblbreadcum;
        //        }

        //        Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

        //        ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

        //        if (!string.IsNullOrEmpty(IsExist) && IsExist != null)
        //            ViewBag.HtmlStr = IsExist;

        //        ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");

        //        if (!string.IsNullOrEmpty(IsExist) && IsExist != null)
        //            ViewBag.HtmlStr = IsExist;

        //        gridModels.DataModel = models;

        //        return PartialView("ExtraCashDividendList", gridModels);

        //    }

        //    catch (Exception ex)
        //    {
        //        string message = ex.Message;

        //        return RedirectToAction("Index", "ErrorPage", new { message });
        //    }

        //}



        public ActionResult ExtraDividendReceivedList(string sortdir, string sort, int? page, int? rows, DateTime? FromDate, DateTime? ToDate, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
                string IsExist = Convert.ToString(TempData["message"]);

                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<EXTRADIVIDENDRECEIVED> gridModels = new GridModel<EXTRADIVIDENDRECEIVED>();
                List<EXTRADIVIDENDRECEIVED> models = new List<EXTRADIVIDENDRECEIVED>();

                //grid settings                
                gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;

                //  int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;
               
                if (FromDate != null && ToDate != null)
                    models = new Entities(Session["Connection"] as EntityConnection).EXTRADIVIDENDRECEIVEDs.Include("INSTRUMENT").Where(t => t.CASHRECEIVEDDATE >= FromDate && t.CASHRECEIVEDDATE <= ToDate).OrderByDescending(t => t.CASHRECEIVEDDATE).Take(gridModels.RowsPerPage).ToList();


                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());


                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                if (!string.IsNullOrEmpty(IsExist) && IsExist != null)
                    ViewBag.HtmlStr = IsExist;

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");

                if (!string.IsNullOrEmpty(IsExist) && IsExist != null)
                    ViewBag.HtmlStr = IsExist;

                gridModels.DataModel = models;

                @ViewBag.HtmlStr = Convert.ToString(TempData["Error"]);

                return PartialView("ExtraDividendReceivedList", gridModels);

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        public ActionResult AddExtraDividend()
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                ViewBag.InstrumentList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(p => p.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(p => p.SHORTNAME).ToList(), "REFERENCE", "SHORTNAME");                                           
                return PartialView();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                TempData["Error"] ="<p style=\"color:red\">"+ ex.Message+"</p>";
            }
            return RedirectToAction("ExtraDividendReceivedList");
            
        }

        [HttpPost]
        public ActionResult AddExtraDividend(EXTRADIVIDENDRECEIVED model)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                Entities db = new Entities(Session["Connection"] as EntityConnection);             

                model.REFERENCE = Guid.NewGuid().ToString();
                model.CREATEDBY = Session["UserId"].ToString();
                model.CREATEDDATE = DateTime.Now;

                db.EXTRADIVIDENDRECEIVEDs.Add(model);
                db.SaveChanges();
                TempData["Error"] = "<p style=\"color:green\">Data Save Successful.</p>"; 
                //ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                //ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                //ViewBag.Header = "Add " + Session["currentPage"];
                //ViewBag.InstrumentList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(p => p.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(p => p.SHORTNAME).ToList(), "REFERENCE", "SHORTNAME");
                //return PartialView();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "<p style=\"color:red\">" + ex.Message + "</p>"; 
              
            }

            return RedirectToAction("ExtraDividendReceivedList");
        }


        public ActionResult EditExtraDividend(string Ref)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
                Entities db = new Entities(Session["Connection"] as EntityConnection);

                EXTRADIVIDENDRECEIVED model = db.EXTRADIVIDENDRECEIVEDs.Find(Ref);

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Edit " + Session["currentPage"];
                ViewBag.InstrumentList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(p => p.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(p => p.SHORTNAME).ToList(), "REFERENCE", "SHORTNAME",model.INSTRUMENTACREF);
                
                return PartialView("EditExtraDividend", model);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                TempData["Error"] = "<p style=\"color:red\">" + ex.Message + "</p>";
            }
            return RedirectToAction("ExtraDividendReceivedList");        
        }

        [HttpPost]
        public ActionResult EditExtraDividend(EXTRADIVIDENDRECEIVED model)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
                Entities db = new Entities(Session["Connection"] as EntityConnection);
               
                              
                model.LASTUPDATEDBY = Session["UserId"].ToString();
                model.LASTUPDATED = DateTime.Now;

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Error"] = "<p style=\"color:green\">Data Save Successful.</p>"; 

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                TempData["Error"] = "<p style=\"color:red\">" + ex.Message + "</p>";
            }
            return RedirectToAction("ExtraDividendReceivedList");

        }
        #endregion


        #region SellShareWhenCashPaid
     
        public ActionResult SellMutualFund(string sortdir, string sort, int? page, int? rows, DateTime? FromDate,DateTime? ToDate, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 9999)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            try
            {
                string IsExist = Convert.ToString(TempData["message"]);

                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<SCRIPTTRANSFER> gridModels = new GridModel<SCRIPTTRANSFER>();
                List<SCRIPTTRANSFER> models = new List<SCRIPTTRANSFER>();

                //grid settings                
                gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;

                if (ToDate == null)
                    ToDate = DateTime.Today;
                //int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;
                //DateTime dt = DateTime.Parse("12-MAY-15");
            
                models = new Entities(Session["Connection"] as EntityConnection).SCRIPTTRANSFERs.Where(t => t.TRANSACTIONTYPE == "MS").OrderByDescending(t => t.ENTRYDATE).Take(gridModels.RowsPerPage).ToList();

                if (FromDate.HasValue)
                {
                    models = models.Where(t => t.ENTRYDATE >= FromDate && t.ENTRYDATE <= ToDate).OrderByDescending(t => t.ENTRYDATE).Take(gridModels.RowsPerPage).ToList();
                }
                else
                {
                    models = models.Where(t =>t.ENTRYDATE <= ToDate).OrderByDescending(t => t.ENTRYDATE).Take(gridModels.RowsPerPage).ToList();
                }

                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());


                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                if (!string.IsNullOrEmpty(IsExist) && IsExist != null)
                    ViewBag.HtmlStr = IsExist;

                ViewBag.Message = Convert.ToString(TempData["message"]);

                return PartialView("SellMutualFundList", gridModels);
             
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult AddEditSellMutualFund(string Option, string Ref)  //Option =Add or Edir
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                Entities db = new Entities(Session["Connection"] as EntityConnection);

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = Option+ " " + Session["currentPage"];

                ViewBag.InstrumentList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(p => p.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(p => p.SHORTNAME).ToList(), "REFERENCE", "SHORTNAME");

                ViewBag.transactionDate = DateTime.Today;
                ViewBag.Option = Option;

                if(Option == "Edit")
                {
                    var oScript = db.SCRIPTTRANSFERs.Where(i => i.REFERENCE == Ref).SingleOrDefault();
                    return PartialView("AddEditSellMutualFund",oScript);
                }
                return PartialView("AddEditSellMutualFund");
            }
            catch (Exception ex)
            {
                TempData["message"] = "<span style='color:red'>" + ex.Message + "</span>";
                return RedirectToAction("SellMutualFund", "ScriptTransfer");

            }
        }

        [HttpPost]
        public ActionResult AddEditSellMutualFund(string Option, SCRIPTTRANSFER OScript)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                    {
                        return RedirectToAction("LogOut", "Home");
                    }

                    Entities db = new Entities(Session["Connection"] as EntityConnection);

                    var investor = db.INVESTORs.Where(t => t.ACCOUNTNUMBER == OScript.ACCOUNTNUMBER).SingleOrDefault();

                    if (investor == null)
                    {
                        TempData["message"] = "<span style='color:red'>Can not find any Investor of Account Number " + OScript.ACCOUNTNUMBER + ".</span>";
                        return RedirectToAction("SellMutualFund", "ScriptTransfer");
                     
                    }
                    var Instrument = db.INSTRUMENTs.Where(t => t.REFERENCE == OScript.INSTRUMENTACREF).SingleOrDefault();

                    //check if an exixting record already have on same entry date  and same Instrument for Trade Order and Fraction or all

                    var IsExists = db.SCRIPTTRANSFERs.Where(t => t.ENTRYDATE == OScript.ENTRYDATE && t.INSTRUMENTACREF == OScript.INSTRUMENTACREF && t.TRANSACTIONTYPE == OScript.TRANSACTIONTYPE).SingleOrDefault();


                    if (IsExists != null && Option=="Add")
                    {
                        TempData["message"] = "<span style='color:red'>Record already exixts</span>";
                    }
                    else if (IsExists == null && Option == "Add")
                    {

                        SCRIPTTRANSFER scriptObj = new SCRIPTTRANSFER();

                        scriptObj.REFERENCE = Guid.NewGuid().ToString();
                        //Investor Account Number
                        scriptObj.ACCOUNTNUMBER = OScript.ACCOUNTNUMBER;
                        scriptObj.INSTRUMENT = Instrument;
                        scriptObj.SHAREQTY = OScript.SHAREQTY;
                        scriptObj.RATE = OScript.RATE;
                    
                        scriptObj.TRANSACTIONTYPE = OScript.TRANSACTIONTYPE;
                     
                        scriptObj.CREATEDBY = Session["UserId"].ToString();
                        scriptObj.CREATEDDATE = DateTime.Now;
                        scriptObj.SHAREQTY = OScript.SHAREQTY;
                        scriptObj.ENTRYDATE = OScript.ENTRYDATE;

                      
                        scriptObj.DESCRIPTION = OScript.DESCRIPTION;
                        scriptObj.RECORDDATE = OScript.ENTRYDATE;

                        scriptObj.STATUS = ConstantVariable.STATUS_APPROVED; //added 11-Jun-17

                        oCommonFunction.CustomObjectNullValidation<SCRIPTTRANSFER>(ref scriptObj);
                        db.SCRIPTTRANSFERs.Add(scriptObj);
                        db.SaveChanges();

                        TempData["message"] = "<span style='color:green'>Data Save Successfull.</span>";
                    }
                    else if (IsExists != null && Option == "Edit")
                    {
                        IsExists.ACCOUNTNUMBER = OScript.ACCOUNTNUMBER;
                        IsExists.INSTRUMENT = Instrument;

                        IsExists.RATE = OScript.RATE;

                        IsExists.TRANSACTIONTYPE = OScript.TRANSACTIONTYPE;

                        IsExists.SHAREQTY = OScript.SHAREQTY;
                        IsExists.ENTRYDATE = OScript.ENTRYDATE;

                        IsExists.LASTUPDATED = DateTime.Today;
                        IsExists.LASTUPDATEDBY = Session["UserId"].ToString();

                        //For Trade Order Update
                        IsExists.TOTAL = OScript.TOTAL;
                      
                        //For Fraction Received
                        IsExists.DESCRIPTION = OScript.DESCRIPTION;
                        IsExists.RECORDDATE = OScript.ENTRYDATE;

                        oCommonFunction.CustomObjectNullValidation<SCRIPTTRANSFER>(ref IsExists);
                        db.Entry(IsExists).State = EntityState.Modified;
                        db.SaveChanges();


                        TempData["message"] = "<span style='color:green'>Data Edit Successfull.</span>";
                    }
                }
                catch (Exception ex)
                {
                    //   throw ex;

                    TempData["message"] = "<span style='color:red'>" + ex.Message+"</span>";
                    return RedirectToAction("SellMutualFund", "ScriptTransfer");

                }
            }

            ViewBag.Option = Option;
           
            return RedirectToAction("SellMutualFund", "ScriptTransfer");

        }

       
        #endregion
    }
}




