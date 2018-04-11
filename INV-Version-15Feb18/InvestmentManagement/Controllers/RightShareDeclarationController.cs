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
    public class RightShareDeclarationController : Controller
    {

        CommonFunction oCommonFunction = new CommonFunction();

        public ActionResult ListRightShare(string sortdir, string sort, int? page, int? rows, DateTime? CreatedDate, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {

                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<RIGHTSHAREDECLARATION> gridModels = new GridModel<RIGHTSHAREDECLARATION>();
                List<RIGHTSHAREDECLARATION> models = new List<RIGHTSHAREDECLARATION>();

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

                models = new Entities(Session["Connection"] as EntityConnection).RIGHTSHAREDECLARATIONs.Where(t => t.RECORDDATE <= CreatedDate).OrderByDescending(t => t.RECORDDATE).Take(gridModels.RowsPerPage).ToList(); ;  //

                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());

                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());


                if (Convert.ToString(TempData["Approved"]) == "Approved")
                {                                     
                        ViewBag.Message = Convert.ToString(TempData["result"]);
                }

                return PartialView("ListRightShare", gridModels);

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult AddRightShare()
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

                ViewBag.InstrumentList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.OrderBy(p => p.SHORTNAME).ToList(), "REFERENCE", "SHORTNAME");

                ViewBag.transactionDate = DateTime.Today;

                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddRightShare(RIGHTSHAREDECLARATION ORightShare)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TempData["Approved"] = "Approved";

                    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                    {
                        return RedirectToAction("LogOut", "Home");
                    }
                       Entities db = new Entities(Session["Connection"] as EntityConnection);                  
                      
                       var Instrument = db.INSTRUMENTs.Where(t => t.REFERENCE == ORightShare.INSTRUMENTACREF).SingleOrDefault();

                      

                       var IsExists = db.RIGHTSHAREDECLARATIONs.Where(t => t.ENTRYDATE == ORightShare.ENTRYDATE && t.INSTRUMENTACREF == ORightShare.INSTRUMENTACREF).SingleOrDefault();

                       if (IsExists == null)
                       {

                           RIGHTSHAREDECLARATION RightShareObj = new RIGHTSHAREDECLARATION();
                           RightShareObj.REFERENCE = Guid.NewGuid().ToString();
                           RightShareObj.CREATEDBY = Session["UserId"].ToString();
                           RightShareObj.CREATEDDATE = DateTime.Now;

                           RightShareObj.RECORDDATE = ORightShare.RECORDDATE;

                           //Investor Account Number
                           RightShareObj.ACCOUNTNUMBER = ConstantVariable.INVESTOR_ACCOUNT_NUMBER;
                           //ORightSaher.ACCOUNTNUMBER;
                           RightShareObj.INSTRUMENT = Instrument;
                           RightShareObj.ISIN = Instrument.ISIN;
                           RightShareObj.PERCENTAGE = ORightShare.PERCENTAGE;
                           RightShareObj.RATIO = ORightShare.RATIO;
                           RightShareObj.BUYRATE = ORightShare.BUYRATE;
                           RightShareObj.ENTRYDATE = ORightShare.ENTRYDATE; //business date

                           oCommonFunction.CustomObjectNullValidation<RIGHTSHAREDECLARATION>(ref RightShareObj);

                           //db.RIGHTSHAREDECLARATIONs.Attach(RightShareObj);
                           db.RIGHTSHAREDECLARATIONs.Add(RightShareObj);

                           //get record from corporate action received to update Premium as BUY Rate
                           var corporateAction = db.CORPORATEACTIONs.Where(t => t.ISIN == Instrument.ISIN && t.CATYPE == ConstantVariable.RightShare && t.RECORDDATE == ORightShare.RECORDDATE).SingleOrDefault(); //t.EFFECTIVEDATE == ORightShare.ENTRYDATE
                           if (corporateAction != null)
                           {
                               corporateAction.PREMIUM = ORightShare.BUYRATE;
                               db.Entry(corporateAction).State = EntityState.Modified;
                           }

                           db.SaveChanges();
                           TempData["result"] = new HtmlString("<div style=\"color:green;display:inline\">Save Successfull.</div>");   
                       }
                       else
                       {
                           TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">Already Exists !!</div>");                       
                       }
                  

                }
                catch (Exception ex)
                {
                    //   throw ex;
                    string message = ex.Message;
                   // return RedirectToAction("Index", "ErrorPage", new { message });
                    TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">"+ex.Message+"</div>");                       
                }
            }

            return RedirectToAction("ListRightShare", "RightShareDeclaration", new { lblbreadcum ="Right Share" });
        

        }

        [HttpGet]
        public ActionResult EditRightShare(string Ref)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                RIGHTSHAREDECLARATION oRightShare = new RIGHTSHAREDECLARATION();
                Entities db = new Entities(Session["Connection"] as EntityConnection);

                oRightShare = db.RIGHTSHAREDECLARATIONs.Where(i => i.REFERENCE == Ref).SingleOrDefault();
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                ViewBag.InstrumentList = new SelectList(db.INSTRUMENTs.OrderBy(p => p.SHORTNAME).ToList(), "REFERENCE", "SHORTNAME", oRightShare.INSTRUMENTACREF);

                return PartialView("EditRightShare", oRightShare);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult EditRightShare(RIGHTSHAREDECLARATION ORightSaher)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                RIGHTSHAREDECLARATION RightShareObj = new RIGHTSHAREDECLARATION();
                Entities db = new Entities(Session["Connection"] as EntityConnection);

             

                RightShareObj = db.RIGHTSHAREDECLARATIONs.Where(i => i.REFERENCE == ORightSaher.REFERENCE).SingleOrDefault();

                var Instrument = db.INSTRUMENTs.Where(t => t.REFERENCE == ORightSaher.INSTRUMENTACREF).SingleOrDefault();

                RightShareObj.ACCOUNTNUMBER = ORightSaher.ACCOUNTNUMBER;
                RightShareObj.INSTRUMENT = Instrument;

             
                //Investor Account Number
                RightShareObj.ACCOUNTNUMBER = ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //ORightSaher.ACCOUNTNUMBER;
                RightShareObj.INSTRUMENT = Instrument;
                RightShareObj.ISIN = Instrument.ISIN;
                RightShareObj.PERCENTAGE = ORightSaher.PERCENTAGE;
                RightShareObj.RATIO = ORightSaher.RATIO;
                RightShareObj.BUYRATE = ORightSaher.BUYRATE;
                RightShareObj.ENTRYDATE = ORightSaher.ENTRYDATE; //business date

                RightShareObj.RECORDDATE = ORightSaher.RECORDDATE;

                RightShareObj.LASTUPDATED = DateTime.Now;
                RightShareObj.LASTUPDATEDBY = Session["UserId"].ToString();

                oCommonFunction.CustomObjectNullValidation<RIGHTSHAREDECLARATION>(ref RightShareObj);
                db.Entry(RightShareObj).State = EntityState.Modified;


                //get record from corporate action receivable to update Premium as BUY Rate
                var corporateAction = db.CORPORATEACTIONs.Where(t => t.ISIN == Instrument.ISIN && t.CATYPE == ConstantVariable.RightShare && t.RECORDDATE == ORightSaher.RECORDDATE).SingleOrDefault(); //t.EFFECTIVEDATE == ORightSaher.ENTRYDATE)
                if (corporateAction != null)
                {
                    corporateAction.PREMIUM = ORightSaher.BUYRATE;
                    db.Entry(corporateAction).State = EntityState.Modified;
                }


                db.SaveChanges();

                return RedirectToAction("ListRightShare", "RightShareDeclaration", new { lblbreadcum = "Right Share" });
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }

        }

    }
}
