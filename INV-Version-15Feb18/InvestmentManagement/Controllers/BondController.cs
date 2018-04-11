using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using InvestmentManagement.ViewModel;
using InvestmentManagement.InvestmentManagement.Models;
using System.Data;
using System.Data.EntityClient;
using System.Linq.Expressions;
using System.Web.Script.Serialization;
using System.Globalization;
using InvestmentManagement.App_Code;

using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Web.Helpers;

namespace InvestmentManagement.Controllers
{
    public class BondController : Controller
    {
       // http://www.binaryintellect.net/articles/4a00a9ce-73e5-4d89-aaae-2d835eca0854.aspx
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /Bond/
        
        
        [HttpGet]
        public ActionResult ListBond(string sortdir, string sort, string sortdefault, int? rows, string lblbreadcum, string PagingType, DateTime? fromdate, DateTime? toDate, string BondNo, string FINANCIALINSTITUTION_REFERENCE, string STATUS)
        {
            try
            {
                
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<BOND> gridModels = new GridModel<BOND>();
                List<BOND> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ? 15 : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;

                //Paging direction
                //switch (PagingType)
                //{
                //    case "Next":
                //        Session["pageNo"] = (int)Session["pageNo"] + 1;
                //        break;
                //    case "Prev":
                //        Session["pageNo"] = (int)Session["pageNo"] - 1;
                //        break;
                //    default:
                //        Session["pageNo"] = 1;
                //        ViewBag.Prev = "disabled";
                //        ViewBag.PrevNotActive = "not-active";
                //        break;
                //}
                
                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
                               

                Entities db=new Entities(Session["Connection"] as EntityConnection);
               // models = db.BONDs.Include("FINANCIALINSTITUTION").AsNoTracking().OrderBy(sort + " " + sortdir).ToList();

                if (STATUS == null && string.IsNullOrEmpty(STATUS))
                {
                    models = db.BONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.STATUS == ConstantVariable.STATUS_PENDING).OrderBy(t=>t.OPENINGDATE).ToList();
                }
                else
                {
                    models = db.BONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.STATUS == STATUS).OrderByDescending(t=>t.MATURITYDATE).ToList();
                }

               // models.Where(t=>t.FINANCIALINSTITUTION.NAME)

                if (fromdate != null)
                    models = models.Where(t => t.OPENINGDATE >= fromdate).ToList();
                
                if (toDate != null)
                    models = models.Where(t => t.OPENINGDATE <= toDate).ToList();

                if (BondNo != null && !string.IsNullOrEmpty(BondNo))
                    models = models.Where(t => t.BONDID == BondNo).ToList();

                gridModels.DataModel = models;
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), 
                Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), 
                this.ControllerContext.RouteData.Values["controller"].ToString());
            
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }

                var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "STATUS").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app => app.DESCRIPTION).ToList();

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");
               
                if(Convert.ToString(TempData["Approved"]) == "Approved")
                {
                   // ViewBag.Message = Convert.ToString(TempData["result"]);
                    if (Convert.ToString(TempData["result"]) != "ok")
                        ViewBag.Message = Convert.ToString(TempData["Error"]);
                }
                
            
                return PartialView("ListBond", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }


        [HttpGet]
        public JsonResult EfficentPaging(int? page,string STATUS)
        {
            Entities db = new Entities(Session["Connection"] as EntityConnection);
            int skip = page.HasValue ? page.Value - 1 : 0;

            List<BOND> models = new List<BOND>();
            if (STATUS == null && string.IsNullOrEmpty(STATUS))
            {
                models = db.BONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.STATUS == ConstantVariable.STATUS_PENDING).OrderBy(t => t.OPENINGDATE).ToList();
            }
            else
            {
                models = db.BONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.STATUS == STATUS).OrderByDescending(t => t.MATURITYDATE).ToList();
            }
            // List<Student> listStudent = new List<Student>();
            
            var data = models.Skip(skip * 5).Take(5).ToList();
            var grid = new WebGrid(data);
            
            
            
            var htmlString = grid.GetHtml(tableStyle: "WebGrid", headerStyle: "header", alternatingRowStyle: "alt", htmlAttributes: new { id = "DataTable" });
            
            
            return Json(new { Data = htmlString.ToHtmlString(), Count = models.Count() / 5 }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult AddBond()
        {
            ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
            ViewBag.Header = "Add " + Session["currentPage"];

            var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY.ToLower() == "TenureTerm".ToLower()).FirstOrDefault().REFERENCE.ToString();
            var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();

            var bondTypeRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Bond".ToLower() && app.PROPERTY == "BondType").FirstOrDefault().REFERENCE.ToString();
            var bondTypeList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == bondTypeRef).ToList();

            var interestModeRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
            var interestModeList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestModeRef).ToList();

            var compoundInterestIntervalRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
            var compoundInterestIntervalList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == compoundInterestIntervalRef).ToList();

            ViewBag.bondTypeList = new SelectList(bondTypeList, "DESCRIPTION", "DESCRIPTION");
            ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
            ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION", "Years");

           // ViewBag.InterestMode = new SelectList(interestModeList, "DESCRIPTION", "DESCRIPTION", "Flat");
            // ViewBag.CompoundInterestInterval = new SelectList(compoundInterestIntervalList, "DESCRIPTION", "DESCRIPTION", "HalfYearly");
            string DefaultId = "6";
            ViewBag.INTERESTMODE = interestModeList.ToList()
                    .Select(x => new SelectListItem
                    {
                        Value = x.CODE.ToString(),
                        Text = x.DESCRIPTION,
                        Selected = (x.CODE == "Flat")
                    });
            
     

            ViewBag.COMPOUNDINTERESTINTERVAL = compoundInterestIntervalList.ToList()
               .Select(x => new SelectListItem
               {
                   Value = x.CODE.ToString(),
                   Text = x.DESCRIPTION,
                   Selected = (x.CODE == DefaultId)
               });

            var proposaedlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "FDR Note" && app.PROPERTY == "ProposedAction").FirstOrDefault().REFERENCE.ToString();
            var actionlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == proposaedlist).ToList();
            ViewBag.ProposaedAction = new SelectList(actionlist, "DESCRIPTION", "DESCRIPTION");

            BOND model = new BOND();
            return PartialView(model);
        }
        

        [HttpPost]
        public ActionResult AddBond(BOND oBOND, string AuctionType)  // BOND oBOND
        {
           try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (Request.IsAjaxRequest())
                {
                    
                    oBOND.REFERENCE = Guid.NewGuid().ToString();
                    oBOND.CREATEDBY = Session["UserId"].ToString();
                    oBOND.CREATEDDATE = DateTime.Now;
                    oBOND.LASTUPDATED = DateTime.Now;
                    oBOND.STATUS = ConstantVariable.STATUS_PENDING;

                    if (AuctionType == "Auction")
                        oBOND.AUCTION = oBOND.AUCTION + " " + AuctionType;

                    oCommonFunction.CustomObjectNullValidation<BOND>(ref oBOND);
                    
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.BONDs.Add(oBOND);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListBond", "Bond");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }        

        [HttpGet]
        public ActionResult EditBond(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                BOND oBOND = new BOND();
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oBOND = db.BONDs.Include("FINANCIALINSTITUTION").Where(b => b.REFERENCE == id).SingleOrDefault();
                    string[] list = oBOND.AUCTION.Split(' ');
                    oBOND.AUCTION = list[0];                
                }

                ViewBag.FinancialInstitutionName = oBOND.FINANCIALINSTITUTION.NAME.ToString();


                //ViewBag.test = "test";
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                
                ViewBag.Header = "Update " + Session["currentPage"];


                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY.ToLower() == "TenureTerm".ToLower()).FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();



                var bondTypeRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Bond".ToLower() && app.PROPERTY == "BondType").FirstOrDefault().REFERENCE.ToString();
                var bondTypeList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == bondTypeRef).ToList();



                var interestModeRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var interestModeList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestModeRef).ToList();

                var compoundInterestIntervalRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
                var compoundInterestIntervalList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == compoundInterestIntervalRef).ToList();

                
                ViewBag.bondTypeList = new SelectList(bondTypeList, "DESCRIPTION", "DESCRIPTION");
                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION");
               // ViewBag.InterestMode = new SelectList(interestModeList, "DESCRIPTION", "DESCRIPTION");
               // ViewBag.CompoundInterestInterval = new SelectList(compoundInterestIntervalList, "DESCRIPTION", "DESCRIPTION");

               
                ViewBag.INTERESTMODE = interestModeList.ToList()
                        .Select(x => new SelectListItem
                        {
                            Value = x.CODE.ToString(),
                            Text = x.DESCRIPTION,
                            Selected = (x.CODE == oBOND.INTERESTMODE)
                        });
                
                ViewBag.COMPOUNDINTERESTINTERVAL = compoundInterestIntervalList.ToList()
                   .Select(x => new SelectListItem
                   {
                       Value = x.CODE.ToString(),
                       Text = x.DESCRIPTION,
                       Selected = (x.CODE == oBOND.COMPOUNDINTERESTINTERVAL)
                   });

                return PartialView("EditBond", oBOND);
            }

            catch (Exception ex)
            {
                throw ex;

            }
        }

        [HttpGet]
        public ActionResult PVIRCalculation(BOND oBOND)
        {
            string json = null;
            string message = "";
            try
            {
                message = CheckBond(oBOND);

                if (message == "ok")
                {

                   

                    BOND newBond = new BOND();
                    newBond.COSTPRICE = oBOND.FACEVALUE + oBOND.PREMIUMPAID - oBOND.DISCOUNT;  // oBOND.PREMIUMPAID + oBOND.BUYINGPRICE;            //Convert.ToDecimal((oBOND.FACEVALUE * oBOND.BUYINGPRICE) / 100);

                    if (oBOND.DISCOUNT != null && oBOND.DISCOUNT > 0)  // if (oBOND.COMMISSION != null && oBOND.COMMISSION > 0)
                        newBond.TOTALCOMMISSIONGAIN = Convert.ToDecimal(oBOND.FACEVALUE - newBond.COSTPRICE);
                    else
                        newBond.TOTALCOMMISSIONGAIN = 0;


                    if (oBOND.TENURETERM == ConstantVariable.TENURETERM_YEARS)
                        newBond.MATURITYDATE = oBOND.BONDISSUEDATE.Value.AddYears(Convert.ToInt32(oBOND.TENURE.Value));
                    else if (oBOND.TENURETERM == ConstantVariable.TENURETERM_MONTHS)  // "Months"
                        newBond.MATURITYDATE = oBOND.BONDISSUEDATE.Value.AddMonths(Convert.ToInt32(oBOND.TENURE.Value));
                    else if(oBOND.TENURETERM == ConstantVariable.TENURETERM_DAYS)
                        newBond.MATURITYDATE = oBOND.BONDISSUEDATE.Value.AddDays(Convert.ToInt32(oBOND.TENURE.Value));                                       

                    if (newBond.TOTALCOMMISSIONGAIN <= 0)
                    {
                        newBond.TOTALCOMMISSIONGAIN = 0;
                    }

                    newBond.PREMIUMPAID = oBOND.PREMIUMPAID; //Convert.ToDecimal(newBond.COSTPRICE - oBOND.FACEVALUE);

                    if (newBond.PREMIUMPAID <= 0)
                    {
                        newBond.PREMIUMPAID = 0;
                    }

                    #region HoldingPeriodCalculation
                    //if (oBOND.OPENINGDATE != null && oBOND.OPENINGDATE > oBOND.BONDISSUEDATE)
                    //{
                    //    HoldingInterestModel model;
                    //    model = ConstantVariable.GetHoldingPeriod(oBOND.OPENINGDATE.Value, oBOND.BONDISSUEDATE.Value, oBOND.FACEVALUE.Value, oBOND.COUPONRATE.Value, oBOND.ANNUALDAYS.Value);
                    //    newBond.HOLDINGPERIOD = model.holdingPeriod;
                    //    newBond.HOLDINGINTERESTPAID = model.HoldingInterest;
                    //    //newBond.HOLDINGINTERESTPAID = (oBOND.FACEVALUE * oBOND.COUPONRATE) / 2; //6(half year) month Interest
                    //    //decimal.Round(Convert.ToDecimal((((oBOND.FACEVALUE * oBOND.COUPONRATE) / 100) / oBOND.ANNUALDAYS) * newBond.HOLDINGPERIOD), 2, MidpointRounding.AwayFromZero);
                    //}
                    //else
                    //{
                    //    newBond.HOLDINGPERIOD = 0;
                    //    newBond.HOLDINGINTERESTPAID = 0;
                    //}
                    #endregion

                    newBond.GROSSINTEREST = Convert.ToDecimal(((oBOND.FACEVALUE * oBOND.COUPONRATE) / 100) * oBOND.TENURE);
                    newBond.SOURCETAX = newBond.GROSSINTEREST * oBOND.TAXRATE / 100;

                    newBond.TOTALPURCHASEAMOUNT = oBOND.BUYINGPRICE;
                                                //decimal.Round(Convert.ToDecimal(newBond.COSTPRICE + newBond.PREMIUMPAID + newBond.HOLDINGINTERESTPAID - newBond.TOTALCOMMISSIONGAIN), 2, MidpointRounding.AwayFromZero);                   
                    newBond.EXCISEDUTY = 0; // oFi.EXCISEDUTY;
                    newBond.OTHERCHARGE = 0;  // oFi.OTHERCHARGE;
                    newBond.NETINTEREST = Convert.ToDecimal(newBond.GROSSINTEREST - newBond.SOURCETAX - newBond.EXCISEDUTY - newBond.OTHERCHARGE);
                    //newBond.SOURCETAX = oFi.TAXRATE;
                    newBond.REJECTEDBY = newBond.MATURITYDATE.Value.ToString("dd-MMM-yyyy");
                    json = new JavaScriptSerializer().Serialize(newBond);
                }
            }
            catch (Exception ex)
            {
                message = "An error occured." + ex.Message;
            }

            return Json(new { list = json, message = message }, JsonRequestBehavior.AllowGet);
        }

        string CheckBond(BOND bond)
        { 
            string msg="";
            if (bond.FACEVALUE == null || bond.FACEVALUE < 1)
                    msg  += "Face Value must have a positive value. ";
            if(bond.COUPONRATE == null || bond.COUPONRATE <1)
                 msg +="Coupon Rate must have a positive value. ";
            if(bond.TAXRATE == null || bond.TAXRATE < 0)
                msg += "Tax Rate must have a positive value. ";
            if (bond.ANNUALDAYS == null || bond.ANNUALDAYS < 1)
                msg += "Annual Days must have a positive value. ";
            if(bond.DISCOUNT == null || bond.DISCOUNT <0)
                msg += "Discount must have Zero or a positive value. ";
            //if(bond.COMMISSION == null || bond.COMMISSION <0)
            //    msg += "Commission must have Zero or a positive value. ";
            if(bond.PREMIUMPAID == null || bond.PREMIUMPAID <0)
                msg += "Premium must have Zero or a positive value. ";
            if(bond.BUYINGPRICE == null || bond.BUYINGPRICE < 1)
                msg += "Buying price must have a positive value. ";
            if (bond.TENURE == null || bond.TENURE < 1)
                msg += "Tenure is required.";
            if (bond.TENURETERM == null && string.IsNullOrEmpty(bond.TENURETERM))
                msg += "Select a Tenure Terms.";

            if (msg == "")
                msg = "ok";

            return msg;        
        }

        [HttpPost]
        public ActionResult EditBond(BOND oBOND, string AuctionType)
        {
            //oAPPLICATIONUSER.CREATEDBY = "BOSL";
            //oAPPLICATIONUSER.CREATEDDATE = DateTime.Parse("05-04-2015");
            //oAPPLICATIONUSER.DEPARTMENT = department;
            //oAPPLICATIONUSER.LASTUPDATED = DateTime.Parse("04-04-2015");
            //oAPPLICATIONUSER.LASTUPDATEDBY = "BOSL";
            //oAPPLICATIONUSER.USERID = "1254";
            //oAPPLICATIONUSER.LASTLOGIN = DateTime.Now;

            try
            {
                string[] list = oBOND.AUCTION.Split(' ');

                if (AuctionType == "Auction")
                {
                    oBOND.AUCTION = list[0] + " " + AuctionType;
                }
                else
                    oBOND.AUCTION = list[0];

                oBOND.LASTUPDATED = DateTime.Now;
                oBOND.LASTUPDATEDBY = Session["UserId"].ToString();
                oCommonFunction.CustomObjectNullValidation<BOND>(ref oBOND);

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    db.Entry(oBOND).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListBond", "Bond");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult ApprovedBond(string id)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            //ViewBag.test = "test";
            ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.breadcum = oCommonFunction.GetDetailsPath(Session["Path"] as IHtmlString, "BOND", Session["currentPage"].ToString(), "ApprovedBond", id);

            ViewBag.Header = "Approved " + Session["currentPage"];

            var oBOND = new Entities(Session["Connection"] as EntityConnection).BONDs.Where(b => b.REFERENCE == id).SingleOrDefault();

            return PartialView("Approved", oBOND);
        }

        [HttpPost]
        public ActionResult ApproveBond(BOND model)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                TempData["Approved"] = "Approved";

                BOND oBOND = new BOND();
                List<CHARGEHEAD> oChargeHeads = new List<CHARGEHEAD>();
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oBOND = db.BONDs.Include("FINANCIALINSTITUTION").Where(b => b.REFERENCE == model.REFERENCE).SingleOrDefault();
                    oChargeHeads = db.CHARGEHEADs.Where(chargeHeader => chargeHeader.CHARGETYPE == "BOND" && chargeHeader.ISACTIVE == "Y").ToList();


                    DateTime interestDate = oBOND.BONDISSUEDATE.Value;
                    int seqNo = 1;

                    while (interestDate.Date <= oBOND.MATURITYDATE.Value.Date)
                    {
                        interestDate = interestDate.AddMonths(6);

                        if (interestDate.Date > oBOND.MATURITYDATE.Value.Date)
                            break;

                        //Add Interest Schedule
                        GOVBONDINTERESTSCHEDULE interestSchedule = new GOVBONDINTERESTSCHEDULE();
                        interestSchedule.BOND = oBOND;
                        interestSchedule.BOND_REFERENCE = oBOND.REFERENCE;
                        interestSchedule.DUEDATE = interestDate;
                        interestSchedule.GROSSINTEREST = ((oBOND.FACEVALUE * (oBOND.COUPONRATE / 100)) / 2); //*half year so divided by 2

                        interestSchedule.TAXRATE = oBOND.TAXRATE;

                        if (oBOND.SOURCETAX == 0)
                        {
                            interestSchedule.SOURCETAX = 0;                           
                        }
                        else { 
                          var GrossYearly=  (oBOND.FACEVALUE * oBOND.COUPONRATE) / 100;  //1 year gross
                          interestSchedule.SOURCETAX = (GrossYearly * (oBOND.TAXRATE / 100)) / 2; //half yearly Source Tax                         
                        }

                        interestSchedule.OTHERCHARGE =0;
                       
                        //set charges
                        //foreach (CHARGEHEAD oCHARGEHEAD in oChargeHeads)
                        //{
                        //    decimal? chargeAmount = 0;

                        //    if (oCHARGEHEAD.ISPERCENTAGE == "Y")
                        //    {
                        //        chargeAmount = (interestSchedule.GROSSINTEREST * oCHARGEHEAD.AMOUNT) / 100;
                        //    }
                        //    else
                        //        chargeAmount = oCHARGEHEAD.AMOUNT;
                        //    if (oCHARGEHEAD.CODE.ToLower() == "SourceTAX".ToLower())
                        //    {
                        //        interestSchedule.SOURCETAX = +chargeAmount;
                        //        interestSchedule.TAXRATE = oCHARGEHEAD.AMOUNT;
                        //    }
                        //    else
                        //        interestSchedule.OTHERCHARGE = +chargeAmount;
                        //}


                        interestSchedule.NETINTEREST = interestSchedule.GROSSINTEREST - interestSchedule.SOURCETAX - interestSchedule.OTHERCHARGE;
                        interestSchedule.SEQUENCENUMBER = seqNo;
                        interestSchedule.REFERENCE = Guid.NewGuid().ToString();
                        interestSchedule.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";

                        interestSchedule.CREATEDDATE = DateTime.Now;
                        interestSchedule.CREATEDBY = Session["UserId"].ToString();

                        db.GOVBONDINTERESTSCHEDULEs.Add(interestSchedule);


                        seqNo++;
                    }


                    //Post Journal
                    //new TransactionEngine().CreateFinancialTransaction("TBond", oBOND.COSTPRICE, oBOND.BONDISSUEDATE);

                    oBOND.ACCEPTEDDATE = model.ACCEPTEDDATE;          //DateTime.Now;
                    oBOND.ACCEPTEDBY = Session["UserId"].ToString();
                    oBOND.REMARKS = model.REMARKS;

                    oBOND.STATUS = ConstantVariable.STATUS_APPROVED; // "Approved";                    
                    db.Entry(oBOND).State = EntityState.Modified;

                    db.SaveChanges();

                }
                TempData["result"] = "ok";               
            }

            catch (Exception ex)
            {
                //throw ex;

                TempData["Error"] = new HtmlString("<div style=\"color:red;display:inline\">"+ex.Message +"</div>");
            }

            return RedirectToAction("ListBond");
        }


        [HttpGet]
        public ActionResult EncashBond(string id)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            //ViewBag.test = "test";
            ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString,Session["currentPage"].ToString());

            ViewBag.Header = "Encash " + Session["currentPage"];

            var oBOND = new Entities(Session["Connection"] as EntityConnection).BONDs.Where(b => b.REFERENCE == id).SingleOrDefault();

            return PartialView("EncashBond", oBOND);
        }

        [HttpPost]
        public ActionResult EncashBond(BOND model)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                TempData["Approved"] = "Approved";
                BOND oBOND = new BOND();
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                  oBOND = db.BONDs.Include("FINANCIALINSTITUTION").Where(b => b.REFERENCE == model.REFERENCE).SingleOrDefault();
                  oBOND.ENCASHEDDATE = model.ENCASHEDDATE;      //DateTime.Now;
                  oBOND.REMARKS = model.REMARKS;

                  oBOND.STATUS = ConstantVariable.STATUS_ENCASHED;
                  oBOND.LASTUPDATED = DateTime.Now;
                  oBOND.LASTUPDATEDBY = Session["UserId"].ToString();

                  db.Entry(oBOND).State = EntityState.Modified;
                  db.SaveChanges();
                  TempData["result"] = "ok";               
                }
            }
            catch (Exception ex)
            {
                //throw ex;

                TempData["Error"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
            }

            return RedirectToAction("ListBond");
        }


        [HttpPost]
        public ActionResult ProcurementLetter(string id)
        {

            
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }


            var bond = new Entities(Session["Connection"] as EntityConnection).BONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.REFERENCE == id).SingleOrDefault();

            var db = new Entities(Session["Connection"] as EntityConnection);

            string report_name ="BondParticipation-"+ bond.BONDID + ".pdf";

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/BondReport/ParticipationLetter.rdlc");

         

            ReportDataSource rd = new ReportDataSource();

           // rd.Name = "HalfYearInterest";
          
       
          

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("TenureTerm",bond.TENURE+" "+bond.TENURETERM),                          

                             new ReportParameter("FaceValue",bond.FACEVALUE.Value.ToString("N")),
                             new ReportParameter("AuctionDate", bond.BONDISSUEDATE.Value.ToString()), 
                            
                                                         
                           };


            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>14in</PageWidth>" +
                "  <PageHeight>10in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            renderedBytes = lr.Render(reportType);
            return File(renderedBytes, mimeType, report_name);

        
        }

        [HttpGet]
        public ActionResult PostToJournal(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                GOVBONDINTERESTSCHEDULE oBOND = new GOVBONDINTERESTSCHEDULE();
              
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oBOND = db.GOVBONDINTERESTSCHEDULEs.Where(b => b.REFERENCE == id).SingleOrDefault();
                    oBOND.STATUS = "Posted";

                    //Create financial transaction
                    new TransactionEngine().CreateFinancialTransaction("TB-INT", oBOND.GROSSINTEREST, oBOND.DUEDATE);
                    new TransactionEngine().CreateFinancialTransaction("TB-TAX", oBOND.SOURCETAX, oBOND.DUEDATE);
                    new TransactionEngine().CreateFinancialTransaction("TB-CHARGE", oBOND.OTHERCHARGE, oBOND.DUEDATE);

                    db.Entry(oBOND).State = EntityState.Modified;
                    db.SaveChanges();

                }


                return RedirectToAction("ListInterestPaymentSchedule");
            }

            catch (Exception ex)
            {
                throw ex;

            }
        }


        public ActionResult SlotApproved(string Ref,string BondRef)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
                GOVBONDINTERESTSCHEDULE editmodel = new GOVBONDINTERESTSCHEDULE();

                editmodel = new Entities(Session["Connection"] as EntityConnection).GOVBONDINTERESTSCHEDULEs.Include("BOND").AsNoTracking().Where(t => t.REFERENCE == Ref).SingleOrDefault();


                editmodel.STATUS = ConstantVariable.STATUS_APPROVED;
                editmodel.LASTUPDATED = DateTime.Now;
                editmodel.LASTUPDATEDBY = Session["UserId"].ToString();

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    db.Entry(editmodel).State = EntityState.Modified;
                    db.SaveChanges();
                }

                TempData["Error"] = new HtmlString("<div style=\"color:green;display:inline\">Save Successful.</div>");
                return RedirectToAction("ListInterestPaymentSchedule", new { reference = BondRef });
            }
            catch (Exception ex)
            {
                TempData["Error"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
                return RedirectToAction("ListInterestPaymentSchedule", new { reference = BondRef });
            }
        }

        //   public ActionResult ListInterestPaymentSchedule(string sortdir, string sort, string sortdefault, int? rows, string lblbreadcum, string PagingType, DateTime? fromDate, DateTime? toDate, string STATUS)
      
        [HttpGet]
        public ActionResult ListInterestPaymentSchedule(string reference, string lblbreadcum)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
             
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<GOVBONDINTERESTSCHEDULE> gridModels = new GridModel<GOVBONDINTERESTSCHEDULE>();
                List<GOVBONDINTERESTSCHEDULE> models = new List<GOVBONDINTERESTSCHEDULE>();
                
                var bond = new Entities(Session["Connection"] as EntityConnection).BONDs.Where(t => t.REFERENCE == reference).SingleOrDefault();
                ViewBag.Bond = bond;

                models = new Entities(Session["Connection"] as EntityConnection).GOVBONDINTERESTSCHEDULEs.Where(t => t.BOND_REFERENCE == reference).OrderBy(t=>t.DUEDATE).ToList();
               
                gridModels.DataModel = models;
              
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                    Session["Path"] = oCommonFunction.GetDetailsPath(Session["Path"] as IHtmlString, this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), reference);
                }
                
                //Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                //ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetDetailsListPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
              

                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }

                var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "STATUS").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app => app.DESCRIPTION).ToList();

                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");

                ViewBag.Message = Convert.ToString(TempData["Error"]);
                return PartialView("ListInterestPaymentSchedule", gridModels);

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult UpdateSourceTAX(DateTime? fromDate, DateTime? toDate,decimal? sourceTAX)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<GOVBONDINTERESTSCHEDULE> gridModels = new GridModel<GOVBONDINTERESTSCHEDULE>();
                List<GOVBONDINTERESTSCHEDULE> models = new List<GOVBONDINTERESTSCHEDULE>();

                //grid settings                
                gridModels.RowsPerPage = 15 ;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;


                if (fromDate != null)
                    models = new Entities(Session["Connection"] as EntityConnection).GOVBONDINTERESTSCHEDULEs.AsNoTracking().Where(interSchedule => interSchedule.DUEDATE >= fromDate.Value).OrderBy(interSchedule => interSchedule.DUEDATE).ToList();

                if (toDate != null)
                    models = models.Where(interestSchedule => interestSchedule.DUEDATE <= toDate.Value).OrderBy(interestShedule => interestShedule.DUEDATE).ToList();

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    foreach (GOVBONDINTERESTSCHEDULE bondInterestSchedule in models)
                    {
                        bondInterestSchedule.SOURCETAX = (bondInterestSchedule.GROSSINTEREST * sourceTAX) / 100;
                        bondInterestSchedule.TAXRATE = sourceTAX;
                        bondInterestSchedule.NETINTEREST = bondInterestSchedule.GROSSINTEREST - bondInterestSchedule.SOURCETAX - bondInterestSchedule.OTHERCHARGE;

                        db.Entry(bondInterestSchedule).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                }


                gridModels.DataModel = models;

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }



                var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "STATUS").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app => app.DESCRIPTION).ToList();

                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");
                return RedirectToAction("ListInterestPaymentSchedule");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult WorkSheetNote(string Ref)
        {
            BOND bond= new BOND();
             try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                GOVBONDINTERESTSCHEDULE model = new Entities(Session["Connection"] as EntityConnection).GOVBONDINTERESTSCHEDULEs.Where(t => t.REFERENCE == Ref).SingleOrDefault();
                bond = new Entities(Session["Connection"] as EntityConnection).BONDs.Where(t => t.REFERENCE == model.BOND_REFERENCE).SingleOrDefault();

                ViewBag.Bond = bond;

              return PartialView(model);
            
             }
            catch(Exception ex)
             {
               TempData["Error"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
               return RedirectToAction("ListInterestPaymentSchedule", new { reference = bond.REFERENCE });
            }
        }

        [HttpPost]
        public ActionResult WorkSheetNote(GOVBONDINTERESTSCHEDULE model)
        {
             try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
            GOVBONDINTERESTSCHEDULE editmodel = new GOVBONDINTERESTSCHEDULE();

            editmodel = new Entities(Session["Connection"] as EntityConnection).GOVBONDINTERESTSCHEDULEs.Include("BOND").AsNoTracking().Where(t => t.REFERENCE == model.REFERENCE).SingleOrDefault();

            editmodel.MRNO = model.MRNO;
            editmodel.MRDATE = model.MRDATE;
            editmodel.INTERESTRECEIVEDDATE = model.INTERESTRECEIVEDDATE;
            editmodel.REMARKS = model.REMARKS;
            editmodel.LASTUPDATED = DateTime.Now;
            editmodel.LASTUPDATEDBY = Session["UserId"].ToString();

            using (Entities db = new Entities(Session["Connection"] as EntityConnection))
            {
                db.Entry(editmodel).State = EntityState.Modified;
                db.SaveChanges();
            }
             

            TempData["Error"] = new HtmlString("<div style=\"color:green;display:inline\">Save Successful.</div>");
            return RedirectToAction("ListInterestPaymentSchedule", new { reference = model.BOND_REFERENCE });
            }
             catch (Exception ex)
             {
                 TempData["Error"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
                 return RedirectToAction("ListInterestPaymentSchedule", new { reference = model.BOND_REFERENCE });
             }
        }

        public ActionResult WorkSheetReport(string Ref)
        {
            GOVBONDINTERESTSCHEDULE model = new Entities(Session["Connection"] as EntityConnection).GOVBONDINTERESTSCHEDULEs.Where(t => t.REFERENCE == Ref).SingleOrDefault();
            var bond = new Entities(Session["Connection"] as EntityConnection).BONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.REFERENCE == model.BOND_REFERENCE).SingleOrDefault();

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            var db = new Entities(Session["Connection"] as EntityConnection);
            
            string report_name = "TresuryBondWorksheet-";        

            report_name += bond.BONDID+ ".pdf";

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/BondReport/HalfYearInterestTreasuryBond.rdlc");

            var prevInterestDate = model.DUEDATE.Value.AddMonths(-6);

            List<BOND> Interest = new List<BOND>();

            if (model.DUEDATE.Value.Year == prevInterestDate.Year)
            {
                Interest.Add(new BOND { REMARKS = model.DUEDATE.Value.Year.ToString(), GROSSINTEREST = model.GROSSINTEREST, SOURCETAX = model.SOURCETAX, NETINTEREST = model.NETINTEREST });
            }
            else { 
                
                var lastDateOfprevInterestDate= new DateTime(prevInterestDate.Year,12,31);
                double PrevYearsDays = (lastDateOfprevInterestDate - prevInterestDate).TotalDays;

                var PrevGrossInterest =Convert.ToDecimal((model.GROSSINTEREST.Value * Convert.ToDecimal(PrevYearsDays)) / 180);
                var PrevSourceTax = (model.SOURCETAX.Value * Convert.ToDecimal(PrevYearsDays)) / 180;
                var PrevNetInterest = (model.NETINTEREST.Value * Convert.ToDecimal(PrevYearsDays)) / 180;

                Interest.Add(new BOND { REMARKS = prevInterestDate.Year.ToString(), GROSSINTEREST =PrevGrossInterest , SOURCETAX = PrevSourceTax, NETINTEREST = PrevNetInterest });

                Interest.Add(new BOND { REMARKS = model.DUEDATE.Value.Year.ToString(), GROSSINTEREST = model.GROSSINTEREST - PrevGrossInterest, SOURCETAX = model.SOURCETAX - PrevSourceTax, NETINTEREST = model.NETINTEREST - PrevNetInterest });
            
            }

            ReportDataSource rd = new ReportDataSource();

            rd.Name = "HalfYearInterest";
            rd.Value = oCommonFunction.ConvertToDataTable(Interest);

            string PrincipalInterest = null;
            if (model.DUEDATE.Value.Date == bond.MATURITYDATE.Value.Date) //that mean last interest worksheet
            {
                PrincipalInterest = (bond.FACEVALUE.Value + model.NETINTEREST.Value).ToString("N");
            }

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("TenureTerm",bond.TENURE+" "+bond.TENURETERM),
                             new ReportParameter("CuponRate",Convert.ToString(bond.COUPONRATE)),
                             new ReportParameter("BOID",bond.BONDID),
                             new ReportParameter("ReferenceBank",bond.FINANCIALINSTITUTION.NAME),

                             new ReportParameter("FaceValue",bond.FACEVALUE.Value.ToString("N")),
                             new ReportParameter("IssueDate", bond.BONDISSUEDATE.Value.ToString("dd-MMM-yyyy")), 
                             new ReportParameter("MaturityDate", bond.MATURITYDATE.Value.ToString("dd-MMM-yyyy")),
                             new ReportParameter("PrinciplesInterest",PrincipalInterest),
                        
                             new ReportParameter("MRNo", model.MRNO == null ? "" : Convert.ToString(model.MRNO)),
                             new ReportParameter("MRDate",model.MRDATE.HasValue ? model.MRDATE.Value.ToString("dd-MMM-yyyy"):""),
                             new ReportParameter("GrossInterest",model.GROSSINTEREST.Value.ToString("N")),
                             new ReportParameter("SourceTax",model.SOURCETAX.Value.ToString("N")),
                             new ReportParameter("NetInterest",model.NETINTEREST.Value.ToString("N")),
                                                         
                           };


            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>14in</PageWidth>" +
                "  <PageHeight>10in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            renderedBytes = lr.Render(reportType);
            return File(renderedBytes, mimeType, report_name);


        }
    }
}
