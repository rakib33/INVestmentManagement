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
using Microsoft.Reporting.WebForms;
//for variable
using InvestmentManagement.App_Code;
using System.Globalization;

namespace InvestmentManagement.Controllers
{
    public class FDRNoteController : Controller
    {
        Variable _var = new Variable();
        // GET: /FDRNote/
        CommonFunction oCommonFunction = new CommonFunction();

        public ActionResult ListFDRNote(string sortdir, string sort, int? page, int? rows, string lblbreadcum, string PagingType, int? currentRowPerPage = 15, string FINANCIALINSTITUTION_REFERENCE = null, string STATUS = null, DateTime? issueFrom = null, DateTime? issueTo = null)
        {
            //return if session closed
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            GridModel<FDRNOTE> gridModels = new GridModel<FDRNOTE>();
            List<FDRNOTE> models = null;

            if (TempData["GridHeader"] != null)
            {
                lblbreadcum = TempData["GridHeader"].ToString();
            }
            //grid settings                
            gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
            ViewBag.currentRowPerPage = gridModels.RowsPerPage;


            //loading configuration
            sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
            sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;

            try
            {

                //Filter
                if (STATUS == null)
                    models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").Where(note => note.STATUS == "Pending" && note.NOTETYPE == "New").OrderByDescending(t => t.CREATEDDATE).ToList();  //.OrderBy(sort + " " + sortdir).ToList(); .AsNoTracking()
                else if (STATUS == "")
                    models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").Where(note => note.NOTETYPE == "New").OrderByDescending(t => t.CREATEDDATE).ToList();   //.OrderBy(sort + " " + sortdir).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").Where(note => note.STATUS == STATUS && note.NOTETYPE == "New").OrderByDescending(t => t.CREATEDDATE).ToList();



                #region MyModifiedCode blocked Tody 4/21/2016
                //models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note=>note.NOTETYPE !=ConstantVariable.STATUS_NOTETYPE_ENCASH).OrderBy(sort + " " + sortdir).ToList();
                // //note=>note.STATUS != ConstantVariable.STATUS_RECONCILLED                 

                ////Filter
                //if (STATUS ==ConstantVariable.STATUS_PENDING)
                //    models =models.ToList().Where(note => note.STATUS == STATUS).ToList();

                //else if (STATUS == ConstantVariable.STATUS_APPROVED)
                //    models = models.ToList().Where(note => note.STATUS == STATUS).ToList();
                //models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE == "New").OrderBy(sort + " " + sortdir).ToList();
                //else
                //    models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.STATUS == STATUS && note.NOTETYPE == "New").OrderBy(sort + " " + sortdir).ToList();
                #endregion



                if (issueFrom != null && issueTo != null)
                {
                    //if (issueFrom == issueTo)
                    //{
                    //    models = models.Where(fdrnote => fdrnote.CREATEDDATE.Value.Date == issueFrom.Value.Date).OrderByDescending(t => t.CREATEDDATE).ToList();
                    //}
                    //else
                    //{
                        models = models.Where(fdrnote => fdrnote.CREATEDDATE.Value.Date >= issueFrom.Value.Date && fdrnote.CREATEDDATE.Value.Date <= issueTo.Value.Date).OrderByDescending(t => t.CREATEDDATE).ToList();
                   // }
                }

               else if (issueFrom != null)
                {
                    TempData["openingDate"] = issueFrom;
                    models = models.Where(fdrnote => fdrnote.CREATEDDATE >= issueFrom).OrderByDescending(t => t.CREATEDDATE).ToList();  //.OrderBy(sort + " " + sortdir).ToList();
                }
               else if (issueTo != null)
                {
                    TempData["toDate"] = issueTo;
                    models = models.Where(fdrnote => fdrnote.CREATEDDATE.Value.Date <= issueTo.Value.Date).OrderByDescending(t => t.CREATEDDATE).ToList(); //.OrderBy(sort + " " + sortdir).ToList();
                }
                else { }

                if (!string.IsNullOrEmpty(FINANCIALINSTITUTION_REFERENCE))
                {
                    TempData["FINANCIALINSTITUTION_REFERENCE"] = FINANCIALINSTITUTION_REFERENCE;
                    models = models.Where(fdrnote => fdrnote.FINANCIALINSTITUTION_REFERENCE == FINANCIALINSTITUTION_REFERENCE).ToList();

                }
                var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "FDR Note" && app.PROPERTY == "STATUS").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app => app.DESCRIPTION).ToList();

                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }


                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());


                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");

                gridModels.DataModel = models;
                return PartialView("ListFDRNote", gridModels);
            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }


        /// <summary>
        /// Current Holding: The total FDR Principle Amount that is not matured and not encashed
        /// </summary>

        public ActionResult GetFIDetails(string reference = null)
        {

            //string json;
            Decimal? totalPrincialAmount = 0;
            Decimal? currentDeposit = 0;
            FIInformation oFIInformation = new FIInformation();
            using (Entities db = new Entities(Session["Connection"] as EntityConnection))
            {

                try
                {
                    //throw new Exception(db.Configuration.)
                    var oFI = db.FINANCIALINSTITUTIONs.Where(fi => fi.REFERENCE == reference).Select(f => new { NPL = f.NPLPERCENTAGE, existingCapLimit = f.CAPLIMIT }).FirstOrDefault();

                    try
                    {
                        //Sum of All Principal Amount of FixedDeposit table where Status= Approved
                        totalPrincialAmount = db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.STATUS == ConstantVariable.STATUS_APPROVED).Sum(fixDeposit => fixDeposit.PRINCIPALAMOUNT);
                    }
                    catch { totalPrincialAmount = 0; }
                    try
                    {
                        // currentDeposit or current Holding or Existing Deposit without corer // "Approved" sum of all  Principal Amount of given FI/Bank of FixedDeposit table where Status= Approved
                        currentDeposit = db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.STATUS == ConstantVariable.STATUS_APPROVED && fixDeposit.FINANCIALINSTITUTION_REFERENCE == reference).Sum(fixDeposit => fixDeposit.PRINCIPALAMOUNT);
                    }
                    catch { currentDeposit = 0; }

                    if (currentDeposit == null)
                        currentDeposit = 0;
                    if (totalPrincialAmount == null)
                        totalPrincialAmount = 1;

                    // FIInformation oFIInformation = new FIInformation();
                    oFIInformation.CAPLimit = oFI.existingCapLimit == null ? 0 : oFI.existingCapLimit;
                    oFIInformation.ExitsingDeposit = currentDeposit == null ? 0 : currentDeposit;

                    oFIInformation.FDRPerentage = Math.Round((currentDeposit.Value / totalPrincialAmount.Value) * 100, 2);

                    //totalPrincialAmount==null ? 0 : Math.Round((totalPrincialAmount != 0 ? (currentDeposit / totalPrincialAmount) * 100 : 0).Value, 2);
                    //oFIInformation.NPL = oFI.NPL;
                    //converting corer
                    oFIInformation.ExitsingDeposit = oFIInformation.ExitsingDeposit > 0 ? (oFIInformation.ExitsingDeposit / 10000000) : 0;

                    var BranchList = from e in db.FIBRANCHes.ToList()
                                     where e.FINANCIALINSTITUTION_REFERENCE == reference
                                     select new
                                     {
                                         REFERENCE = e.REFERENCE,
                                         NAME = e.NAME,

                                         CAPLimit = oFIInformation.CAPLimit,
                                         ExitsingDeposit = oFIInformation.ExitsingDeposit,
                                         FDRPerentage = oFIInformation.FDRPerentage,
                                         //NPL = oFI.NPL
                                     };
                    return Json(BranchList, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]
        public ActionResult FDRPurchaseNoteNew(string lblbreadcum = "")
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetDetailsAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                FDRNOTE oFDRNote = new FDRNOTE();
                oFDRNote.NOTETYPE = ConstantVariable.STATUS_NOTETYPE_NEW;// "New";
                oFDRNote.FINANCIALINSTITUTION_REFERENCE = "Select an Institution";
                oFDRNote.BRANCH_REFERENCE = "Select a Branch";

                ViewBag.COMPOUNDINTERESTTYPEList = "";
                ViewBag.ComoundInterestIntervalList = "";
                ViewBag.AnnualDays = 365;


                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "TenureTerm").FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.FIBranch = new SelectList(new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION");

                Session["currentPage"] = lblbreadcum = "" ?? "FDR Purchase Note";

                //added by Rakibul Date<4th Feb 2016>
                var interestType = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var COMPOUNDINTERESTTYPE = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestType).OrderBy(app => app.DESCRIPTION).ToList();
                ViewBag.COMPOUNDINTERESTTYPEList = new SelectList(COMPOUNDINTERESTTYPE, "DESCRIPTION", "DESCRIPTION");



                var interval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
                var ComoundInterestInterval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interval).OrderBy(app => app.DESCRIPTION).ToList();
                ViewBag.ComoundInterestIntervalList = new SelectList(ComoundInterestInterval, "DESCRIPTION", "DESCRIPTION");

                ViewBag.OfferRate = null;

                //assign this in a hidden field 
                ViewBag.FDRPurchaseNoteNew = ConstantVariable.FDRPurchaseNoteNew;

                return PartialView(oFDRNote);
                //return PartialView("FDRPurchaseNoteEntry", oFDRNote);

            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult FDRPurchaseNoteEdit(string proposalDetails, string Ref = null, string lblbreadcum = "", string errorMsg = "")
        {
            //return if session closed
            if (Ref == null || Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString()) + errorMsg;
            ViewBag.Refference = proposalDetails;
            ViewBag.breadcum = oCommonFunction.GetDetailsAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
            ViewBag.Header = "Edit " + Session["currentPage"];



            FDRNOTE oFDRNote;

            try
            {
                FDRPROPOSALDETAIL oDetails = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALDETAILS.Where(app => app.REFERENCE == proposalDetails).FirstOrDefault();

                ViewBag.ProposalDetailsReference = proposalDetails;
                ViewBag.PercentOfFDR = oDetails.PERCENTAGEOFTOTALFDR;
                ViewBag.OfferRate = oDetails.OFFERRATE;
                ViewBag.Tenure = oDetails.TENURE;
                ViewBag.Terms = oDetails.TERMS;
                ViewBag.InterestMode = oDetails.INTERESTMODE;


                List<OfferRate> offerRate = new List<OfferRate> { 
                   new OfferRate{Offer_Rate=oDetails.OFFERRATE,Tenure=oDetails.TENURE,Terms=oDetails.TERMS, InterestMode=oDetails.INTERESTMODE},
                   new OfferRate{Offer_Rate=oDetails.OFFERRATE1,Tenure=oDetails.TENURE1,Terms=oDetails.TERM1,InterestMode=oDetails.INTERESTMODE1},
                   new OfferRate{Offer_Rate=oDetails.OFFERRATE2,Tenure=oDetails.TENURE2,Terms=oDetails.TERM2,InterestMode=oDetails.INTERESTMODE2},
                   new OfferRate{Offer_Rate=oDetails.OFFERRATE3,Tenure=oDetails.TENURE3,Terms=oDetails.TERM3,InterestMode=oDetails.INTERESTMODE3},
                   new OfferRate{Offer_Rate=oDetails.OFFERRATE4,Tenure=oDetails.TENURE4,Terms=oDetails.TERM4,InterestMode=oDetails.INTERESTMODE4}
                };
                ViewBag.OfferRate = offerRate;

                ViewBag.AnnualDays = oDetails.ANNUALDAYS;

                oFDRNote = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Where(t => t.REFERENCE == Ref).SingleOrDefault();

                ////Set default note value
                //oFDRNote.FINANCIALINSTITUTION_REFERENCE = oDetails.FINANCIALINSTITUTION.REFERENCE;
                //oFDRNote.BRANCH_REFERENCE = oDetails.BRANCH_REFERENCE;
                //oFDRNote.EXISTINGDEPOSIT = oDetails.CURRENTHOLDING;
                //oFDRNote.CAPLIMIT = oDetails.EXISTINGCAPLIMIT;
                //oFDRNote.PERCENTAGEOFFDR = oDetails.PERCENTAGEOFTOTALFDR;

                ////added by rakibul 4/2/2016
                //oFDRNote.OFFERRATE = oDetails.OFFERRATE;
                ////oFDRNote.TENURE = oDetails.TENURE;
                ////oFDRNote.TENURETERM = oDetails.TERMS;
                //oFDRNote.ANNUALDAYS = oDetails.ANNUALDAYS;
                ////oFDRNote.INTERESTMODE = oDetails.INTERESTMODE;
                ////end editing

                //oFDRNote.PRINCIPALAMOUNT = oDetails.PRINCIPALAMOUNT;
                //oFDRNote.NOTETYPE = ConstantVariable.STATUS_NOTETYPE_NEW;

                //if (oDetails.FINANCIALINSTITUTION.CONTACTPERSON != null && !string.IsNullOrEmpty(oDetails.FINANCIALINSTITUTION.CONTACTPERSON))
                //{
                //    oFDRNote.CONTACTPERSON = oDetails.FINANCIALINSTITUTION.CONTACTPERSON + " - " + oDetails.FINANCIALINSTITUTION.CONTACTNO;
                //}
                //else
                //{
                //    oFDRNote.CONTACTPERSON = "";
                //}

                //oFDRNote.PROPOSALSUMMARY = oDetails.CREATEDDATE.Value.ToString("dd-MM-yyyy") + " - " + (oDetails.FINANCIALINSTITUTION.NAME.Count() > 10 ? oDetails.FINANCIALINSTITUTION.NAME.Substring(0, 10) : oDetails.FINANCIALINSTITUTION.NAME);


                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "TenureTerm").FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.FIBranch = new SelectList(new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION", oDetails.TERMS);

                Session["currentPage"] = lblbreadcum = "" ?? "FDR Purchase Note";

                ViewBag.breadcum = oCommonFunction.GetDetailsAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = Session["currentPage"];
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());

                TempData["oProposalDetail"] = oDetails;

                //added by Rakibul Date<4th Feb 2016>
                var interestType = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var COMPOUNDINTERESTTYPE = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestType).OrderBy(app => app.DESCRIPTION).ToList();
                ViewBag.COMPOUNDINTERESTTYPEList = new SelectList(COMPOUNDINTERESTTYPE, "DESCRIPTION", "DESCRIPTION", oDetails.INTERESTMODE);



                var interval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
                var ComoundInterestInterval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interval).OrderBy(app => app.DESCRIPTION).ToList();
                ViewBag.ComoundInterestIntervalList = new SelectList(ComoundInterestInterval, "DESCRIPTION", "DESCRIPTION");

                ViewBag.COMPOUNDINTERESTINTERVAL = oFDRNote.COMPOUNDINTERESTINTERVAL == null ? "Select a Interval" : oFDRNote.COMPOUNDINTERESTINTERVAL;

                return PartialView("FDRPurchaseNoteEdit", oFDRNote);
            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }



        [HttpGet]
        public ActionResult FDRPurchaseNoteEntry(string proposalDetails, string Ref = null, string lblbreadcum = "", string errorMsg = "")
        {
            //return if session closed
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString()) + errorMsg;
            ViewBag.Refference = proposalDetails;
            ViewBag.breadcum = oCommonFunction.GetDetailsAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
            ViewBag.Header = "Add " + Session["currentPage"];



            FDRNOTE oFDRNote = new FDRNOTE();

            try
            {
                FDRPROPOSALDETAIL oDetails = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALDETAILS.Where(app => app.REFERENCE == proposalDetails).FirstOrDefault();

                ViewBag.ProposalDetailsReference = proposalDetails;
                ViewBag.PercentOfFDR = oDetails.PERCENTAGEOFTOTALFDR;
                ViewBag.OfferRate = oDetails.OFFERRATE;
                ViewBag.Tenure = oDetails.TENURE;
                ViewBag.Terms = oDetails.TERMS;
                ViewBag.InterestMode = oDetails.INTERESTMODE;

                List<OfferRate> offerRate = new List<OfferRate> { 
                   new OfferRate{Offer_Rate=oDetails.OFFERRATE,Tenure=oDetails.TENURE,Terms=oDetails.TERMS, InterestMode=oDetails.INTERESTMODE},
                   new OfferRate{Offer_Rate=oDetails.OFFERRATE1,Tenure=oDetails.TENURE1,Terms=oDetails.TERM1,InterestMode=oDetails.INTERESTMODE1},
                   new OfferRate{Offer_Rate=oDetails.OFFERRATE2,Tenure=oDetails.TENURE2,Terms=oDetails.TERM2,InterestMode=oDetails.INTERESTMODE2},
                   new OfferRate{Offer_Rate=oDetails.OFFERRATE3,Tenure=oDetails.TENURE3,Terms=oDetails.TERM3,InterestMode=oDetails.INTERESTMODE3},
                   new OfferRate{Offer_Rate=oDetails.OFFERRATE4,Tenure=oDetails.TENURE4,Terms=oDetails.TERM4,InterestMode=oDetails.INTERESTMODE4}
                };
                ViewBag.OfferRate = offerRate;

                ViewBag.AnnualDays = oDetails.ANNUALDAYS;


                //Set default note value
                oFDRNote.FINANCIALINSTITUTION_REFERENCE = oDetails.FINANCIALINSTITUTION.REFERENCE;
                oFDRNote.BRANCH_REFERENCE = oDetails.BRANCH_REFERENCE;
                oFDRNote.EXISTINGDEPOSIT = oDetails.CURRENTHOLDING;
                oFDRNote.CAPLIMIT = oDetails.EXISTINGCAPLIMIT;
                oFDRNote.PERCENTAGEOFFDR = oDetails.PERCENTAGEOFTOTALFDR;

                //added by rakibul 4/2/2016
                oFDRNote.OFFERRATE = oDetails.OFFERRATE;
                //oFDRNote.TENURE = oDetails.TENURE;
                //oFDRNote.TENURETERM = oDetails.TERMS;
                oFDRNote.ANNUALDAYS = oDetails.ANNUALDAYS;
                //oFDRNote.INTERESTMODE = oDetails.INTERESTMODE;
                //end editing

                oFDRNote.PRINCIPALAMOUNT = oDetails.PRINCIPALAMOUNT;
                oFDRNote.NOTETYPE = ConstantVariable.STATUS_NOTETYPE_NEW;

                if (oDetails.FINANCIALINSTITUTION.CONTACTPERSON != null && !string.IsNullOrEmpty(oDetails.FINANCIALINSTITUTION.CONTACTPERSON))
                {
                    oFDRNote.CONTACTPERSON = oDetails.FINANCIALINSTITUTION.CONTACTPERSON + " - " + oDetails.FINANCIALINSTITUTION.CONTACTNO;
                }
                else
                {
                    oFDRNote.CONTACTPERSON = "";
                }

                oFDRNote.PROPOSALSUMMARY = oDetails.CREATEDDATE.Value.ToString("dd-MM-yyyy") + " - " + (oDetails.FINANCIALINSTITUTION.NAME.Count() > 10 ? oDetails.FINANCIALINSTITUTION.NAME.Substring(0, 10) : oDetails.FINANCIALINSTITUTION.NAME);


                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "TenureTerm").FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.FIBranch = new SelectList(new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION", oDetails.TERMS);

                Session["currentPage"] = lblbreadcum = "" ?? "FDR Purchase Note";

                ViewBag.breadcum = oCommonFunction.GetDetailsAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = Session["currentPage"];
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());

                TempData["oProposalDetail"] = oDetails;

                //added by Rakibul Date<4th Feb 2016>
                var interestType = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var COMPOUNDINTERESTTYPE = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestType).OrderBy(app => app.DESCRIPTION).ToList();
                ViewBag.COMPOUNDINTERESTTYPEList = new SelectList(COMPOUNDINTERESTTYPE, "DESCRIPTION", "DESCRIPTION", oDetails.INTERESTMODE);



                var interval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
                var ComoundInterestInterval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interval).OrderBy(app => app.DESCRIPTION).ToList();
                ViewBag.ComoundInterestIntervalList = new SelectList(ComoundInterestInterval, "DESCRIPTION", "DESCRIPTION");

                ViewBag.COMPOUNDINTERESTINTERVAL = oFDRNote.COMPOUNDINTERESTINTERVAL == null ? "Select a Interval" : oFDRNote.COMPOUNDINTERESTINTERVAL;

                return PartialView("FDRPurchaseNoteEntry", oFDRNote);
            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        [NonAction]
        public string FDRPurchaseNoteEntryValidation(decimal tenure, string tenuerTerms, string InterestMode, string InterestInterval, int? AnnualDays)
        {
            if (InterestMode == ConstantVariable.INTERESTMODE_COMPOUND)
            {
                if (tenuerTerms == ConstantVariable.TENURETERM_DAYS)
                    return "Days is not valid for Compound";

                if (InterestInterval == ConstantVariable.COMPOUND_INTEREST_QUARTERLY_MSG)
                {
                    if (tenuerTerms == ConstantVariable.TENURETERM_MONTHS)
                    {
                        if (tenure % 3 == 0) return "True"; else return "Invalid Tenure";
                    }
                    else if (tenuerTerms == ConstantVariable.TENURETERM_YEARS)
                    {
                        tenure = tenure * 12;
                        if (tenure % 3 == 0) return "True"; else return "Invalid Tenure";
                    }
                    else
                        return "Invalid Tenure Terms";
                }
                else if (InterestInterval == ConstantVariable.COMPOUND_INTEREST_HALFYEARLY_MSG)
                {
                    if (tenuerTerms == ConstantVariable.TENURETERM_MONTHS)
                    {
                        if (tenure % 6 == 0) return "True"; else return "Invalid Tenure";
                    }
                    else if (tenuerTerms == ConstantVariable.TENURETERM_YEARS)
                    {
                        tenure = tenure * 12;
                        if (tenure % 6 == 0) return "True"; else return "Invalid Tenure";
                    }
                    else
                        return "Invalid Tenure Terms";
                }
                else if (InterestInterval == ConstantVariable.COMPOUND_INTEREST_YEARLY_MSG)
                {
                    if (tenuerTerms == ConstantVariable.TENURETERM_MONTHS)
                    {
                        if (tenure % 12 == 0) return "True"; else return "Invalid Tenure";
                    }
                    else if (tenuerTerms == ConstantVariable.TENURETERM_YEARS)
                    {
                        tenure = tenure * 12;
                        if (tenure % 12 == 0) return "True"; else return "Invalid Tenure";
                    }
                    else
                        return "Invalid Tenure Terms";
                }
                else if (InterestInterval == ConstantVariable.COMPOUND_INTEREST_MONTHLY_MSG)
                {
                    //nothing to do
                    return "True";
                }


            }
            else if (InterestMode == ConstantVariable.INTERESTMODE_FLAT)
            {
                //only check Annual Days when tenure terms Days
                if (tenuerTerms == ConstantVariable.TENURETERM_DAYS)
                {
                    //check AnnualDays
                    if (AnnualDays == null)
                        return "Annual Days is Required !!";
                    else if (AnnualDays.Value >= 360 && AnnualDays.Value <= 366)
                    {
                        return "True";
                    }
                    else
                        return "Annual Days must be 360 to 366 range !!";
                }

            }
            else
                return "Compounding Interest Interval is required!!";

            return "True";
            //  return "Interest Mode is Requered!!";

        }


        [HttpPost]
        public ActionResult FDRPurchaseNoteEntry(FDRNOTE oFDRNOTE, string FDRPurchaseNoteNew = null)
        {
            string IsValid = null;
            //return if session closed
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            #region Validation

            var FdrProposalReference = TempData["PrposalReference"] as string;

            //Validate the data InterestMode CompoundInterestInterval etc
            if (oFDRNOTE.TENURE.Value > 0)
            {
                if (oFDRNOTE.TENURETERM != null && !string.IsNullOrEmpty(oFDRNOTE.TENURETERM))
                {
                    if (oFDRNOTE.INTERESTMODE == null && string.IsNullOrEmpty(oFDRNOTE.INTERESTMODE))
                    {
                        IsValid = "Interest Mode is Required !!";
                    }
                    else
                    {
                        //Cal Validation method that return true if ok else return invalid message
                        IsValid = FDRPurchaseNoteEntryValidation(oFDRNOTE.TENURE.Value, oFDRNOTE.TENURETERM, oFDRNOTE.INTERESTMODE, oFDRNOTE.COMPOUNDINTERESTINTERVAL, oFDRNOTE.ANNUALDAYS);

                    }
                }
                else
                    IsValid = "Tenure Terms is requered !!";

            }
            else
                IsValid = "Invalid Tenure !!";


            if (IsValid != ConstantVariable.TRUE)
            {
                return RedirectToAction("Index", "ErrorPage", new { message = IsValid });
            }


            #endregion


            if (oFDRNOTE.REFERENCE != null && !string.IsNullOrEmpty(oFDRNOTE.REFERENCE))
            {
                #region EditRecord



                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    FDRNOTE EditFDR = db.FDRNOTEs.Where(t => t.REFERENCE == oFDRNOTE.REFERENCE).SingleOrDefault();


                    EditFDR.FINANCIALINSTITUTION_REFERENCE = oFDRNOTE.FINANCIALINSTITUTION_REFERENCE;
                    EditFDR.BRANCH_REFERENCE = oFDRNOTE.BRANCH_REFERENCE;

                    EditFDR.EXISTINGDEPOSIT = oFDRNOTE.EXISTINGDEPOSIT;
                    EditFDR.PERCENTAGEOFFDR = oFDRNOTE.PERCENTAGEOFFDR;
                    EditFDR.CAPLIMIT = oFDRNOTE.CAPLIMIT;
                    EditFDR.SIGNATORY1 = oFDRNOTE.SIGNATORY1;
                    EditFDR.SIGNATORY2 = oFDRNOTE.SIGNATORY2;

                    EditFDR.CONTACTPERSON = oFDRNOTE.CONTACTPERSON;
                    EditFDR.PRINCIPALAMOUNT = oFDRNOTE.PRINCIPALAMOUNT;

                    EditFDR.PROPOSEDRATE = oFDRNOTE.PROPOSEDRATE;
                    EditFDR.OFFERRATE = oFDRNOTE.OFFERRATE;

                    EditFDR.TENURE = oFDRNOTE.TENURE;
                    EditFDR.TENURETERM = oFDRNOTE.TENURETERM;
                    EditFDR.INTERESTMODE = oFDRNOTE.INTERESTMODE;
                    EditFDR.ANNUALDAYS = oFDRNOTE.ANNUALDAYS;
                    EditFDR.COMPOUNDINTERESTINTERVAL = oFDRNOTE.COMPOUNDINTERESTINTERVAL;

                    EditFDR.LASTUPDATED = DateTime.Now;
                    EditFDR.LASTUPDATEDBY = Session["UserId"].ToString();

                    db.Entry(EditFDR).State = EntityState.Modified;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        string message = ex.Message;

                        return RedirectToAction("Index", "ErrorPage", new { message });
                    }

                }
                #endregion
            }
            else
            {
                #region AddNewRecord
                //if we post without creating a fdrproposal and directly add data from FDRPurchaseNoteNew so there are now value in saveDetails

                FDRPROPOSALDETAIL saveDetails = TempData["oProposalDetail"] as FDRPROPOSALDETAIL;
                //added by rakibul 7th Feb 2016 
                //edit anylysis 17/11/2016 for add new record directly from FDR PurchaseNote without creating FDRProposalDetails
                //we Should add Null value in FDRProposalDetailsREF
                //oFDRNOTE.FDRPROPOSALDETAILSREF = null;                      // saveDetails.REFERENCE;

                //Set default note value
                oFDRNOTE.REFERENCE = Guid.NewGuid().ToString();
                oFDRNOTE.NOTEID = Guid.NewGuid().ToString();
                oFDRNOTE.FDRPROPOSALDETAILSREF = saveDetails.REFERENCE;
                oFDRNOTE.FINANCIALINSTITUTION_REFERENCE = saveDetails.FINANCIALINSTITUTION.REFERENCE;
                oFDRNOTE.CAPLIMIT = saveDetails.FINANCIALINSTITUTION.CAPLIMIT;
                //oFDRNOTE.PROPOSEDRATE=saveDetails.p
                //Commented by Nazmul: Usre want to input the summery manually.
                //oFDRNOTE.PROPOSALSUMMARY = saveDetails.CREATEDDATE.Value.ToString("dd-MM-yyyy") + " - " + (saveDetails.FINANCIALINSTITUTION.NAME.Count() > 10 ? saveDetails.FINANCIALINSTITUTION.NAME.Substring(0, 10) : saveDetails.FINANCIALINSTITUTION.NAME);

                oFDRNOTE.NOTETYPE = ConstantVariable.STATUS_NOTETYPE_NEW;
                oFDRNOTE.STATUS = ConstantVariable.STATUS_PENDING;

                oFDRNOTE.CREATEDBY = Session["UserId"].ToString();
                oFDRNOTE.CREATEDDATE = DateTime.Now; // DateTime.Today;


                //still it is not opened or issued so its null
                oFDRNOTE.OPENEDDATE = null;

                oCommonFunction.CustomObjectNullValidation<FDRNOTE>(ref oFDRNOTE);
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    FDRPROPOSALDETAIL oFDRPROPOSALDETAIL = db.FDRPROPOSALDETAILS.Where(app => app.REFERENCE == saveDetails.REFERENCE).FirstOrDefault();
                    //Update Proposal detail status
                    oFDRPROPOSALDETAIL.STATUS = ConstantVariable.STATUS_APPROVED; // "Approved";
                    db.Entry(oFDRPROPOSALDETAIL).State = EntityState.Modified;

                    db.FDRNOTEs.Add(oFDRNOTE);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        string message = ex.Message;

                        return RedirectToAction("Index", "ErrorPage", new { message });
                    }

                }

                #endregion
            }

            TempData["GridHeader"] = "Purchase Note List";
            return RedirectToAction("ListFDRNote", new { lblbreadcum = "FDR Purchase Notes" });
        }

        public ActionResult PrintFDRPurchaseNote(string reference = null)
        {

            List<FDRNOTE> oFDRNotes = new List<FDRNOTE>();
            try
            {
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    //models = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALDETAILS.ToList();
                    //models = models.Where(m => m.FDRPROPOSAL_REFERENCE == reference).ToList();
                    oFDRNotes = db.FDRNOTEs.Where(f => f.REFERENCE == reference).ToList();

                }

                ReportDataSource reportSource = new ReportDataSource();
                reportSource.Name = "PurchaseNoteList";
                reportSource.Value = new CommonFunction().ConvertToDataTable(oFDRNotes);


                LocalReport lr = new LocalReport();
                lr.ReportPath = Server.MapPath("~/Reports/FDRPurchaseNote.rdlc");
                lr.DataSources.Add(reportSource);


                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;


                string deviceInfo =

                "<DeviceInfo>" +
                    "  <OutputFormat>PDF</OutputFormat>" +
                    "  <PageWidth>8.12in</PageWidth>" +
                    "  <PageHeight>11.5in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>0.5in</MarginLeft>" +
                    "  <MarginRight>0in</MarginRight>" +
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


                return File(renderedBytes, mimeType, "FDRPurchaseNote.pdf");
            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        ChequeViewModel cheque = new ChequeViewModel();

        [HttpGet]
        public ActionResult ApproveFDRPurchaseNote(string purchaseNoteReference, string lblbreadcum = "", string Number = "", string errorMsg = "")
        {
            //return if session closed
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            CHEQUEDRAWNDummy.Add(new ChequeDrawnViewModel { CHEQUEAMOUNT = null, CHEQUEDATE = null, CHEQUEDRAWNFROM = "", CHEQUENO = "", PROPOSEDACTION = "" });

            if (Number != "")
            {
                purchaseNoteReference = TempData["pnref"] as string;
                lblbreadcum = TempData["lblbredcum"] as string;
                int num = Convert.ToInt32(Number);

                for (int i = 0; i < num - 1; i++)
                {
                    CHEQUEDRAWNDummy.Add(new ChequeDrawnViewModel { CHEQUEAMOUNT = null, CHEQUEDATE = null, CHEQUEDRAWNFROM = "", CHEQUENO = "", PROPOSEDACTION = "" });
                    //  cheque.ChequeDrawnList.Add(new ChequeDrawnViewModel { CHEQUEAMOUNT = null, CHEQUEDATE = null, CHEQUEDRAWNFROM = "", CHEQUENO = "", PROPOSEDACTION = "" } );
                }

            }


            ViewBag.pnref = purchaseNoteReference;
            ViewBag.lblbredcum = lblbreadcum;

            ViewBag.CHEQUEDATE = DateTime.Today.ToString("dd-MMM-yy");

            Session["currentPage"] = lblbreadcum;

            ViewBag.breadcum = oCommonFunction.GetDetailsAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
            ViewBag.Header = "Approve " + Session["currentPage"];
            ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString()) + errorMsg;


            try
            {

                FDRNOTE oFDRNote = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Where(app => app.REFERENCE == purchaseNoteReference).FirstOrDefault();


                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "TenureTerm").FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();


                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.FInstitution_Ref = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME", oFDRNote.FINANCIALINSTITUTION_REFERENCE);

                ViewBag.FIBranch = new SelectList(new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.FIBranch_Ref = new SelectList(new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME", oFDRNote.BRANCH_REFERENCE);


                var proposaedlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "FDR Note" && app.PROPERTY == "ProposedAction").FirstOrDefault().REFERENCE.ToString();
                var actionlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == proposaedlist).ToList();

                ViewBag.ProposaedAction = new SelectList(actionlist, "DESCRIPTION", "DESCRIPTION");
                //return PartialView("ApproveFDRPurchaseNote", oFDRNote);

                List<FDRNOTE> fdrnotelist = new List<FDRNOTE> { 
            
            new FDRNOTE{ 
                         REFERENCE=oFDRNote.REFERENCE, 
                         FINANCIALINSTITUTION=oFDRNote.FINANCIALINSTITUTION,
                         BRANCH_REFERENCE=oFDRNote.BRANCH_REFERENCE,
                         EXISTINGDEPOSIT=oFDRNote.EXISTINGDEPOSIT,
                         PERCENTAGEOFFDR=oFDRNote.PERCENTAGEOFFDR,
                         CAPLIMIT=oFDRNote.CAPLIMIT,
                         CONTACTPERSON=oFDRNote.CONTACTPERSON,
                         SIGNATORY1=oFDRNote.SIGNATORY1,
                         SIGNATORY2=oFDRNote.SIGNATORY2,
                         PROPOSEDRATE=oFDRNote.PROPOSEDRATE,
                         OFFERRATE=oFDRNote.OFFERRATE,
                         PRINCIPALAMOUNT=oFDRNote.PRINCIPALAMOUNT,
                         TENURE=oFDRNote.TENURE,
                         TENURETERM=oFDRNote.TENURETERM,
                         PROPOSALSUMMARY=oFDRNote.PROPOSALSUMMARY,
                         INTERESTMODE=oFDRNote.INTERESTMODE,
                         COMPOUNDINTERESTINTERVAL=oFDRNote.COMPOUNDINTERESTINTERVAL,
                         ANNUALDAYS=oFDRNote.ANNUALDAYS
                         
                     }
            };

                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION", oFDRNote.TENURETERM);
                //  ViewBag.TenureTerm = oFDRNote.TENURETERM;

                ViewBag.FDRNoteList = fdrnotelist;
                var list = CHEQUEDRAWNDummy;
                //cheque;
                //CHEQUEDRAWNDummy;

                return PartialView("ApproveFDRPurchaseNote", list);
            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        private List<InvestmentManagement.Models.ChequeDrawnViewModel> CHEQUEDRAWNDummy = new List<ChequeDrawnViewModel>();

        //List<ChequeDrawnViewModel> model
        [HttpPost]
        public ActionResult ApproveFDRPurchaseNote(List<ChequeDrawnViewModel> model, string Number, decimal? EXISTINGDEPOSIT, string REFERENCE)  //ChequeViewModel model   //FDRNOTE oFDRNOTE,string _CHEQUEDATE,
        {
            //return if session closed
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            try
            {
                
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    FDRNOTE saveFDRNote = db.FDRNOTEs.Where(app => app.REFERENCE == REFERENCE).FirstOrDefault();

                    //Save for Fix Deposit
                    FIXEDDEPOSIT oFIXEDDEPOSIT = new FIXEDDEPOSIT();

                    #region AmountValidation
                    //check the amount is ok

                    decimal ChequeAmount = model.ToList().Sum(t => t.CHEQUEAMOUNT).Value;

                    if (ChequeAmount != saveFDRNote.PRINCIPALAMOUNT.Value) // given cheque amount does not match with principle amount
                    {
                        // return RedirectToAction("ApproveFDRPurchaseNote", "FDRNote", new { purchaseNoteReference = REFERENCE, lblbreadcum = "", Number = "", errorMsg = "Cheque amount does not matched with principle amount !!please insert exact amount." });
                        return RedirectToAction("Index", "ErrorPage", new { message = "Cheque amount does not matched with principle amount !!please insert exact amount." });
                    }
                    #endregion


                    oFIXEDDEPOSIT.REFERENCE = Guid.NewGuid().ToString();
                    oFIXEDDEPOSIT.CREATEDBY = Session["UserId"].ToString();
                    oFIXEDDEPOSIT.CREATEDDATE = DateTime.Now;

                    oFIXEDDEPOSIT.LASTUPDATED = DateTime.Now;
                    oFIXEDDEPOSIT.STATUS = ConstantVariable.STATUS_PENDING;    // "Pending";

                    oFIXEDDEPOSIT.TENURE = saveFDRNote.TENURE;
                    oFIXEDDEPOSIT.TENURETERM = saveFDRNote.TENURETERM;
                    oFIXEDDEPOSIT.PRINCIPALAMOUNT = saveFDRNote.PRINCIPALAMOUNT;
                    oFIXEDDEPOSIT.PRESENTPRINCIPALAMOUNT = saveFDRNote.PRINCIPALAMOUNT;

                    //added 06-10-16
                    oFIXEDDEPOSIT.INITIALPRINCIPALAMOUNT = saveFDRNote.PRINCIPALAMOUNT.Value;

                    //addede 07/11/2016 for history report 
                    oFIXEDDEPOSIT.INITIALFIXEDDEPOSITREF = oFIXEDDEPOSIT.REFERENCE;


                    //though we Add multiple CHEQUE in CHEQUEDRAWN table SoapVersionMismatchException we Add  oFIXEDDEPOSIT.REFERENCE
                    //in CHEQUEDRAWN FIXEDDEPOSIT_REFERENCE field for Trace ChequeNo but chequeDate is one so ChequeDate is in FixedDeposit table
                    //oFIXEDDEPOSIT.CHEQUEREFERENCE = oFDRNOTE.CHEQUENO;

                    oFIXEDDEPOSIT.CHEQUEDATE = DateTime.Now;     //date.Date; edited 12-05-17

                    oFIXEDDEPOSIT.COMPOUNDINTERESTINTERVAL = saveFDRNote.COMPOUNDINTERESTINTERVAL;                // "Quarterly";
                    oFIXEDDEPOSIT.INTERESTMODE = saveFDRNote.INTERESTMODE;                                        // "Flat";
                    oFIXEDDEPOSIT.ANNUALDAYS = saveFDRNote.ANNUALDAYS;                                            // 365;

                    oFIXEDDEPOSIT.RATEOFINTEREST = saveFDRNote.PROPOSEDRATE;
                    oFIXEDDEPOSIT.EXISTINGCAPLIMIT = saveFDRNote.CAPLIMIT;
                    oFIXEDDEPOSIT.FINANCIALINSTITUTION_REFERENCE = saveFDRNote.FINANCIALINSTITUTION.REFERENCE;
                    oFIXEDDEPOSIT.BRANCH_REFERENCE = saveFDRNote.BRANCH_REFERENCE;

                    oFIXEDDEPOSIT.TAXRATE = db.FINANCIALINSTITUTIONs.Where(t => t.REFERENCE == oFIXEDDEPOSIT.FINANCIALINSTITUTION_REFERENCE).SingleOrDefault().TAXRATE;

                    oFIXEDDEPOSIT.SIGNATORY1 = saveFDRNote.SIGNATORY1;
                    oFIXEDDEPOSIT.SIGNATORY2 = saveFDRNote.SIGNATORY2;


                    if (oFIXEDDEPOSIT.TENURETERM.ToLower() == "Months".ToLower())
                        oFIXEDDEPOSIT.MATURITYDATE = DateTime.Now.AddMonths((int)oFIXEDDEPOSIT.TENURE);
                    else if (oFIXEDDEPOSIT.TENURETERM.ToLower() == "Years".ToLower())
                        oFIXEDDEPOSIT.MATURITYDATE = DateTime.Now.AddYears((int)oFIXEDDEPOSIT.TENURE);
                    else
                        oFIXEDDEPOSIT.MATURITYDATE = DateTime.Now.AddDays((int)oFIXEDDEPOSIT.TENURE);


                    oCommonFunction.CustomObjectNullValidation<FIXEDDEPOSIT>(ref oFIXEDDEPOSIT);
                    oFIXEDDEPOSIT.RENWALDATE = null;
                    //
                    oFIXEDDEPOSIT.ENCASHMENTDATE = null; //added 26-Jul-17

                    db.FIXEDDEPOSITs.Add(oFIXEDDEPOSIT);

                    //End


                    //Update FDR Note  

                    //saveFDRNote.CHEQUEDATE = date.Date; //block this when add Chack Date on ChaqueDrawn 12-06-17
                              
                    
                    saveFDRNote.APPROVEDBY = Session["UserId"].ToString();
                    saveFDRNote.APPROVEDDATE = DateTime.Now; // DateTime.Today;
                    saveFDRNote.STATUS = "Approved";
                    saveFDRNote.FIXEDDEPOSIT_REFERENCE = oFIXEDDEPOSIT.REFERENCE;
                    new CommonFunction().CustomObjectNullValidation<FDRNOTE>(ref saveFDRNote);

                    db.Entry(saveFDRNote).State = EntityState.Modified;
                    //db.SaveChanges();

                    //added in chequeDrawn table

                    foreach (var item in model)
                    {
                        CHEQUEDRAWN obj = new CHEQUEDRAWN();
                        obj.REFERENCE = Guid.NewGuid().ToString();
                        obj.CHEQUEDRAWNFROM = item.CHEQUEDRAWNFROM;
                        obj.CHEQUEDATE = item.CHEQUEDATE; // date.Date;
                        //date.Date;
                        //item.CHEQUEDATE;
                        obj.CHEQUENO = item.CHEQUENO;
                        obj.CHEQUEAMOUNT = item.CHEQUEAMOUNT;
                        obj.PROPOSEDACTION = item.PROPOSEDACTION;
                        obj.APPROVEDBY = Session["UserId"].ToString();
                        obj.APPROVEDDATE = DateTime.Now; // DateTime.Today;
                        obj.STATUS = ConstantVariable.STATUS_APPROVED;
                        obj.FIXEDDEPOSIT_REFERENCE = oFIXEDDEPOSIT.REFERENCE;
                        obj.FDRNOTE_REFERENCE = saveFDRNote.REFERENCE;

                        oCommonFunction.CustomObjectNullValidation<CHEQUEDRAWN>(ref obj);

                        db.CHEQUEDRAWNs.Add(obj);

                    }

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        string message = ex.Message;

                        return RedirectToAction("Index", "ErrorPage", new { message = "Can not Save due to internal problem!" });
                    }
                }

                return RedirectToAction("ListFixedDeposit", "FixedDeposit", new { lblbreadcum = "Fixed Deposit" });
            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }


        [HttpPost]
        public ActionResult GeneratePurchaseNoteList(string reference, string appStatus = null, string penStatus = null, string sort = null, string sortdir = null)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }


            //loading configuration
            //sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
            //sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;

            sort = "CREATEDDATE";
            sortdir = "DESC";

            List<FDRNOTE> fdrnoteList = new List<FDRNOTE>();
            ReportDataSource oFDRNotes = new ReportDataSource();

            //fdrnoteList = TempData["FDRNoteList"] as List<FDRNOTE>;

            string fiRef = TempData["FINANCIALINSTITUTION_REFERENCE"] as string;


            string reportTitle = "Approved Purchase Note"; ;
            string reportName = "ApprovedPurchaseNoteList.pdf";
            string reportStatus = "Approved";

            if (!string.IsNullOrEmpty(penStatus))
            {
                reportStatus = "Pending";
                reportTitle = "Pending Purchase Note";
                reportName = "PendingPurchaseNoteList.pdf";
            }


            try
            {
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    //in this place we do if has reference request come as link
                    if (reference != null && !string.IsNullOrEmpty(reference))
                    {
                        fdrnoteList = db.FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.REFERENCE == reference).OrderBy(sort + " " + sortdir).ToList();
                        if (fdrnoteList[0].STATUS == ConstantVariable.STATUS_PENDING)
                            penStatus = ConstantVariable.STATUS_PENDING; //else penstatus =null
                    }
                    else
                    {

                        fdrnoteList = db.FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE == "New" && note.STATUS == reportStatus).OrderBy(sort + " " + sortdir).ToList();

                        if (!string.IsNullOrEmpty(fiRef))
                        {

                            fdrnoteList = fdrnoteList.Where(fdrnote => fdrnote.FINANCIALINSTITUTION_REFERENCE == fiRef).ToList();
                        }
                    }




                    if (TempData["openingDate"] != null && TempData["toDate"] != null)
                    {
                        DateTime openingDate = DateTime.Parse(TempData["openingDate"].ToString());
                        DateTime toDate = DateTime.Parse(TempData["toDate"].ToString());
                        if (openingDate != null && toDate != null)
                        {
                            fdrnoteList = fdrnoteList.Where(pi => pi.CREATEDDATE >= openingDate && pi.CREATEDDATE <= toDate).ToList();
                        }
                    }



                    var newfdrNoteList = from fdrNote in fdrnoteList
                                         select new
                                         {
                                             FinancialInstitution = fdrNote.FINANCIALINSTITUTION.NAME,
                                             FIBranch = fdrNote.FIBRANCH.NAME,

                                             PrincipalAmount = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(fdrNote.PRINCIPALAMOUNT.Value)).Replace("$", string.Empty),  //C2 means get 2 decimap places value Math.Round(item.PRINCIPALAMOUNT.Value,2);
                                             Period = Convert.ToString(fdrNote.TENURE.Value) + " " + fdrNote.TENURETERM,
                                             OfferRate = Convert.ToString(fdrNote.PROPOSEDRATE.Value) + "%\n" + (fdrNote.INTERESTMODE == ConstantVariable.INTERESTMODE_COMPOUND ? "(" + fdrNote.COMPOUNDINTERESTINTERVAL + " com)" : ""),
                                             ExisxtingDeposit = Math.Round(Convert.ToDecimal(db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.STATUS == ConstantVariable.STATUS_APPROVED && fixDeposit.FINANCIALINSTITUTION_REFERENCE == fdrNote.FINANCIALINSTITUTION_REFERENCE).Sum(fixDeposit => fixDeposit.PRINCIPALAMOUNT) / 10000000), 2),
                                             CapLimit = Convert.ToString(fdrNote.CAPLIMIT.Value),
                                             FDRPercentage = Convert.ToString(fdrNote.PERCENTAGEOFFDR.Value)


                                         };
                    LocalReport lr = new LocalReport();

                    ReportDataSource rd = new ReportDataSource();
                    //ReportDataSource dd = new ReportDataSource();

                    DataTable dtFDRNote = oCommonFunction.ConvertToDataTable(newfdrNoteList.ToList());
                    rd.Name = "PurchaseNoteList";
                    rd.Value = dtFDRNote;




                    lr.ReportPath = Server.MapPath("~/Reports/FDRPurchaseNote.rdlc");
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
        public ActionResult GeneratePurchaseNoteLetter(string reference)
        {
            //addded by me
            List<CHEQUEDRAWN> chequeDrawnList = new List<CHEQUEDRAWN>();
            //
            DataSet ds = new DataSet();
            List<FDRNOTE> fdrnoteList = new List<FDRNOTE>();

            ReportDataSource oFDRNotes = new ReportDataSource();

            string Name = "";
            string CONTACT_PERSON = "";
            string PrincipalAmount = "";
            string PrincipalAmountTK = "";
            string Branch = "";
            string Tenure = "";
            string TenureTerm = "";
            string ProposedRate = "";
            string Signatory1 = "";
            string Signatory2 = "";

            string fiRef = TempData["FINANCIALINSTITUTION_REFERENCE"] as string;

            try
            {
                //we get a single model data as list from hear
                fdrnoteList = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.REFERENCE == reference).OrderBy(pi => pi.CREATEDDATE).ToList();


                if (!string.IsNullOrEmpty(fiRef))
                {

                    fdrnoteList = fdrnoteList.Where(fdrnote => fdrnote.FINANCIALINSTITUTION_REFERENCE == fiRef).ToList();
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

                foreach (var item in fdrnoteList)
                {
                    Name = item.FINANCIALINSTITUTION.NAME;
                    CONTACT_PERSON = item.CONTACTPERSON;
                    Branch = item.FIBRANCH.NAME + " Branch" + Environment.NewLine + item.FIBRANCH.ADDRESSLINE1 + Environment.NewLine + item.FIBRANCH.ADDRESSLINE2;
                    Tenure = Convert.ToString(item.TENURE);
                    TenureTerm = item.TENURETERM;

                    PrincipalAmount = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(item.PRINCIPALAMOUNT.Value)).Replace("$", string.Empty);
                    PrincipalAmountTK = Convert.ToString(item.PRINCIPALAMOUNT.Value);

                    ProposedRate = Convert.ToString(item.PROPOSEDRATE) + "% " + (item.INTERESTMODE == ConstantVariable.INTERESTMODE_COMPOUND ? item.COMPOUNDINTERESTINTERVAL + " compounding" : "");
                    Signatory1 = item.SIGNATORY1;
                    Signatory2 = item.SIGNATORY2;

                }

                //added by me 1/20/16  select new CHEQUEDRAWN
                chequeDrawnList = new Entities(Session["Connection"] as EntityConnection).CHEQUEDRAWNs.Where(ChequeDran => ChequeDran.FDRNOTE_REFERENCE == reference).OrderBy(t => t.CHEQUEDATE).ToList();
                var newChequeDrawnList = (from fdrNote in fdrnoteList
                                          join Cheque in chequeDrawnList
                                               on fdrNote.REFERENCE equals Cheque.FDRNOTE_REFERENCE

                                          select new
                                          {
                                              // REFERENCE = Cheque.REFERENCE,
                                              STRCHEQUEDATE = Convert.ToString(Cheque.CHEQUEDATE.Value.ToString("dd-MMM-yyyy")),
                                              CHEQUENO = Cheque.CHEQUENO,
                                              CHEQUEAMOUNT = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(Cheque.CHEQUEAMOUNT.Value)).Replace("$", string.Empty),
                                              CHEQUEDRAWNFROM = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.Where(fi => fi.REFERENCE == Cheque.CHEQUEDRAWNFROM).FirstOrDefault().NAME
                                          }).OrderBy(t => t.CHEQUENO).ToList();

                LocalReport lr = new LocalReport();

                ReportDataSource rd = new ReportDataSource();
                //ReportDataSource dd = new ReportDataSource();

                DataTable dtChequeDrawn = oCommonFunction.ConvertToDataTable(newChequeDrawnList.ToList());
                rd.Name = "ChequeDrawn";             // "DataSet1";
                rd.Value = dtChequeDrawn;


               ReportParameter[] parameters = new ReportParameter[] 
               {
                 new ReportParameter("Name",Name),
                 new ReportParameter("ContactPerson",CONTACT_PERSON),
             
                 new ReportParameter("PrincipalAmount",PrincipalAmount),
                 new ReportParameter("PrincipalAmountTK",PrincipalAmountTK),

                 new ReportParameter("Branch",Branch),
                 new ReportParameter("Tenure",Tenure),
                 new ReportParameter("TenureTerm",TenureTerm),
                 new ReportParameter("ProposedRate",ProposedRate),
                 new ReportParameter("Signatory1",Signatory1),
                 new ReportParameter("Signatory2",Signatory2)

             };



                lr.ReportPath = Server.MapPath("~/Reports/FDRPurchaseNoteLetter.rdlc");
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

                string reportName = fdrnoteList[0].FINANCIALINSTITUTION.NAME + "-PurchaseNote-" + fdrnoteList[0].CREATEDDATE.Value.ToString("ddMMyyyy") + ".pdf";

                return File(renderedBytes, mimeType, reportName);
            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpGet]
        public ActionResult ListFDRRenewalNote(string FdrNo, string sortdir, string sort, int? page, int? rows, string lblbreadcum, string PagingType, int? currentRowPerPage = 15, string FINANCIALINSTITUTION_REFERENCE = null, string STATUS = null, DateTime? issueFrom = null, DateTime? issueTo = null)
        {
            //return if session closed
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            GridModel<FDRNOTE> gridModels = new GridModel<FDRNOTE>();
            List<FDRNOTE> models = null;

            if (TempData["GridHeader"] != null)
            {
                lblbreadcum = TempData["GridHeader"].ToString();
            }
            //grid settings                
            gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
            ViewBag.currentRowPerPage = gridModels.RowsPerPage;


            //loading configuration
            sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
            sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;

            models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_RENEWAL).OrderByDescending(t => t.CREATEDDATE).ToList();


            if (STATUS == null)
                models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_RENEWAL && note.STATUS == ConstantVariable.STATUS_PENDING).OrderByDescending(t => t.CREATEDDATE).ToList();
            else if (STATUS == "")
                models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_RENEWAL).OrderByDescending(t => t.CREATEDDATE).ToList();
            else
                models = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.STATUS == STATUS && note.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_RENEWAL).OrderByDescending(t => t.CREATEDDATE).ToList();

               
            if (issueTo != null)
            {
                TempData["toDate"] = issueTo;
                models = models.Where(fdrnote => fdrnote.CREATEDDATE.Value.Date <= issueTo.Value.Date).OrderByDescending(t => t.CREATEDDATE).ToList();
            }

            if (issueFrom != null)
            {
                TempData["openingDate"] = issueFrom;
                models = models.Where(fdrnote => fdrnote.CREATEDDATE.Value.Date >= issueFrom.Value.Date).OrderByDescending(t => t.CREATEDDATE).ToList();
            }
            //End filter

            if (FdrNo != null && !string.IsNullOrEmpty(FdrNo))
            {
                models = models.Where(t => t.FDRNUMBER == FdrNo).ToList();
            }

            if (!string.IsNullOrEmpty(FINANCIALINSTITUTION_REFERENCE))
            {
                TempData["FINANCIALINSTITUTION_REFERENCE"] = FINANCIALINSTITUTION_REFERENCE;
                models = models.Where(fdrnote => fdrnote.FINANCIALINSTITUTION_REFERENCE == FINANCIALINSTITUTION_REFERENCE).ToList();

            }
            var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "FDR Note" && app.PROPERTY == "STATUS").FirstOrDefault().REFERENCE.ToString();
            var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app => app.DESCRIPTION).ToList();

            if (!string.IsNullOrEmpty(lblbreadcum))
            {
                Session["currentPage"] = lblbreadcum;
            }


            Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

            ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());


            ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
            ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");

            gridModels.DataModel = models;
            return PartialView("ListFDRRenewalNote", gridModels);
        }

        /// <summary>      
        /// A new Fixed Deposit is created of Renewal FDR Note .But in Renewal FDR Note FixedDeposit_Reference Remain previous parents
        /// so replace newly created FixedDeposit Reference key as given FDR NOTE FixedDeposit_Reference
        ///   //replace FixedDeposit_Reference by Rakibul <Date 7th March,2016>
        /// saveFDRNote.FIXEDDEPOSIT_REFERENCE = NewDeposit.REFERENCE;  //off replacing date 13-10-2016
        ///
        /// </summary>

        //[HttpGet]
        public ActionResult ApproveFDRRenewalNote(string reference)
        {
            //return if session closed
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    FDRNOTE saveFDRNote = db.FDRNOTEs.Where(app => app.REFERENCE == reference).FirstOrDefault();

                    //Update old FIXED Deposit(Deposit that to be Renewd )
                    FIXEDDEPOSIT oldFIXDeposit = db.FIXEDDEPOSITs.Where(t => t.REFERENCE == saveFDRNote.FIXEDDEPOSIT_REFERENCE).SingleOrDefault();

                    //This FDR Now renewed so update LastUpdated field
                    oldFIXDeposit.LASTUPDATED = DateTime.Now;
                 
                    oldFIXDeposit.STATUS = ConstantVariable.STATUS_RENEWED;   
                    db.Entry(oldFIXDeposit).State = EntityState.Modified;



                    //Because All Renewal FDR with pending status of RenewalNote list has some null value.RenewalDepositNumber=null,Status=Approved/Encashed
                    //this is the new Deposit while we Approved in FixedDeposit already a deposit create Status=null,DepositNumber=null and RenewdDepositNumber=saveFDRNote.FdrNumber
                    //we should get this Deposit entity hear
                    FIXEDDEPOSIT NewDeposit = db.FIXEDDEPOSITs.Where(t => t.STATUS == null && t.DEPOSITNUMBER == null && t.RENEWALDEPOSITNUMBER == saveFDRNote.FDRNUMBER).SingleOrDefault();

                    //Edit Newly created Deposit status ED and OC
                    NewDeposit.CREATEDBY = Session["UserId"].ToString();
                    NewDeposit.CREATEDDATE = DateTime.Now;
                    NewDeposit.LASTUPDATED = DateTime.Now;
                    NewDeposit.LASTUPDATEDBY = Session["UserId"].ToString();

                    //*************************//
                    NewDeposit.STATUS = ConstantVariable.STATUS_PENDING;
                  
                    //************************//


                    db.Entry(NewDeposit).State = EntityState.Modified;
                                      

                    //Update FDR Note                 
                    saveFDRNote.APPROVEDBY = Session["UserId"].ToString();
                    saveFDRNote.APPROVEDDATE = DateTime.Now; // DateTime.Today;
                    saveFDRNote.PROPOSEDACTION = ConstantVariable.STATUS_RENEWED;
                    saveFDRNote.STATUS = ConstantVariable.STATUS_APPROVED; // "Approved";    

                   //Replace Old Fixed Deposit here is now off
                    new CommonFunction().CustomObjectNullValidation<FDRNOTE>(ref saveFDRNote);

                    db.Entry(saveFDRNote).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListFixedDeposit", "FixedDeposit", new { lblbreadcum = "Fixed Deposit" });
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        [HttpGet]
        public ActionResult EditFDRRenewalNote(string fdrRef)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                Entities db = new Entities(Session["Connection"] as EntityConnection);

                //This Filed tenure,terms,compound,matured date .... need to be edit
                FDRNOTE saveFDRNote = db.FDRNOTEs.Where(t => t.REFERENCE == fdrRef).SingleOrDefault();
            
                //get parent Fixed Deposit thow this  FDR contain Parent Fixed Deposit Ref(until Approved) not New FixedDeposit that is created
                //with DepositNumber=Status= null .This Fixed deposit should find out for Edit

                FIXEDDEPOSIT oldFIXDeposit = db.FIXEDDEPOSITs.Where(t => t.REFERENCE == saveFDRNote.FIXEDDEPOSIT_REFERENCE).SingleOrDefault(); 
                //now get newly created FixedDeposit 

                FIXEDDEPOSIT newDeposit = db.FIXEDDEPOSITs.Where(t => t.INITIALFIXEDDEPOSITREF ==  t.DEPOSITNUMBER == null && t.STATUS == null && t.RENEWALDEPOSITNUMBER == saveFDRNote.FDRNUMBER).SingleOrDefault();

                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "TenureTerm").FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();

                var interestType = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var COMPOUNDINTERESTTYPE = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestType).OrderBy(app => app.DESCRIPTION).ToList();

                var ItemToRemove = COMPOUNDINTERESTTYPE.SingleOrDefault(t => t.DESCRIPTION == "None");
                COMPOUNDINTERESTTYPE.Remove(ItemToRemove);

                var interval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
                var ComoundInterestInterval = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interval).OrderBy(app => app.DESCRIPTION).ToList();

                ItemToRemove = ComoundInterestInterval.SingleOrDefault(t => t.DESCRIPTION == "Daily");
                ComoundInterestInterval.Remove(ItemToRemove);



                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION");
                ViewBag.COMPOUNDINTERESTTYPEList = new SelectList(COMPOUNDINTERESTTYPE, "DESCRIPTION", "DESCRIPTION");
                ViewBag.ComoundInterestInterval = new SelectList(ComoundInterestInterval, "DESCRIPTION", "DESCRIPTION");

              
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Renew " + Session["currentPage"];

                //This two record need to be Update 
                ViewBag.NewDeposit = newDeposit; 
                ViewBag.RenewalFDR = saveFDRNote;

                ViewBag.ParentPrincipalAmount = oldFIXDeposit.PRINCIPALAMOUNT;

                return PartialView("EditFDRRenewalNote",newDeposit);             
              
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            
            }
        
        
        }

        [HttpPost]
        public ActionResult EditFDRRenewalNote(FIXEDDEPOSIT RenewModel,string ContactPerson)
        { 
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                var db = new Entities(Session["Connection"] as EntityConnection);

                int Tenure = (int)RenewModel.TENURE;

                FDRNOTE RenewFRD =(FDRNOTE)TempData["RenewalFDR"];
                FIXEDDEPOSIT NewDeposit = (FIXEDDEPOSIT)TempData["NewDeposit"];     


               //This Filed tenure,terms,compound,matured date .... need to be edit
                FDRNOTE saveFDRNote = db.FDRNOTEs.Where(t => t.REFERENCE == RenewFRD.REFERENCE).SingleOrDefault();

                saveFDRNote.TENURE = RenewModel.TENURE;
                saveFDRNote.TENURETERM = RenewModel.TENURETERM;                
                saveFDRNote.PROPOSEDRATE = RenewModel.RATEOFINTEREST;
                
                saveFDRNote.CONTACTPERSON = ContactPerson;
                saveFDRNote.SIGNATORY1 = RenewModel.SIGNATORY1;
                saveFDRNote.SIGNATORY2 = RenewModel.SIGNATORY2;
                
                saveFDRNote.INTERESTMODE = RenewModel.INTERESTMODE;                
                saveFDRNote.COMPOUNDINTERESTINTERVAL = RenewModel.INTERESTMODE == ConstantVariable.INTERESTMODE_FLAT ? "" : RenewModel.COMPOUNDINTERESTINTERVAL;

                saveFDRNote.ANNUALDAYS = (int)RenewModel.ANNUALDAYS.Value;
                db.Entry(saveFDRNote).State = EntityState.Modified;                

                FIXEDDEPOSIT editDeposit = db.FIXEDDEPOSITs.Where(t => t.REFERENCE == RenewModel.REFERENCE).SingleOrDefault();

                editDeposit.TENURE = RenewModel.TENURE;
                editDeposit.TENURETERM = RenewModel.TENURETERM;
                editDeposit.RATEOFINTEREST = RenewModel.RATEOFINTEREST;

                editDeposit.INTERESTMODE = RenewModel.INTERESTMODE;
                editDeposit.COMPOUNDINTERESTINTERVAL = RenewModel.INTERESTMODE == ConstantVariable.INTERESTMODE_FLAT ? "" : RenewModel.COMPOUNDINTERESTINTERVAL;

                editDeposit.SIGNATORY1 = RenewModel.SIGNATORY1;
                editDeposit.SIGNATORY2 = RenewModel.SIGNATORY2;
             
                editDeposit.ANNUALDAYS = RenewModel.ANNUALDAYS;
                editDeposit.TERMSINDAYS = RenewModel.ANNUALDAYS;

                /********Matured Date Calculation******/
                if (RenewModel.TENURETERM == ConstantVariable.TENURETERM_YEARS)
                 editDeposit.MATURITYDATE = editDeposit.OPENINGDATE.Value.AddYears(Tenure);
                else if (RenewModel.TENURETERM == ConstantVariable.TENURETERM_MONTHS)
                    editDeposit.MATURITYDATE = editDeposit.OPENINGDATE.Value.AddMonths(Tenure);
                else if (RenewModel.TENURETERM == ConstantVariable.TENURETERM_DAYS)
                    editDeposit.MATURITYDATE = editDeposit.OPENINGDATE.Value.AddDays(Tenure);
                else { }

                db.Entry(editDeposit).State = EntityState.Modified;
                db.SaveChanges();
                
                return RedirectToAction("ListFDRRenewalNote", "FDRNote", new { lblbreadcum = "Renewal Notes" });
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }




        [HttpPost]
        public ActionResult PrintRenewalNoteList(string reference, string appStatus = null, string penStatus = null, string GivenDate=null)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }


            Entities db = new Entities(Session["Connection"] as EntityConnection);

            List<FDRNOTE> fdrnoteList = new List<FDRNOTE>();
            ReportDataSource oFDRNotes = new ReportDataSource();

            List<SIGNATORYDETAIL> SignatoryDetails = new List<SIGNATORYDETAIL>();

            DateTime date = Convert.ToDateTime(GivenDate);

            SignatoryDetails = db.SIGNATORies
                         .Where(p => p.SIGNATORYDETAILS.Any(c => c.Status == "Active" && c.FromDate <= date && c.ToDate >= date) && p.Status == "Active" && p.Code == "FDRRenewalNote")
                         .SelectMany(t => t.SIGNATORYDETAILS).ToList();


            SignatoryDetails = SignatoryDetails.Where(t => t.ToDate >= date && t.FromDate.Value <= date && t.Status == "Active").OrderBy(t => t.ORDERINDEX).ToList();


            var signatory = (from s in SignatoryDetails
                             select new
                             {
                                 Title = s.TITLE,
                                 Name = s.SignatureLine1 + "\n" + s.SignatureLine2
                             }).ToList();
           

            string fiRef = TempData["FINANCIALINSTITUTION_REFERENCE"] as string;



            string reportTitle = "Approved Renewal Note"; ;
            string reportName = "ApprovedRenewalNoteList.pdf";
            string reportStatus = "Approved";

            //if (!string.IsNullOrEmpty(appStatus))
            //{
            //    //reportStatus = "Approved"; 
            //    reportTitle = "Approved Encashment Note";
            //    reportName = "ApprovedEncashmentNote.pdf";
            //}
            if (!string.IsNullOrEmpty(penStatus))
            {
                reportStatus = "Pending";
                reportTitle = "Pending Renewal Note";
                reportName = "PendingRenewalNoteList.pdf";
            }
            fdrnoteList = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_RENEWAL && note.STATUS == reportStatus).OrderBy(pi => pi.CREATEDDATE).ToList();
            // fdrnoteList =new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").Where(p => p.STATUS == reportStatus && p.NOTETYPE ="Encashed").Take 


            if (!string.IsNullOrEmpty(fiRef))
            {

                fdrnoteList = fdrnoteList.Where(fdrnote => fdrnote.FINANCIALINSTITUTION_REFERENCE == fiRef).ToList();
            }
            else if (!string.IsNullOrEmpty(reference) && reference != null)
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
                                     TENURE = Convert.ToString(fdrNote.TENURE.Value),
                                     TENURETERM = fdrNote.TENURETERM,
                                     PRINCIPALAMOUNT = String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C2}", Convert.ToDecimal(db.FIXEDDEPOSITs.Where(t => t.REFERENCE == fdrNote.FIXEDDEPOSIT_REFERENCE).SingleOrDefault().PRINCIPALAMOUNT.Value)).Replace("$", string.Empty),

                                     CAPLIMIT = Convert.ToString(fdrNote.CAPLIMIT.Value),
                                     PRVOFFERRATE = Convert.ToString(db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.REFERENCE == fdrNote.FIXEDDEPOSIT_REFERENCE).SingleOrDefault().RATEOFINTEREST.Value),

                                     PROPOSEDRATE = Convert.ToString(fdrNote.PROPOSEDRATE.Value) + "%\n" + (fdrNote.INTERESTMODE == ConstantVariable.INTERESTMODE_COMPOUND ? "(" + fdrNote.COMPOUNDINTERESTINTERVAL + " com)" : ""),

                                     MATURITYDATE = db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.REFERENCE == fdrNote.FIXEDDEPOSIT_REFERENCE).SingleOrDefault().MATURITYDATE.Value.ToString("dd-MMM-yy"),
                                     maturedMonthYear = db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.REFERENCE == fdrNote.FIXEDDEPOSIT_REFERENCE).SingleOrDefault().MATURITYDATE.Value.ToString("MMMM ,yyyy"),

                                     PROPOSEDACTION = "Renew for \n" + fdrNote.TENURE + " " + fdrNote.TENURETERM



                                 };

            // string matureddateinword = newfdrNoteList.ToList().First(0).MATURITYDATE.ToString();
            LocalReport lr = new LocalReport();

            ReportDataSource rd = new ReportDataSource();
            ReportDataSource dd = new ReportDataSource();

            DataTable dtFDRNote = oCommonFunction.ConvertToDataTable(newfdrNoteList.ToList());
            rd.Name = "RenewNoteList";   // "PurchaseNoteList";
            rd.Value = dtFDRNote;

            dd.Name = "SignatoryList";
            dd.Value = oCommonFunction.ConvertToDataTable(signatory.ToList());

            ReportParameter[] parameters = new ReportParameter[] 
            {
             new ReportParameter("Todays",GivenDate),            //DateTime.Today.ToString("dd-MMM-yy")
             new ReportParameter("Status","")
            };


            lr.ReportPath = Server.MapPath("~/Reports/FDRRenewalNote.rdlc");
            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);
            lr.DataSources.Add(dd);

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

        [HttpPost]
        public ActionResult PrintRenewalNoteLetter(string reference, string GivenDate)
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

                string fiRef = TempData["FINANCIALINSTITUTION_REFERENCE"] as string;
                fdrnoteList = new Entities(Session["Connection"] as EntityConnection).FDRNOTEs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(note => note.REFERENCE == reference).OrderBy(pi => pi.CREATEDDATE).ToList();


                if (!string.IsNullOrEmpty(fiRef))
                {
                    fdrnoteList = fdrnoteList.Where(fdrnote => fdrnote.FINANCIALINSTITUTION_REFERENCE == fiRef).ToList();
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

                //while pending we can get FixedDepost by fdrnote FixedDepostREf
                var list = db.FIXEDDEPOSITs.ToList();

                var fixedDepositList = (from fd in db.FIXEDDEPOSITs.ToList()
                                        from fr in fdrnoteList
                                        where fd.REFERENCE == fr.FIXEDDEPOSIT_REFERENCE   //while status pending
                                        //   where fd.DEPOSITNUMBER == fr.FDRNUMBER && fd.STATUS == ConstantVariable.STATUS_RENEWED
                                        select new
                                        {
                                            Principal = fd.PRINCIPALAMOUNT,
                                            OpeningDate = fd.OPENINGDATE,
                                            maturedDate = fd.MATURITYDATE

                                        }).ToList();

                var newfdrNoteList = from fdrNote in fdrnoteList
                                     from fixedDepo in fixedDepositList
                                     select new
                                     {

                                         NAME = fdrNote.FINANCIALINSTITUTION.NAME,
                                         BRANCH = fdrNote.FIBRANCH.NAME + " Branch\n" + fdrNote.FIBRANCH.ADDRESSLINE1 + "\n" + fdrNote.FIBRANCH.ADDRESSLINE2,
                                         FDRNUMBER = fdrNote.FDRNUMBER,  //18 digit in one line fdrNote.FDRNUMBER

                                         MATURITYDATE = fixedDepo.maturedDate,                                        
                                         TENURE = fdrNote.TENURE,
                                         TENURETERM = fdrNote.TENURETERM,

                                         PRINCIPALAMOUNT = fixedDepo.Principal,
                                         EXISTINGDEPOSIT = fdrNote.EXISTINGDEPOSIT,
                                         PERCENTAGEOFFDR = fdrNote.PERCENTAGEOFFDR,
                                         CAPLIMIT = fdrNote.CAPLIMIT,
                                         OFFERRATE = fdrNote.OFFERRATE,
                                         PROPOSEDRATE = fdrNote.PROPOSEDRATE,
                                         CONTACTPERSON = fdrNote.CONTACTPERSON,
                                         CHEQUENO = fdrNote.CHEQUENO,

                                         //display this deposit opened date or IssueDate not Checque Date we get it from Fixed Deposit
                                         CHEQUEDATE = fixedDepo.OpeningDate,
                                        
                                         //if diposit was compounding displya with interval else disply none
                                         STATUS = fdrNote.INTERESTMODE == ConstantVariable.INTERESTMODE_COMPOUND ? " " + fdrNote.COMPOUNDINTERESTINTERVAL.ToLower() + " compounding" : ""
                                     };



                //string Reportbody = "We are pleased to inform you that the management wishes to renew the Fix Deposit Account No. " + newfdrNoteList.First().FDRNUMBER;
                //Reportbody += "";

                LocalReport lr = new LocalReport();

                ReportDataSource rd = new ReportDataSource();
                //ReportDataSource dd = new ReportDataSource();

                DataTable dtFDRNote = oCommonFunction.ConvertToDataTable(newfdrNoteList.ToList());
                rd.Name = "PurchaseNoteList";
                rd.Value = dtFDRNote;

                //var fdrnumber = newfdrNoteList.SingleOrDefault().FDRNUMBER;

                //string FdrNumber1 = !String.IsNullOrWhiteSpace(fdrnumber) && fdrnumber.Length >= 18? fdrnumber.Substring(0, 18) : fdrnumber;
                //string FdrNumber2 = !String.IsNullOrWhiteSpace(fdrnumber) && fdrnumber.Length > 18 ? fdrnumber.Substring(18) : ""; 
                
                ReportParameter[] parameters = new ReportParameter[] 
                {
                 new ReportParameter("date",GivenDate),
               //  new ReportParameter("FdrNumber1",FdrNumber1),
               //  new ReportParameter("FdrNumber2",FdrNumber2)
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

                string reportName = fdrnoteList[0].FINANCIALINSTITUTION.NAME + "-RenewalNote.pdf";

                return File(renderedBytes, mimeType, reportName);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        /// <summary>
        /// Added by Rakibul <12/01/2016>
        /// </summary>       
        #region Helper Method

        public decimal GetGrettestOfferRate(decimal oferrate1, decimal offerrate2, decimal offerrate3)
        {
            try
            {
                return System.Math.Max(System.Math.Max(oferrate1, offerrate2), offerrate3);
            }
            catch
            {

            }
            return -1;
        }
        #endregion
    }

    public class OfferRate
    {
        public Nullable<decimal> Offer_Rate { get; set; }
        public Nullable<decimal> Tenure { get; set; }
        public string Terms { get; set; }
        public string InterestMode { get; set; }
    }
}
