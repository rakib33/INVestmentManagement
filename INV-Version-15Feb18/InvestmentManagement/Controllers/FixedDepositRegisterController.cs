using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using InvestmentManagement.ViewModel;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.Globalization;
using InvestmentManagement.App_Code;
//for find out Data Entity Validation which field of thr data entity
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace InvestmentManagement.Controllers
{
    public class FixedDepositRegisterController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
        Variable _var = new Variable();

        [HttpGet]
        public ActionResult ListFixedDepositRegister(string sortdir, string sort, string sortdefault, int? rows, string filterstring, string lblbreadcum, DateTime? openingDate, string FINANCIALINSTITUTION_REFERENCE, DateTime? matureDate, string STATUS, string showOpeningDate, string showMatureDate, string depositNumber)  //
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

             
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

                sort = string.IsNullOrEmpty(sort) == true ? "MATURITYDATE" : sort; 
               // sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
                //int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                //if (filterstring == null)
                //{
                //    filterstring = currentFilter;
                //}
                //DateTime date=DateTime.Today;

                ViewBag.CurrentFilter = filterstring;
                
                models = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().OrderBy(sort + " " + sortdir).ToList();

                if (models.Count > 0)
                {
                    if (STATUS == null || STATUS == ConstantVariable.STATUS_APPROVED)  //
                    {
                        models = models.ToList().Where(f => f.STATUS == ConstantVariable.STATUS_APPROVED && f.PROPOSEDACTION==null).OrderBy(t=>t.MATURITYDATE.Value.Date).ToList();

                    }
                    else if(STATUS=="") //select all registered FDR 
                    {
                        models = models.ToList().Where(f => f.STATUS != ConstantVariable.STATUS_PENDING).OrderBy(t => t.MATURITYDATE.Value.Date).ToList();
                    }
                    else if (STATUS == ConstantVariable.STATUS_PENDING)  //|| STATUS == string.Empty
                    {
                        models = models.ToList().Where(f => f.STATUS == ConstantVariable.STATUS_PENDING).OrderBy(t => t.MATURITYDATE.Value.Date).ToList();
                    }

                    else if (STATUS == ConstantVariable.STATUS_ENCASHED)
                    {
                        models = models.ToList().Where(f => f.STATUS == ConstantVariable.STATUS_ENCASHED).OrderBy(t => t.MATURITYDATE.Value.Date).ToList();
                        //models = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(f => f.STATUS == STATUS).OrderBy(sort + " " + sortdir).ToList();
                    }
                    else if (STATUS == ConstantVariable.STATUS_RENEWED)
                    {
                        models = models.ToList().Where(f => f.STATUS == STATUS).OrderBy(t => t.MATURITYDATE.Value.Date).ToList();
                       // models = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(f => f.STATUS == STATUS).OrderBy(sort + " " + sortdir).ToList();

                    }
                    else if (STATUS == ConstantVariable.STATUS_REJECTED)
                    {
                        models = models.ToList().Where(f => f.STATUS == STATUS).ToList();
                    }

                    if (!string.IsNullOrEmpty(depositNumber))
                    {
                        models = models.Where(o => o.DEPOSITNUMBER == depositNumber).ToList();
                    }


                    if (openingDate != null)
                    {
                        models = models.Where(o => o.OPENINGDATE.Value.Date >= openingDate.Value.Date).ToList();
                    }

                    if (matureDate != null)
                    {
                        models = models.Where(o => o.MATURITYDATE.Value.Date <= matureDate.Value.Date).ToList();
                    }

                   

                    if (!string.IsNullOrEmpty(FINANCIALINSTITUTION_REFERENCE))
                    {
                        models = models.Where(o => o.FINANCIALINSTITUTION_REFERENCE == FINANCIALINSTITUTION_REFERENCE).ToList();
                    }

                    //display model descending order by opening date
                   // models = models.ToList().OrderByDescending(t => t.OPENINGDATE.Value.Date).ToList();

                    //ReceivableTill Calculation Formula
                    //HoldingPeriod = Total days between Fixed Deposit Opening date and todays date
                    //TotalDays = total days between IssueDate and Matured Date
                    //Formula ReceivableTill= (NetInterestReceivable / TotalDays) * HoldingPeriod

                    foreach (FIXEDDEPOSIT oFIXEDDEPOSIT in models)
                    {
                        TimeSpan t = DateTime.Today - oFIXEDDEPOSIT.OPENINGDATE.Value.Date;
                        oFIXEDDEPOSIT.HOLDINGPERIOD = (decimal)t.TotalDays;

                        TimeSpan total = oFIXEDDEPOSIT.MATURITYDATE.Value - oFIXEDDEPOSIT.OPENINGDATE.Value; //if renew then renewal Date
                        decimal TotalDays = Convert.ToDecimal(total.TotalDays);

                        if (oFIXEDDEPOSIT.HOLDINGPERIOD != 0 && oFIXEDDEPOSIT.NETINTERESTRECEIVABLE != null)
                            oFIXEDDEPOSIT.RECEIVABLETILL = (oFIXEDDEPOSIT.NETINTERESTRECEIVABLE.Value / TotalDays) * oFIXEDDEPOSIT.HOLDINGPERIOD;
                        else
                            oFIXEDDEPOSIT.RECEIVABLETILL = 0;



                        //Previous Calculated in wrong concept
                        //(oFIXEDDEPOSIT.NETINTERESTRECEIVABLE * 365) / oFIXEDDEPOSIT.HOLDINGPERIOD;


                    }
                }                               


                TempData["FIXEDDEPOSITList"] = models;

                gridModels.DataModel = models;
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



                var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "STATUS").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app => app.DESCRIPTION).ToList();

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");


                if (Convert.ToString(TempData["result"]) != null)
                {
                    ViewBag.Info = Convert.ToString(TempData["result"]);
                }

                return PartialView("ListFixedDepositRegister", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }
        
        [HttpGet]
        public ActionResult RenewDeposit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) && id == null)
                {
                    return RedirectToAction("Index", "ErrorPage", new { message="Nullable reference not allowed !! " });
                }

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                Entities db = new Entities(Session["Connection"] as EntityConnection);

                ViewModelBase oViewModelBase = new ViewModelBase();


                FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == id);

                //get contact Person from FDRNOTE of this FixedDeposit added by rakibul date 3rd march 2016
                ViewBag.ContactPerson = "";
                //ViewBag.ContactPerson = "";            //db.FDRNOTEs.Where(t => t.FIXEDDEPOSIT_REFERENCE == id).SingleOrDefault().CONTACTPERSON;
                //

                //added 28-5-17 mailed 16 -May Issue 8
                ViewBag.Signatory1 = oFIXEDDEPOSIT.SIGNATORY1;
                ViewBag.Signatory2 = oFIXEDDEPOSIT.SIGNATORY2;


                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "TenureTerm").FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();


                var interestType = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var COMPOUNDINTERESTTYPE = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestType).OrderBy(app => app.DESCRIPTION).ToList();

                var ItemToRemove = COMPOUNDINTERESTTYPE.SingleOrDefault(t=>t.DESCRIPTION=="None");
                COMPOUNDINTERESTTYPE.Remove(ItemToRemove);

                var interval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
                var ComoundInterestInterval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interval).OrderBy(app => app.DESCRIPTION).ToList();

                ItemToRemove = ComoundInterestInterval.SingleOrDefault(t=>t.DESCRIPTION=="Daily");
                ComoundInterestInterval.Remove(ItemToRemove);



                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION");
                ViewBag.COMPOUNDINTERESTTYPEList = new SelectList(COMPOUNDINTERESTTYPE, "DESCRIPTION", "DESCRIPTION");
                ViewBag.ComoundInterestInterval = new SelectList(ComoundInterestInterval, "DESCRIPTION", "DESCRIPTION");
                ViewBag.SOURCETAX = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.Where(f => f.REFERENCE == oFIXEDDEPOSIT.FINANCIALINSTITUTION_REFERENCE).FirstOrDefault().TAXRATE.ToString();

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Renew " + Session["currentPage"];

                // TempData["renewFDR"] = oFIXEDDEPOSIT;
                ViewBag.renewFDR = oFIXEDDEPOSIT; //this line must be hear because bellow maturity date and others added in  oFIXEDDEPOSIT but we don't add this in ViewBag.renewFDR


                oFIXEDDEPOSIT.RENWALDATE = oFIXEDDEPOSIT.MATURITYDATE.Value;

                //Calculate maturity date
                DateTime? MaturedDate=null;

                if (oFIXEDDEPOSIT.TENURETERM == ConstantVariable.TENURETERM_MONTHS)
                { 
                  MaturedDate=oFIXEDDEPOSIT.MATURITYDATE.Value.AddMonths(Convert.ToInt32(oFIXEDDEPOSIT.TENURE.Value));
                }
                else if(oFIXEDDEPOSIT.TENURETERM==ConstantVariable.TENURETERM_YEARS)
                {
                 MaturedDate=oFIXEDDEPOSIT.MATURITYDATE.Value.AddYears(Convert.ToInt32(oFIXEDDEPOSIT.TENURE.Value));                
                }
                else if(oFIXEDDEPOSIT.TENURETERM==ConstantVariable.TENURETERM_DAYS)
                {
                 MaturedDate=oFIXEDDEPOSIT.MATURITYDATE.Value.AddDays(Convert.ToInt32(oFIXEDDEPOSIT.TENURE.Value));                
                }

                ViewBag.MATURITYDATE =MaturedDate.Value.ToString("dd-MMM-yy");
                ViewBag.TaxRate =Convert.ToDecimal(oFIXEDDEPOSIT.TAXRATE.Value);
                ViewBag.NullValue = null;

                ViewBag.Reference = id; //assign this it helps to get ofixeddeposit when  ViewBag.renewFDR is null



                return PartialView("RenewDeposit", oFIXEDDEPOSIT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }
        
        
        /// <summary>
        /// chkPrincpleNet is responsible Renew Principal amount including Net Interest check and uncheck if check its true else false
        /// while Renew a FDR two Operation made Create a new FDR status Pending and then and current FDR ststus Encashed
        /// </summary>
        [HttpPost]
        public ActionResult RenewDeposit(FIXEDDEPOSIT aFIXEDDEPOSIT, string mdate, bool chkPrincpleNet = false, string ContactPerson = null, string prevContactPerson = null, string PrevSignatory1 = null, string PrevSignatory2 = null)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }


            try
            {
                _var.ExistingDeposit = 0;
                DateTime maturedDate = Convert.ToDateTime(mdate);
                    
                //DateTime.ParseExact(mdate, "dd-MMM-yy", System.Globalization.CultureInfo.InvariantCulture);//"dd-MMM-yyyy"

                //This will save FDR Renewal note
                FDRNOTE oFDRNote = new FDRNOTE();
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    //Load Fix Deposit
                    FIXEDDEPOSIT oldFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == aFIXEDDEPOSIT.REFERENCE);


                    //check if RenewalDate is less then oldFixed Deposit matured Date

                    if (aFIXEDDEPOSIT.RENWALDATE.Value < oldFIXEDDEPOSIT.MATURITYDATE)
                    {
                        return RedirectToAction("Index", "ErrorPage", new { message="Renewal Date:"+ aFIXEDDEPOSIT.RENWALDATE.Value.ToString("dd-MMM-yy") +"must equal or greater then Renewed Deposit: "+oldFIXEDDEPOSIT.DEPOSITNUMBER+ " Matured Date :"+oldFIXEDDEPOSIT.MATURITYDATE.Value.ToString("dd-MMM-yy")});
                    }
                    else{

                    //Edit Fixed Deposit 
                    //edit renewal it means that the fdr attempt to renew 
                    oldFIXEDDEPOSIT.RENWALDATE = Convert.ToDateTime(aFIXEDDEPOSIT.RENWALDATE);
                    oldFIXEDDEPOSIT.PROPOSEDACTION = ConstantVariable.STATUS_NOTETYPE_RENEWAL;
                    oldFIXEDDEPOSIT.LASTUPDATED = DateTime.Now;
                    oldFIXEDDEPOSIT.LASTUPDATEDBY = Session["UserId"].ToString();
                    
                   
                    db.Entry(oldFIXEDDEPOSIT).State = EntityState.Modified;


                    #region added 5/04/2016
                    //Calculation CurrentHolding or Existing Deposit of the financial Institution
                   _var.ExistingDeposit = db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.STATUS == ConstantVariable.STATUS_APPROVED && fixDeposit.FINANCIALINSTITUTION_REFERENCE == oldFIXEDDEPOSIT.FINANCIALINSTITUTION_REFERENCE).Sum(fixDeposit => fixDeposit.PRINCIPALAMOUNT);

                   if (_var.ExistingDeposit == null)
                       _var.ExistingDeposit = 0;
                   //now convert int into core
                   _var.ExistingDeposit = _var.ExistingDeposit > 0 ? (_var.ExistingDeposit / 10000000) : 0;

                   //FixedDeposit new field add while renew
                   FIXEDDEPOSIT renewDeposit = new FIXEDDEPOSIT();
                                       
                   renewDeposit.REFERENCE = Guid.NewGuid().ToString();
                   renewDeposit.DEPOSITNUMBER = string.Empty;
                   renewDeposit.CREATEDDATE = DateTime.Now;    // DateTime.Today;
                   renewDeposit.CREATEDBY = Session["UserId"].ToString();

                   renewDeposit.OPENINGDATE = aFIXEDDEPOSIT.RENWALDATE;
                   // renewDeposit.RENWALDATE = aFIXEDDEPOSIT.RENWALDATE; //Erase 10-04-16
                   renewDeposit.MATURITYDATE = Convert.ToDateTime(maturedDate);

                   //this contains the DeposiNumber which we renewed so taht we can trace parent chield hirarcy
                   renewDeposit.RENEWALDEPOSITNUMBER = oldFIXEDDEPOSIT.DEPOSITNUMBER;

                   //adeded 05-10-16 INITIALOPENINGDATE INITIALPRINCIPALAMOUNT
                   renewDeposit.INITIALOPENINGDATE = oldFIXEDDEPOSIT.INITIALOPENINGDATE;
                   renewDeposit.INITIALPRINCIPALAMOUNT = oldFIXEDDEPOSIT.INITIALPRINCIPALAMOUNT;
                   renewDeposit.INITIALFIXEDDEPOSITREF = oldFIXEDDEPOSIT.INITIALFIXEDDEPOSITREF;

                   renewDeposit.LASTUPDATED = DateTime.Now;  // DateTime.Today;
                   renewDeposit.LASTUPDATEDBY = string.Empty;

                     //if renew without interest then principal amount will be oldFIXEDDEPOSIT principal amount 
                     //if renew with Interest then principal amount will be  oldFIXEDDEPOSIT principal + netinterest                 
                    renewDeposit.PRINCIPALAMOUNT= chkPrincpleNet == false ? oldFIXEDDEPOSIT.PRINCIPALAMOUNT :(oldFIXEDDEPOSIT.PRINCIPALAMOUNT+oldFIXEDDEPOSIT.NETINTERESTRECEIVABLE);

                    renewDeposit.FINANCIALINSTITUTION_REFERENCE = oldFIXEDDEPOSIT.FINANCIALINSTITUTION_REFERENCE;
                    renewDeposit.BRANCH_REFERENCE = oldFIXEDDEPOSIT.BRANCH_REFERENCE;

                    renewDeposit.SIGNATORY1 = aFIXEDDEPOSIT.SIGNATORY1;
                    renewDeposit.SIGNATORY2 = aFIXEDDEPOSIT.SIGNATORY2;

                    
                    renewDeposit.TENURE = aFIXEDDEPOSIT.TENURE;
                    renewDeposit.TENURETERM = aFIXEDDEPOSIT.TENURETERM;
                    renewDeposit.INTERESTMODE = aFIXEDDEPOSIT.INTERESTMODE;
                    renewDeposit.COMPOUNDINTERESTINTERVAL = aFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL;
                    renewDeposit.ANNUALDAYS = aFIXEDDEPOSIT.ANNUALDAYS;

                    renewDeposit.TERMSINDAYS = oldFIXEDDEPOSIT.TERMSINDAYS;
                   
                    renewDeposit.EXCISEDUTY = aFIXEDDEPOSIT.EXCISEDUTY;
                    renewDeposit.OTHERCHARGE = aFIXEDDEPOSIT.OTHERCHARGE;

                    renewDeposit.TAXRATE = aFIXEDDEPOSIT.TAXRATE;
                    renewDeposit.RATEOFINTEREST = aFIXEDDEPOSIT.RATEOFINTEREST;

                    renewDeposit.EXISTINGCAPLIMIT = oldFIXEDDEPOSIT.EXISTINGCAPLIMIT;
                    renewDeposit.CHEQUEDATE = oldFIXEDDEPOSIT.CHEQUEDATE;

                    renewDeposit.ADVANCEDINTERESTRATE = oldFIXEDDEPOSIT.ADVANCEDINTERESTRATE;
                    renewDeposit.ACTUALINTERESTRECEIVED = oldFIXEDDEPOSIT.ACTUALINTERESTRECEIVED;

                    renewDeposit.STATUS = string.Empty;  //this Status assigned empty string 
                        //ConstantVariable.STATUS_PENDING; 

                   // oCommonFunction.CustomObjectNullValidation<FIXEDDEPOSIT>(ref renewDeposit);
                    db.FIXEDDEPOSITs.Add(renewDeposit);
                  

                    #endregion


                    oFDRNote.REFERENCE = Guid.NewGuid().ToString();
                    oFDRNote.NOTEID = Guid.NewGuid().ToString();
                    oFDRNote.CAPLIMIT = oldFIXEDDEPOSIT.FINANCIALINSTITUTION.CAPLIMIT;

                    oFDRNote.CONTACTPERSON = Convert.ToString(ContactPerson);
                       
                    
                    oFDRNote.CHEQUENO = oldFIXEDDEPOSIT.CHEQUEREFERENCE;
                    oFDRNote.CREATEDDATE = oldFIXEDDEPOSIT.CHEQUEDATE;
                    oFDRNote.CREATEDBY = Session["UserId"].ToString();
                    
                    //newly added
                    oFDRNote.OPENEDDATE = Convert.ToDateTime(aFIXEDDEPOSIT.RENWALDATE); 
                    //

                    oFDRNote.CREATEDDATE = DateTime.Now;     // DateTime.Today;
                    oFDRNote.EXISTINGDEPOSIT = _var.ExistingDeposit;

                    oFDRNote.FINANCIALINSTITUTION_REFERENCE = oldFIXEDDEPOSIT.FINANCIALINSTITUTION_REFERENCE;
                    oFDRNote.FIXEDDEPOSIT_REFERENCE = oldFIXEDDEPOSIT.REFERENCE;
                    oFDRNote.BRANCH_REFERENCE = oldFIXEDDEPOSIT.BRANCH_REFERENCE;
                    oFDRNote.NOTETYPE = ConstantVariable.STATUS_NOTETYPE_RENEWAL; // "Renewal";
                    oFDRNote.OFFERRATE = aFIXEDDEPOSIT.RATEOFINTEREST;

                    oFDRNote.PROPOSEDRATE = aFIXEDDEPOSIT.RATEOFINTEREST;

                    oFDRNote.PRINCIPALAMOUNT = chkPrincpleNet == false ? oldFIXEDDEPOSIT.PRINCIPALAMOUNT : (oldFIXEDDEPOSIT.PRINCIPALAMOUNT + oldFIXEDDEPOSIT.NETINTERESTRECEIVABLE);
                    oFDRNote.PROPOSALSUMMARY = aFIXEDDEPOSIT.CREATEDDATE.Value.ToString("dd-MM-yyyy") + " - " + (oldFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME.Count() > 10 ? oldFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME.Substring(0, 10) : oldFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME);
                    oFDRNote.PROPOSEDRATE = aFIXEDDEPOSIT.RATEOFINTEREST;
                    oFDRNote.STATUS = ConstantVariable.STATUS_PENDING;  // "Pending";
                    oFDRNote.TENURE = aFIXEDDEPOSIT.TENURE;
                    oFDRNote.TENURETERM = aFIXEDDEPOSIT.TENURETERM;

                    //added 05-10-2016                   
                    oFDRNote.OFFERRATE = aFIXEDDEPOSIT.RATEOFINTEREST;
                    //End 
                    
                    //while approved this renewd fdr from renewal note list search Fixed Deposit list 
                    //where FixedDeposit.RenewalDepositNUmber==FDRNote.FDRNumber then edit 
                    oFDRNote.FDRNUMBER = oldFIXEDDEPOSIT.DEPOSITNUMBER;


                    oFDRNote.CONTACTPERSON = ContactPerson;
                    oFDRNote.SIGNATORY1 = aFIXEDDEPOSIT.SIGNATORY1;
                    oFDRNote.SIGNATORY2 = aFIXEDDEPOSIT.SIGNATORY2;
                    oFDRNote.ANNUALDAYS = aFIXEDDEPOSIT.ANNUALDAYS.HasValue ? Convert.ToInt32(aFIXEDDEPOSIT.ANNUALDAYS.Value) : Convert.ToInt32(oldFIXEDDEPOSIT.ANNUALDAYS.Value);
                    oFDRNote.INTERESTMODE = aFIXEDDEPOSIT.INTERESTMODE;            // oldFIXEDDEPOSIT.INTERESTMODE;
                    oFDRNote.COMPOUNDINTERESTINTERVAL = aFIXEDDEPOSIT.INTERESTMODE == ConstantVariable.INTERESTMODE_FLAT ? null : Convert.ToString(aFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL);   // oldFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL;
                                                    
                    
                    oCommonFunction.CustomObjectNullValidation<FDRNOTE>(ref oFDRNote);
                    db.FDRNOTEs.Add(oFDRNote);


                    //Create Encashment Note
                    if (chkPrincpleNet == false)
                    {
                        FDRNOTE oFDREncashmentNote = new FDRNOTE();

                        oFDREncashmentNote.REFERENCE = Guid.NewGuid().ToString();
                        oFDREncashmentNote.FIXEDDEPOSIT_REFERENCE = oldFIXEDDEPOSIT.REFERENCE;
                        oFDREncashmentNote.FINANCIALINSTITUTION_REFERENCE = oldFIXEDDEPOSIT.FINANCIALINSTITUTION.REFERENCE;
                        oFDREncashmentNote.BRANCH_REFERENCE = oldFIXEDDEPOSIT.BRANCH_REFERENCE;

                        oFDREncashmentNote.NOTEID = Guid.NewGuid().ToString();
                        oFDREncashmentNote.CAPLIMIT = oldFIXEDDEPOSIT.FINANCIALINSTITUTION.CAPLIMIT;
                        oFDREncashmentNote.NOTETYPE = ConstantVariable.STATUS_NOTETYPE_ENCASH;   // "Encash";
                        oFDREncashmentNote.PROPOSALSUMMARY = oldFIXEDDEPOSIT.CREATEDDATE.Value.ToString("dd-MM-yyyy") + " - " + (oldFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME.Count() > 10 ? oldFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME.Substring(0, 10) : oldFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME);
                        oFDREncashmentNote.STATUS = ConstantVariable.STATUS_PENDING;   // "Pending";
                       
                        oFDREncashmentNote.PRINCIPALAMOUNT = oldFIXEDDEPOSIT.NETINTERESTRECEIVABLE;                     
                        
                        oFDRNote.EXISTINGDEPOSIT = _var.ExistingDeposit;

                        oFDREncashmentNote.TENURE = oldFIXEDDEPOSIT.TENURE;
                        oFDREncashmentNote.TENURETERM = oldFIXEDDEPOSIT.TENURETERM;
                        oFDREncashmentNote.OFFERRATE = oldFIXEDDEPOSIT.RATEOFINTEREST;
                        oFDREncashmentNote.FDRNUMBER = oldFIXEDDEPOSIT.DEPOSITNUMBER;
                        oFDREncashmentNote.CREATEDBY = Session["UserId"].ToString();

                        oFDREncashmentNote.CREATEDDATE = DateTime.Now; // DateTime.Today;
                        //
                        oFDREncashmentNote.OPENEDDATE = oldFIXEDDEPOSIT.OPENINGDATE; // newly added

                        //
                        oFDREncashmentNote.CONTACTPERSON = prevContactPerson;
                        oFDREncashmentNote.SIGNATORY1 = Convert.ToString(PrevSignatory1); 
                        oFDREncashmentNote.SIGNATORY2 = Convert.ToString(PrevSignatory2); 


                        oFDREncashmentNote.ANNUALDAYS = Convert.ToInt32(oldFIXEDDEPOSIT.ANNUALDAYS.Value);
                        oFDREncashmentNote.INTERESTMODE = oldFIXEDDEPOSIT.INTERESTMODE;
                        oFDREncashmentNote.COMPOUNDINTERESTINTERVAL = oldFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL;

                      

                        oCommonFunction.CustomObjectNullValidation<FDRNOTE>(ref oFDRNote);
                        db.FDRNOTEs.Add(oFDREncashmentNote);
                    }
                    //End Encashment Note

                    db.SaveChanges();
                  }
                }



                return RedirectToAction("ListFDRRenewalNote", "FDRNote", new { lblbreadcum = "FDR Renewal Note" });

            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _var.ErrorMessage = "Field Name :" + validationError.PropertyName + " Message :" + validationError.ErrorMessage + "\n";
                        //Trace.TraceInformation("Property: {0} Error: {1}",
                        //                        validationError.PropertyName,
                        //                        validationError.ErrorMessage);
                    }
                }
                return RedirectToAction("Index", "ErrorPage", new { message = _var.ErrorMessage });
            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
        

        /// <summary>
        /// Print Renewal Note Letter from hear ref comes from FixedDeposit directly
        /// Itsa carbon copy of FDRNote PrintRenewalNoteLetter action where ref comes from FDRNOTE
        /// </summary>        
        [HttpPost]
        public ActionResult FdPrintRenewalNoteLetter(string fixedDepoRef)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                List<FIXEDDEPOSIT> fdrnoteList = new List<FIXEDDEPOSIT>();
                ReportDataSource oFDRNotes = new ReportDataSource();


               
                fdrnoteList = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.REFERENCE == fixedDepoRef).OrderBy(pi => pi.CREATEDDATE).ToList();
               
              
                var newfdrNoteList = from fixedDeposit in fdrnoteList                                    
                                     select new
                                     {

                                         NAME = fixedDeposit.FINANCIALINSTITUTION.NAME,
                                         BRANCH = fixedDeposit.FIBRANCH.NAME + " Branch\n" + fixedDeposit.FIBRANCH.ADDRESSLINE1 + "\n" + fixedDeposit.FIBRANCH.ADDRESSLINE2,
                                         FDRNUMBER = fixedDeposit.DEPOSITNUMBER,

                                         MATURITYDATE =fixedDeposit.MATURITYDATE,
                                         //fdrNote.FIXEDDEPOSIT.MATURITYDATE,

                                         TENURE = fixedDeposit.TENURE,
                                         TENURETERM = fixedDeposit.TENURETERM,

                                         PRINCIPALAMOUNT =fixedDeposit.PRINCIPALAMOUNT,

                                         //db.FIXEDDEPOSITs.Where(t => t.DEPOSITNUMBER == fdrNote.FDRNUMBER && t.STATUS==ConstantVariable.STATUS_RENEWED).SingleOrDefault().PRINCIPALAMOUNT,     //fdrNote.PRINCIPALAMOUNT,

                                         EXISTINGDEPOSIT =0,
                                         PERCENTAGEOFFDR = 0,
                                         CAPLIMIT =0,
                                         OFFERRATE =0,
                                         PROPOSEDRATE = fixedDeposit.RATEOFINTEREST,
                                         CONTACTPERSON = "",
                                         CHEQUENO = fixedDeposit.CHEQUEREFERENCE,

                                         //display this deposit opened date or IssueDate not Checque Date we get it from Fixed Deposit
                                         CHEQUEDATE =fixedDeposit.OPENINGDATE,

                                         //db.FIXEDDEPOSITs.Where(t => t.DEPOSITNUMBER == fdrNote.FDRNUMBER && t.STATUS == ConstantVariable.STATUS_RENEWED).SingleOrDefault().OPENINGDATE,

                                         //if diposit was compounding displya with interval else disply none
                                         STATUS = fixedDeposit.INTERESTMODE == ConstantVariable.INTERESTMODE_COMPOUND ? " " + fixedDeposit.COMPOUNDINTERESTINTERVAL.ToLower() + " compounding" : ""
                                     };



                LocalReport lr = new LocalReport();
                ReportDataSource rd = new ReportDataSource();              

                DataTable dtFDRNote = oCommonFunction.ConvertToDataTable(newfdrNoteList.ToList());
                rd.Name = "PurchaseNoteList";
                rd.Value = dtFDRNote;


                lr.ReportPath = Server.MapPath("~/Reports/FDRRenewalNoteLetter.rdlc");           
                lr.DataSources.Add(rd);            


                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =

                "<DeviceInfo>" +
                    "  <OutputFormat>PDF</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>1in</MarginLeft>" +
                    "  <MarginRight>1in</MarginRight>" +
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

                string reportName = fdrnoteList[0].FINANCIALINSTITUTION.NAME + "-RenewalNote.pdf";

                return File(renderedBytes, mimeType, reportName);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        
        }
      
        
        [HttpGet]
        public ActionResult EncashFixedDeposit(string id)
        {
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();

                FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.Status = oFIXEDDEPOSIT.STATUS;            


                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Encash " + Session["currentPage"];

                TempData["FDR"] = oFIXEDDEPOSIT;

                return PartialView("EncashFixedDeposit", oFIXEDDEPOSIT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        /// <summary>
        /// For Encashed Reconcilation
        /// </summary>
        
        [HttpPost]
        public ActionResult EncashFixedDeposit(FIXEDDEPOSIT aFIXEDDEPOSIT)
        {


            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                    oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == aFIXEDDEPOSIT.REFERENCE);
                    
                    oFIXEDDEPOSIT.LASTUPDATED = DateTime.Now;
                    oFIXEDDEPOSIT.LASTUPDATEDBY = Session["UserId"].ToString();
                    oFIXEDDEPOSIT.STATUS = ConstantVariable.STATUS_ENCASHED; //"Encashed";
                    
                    oFIXEDDEPOSIT.MRDATE = aFIXEDDEPOSIT.MRDATE;
                    oFIXEDDEPOSIT.MRNO = aFIXEDDEPOSIT.MRNO;
                    oFIXEDDEPOSIT.ENCASHMENTDATE = aFIXEDDEPOSIT.ENCASHMENTDATE;
                    oFIXEDDEPOSIT.ACTUALINTERESTRECEIVED = aFIXEDDEPOSIT.ACTUALINTERESTRECEIVED;

                    //though Excise Duties(ED) and Other Charge(OC) changed so we need to update ED and OC
                    //and also NetInterest and Principle amount so that FDRStatement report will be accurate 
                    //otherwise FDRStatement report remain previous data

                    oFIXEDDEPOSIT.GROSSINTEREST = aFIXEDDEPOSIT.GROSSINTEREST;
                    oFIXEDDEPOSIT.SOURCETAX = aFIXEDDEPOSIT.SOURCETAX;
                    oFIXEDDEPOSIT.EXCISEDUTY = aFIXEDDEPOSIT.EXCISEDUTY;
                    oFIXEDDEPOSIT.OTHERCHARGE = aFIXEDDEPOSIT.OTHERCHARGE;
                    oFIXEDDEPOSIT.NETINTERESTRECEIVABLE = aFIXEDDEPOSIT.NETINTERESTRECEIVABLE;
                    oFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE = aFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE;
                                
                    oFIXEDDEPOSIT.PRESENTPRINCIPALAMOUNT = aFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE;
                   
                    oCommonFunction.CustomObjectNullValidation<FIXEDDEPOSIT>(ref oFIXEDDEPOSIT);

                    db.Entry(oFIXEDDEPOSIT).State = EntityState.Modified;


                    #region Block FDREncashment                    
                                                                
                   
                    //Added By Nazmul:11-07-2015
                    //Create financial transaction
                     new TransactionEngine().CreateFinancialTransaction("FDR02", oFIXEDDEPOSIT.PRINCIPALAMOUNT, oFIXEDDEPOSIT.ENCASHMENTDATE);

                    #endregion

                    //Update FDR Note added by rakibul date<20th march,2016>
                    FDRNOTE oFDRNote = db.FDRNOTEs.Where(i => i.FIXEDDEPOSIT_REFERENCE == aFIXEDDEPOSIT.REFERENCE && i.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_ENCASH).SingleOrDefault();
                       
                    oFDRNote.CHEQUENO = oFIXEDDEPOSIT.CHEQUEREFERENCE; 
                  

                    oFDRNote.LASTUPDATED = DateTime.Now;
                    oFDRNote.LASTUPDATEDBY = Session["UserId"].ToString();

                    //added by rakibul date<3th march,2016>
                    oFDRNote.PRINCIPALAMOUNT = aFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE;
                    oFDRNote.INTERESTMODE = oFIXEDDEPOSIT.INTERESTMODE; 
                    oFDRNote.COMPOUNDINTERESTINTERVAL = oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL; 
                    oFDRNote.SIGNATORY1 = oFIXEDDEPOSIT.SIGNATORY1; 
                    oFDRNote.SIGNATORY2 = oFIXEDDEPOSIT.SIGNATORY2;                               
                   
                    
                    db.Entry(oFDRNote).State = EntityState.Modified;

                    //Save all information
                    db.SaveChanges();
                }

                return RedirectToAction("ListFixedDepositRegister", "FixedDepositRegister");

            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult AddRemark(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();

                FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == id);
              
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Encash " + Session["currentPage"];

                return PartialView("AddRemark", oFIXEDDEPOSIT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }        
        }

        [HttpPost]
        public ActionResult AddRemarks(FIXEDDEPOSIT Old_FIXEDDEPOSIT)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();

                FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == Old_FIXEDDEPOSIT.REFERENCE);
                //now add remarks

                oFIXEDDEPOSIT.REMARKS = Old_FIXEDDEPOSIT.REMARKS;

                db.Entry(oFIXEDDEPOSIT).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ListFixedDepositRegister", "FixedDepositRegister");
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        [HttpGet]
        public ActionResult ViewOnlyFixedDeposit(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();

                FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.Status = oFIXEDDEPOSIT.STATUS;

                ViewBag.InterestDifference = oFIXEDDEPOSIT.NETINTERESTRECEIVABLE - oFIXEDDEPOSIT.ACTUALINTERESTRECEIVED;

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Encash " + Session["currentPage"];

                return PartialView("ViewOnlyFixedDeposit", oFIXEDDEPOSIT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        /// <summary>
        /// Statement report generator
        /// And all report genarator
        /// </summary>     
        public ActionResult GenerateFDRRegisterReport(string title = null, string reportName = null,string reference=null)
        {
             Entities db;
             if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
             {
                 return RedirectToAction("LogOut", "Home");
             }
             else
             {
             db  = new Entities(Session["Connection"] as EntityConnection);
             }
             
            List<FIXEDDEPOSIT> models = new List<FIXEDDEPOSIT>();
            ReportDataSource oFDRStatement = new ReportDataSource();           
          
            
            models = TempData["FIXEDDEPOSITList"] as List<FIXEDDEPOSIT>;

            if (reference != null)
            {
                models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(t => t.REFERENCE == reference).ToList();
            }
          

            if (models != null)
            {

                try
                {

                    var newModels = from item in models
                                    select new
                                    {
                                        REFERENCE = item.REFERENCE,
                                        CREATEDDATE = item.CREATEDDATE,
                                        CREATEDBY = item.CREATEDBY,
                                        LASTUPDATED = item.LASTUPDATED,
                                        LASTUPDATEDBY = item.LASTUPDATEDBY,
                                        DEPOSITNUMBER = item.DEPOSITNUMBER,
                                        PRINCIPALAMOUNT = item.PRINCIPALAMOUNT,
                                        CHEQUEDATE = item.CHEQUEDATE,
                                        CHEQUEREFERENCE = item.CHEQUEREFERENCE,
                                        TENURE = item.TENURE,
                                        TENURETERM = item.TENURETERM,
                                        TERMSINDAYS = item.TERMSINDAYS,
                                        INTERESTRECEIVINGPERIOD = item.INTERESTRECEIVINGPERIOD,
                                        MATURITYDATE = item.MATURITYDATE,
                                        EXISTINGCAPLIMIT = item.EXISTINGCAPLIMIT,
                                        RATEOFINTEREST = item.RATEOFINTEREST,
                                        ADVANCEDINTERESTRATE = item.ADVANCEDINTERESTRATE,
                                        INTERESTMODE = item.INTERESTMODE,
                                        COMPOUNDINTERESTTYPE = item.COMPOUNDINTERESTTYPE,
                                        COMPOUNDINTERESTINTERVAL = item.COMPOUNDINTERESTINTERVAL,
                                        ANNUALDAYS = item.ANNUALDAYS,
                                        STATUS = item.STATUS,
                                        ACCEPTEDBY = item.ACCEPTEDBY,
                                        ACCEPTEDDATE = item.ACCEPTEDDATE,
                                        REJECTEDBY = item.REJECTEDBY,
                                        REJECTEDDATE = item.REJECTEDDATE,
                                        OPENINGDATE = item.OPENINGDATE,
                                        RENWALDATE = item.RENWALDATE,
                                        RENEWALDEPOSITNUMBER = item.RENEWALDEPOSITNUMBER,
                                        TAXDEDUCTIONCRITERIA = item.TAXDEDUCTIONCRITERIA,
                                        HOLDINGPERIOD = item.HOLDINGPERIOD,
                                        GROSSINTEREST = item.GROSSINTEREST,
                                        SOURCETAX = item.SOURCETAX,
                                        EXCISEDUTY = item.EXCISEDUTY,
                                        OTHERCHARGE = item.OTHERCHARGE,
                                        PRESENTPRINCIPALAMOUNT = item.PRESENTPRINCIPALAMOUNT,                                        
                                        REMARKS =item.REMARKS,
                                        //item.REMARKS!=null?"":item.REMARKS,  //because we use this field to trace is this deposit encase or renew but in report it should empty
                                        NETINTERESTRECEIVABLE = item.NETINTERESTRECEIVABLE,
                                        TOTALAMOUNTRECEIVABLE = item.TOTALAMOUNTRECEIVABLE,
                                        RECEIVABLETILL = item.RECEIVABLETILL,
                                        MRNO = item.MRNO,
                                        MRDATE = item.MRDATE,
                                        ENCASHMENTDATE = item.ENCASHMENTDATE,
                                        ACTUALINTERESTRECEIVED = item.ACTUALINTERESTRECEIVED,
                                        NAME = item.FINANCIALINSTITUTION.NAME
                                    };
                    LocalReport lr = new LocalReport();
                    lr.ReportPath = Server.MapPath("~/Reports/FDRStatement.rdlc");

                    string report_title;
                    string report_name;

                    if (title != null && reportName != null)
                    {
                        report_title = title;
                        report_name = reportName;
                    }
                    else
                    {
                        report_title = "FDR Statement";
                        report_name = "FDRStatement.pdf";
                    }
                    
                    ReportDataSource rd = new ReportDataSource();
                    //ReportDataSource dd = new ReportDataSource();

                    DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(newModels.ToList());
                    rd.Name = "FixedDeposit";
                    rd.Value = dtFDRStatement;


            ReportParameter[] parameters = new ReportParameter[] 
              {
              new ReportParameter("CompanyName","DLIC"),
              new ReportParameter("Address","Gulshan-2, Dhaka"),
              new ReportParameter("ReportTitle",report_title)              
            };


                    lr.ReportPath = Server.MapPath("~/Reports/FDRStatement.rdlc");
                    lr.SetParameters(parameters);
                    lr.DataSources.Add(rd);
                    //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);


                    string reportType = "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.5in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0.5in</MarginTop>" +
                        "  <MarginLeft>1in</MarginLeft>" +
                        "  <MarginRight>1in</MarginRight>" +
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

                    //Response.AddHeader("content-disposition",
                    //              "attachment; filename=WeeklyAisleReport-" + DateTime.Now + "." +
                    //              fileNameExtension);        





                    renderedBytes = lr.Render(reportType);

                    return File(renderedBytes, mimeType, report_name);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Index", "ErrorPage", new { message =Convert.ToString(ex.Message) });
                }
            }
            else {
                return RedirectToAction("Index", "ErrorPage", new { message="Session time out!!" });
            
            }

        }

        /// <summary>
        /// FDR NOTE or Encashment Worksheet report
        /// </summary>       
        [HttpPost]
        public ActionResult GenerateEncashedFDRReport(string InExcel,FIXEDDEPOSIT oFIXEDDEPOSIT, string reference, string ED, string OC, string MRNo, string MRDate, string fdrReference = null)
        {
            int fromTempData = 0;
            try
            {
                //return if session closed
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                //if request come from list then get the reference of reconcilled fixedDeposit
                if (fdrReference != null && !string.IsNullOrEmpty(fdrReference))
                {

                    FDRNOTE oFDRNOTE = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.SingleOrDefault(i => i.REFERENCE == fdrReference);
                    reference = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == oFDRNOTE.FIXEDDEPOSIT.REFERENCE).REFERENCE;
                }
                if (TempData["FDR"] != null)
                {
                    oFIXEDDEPOSIT = TempData["FDR"] as FIXEDDEPOSIT;
                    fromTempData = 1;
                }

                if (fromTempData == 1)
                    oFIXEDDEPOSIT = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Where(e => e.REFERENCE == oFIXEDDEPOSIT.REFERENCE).FirstOrDefault();

                else if (reference != null && !string.IsNullOrEmpty(reference))
                    oFIXEDDEPOSIT = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Where(e => e.REFERENCE == reference).FirstOrDefault();

                oCommonFunction.CustomObjectNullValidation(ref oFIXEDDEPOSIT);

                //this is come when Encash prev 24th Feb 2016
                // List<FDRENCASHMENT> oFDRENCASHMENTList = new List<FDRENCASHMENT>();
                //now we get this data from FDRInterest Table 
                List<FDRINTEREST> oFDRInterestList = new List<FDRINTEREST>();
                List<FDRINTEREST_Report> oFDRInterestReport = new List<FDRINTEREST_Report>();

                List<FIXEDDEPOSIT> fdrList = new List<FIXEDDEPOSIT>();


                ReportDataSource oFDRStatement = new ReportDataSource();

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    //  oFDRENCASHMENTList = db.FDRENCASHMENTs.Where(e => e.FDRNUMBER == oFIXEDDEPOSIT.DEPOSITNUMBER).ToList();
                    oFDRInterestList = db.FDRINTERESTs.Where(e => e.FIXEDDEPOSIT_REFERENCE == oFIXEDDEPOSIT.REFERENCE).ToList();
                }

                _var.SourceTax = 0;
                _var.GrossInterest = 0;
                _var.ExciseDuties = oFIXEDDEPOSIT.EXCISEDUTY.Value;
                _var.OthersCharge = oFIXEDDEPOSIT.OTHERCHARGE.Value;
                _var.NetInterestReceivable = 0;

                //foreach (var item in oFDRInterestList)
                //{
                //    _var.GrossInterest += item.GROSSINTEREST.Value;
                //    _var.SourceTax += item.SOURCETAX.Value;
                //    _var.NetInterestReceivable += item.NETINTERESTRECEIVABLE;               

                //}

                _var.GrossInterest = oFIXEDDEPOSIT.GROSSINTEREST.Value;
                _var.SourceTax = oFIXEDDEPOSIT.SOURCETAX.Value;
                _var.NetInterestReceivable = oFIXEDDEPOSIT.NETINTERESTRECEIVABLE;


                if (ED != null && !string.IsNullOrEmpty(ED))
                {
                    _var.ExciseDuties = Convert.ToDecimal(ED);
                    oFIXEDDEPOSIT.EXCISEDUTY = _var.ExciseDuties;
                }
                if (OC != null && !string.IsNullOrEmpty(OC))
                {
                    _var.OthersCharge = Convert.ToDecimal(OC);
                    oFIXEDDEPOSIT.OTHERCHARGE = _var.OthersCharge;
                }

                _var.NetInterestReceivable = _var.GrossInterest.Value - _var.SourceTax.Value - _var.ExciseDuties.Value - _var.OthersCharge.Value;

                oFIXEDDEPOSIT.NETINTERESTRECEIVABLE = _var.NetInterestReceivable;

                if (oFIXEDDEPOSIT.ACTUALINTERESTRECEIVED.Value > 0)
                {
                    oFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE = oFIXEDDEPOSIT.PRINCIPALAMOUNT + oFIXEDDEPOSIT.ACTUALINTERESTRECEIVED;
                }
                else
                {
                    oFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE = oFIXEDDEPOSIT.PRINCIPALAMOUNT + _var.NetInterestReceivable;
                }
                fdrList.Add(oFIXEDDEPOSIT);

                DataTable oFDRINTEREST = null;

                //added by Rakibul           

                int opeingYear = oFIXEDDEPOSIT.OPENINGDATE.Value.Year;
                int MaturedYear = oFIXEDDEPOSIT.MATURITYDATE.Value.Year;

                ReportParameter[] parameters;





                //check is the year Same
                if (opeingYear == MaturedYear)
                {

                    //_var.GrossInteresStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(Math.Round(_var.GrossInterest.Value,2))).Replace("$", string.Empty); ;
                    //_var.NetInterestStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(Math.Round(_var.NetInterestReceivable.Value,2))).Replace("$", string.Empty); ;
                    //_var.SourceTaxStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(Math.Round(_var.SourceTax.Value,2))).Replace("$", string.Empty); ;

                    oFDRInterestReport.Add(new FDRINTEREST_Report { RECEIVEDON = oFIXEDDEPOSIT.MATURITYDATE.Value.ToString("dd-MMM-yy"), GROSSINTEREST = _var.GrossInterest.Value, SOURCETAX = _var.SourceTax.Value, NETINTERESTRECEIVABLE = _var.NetInterestReceivable.Value, EXCISEDUTY = oFIXEDDEPOSIT.EXCISEDUTY.Value, OTHERCHARGE = oFIXEDDEPOSIT.OTHERCHARGE.Value });                   //_var.OthersCharge.Value
                    //noting to do
                    parameters = new ReportParameter[] 
             {
             //new ReportParameter("CompanyName","DLIC"),
             //new ReportParameter("Address","Gulshan-2, Dhaka"),
              new ReportParameter("ReportTitle",oFIXEDDEPOSIT.STATUS != "Encashed"? "FDR Statement": "FDR Encashment"),
              new ReportParameter("FIName",oFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME),
              new ReportParameter("FIBRANCH",oFIXEDDEPOSIT.FIBRANCH.NAME),

         
                           
              
            };
                }
                else
                {
                    string ReceivedOn;
                    DateTime OpeningYearLastDate = new DateTime(opeingYear, 12, 31);

                    DateTime OpeningYearFirstDate = oFIXEDDEPOSIT.OPENINGDATE.Value;

                    TimeSpan openDateDifference;
                    double TotalDaysThisYear;

                    //if year does not same then calculate total days
                    TimeSpan difference = oFIXEDDEPOSIT.MATURITYDATE.Value - oFIXEDDEPOSIT.OPENINGDATE.Value;
                    double totalDays = difference.TotalDays;

                    decimal PerDayGrossInterest = oFIXEDDEPOSIT.GROSSINTEREST.Value / Convert.ToDecimal(totalDays);

                    decimal PerDayNetInterest = _var.NetInterestReceivable.Value / Convert.ToDecimal(totalDays);

                    decimal PerDaySourceTax = oFIXEDDEPOSIT.SOURCETAX.Value / Convert.ToDecimal(totalDays);

                    //   ViewBag.Principle = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(item.PRINCIPALAMOUNT)).Replace("$", string.Empty); 

                    //get how many year
                    for (int year = opeingYear; year <= MaturedYear; year++)
                    {
                        if (year == MaturedYear)
                        {
                            ReceivedOn = oFIXEDDEPOSIT.MATURITYDATE.Value.ToString("dd-MMM-yy");
                            openDateDifference = oFIXEDDEPOSIT.MATURITYDATE.Value - OpeningYearFirstDate;
                            TotalDaysThisYear = openDateDifference.TotalDays;

                            if (ED != null && !string.IsNullOrEmpty(ED))
                                _var.ExciseDuties = Convert.ToDecimal(ED);
                            else
                                _var.ExciseDuties = oFIXEDDEPOSIT.EXCISEDUTY.Value;
                            if (OC != null && !string.IsNullOrEmpty(OC))
                                _var.OthersCharge = Convert.ToDecimal(OC);
                            else
                                _var.OthersCharge = oFIXEDDEPOSIT.OTHERCHARGE.Value;
                        }
                        else
                        {
                            OpeningYearLastDate = new DateTime(year, 12, 31);
                            ReceivedOn = new DateTime(year, 12, 31).ToString("dd-MMM-yy");

                            openDateDifference = OpeningYearLastDate - OpeningYearFirstDate;
                            TotalDaysThisYear = openDateDifference.TotalDays;

                            _var.ExciseDuties = 0;
                            _var.OthersCharge = 0;
                        }

                        //incriment one day to get next Opening Year First Date
                        OpeningYearFirstDate = OpeningYearLastDate;
                        //.AddDays(1);

                        _var.NetInterestReceivable = Math.Round((PerDayNetInterest * Convert.ToDecimal(TotalDaysThisYear)), 2);
                        _var.SourceTax = Math.Round((PerDaySourceTax * Convert.ToDecimal(TotalDaysThisYear)), 2);
                        _var.GrossInterest = Math.Round((PerDayGrossInterest * Convert.ToDecimal(TotalDaysThisYear)), 2);

                        oFDRInterestReport.Add(new FDRINTEREST_Report { RECEIVEDON = ReceivedOn, GROSSINTEREST = _var.GrossInterest.Value, SOURCETAX = _var.SourceTax.Value, NETINTERESTRECEIVABLE = _var.NetInterestReceivable.Value, EXCISEDUTY = _var.ExciseDuties.Value, OTHERCHARGE = _var.OthersCharge.Value });

                        //below convert above _var.NetInterestReceivable,_var.SourceTax,_var.GrossInterest  to string with comma formate 2 decimal places
                        //_var.GrossInteresStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(_var.GrossInterest.Value)).Replace("$", string.Empty); ;
                        //_var.NetInterestStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(_var.NetInterestReceivable.Value)).Replace("$", string.Empty); ;
                        //_var.SourceTaxStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(_var.SourceTax.Value)).Replace("$", string.Empty); ;
                        //oFDRInterestReport.Add(new FDRINTEREST_Report { RECEIVEDON = ReceivedOn, GROSSINTEREST = _var.GrossInteresStr, SOURCETAX = _var.SourceTaxStr, NETINTERESTRECEIVABLE = _var.NetInterestStr, EXCISEDUTY = Convert.ToString(_var.ExciseDuties.Value), OTHERCHARGE = Convert.ToString(_var.OthersCharge.Value) });                   

                    }



                    parameters = new ReportParameter[] 
            {
             //new ReportParameter("CompanyName","DLIC"),
             //new ReportParameter("Address","Gulshan-2, Dhaka"),
              new ReportParameter("ReportTitle",oFIXEDDEPOSIT.STATUS != "Encashed"? "FDR Statement": "FDR Encashment"),
              new ReportParameter("FIName",oFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME),
              new ReportParameter("FIBRANCH",oFIXEDDEPOSIT.FIBRANCH.NAME),        
              
             };
                }



                LocalReport lr = new LocalReport();
                lr.ReportPath = Server.MapPath("~/Reports/FDREncashment.rdlc");

                ReportDataSource rd = new ReportDataSource();
                ReportDataSource dd = new ReportDataSource();


                rd.Name = "FixedDeposit";
                rd.Value = oCommonFunction.ConvertToDataTable(fdrList);

                dd.Name = "FDRInterest";
                dd.Value = oCommonFunction.ConvertToDataTable(oFDRInterestReport);



                lr.ReportPath = Server.MapPath("~/Reports/FDREncashment.rdlc");
                // lr.SetParameters(parameters);
                lr.DataSources.Add(rd);
                lr.DataSources.Add(dd);
                lr.SetParameters(parameters);
                //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);

                string report_name = "Encashed_FDR_" + oFIXEDDEPOSIT.DEPOSITNUMBER;

                string ContentType = "application/vnd.ms-excel"; ;
                string FileType;

                if (InExcel == "true")
                {
                    FileType = "Excel";
                    report_name = report_name + ".{0}";
                }
                else
                {
                    FileType = "PDF";
                    report_name = report_name + ".pdf";
                }

                string reportType = FileType; // "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =

                "<DeviceInfo>" +
                    "  <OutputFormat>" + FileType + "</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>1in</MarginLeft>" +
                    "  <MarginRight>1in</MarginRight>" +
                    "  <MarginBottom>0.5in</MarginBottom>" +
                    "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                if (InExcel == "true")
                {
                    renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                    return File(renderedBytes, ContentType, string.Format(report_name, fileNameExtension));
                }
                else
                {
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
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }

        [HttpPost]
        public ActionResult GenerateRenewFDRReport(FIXEDDEPOSIT oFIXEDDEPOSIT, string id, string InExcel)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (TempData["renewFDR"] != null)
                {
                    oFIXEDDEPOSIT = TempData["renewFDR"] as FIXEDDEPOSIT;
                    ViewBag.renewFDR = oFIXEDDEPOSIT;
                }
                else if (!string.IsNullOrEmpty(id) && id != null)
                {
                    oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                    oFIXEDDEPOSIT = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == id);
                }
                oCommonFunction.CustomObjectNullValidation(ref oFIXEDDEPOSIT);

                //List<FDRENCASHMENT> oFDRENCASHMENTList = new List<FDRENCASHMENT>();
                List<FDRINTEREST> oFDRInterestList = new List<FDRINTEREST>();
                List<FDRINTEREST_Report> oFDRInterestReport = new List<FDRINTEREST_Report>();
                
                //For FDRRenewal.rdlc report Principal Amount will be InitialPrincipalAmount or PresentprincipalAmount-NetInterest
                oFIXEDDEPOSIT.PRINCIPALAMOUNT = oFIXEDDEPOSIT.OPENINGDATE == oFIXEDDEPOSIT.INITIALOPENINGDATE ? oFIXEDDEPOSIT.INITIALPRINCIPALAMOUNT : oFIXEDDEPOSIT.PRINCIPALAMOUNT; //bcause report dataset does not contain InitialPricipalAmount

                List<FIXEDDEPOSIT> fdrList = new List<FIXEDDEPOSIT>();
                fdrList.Add(oFIXEDDEPOSIT);
                ReportDataSource oFDRStatement = new ReportDataSource();

                _var.SourceTax = 0;
                _var.GrossInterest = 0;
                _var.ExciseDuties = 0;
                _var.OthersCharge = 0;
                _var.NetInterestReceivable = 0;

                _var.GrossInterest =oFIXEDDEPOSIT.GROSSINTEREST.Value;
                _var.SourceTax =oFIXEDDEPOSIT.SOURCETAX.Value;
                _var.NetInterestReceivable =oFIXEDDEPOSIT.NETINTERESTRECEIVABLE;
                                       

                int opeingYear = oFIXEDDEPOSIT.OPENINGDATE.Value.Year;
                int MaturedYear = oFIXEDDEPOSIT.MATURITYDATE.Value.Year;

                ReportParameter[] parameters;

                parameters = new ReportParameter[] 
                       {
       
              new ReportParameter("FIName",oFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME),
              new ReportParameter("FIBRANCH",oFIXEDDEPOSIT.FIBRANCH.NAME),

              };

                //check is the year Same
                if (opeingYear == MaturedYear)
                {

                    oFDRInterestReport.Add(new FDRINTEREST_Report { RECEIVEDON = oFIXEDDEPOSIT.MATURITYDATE.Value.ToString("dd-MMM-yy"), GROSSINTEREST = _var.GrossInterest.Value, SOURCETAX = _var.SourceTax.Value, NETINTERESTRECEIVABLE = _var.NetInterestReceivable.Value, EXCISEDUTY = oFIXEDDEPOSIT.EXCISEDUTY.Value, OTHERCHARGE = oFIXEDDEPOSIT.OTHERCHARGE.Value });                   
               
                    //_var.GrossInteresStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(Math.Round(_var.GrossInterest.Value, 2))).Replace("$", string.Empty); ;
                    //_var.NetInterestStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(Math.Round(_var.NetInterestReceivable.Value, 2))).Replace("$", string.Empty); ;
                    //_var.SourceTaxStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(Math.Round(_var.SourceTax.Value, 2))).Replace("$", string.Empty); ;

                    //oFDRInterestReport.Add(new FDRINTEREST_Report { RECEIVEDON = oFIXEDDEPOSIT.MATURITYDATE.Value.ToString("dd-MMM-yy"), GROSSINTEREST = _var.GrossInteresStr, SOURCETAX = _var.SourceTaxStr, NETINTERESTRECEIVABLE = _var.NetInterestStr, EXCISEDUTY = Convert.ToString(oFIXEDDEPOSIT.EXCISEDUTY.Value), OTHERCHARGE = Convert.ToString(oFIXEDDEPOSIT.OTHERCHARGE.Value) });
                    //noting to do
                  
                }
                else
                {
                    string ReceivedOn;
                    DateTime OpeningYearLastDate = new DateTime(opeingYear, 12, 31);

                    DateTime OpeningYearFirstDate = oFIXEDDEPOSIT.OPENINGDATE.Value;

                    TimeSpan openDateDifference;
                    double TotalDaysThisYear;

                    //if year does not same then calculate total days
                    TimeSpan difference = oFIXEDDEPOSIT.MATURITYDATE.Value - oFIXEDDEPOSIT.OPENINGDATE.Value;
                    double totalDays = difference.TotalDays;

                    decimal PerDayGrossInterest = oFIXEDDEPOSIT.GROSSINTEREST.Value / Convert.ToDecimal(totalDays);
                    decimal PerDayNetInterest = oFIXEDDEPOSIT.NETINTERESTRECEIVABLE.Value / Convert.ToDecimal(totalDays);
                    decimal PerDaySourceTax = oFIXEDDEPOSIT.SOURCETAX.Value / Convert.ToDecimal(totalDays);

                    //   ViewBag.Principle = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(item.PRINCIPALAMOUNT)).Replace("$", string.Empty); 

                    //get how many year
                    for (int year = opeingYear; year <= MaturedYear; year++)
                    {
                        if (year == MaturedYear)
                        {
                            ReceivedOn = oFIXEDDEPOSIT.MATURITYDATE.Value.ToString("dd-MMM-yy");
                            openDateDifference = oFIXEDDEPOSIT.MATURITYDATE.Value - OpeningYearFirstDate;
                            TotalDaysThisYear = openDateDifference.TotalDays;

                            _var.ExciseDuties = oFIXEDDEPOSIT.EXCISEDUTY.Value;
                            _var.OthersCharge = oFIXEDDEPOSIT.OTHERCHARGE.Value;
                        }
                        else
                        {
                            OpeningYearLastDate = new DateTime(year, 12, 31);
                            ReceivedOn = new DateTime(year, 12, 31).ToString("dd-MMM-yy");

                            openDateDifference = OpeningYearLastDate - OpeningYearFirstDate;
                            TotalDaysThisYear = openDateDifference.TotalDays;

                            _var.ExciseDuties = 0;
                            _var.OthersCharge = 0;
                        }

                        // OpeningYearLastDate will be the next OpeningYearFirstDate
                        OpeningYearFirstDate = OpeningYearLastDate;

                        _var.NetInterestReceivable = Math.Round((PerDayNetInterest * Convert.ToDecimal(TotalDaysThisYear)), 2);
                        _var.SourceTax = Math.Round((PerDaySourceTax * Convert.ToDecimal(TotalDaysThisYear)), 2);
                        _var.GrossInterest = Math.Round((PerDayGrossInterest * Convert.ToDecimal(TotalDaysThisYear)), 2);

                        oFDRInterestReport.Add(new FDRINTEREST_Report { RECEIVEDON = ReceivedOn, GROSSINTEREST = _var.GrossInterest.Value, SOURCETAX = _var.SourceTax.Value, NETINTERESTRECEIVABLE = _var.NetInterestReceivable.Value, EXCISEDUTY = _var.ExciseDuties.Value, OTHERCHARGE = _var.OthersCharge.Value });                   
               

                        //_var.GrossInteresStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(_var.GrossInterest.Value)).Replace("$", string.Empty); ;
                        //_var.NetInterestStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(_var.NetInterestReceivable.Value)).Replace("$", string.Empty); ;
                        //_var.SourceTaxStr = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(_var.SourceTax.Value)).Replace("$", string.Empty); ;
                        //oFDRInterestReport.Add(new FDRINTEREST_Report { RECEIVEDON = ReceivedOn, GROSSINTEREST = _var.GrossInteresStr, SOURCETAX = _var.SourceTaxStr, NETINTERESTRECEIVABLE = _var.NetInterestStr, EXCISEDUTY = Convert.ToString(_var.ExciseDuties.Value), OTHERCHARGE = Convert.ToString(_var.OthersCharge.Value) });

                    }
                }

                //


                LocalReport lr = new LocalReport();          

                ReportDataSource rd = new ReportDataSource();
                ReportDataSource dd = new ReportDataSource();
               
                rd.Name = "FixedDeposit";
                rd.Value = oCommonFunction.ConvertToDataTable(fdrList);
                dd.Name = "FDRRenew";              
                dd.Value = oCommonFunction.ConvertToDataTable(oFDRInterestReport);
                           

                lr.ReportPath = Server.MapPath("~/Reports/FDRRenewal.rdlc");
                lr.SetParameters(parameters);
                lr.DataSources.Add(rd);
                lr.DataSources.Add(dd);
                //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);

                string ContentType = "application/vnd.ms-excel"; 
                string FileType;
                string ReportName; 

                if (InExcel == "true")
                {
                    FileType = "Excel";
                    ReportName = "Renewal_FDR_" + oFIXEDDEPOSIT.DEPOSITNUMBER + ".{0}";
                }
                else
                {
                    FileType = "PDF";
                    ReportName ="Renewal_FDR_" + oFIXEDDEPOSIT.DEPOSITNUMBER + ".Pdf";

                }



                string reportType = FileType;
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =

                "<DeviceInfo>" +
                    "  <OutputFormat>" + FileType + "</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>1in</MarginLeft>" +
                    "  <MarginRight>1in</MarginRight>" +
                    "  <MarginBottom>0.5in</MarginBottom>" +
                    "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                              

                if (InExcel == "true")
                {
                    renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                    return File(renderedBytes, ContentType, string.Format(ReportName, fileNameExtension));
                }
                else
                {
                    renderedBytes = lr.Render(
                     reportType,
                     deviceInfo,
                     out mimeType,
                     out encoding,
                     out fileNameExtension,
                     out streams,
                     out warnings);

                    renderedBytes = lr.Render(reportType);
                    return File(renderedBytes, mimeType, ReportName);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        public ActionResult GenerateDemoReport(string title = null, string reportName = null)
        {
            try
            {


                ReportDataSource oFDRStatement = new ReportDataSource();


                LocalReport lr = new LocalReport();
                lr.ReportPath = Server.MapPath("~/Reports/FDRMaturityStatement.rdlc");

                string report_title;
                string report_name;

                if (title != null && reportName != null)
                {
                    report_title = title;
                    report_name = reportName;
                }
                else
                {
                    report_title = "FDR Statement";
                    report_name = "FDRStatement.pdf";
                }



                ReportDataSource rd = new ReportDataSource();
                //ReportDataSource dd = new ReportDataSource();

                // DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(newModels.ToList());
                rd.Name = "FixedDeposit";
                //rd.Value = dtFDRStatement;


                //ReportParameter[] parameters = new ReportParameter[] 
                //{
                // new ReportParameter("CompanyName","DLIC"),
                // new ReportParameter("Address","Gulshan-2, Dhaka"),
                //  new ReportParameter("ReportTitle",report_title)

                //};


                lr.ReportPath = Server.MapPath("~/Reports/FDRStatement.rdlc");
                //lr.SetParameters(parameters);
                //lr.DataSources.Add(rd);
                //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);


                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =

                "<DeviceInfo>" +
                    "  <OutputFormat>PDF</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>1in</MarginLeft>" +
                    "  <MarginRight>1in</MarginRight>" +
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

                //Response.AddHeader("content-disposition",
                //              "attachment; filename=WeeklyAisleReport-" + DateTime.Now + "." +
                //              fileNameExtension);        





                renderedBytes = lr.Render(reportType);

                return File(renderedBytes, mimeType, report_name);

            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }


        /// <summary>
        /// For Encashment Note
        /// </summary>
        [HttpGet]
        public ActionResult AddFDRNote(string fdrRegiterRef)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    //if (db.FDRNOTEs.Where(fdrNote => fdrNote.NOTETYPE ==ConstantVariable.STATUS_NOTETYPE_ENCASH && fdrNote.FIXEDDEPOSIT_REFERENCE == fdrRegiterRef).FirstOrDefault() != null)                      
                    //    return RedirectToAction("FDREncashmentNoteList"); //, new {lblbreadcum="FDR Encashment Note List" }

                    //if (db.FDRNOTEs.Where(fdrNote => fdrNote.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_RENEWAL && fdrNote.FIXEDDEPOSIT_REFERENCE == fdrRegiterRef).FirstOrDefault() != null)
                    //    return RedirectToAction("ListFDRRenewalNote");  //, new {lblbreadcum="Renewal Notes" }


                    db.Configuration.LazyLoadingEnabled = false;

                    FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                    oFIXEDDEPOSIT = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(app => app.REFERENCE == fdrRegiterRef).FirstOrDefault();

                    //This is written because branch and FI is not loading together.
                    FIXEDDEPOSIT oFIXEDDEPOSIT2 = new FIXEDDEPOSIT();
                    oFIXEDDEPOSIT2 = db.FIXEDDEPOSITs.Where(app => app.REFERENCE == fdrRegiterRef).FirstOrDefault();


                    ViewBag.EncashmentDate = oFIXEDDEPOSIT.MATURITYDATE.Value;  // Display Matured Date as Encashment Date

                    var r = new Random();
                    FDRNOTE oFDRNote = new FDRNOTE();

                    oFDRNote.REFERENCE = Guid.NewGuid().ToString();
                    oFDRNote.FIXEDDEPOSIT_REFERENCE = fdrRegiterRef;
                    oFDRNote.FINANCIALINSTITUTION_REFERENCE = oFIXEDDEPOSIT.FINANCIALINSTITUTION.REFERENCE;
                    oFDRNote.BRANCH_REFERENCE = oFIXEDDEPOSIT2.BRANCH_REFERENCE;
                    oFDRNote.NOTEID = Convert.ToString(r.Next(555555) + 1);
                    oFDRNote.CAPLIMIT = oFIXEDDEPOSIT.FINANCIALINSTITUTION.CAPLIMIT;
                    oFDRNote.NOTETYPE = ConstantVariable.STATUS_NOTETYPE_ENCASH; // "Encash";
                    oFDRNote.PROPOSALSUMMARY = (oFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME.Count() > 10 ? oFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME.Substring(0, 10) : oFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME) + "-" + oFIXEDDEPOSIT.CREATEDDATE;
                    oFDRNote.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";
                    oFDRNote.PRINCIPALAMOUNT = oFIXEDDEPOSIT.PRINCIPALAMOUNT + oFIXEDDEPOSIT.NETINTERESTRECEIVABLE;
                        //oFIXEDDEPOSIT.PRESENTPRINCIPALAMOUNT + oFIXEDDEPOSIT.NETINTERESTRECEIVABLE; //present principal already with netinterest, netinterest has twiced add with principal date 08-Mar-17

                    ViewBag.Principal = oFIXEDDEPOSIT.PRINCIPALAMOUNT; //Display without NetInterest
                    //oFIXEDDEPOSIT.PRINCIPALAMOUNT + oFIXEDDEPOSIT.NETINTERESTRECEIVABLE; 
                        // //block 20/11/16
                    


                    oFDRNote.TENURE = oFIXEDDEPOSIT.TENURE;
                    oFDRNote.TENURETERM = oFIXEDDEPOSIT.TENURETERM;
                    oFDRNote.OFFERRATE = oFIXEDDEPOSIT.RATEOFINTEREST;
                    oFDRNote.FDRNUMBER = oFIXEDDEPOSIT.DEPOSITNUMBER;
                    oFDRNote.CREATEDBY = Session["UserId"].ToString();
                    
                    //Encashed Date
                    oFDRNote.CREATEDDATE = DateTime.Now;        // DateTime.Today;

                    //date which this Encashed Fixed Deposit will be Opened or Issued
                    oFDRNote.OPENEDDATE = oFIXEDDEPOSIT.OPENINGDATE;

                    oFDRNote.SIGNATORY1 = oFIXEDDEPOSIT.SIGNATORY1;
                    oFDRNote.SIGNATORY2 = oFIXEDDEPOSIT.SIGNATORY2;



                    Session["currentPage"] = "FDR Encashment Note";
                   

                    ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                    ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                    ViewBag.Header = Session["currentPage"];


                    TempData["GridHeader"] = "FDR Encashment Note";

                    return PartialView("ApproveFDRNote",oFDRNote);
                }
            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }


        //while Encsah a fdr register
        [HttpPost]
        public ActionResult AddFDRNote(FDRNOTE oNewFDRNote, DateTime? EncashmentDate)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                    oFIXEDDEPOSIT = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(app => app.REFERENCE == oNewFDRNote.FIXEDDEPOSIT_REFERENCE).FirstOrDefault();


                    //check is Encashment Date is less old Fixed Deposit Matured Date 

                    if (EncashmentDate < oFIXEDDEPOSIT.MATURITYDATE)
                    {
                        return RedirectToAction("Index", "ErrorPage", new { message = "Encashment Date:" + EncashmentDate.Value.ToString("dd-MMM-yy") + "must equal or greater then Encashed Deposit: " + oFIXEDDEPOSIT.DEPOSITNUMBER + " Matured Date :" + oFIXEDDEPOSIT.MATURITYDATE.Value.ToString("dd-MMM-yy") });
                    }
                    else
                    {

                        oFIXEDDEPOSIT.LASTUPDATED = DateTime.Now;
                        //edit Encash it means that the fdr attempt to encash 
                        oFIXEDDEPOSIT.PROPOSEDACTION = ConstantVariable.STATUS_NOTETYPE_ENCASH;
                        oFIXEDDEPOSIT.ENCASHMENTDATE = EncashmentDate;



                        db.Entry(oFIXEDDEPOSIT).State = EntityState.Modified;

                        //This is written because branch and FI is not loading together.
                        FIXEDDEPOSIT oFIXEDDEPOSIT2 = new FIXEDDEPOSIT();
                        oFIXEDDEPOSIT2 = db.FIXEDDEPOSITs.Where(app => app.REFERENCE == oNewFDRNote.FIXEDDEPOSIT_REFERENCE).FirstOrDefault();

                        var r = new Random();
                        FDRNOTE oFDRNote = new FDRNOTE();

                        oFDRNote.REFERENCE = Guid.NewGuid().ToString();
                        oFDRNote.FIXEDDEPOSIT_REFERENCE = oNewFDRNote.FIXEDDEPOSIT_REFERENCE;
                        oFDRNote.FINANCIALINSTITUTION_REFERENCE = oFIXEDDEPOSIT.FINANCIALINSTITUTION.REFERENCE;
                        oFDRNote.BRANCH_REFERENCE = oFIXEDDEPOSIT2.BRANCH_REFERENCE;
                        oFDRNote.NOTEID = Convert.ToString(r.Next(555555) + 1);
                        oFDRNote.CAPLIMIT = oFIXEDDEPOSIT.FINANCIALINSTITUTION.CAPLIMIT;
                        oFDRNote.NOTETYPE = ConstantVariable.STATUS_NOTETYPE_ENCASH; // "Encash";
                        oFDRNote.PROPOSALSUMMARY = (oFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME.Count() > 10 ? oFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME.Substring(0, 10) : oFIXEDDEPOSIT.FINANCIALINSTITUTION.NAME) + "-" + oFIXEDDEPOSIT.CREATEDDATE;
                        oFDRNote.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";
                        oFDRNote.PRINCIPALAMOUNT = oFIXEDDEPOSIT.PRINCIPALAMOUNT + oFIXEDDEPOSIT.NETINTERESTRECEIVABLE;
                            //oFIXEDDEPOSIT.PRESENTPRINCIPALAMOUNT + oFIXEDDEPOSIT.NETINTERESTRECEIVABLE; //block 08-May-17
                        //oFIXEDDEPOSIT.PRINCIPALAMOUNT + oFIXEDDEPOSIT.NETINTERESTRECEIVABLE; //added 20/11/16
                        //;  //block 20/11/16
                        oFDRNote.TENURE = oFIXEDDEPOSIT.TENURE;
                        oFDRNote.TENURETERM = oFIXEDDEPOSIT.TENURETERM;
                        oFDRNote.OFFERRATE = oFIXEDDEPOSIT.RATEOFINTEREST;
                        oFDRNote.PROPOSEDRATE = oNewFDRNote.PROPOSEDRATE;
                        oFDRNote.FDRNUMBER = oFIXEDDEPOSIT.DEPOSITNUMBER;
                        oFDRNote.CREATEDBY = Session["UserId"].ToString();

                        //created date will be Encashed fixed deposit opening date(Issue Date) 
                        //so that FDREncashed report FDREncashedLetter.rdlc opening date will be correct
                        oFDRNote.OPENEDDATE = oFIXEDDEPOSIT.OPENINGDATE;

                        //Encashed Date
                        oFDRNote.CREATEDDATE = DateTime.Now;         //DateTime.Today;


                        oFDRNote.SIGNATORY1 = oFIXEDDEPOSIT.SIGNATORY1;
                        oFDRNote.SIGNATORY2 = oFIXEDDEPOSIT.SIGNATORY2;
                        // oFDRNote.PROPOSEDACTION = "Encash";


                        oCommonFunction.CustomObjectNullValidation<FDRNOTE>(ref oFDRNote);

                        db.FDRNOTEs.Add(oFDRNote);
                        db.SaveChanges();
                    }
                }

                TempData["GridHeader"] = "FDR Encashment Note List";

                return RedirectToAction("FDREncashmentNoteList");
            }
            catch (DbEntityValidationException dbEx)
            {
               
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _var.ErrorMessage = "Field Name :" + validationError.PropertyName + " Message :" + validationError.ErrorMessage + "\n";
                        //Trace.TraceInformation("Property: {0} Error: {1}",
                        //                        validationError.PropertyName,
                        //                        validationError.ErrorMessage);
                    }
                }
                return RedirectToAction("Index", "ErrorPage", new { message=_var.ErrorMessage });
            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }


        public ActionResult FDREncashmentNoteList(string FdrNo, string sortdir, string sort, string sortdefault, int? rows, string filterstring, string lblbreadcum, DateTime? fromdate, string FINANCIALINSTITUTION_REFERENCE, DateTime? toDate, string STATUS, string showOpeningDate, string showMatureDate)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (TempData["GridHeader"] != null)
                {
                    lblbreadcum = TempData["GridHeader"].ToString();
                }               

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<FDRNOTE> gridModels = new GridModel<FDRNOTE>();
                List<FDRNOTE> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ? 15 : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;
            

                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;


                ViewBag.CurrentFilter = filterstring;


                if (STATUS==null)  //string.IsNullOrEmpty(STATUS) null /""
                {
                    models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_ENCASH && note.STATUS == ConstantVariable.STATUS_PENDING).OrderByDescending(t => t.CREATEDDATE).ToList();
                }
                else if (STATUS == "")
                { 
                 //select all
                    models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_ENCASH).OrderByDescending(t => t.CREATEDDATE).ToList();  //OrderBy(sort + " " + sortdir)
               
                }
                else
                {

                    models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_ENCASH && note.STATUS == STATUS).OrderByDescending(t=>t.CREATEDDATE).ToList();   //sort + " " + sortdir
                }

                if (!string.IsNullOrEmpty(FINANCIALINSTITUTION_REFERENCE))
                {
                    TempData["FINANCIALINSTITUTION_REFERENCE"] = FINANCIALINSTITUTION_REFERENCE;
                    models = models.Where(fdrnote => fdrnote.FINANCIALINSTITUTION_REFERENCE == FINANCIALINSTITUTION_REFERENCE).ToList();
                }

                if (fromdate != null) //toDate != null
                {
                    TempData["openingDate"] = fromdate;                   

                    models = models.Where(pi => pi.CREATEDDATE.Value.Date >= fromdate.Value.Date).OrderByDescending(pi => pi.CREATEDDATE).ToList();
                }
                if (toDate != null)
                {
                    TempData["toDate"] = toDate;
                    models = models.Where(pi => pi.CREATEDDATE.Value.Date <= toDate.Value.Date).OrderByDescending(pi => pi.CREATEDDATE).ToList();
                }

                if (FdrNo != null && !string.IsNullOrEmpty(FdrNo))
                {
                    models = models.Where(t => t.FDRNUMBER == FdrNo).ToList();
                }

                TempData["FDRNoteList"] = models;
                gridModels.DataModel = models;
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



                var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "FDR Note" && app.PROPERTY == "STATUS").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app => app.DESCRIPTION).ToList();

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");
                return PartialView("FDREncashmentNoteList", gridModels);

                

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }

        //When Approved Encashment
        public ActionResult ApproveFDRNote(string id)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {

                FDRNOTE ofdrNote = new FDRNOTE();
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    ofdrNote = db.FDRNOTEs.SingleOrDefault(i => i.REFERENCE == id);
                    ofdrNote.APPROVEDBY = Session["UserId"].ToString();
                    ofdrNote.APPROVEDDATE = DateTime.Now;  
                    ofdrNote.PROPOSEDACTION = ConstantVariable.STATUS_NOTETYPE_ENCASH;
                    ofdrNote.STATUS = ConstantVariable.STATUS_APPROVED; 
                    oCommonFunction.CustomObjectNullValidation<FDRNOTE>(ref ofdrNote);


                    db.Entry(ofdrNote).State = EntityState.Modified;                   

                    //Start Encashment
                    FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                    oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == ofdrNote.FIXEDDEPOSIT_REFERENCE);
                    
                    //if this deposit renewed then lasupdated will changed while approved from renewal Not List
                    //Only Encash Deposit lastUpdated from here.
                    if (oFIXEDDEPOSIT.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_ENCASH)
                    {
                        oFIXEDDEPOSIT.LASTUPDATED = DateTime.Now;
                    }

                    oFIXEDDEPOSIT.LASTUPDATEDBY = Session["UserId"].ToString();
                    oFIXEDDEPOSIT.STATUS = ConstantVariable.STATUS_ENCASHED; // "Encashed";                  
                   
                    oCommonFunction.CustomObjectNullValidation<FIXEDDEPOSIT>(ref oFIXEDDEPOSIT);

                    db.Entry(oFIXEDDEPOSIT).State = EntityState.Modified;

                                                      

                    //Added By Nazmul:11-07-2015
                    //Create financial transaction
                     new TransactionEngine().CreateFinancialTransaction("FDR02", oFIXEDDEPOSIT.PRINCIPALAMOUNT, oFIXEDDEPOSIT.ENCASHMENTDATE);

                    //END Encashment

                  


                    db.SaveChanges();

                }
                return RedirectToAction("FDREncashmentNoteList");
            }
            catch (Exception ex)
            {
                string message = ex.Message + Convert.ToString(ex.InnerException);

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
        /// <summary>
        /// Generate Encashment Note List reports
        /// </summary>
      

        [HttpPost]
        public ActionResult GenerateEncashmentNoteList(string reference, string appStatus = null, string penStatus = null, string GivenDate=null)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                List<FDRNOTE> fdrnoteList = new List<FDRNOTE>();
                ReportDataSource oFDRNotes = new ReportDataSource();

                //fdrnoteList = TempData["FDRNoteList"] as List<FDRNOTE>;

                string fiRef = TempData["FINANCIALINSTITUTION_REFERENCE"] as string;
                             

                string reportTitle = "Approved Encashment Note"; ;
                string reportName = "ApprovedEncashmentNote.pdf";
                string reportStatus = "Approved";
              

                if (!string.IsNullOrEmpty(penStatus))
                {
                    reportStatus = "Pending";
                    reportTitle = "Pending Encashment Note";
                    reportName = "PendingEncashmentNote.pdf";
                }

               using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                fdrnoteList = db.FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE ==ConstantVariable.STATUS_NOTETYPE_ENCASH && note.STATUS == reportStatus).OrderBy(pi => pi.CREATEDDATE).ToList();
               

                if (!string.IsNullOrEmpty(fiRef))                {

                    fdrnoteList = fdrnoteList.Where(fdrnote => fdrnote.FINANCIALINSTITUTION_REFERENCE == fiRef).ToList();
                }
                else if (reference != null && !string.IsNullOrEmpty(reference))
                {
                    fdrnoteList = fdrnoteList.Where(fdrnote => fdrnote.REFERENCE == reference).ToList();
                }

                if (TempData["openingDate"] != null && TempData["toDate"] != null)
                {
                    DateTime openingDate = DateTime.Parse(TempData["openingDate"].ToString());
                    DateTime toDate = DateTime.Parse(TempData["toDate"].ToString());
                    if (openingDate != null && toDate != null)
                    {
                        fdrnoteList = fdrnoteList.Where(pi => pi.CREATEDDATE >= openingDate && pi.CREATEDDATE <= toDate).OrderBy(pi => pi.CREATEDDATE).ToList();
                    }
                }


                var newfdrNoteList = from fdrNote in fdrnoteList
                                     select new
                                     {
                                         NAME = fdrNote.FINANCIALINSTITUTION.NAME,
                                         BRANCH = fdrNote.FIBRANCH.NAME,
                                         MATURITYDATE = fdrNote.FIXEDDEPOSIT.MATURITYDATE,
                                         TENURETERM = fdrNote.TENURETERM,
                                         PRINCIPALAMOUNT = fdrNote.FIXEDDEPOSIT.PRINCIPALAMOUNT,                                                                                 
                                         EXISTINGDEPOSIT =Math.Round(Convert.ToDecimal(db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.STATUS ==ConstantVariable.STATUS_APPROVED && fixDeposit.FINANCIALINSTITUTION_REFERENCE == fdrNote.FINANCIALINSTITUTION_REFERENCE).Sum(fixDeposit => fixDeposit.PRINCIPALAMOUNT)/10000000),2),
                                         //fdrNote.EXISTINGDEPOSIT,
                                         CAPLIMIT = fdrNote.CAPLIMIT,
                                         OFFERRATE = fdrNote.OFFERRATE,
                                         PROPOSEDRATE = fdrNote.PROPOSEDRATE,
                                         PROPOSEDACTION = fdrNote.PROPOSEDACTION ==null ? "Encash" : fdrNote.PROPOSEDACTION

                                     };

                LocalReport lr = new LocalReport();

                ReportDataSource rd = new ReportDataSource();
                //ReportDataSource dd = new ReportDataSource();

                DataTable dtFDRNote = oCommonFunction.ConvertToDataTable(newfdrNoteList.ToList());
                rd.Name = "FDRNote";
                rd.Value = dtFDRNote;


                ReportParameter[] parameters = new ReportParameter[] 
                {
                  new ReportParameter("CompanyName","DLIC"),
                  new ReportParameter("Address","Gulshan-2, Dhaka"),
                  new ReportParameter("ReportTitle",reportTitle),
                  new ReportParameter("Date",GivenDate),
                  new ReportParameter("Status","")
            };


                lr.ReportPath = Server.MapPath("~/Reports/FDREncashmentNoteList.rdlc");
                lr.SetParameters(parameters);
                lr.DataSources.Add(rd);
                //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);


                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =

                "<DeviceInfo>" +
                    "  <OutputFormat>PDF</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>1in</MarginLeft>" +
                    "  <MarginRight>1in</MarginRight>" +
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

                return File(renderedBytes, mimeType, reportName);
            } //db using close
          }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }

        
        [HttpPost]
        public ActionResult GenerateEncashmentLetter(string reference, string appStatus = null, string penStatus = null, string GivenDate=null)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            try
            {
                

                FDRNOTE oFDRNote = new FDRNOTE();
                ReportDataSource oFDRNotes = new ReportDataSource();

                string reportTitle = "FDR Encashment Letter"; ;

                oFDRNote = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(p => p.REFERENCE == reference).FirstOrDefault();
                string reportName = oFDRNote.FINANCIALINSTITUTION.NAME + "-Encashment-" + oFDRNote.FIXEDDEPOSIT.DEPOSITNUMBER + ".pdf";

                List<FDRNOTE> fdrnoteList = new List<FDRNOTE>();

                LocalReport lr = new LocalReport();

                ReportDataSource rd = new ReportDataSource();


                DataTable dtFDRNote = oCommonFunction.ConvertToDataTable(fdrnoteList.ToList());
                rd.Name = "EncashmentLetter";
                rd.Value = dtFDRNote;     

                try
                {
                              
                    
            string PrincipalAmount = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(oFDRNote.FIXEDDEPOSIT.PRINCIPALAMOUNT.Value)).Replace("$", string.Empty); 

        
             ReportParameter[] parameters = new ReportParameter[] 
               {
             
              new ReportParameter("FIName",oFDRNote.FINANCIALINSTITUTION.NAME.ToUpper()),
              new ReportParameter("FIAddress",oFDRNote.FIBRANCH.NAME+" Branch\n"+ oFDRNote.FIBRANCH.ADDRESSLINE1 +Environment.NewLine+oFDRNote.FIBRANCH.ADDRESSLINE2),             

              new ReportParameter("FDRNumber",oFDRNote.FDRNUMBER),
              new ReportParameter("OpeningDate",oFDRNote.OPENEDDATE.Value.ToString("dd-MMM-yyyy")),
              new ReportParameter("Tenure",oFDRNote.TENURE.ToString()),
              new ReportParameter("TenureTerm",oFDRNote.TENURETERM.ToLower()),
             
              new ReportParameter("PrincipleAmount",PrincipalAmount),  
    
              new ReportParameter("MaturityDate",  oFDRNote.FIXEDDEPOSIT.MATURITYDATE.Value.ToString("dd-MMM-yyyy")),

              new ReportParameter("SIGNATORY1",oFDRNote.SIGNATORY1),
              new ReportParameter("SIGNATORY2",oFDRNote.SIGNATORY2),
              new ReportParameter("Date",GivenDate)
              
            };


                    lr.ReportPath = Server.MapPath("~/Reports/FDREncashmentLetter.rdlc");
                    lr.DataSources.Add(rd);
                    lr.SetParameters(parameters);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message + " " + ex.InnerException.Message + " " + ex.InnerException.InnerException.Message + " " + ex.GetType();
                    return RedirectToAction("Index", "ErrorPage", new { message = msg });
                }
                //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);


                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =

                "<DeviceInfo>" +
                    "  <OutputFormat>PDF</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>1in</MarginLeft>" +
                    "  <MarginRight>1in</MarginRight>" +
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

                return File(renderedBytes, mimeType, reportName);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "ErrorPage", new { message=ex.Message+ " "+ ex.InnerException.Message +" "+ex.InnerException.InnerException.Message });
            }
        }


        public ActionResult FDRNoteReconciliationList(string FdrNo, string sortdir, string sort, string sortdefault, int? rows, string filterstring, string lblbreadcum, DateTime? openingDate, string FINANCIALINSTITUTION_REFERENCE, DateTime? toDate, string STATUS, string showOpeningDate, string showMatureDate)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (TempData["GridHeader"] != null)
                {
                    lblbreadcum = TempData["GridHeader"].ToString();
                }

               
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<FDRNOTE> gridModels = new GridModel<FDRNOTE>();
                List<FDRNOTE> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ? 15 : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;

                //}


                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;


                ViewBag.CurrentFilter = filterstring;
                models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_ENCASH && note.STATUS == ConstantVariable.STATUS_APPROVED).OrderByDescending(t=>t.CREATEDDATE).ToList();  //.OrderBy(sort + " " + sortdir)
               
                if (!string.IsNullOrEmpty(FINANCIALINSTITUTION_REFERENCE))
                {
                    TempData["FINANCIALINSTITUTION_REFERENCE"] = FINANCIALINSTITUTION_REFERENCE;
                    models = models.Where(fdrnote => fdrnote.FINANCIALINSTITUTION_REFERENCE == FINANCIALINSTITUTION_REFERENCE).ToList();
                }

                if (openingDate != null && toDate != null)
                {
                    TempData["openingDate"] = openingDate;
                    TempData["toDate"] = toDate;

                    models = models.Where(pi => pi.CREATEDDATE.Value.Date >= openingDate.Value.Date && pi.CREATEDDATE.Value.Date <= toDate.Value.Date).OrderBy(pi => pi.CREATEDDATE).ToList();
                }
                else
                {
                    ViewBag.Nullvalue = null;
                
                }


                if (FdrNo != null && !string.IsNullOrEmpty(FdrNo))
                {
                    models = models.Where(t => t.FDRNUMBER == FdrNo).ToList();
                }


                TempData["FDRNoteList"] = models;
                gridModels.DataModel = models;
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



                var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "FDR Note" && app.PROPERTY == "STATUS").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app => app.DESCRIPTION).ToList();

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");
                return PartialView("FDRNoteReconciliationList", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }

     
        /// <summary>
        /// FDR Reconcilation
        /// padded FDR PK as id then take the FDR.Only Encashed FDR is Reconcilled hear.
        /// </summary>
        
        [HttpGet]
        public ActionResult EncashFDRNote(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();

                FDRNOTE oFDRNOTE = new FDRNOTE();
                oFDRNOTE = db.FDRNOTEs.SingleOrDefault(i => i.REFERENCE == id);

                ViewBag.Reconcilled = "";
                if(oFDRNOTE.STATUS==ConstantVariable.STATUS_APPROVED && oFDRNOTE.INTERESTMODE !=null)
                 ViewBag.Reconcilled ="True";

                FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == oFDRNOTE.FIXEDDEPOSIT.REFERENCE);

                //according burhan vai request point 9 mailed May 16, 2017 at 5:48 PM 9.MR date will be encahment date.
                ViewBag.Mrdate = oFIXEDDEPOSIT.ENCASHMENTDATE.Value != null ? oFIXEDDEPOSIT.ENCASHMENTDATE.Value.ToString("dd-MMM-yy") : null;
                    //oFIXEDDEPOSIT.MRDATE.Value !=null? oFIXEDDEPOSIT.MRDATE.Value.ToString("dd-MMM-yy"):null;
                ViewBag.EncashDate = oFIXEDDEPOSIT.ENCASHMENTDATE.Value != null ? oFIXEDDEPOSIT.ENCASHMENTDATE.Value.ToString("dd-MMM-yy") : null;

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Encash " + Session["currentPage"];

                

                TempData["FDR"] = oFIXEDDEPOSIT;


                return PartialView("EncashFixedDeposit", oFIXEDDEPOSIT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpGet]
        public ActionResult GetInterestSchedule(string FDRNumber, string lblbreadcum)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                GridModel<FDRENCASHMENT> gridModels = new GridModel<FDRENCASHMENT>();
                List<FDRENCASHMENT> models = null;

                //grid settings                
                gridModels.RowsPerPage =  15;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;


                if (FDRNumber != null)
                    models = new Entities(Session["Connection"] as EntityConnection).FDRENCASHMENTs.AsNoTracking().Where(fdr => fdr.FDRNUMBER == FDRNumber).OrderByDescending(encash => encash.RECEIVEDON).ToList();
                else
                    models = new List<FDRENCASHMENT>();


                TempData["FDRENCASHMENT"] = models;
                gridModels.DataModel = models;
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

                return PartialView("ListFDREncashment", gridModels);

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpGet]
        public ActionResult EditFDRENCASHMENT(string reference, string lblbreadcum)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    ViewModelBase oViewModelBase = new ViewModelBase();

                    FDRENCASHMENT oFDRENCASHMENT = db.FDRENCASHMENTs.SingleOrDefault(i => i.REFERENCE == reference);

                    if (!string.IsNullOrEmpty(lblbreadcum))
                    {
                        Session["currentPage"] = lblbreadcum;
                    }

                    ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                    ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                    ViewBag.Header = Session["currentPage"];



                    return PartialView("EditFDRENCASHMENT", oFDRENCASHMENT);
                }

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult EditFDRENCASHMENT(FDRENCASHMENT oFDRENCASHMENT)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    FDRENCASHMENT OldFDRENCASHMENT = db.FDRENCASHMENTs.SingleOrDefault(i => i.REFERENCE == oFDRENCASHMENT.REFERENCE);

                    OldFDRENCASHMENT.GROSSINTEREST = oFDRENCASHMENT.GROSSINTEREST;
                    OldFDRENCASHMENT.SOURCETAX = oFDRENCASHMENT.SOURCETAX;
                    OldFDRENCASHMENT.EXCISEDUTY = oFDRENCASHMENT.EXCISEDUTY;
                    OldFDRENCASHMENT.OTHERCHARGE = oFDRENCASHMENT.OTHERCHARGE;
                    OldFDRENCASHMENT.NETINTEREST = oFDRENCASHMENT.NETINTEREST;


                    db.Entry(OldFDRENCASHMENT).State = EntityState.Modified;

                    db.SaveChanges();

                    return RedirectToAction("GetInterestSchedule", new { FDRNumber = OldFDRENCASHMENT.FDRNUMBER, lblbreadcum = "FDR Encashment Schedule" });
                }

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        #region RenewReconcile
        /// <summary>
        /// Renewal Reconcile only done when FDRNOte notetype=Renewal and Status=Pending and FixedDeposit_Ref= Parent FixedDeposit Reference
        /// because after Approved a new FixedDeposit is Inserted with parent FDR TotalAmountRecivable as Principal and status pending 
        /// also this FDR changed after Approved by FixedDeposit_Ref= newly created FixedDeposit Reference,Parent FixedDeposit Reference will removed
        /// so you can Reconcile before Approved 
        /// </summary>
       
        [HttpGet]
        public ActionResult RenewReconcile(string fdrnote_ref)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();

                FDRNOTE oFDRNOTE = new FDRNOTE();
                oFDRNOTE = db.FDRNOTEs.SingleOrDefault(i => i.REFERENCE==fdrnote_ref && i.NOTETYPE==ConstantVariable.STATUS_NOTETYPE_RENEWAL);

                if (oFDRNOTE != null)
                {

                    if (oFDRNOTE.STATUS == ConstantVariable.STATUS_PENDING)
                    {
                        FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();
                        oFIXEDDEPOSIT = db.FIXEDDEPOSITs.SingleOrDefault(i => i.REFERENCE == oFDRNOTE.FIXEDDEPOSIT_REFERENCE);

                        ViewBag.Mrdate = oFIXEDDEPOSIT.MRDATE.Value != null ? oFIXEDDEPOSIT.MRDATE.Value.ToString("dd-MMM-yy") : null;
                        //ViewBag.EncashDate = oFIXEDDEPOSIT.ENCASHMENTDATE.Value != null ? oFIXEDDEPOSIT.ENCASHMENTDATE.Value.ToString("dd-MMM-yy") : null;

                        ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                        ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                        ViewBag.Header = "Reconcile " + Session["currentPage"];

                        ViewBag.RenewalNoteType=oFDRNOTE;
                        return PartialView("RenewReconcile", oFIXEDDEPOSIT);
                    }
                    else
                    {
                        return RedirectToAction("Index", "ErrorPage", new { message = "This Renewal Note FDR may alraedy Approved." });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "ErrorPage", new { message="Renewal Note FDR does not Exists!!" });
                }
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        /// <summary>
        /// aFixedDeposit is parent Fixed Deposit that is Renewed and now reconcile hear
        /// </summary>
      
        [HttpPost]
        public ActionResult RenewReconcile(FIXEDDEPOSIT aFIXEDDEPOSIT)
        {


            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    //now check FDR_NOTE if any record found of NoteType is Encashed Status Pending/Approved
                    //if found that means Its Renewed Without Interest If not that means its Renewed WithInterest

                    FDRNOTE NoteTypeEncash = db.FDRNOTEs.SingleOrDefault(t=>t.FIXEDDEPOSIT_REFERENCE==aFIXEDDEPOSIT.REFERENCE && t.NOTETYPE==ConstantVariable.STATUS_NOTETYPE_ENCASH);

                    FDRNOTE NoteTypeRenewal =TempData["FDRrenewalNoteType"] as FDRNOTE;

                    FIXEDDEPOSIT parentDeposit = db.FIXEDDEPOSITs.Where(t => t.REFERENCE == aFIXEDDEPOSIT.REFERENCE).SingleOrDefault();
                  

                    if (NoteTypeRenewal != null)
                    {                        

                        ////update parent Fixed Deposit
                        //the actual Interest get from bank.next fdr principal will be this actualInterest+Principal(for renew with interest) 
                        parentDeposit.ACTUALINTERESTRECEIVED = aFIXEDDEPOSIT.ACTUALINTERESTRECEIVED;

                        parentDeposit.GROSSINTEREST = aFIXEDDEPOSIT.GROSSINTEREST;
                        parentDeposit.SOURCETAX = aFIXEDDEPOSIT.SOURCETAX;
                        parentDeposit.EXCISEDUTY = aFIXEDDEPOSIT.EXCISEDUTY;
                        parentDeposit.OTHERCHARGE = aFIXEDDEPOSIT.OTHERCHARGE;
                        parentDeposit.NETINTERESTRECEIVABLE = aFIXEDDEPOSIT.NETINTERESTRECEIVABLE;
                        parentDeposit.TOTALAMOUNTRECEIVABLE = aFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE;
                        parentDeposit.PRESENTPRINCIPALAMOUNT = aFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE; //Renew With Interest

                        db.Entry(parentDeposit).State = EntityState.Modified;

                        /* with interest one FDR NoteType=Renewal also change parent deposit gross,
                           source,net,Principal Receivable and new deposit Principal amount
                         */

                        if (NoteTypeEncash == null) 
                        {
                            //update Renewal note with Principal+Interest though it renewed with interest
                            FDRNOTE renewalNote = db.FDRNOTEs.Where(t => t.REFERENCE == NoteTypeRenewal.REFERENCE).SingleOrDefault();
                            renewalNote.PRINCIPALAMOUNT = aFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE;                            
                            db.Entry(renewalNote).State = EntityState.Modified;
                       
                            //also update newly created FixedDeposit Principal Amount  
                            FIXEDDEPOSIT newlyFixedDeposit = db.FIXEDDEPOSITs.Where(t => t.RENEWALDEPOSITNUMBER == aFIXEDDEPOSIT.DEPOSITNUMBER && t.STATUS == null && t.DEPOSITNUMBER == null).SingleOrDefault();
                            newlyFixedDeposit.PRINCIPALAMOUNT = aFIXEDDEPOSIT.TOTALAMOUNTRECEIVABLE;
                            db.Entry(newlyFixedDeposit).State = EntityState.Modified;

                        }

                        /* 
                        without Interest has two fdr. one: NoteType=Renewal, secound: NoteType =  Encash.
                        NoteType=Renewal FDR the Principal amount remain same. But NoteType= Encash 
                         FDR Principal amount replaced Changed with new Interest.If ActualInterestReceivable>0 and != NetInterestReceivable
                         *then   NoteTypeEncash.PRINCIPALAMOUNT =ActualInterestReceivable(Interest that given by bank)
                        */
                        else   
                        {
                            if (aFIXEDDEPOSIT.ACTUALINTERESTRECEIVED > 0 && aFIXEDDEPOSIT.ACTUALINTERESTRECEIVED != aFIXEDDEPOSIT.NETINTERESTRECEIVABLE)
                            {
                                NoteTypeEncash.PRINCIPALAMOUNT = aFIXEDDEPOSIT.ACTUALINTERESTRECEIVED;
                            }
                            else
                            {
                                NoteTypeEncash.PRINCIPALAMOUNT = aFIXEDDEPOSIT.NETINTERESTRECEIVABLE;
                            }
                            db.Entry(NoteTypeEncash).State = EntityState.Modified;

                        }
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return RedirectToAction("Index", "ErrorPage", new { message = ex.Message });
                        }

                    }
                    else
                    {
                        return RedirectToAction("Index", "ErrorPage", new { message="Renewal Note has null value !!" });
                    }                   
                                       
                }

                return RedirectToAction("ListFDRRenewalNote", "FDRNote");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        #endregion



        #region FDREncashMentRenewalNoteLetter


        //This report as same copy of FDR NOTE / Renewal Note Letter
        //here this report is added untill FixedDeposit status =Approved

        [HttpPost]
        public ActionResult PrintRenewalNoteLetter(string FixedDepositRef, string providedate) //Remember In FDR Note its FDR Note Reference
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                List<FDRNOTE> fdrnoteList = new List<FDRNOTE>();
                ReportDataSource oFDRNotes = new ReportDataSource();

               
                //while pending we can get FixedDepost by fdrnote FixedDepostREf
                var FDlist =db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.REFERENCE == FixedDepositRef).ToList();

                var List = (from t in FDlist
                            select new
                            {
                                Name = t.FINANCIALINSTITUTION.NAME,
                                BRANCH = t.FIBRANCH.NAME + " Branch\n" + t.FIBRANCH.ADDRESSLINE1 + "\n" + t.FIBRANCH.ADDRESSLINE2,
                                FDRNUMBER = t.DEPOSITNUMBER,
                                MATURITYDATE = t.MATURITYDATE,
                                TENURE = t.TENURE,
                                TENURETERM = t.TENURETERM,
                                PRINCIPALAMOUNT = t.PRINCIPALAMOUNT,
                                EXISTINGDEPOSIT = 0,
                                PERCENTAGEOFFDR = 0,
                                CAPLIMIT = 0,
                                OFFERRATE = 0,
                                PROPOSEDRATE = t.RATEOFINTEREST,
                                CONTACTPERSON = "",                             
                                CHEQUENO = t.CHEQUEREFERENCE,
                                CHEQUEDATE = t.OPENINGDATE,
                               

                                STATUS = t.INTERESTMODE == ConstantVariable.INTERESTMODE_COMPOUND ? " " + t.COMPOUNDINTERESTINTERVAL.ToLower() + " compounding" : ""
           
                            }).ToList();
                  

                //var fdrnumber = List.SingleOrDefault().FDRNUMBER;

                //string FdrNumber1 = !String.IsNullOrWhiteSpace(fdrnumber) && fdrnumber.Length >= 18 ? fdrnumber.Substring(0, 18) : fdrnumber;
                //string FdrNumber2 = !String.IsNullOrWhiteSpace(fdrnumber) && fdrnumber.Length > 18 ? fdrnumber.Substring(18) : "";

              
                LocalReport lr = new LocalReport();

                ReportDataSource rd = new ReportDataSource();
                //ReportDataSource dd = new ReportDataSource();

                DataTable dtFDRNote = oCommonFunction.ConvertToDataTable(List.ToList());
                rd.Name = "PurchaseNoteList";
                rd.Value = dtFDRNote;

                ReportParameter[] parameters = new ReportParameter[] 
                {
                 new ReportParameter("date",providedate),
                
                };


                lr.ReportPath = Server.MapPath("~/Reports/FDRRenewalNoteLetter.rdlc");
                lr.SetParameters(parameters);
                lr.DataSources.Add(rd);
                //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);


                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =

                "<DeviceInfo>" +
                    "  <OutputFormat>PDF</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>1in</MarginLeft>" +
                    "  <MarginRight>1in</MarginRight>" +
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

                //Response.AddHeader("content-disposition",
                //"attachment; filename=WeeklyAisleReport-" + DateTime.Now + "." +
                //fileNameExtension);        

                renderedBytes = lr.Render(reportType);

                string reportName =List[0].Name + "-RenewalNote.pdf";

                return File(renderedBytes, mimeType, reportName);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }



        [HttpPost]
        public ActionResult PrintRenewalNoteList(string FixedDepositRef, string appStatus = null, string penStatus = null, string GivenDate = null,string ProposedRate=null,string ProposedAction=null)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            try
            {
            Entities db = new Entities(Session["Connection"] as EntityConnection);
            List<SIGNATORYDETAIL> SignatoryDetails = new List<SIGNATORYDETAIL>();
            
            DateTime date = Convert.ToDateTime(GivenDate);

            SignatoryDetails = db.SIGNATORies
                         .Where(p => p.SIGNATORYDETAILS.Any(c => c.Status == "Active" && c.FromDate <= date && c.ToDate >= date) && p.Status == "Active" && p.Code == "FDRRenewalNote")                                               
                         .SelectMany(t => t.SIGNATORYDETAILS).ToList();


            SignatoryDetails = SignatoryDetails.Where(t=>t.ToDate>= date && t.FromDate.Value <= date && t.Status=="Active").OrderBy(t => t.ORDERINDEX).ToList();

            ReportDataSource oFDRNotes = new ReportDataSource();

            string fiRef = TempData["FINANCIALINSTITUTION_REFERENCE"] as string;

            string reportTitle ="Renewal Note"; ;
            string reportName = "RenewalNoteList.pdf";
            string reportStatus ="Approved";
           
            var FDlist = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.REFERENCE == FixedDepositRef).ToList();

            var newfdrNoteList = from fdrNote in FDlist
                                 select new
                                 {

                                     NAME = fdrNote.FINANCIALINSTITUTION.NAME,
                                     BRANCH = fdrNote.FIBRANCH.NAME,
                                     TENURE = Convert.ToString(fdrNote.TENURE.Value),
                                     TENURETERM = fdrNote.TENURETERM,
                                     PRINCIPALAMOUNT = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(fdrNote.PRINCIPALAMOUNT.Value)).Replace("$", string.Empty),

                                     CAPLIMIT = Convert.ToString(fdrNote.FINANCIALINSTITUTION.CAPLIMIT),  //Convert.ToString(fdrNote.EXISTINGCAPLIMIT.Value)
                                     PRVOFFERRATE = Convert.ToString(fdrNote.RATEOFINTEREST.Value),

                                     PROPOSEDRATE = ProposedRate ,
                                     //Convert.ToString(fdrNote.RATEOFINTEREST.Value) + "%\n" + (fdrNote.INTERESTMODE == ConstantVariable.INTERESTMODE_COMPOUND ? "(" + fdrNote.COMPOUNDINTERESTINTERVAL + " com)" : ""),

                                     MATURITYDATE = fdrNote.MATURITYDATE.Value.ToString("dd-MMM-yy"),
                                     maturedMonthYear = fdrNote.MATURITYDATE.Value.ToString("MMMM ,yyyy"),

                                     PROPOSEDACTION = ProposedAction
                                     //"Renew for \n" + fdrNote.TENURE + " " + fdrNote.TENURETERM



                                 };

           var signatory = (from s in SignatoryDetails select new{
                           Title=s.TITLE,
                           Name= s.SignatureLine1 +"\n"+s.SignatureLine2
                          }).ToList();
            
            // string matureddateinword = newfdrNoteList.ToList().First(0).MATURITYDATE.ToString();
            LocalReport lr = new LocalReport();

            ReportDataSource rd = new ReportDataSource();
            ReportDataSource dd = new ReportDataSource();

            DataTable dtFDRNote = oCommonFunction.ConvertToDataTable(newfdrNoteList.ToList());
            rd.Name = "RenewNoteList";   // "PurchaseNoteList";
            rd.Value = dtFDRNote;

            dd.Name = "SignatoryList";
            dd.Value = oCommonFunction.ConvertToDataTable(signatory.ToList());

            var Title = db.SIGNATORies.Where(t => t.Code == "FDRRenewalNote").SingleOrDefault().TITLE;
            ReportParameter[] parameters = new ReportParameter[] 
                  {
                     new ReportParameter("Todays",GivenDate),
                     new ReportParameter("Title",Title),
                     new ReportParameter("Status","...") //Pending=... this mean this note is taken before renew for decision 
                   };


            lr.ReportPath = Server.MapPath("~/Reports/FDRRenewalNote.rdlc");
            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);
            lr.DataSources.Add(dd);
            //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>1in</MarginLeft>" +
                "  <MarginRight>1in</MarginRight>" +
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
            return File(renderedBytes, mimeType, reportName);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult GenerateEncashmentLetterCopy(string FixedDepositRef, string appStatus = null, string penStatus = null, string GivenDate = null)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            try
            {
             
                var  oFixedDepositNote = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(p => p.REFERENCE == FixedDepositRef).FirstOrDefault();
                string reportName = oFixedDepositNote.FINANCIALINSTITUTION.NAME + "-Encashment-" + oFixedDepositNote.DEPOSITNUMBER + ".pdf";

                List<FDRNOTE> fdrnoteList = new List<FDRNOTE>();

                LocalReport lr = new LocalReport();

                ReportDataSource rd = new ReportDataSource();


                DataTable dtFDRNote = oCommonFunction.ConvertToDataTable(fdrnoteList.ToList());
                rd.Name = "EncashmentLetter";
                rd.Value = dtFDRNote;

                try
                {
               string PrincipalAmount = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(oFixedDepositNote.PRINCIPALAMOUNT.Value)).Replace("$", string.Empty);

               ReportParameter[] parameters = new ReportParameter[] 
               {
             
              new ReportParameter("FIName",oFixedDepositNote.FINANCIALINSTITUTION.NAME.ToUpper()),
              new ReportParameter("FIAddress",oFixedDepositNote.FIBRANCH.NAME+" Branch\n"+ oFixedDepositNote.FIBRANCH.ADDRESSLINE1 +Environment.NewLine+oFixedDepositNote.FIBRANCH.ADDRESSLINE2),             

              new ReportParameter("FDRNumber",oFixedDepositNote.DEPOSITNUMBER),
              new ReportParameter("OpeningDate",oFixedDepositNote.OPENINGDATE.Value.ToString("dd-MMM-yyyy")),
              new ReportParameter("Tenure",oFixedDepositNote.TENURE.ToString()),
              new ReportParameter("TenureTerm",oFixedDepositNote.TENURETERM.ToLower()),
             
              new ReportParameter("PrincipleAmount",PrincipalAmount),  
    
              new ReportParameter("MaturityDate",  oFixedDepositNote.MATURITYDATE.Value.ToString("dd-MMM-yyyy")),

              new ReportParameter("SIGNATORY1",oFixedDepositNote.SIGNATORY1),
              new ReportParameter("SIGNATORY2",oFixedDepositNote.SIGNATORY2),
              new ReportParameter("Date",GivenDate)
              
            };


                    lr.ReportPath = Server.MapPath("~/Reports/FDREncashmentLetter.rdlc");
                    lr.DataSources.Add(rd);
                    lr.SetParameters(parameters);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message + " " + ex.InnerException.Message + " " + ex.InnerException.InnerException.Message + " " + ex.GetType();
                    return RedirectToAction("Index", "ErrorPage", new { message = msg });
                }
                //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);


                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =

                "<DeviceInfo>" +
                    "  <OutputFormat>PDF</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>1in</MarginLeft>" +
                    "  <MarginRight>1in</MarginRight>" +
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

                return File(renderedBytes, mimeType, reportName);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "ErrorPage", new { message = ex.Message + " " + ex.InnerException.Message + " " + ex.InnerException.InnerException.Message });
            }
        }

        [HttpPost]
        public ActionResult GenerateEncashmentNoteListCopy(string FixedDepositRef, string appStatus = null, string penStatus = null, string GivenDate = null, string ProposedRate = null, string ProposedAction = null)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
               
                ReportDataSource oFDRNotes = new ReportDataSource();

                //fdrnoteList = TempData["FDRNoteList"] as List<FDRNOTE>;

                string fiRef = TempData["FINANCIALINSTITUTION_REFERENCE"] as string;


                string reportTitle ="Encashment Note"; ;
                string reportName = "EncashmentNote.pdf";
                string reportStatus = "Approved";


                if (!string.IsNullOrEmpty(penStatus))
                {
                    reportStatus = "Pending";
                    reportTitle = "Pending Encashment Note";
                    reportName = "PendingEncashmentNote.pdf";
                }

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                  var  fdrnoteList = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t=>t.REFERENCE == FixedDepositRef).OrderBy(pi => pi.CREATEDDATE).ToList();


                    var newfdrNoteList = from fdrNote in fdrnoteList
                                         select new
                                         {
                                             NAME = fdrNote.FINANCIALINSTITUTION.NAME,
                                             BRANCH = fdrNote.FIBRANCH.NAME,
                                             MATURITYDATE = fdrNote.MATURITYDATE,
                                             TENURETERM = fdrNote.TENURETERM,
                                             PRINCIPALAMOUNT = fdrNote.PRINCIPALAMOUNT,
                                             EXISTINGDEPOSIT = Math.Round(Convert.ToDecimal(db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.STATUS == ConstantVariable.STATUS_APPROVED && fixDeposit.FINANCIALINSTITUTION_REFERENCE == fdrNote.FINANCIALINSTITUTION_REFERENCE).Sum(fixDeposit => fixDeposit.PRINCIPALAMOUNT) / 10000000), 2),
                                             //fdrNote.EXISTINGDEPOSIT,
                                             CAPLIMIT = fdrNote.FINANCIALINSTITUTION.CAPLIMIT,            //fdrNote.EXISTINGCAPLIMIT,
                                             OFFERRATE = fdrNote.RATEOFINTEREST,
                                             PROPOSEDRATE = ProposedRate,  //fdrNote.RATEOFINTEREST,
                                             PROPOSEDACTION = ProposedAction != null ? ProposedAction :"Encash"  //fdrNote.PROPOSEDACTION

                                         };

                    LocalReport lr = new LocalReport();

                    ReportDataSource rd = new ReportDataSource();
                    //ReportDataSource dd = new ReportDataSource();

                    DataTable dtFDRNote = oCommonFunction.ConvertToDataTable(newfdrNoteList.ToList());
                    rd.Name = "FDRNote";
                    rd.Value = dtFDRNote;


                  ReportParameter[] parameters = new ReportParameter[] 
                   {
                     new ReportParameter("CompanyName","DLIC"),
                     new ReportParameter("Address","Gulshan-2, Dhaka"),
                     new ReportParameter("ReportTitle",reportTitle),
                     new ReportParameter("Date",GivenDate),
                     new ReportParameter("Status","...") //Pending=... this mean this note is taken before Encash for decision 
                  };


                    lr.ReportPath = Server.MapPath("~/Reports/FDREncashmentNoteList.rdlc");
                    lr.SetParameters(parameters);
                    lr.DataSources.Add(rd);
                    //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);


                    string reportType = "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                        "  <PageWidth>8.5in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0.5in</MarginTop>" +
                        "  <MarginLeft>1in</MarginLeft>" +
                        "  <MarginRight>1in</MarginRight>" +
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

                    return File(renderedBytes, mimeType, reportName);
                } //db using close
            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }

        #endregion


    }
}


