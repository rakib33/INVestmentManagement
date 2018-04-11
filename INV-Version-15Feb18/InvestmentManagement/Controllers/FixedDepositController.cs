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

using InvestmentManagement.App_Code;

namespace InvestmentManagement.Controllers
{
    public class FixedDepositController : Controller
    {

        CommonFunction oCommonFunction = new CommonFunction();

        private Variable _var = new Variable();
        FDRCalculationValue Obj = new FDRCalculationValue();
        FDRCalculation fdrCalculation = new FDRCalculation();    

        [HttpGet]
        public ActionResult ListFixedDeposit(string sortdir, string sort, string sortdefault, int? rows, string filterstring, string lblbreadcum, string PagingType, DateTime? fromdate,DateTime? toDate, string FINANCIALINSTITUTION_REFERENCE, string STATUS, string FdrNo)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                //added by rakibul get tempdata value from Approved action
                var link = TempData["EmptyDepositeNumber"] as string;
             
                if (link != null && !string.IsNullOrEmpty(link))
                {
                    ViewBag.emptynumber = 1;
                    ViewBag.Message = link;
                }
                //end 

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                //DateTime openingDate = DateTime.Parse(openDate);
                //    openingDate = new DateTime(openingDate.Year, openingDate.Month, openingDate.Day);
                                
               
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<FIXEDDEPOSIT> gridModels = new GridModel<FIXEDDEPOSIT>();
                List<FIXEDDEPOSIT> models = null;

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
                //sort = string.IsNullOrEmpty(sort) == true ? "OPENINGDATE" : sort;  
                //sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;

                sort = string.IsNullOrEmpty(sort) == true ? "MATURITYDATE" : sort;  
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "ASC" : sortdir;
   

                ViewBag.CurrentFilter = filterstring;

                 
                

                if(STATUS == null)
                    models = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.STATUS == ConstantVariable.STATUS_PENDING).OrderByDescending(t=>t.OPENINGDATE).ToList();   // short descending opening date by burhan vai 8-May-17  OrderBy(sort + " " + sortdir).ToList(); 
                
                else if (STATUS == "")
                    models = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().OrderByDescending(t => t.OPENINGDATE).ToList();

                else if (!string.IsNullOrEmpty(STATUS))
                    models = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(w => w.STATUS == STATUS).OrderByDescending(t => t.OPENINGDATE).ToList();

                // Status=string.empty does not displayed its mean that FixedDeposit attempt to renew but yet not approved important
                 models = models.Where(t => t.STATUS != "" && t.STATUS != null).OrderByDescending(t => t.OPENINGDATE).ToList();

                 if (fromdate != null && toDate != null)
                 {
                     models = models.Where(w => w.OPENINGDATE.Value.Date >= fromdate.Value.Date && w.OPENINGDATE.Value.Date <= toDate.Value.Date).OrderByDescending(t => t.OPENINGDATE).ToList();
                 }
                 else if (fromdate != null)
                        models = models.Where(w => w.OPENINGDATE.Value.Date >= fromdate.Value.Date).AsQueryable().OrderByDescending(t => t.OPENINGDATE).ToList();
                 else if (toDate != null)
                     models = models.Where(w => w.OPENINGDATE.Value.Date <= toDate.Value.Date).AsQueryable().OrderByDescending(t => t.OPENINGDATE).ToList();
                 else { }

                    if (!string.IsNullOrEmpty(FINANCIALINSTITUTION_REFERENCE))
                        models = models.Where(w => w.FINANCIALINSTITUTION_REFERENCE == FINANCIALINSTITUTION_REFERENCE).AsQueryable().OrderByDescending(t => t.OPENINGDATE).ToList();

                    if (!string.IsNullOrEmpty(FdrNo) && FdrNo != null)
                        models = models.Where(w => w.DEPOSITNUMBER == FdrNo).OrderByDescending(t => t.OPENINGDATE).ToList();

                    //if (STATUS != null && !string.IsNullOrEmpty(STATUS))
                    //    models = models.Where(w => w.STATUS == STATUS).OrderByDescending(t => t.OPENINGDATE).ToList();
              

                gridModels.DataModel = models;
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }



                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                
                //if (models.Count() < gridModels.RowsPerPage)
                //{

                //    ViewBag.Prev = "disabled";
                //    ViewBag.Next = "disabled";
                //    ViewBag.PrevNotActive = "not-active";
                //    ViewBag.NextNotActive = "not-active";
                //}



                var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "STATUS").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app=>app.DESCRIPTION).ToList();

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");


                if (Convert.ToString(TempData["result"]) != null)
                {
                    ViewBag.Info = Convert.ToString(TempData["result"]);
                }

                return PartialView("ListFixedDeposit", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }
      

        [HttpGet]
        public ActionResult AddFixedDeposit(string financialInstitution, FIXEDDEPOSIT oFIXEDDEPOSIT)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                if (!string.IsNullOrEmpty(financialInstitution))
                {
                    ViewBag.DEPOSITNUMBER = oFIXEDDEPOSIT.DEPOSITNUMBER;
                    ViewBag.OPENINGDATE = oFIXEDDEPOSIT.OPENINGDATE;
                    ViewBag.CHEQUEREFERENCE = oFIXEDDEPOSIT.CHEQUEREFERENCE;
                    ViewBag.PRINCIPALAMOUNT = oFIXEDDEPOSIT.PRINCIPALAMOUNT;
                    ViewBag.CHEQUEDATE = oFIXEDDEPOSIT.CHEQUEDATE;
                    ViewBag.TENURE = oFIXEDDEPOSIT.TENURE;
                    ViewBag.TENURETERM = oFIXEDDEPOSIT.TENURETERM;
                    ViewBag.ADVANCEDINTERESTRATE = oFIXEDDEPOSIT.ADVANCEDINTERESTRATE;
                    ViewBag.COMPOUNDINTERESTINTERVAL = oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL == null ? "Select a list" : oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL;
                    ViewBag.RATEOFINTEREST = oFIXEDDEPOSIT.RATEOFINTEREST;
                    ViewBag.INTERESTMODE = oFIXEDDEPOSIT.INTERESTMODE == null ? "Select a list" : oFIXEDDEPOSIT.INTERESTMODE;
                    ViewBag.MATURITYDATE = oFIXEDDEPOSIT.MATURITYDATE;
                    ViewBag.ANNUALDAYS = oFIXEDDEPOSIT.ANNUALDAYS;




                    ViewBag.FinancialInstitution = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.Where(f => f.REFERENCE == financialInstitution).FirstOrDefault().NAME.ToString();
                }

                else
                {

                    ViewBag.FinancialInstitution = "Select a list";
                    //ViewBag.COMPOUNDINTERESTTYPE = "Select a list";
                    ViewBag.COMPOUNDINTERESTINTERVAL = "Select a list";

                }
                ViewBag.EXCISEDUTY = financialInstitution == null ? "0.00" : (new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.Where(f => f.REFERENCE == financialInstitution).FirstOrDefault().EXCISEDUTY.ToString());
                ViewBag.SOURCETAX = financialInstitution == null ? "0.00" : new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.Where(f => f.REFERENCE == financialInstitution).FirstOrDefault().TAXRATE.ToString();
                ViewBag.OTHERCHARGE = financialInstitution == null ? "0.00" : new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.Where(f => f.REFERENCE == financialInstitution).FirstOrDefault().OTHERCHARGE.ToString();
                var a = ViewBag.SOURCETAX;

                ViewBag.test = "test";
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];


                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "TenureTerm").FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();


                var interestType = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var COMPOUNDINTERESTTYPE = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestType).OrderBy(app => app.DESCRIPTION).ToList();


                var interval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
                var ComoundInterestInterval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interval).OrderBy(app => app.DESCRIPTION).ToList();


                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.FIBranch = new SelectList(new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION");
                ViewBag.COMPOUNDINTERESTTYPEList = new SelectList(COMPOUNDINTERESTTYPE, "DESCRIPTION", "DESCRIPTION");
                ViewBag.ComoundInterestInterval = new SelectList(ComoundInterestInterval, "DESCRIPTION", "DESCRIPTION");





                return PartialView(oFIXEDDEPOSIT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddFixedDeposit(FIXEDDEPOSIT oFIXEDDEPOSIT)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {
                //Deposit Number Must be Unique

                FIXEDDEPOSIT isExists = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Where(t => t.DEPOSITNUMBER == oFIXEDDEPOSIT.DEPOSITNUMBER).SingleOrDefault();

                if (isExists != null)
                {
                    return RedirectToAction("Index", "ErrorPage", new { message="Deposit Number "+ oFIXEDDEPOSIT.DEPOSITNUMBER+" already exists!!" });
                }
                else
                {
                    if (Request.IsAjaxRequest())
                    {

                        oFIXEDDEPOSIT.REFERENCE = Guid.NewGuid().ToString();
                        oFIXEDDEPOSIT.CREATEDBY = Session["UserId"].ToString();
                        oFIXEDDEPOSIT.CREATEDDATE = DateTime.Now;
                        oFIXEDDEPOSIT.LASTUPDATED = DateTime.Now;
                        oFIXEDDEPOSIT.STATUS = "Pending";
                        TimeSpan t = oFIXEDDEPOSIT.MATURITYDATE.Value.Date - oFIXEDDEPOSIT.OPENINGDATE.Value.Date;
                        oFIXEDDEPOSIT.TERMSINDAYS = (decimal)t.TotalDays;


                        oFIXEDDEPOSIT.PRESENTPRINCIPALAMOUNT = oFIXEDDEPOSIT.PRINCIPALAMOUNT;

                        oCommonFunction.CustomObjectNullValidation<FIXEDDEPOSIT>(ref oFIXEDDEPOSIT);
                        oFIXEDDEPOSIT.RENWALDATE = null;
                        //oUSERGROUP.LASTLOGIN = DateTime.Now;
                        using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                        {
                            db.FIXEDDEPOSITs.Add(oFIXEDDEPOSIT);
                            db.SaveChanges();
                        }

                    }
                    return RedirectToAction("ListFixedDeposit", "FixedDeposit");
                }

            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
        private DateTime maturedDate = new DateTime(); 
        public ActionResult SetDate(string openDate,int tenure,string Term)
        {

            //if (Term == "Months")
            //    maturedDate = openDate.AddMonths(tenure);
            //else if (Term == "Years")
            //    maturedDate = openDate.AddYears(tenure);
            //else if (Term == "Days")
            //    maturedDate = openDate.AddDays(tenure);

            return Json(maturedDate,JsonRequestBehavior.AllowGet);
        }
   
        //this is id FixedDeposit pk and we get ChaqueDrawn list of this fixed deposit and assign it viewbag
        /// <summary>
        /// edeited  by rakibul 7th Feb 2016
        /// </summary>

      
        [HttpGet]
        public ActionResult EditFixedDeposit(string id, string financialInstitution, FIXEDDEPOSIT oFIXEDDEPOSIT)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                _var.taxRate = 0;
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();

                //trace per interval Interest for Compound 
                List<FDRINTEREST> oFDRInterest = new List<FDRINTEREST>();
                DateTime? ToDate=null;

                if (oFIXEDDEPOSIT != null && financialInstitution==null)
                {
                    oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == id);
                   _var.taxRate = oFIXEDDEPOSIT.FINANCIALINSTITUTION.TAXRATE;
                }

                else 
                {
                    //ViewBag.DEPOSITNUMBER = oFIXEDDEPOSIT.DEPOSITNUMBER ==null?oFIXEDDEPOSIT.RENEWALDEPOSITNUMBER:"";
                    ViewBag.OPENINGDATE = oFIXEDDEPOSIT.OPENINGDATE;
                    ViewBag.CHEQUEREFERENCE = oFIXEDDEPOSIT.CHEQUEREFERENCE;
                    ViewBag.PRINCIPALAMOUNT = oFIXEDDEPOSIT.PRINCIPALAMOUNT;
                    ViewBag.CHEQUEDATE = oFIXEDDEPOSIT.CHEQUEDATE;
                    ViewBag.TENURE = oFIXEDDEPOSIT.TENURE;
                    ViewBag.TENURETERM = oFIXEDDEPOSIT.TENURETERM;
                    ViewBag.ADVANCEDINTERESTRATE = oFIXEDDEPOSIT.ADVANCEDINTERESTRATE;
                    ViewBag.COMPOUNDINTERESTINTERVAL = oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL == null ? "Select a list" : oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL;
                    ViewBag.RATEOFINTEREST = oFIXEDDEPOSIT.RATEOFINTEREST;
                    ViewBag.INTERESTMODE = oFIXEDDEPOSIT.INTERESTMODE == null ? "Select a list" : oFIXEDDEPOSIT.INTERESTMODE;
                    ViewBag.MATURITYDATE = oFIXEDDEPOSIT.MATURITYDATE;
                    
                    if(oFIXEDDEPOSIT.ANNUALDAYS==0)
                    ViewBag.ANNUALDAYS = 365;  //assign a default Annual Days with Satndard value
                    else
                        ViewBag.ANNUALDAYS = oFIXEDDEPOSIT.ANNUALDAYS;
                 
                    FINANCIALINSTITUTION FInstitution= new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.Where(f => f.REFERENCE == financialInstitution).FirstOrDefault();

                    ViewBag.FinancialInstitution = FInstitution.NAME.ToString();
                    _var.taxRate = FInstitution.TAXRATE;
                
                }
                ViewBag.DEPOSITNUMBER = oFIXEDDEPOSIT.DEPOSITNUMBER == null ? oFIXEDDEPOSIT.RENEWALDEPOSITNUMBER : null;
                 //this is for compound to display any annual days
                if (oFIXEDDEPOSIT.ANNUALDAYS == 0)
                {
                    ViewBag.ANNUALDAYS = 365;  //assign a default Annual Days with Satndard value
                    oFIXEDDEPOSIT.ANNUALDAYS = 365;
                }
                else
                    ViewBag.ANNUALDAYS = oFIXEDDEPOSIT.ANNUALDAYS;


             
                #region Calculation           

                                
                //if Deposit Number is not provided then ED and OC come from Financial Institution               

                if (oFIXEDDEPOSIT.DEPOSITNUMBER == null && string.IsNullOrEmpty(oFIXEDDEPOSIT.DEPOSITNUMBER))
                {
                    if(oFIXEDDEPOSIT.EXCISEDUTY==null) //only when it null then assign
                         oFIXEDDEPOSIT.EXCISEDUTY = oFIXEDDEPOSIT.FINANCIALINSTITUTION.EXCISEDUTY;
                    if(oFIXEDDEPOSIT.OTHERCHARGE==null) //only when it null
                         oFIXEDDEPOSIT.OTHERCHARGE = oFIXEDDEPOSIT.FINANCIALINSTITUTION.OTHERCHARGE;
                }
                //but if ED and OC has value
                    
                
                #region FLAT_CALCULATION
                
                if (oFIXEDDEPOSIT.INTERESTMODE == ConstantVariable.INTERESTMODE_FLAT)
                  {
                      //flat has only one Interval
                      _var.Interval = 1;
                      Obj = fdrCalculation.GetInterestSlab(oFIXEDDEPOSIT.REFERENCE, oFIXEDDEPOSIT.PRINCIPALAMOUNT.Value, oFIXEDDEPOSIT.OPENINGDATE.Value, oFIXEDDEPOSIT.MATURITYDATE.Value, oFIXEDDEPOSIT.RATEOFINTEREST.Value, _var.taxRate.Value, oFIXEDDEPOSIT.EXCISEDUTY.Value, oFIXEDDEPOSIT.OTHERCHARGE.Value, _var.Interval,Convert.ToInt32(oFIXEDDEPOSIT.TENURE.Value), oFIXEDDEPOSIT.TENURETERM,oFIXEDDEPOSIT.ANNUALDAYS.Value);
                      if (Obj != null)
                      {
                          oFIXEDDEPOSIT.GROSSINTEREST = Obj.SumGrossInterest;
                          oFIXEDDEPOSIT.SOURCETAX = Obj.SumSourceTax;
                          oFIXEDDEPOSIT.NETINTERESTRECEIVABLE = Obj.SumNetInterest;
                          oFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE = Obj.LastReceivableAmount;

                          oFDRInterest = Obj.InterestList;
                          ViewBag.FDRINTEREST = oFDRInterest;
                      }
                      else
                      {
                          return RedirectToAction("Index", "ErrorPage", new { message = "DataBase Connection Timeout !!" });
                      }
                  }

                  #endregion

                #region CompoundCalculation

                  else if (oFIXEDDEPOSIT.INTERESTMODE == ConstantVariable.INTERESTMODE_COMPOUND)
                            {

                      _var.CompoundInterval = 0;
                   
                      if(oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL==ConstantVariable.COMPOUND_INTEREST_QUARTERLY_MSG) //Quarterly
                        {
                            _var.CompoundInterval = ConstantVariable.COMPOUND_INTEREST_QUERTERLY_ID; //4
                        }

                      else if (oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL == ConstantVariable.COMPOUND_INTEREST_HALFYEARLY_MSG)
                       {
                        _var.CompoundInterval = ConstantVariable.COMPOUND_INTEREST_HALFYEARLY_ID;//2
                       }
                     else if (oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL == ConstantVariable.COMPOUND_INTEREST_YEARLY_MSG)
                       {
                        _var.CompoundInterval = ConstantVariable.COMPOUND_INTEREST_YEARLY_ID;//1
                       }
                     else if (oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL ==ConstantVariable.COMPOUND_INTEREST_MONTHLY_MSG)
                       {
                        _var.CompoundInterval = ConstantVariable.COMPOUND_INTEREST_MONTHLY_ID;//12
                       }   

                    
                        //get how many Interval
                        _var.Interval = fdrCalculation.GetCompoundInterval(Convert.ToInt32(oFIXEDDEPOSIT.TENURE.Value), oFIXEDDEPOSIT.TENURETERM,oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL);
                        if (_var.Interval != 0 && _var.CompoundInterval != 0)
                        {
                            Obj = fdrCalculation.GetInterestSlab(oFIXEDDEPOSIT.REFERENCE,oFIXEDDEPOSIT.PRINCIPALAMOUNT.Value,oFIXEDDEPOSIT.OPENINGDATE.Value,oFIXEDDEPOSIT.MATURITYDATE.Value, oFIXEDDEPOSIT.RATEOFINTEREST.Value,_var.taxRate.Value,oFIXEDDEPOSIT.EXCISEDUTY.Value,oFIXEDDEPOSIT.OTHERCHARGE.Value,_var.Interval,_var.CompoundInterval,"",oFIXEDDEPOSIT.ANNUALDAYS.Value);
                            if (Obj != null)
                            {
                                //now we get sumation of all netInterest now we minus Excise and others charge from SumNetInterest 
                                //we get Total amount receivable
                              oFIXEDDEPOSIT.GROSSINTEREST = Obj.SumGrossInterest.Value;
                              oFIXEDDEPOSIT.SOURCETAX = Obj.SumSourceTax.Value;
                              oFIXEDDEPOSIT.NETINTERESTRECEIVABLE = Obj.SumGrossInterest.Value - Obj.SumSourceTax - oFIXEDDEPOSIT.EXCISEDUTY.Value - oFIXEDDEPOSIT.OTHERCHARGE.Value;  //Obj.SumNetInterest -oFIXEDDEPOSIT.EXCISEDUTY.Value - oFIXEDDEPOSIT.OTHERCHARGE.Value;
                              oFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE = oFIXEDDEPOSIT.PRINCIPALAMOUNT.Value + oFIXEDDEPOSIT.NETINTERESTRECEIVABLE.Value;                                                 //Obj.LastReceivableAmount.Value - oFIXEDDEPOSIT.EXCISEDUTY.Value - oFIXEDDEPOSIT.OTHERCHARGE.Value;

                                //assign this two value in view so if Excise duty and others charge change from view 
                                //then depend this two value we calculate NetInterestReceivable and TotalamountReceivable in view in js
                                ViewBag.SumGrossInterest = Obj.SumGrossInterest.Value;
                                ViewBag.SumSourceTax = Obj.SumSourceTax.Value;
                                ViewBag.LastReceivableAmount = Obj.LastReceivableAmount.Value;
                                ViewBag.SumNetInterest = Obj.SumNetInterest.Value;

                                //send view 
                             oFDRInterest = Obj.InterestList;

                             ViewBag.FDRINTEREST = oFDRInterest;
                            
                            }
                            else
                                return RedirectToAction("Index", "ErrorPage", new { message="Null reference Error !!" });
                        }
                        else
                        {
                            return RedirectToAction("Index", "ErrorPage", new { message = "Interval must have a value !!" });
                        }
                
                    }

                #endregion


                  //display only 2 decimal places value
                ViewBag.GrossInterest = Math.Round(oFIXEDDEPOSIT.GROSSINTEREST.Value, 2);
                ViewBag.SourceTax = Math.Round(oFIXEDDEPOSIT.SOURCETAX.Value, 2);
               
                ViewBag.ExciseDuty = Math.Round(oFIXEDDEPOSIT.EXCISEDUTY.Value, 2);
                ViewBag.OthersCharge = Math.Round(oFIXEDDEPOSIT.OTHERCHARGE.Value,2);
                
                ViewBag.NetInterestReceivable = Math.Round(oFIXEDDEPOSIT.NETINTERESTRECEIVABLE.Value,2);
                ViewBag.TotalamountReceivable = Math.Round(oFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE.Value,2);

                ViewBag.Interestmode = oFIXEDDEPOSIT.INTERESTMODE.ToString();
                ViewBag.InterestInterval =Convert.ToString(oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL);
                ViewBag.tenureTermMsg =Convert.ToString(oFIXEDDEPOSIT.TENURETERM);

                //if (oFIXEDDEPOSIT.INTERESTMODE == ConstantVariable.INTERESTMODE_FLAT)
                //{
                //    ViewBag.SumGrossInterest = Obj.SumGrossInterest.Value;
                //    ViewBag.SumSourceTax = Obj.SumSourceTax.Value;
                //    ViewBag.LastReceivableAmount = Obj.LastReceivableAmount.Value;
                //    ViewBag.SumNetInterest = Obj.SumNetInterest.Value;
                //}
                #endregion              

              
              

                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "TenureTerm").FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();


                var interestType = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var COMPOUNDINTERESTTYPE = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestType).OrderBy(app => app.DESCRIPTION).ToList();


                var interval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
                var ComoundInterestInterval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interval).OrderBy(app => app.DESCRIPTION).ToList();


                var FIList    =new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(); 
                var FIBranchList  =new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.OrderBy(f => f.NAME).ToList(); 

                ViewBag.financialInstitutionList =new SelectList(FIList, "REFERENCE", "NAME",oFIXEDDEPOSIT.FINANCIALINSTITUTION_REFERENCE); 
                ViewBag.FIBranch =new SelectList(FIBranchList, "REFERENCE", "NAME",oFIXEDDEPOSIT.BRANCH_REFERENCE);
 
                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION",oFIXEDDEPOSIT.TENURETERM);
                ViewBag.COMPOUNDINTERESTTYPEList = new SelectList(COMPOUNDINTERESTTYPE, "DESCRIPTION", "DESCRIPTION");
                ViewBag.ComoundInterestInterval = new SelectList(ComoundInterestInterval, "DESCRIPTION", "DESCRIPTION");


                
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                //get cheque info of this fixed deposit given id edited by rakibul 1/25/2016

                List<CHEQUEDRAWN> ChequeRef = db.CHEQUEDRAWNs.Where(t => t.FIXEDDEPOSIT_REFERENCE == id).OrderBy(t=>t.CHEQUENO).ToList();
                ViewBag.DepositChequeRef = ChequeRef;

               ViewBag.chequedate=oFIXEDDEPOSIT.CHEQUEDATE.Value.ToString("dd-MMM-yy"); //look like 22-Feb-16
               ViewBag.maturityDate = oFIXEDDEPOSIT.MATURITYDATE.Value.ToString("dd-MMM-yy");            
              
                //To calculate client side if excise duty and others charge change when compound or if annual days change when Flat 
                //we shoud send RateofInterest,TaxRate,and last interval Principle Amount(for compound)

               ViewBag.RateofInterest = oFIXEDDEPOSIT.RATEOFINTEREST;
               ViewBag.taxRate = oFIXEDDEPOSIT.TAXRATE;

             
             
                return PartialView("EditFixedDeposit", oFIXEDDEPOSIT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

       

        /// <summary>
        /// only Deposit number will be Dupliacte while it Renew 
        /// for that we matched given Deposit number with its Renewed Depost Number
        /// if Renewd Deposit number is blank or empty but given deposit is matched any Deposit number that means its newly created FDR
        /// if Renewed Depost number is matched given Deposit that means its Same Deposit will be Renew 
        /// </summary>
       

        [HttpPost]
        public ActionResult EditFixedDeposit(FIXEDDEPOSIT oFIXEDDEPOSIT)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
          try
            {
                _var.Flag = 0;
               if (oFIXEDDEPOSIT.RENEWALDEPOSITNUMBER != oFIXEDDEPOSIT.DEPOSITNUMBER)
                {
                    //Deposit number must be unique.So Find Deposit Number Status not Rejected. if exists then return 
                    FIXEDDEPOSIT isExists = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Where(t => t.DEPOSITNUMBER == oFIXEDDEPOSIT.DEPOSITNUMBER && t.STATUS!=ConstantVariable.STATUS_REJECTED).FirstOrDefault(); //take first from list

                if (isExists != null && isExists.STATUS != ConstantVariable.STATUS_PENDING)
                {
                    _var.Flag = 1;
                    return RedirectToAction("Index", "ErrorPage", new { message = "Deposit Number " + oFIXEDDEPOSIT.DEPOSITNUMBER + " already exists!!" });
                }
               }
               if(_var.Flag==0)
                {

                    //Check Opening and Matured Date validation
                    string Result = IsMaturedDateValid(oFIXEDDEPOSIT.OPENINGDATE,oFIXEDDEPOSIT.MATURITYDATE,oFIXEDDEPOSIT.TENURE,oFIXEDDEPOSIT.TENURETERM);
                    
                   if (Result != "true")
                    {
                        return RedirectToAction("Index", "ErrorPage", new { message =Result  });
                    }
                    
                    IEnumerable<FDRINTEREST> interestList = new Entities(Session["Connection"] as EntityConnection).FDRINTERESTs.Where(t => t.FIXEDDEPOSIT_REFERENCE == oFIXEDDEPOSIT.REFERENCE).ToList();
                    List<FDRINTEREST> ofdrInterest = TempData["oFDRInterest"] as List<FDRINTEREST>;

                    if (ofdrInterest == null)
                    {
                        return RedirectToAction("Index", "ErrorPage", new { message = "Interest Break Down has null value !!" });
                    }

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {

                        foreach (var item in interestList)
                        {
                            FDRINTEREST obj = new FDRINTEREST();

                            obj.REFERENCE = item.REFERENCE;
                            obj.FIXEDDEPOSIT_REFERENCE = item.FIXEDDEPOSIT_REFERENCE;
                            obj.PRINCIPALAMOUNT = item.PRINCIPALAMOUNT;
                            obj.RATEOFINTEREST = item.RATEOFINTEREST;
                            obj.FROMDATE = item.FROMDATE;
                            obj.TODATE = item.TODATE;
                            obj.GROSSINTEREST = item.GROSSINTEREST;
                            obj.TAXRATE = item.TAXRATE;
                            obj.SOURCETAX = item.SOURCETAX;
                            obj.EXCISEDUTY = item.EXCISEDUTY;
                            obj.OTHERCHARGE = item.OTHERCHARGE;
                            
                            obj.NETINTERESTRECEIVABLE = item.NETINTERESTRECEIVABLE;
                            obj.AMOUNTRECEIVABLE = item.AMOUNTRECEIVABLE;
                            obj.COMPOUNDVALUE = item.COMPOUNDVALUE;                       
                            
                            db.FDRINTERESTs.Attach(obj);
                            db.FDRINTERESTs.Remove(obj);
                            // db.SaveChanges();
                        }


                        oFIXEDDEPOSIT.LASTUPDATED = DateTime.Now;
                        oFIXEDDEPOSIT.LASTUPDATEDBY = Session["UserId"].ToString();

                        //added by 6th apr-2016
                        oFIXEDDEPOSIT.PRESENTPRINCIPALAMOUNT = oFIXEDDEPOSIT.PRINCIPALAMOUNT + oFIXEDDEPOSIT.NETINTERESTRECEIVABLE; //oFIXEDDEPOSIT.PRINCIPALAMOUNT  //edited 24-11-16
                        oFIXEDDEPOSIT.ACTUALINTERESTRECEIVED = oFIXEDDEPOSIT.NETINTERESTRECEIVABLE;

                        TimeSpan t = oFIXEDDEPOSIT.MATURITYDATE.Value.Date - oFIXEDDEPOSIT.OPENINGDATE.Value.Date;

                        oFIXEDDEPOSIT.TERMSINDAYS = (decimal)t.TotalDays;
                        oCommonFunction.CustomObjectNullValidation<FIXEDDEPOSIT>(ref oFIXEDDEPOSIT);
                        oFIXEDDEPOSIT.RENWALDATE = null;
                        oFIXEDDEPOSIT.ENCASHMENTDATE = null; //added 6-12-16

                        //added 04/10/2016 2 field value

                        if (oFIXEDDEPOSIT.RENEWALDEPOSITNUMBER == null || oFIXEDDEPOSIT.RENEWALDEPOSITNUMBER == "") //that means it is first FDR not renewed from another FDR/Diposit
                        {
                            if (oFIXEDDEPOSIT.INITIALPRINCIPALAMOUNT == 0)
                                oFIXEDDEPOSIT.INITIALPRINCIPALAMOUNT = oFIXEDDEPOSIT.PRINCIPALAMOUNT.Value;
                            else
                                oFIXEDDEPOSIT.INITIALPRINCIPALAMOUNT = oFIXEDDEPOSIT.INITIALPRINCIPALAMOUNT;

                            oFIXEDDEPOSIT.INITIALOPENINGDATE = oFIXEDDEPOSIT.OPENINGDATE;
                        }
                      
                        

                        db.Entry(oFIXEDDEPOSIT).State = EntityState.Modified;


                        //Delete a record                


                        if (oFIXEDDEPOSIT.INTERESTMODE == ConstantVariable.INTERESTMODE_COMPOUND)
                        {
                            //To do edit will not first we delete then add
                            foreach (var oItem in ofdrInterest)
                            {

                                FDRINTEREST oFDRInterest = new FDRINTEREST();

                                oFDRInterest.REFERENCE = Guid.NewGuid().ToString();
                                oFDRInterest.FIXEDDEPOSIT_REFERENCE = oFIXEDDEPOSIT.REFERENCE;

                                oFDRInterest.COMPOUNDVALUE = oItem.COMPOUNDVALUE;
                                oFDRInterest.GROSSINTEREST = oItem.GROSSINTEREST;
                                oFDRInterest.NETINTERESTRECEIVABLE = oItem.NETINTERESTRECEIVABLE;
                                oFDRInterest.OTHERCHARGE = oItem.OTHERCHARGE;
                                oFDRInterest.PRINCIPALAMOUNT = oItem.PRINCIPALAMOUNT;
                                oFDRInterest.RATEOFINTEREST = oItem.RATEOFINTEREST;
                                oFDRInterest.SOURCETAX = oItem.SOURCETAX;
                                oFDRInterest.TAXRATE = oItem.TAXRATE;
                                oFDRInterest.TODATE = oItem.TODATE;
                                oFDRInterest.FROMDATE = oItem.FROMDATE;
                                oFDRInterest.PRINCIPALAMOUNT = oItem.PRINCIPALAMOUNT;
                                oFDRInterest.AMOUNTRECEIVABLE = oItem.AMOUNTRECEIVABLE;

                                oCommonFunction.CustomObjectNullValidation<FDRINTEREST>(ref oFDRInterest);

                                db.FDRINTERESTs.Add(oFDRInterest);

                            }//foreach


                        }//if
                        else if (oFIXEDDEPOSIT.INTERESTMODE == ConstantVariable.INTERESTMODE_FLAT)
                        {
                            foreach (var oItem in ofdrInterest)
                            {

                                FDRINTEREST oFDRInterest = new FDRINTEREST();

                                oFDRInterest.REFERENCE = Guid.NewGuid().ToString();
                                oFDRInterest.FIXEDDEPOSIT_REFERENCE = oFIXEDDEPOSIT.REFERENCE;
                                oFDRInterest.COMPOUNDVALUE = oItem.COMPOUNDVALUE;

                                oFDRInterest.FROMDATE = oFIXEDDEPOSIT.OPENINGDATE;
                                oFDRInterest.TODATE = oFIXEDDEPOSIT.MATURITYDATE;

                                oFDRInterest.PRINCIPALAMOUNT = oFIXEDDEPOSIT.PRINCIPALAMOUNT;
                                oFDRInterest.RATEOFINTEREST = oFIXEDDEPOSIT.RATEOFINTEREST;
                                oFDRInterest.TAXRATE = oFIXEDDEPOSIT.TAXRATE;

                                oFDRInterest.EXCISEDUTY = oFIXEDDEPOSIT.EXCISEDUTY;
                                oFDRInterest.OTHERCHARGE = oFIXEDDEPOSIT.OTHERCHARGE;

                                oFDRInterest.GROSSINTEREST = oFIXEDDEPOSIT.GROSSINTEREST;
                                oFDRInterest.SOURCETAX = oFIXEDDEPOSIT.SOURCETAX;
                                oFDRInterest.NETINTERESTRECEIVABLE = oFIXEDDEPOSIT.NETINTERESTRECEIVABLE;
                                oFDRInterest.AMOUNTRECEIVABLE = oFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE;

                                oCommonFunction.CustomObjectNullValidation<FDRINTEREST>(ref oFDRInterest);

                                db.FDRINTERESTs.Add(oFDRInterest);

                            }

                        }


                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            string message = ex.Message;

                            return RedirectToAction("Index", "ErrorPage", new { message });
                        }


                    }//using



                    return RedirectToAction("ListFixedDeposit", "FixedDeposit");
                }
            }// 1st try

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

          return RedirectToAction("Index", "ErrorPage", new { message="Invalid Operation!!" });
        }
      
        
        [HttpGet]
        public ActionResult FixedDepositInterestBreakDown(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.Refference = id;
                ViewBag.breadcum = oCommonFunction.GetDetailsEditPath(Session["Path"] as IHtmlString, "Tax Rate"); //Session["currentPage"].ToString()
                ViewBag.Header = "Edit " + Session["currentPage"];

                Entities db = new Entities(Session["Connection"] as EntityConnection);
                if (db == null)
                {
                    return RedirectToAction("Index", "ErrorPage", new { message = "Data Base Connection time out!!" });
                }

                FIXEDDEPOSIT fixedDeposit = db.FIXEDDEPOSITs.Where(t => t.REFERENCE == id).SingleOrDefault();

                ViewBag.ExciseDuty = fixedDeposit.EXCISEDUTY.Value;
                ViewBag.OthersCharge = fixedDeposit.OTHERCHARGE.Value;

                ViewBag.SumSourceTax = fixedDeposit.SOURCETAX.Value;
                ViewBag.totalNetInterest = fixedDeposit.NETINTERESTRECEIVABLE;
                ViewBag.AmountReceive = fixedDeposit.TOTALAMOUNTRECEIVABLE;

                List<FDRInterestViewModel> InterestViewModelList = new List<FDRInterestViewModel>();
                List<FDRINTEREST> InterestList = db.FDRINTERESTs.Where(t => t.FIXEDDEPOSIT_REFERENCE == id).OrderBy(t => t.FROMDATE).ToList();

                foreach (var item in InterestList)
                {
                    InterestViewModelList.Add(new FDRInterestViewModel
                    {
                        REFERENCE = item.REFERENCE,
                        FIXEDDEPOSIT_REFERENCE = item.FIXEDDEPOSIT_REFERENCE,
                        EXCISEDUTY = item.EXCISEDUTY,
                        FROMDATE = item.FROMDATE,
                        TODATE = item.TODATE,
                        AMOUNTRECEIVABLE = item.AMOUNTRECEIVABLE.Value.ToString(),
                        COMPOUNDVALUE = item.COMPOUNDVALUE,
                        GROSSINTEREST = item.GROSSINTEREST.ToString(),
                        NETINTERESTRECEIVABLE = item.NETINTERESTRECEIVABLE.ToString(),
                        OTHERCHARGE = item.OTHERCHARGE,
                        PRINCIPALAMOUNT = item.PRINCIPALAMOUNT.ToString(),
                        RATEOFINTEREST = item.RATEOFINTEREST,
                        TAXRATE = item.TAXRATE,
                        SOURCETAX = item.SOURCETAX.ToString()
                        

                    });

                }

                ViewBag.TaxUpOnGross = InterestList[0].TAXUPONGROSS;
                ViewBag.PrincipalUpOnNet = InterestList[0].PRICIPALUPONNET;
                //ViewBag.ReceivedAmt = fixedDeposit.TOTALAMOUNTRECEIVABLE;
                // return PartialView(InterestList);
                return PartialView(InterestViewModelList);
            }
            catch(Exception ex) {

                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
           
        }

        [HttpPost]
        public ActionResult FixedDepositInterestBreakDown(List<FDRInterestViewModel> model, string SumGrossInterest, string SumSourceTax, string FinalNetInterest, string FinalAmountReceivable, string ED, string OC, string TaxRate, string Principal)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            Entities db = new Entities(Session["Connection"] as EntityConnection);
            if (db == null)
            {
                return RedirectToAction("Index", "ErrorPage", new { message = "Data Base Connection time out!!" });
            }

            _var.GrossInterest = 0;
            _var.SourceTax = 0;
            _var.NetInterestReceivable = 0;
            _var.TotalAmountReceivable = 0;
            
            //now first we  edit fixed deposit then delete all row from FDRInterest with reference then newly added then save change
            //check is model contains data or not
            if (model.Count > 0 && (!string.IsNullOrEmpty(SumGrossInterest) && SumGrossInterest!=null) &&(!string.IsNullOrEmpty(SumSourceTax) && SumSourceTax!=null) &&(!string.IsNullOrEmpty(FinalNetInterest) && FinalNetInterest!=null) &&(!string.IsNullOrEmpty(FinalAmountReceivable) && FinalAmountReceivable!=null))
            {
                try
                {
                    

                    _var.FKreference = model.Take(1).SingleOrDefault().FIXEDDEPOSIT_REFERENCE;

                    ////now get fixed deposit single entity
                    FIXEDDEPOSIT fixeddeposit = db.FIXEDDEPOSITs.Where(t => t.REFERENCE == _var.FKreference).SingleOrDefault();

                    //we can calculate AmountReceivable in two one .One is given below another is  fixeddeposit.PRINCIPALAMOUNT+_var.NetInterestReceivable(above calculated NetInterest)
                    fixeddeposit.TOTALAMOUNTRECEIVABLE = Convert.ToDecimal(FinalAmountReceivable);                          // _var.TotalAmountReceivable.Value - _var.ExciseDuties.Value - _var.OthersCharge.Value; // or fixeddeposit.PRINCIPALAMOUNT+_var.NetInterestReceivable
                    fixeddeposit.NETINTERESTRECEIVABLE = Convert.ToDecimal(FinalNetInterest);                         //_var.NetInterestReceivable.Value;
                    fixeddeposit.GROSSINTEREST = Convert.ToDecimal(SumGrossInterest);                               //_var.GrossInterest.Value;
                    fixeddeposit.SOURCETAX = Convert.ToDecimal(SumSourceTax);                                     //_var.SourceTax.Value;

                    fixeddeposit.ACTUALINTERESTRECEIVED = Convert.ToDecimal(FinalNetInterest);
                    fixeddeposit.PRESENTPRINCIPALAMOUNT = Convert.ToDecimal(FinalAmountReceivable);     

                    if (TaxRate != null && !string.IsNullOrEmpty(TaxRate))
                    {
                        fixeddeposit.TAXRATE = Convert.ToDecimal(TaxRate);
                    }

                    oCommonFunction.CustomObjectNullValidation<FIXEDDEPOSIT>(ref fixeddeposit);
                    db.Entry(fixeddeposit).State = EntityState.Modified;

                    foreach (var item in model)
                    {
                        
                        FDRINTEREST interest = db.FDRINTERESTs.Where(t=>t.REFERENCE==item.REFERENCE).SingleOrDefault();
                        interest.TAXRATE = item.TAXRATE;
                        interest.SOURCETAX =Convert.ToDecimal(item.SOURCETAX);
                        interest.PRINCIPALAMOUNT =Convert.ToDecimal(item.PRINCIPALAMOUNT);
                        interest.NETINTERESTRECEIVABLE =Convert.ToDecimal(item.NETINTERESTRECEIVABLE);
                        interest.GROSSINTEREST =Convert.ToDecimal(item.GROSSINTEREST);
                        interest.AMOUNTRECEIVABLE =Convert.ToDecimal(item.AMOUNTRECEIVABLE);

                        interest.TAXUPONGROSS = TaxRate !=null ? Convert.ToDecimal(TaxRate): Convert.ToDecimal(DBNull.Value);
                        interest.PRICIPALUPONNET = Principal != null ? Convert.ToDecimal(Principal) : Convert.ToDecimal(DBNull.Value);

                        interest.LASTUPDATED = DateTime.Now;
                        interest.LASTUPDATEDBY = Session["UserId"].ToString();

                        oCommonFunction.CustomObjectNullValidation<FDRINTEREST>(ref interest);
                        db.Entry(interest).State = EntityState.Modified;

                    }

                    try
                    {
                        db.SaveChanges();

                        return RedirectToAction("ListFixedDeposit", "FixedDeposit");
                    }
                    catch (Exception)
                    {
                       // string message = ex.Message;
                        return RedirectToAction("Index", "ErrorPage", new { message="Internal Error.Data Can not be saved!!" });
                    }

                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    return RedirectToAction("Index", "ErrorPage", new { message });
                }
            
            }



            if (!string.IsNullOrEmpty(ED) && ED != null && !string.IsNullOrEmpty(OC) && OC != null)
            {
                ViewBag.ExciseDuty = Convert.ToDecimal(ED);
                ViewBag.OthersCharge = Convert.ToDecimal(OC);
            }
            else {

                return RedirectToAction("Index", "ErrorPage", new { message="Null value error!!" });
            }


            ViewBag.Message = "Can not save data due to some Invalid value !!";
            return PartialView(model);

        }
        //Edited By Hemel on 8-jul-15 for Approve and Reject menu
        
        //Approve Post
        
        public ActionResult ApproveFixedDeposit(string id)
        {

             
            try
            {

                FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                 
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == id);
                  
                    if (oFIXEDDEPOSIT.DEPOSITNUMBER != null && !string.IsNullOrEmpty(oFIXEDDEPOSIT.DEPOSITNUMBER))
                    {
                        oFIXEDDEPOSIT.ACCEPTEDBY = Session["UserId"].ToString();

                        //in history report  AcceptedDate<= SearchDate <= Encashed Date(for proposed Action Encash)/ RenewalDate (for proposed action Renewal) / for status = Approved 
                        oFIXEDDEPOSIT.ACCEPTEDDATE = DateTime.Today;
                        oFIXEDDEPOSIT.STATUS = ConstantVariable.STATUS_APPROVED; 

                        db.Entry(oFIXEDDEPOSIT).State = EntityState.Modified;

                        //Added By Nazmul:11-07-2015
                        //Create financial transaction
                        new TransactionEngine().CreateFinancialTransaction("FDR01", oFIXEDDEPOSIT.PRINCIPALAMOUNT, oFIXEDDEPOSIT.ACCEPTEDDATE);


                        db.SaveChanges();
                    }
                    else {
                        TempData["EmptyDepositeNumber"] ="Deposit Number is Required!!"; 
                        
                    }
                }

                return RedirectToAction("ListFixedDeposit", "FixedDeposit");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        //Reject Post

        public ActionResult RejectFixedDeposit(string id)
        {


            try
            {

                FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == id);
                    oFIXEDDEPOSIT.REJECTEDBY = Session["UserId"].ToString();
                    oFIXEDDEPOSIT.REJECTEDDATE = DateTime.Today;
                    oFIXEDDEPOSIT.STATUS = ConstantVariable.STATUS_REJECTED;  // "Rejeted";
                    db.Entry(oFIXEDDEPOSIT).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListFixedDeposit", "FixedDeposit");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        #region Helper

        public string IsMaturedDateValid(DateTime? OpeningDate,DateTime? MaturedDate,decimal? Tenure,string TenureTerm)
        {           
            DateTime CalculatedMaturedDate;
            int tenure = Convert.ToInt16(Tenure);

            if (OpeningDate < MaturedDate)
            {
                if (TenureTerm == ConstantVariable.TENURETERM_DAYS)
                {
                    CalculatedMaturedDate = OpeningDate.Value.AddDays(tenure);
                }
                else if (TenureTerm == ConstantVariable.TENURETERM_MONTHS)
                {
                    CalculatedMaturedDate = OpeningDate.Value.AddMonths(tenure);
                }
                else if (TenureTerm == ConstantVariable.TENURETERM_YEARS)
                {
                    CalculatedMaturedDate = OpeningDate.Value.AddYears(tenure);
                }
                else
                    return "Opening Date and Matured Date encounter an Error";

                if (MaturedDate.Value.Date == CalculatedMaturedDate.Date)
                    return "true";
                else
                    return "Invalid Maturity Date";
                
            }

            return "Invalid Matured Date.Matured Date("+MaturedDate+")  must be greater then Opening Date("+OpeningDate+")";        
        }

        #endregion
    }
}
