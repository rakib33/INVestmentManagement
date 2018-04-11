using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Data;
using InvestmentManagement.App_Code;
using InvestmentManagement.ViewModel;

using CrystalDecisions.CrystalReports.Engine;
using System.IO;
//using CrystalDecisions.Shared;
using System.Data.Objects;

using System.Data.OracleClient;



namespace InvestmentManagement.Controllers
{
    public class ReportController : Controller
    {
        //https://stackoverflow.com/questions/29609607/export-report-data-to-excel
        string message = "";

        CommonFunction oCommonFunction = new CommonFunction();

        // GET: /Report/
        [HttpGet]
        public ActionResult ListReport()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");                            
            }

            ViewBag.FINANCIALINSTITUTIONList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(p => p.NAME).ToList(), "REFERENCE", "NAME");

            var tPDetailRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(w => w.ENTITY.ToLower() == "Fixed Deposit".ToLower() && w.PROPERTY.ToLower() == "TenureTerm".ToLower()).FirstOrDefault().REFERENCE.ToString();
            var termList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(w => w.APPLICATIONPARAMETER_REFERENCE == tPDetailRef).ToList();
            ViewBag.termList = new SelectList(termList, "DESCRIPTION", "DESCRIPTION");

            var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();

            ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");           
            ViewBag.InstrumentList = new SelectList(InstrumentList, "SHORTNAME", "SHORTNAME");
            ViewBag.currentPage = "Report List";
            return PartialView();
        }

        #region FDR_REPORT

        /*
        this is FDR Main Statement Report
        ReceivableTill Calculation Formula
        HoldingPeriod = Total days between Fixed Deposit Opening date and todays date or search date
        TotalDays = total days between IssueDate and Matured Date
        Formula ReceivableTill= (NetInterestReceivable / TotalDays) * HoldingPeriod
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
        }
       */

        public ActionResult FDRStatementReportByDate(DateTime? fromDateFS, DateTime? toDateFS, string FI_Ref, string HasHistory, int? TENURE, string TERMS, string IsExcell)
        {
            List<FDRMainViewModel> FDList = new List<FDRMainViewModel>();
            List<FIXEDDEPOSIT> models;
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                Entities db = new Entities(Session["Connection"] as EntityConnection);

                toDateFS = DateTime.Now.Date;

                models = new List<FIXEDDEPOSIT>();

                if (toDateFS == null)
                {
                    return RedirectToAction("Index", "ErrorPage", new { message = "From date or To date is Required!!" });
                }
                //Which FDR was Approved withing begining and toDateFs/ search date they were Approved/Pending

                //models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr => (fdr.STATUS == ConstantVariable.STATUS_APPROVED || fdr.STATUS ==ConstantVariable.STATUS_PENDING) && 
                //    (EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value) <= EntityFunctions.TruncateTime(toDateFS))).OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();

                //Test Purpose according Burhan Vai
                models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr => (fdr.STATUS == ConstantVariable.STATUS_APPROVED || fdr.STATUS == ConstantVariable.STATUS_PENDING) &&
                 (EntityFunctions.TruncateTime(toDateFS) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value))
                  &&                  
                 (EntityFunctions.TruncateTime(toDateFS) < EntityFunctions.TruncateTime(fdr.MATURITYDATE.Value))).OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();
                


                //(fdr.PROPOSEDACTION ==null || fdr.PROPOSEDACTION=="") added 14-Dec-17
                                
                if (!string.IsNullOrEmpty(FI_Ref) && FI_Ref != null)
                {
                    models = models.Where(t => t.FINANCIALINSTITUTION_REFERENCE == FI_Ref).ToList();
                }

                if (!string.IsNullOrEmpty(TERMS) && TERMS != null && TENURE != null && TENURE > 0)
                {

                    if ((TERMS == ConstantVariable.TENURETERM_MONTHS && TENURE == 12) || (TERMS == ConstantVariable.TENURETERM_YEARS && TENURE == 1)) //acording burhan vai 14-06-17
                    {
                        models = models.Where(t => t.TENURE == 12 || t.TENURE == 1 && (t.TENURETERM == ConstantVariable.TENURETERM_MONTHS || t.TENURETERM == ConstantVariable.TENURETERM_YEARS)).ToList();
                    }
                    else
                    {
                        models = models.Where(t => t.TENURE == TENURE && t.TENURETERM == TERMS).ToList();
                    }
                }

                ReportDataSource oFDRStatement = new ReportDataSource();

                if (models != null)
                {
                    try
                    {
                        //now create a FixedDeposit List list 
                        string FinancialInstitution_Ref = "";
                        decimal TotalPresentPrincipal = 0;
                        decimal TotalPrincipal = 0;
                        decimal ReceivableTill = 0;
                        decimal AVGRateOfInterest = 0;
                        decimal ReceivableUp = 0;
                        int NumberCount = 0;
                        int initialrow = 0;
                        decimal HoldingPeriod = 0;
                        int slNo = 0;
                        foreach (var item in models)
                        {

                            //assign FI_Ref if not aexists
                            if (FinancialInstitution_Ref != item.FINANCIALINSTITUTION_REFERENCE)
                            {
                                FinancialInstitution_Ref = item.FINANCIALINSTITUTION_REFERENCE;
                                NumberCount = 0;
                                initialrow = 0;
                                //Count number of a sepecific bank
                                NumberCount = (from e in models where e.FINANCIALINSTITUTION_REFERENCE == FinancialInstitution_Ref select e).ToList().Count();

                                TotalPresentPrincipal = 0;
                                TotalPrincipal = 0;
                                ReceivableTill = 0;
                                AVGRateOfInterest = 0;

                            }

                            if (FinancialInstitution_Ref == item.FINANCIALINSTITUTION_REFERENCE)
                            {
                                initialrow++;

                                slNo++;
                                //Holding Period
                                HoldingPeriod = Convert.ToDecimal((toDateFS.Value.Date - item.OPENINGDATE.Value.Date).TotalDays);

                                //Calculate receivable Till hear it calculate up on Gross not NetInterest Receivable according DLIC FDR department
                                ReceivableTill = 0; //first assign 0 then claculate 
                                if (item.GROSSINTEREST != null)
                                {
                                    decimal TotalDays = Convert.ToDecimal((item.MATURITYDATE.Value - item.OPENINGDATE.Value).TotalDays); //if renew then renewal Date                                           
                                    ReceivableTill = (item.GROSSINTEREST.Value / TotalDays) * HoldingPeriod;

                                    //Receibale For the period Up to period
                                    //Up To period =Gross Interest of (maturityDate-OpeningDate) Days=GrossInterest
                                    //For the period= get openingDate and maturityDate years if years same then GrossInterest
                                    //if not then create a date 01-Jan-MaturityYear as lastTime
                                    //then calculate total days Todays- lastTime as lastSlotDays
                                    //ReceivableUpPeriod= (item.GROSSINTEREST.Value / TotalDays)* lastSlotDays;     

                                    int RenewedIssuedYear = item.OPENINGDATE.Value.Year;
                                    int SearchedDateYear = toDateFS.Value.Year;
                                    //item.MATURITYDATE.Value.Year;

                                    if (RenewedIssuedYear == SearchedDateYear)
                                        ReceivableUp = ReceivableTill;
                                    //item.GROSSINTEREST.Value;
                                    else if (RenewedIssuedYear < SearchedDateYear)
                                    {
                                        DateTime SearcedDateYearFirstDate = new DateTime(SearchedDateYear, 01, 01);
                                        decimal lastYearSlotDays = Convert.ToDecimal((toDateFS.Value.Date - SearcedDateYearFirstDate).TotalDays);
                                        //Convert.ToDecimal((item.MATURITYDATE.Value - MaturityYearFirstDate).TotalDays); //Total days of last year slot                                           

                                        ReceivableUp = (item.GROSSINTEREST.Value / TotalDays) * lastYearSlotDays;
                                    }
                                    else
                                        ReceivableUp = 0;
                                }

                                TotalPrincipal += item.RENEWALDEPOSITNUMBER == null ? item.PRINCIPALAMOUNT.Value : item.INITIALPRINCIPALAMOUNT; //same as parent deposit if renewed
                                //item.PRINCIPALAMOUNT ==null? 0 : item.PRINCIPALAMOUNT.Value;

                                TotalPresentPrincipal += item.STATUS == ConstantVariable.STATUS_APPROVED ? item.PRINCIPALAMOUNT.Value : item.PRESENTPRINCIPALAMOUNT.Value;
                                //item.PRESENTPRINCIPALAMOUNT ==null ? 0 :item.PRESENTPRINCIPALAMOUNT.Value;
                                //item.PRINCIPALAMOUNT == null ? 0 : item.PRINCIPALAMOUNT.Value;

                                AVGRateOfInterest += item.RATEOFINTEREST == null ? 0 : item.RATEOFINTEREST.Value;

                                //add data into list
                                FDList.Add(new FDRMainViewModel
                                {

                                    SlNo = slNo.ToString(),
                                    FDRNo = item.DEPOSITNUMBER,
                                    BankName = item.FINANCIALINSTITUTION.NAME + "\n" + item.FIBRANCH.NAME,

                                    //if RENEWALDEPOSITNUMBER=null then this FDR does not renewed so its openingdate and principal amount remain same
                                    //else this FDR is renewed so its opening Date and Principal Amount will be parent FDR opening Date and Principal Amount
                                    //that is store in INITIALOPENINGDATE and INITIALPRINCIPALAMOUNT

                                    OpeningDate = item.RENEWALDEPOSITNUMBER == null ? item.OPENINGDATE.Value : item.INITIALOPENINGDATE.Value,  //same of parent deposit if renewed                                    
                                    PrincipalAmount = item.RENEWALDEPOSITNUMBER == null ? item.PRINCIPALAMOUNT : item.INITIALPRINCIPALAMOUNT, //same as parent deposit if renewed

                                    BankWisePA = null,
                                    RenewedDate = item.OPENINGDATE.Value,                                   
                                    Period = item.TENURE + " " + item.TENURETERM,

                                    MaturityDate = item.MATURITYDATE.Value,

                                    HoldingPeriod = HoldingPeriod,
                                    PresentPrincipalAmount = item.STATUS == ConstantVariable.STATUS_APPROVED ? item.PRINCIPALAMOUNT : item.PRESENTPRINCIPALAMOUNT,
                                    BankWisePPA = null,
                                    ExistingCAPLimit = null,
                                    PresentRateOfInterest = item.RATEOFINTEREST,
                                    ReceivableTill = ReceivableTill,
                                    ReceivableUp = ReceivableUp,
                                    Remarks = item.REMARKS
                                    //item.INTERESTMODE==ConstantVariable.INTERESTMODE_COMPOUND? item.COMPOUNDINTERESTINTERVAL+"\n Com.": null,
                                });


                                if (initialrow == NumberCount)
                                {
                                    //add extra row into the list                                    
                                    FDList.Add(new FDRMainViewModel
                                    {
                                        SlNo = null,
                                        FDRNo = null,
                                        BankName = null,
                                        OpeningDate = null,  //same of parent deposit if renewed
                                        PrincipalAmount = null, //same as parent deposit if renewed
                                        BankWisePA = TotalPrincipal,
                                        RenewedDate = null,
                                        Period = null,
                                        MaturityDate = null,
                                        HoldingPeriod = null,
                                        PresentPrincipalAmount = null,
                                        BankWisePPA = TotalPresentPrincipal,
                                        ExistingCAPLimit = item.FINANCIALINSTITUTION.CAPLIMIT,
                                        PresentRateOfInterest = Convert.ToDecimal(AVGRateOfInterest / NumberCount),
                                        ReceivableTill = null,
                                        ReceivableUp = null,
                                        Remarks = null
                                    });
                                }
                            }
                        }

                        LocalReport lr = new LocalReport();
                        lr.ReportPath = Server.MapPath("~/Reports/FDRMainStatement.rdlc");

                        //string report_name = "FDRMainStatement-"+ toDateFS.Value.ToString("dd-MMM-yy")+".pdf";

                        ReportDataSource rd = new ReportDataSource();

                        DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(FDList.ToList());
                        rd.Name = "FDRMainStatement";
                        rd.Value = dtFDRStatement;

                        ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("SearchDate",toDateFS.Value.ToString("dd-MMM-yy"))                                
                           };


                        lr.ReportPath = Server.MapPath("~/Reports/FDRMainStatement.rdlc");
                        lr.SetParameters(parameters);
                        lr.DataSources.Add(rd);


                        string ContentType = "application/vnd.ms-excel";
                        string FileType;
                        string ReportName;

                        if (IsExcell == "true")
                        {
                            FileType = "Excel";
                            ReportName = "FDRMainStatement-" + toDateFS.Value.ToString("dd-MMM-yy") + ".{0}";
                        }
                        else
                        {
                            FileType = "PDF";
                            ReportName = "FDRMainStatement-" + toDateFS.Value.ToString("dd-MMM-yy") + ".pdf";

                        }
                        

                        string reportType = FileType; // "PDF";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;

                        string deviceInfo =

                        "<DeviceInfo>" +
                            "  <OutputFormat>" + FileType + "</OutputFormat>" +
                            "  <PageWidth>12.5in</PageWidth>" +
                            "  <PageHeight>11in</PageHeight>" +
                            "  <MarginTop>0.5in</MarginTop>" +
                            "  <MarginLeft>0.75in</MarginLeft>" +
                            "  <MarginRight>0.25in</MarginRight>" +
                            "  <MarginBottom>0.5in</MarginBottom>" +
                            "</DeviceInfo>";

                        Warning[] warnings;
                        string[] streams;
                        byte[] renderedBytes;


                        if (IsExcell == "true")
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
                        return RedirectToAction("Index", "ErrorPage", new { message = Convert.ToString(ex.Message) });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "ErrorPage", new { message = "Sorry, we can't find any record you searched!!" });
                }               
            }
            catch (Exception ex)
            {
                string message = ex.Message + " Inner Error: " + ex.InnerException.Message; 
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        public ActionResult FDRStatementReportHistoryByDate(DateTime? fromDateFS, DateTime toDateFS, string FI_Ref, string HasHistory, int? TENURE, string TERMS, string IsExcell)
        {
            List<FDRMainViewModel> FDList = new List<FDRMainViewModel>();
            List<FIXEDDEPOSIT> models;
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                var Searchdate = toDateFS.Date;
                Entities db = new Entities(Session["Connection"] as EntityConnection);


                models = new List<FIXEDDEPOSIT>();

                if (toDateFS == null)
                {
                    return RedirectToAction("Index", "ErrorPage", new { message = "From date or To date is Required!!" });
                }

                //1.FDR who are Approved by given search date
                //2.FDR ProposedAction =Encash check Approved/AcceptDate >= SearchDate <=EncashedDate (EncashDate is the MaturedDate now)
                //3.FDR ProposedAction = Renewal check Approved/AcceptDate >= SearchDate <=RenewalDate (RenewalDate is the MaturedDate now)
                //DateTime.Date cannot be converted to SQL. Use EntityFunctions.TruncateTime method to get date part in EF 5 for latest EF You should use DbFunctions.TruncateTime.
                // FDR Statement History wanted.Which FDR was Approved withing begining and toDateFs.May some one now Encashed/Renewed but within search date they were Approved.So we chcek  LASTUPDATED
             
                //models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION")
                //           .Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED ||fdr.STATUS == ConstantVariable.STATUS_PENDING)                           
                //           .Where(fdr => (fdr.STATUS == ConstantVariable.STATUS_RENEWED && fdr.STATUS != ConstantVariable.STATUS_APPROVED) ?
                //               (EntityFunctions.TruncateTime(toDateFS) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value) && EntityFunctions.TruncateTime(toDateFS) < EntityFunctions.TruncateTime(fdr.LASTUPDATED.Value)) :  //fdr.RENWALDATE.Value // EntityFunctions.TruncateTime(toDateFS) >= EntityFunctions.TruncateTime(fdr.ACCEPTEDDATE.Value) change AcceptedDate to OpeningDate //change fdr.LASTUPDATED.Value
                               
                //               (fdr.STATUS == ConstantVariable.STATUS_ENCASHED && fdr.STATUS != ConstantVariable.STATUS_APPROVED) ?
                //               (EntityFunctions.TruncateTime(toDateFS) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value) && EntityFunctions.TruncateTime(toDateFS) < EntityFunctions.TruncateTime(fdr.ENCASHMENTDATE.Value)) : //fdr.ENCASHMENTDATE.Value //fdr.LASTUPDATED.Value
                               
                //               EntityFunctions.TruncateTime(toDateFS) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value))  //this is for approved status //fdr.OPENINGDATE <= toDateFS                                                        
                //           .OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();

                #region ChangedCode<14-Dec-17>
                //models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION")
                //         .Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED)
                //         .Where(fdr => fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_RENEWAL ?
                //             (EntityFunctions.TruncateTime(toDateFS) >= EntityFunctions.TruncateTime(fdr.ACCEPTEDDATE.Value) && EntityFunctions.TruncateTime(toDateFS) < EntityFunctions.TruncateTime(fdr.LASTUPDATED)) :  //fdr.RENWALDATE.Value
                //             fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_ENCASH ?
                //             (EntityFunctions.TruncateTime(toDateFS) >= EntityFunctions.TruncateTime(fdr.ACCEPTEDDATE.Value) && EntityFunctions.TruncateTime(toDateFS) < EntityFunctions.TruncateTime(fdr.LASTUPDATED)) :  //fdr.ENCASHMENTDATE.Value

                //             EntityFunctions.TruncateTime(toDateFS) >= EntityFunctions.TruncateTime(fdr.ACCEPTEDDATE.Value))  //this is for approved status //fdr.OPENINGDATE <= toDateFS                                                        
                //         .OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();

                models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED || fdr.STATUS == ConstantVariable.STATUS_PENDING)
                        .Where(fdr => (EntityFunctions.TruncateTime(toDateFS) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE)) && (EntityFunctions.TruncateTime(toDateFS) < EntityFunctions.TruncateTime(fdr.MATURITYDATE)))   //fdr.RENWALDATE.Value
                             //this is for approved status //fdr.OPENINGDATE <= toDateFS                                                        
                        .OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();
    

                #endregion

                if (!string.IsNullOrEmpty(FI_Ref) && FI_Ref != null)
                {
                    models = models.Where(t => t.FINANCIALINSTITUTION_REFERENCE == FI_Ref).ToList();
                }

                if (!string.IsNullOrEmpty(TERMS) && TERMS != null && TENURE != null && TENURE > 0)
                {
                    models = models.Where(t => t.TENURE == TENURE && t.TENURETERM == TERMS).ToList();
                }

                ReportDataSource oFDRStatement = new ReportDataSource();

                if (models != null)
                {
                    try
                    {
                        //now create a FixedDeposit List list 
                        string FinancialInstitution_Ref = "";

                        decimal TotalPresentPrincipal = 0;
                        decimal TotalPrincipal = 0;
                        decimal ReceivableTill = 0;
                        decimal AVGRateOfInterest = 0;
                        decimal ReceivableUp = 0;
                        int NumberCount = 0;
                        int initialrow = 0;
                        decimal HoldingPeriod = 0;
                        int slNo = 0;
                        foreach (var item in models)
                        {

                            //assign FI_Ref if not aexists
                            if (FinancialInstitution_Ref != item.FINANCIALINSTITUTION_REFERENCE)
                            {
                                FinancialInstitution_Ref = item.FINANCIALINSTITUTION_REFERENCE;
                                NumberCount = 0;
                                initialrow = 0;
                                //Count number of a sepecific bank
                                NumberCount = (from e in models where e.FINANCIALINSTITUTION_REFERENCE == FinancialInstitution_Ref select e).ToList().Count();

                                TotalPresentPrincipal = 0;
                                TotalPrincipal = 0;
                                ReceivableTill = 0;
                                AVGRateOfInterest = 0;

                            }

                            if (FinancialInstitution_Ref == item.FINANCIALINSTITUTION_REFERENCE)
                            {
                                initialrow++;

                                slNo++;
                                //Holding Period
                                HoldingPeriod = Convert.ToDecimal((toDateFS.Date - item.OPENINGDATE.Value.Date).TotalDays);

                                //Calculate receivable Till hear it calculate up on Gross not NetInterest Receivable according DLIC FDR department
                                ReceivableTill = 0; //first assign 0 then claculate 
                                if (item.GROSSINTEREST != null)
                                {
                                    decimal TotalDays = Convert.ToDecimal((item.MATURITYDATE.Value - item.OPENINGDATE.Value).TotalDays); //if renew then renewal Date                                           
                                    ReceivableTill = (item.GROSSINTEREST.Value / TotalDays) * HoldingPeriod;

                                    //Receibale For the period Up to period
                                    //Up To period =Gross Interest of (maturityDate-OpeningDate) Days=GrossInterest
                                    //For the period= get openingDate and maturityDate years if years same then GrossInterest
                                    //if not then create a date 01-Jan-MaturityYear as lastTime
                                    //then calculate total days Todays- lastTime as lastSlotDays
                                    //ReceivableUpPeriod= (item.GROSSINTEREST.Value / TotalDays)* lastSlotDays;     

                                    int RenewedIssuedYear = item.OPENINGDATE.Value.Year;
                                    int SearchedDateYear = toDateFS.Year;
                                    //item.MATURITYDATE.Value.Year;

                                    if (RenewedIssuedYear == SearchedDateYear)
                                        ReceivableUp = ReceivableTill;
                                    //item.GROSSINTEREST.Value;
                                    else if (RenewedIssuedYear < SearchedDateYear)
                                    {
                                        DateTime SearcedDateYearFirstDate = new DateTime(SearchedDateYear, 01, 01);
                                        decimal lastYearSlotDays = Convert.ToDecimal((toDateFS.Date - SearcedDateYearFirstDate).TotalDays);
                                        //Convert.ToDecimal((item.MATURITYDATE.Value - MaturityYearFirstDate).TotalDays); //Total days of last year slot                                           

                                        ReceivableUp = (item.GROSSINTEREST.Value / TotalDays) * lastYearSlotDays;
                                    }
                                    else
                                        ReceivableUp = 0;


                                }



                                TotalPrincipal += item.RENEWALDEPOSITNUMBER == null ? item.PRINCIPALAMOUNT.Value : item.INITIALPRINCIPALAMOUNT; //same as parent deposit if renewed
                               

                                //Point1:added 29-May-17
                                //DateTime openingdate2 = item.RENEWALDEPOSITNUMBER == null ? item.OPENINGDATE.Value : item.INITIALOPENINGDATE.Value; 
                                TotalPresentPrincipal += item.PRINCIPALAMOUNT.Value;
                                    //item.STATUS == ConstantVariable.STATUS_APPROVED ? item.PRINCIPALAMOUNT.Value : item.PRESENTPRINCIPALAMOUNT.Value;
                               

                                AVGRateOfInterest += item.RATEOFINTEREST == null ? 0 : item.RATEOFINTEREST.Value;

                                //add data into list
                                FDList.Add(new FDRMainViewModel
                                {

                                    SlNo = slNo.ToString(),
                                    FDRNo = item.DEPOSITNUMBER,
                                    BankName = item.FINANCIALINSTITUTION.NAME + "\n" + item.FIBRANCH.NAME,

                                    //if RENEWALDEPOSITNUMBER=null then this FDR does not renewed so its openingdate and principal amount remain same
                                    //else this FDR is renewed so its opening Date and Principal Amount will be parent FDR opening Date and Principal Amount
                                    //that is store in INITIALOPENINGDATE and INITIALPRINCIPALAMOUNT

                                    OpeningDate = item.RENEWALDEPOSITNUMBER == null ? item.OPENINGDATE.Value: item.INITIALOPENINGDATE.Value,  //same of parent deposit if renewed                                    
                                    PrincipalAmount = item.RENEWALDEPOSITNUMBER == null ? item.PRINCIPALAMOUNT : item.INITIALPRINCIPALAMOUNT, //same as parent deposit if renewed

                                    BankWisePA = null,
                                    RenewedDate = item.OPENINGDATE.Value,      //.ToString("dd-MMM-yy"),
                                    //item.RENWALDATE == null ? item.OPENINGDATE.Value.ToString("dd-MMM-yy") : item.RENWALDATE.Value.ToString("dd-MMM-yy"),
                                    Period = item.TENURE + " " + item.TENURETERM,

                                    MaturityDate = item.MATURITYDATE.Value, //item.MATURITYDATE == null ? null :item.MATURITYDATE.ToString("dd-MMM-yy"),

                                    HoldingPeriod = HoldingPeriod,
                                    //Point1:added 29-May-17
                                    PresentPrincipalAmount = item.PRINCIPALAMOUNT.Value,
                                                           //item.STATUS == ConstantVariable.STATUS_APPROVED ? item.PRINCIPALAMOUNT : item.PRESENTPRINCIPALAMOUNT,
                                    BankWisePPA = null,
                                    ExistingCAPLimit = null,
                                    PresentRateOfInterest = item.RATEOFINTEREST,
                                    ReceivableTill = ReceivableTill,
                                    ReceivableUp = ReceivableUp,
                                    Remarks = item.REMARKS
                                    //item.INTERESTMODE==ConstantVariable.INTERESTMODE_COMPOUND? item.COMPOUNDINTERESTINTERVAL+"\n Com.": null,
                                });


                                if (initialrow == NumberCount)
                                {
                                    //add extra row into the list                                    
                                    FDList.Add(new FDRMainViewModel
                                    {
                                        SlNo = null,
                                        FDRNo = null,
                                        BankName = null,
                                        OpeningDate =null ,  //same of parent deposit if renewed
                                        PrincipalAmount = null, //same as parent deposit if renewed
                                        BankWisePA = TotalPrincipal,
                                        RenewedDate = null,
                                        Period = null,
                                        MaturityDate = null,
                                        HoldingPeriod = null,
                                        PresentPrincipalAmount = null,
                                        BankWisePPA = TotalPresentPrincipal,
                                        ExistingCAPLimit = item.FINANCIALINSTITUTION.CAPLIMIT,
                                        PresentRateOfInterest = Convert.ToDecimal(AVGRateOfInterest / NumberCount),
                                        ReceivableTill = null,
                                        ReceivableUp = null,
                                        Remarks = null
                                    });
                                }
                            }
                        }

                        LocalReport lr = new LocalReport();
                        lr.ReportPath = Server.MapPath("~/Reports/FDRMainStatementHistory.rdlc");

                      
                        ReportDataSource rd = new ReportDataSource();

                        DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(FDList.ToList());
                        rd.Name = "FDRMainStatement";
                        rd.Value = dtFDRStatement;

                        ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("SearchDate",toDateFS.ToString("dd-MMM-yy"))                                
                           };


                        lr.ReportPath = Server.MapPath("~/Reports/FDRMainStatementHistory.rdlc");
                        lr.SetParameters(parameters);
                        lr.DataSources.Add(rd);


                        string ContentType = "application/vnd.ms-excel";
                        string FileType;
                        string ReportName;

                        if (IsExcell == "true")
                        {
                            FileType = "Excel";
                            ReportName = "FDRMainStatementHistory-" + toDateFS.ToString("dd-MMM-yy") + ".{0}";
                        }
                        else
                        {
                            FileType = "PDF";
                            ReportName = "FDRMainStatementHistory-" + toDateFS.ToString("dd-MMM-yy") + ".pdf";

                        }


                        string reportType = FileType; // "PDF";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;

                        string deviceInfo =

                        "<DeviceInfo>" +
                            "  <OutputFormat>" + FileType + "</OutputFormat>" +
                            "  <PageWidth>12.5in</PageWidth>" +
                            "  <PageHeight>11in</PageHeight>" +
                            "  <MarginTop>0.5in</MarginTop>" +
                            "  <MarginLeft>0.75in</MarginLeft>" +
                            "  <MarginRight>0.25in</MarginRight>" +
                            "  <MarginBottom>0.5in</MarginBottom>" +
                            "</DeviceInfo>";

                        Warning[] warnings;
                        string[] streams;
                        byte[] renderedBytes;


                        if (IsExcell == "true")
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
                        return RedirectToAction("Index", "ErrorPage", new { message = Convert.ToString(ex.Message) });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "ErrorPage", new { message = "Sorry, we can't find any record you searched!!" });

                }

                //return RedirectToAction("GenerateFDRRegisterReport", "FixedDepositRegister");
            }
            catch (Exception ex)
            {
                string message = ex.Message + " Inner Error: " + ex.InnerException.Message; ;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        public ActionResult FDRACHistoryByDate(string FDR_NO, string IsExcell)  //DateTime? toDate, string FI_Ref, 
        {
            List<FDRMainViewModel> FDList = new List<FDRMainViewModel>();
            List<FIXEDDEPOSIT> FDRAcHistoryList;

            int flag = 0;
            string fdrNo = null;
            string BankName = null;
            string BranchName = null;

            string InitialFDRRef;

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                Entities db = new Entities(Session["Connection"] as EntityConnection);




                if (FDR_NO != null && !string.IsNullOrEmpty(FDR_NO))
                {

                    //Step 01: get the list of matched FDR No from FixedDeposit and tack InitialFDRRef
                    //Step 02: get all list by matching InitialFDRRef and Order by Opening Date

                    //var check = from d in db.FIXEDDEPOSITs
                    //            where d.DEPOSITNUMBER == FDR_NO
                    //            select d;

                    FDRAcHistoryList = new List<FIXEDDEPOSIT>();
                    try
                    {
                        InitialFDRRef = db.FIXEDDEPOSITs.Where(t => t.DEPOSITNUMBER == FDR_NO).ToList().Take(1).SingleOrDefault().INITIALFIXEDDEPOSITREF;

                        FDRAcHistoryList = db.FIXEDDEPOSITs.Where(t => t.INITIALFIXEDDEPOSITREF == InitialFDRRef).OrderBy(t => t.OPENINGDATE).ToList();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == ConstantVariable.ERROR_OBJECT_REF_NOT_SET_OBJECT)
                        {
                            return RedirectToAction("Index", "ErrorPage", new { message = "Can not Find any record of FDR No " + FDR_NO });
                        }
                        else
                            return RedirectToAction("Index", "ErrorPage", new { message = "Initial Fixed Deposit ref/FixedDeposit List retriving Error!!" });
                    }
                    if (FDRAcHistoryList != null)
                    {
                        foreach (var item in FDRAcHistoryList)
                        {
                            if (flag == 0) //for first row
                            {
                                FDList.Add(new FDRMainViewModel
                                {

                                    FDRNo = item.DEPOSITNUMBER,
                                    BankName = item.FINANCIALINSTITUTION.NAME + "\n" + item.FIBRANCH.NAME,

                                    OpeningDate = item.INITIALOPENINGDATE.Value,  //.ToString("dd-MMM-yy"),  //same of parent deposit if renewed                                    
                                    PrincipalAmount = item.INITIALPRINCIPALAMOUNT, //same as parent deposit if renewed

                                    RenewedDate = item.OPENINGDATE.Value, // .ToString("dd-MMM-yy"),                  // update 01-12-2016
                                    //item.RENWALDATE == null ? item.OPENINGDATE.Value.ToString("dd-MMM-yy") : item.RENWALDATE.Value.ToString("dd-MMM-yy"),
                                    Period = item.TENURE + " " + item.TENURETERM,

                                    MaturityDate = item.MATURITYDATE.Value, //item.MATURITYDATE == null ? "" : item.MATURITYDATE.Value.ToString("dd-MMM-yy"),
                                    PresentRateOfInterest = item.RATEOFINTEREST,
                                    GrossInterest = item.GROSSINTEREST,
                                    SourceTax = item.SOURCETAX,
                                    ExciseDuty = item.EXCISEDUTY,
                                    OtherCharges = item.OTHERCHARGE,
                                    NetInterest = item.NETINTERESTRECEIVABLE,

                                    PresentPrincipalAmount = item.TOTALAMOUNTRECEIVABLE,
                                    //item.PRESENTPRINCIPALAMOUNT,
                                    Remarks = item.REMARKS
                                    //item.INTERESTMODE==ConstantVariable.INTERESTMODE_COMPOUND? item.COMPOUNDINTERESTINTERVAL+"\n Com.": null,
                                });

                                fdrNo = item.DEPOSITNUMBER;
                                BankName = item.FINANCIALINSTITUTION.NAME;
                                BranchName = item.FIBRANCH.NAME;

                                flag = 1;
                            }
                            else
                            {

                                FDList.Add(new FDRMainViewModel
                                {

                                    FDRNo = (fdrNo == item.DEPOSITNUMBER ? "" : item.DEPOSITNUMBER),
                                    BankName = (BankName == item.FINANCIALINSTITUTION.NAME ? item.FIBRANCH.NAME == BranchName ? "" : item.FINANCIALINSTITUTION.NAME + "\n" + item.FIBRANCH.NAME : item.FINANCIALINSTITUTION.NAME + "\n" + item.FIBRANCH.NAME),

                                    OpeningDate = null,
                                    PrincipalAmount = null,

                                    RenewedDate = item.OPENINGDATE.Value, //.ToString("dd-MMM-yy"),
                                    //item.RENWALDATE == null ? item.OPENINGDATE.Value.ToString("dd-MMM-yy") : item.RENWALDATE.Value.ToString("dd-MMM-yy"),

                                    Period = item.TENURE + " " + item.TENURETERM,

                                    MaturityDate = item.MATURITYDATE.Value,
                                    PresentRateOfInterest = item.RATEOFINTEREST,
                                    GrossInterest = item.GROSSINTEREST,
                                    SourceTax = item.SOURCETAX,
                                    ExciseDuty = item.EXCISEDUTY,
                                    OtherCharges = item.OTHERCHARGE,
                                    NetInterest = item.NETINTERESTRECEIVABLE,

                                    PresentPrincipalAmount = item.PRESENTPRINCIPALAMOUNT,
                                    Remarks = item.REMARKS
                                    //item.INTERESTMODE==ConstantVariable.INTERESTMODE_COMPOUND? item.COMPOUNDINTERESTINTERVAL+"\n Com.": null,
                                });

                                fdrNo = item.DEPOSITNUMBER;
                                BankName = item.FINANCIALINSTITUTION.NAME;
                                BranchName = item.FIBRANCH.NAME;


                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "ErrorPage", new { message = "Sorry, we can't find any record you searched!!" });
                    }



                    ReportDataSource oFDRStatement = new ReportDataSource();


                    LocalReport lr = new LocalReport();
                    lr.ReportPath = Server.MapPath("~/Reports/FDRSingleACHistory.rdlc");
                                     

                    ReportDataSource rd = new ReportDataSource();

                    DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(FDList.ToList());
                    rd.Name = "FDRACStatment";
                    rd.Value = dtFDRStatement;

                   

                    lr.ReportPath = Server.MapPath("~/Reports/FDRSingleACHistory.rdlc");
                    //lr.SetParameters(parameters);
                    lr.DataSources.Add(rd);


                    string ContentType = "application/vnd.ms-excel";
                    string FileType;
                    string ReportName;

                    if (IsExcell == "true")
                    {
                        FileType = "Excel";
                        ReportName = "FDRACHistoryStatment.{0}";
                    }
                    else
                    {
                        FileType = "PDF";
                        ReportName = "FDRACHistoryStatment.pdf";

                    }


                    string reportType = FileType; // "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                        "  <OutputFormat>" + FileType + "</OutputFormat>" +
                        "  <PageWidth>12.5in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0.5in</MarginTop>" +
                        "  <MarginLeft>0.5in</MarginLeft>" +
                        "  <MarginRight>0.5in</MarginRight>" +
                        "  <MarginBottom>0.5in</MarginBottom>" +
                        "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;


                    if (IsExcell == "true")
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
                else
                {
                    return RedirectToAction("Index", "ErrorPage", new { message = "FDR Number is required!" });
                }
            }
            catch (Exception ex)
            {
                string innerExp = "";
                try
                {
                    innerExp = ex.InnerException.Message;
                }
                catch (Exception) { }

                return RedirectToAction("Index", "ErrorPage", new { message = ex.Message + " Inner Exp:" + innerExp });
            }

        }

        public ActionResult FDREncashmentReportByDate(DateTime? fromDateFS, DateTime? toDateFS, string IsExcell)
        {
            List<FIXEDDEPOSIT> models = new List<FIXEDDEPOSIT>();
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (fromDateFS != null && toDateFS != null)
                {
                                  

                    //Point 1: get those FDR from FDRNote and Fixed Deposit table where FDRNOTE.FIXEDDEPOSIT_REFERENCE=FIXEDDEPOSIT.REFERENCE AND FDRNOTE.NOTETYPE='Encash' 
                    Entities db = new Entities(Session["Connection"] as EntityConnection);

                    models = (from fd in db.FIXEDDEPOSITs
                              join fdrnote in db.FDRNOTEs on fd.REFERENCE equals fdrnote.FIXEDDEPOSIT_REFERENCE
                              where fd.ENCASHMENTDATE >= fromDateFS && fd.ENCASHMENTDATE <= toDateFS && fdrnote.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_ENCASH
                              select fd).OrderBy(t => t.MATURITYDATE).ThenBy(t => t.FINANCIALINSTITUTION.NAME).ToList();
                    //.ThenByDescending(t=>t.ENCASHMENTDATE)

                    ReportDataSource oFDRStatement = new ReportDataSource();

                    if (models != null)
                    {
                        List<FDREncashedOrRenewedModel> newModels = new List<FDREncashedOrRenewedModel>();
                        YearlyInterestSlot ReceivableGrossSlot = new YearlyInterestSlot();

                        try
                        {
                            int count = 0;

                            foreach (var fixedDeposit in models)
                            {
                                count++;
                                ReceivableGrossSlot = GetGrossSlot(fixedDeposit);

                                newModels.Add(new FDREncashedOrRenewedModel
                                {
                                    FDRNO = fixedDeposit.DEPOSITNUMBER,
                                    NameAddressOfBank = fixedDeposit.FINANCIALINSTITUTION.NAME + "\n" + fixedDeposit.FIBRANCH.NAME,
                                    OpeningDate = fixedDeposit.OPENINGDATE.Value,
                                    PrincipalAmount = fixedDeposit.PRINCIPALAMOUNT.Value,
                                    MaturityDate = fixedDeposit.MATURITYDATE.Value,
                                    EncashDate = fixedDeposit.ENCASHMENTDATE.Value,
                                    GrossInterest = fixedDeposit.GROSSINTEREST.Value,
                                    SourceTaxTK = fixedDeposit.SOURCETAX.Value,
                                    ExciseDuty = fixedDeposit.EXCISEDUTY.Value,
                                    POtherCharges = fixedDeposit.OTHERCHARGE.Value,
                                    NetInterest = fixedDeposit.NETINTERESTRECEIVABLE.Value,
                                    TotalPrincipalAndInterest = fixedDeposit.TOTALAMOUNTRECEIVABLE.Value,

                                    //slot calculation
                                    ReceivableGross = ReceivableGrossSlot.ReceivableGross,
                                    Gross = ReceivableGrossSlot.Gross,
                                    Tax = ReceivableGrossSlot.SourceTax,
                                    ED = ReceivableGrossSlot.ED,
                                    PO = ReceivableGrossSlot.PO,
                                    Net = ReceivableGrossSlot.Net,
                                    //end slot calculation

                                    Remarks = fixedDeposit.REMARKS,
                                  //  RenewedDate =  fixedDeposit.RENWALDATE.HasValue? fixedDeposit.RENWALDATE.Value : null

                                });
                            }


                            LocalReport lr = new LocalReport();
                            lr.ReportPath = Server.MapPath("~/Reports/FDREncshmentStatement.rdlc");

                            //string report_title = "FDR Main Statement";;
                           // string report_name = "FDREncashmentStatement.pdf";


                            ReportDataSource rd = new ReportDataSource();
                            //ReportDataSource dd = new ReportDataSource();

                            DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(newModels.ToList());
                            rd.Name = "FDREncashmentStatement";            
                            rd.Value = dtFDRStatement;

                            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("FromDate", fromDateFS.Value.ToString("dd-MMM-yy")),
                             new ReportParameter("ToDate",  toDateFS.Value.ToString("dd-MMM-yy"))                                
                           };


                            lr.ReportPath = Server.MapPath("~/Reports/FDREncshmentStatement.rdlc");
                            lr.SetParameters(parameters);
                            lr.DataSources.Add(rd);

                            //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                            string ContentType = "application/vnd.ms-excel";
                            string FileType;
                            string ReportName;

                            if (IsExcell == "true")
                            {
                                FileType = "Excel";
                                ReportName = "FDREncashmentStatement-" + toDateFS.Value.ToString("dd-MMM-yy") + ".{0}";
                            }
                            else
                            {
                                FileType = "PDF";
                                ReportName = "FDREncashmentStatement-" + toDateFS.Value.ToString("dd-MMM-yy") + ".pdf";

                            }


                            string reportType = FileType; // "PDF";
                            string mimeType;
                            string encoding;
                            string fileNameExtension;

                            string deviceInfo =

                            "<DeviceInfo>" +
                                "  <OutputFormat>" + FileType + "</OutputFormat>" +
                                "  <PageWidth>13in</PageWidth>" +
                                "  <PageHeight>11in</PageHeight>" +
                                "  <MarginTop>0.5in</MarginTop>" +
                                "  <MarginLeft>0.75in</MarginLeft>" +
                                "  <MarginRight>0.5in</MarginRight>" +
                                "  <MarginBottom>0.5in</MarginBottom>" +
                                "</DeviceInfo>";

                            Warning[] warnings;
                            string[] streams;
                            byte[] renderedBytes;


                            if (IsExcell == "true")
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
                            return RedirectToAction("Index", "ErrorPage", new { message = Convert.ToString(ex.Message + " " + ex.InnerException.Message) });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "ErrorPage", new { message = "Sorry, we can't find any record you searched!!" });

                    }


                }
                else
                    return RedirectToAction("Index", "ErrorPage", new { message = "Please search by providing From Date and To Date!!" });
                //  return RedirectToAction("GenerateFDRRegisterReport", "FixedDepositRegister", new { title = "FDR Encashment", reportName = "FDREncashment.pdf" });
            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        public ActionResult FDRRenewReportByDate(DateTime? fromDateFS, DateTime? toDateFS, string IsExcell)
        {
            List<FIXEDDEPOSIT> models = new List<FIXEDDEPOSIT>();
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                if (fromDateFS != null && toDateFS != null)
                {
                    Entities db = new Entities(Session["Connection"] as EntityConnection);

                    models = (from fd in db.FIXEDDEPOSITs
                              join fdrnote in db.FDRNOTEs on fd.REFERENCE equals fdrnote.FIXEDDEPOSIT_REFERENCE
                              where fd.RENWALDATE >= fromDateFS && fd.RENWALDATE <= toDateFS && fdrnote.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_RENEWAL
                              select fd).OrderBy(t => t.MATURITYDATE).ThenBy(t => t.FINANCIALINSTITUTION.NAME).ToList();


                    //update 28-09-2016
                    if (models != null)
                    {
                        List<FDREncashedOrRenewedModel> newModels = new List<FDREncashedOrRenewedModel>();
                        YearlyInterestSlot ReceivableGrossSlot = new YearlyInterestSlot();

                        foreach (var fixedDeposit in models)
                        {

                            ReceivableGrossSlot = GetGrossSlot(fixedDeposit);

                            newModels.Add(new FDREncashedOrRenewedModel
                            {
                                FDRNO = fixedDeposit.DEPOSITNUMBER,
                                NameAddressOfBank = fixedDeposit.FINANCIALINSTITUTION.NAME + "\n" + fixedDeposit.FIBRANCH.NAME,
                                OpeningDate = fixedDeposit.OPENINGDATE.Value,
                                PrincipalAmount = fixedDeposit.PRINCIPALAMOUNT.Value,
                                MaturityDate = fixedDeposit.MATURITYDATE.Value,

                                GrossInterest = fixedDeposit.GROSSINTEREST.Value,
                                SourceTaxTK = fixedDeposit.SOURCETAX.Value,
                                ExciseDuty = fixedDeposit.EXCISEDUTY.Value,
                                POtherCharges = fixedDeposit.OTHERCHARGE.Value,
                                NetInterest = fixedDeposit.NETINTERESTRECEIVABLE.Value,
                                TotalPrincipalAndInterest = fixedDeposit.TOTALAMOUNTRECEIVABLE.Value,

                                //slot calculation
                                ReceivableGross = ReceivableGrossSlot.ReceivableGross,
                                Gross = ReceivableGrossSlot.Gross,
                                Tax = ReceivableGrossSlot.SourceTax,
                                ED = ReceivableGrossSlot.ED,
                                PO = ReceivableGrossSlot.PO,
                                Net = ReceivableGrossSlot.Net,
                                //end slot calculation

                                Remarks = fixedDeposit.REMARKS,
                                RenewedDate = fixedDeposit.RENWALDATE.Value,
                                InitialOpeningDate = fixedDeposit.INITIALOPENINGDATE.Value,
                                InitialPrincipalAmount = fixedDeposit.INITIALPRINCIPALAMOUNT

                            });

                        }

                        LocalReport lr = new LocalReport();
                        lr.ReportPath = Server.MapPath("~/Reports/FDRRenewalStatement.rdlc");

                        //string report_title = "FDR Main Statement";;
                       // string report_name = "FDRRenewalStatement.pdf";


                        ReportDataSource rd = new ReportDataSource();
                        //ReportDataSource dd = new ReportDataSource();

                        DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(newModels.ToList());
                        rd.Name = "FDRRenewStatement";
                        rd.Value = dtFDRStatement;




                        ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("FromDate", fromDateFS.Value.ToString("dd-MMM-yy")),
                             new ReportParameter("ToDate",  toDateFS.Value.ToString("dd-MMM-yy"))                                
                           };


                        lr.ReportPath = Server.MapPath("~/Reports/FDRRenewalStatement.rdlc");
                        lr.SetParameters(parameters);
                        lr.DataSources.Add(rd);
                        //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);

                        string ContentType = "application/vnd.ms-excel";
                        string FileType;
                        string ReportName;

                        if (IsExcell == "true")
                        {
                            FileType = "Excel";
                            ReportName = "FDRRenewalStatement-" +fromDateFS.Value.ToString("dd-MMM-yy")+"-to-"+ toDateFS.Value.ToString("dd-MMM-yy") + ".{0}";
                        }
                        else
                        {
                            FileType = "PDF";
                            ReportName = "FDRRenewalStatement-" + fromDateFS.Value.ToString("dd-MMM-yy") + "-to-" + toDateFS.Value.ToString("dd-MMM-yy") + ".pdf";

                        }


                        string reportType = FileType; // "PDF";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;

                        string deviceInfo =

                        "<DeviceInfo>" +
                            "  <OutputFormat>" + FileType + "</OutputFormat>" +
                            "  <PageWidth>12.75in</PageWidth>" +
                            "  <PageHeight>11in</PageHeight>" +
                            "  <MarginTop>0.5in</MarginTop>" +
                            "  <MarginLeft>0.5in</MarginLeft>" +
                            "  <MarginRight>0.5in</MarginRight>" +
                            "  <MarginBottom>0.5in</MarginBottom>" +
                            "</DeviceInfo>";

                        Warning[] warnings;
                        string[] streams;
                        byte[] renderedBytes;


                        if (IsExcell == "true")
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
                       

                        //End
                    }
                    else
                    {
                        return RedirectToAction("Index", "ErrorPage", new { message = "Sorry, we can't find any record you searched!!" });

                    }
                    //  return RedirectToAction("GenerateFDRRegisterReport", "FixedDepositRegister", new { title = "FDR Renew", reportName = "FDRRenew.pdf" });
                }
                else
                    return RedirectToAction("Index", "ErrorPage", new { message = "Please search by providing From Date and To Date!!" });
            }
            catch (Exception ex)
            {
                string message = ex.Message + " Inner Error: "; //+ Convert.ToString(ex.InnerException.Message)

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
        /// <summary>
        /// take all FDR from FDR Register whose Maturity Date will be the range between fromDateFS and toDateFS
        /// </summary>     
        public ActionResult FDRMaturityStatement(DateTime? fromDateFS, DateTime? toDateFS, string HasHistory, string IsExcell)
        {
            List<FIXEDDEPOSIT> models = new List<FIXEDDEPOSIT>();
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                if (fromDateFS != null && toDateFS != null)
                {
                    Entities db = new Entities(Session["Connection"] as EntityConnection);
                    models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr => fdr.MATURITYDATE >= fromDateFS && fdr.MATURITYDATE <= toDateFS).OrderBy(t => t.MATURITYDATE).ThenBy(t => t.FINANCIALINSTITUTION.NAME).ToList();

                    #region SUMMARY
                    //select DEPOSITNUMBER,FINANCIALINSTITUTION.NAME,PRINCIPALAMOUNT,PRESENTPRINCIPALAMOUNT,INITIALPRINCIPALAMOUNT,MATURITYDATE,NETINTERESTRECEIVABLE 
                    //from FIXEDDEPOSIT,FINANCIALINSTITUTION
                    //where MATURITYDATE>='01-JAN-16' and MATURITYDATE<='31-JAN-16' and FIXEDDEPOSIT.FINANCIALINSTITUTION_REFERENCE = FINANCIALINSTITUTION.REFERENCE
                    //and FIXEDDEPOSIT.STATUS != 'Pending'
                    //order by MATURITYDATE;
                    //
                    // check renewed flow in db
                    //select PROPOSEDACTION,OPENINGDATE,INITIALOPENINGDATE,INITIALFIXEDDEPOSITREF,INITIALPRINCIPALAMOUNT from FIXEDDEPOSIT where PROPOSEDACTION='Renewal';
                    //select PROPOSEDACTION,OPENINGDATE,INITIALOPENINGDATE,INITIALFIXEDDEPOSITREF,PRINCIPALAMOUNT, INITIALPRINCIPALAMOUNT from FIXEDDEPOSIT where INITIALFIXEDDEPOSITREF='b5565bf1-eb0c-4b7d-9936-edb56c72134c';

                    //IsDateSame= e.INITIALOPENINGDATE.Value.Date.CompareTo(e.OPENINGDATE.Value.Date),  //Same Date resylt=0, Diff Date result=-1
                    //--------------------------------------------------------------
                    //For Renewed fdr maturity statement PresentPrincipalTk will be
                    //here if initial opening date and Opening date is same then this fdr is first fdr so Print its Initial principal Amount
                    //else print only principal amount 
                    // this condition implement on FDRMaturityStatement.rdlc by comparing OpeningDate with RenewedorIssuedDate
                    //-------------------------------------------------------------

                    #endregion

                    var Maturity = (from e in models
                                    where
                                    e.STATUS == ConstantVariable.STATUS_APPROVED || e.STATUS == ConstantVariable.STATUS_ENCASHED || e.STATUS == ConstantVariable.STATUS_RENEWED
                                    select new
                                    {
                                        FDRNo = e.DEPOSITNUMBER,
                                        BankName = e.FINANCIALINSTITUTION.NAME + "\n" + e.FIBRANCH.NAME,
                                        OpeningDate = e.INITIALOPENINGDATE.Value,              //.ToString("dd-MMM-yy"),
                                        RenewedorIssuedDate = e.OPENINGDATE.Value,            //.ToString("dd-MMM-yy"),                                       
                                        Period = e.TENURE + " " + e.TENURETERM,
                                        MaturityDate = e.MATURITYDATE.Value,                 //.ToString("dd-MMM-yy"),
                                        PresentPrincipalTK = e.PRESENTPRINCIPALAMOUNT,
                                        PrincipalAmount = e.PRINCIPALAMOUNT,
                                        NetInterest = e.NETINTERESTRECEIVABLE,
                                        InitialPrincipalAmount = e.INITIALPRINCIPALAMOUNT,
                                        PresentRate = e.RATEOFINTEREST,
                                        Offer1 = e.FINANCIALINSTITUTION.OFFERRATE1,
                                        Offer2 = e.FINANCIALINSTITUTION.OFFERRATE2,
                                        Offer3 = e.FINANCIALINSTITUTION.OFFERRATE3,
                                        Remarks = e.REMARKS
                                    }).ToList();

                    
                    LocalReport lr = new LocalReport();
                                      
                    ReportDataSource rd = new ReportDataSource();
                    DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(Maturity);
                    rd.Name = "FDRMaturityStatement";
                    rd.Value = dtFDRStatement;


                    ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("FromDate",fromDateFS.Value.ToString("dd-MMM-yy")),
                             new ReportParameter("ToDate",toDateFS.Value.ToString("dd-MMM-yy"))                                
                           };


                    lr.ReportPath = Server.MapPath("~/Reports/FDRMaturityStatement.rdlc");
                    lr.SetParameters(parameters);
                    lr.DataSources.Add(rd);


                    string ContentType = "application/vnd.ms-excel";
                    string FileType;
                    string ReportName;

                    if (IsExcell == "true")
                    {
                        FileType = "Excel";
                        ReportName = "FDRMaturityStatement-" + fromDateFS.Value.ToString("dd-MMM-yy") + "-to-" + toDateFS.Value.ToString("dd-MMM-yy") + ".{0}";
                    }
                    else
                    {
                        FileType = "PDF";
                        ReportName = "FDRMaturityStatement-" + fromDateFS.Value.ToString("dd-MMM-yy") + "-to-" + toDateFS.Value.ToString("dd-MMM-yy") + ".pdf";

                    }


                    string reportType = FileType; // "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                        "  <OutputFormat>" + FileType + "</OutputFormat>" +
                        "  <PageWidth>12in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0.5in</MarginTop>" +
                        "  <MarginLeft>0.5in</MarginLeft>" +
                        "  <MarginRight>0.5in</MarginRight>" +
                        "  <MarginBottom>0.5in</MarginBottom>" +
                        "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;


                    if (IsExcell == "true")
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

                message = "From date or To date is required!!";
                return RedirectToAction("Index", "ErrorPage", new { message });


            }
            catch (Exception ex)
            {
                string message = ex.Message + " " + ex.InnerException.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        public ActionResult BONDInterestIncome(DateTime? fromDateBII)
        {
            List<BOND> models = new List<BOND>();
            List<GOVBONDINTERESTSCHEDULE> interestSchedule = new List<GOVBONDINTERESTSCHEDULE>();


            try
            {
                if (fromDateBII != null)
                    models = new Entities(Session["Connection"] as EntityConnection).BONDs.Where(bond => bond.STATUS == "Approved").ToList();

                var bondReference = from item in models select item.REFERENCE;
                DateTime? lastDate = new DateTime(fromDateBII.Value.Year, 12, 31);

                interestSchedule = new Entities(Session["Connection"] as EntityConnection).GOVBONDINTERESTSCHEDULEs.Where(bond => bondReference.Contains(bond.BOND_REFERENCE) && bond.DUEDATE <= lastDate).ToList();

                //Start Calculate Income
                ReportDataSet bondTable = new ReportDataSet();
                foreach (var item in models)
                {
                    //If maturity has passed.
                    if (item.MATURITYDATE.Value < fromDateBII.Value)
                        continue;

                    if (item.BONDISSUEDATE > fromDateBII.Value)
                        continue;


                    ReportDataSet.BONDIncomeRow newRow = bondTable.BONDIncome.NewBONDIncomeRow();

                    newRow.Bank = item.FINANCIALINSTITUTION.NAME;
                    newRow.BondIssueDate = item.BONDISSUEDATE.Value;
                    newRow.FaceValue = item.FACEVALUE.Value;
                    newRow.MaturityDate = item.MATURITYDATE.Value;
                    newRow.InterestRate = item.COUPONRATE.Value;
                    newRow.Tenure = item.TENURE.Value;
                    newRow.Term = item.TENURETERM;

                    //Get Due Date
                    var lastInterestReceived = interestSchedule.Where(bond => bond.BOND_REFERENCE == item.REFERENCE && bond.DUEDATE <= fromDateBII.Value && bond.STATUS == "Posted").OrderBy(bond => bond.SEQUENCENUMBER).FirstOrDefault();

                    if (lastInterestReceived == null)
                        lastInterestReceived = interestSchedule.Where(bond => bond.BOND_REFERENCE == item.REFERENCE && bond.STATUS == "Pending").OrderBy(bond => bond.SEQUENCENUMBER).FirstOrDefault();


                    //Last date interet due is not same year of report date
                    if (lastInterestReceived.DUEDATE.Value.Year != fromDateBII.Value.Year)
                    {
                        newRow.DuesDays = (fromDateBII.Value - new DateTime(fromDateBII.Value.Year, 1, 1)).Days + 1;
                    }

                    newRow.InterestReceivable = ((item.FACEVALUE * (item.COUPONRATE / 100)) / item.ANNUALDAYS * newRow.DuesDays).Value;
                    newRow.NextInterestSchedule = DateTime.Today;
                    newRow.GrossInterest = ((item.FACEVALUE * (item.COUPONRATE / 100))).Value;
                    newRow.SourceTax = newRow.GrossInterest * decimal.Parse(".20");


                    //Last Year receivable
                    var lastYearDueDate = interestSchedule.Where(bond => bond.BOND_REFERENCE == item.REFERENCE && bond.DUEDATE.Value.Year == (fromDateBII.Value.Year - 1)).OrderByDescending(bond => bond.SEQUENCENUMBER).FirstOrDefault();

                    int dueDiffer = (new DateTime(fromDateBII.Value.Year - 1, 12, 31) - lastYearDueDate.DUEDATE.Value).Days;

                    newRow.InterestReceivableUpto = ((item.FACEVALUE * (item.COUPONRATE / 100)) / item.ANNUALDAYS * dueDiffer).Value;

                    bondTable.BONDIncome.AddBONDIncomeRow(newRow);
                }


                LocalReport lr = new LocalReport();
                lr.ReportPath = Server.MapPath("~/Reports/BONDIncomeStatement.rdlc");

                string report_title = "BOND Income Statement";
                string report_name = "BONDIncomeStatement.pdf";

                ReportDataSource rd = new ReportDataSource();
                //DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(newModels.ToList());
                rd.Name = "BONDIncome";
                rd.Value = bondTable.BONDIncome;


                ReportParameter[] parameters = new ReportParameter[] 
                {
                     new ReportParameter("CompanyName","DLIC"),
                     new ReportParameter("Address","Gulshan-2, Dhaka"),
                     new ReportParameter("ReportTitle",report_title),              
                     new ReportParameter("AsOn",fromDateBII.Value.ToShortDateString()) 
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
                return File(renderedBytes, mimeType, report_name);

            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

      

        /// <summary>
        /// create date <7th june 2016,1st day or ramadan>
        /// </summary>

        public ActionResult FDREncashmentTaxStatement(DateTime? fromDateFdrTax, DateTime? toDateFdrTax, string IsExcell)
        {

            List<FIXEDDEPOSIT> models = new List<FIXEDDEPOSIT>();
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (fromDateFdrTax != null && toDateFdrTax != null)
                {

                    Entities db = new Entities(Session["Connection"] as EntityConnection);

                    //models =db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr => fdr.ENCASHMENTDATE >= fromDateFdrTax && fdr.ENCASHMENTDATE <= toDateFdrTax).ToList();  // "Encashed"
                    //models = models.ToList().Where(t => t.STATUS == ConstantVariable.STATUS_ENCASHED).ToList();

                    models = (from fd in db.FIXEDDEPOSITs
                              join fdrnote in db.FDRNOTEs on fd.REFERENCE equals fdrnote.FIXEDDEPOSIT_REFERENCE
                              where fd.ENCASHMENTDATE >= fromDateFdrTax && fd.ENCASHMENTDATE <= toDateFdrTax && fdrnote.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_ENCASH
                              select fd).OrderBy(t => t.MATURITYDATE).ThenBy(t => t.FINANCIALINSTITUTION.NAME).ToList();

                    //.ThenByDescending(t=>t.ENCASHMENTDATE)

                    var newModels = from item in models
                                    select new
                                    {
                                        FdrNo = item.DEPOSITNUMBER,
                                        FINameAndBranch = item.FINANCIALINSTITUTION.NAME + "\n" + item.FIBRANCH.NAME + "Branch",
                                        OpeningOrRenewedDate = item.OPENINGDATE.Value.ToString("dd-MMM-yy"),
                                        MaturityDate = item.MATURITYDATE.Value.ToString("dd-MMM-yy"),
                                        RenewalDate = "",
                                        GrossInterest = item.GROSSINTEREST.Value,
                                        SourceTax = item.SOURCETAX.Value
                                    };

                    LocalReport lr = new LocalReport();

                                      

                    ReportDataSource rd = new ReportDataSource();
                    DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(newModels.ToList());
                    rd.Name = "FDRTaxStatement";
                    rd.Value = dtFDRStatement;

                    ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("FromDate",fromDateFdrTax.Value.ToString("dd-MMM-yy")),
                             new ReportParameter("ToDate", toDateFdrTax.Value.ToString("dd-MMM-yy"))                                
                           };

                    lr.ReportPath = Server.MapPath("~/Reports/FDREncashmentTaxStatement.rdlc");
                    lr.SetParameters(parameters);
                    lr.DataSources.Add(rd);


                    string ContentType = "application/vnd.ms-excel";
                    string FileType;
                    string ReportName;

                    if (IsExcell == "true")
                    {
                        FileType = "Excel";
                        ReportName = "FDREncashmentTaxStatement-" + fromDateFdrTax.Value.ToString("dd-MMM-yy") + "-to-" + toDateFdrTax.Value.ToString("dd-MMM-yy") + ".{0}";
                    }
                    else
                    {
                        FileType = "PDF";
                        ReportName = "FDREncashmentTaxStatement-" + fromDateFdrTax.Value.ToString("dd-MMM-yy") + "-to-" + toDateFdrTax.Value.ToString("dd-MMM-yy") + ".pdf";

                    }


                    string reportType = FileType; // "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                        "  <OutputFormat>" + FileType + "</OutputFormat>" +
                        "  <PageWidth>11in</PageWidth>" +
                        "  <PageHeight>8.5in</PageHeight>" +
                        "  <MarginTop>1.25in</MarginTop>" +
                        "  <MarginLeft>0.75in</MarginLeft>" +
                        "  <MarginRight>0.5in</MarginRight>" +
                        "  <MarginBottom>0.5in</MarginBottom>" +
                        "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;


                    if (IsExcell == "true")
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

                message = "From date or To date is required!!";
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
            catch (Exception ex)
            {
                message = ex.Message + " " + ex.InnerException.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }

        /// <summary>
        /// create date <7th june 2016,1st day or ramadan>
        /// </summary>
        public ActionResult FDRRenewalTaxStatement(DateTime? fromDateRewTax, DateTime? toDateFdrRewTax, string IsExcell)
        {
            string message;
            List<FIXEDDEPOSIT> models = new List<FIXEDDEPOSIT>();
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                if (fromDateRewTax != null && toDateFdrRewTax != null)
                {
                    Entities db = new Entities(Session["Connection"] as EntityConnection);

                    //models = new Entities(Session["Connection"] as EntityConnection).FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr =>fdr.RENWALDATE >= fromDateRewTax && fdr.RENWALDATE <= toDateFdrRewTax).ToList(); // "Renewed" fdr => fdr.CREATEDDATE >= fromDateRewTax && fdr.CREATEDDATE <= toDateFdrRewTax || 
                    //models = models.ToList().Where(t => t.STATUS == ConstantVariable.STATUS_RENEWED).ToList();                                                       

                    models = (from fd in db.FIXEDDEPOSITs
                              join fdrnote in db.FDRNOTEs on fd.REFERENCE equals fdrnote.FIXEDDEPOSIT_REFERENCE
                              where fdrnote.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_RENEWAL && fd.RENWALDATE >= fromDateRewTax && fd.RENWALDATE <= toDateFdrRewTax
                              select fd).OrderBy(t => t.MATURITYDATE).ThenBy(t => t.FINANCIALINSTITUTION.NAME).ToList();

                    //.ThenByDescending(t=>t.RENWALDATE)
                    //where fd.RENWALDATE >= fromDateRewTax && fd.RENWALDATE <= fromDateRewTax && fdrnote.NOTETYPE == ConstantVariable.STATUS_NOTETYPE_RENEWAL
                    //select fd).ToList();      

                    var newModels = from item in models
                                    select new
                                    {
                                        FdrNo = item.DEPOSITNUMBER,
                                        FINameAndBranch = item.FINANCIALINSTITUTION.NAME + "\n" + item.FIBRANCH.NAME + "Branch",
                                        OpeningOrRenewedDate = item.OPENINGDATE.HasValue ? item.OPENINGDATE.Value.ToString("dd-MMM-yy") : "",
                                        MaturityDate = item.MATURITYDATE.HasValue ? item.MATURITYDATE.Value.ToString("dd-MMM-yy") : "",
                                        RenewalDate = item.RENWALDATE.HasValue ? item.RENWALDATE.Value.ToString("dd-MMM-yy") : "",
                                        GrossInterest = item.GROSSINTEREST.HasValue ? item.GROSSINTEREST.Value : 0,
                                        SourceTax = item.SOURCETAX.Value
                                    };

                    LocalReport lr = new LocalReport();

                   
                    ReportDataSource rd = new ReportDataSource();
                    DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(newModels.ToList());
                    rd.Name = "FDRTaxStatement";
                    rd.Value = dtFDRStatement;


                    ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("FromDate", fromDateRewTax.Value.ToString("dd-MMM-yy")),
                             new ReportParameter("ToDate", toDateFdrRewTax.Value.ToString("dd-MMM-yy"))                                
                           };


                    lr.ReportPath = Server.MapPath("~/Reports/FDRRenewalTaxStatement.rdlc");
                    lr.SetParameters(parameters);
                    lr.DataSources.Add(rd);

                    string ContentType = "application/vnd.ms-excel";
                    string FileType;
                    string ReportName;

                    if (IsExcell == "true")
                    {
                        FileType = "Excel";
                        ReportName = "FDRRenewalTaxStatement-" + fromDateRewTax.Value.ToString("dd-MMM-yy") + "-to-" + toDateFdrRewTax.Value.ToString("dd-MMM-yy") + ".{0}";
                    }
                    else
                    {
                        FileType = "PDF";
                        ReportName = "FDRRenewalTaxStatement-" + fromDateRewTax.Value.ToString("dd-MMM-yy") + "-to-" + toDateFdrRewTax.Value.ToString("dd-MMM-yy") + ".pdf";

                    }


                    string reportType = FileType; // "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                        "  <OutputFormat>" + FileType + "</OutputFormat>" +
                        "  <PageWidth>10.5in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0.5in</MarginTop>" +
                        "  <MarginLeft>1.5in</MarginLeft>" +
                        "  <MarginRight>1.5in</MarginRight>" +
                        "  <MarginBottom>0.5in</MarginBottom>" +
                        "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;


                    if (IsExcell == "true")
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

                message = "From date or To date is required!!";
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
            catch (Exception ex)
            {
                message = ex.Message + " " + ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        public ActionResult FDRBankWiseStatement(DateTime? toDateBWS, string Option, string IsExcell)
        {
            #region Query
            //return Content(toDateBWS.Value.ToString("dd-MMM-yy"));
            //select SUM(PRINCIPALAMOUNT) "Sum Principal",count(DEPOSITNUMBER) "Deposit", FINANCIALINSTITUTION_REFERENCE
            //from FIXEDDEPOSIT
            //group by FINANCIALINSTITUTION_REFERENCE;

            // select SUM(PRINCIPALAMOUNT) "Sum Principal",count(DEPOSITNUMBER) "Deposit",fi.NAME "Bank Name" 
            //from FIXEDDEPOSIT,FINANCIALINSTITUTION fi
            //where FIXEDDEPOSIT.FINANCIALINSTITUTION_REFERENCE = fi.REFERENCE
            //group by FINANCIALINSTITUTION_REFERENCE, fi.NAME
            //Order by fi.NAME;
            #endregion

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                toDateBWS = DateTime.Now.Date;

                List<FIXEDDEPOSIT> models = new List<FIXEDDEPOSIT>();

                if (toDateBWS != null)  
                {
                    Entities db = new Entities(Session["Connection"] as EntityConnection);
                    //Sum of All Principal Amount of FixedDeposit table where Status= Approved

                    #region prv_Code 14-Dec-17
                    //var totalPrincialAmount = db.FIXEDDEPOSITs.Where(t => t.STATUS == ConstantVariable.STATUS_APPROVED && EntityFunctions.TruncateTime(t.OPENINGDATE.Value) <= EntityFunctions.TruncateTime(toDateBWS.Value)).ToList().Sum(t => t.PRINCIPALAMOUNT ?? 0);  //t.MATURITYDATE.Value                     

                    //if (totalPrincialAmount == null)
                    //    totalPrincialAmount = 1;
                    //if (totalPrincialAmount <= 0)
                    //    totalPrincialAmount = 1;

               
                    //var FIbankwise = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(t => t.NAME).Select(t => new
                    //   {
                    //       BankName = t.NAME,
                    //       FDRNo = t.FIXEDDEPOSITs.Where(x => x.STATUS == ConstantVariable.STATUS_APPROVED && EntityFunctions.TruncateTime(x.OPENINGDATE) <= EntityFunctions.TruncateTime(toDateBWS.Value)).Count(),

                    //       ExistingCapLimit = t.CAPLIMIT.Value,
                    //       PrincipalAmountTK = Math.Round((t.FIXEDDEPOSITs.Where(x => x.STATUS == ConstantVariable.STATUS_APPROVED && EntityFunctions.TruncateTime(x.OPENINGDATE.Value) <= EntityFunctions.TruncateTime(toDateBWS.Value)).Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),        //&& x.OPENINGDATE >= fromDateFS       Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),                   
                    //       PercentagetotalFDR = Math.Round(((t.FIXEDDEPOSITs.Where(x => x.STATUS == ConstantVariable.STATUS_APPROVED && EntityFunctions.TruncateTime(x.OPENINGDATE.Value) <= EntityFunctions.TruncateTime(toDateBWS.Value)).Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / totalPrincialAmount) * 100, 2),    //Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),               

                    //       Offerrate1 = t.OFFERRATE1,
                    //       Offerrate2 = t.OFFERRATE2,
                    //       Offerrate3 = t.OFFERRATE3,
                    //       NPL = t.NPLPERCENTAGE,
                    //       CamelsRating = t.CAMELRATING,
                    //       InstitutionType = t.INSTITUTIONTYPE

                    //   }).OrderByDescending(t => t.FDRNo).ToList();
                    #endregion

                    models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr => (fdr.STATUS == ConstantVariable.STATUS_APPROVED || fdr.STATUS == ConstantVariable.STATUS_PENDING) &&
                                                   (EntityFunctions.TruncateTime(toDateBWS.Value) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value)) && (EntityFunctions.TruncateTime(toDateBWS.Value) < EntityFunctions.TruncateTime(fdr.MATURITYDATE.Value))).OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();


                    var totalPrincialAmount = models.Sum(t => t.PRINCIPALAMOUNT ?? 0); 
                       
                    
                    if (totalPrincialAmount == null)
                        totalPrincialAmount = 1;
                    if (totalPrincialAmount <= 0)
                        totalPrincialAmount = 1;


                    //var FIbankwise = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(t => t.NAME).Select(t => new
                    //{
                    //    BankName = t.NAME,
                    //    FDRNo = t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_PENDING) && EntityFunctions.TruncateTime(x.OPENINGDATE) <= EntityFunctions.TruncateTime(toDateBWS.Value)).Count(),

                    //    ExistingCapLimit = t.CAPLIMIT.Value,
                    //    PrincipalAmountTK = Math.Round((t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_PENDING) && EntityFunctions.TruncateTime(x.OPENINGDATE.Value) <= EntityFunctions.TruncateTime(toDateBWS.Value)).Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),        //&& x.OPENINGDATE >= fromDateFS       Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),                   
                    //    PercentagetotalFDR = Math.Round(((t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_PENDING) && EntityFunctions.TruncateTime(x.OPENINGDATE.Value) <= EntityFunctions.TruncateTime(toDateBWS.Value)).Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / totalPrincialAmount) * 100, 2),    //Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),               

                    //    Offerrate1 = t.OFFERRATE1,
                    //    Offerrate2 = t.OFFERRATE2,
                    //    Offerrate3 = t.OFFERRATE3,
                    //    NPL = t.NPLPERCENTAGE,
                    //    CamelsRating = t.CAMELRATING,
                    //    InstitutionType = t.INSTITUTIONTYPE

                    //}).OrderByDescending(t => t.FDRNo).ToList();


                   // var FI_List = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(t => t.NAME).ToList();


                    var FIbankwise = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(t => t.NAME).Select(t => new
                    {
                        BankName = t.NAME,
                        FDRNo = t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_PENDING) && (EntityFunctions.TruncateTime(x.OPENINGDATE) <= EntityFunctions.TruncateTime(toDateBWS.Value) && EntityFunctions.TruncateTime(x.MATURITYDATE) > EntityFunctions.TruncateTime(toDateBWS.Value))).Count(),

                        ExistingCapLimit = t.CAPLIMIT.Value,
                        PrincipalAmountTK = Math.Round((t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_PENDING) && (EntityFunctions.TruncateTime(x.OPENINGDATE) <= EntityFunctions.TruncateTime(toDateBWS.Value) && EntityFunctions.TruncateTime(x.MATURITYDATE) > EntityFunctions.TruncateTime(toDateBWS.Value))).Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),        //&& x.OPENINGDATE >= fromDateFS       Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),                   
                        PercentagetotalFDR = Math.Round(((t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_PENDING) && (EntityFunctions.TruncateTime(x.OPENINGDATE) <= EntityFunctions.TruncateTime(toDateBWS.Value) && EntityFunctions.TruncateTime(x.MATURITYDATE) > EntityFunctions.TruncateTime(toDateBWS.Value))).Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / totalPrincialAmount) * 100, 2),    //Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),               

                        Offerrate1 = t.OFFERRATE1,
                        Offerrate2 = t.OFFERRATE2,
                        Offerrate3 = t.OFFERRATE3,
                        NPL = t.NPLPERCENTAGE,
                        CamelsRating = t.CAMELRATING,
                        InstitutionType = t.INSTITUTIONTYPE

                    }).OrderByDescending(t => t.FDRNo).ToList();


                    if (Option == "FB") //Only FDR Bank
                    {
                        FIbankwise = FIbankwise.Where(t => t.FDRNo > 0).ToList();
                    }

                    LocalReport lr = new LocalReport();
                                      
                    ReportDataSource rd = new ReportDataSource();
                    DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(FIbankwise);
                    rd.Name = "StatementBankwise";
                    rd.Value = dtFDRStatement;

                    ReportParameter[] parameters = new ReportParameter[] 
                    {
                     new ReportParameter("SearchDate", toDateBWS.Value.ToString("dd-MMM-yy")),
                     new ReportParameter("ReportHeading","FDR Bank Wise Statement")
                   
                    };

                    lr.ReportPath = Server.MapPath("~/Reports/FDRStatementBankWise.rdlc");
                    lr.SetParameters(parameters);

                    lr.DataSources.Add(rd);


                    string ContentType = "application/vnd.ms-excel";
                    string FileType;
                    string ReportName;

                    if (IsExcell == "true")
                    {
                        FileType = "Excel";
                        ReportName = "FDRBankWiseStatement-" + toDateBWS.Value.ToString("dd-MMM-yy") + ".{0}";
                    }
                    else
                    {
                        FileType = "PDF";
                        ReportName = "FDRBankWiseStatement-" + toDateBWS.Value.ToString("dd-MMM-yy") + ".pdf";

                    }


                    string reportType = FileType; // "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                        "  <OutputFormat>" + FileType + "</OutputFormat>" +
                        "  <PageWidth>11.5in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0.5in</MarginTop>" +
                        "  <MarginLeft>0.5in</MarginLeft>" +
                        "  <MarginRight>1in</MarginRight>" +
                        "  <MarginBottom>0.5in</MarginBottom>" +
                        "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;


                    if (IsExcell == "true")
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

                message = "From date or To date is required!!";
                return RedirectToAction("Index", "ErrorPage", new { message });


            }
            catch (NullReferenceException nex)
            {
                string message = nex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
            catch (ArgumentException aex)
            {
                string message = aex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }

        public ActionResult FDRBankWiseHistory(DateTime? toDateBWH, string Option, string IsExcell)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                List<FIXEDDEPOSIT> models = new List<FIXEDDEPOSIT>();
                if (toDateBWH != null)  //fromDateFS != null && 
                {
                    Entities db = new Entities(Session["Connection"] as EntityConnection);
                    //Sum of All Principal Amount of FixedDeposit table where Status= Approved

                    //prev 23-3-17               
                    //For history many of FDR will be encash/renwed  so we should check status =Approved || Encashed || Renewed
                    //var totalPrincialAmount = db.FIXEDDEPOSITs.Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED)
                    //.Where(fdr => fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_RENEWAL ? toDateBWH.Value < fdr.RENWALDATE : fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_ENCASH ? toDateBWH.Value < fdr.ENCASHMENTDATE : fdr.OPENINGDATE <= toDateBWH.Value).ToList().Sum(fdr => fdr.PRINCIPALAMOUNT ?? 0);

                    //models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION")
                    //      .Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED || fdr.STATUS == ConstantVariable.STATUS_PENDING)

                    //      .Where(fdr => (fdr.STATUS == ConstantVariable.STATUS_RENEWED && fdr.STATUS != ConstantVariable.STATUS_APPROVED) ?
                    //          (EntityFunctions.TruncateTime(toDateBWH.Value) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value) && EntityFunctions.TruncateTime(toDateBWH.Value) < EntityFunctions.TruncateTime(fdr.LASTUPDATED.Value)) :  //fdr.RENWALDATE.Value //Accepted date

                    //          (fdr.STATUS == ConstantVariable.STATUS_ENCASHED && fdr.STATUS != ConstantVariable.STATUS_APPROVED) ?
                    //          (EntityFunctions.TruncateTime(toDateBWH.Value) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value) && EntityFunctions.TruncateTime(toDateBWH.Value) < EntityFunctions.TruncateTime(fdr.LASTUPDATED.Value)) : //fdr.ENCASHMENTDATE.Value //Accepted Date

                    //          EntityFunctions.TruncateTime(toDateBWH.Value) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value))//AcceptDate to Opening //this is for approved status //fdr.OPENINGDATE <= toDateFS                                                        
                    //      .OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();


                    models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED || fdr.STATUS == ConstantVariable.STATUS_PENDING)
                     .Where(fdr => (EntityFunctions.TruncateTime(toDateBWH.Value) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE)) && (EntityFunctions.TruncateTime(toDateBWH.Value) < EntityFunctions.TruncateTime(fdr.MATURITYDATE)))   //fdr.RENWALDATE.Value
                        //this is for approved status //fdr.OPENINGDATE <= toDateFS                                                        
                     .OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();

                   
                    var totalPrincialAmount = models.ToList().Sum(fdr => fdr.PRINCIPALAMOUNT ?? 0);

                   if (totalPrincialAmount == null)
                        totalPrincialAmount = 1;
                    if (totalPrincialAmount <= 0)
                        totalPrincialAmount = 1;

                    #region PrvCode 23-3-17
                    //var FIbankwise = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(t => t.NAME).Select(t => new
                    //{
                    //    BankName = t.NAME,
                    //    FDRNo = t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_RENEWED || x.STATUS == ConstantVariable.STATUS_ENCASHED) && x.OPENINGDATE <= toDateBWH).Count(),
                    //    ExistingCapLimit = t.CAPLIMIT.Value,
                    //    PrincipalAmountTK = Math.Round((t.FIXEDDEPOSITs.Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED)
                    //                         .Where(fdr => fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_RENEWAL ? toDateBWH.Value < fdr.RENWALDATE : fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_ENCASH ? toDateBWH.Value < fdr.ENCASHMENTDATE : fdr.OPENINGDATE <= toDateBWH.Value).Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),
                    //    PercentagetotalFDR = Math.Round(((t.FIXEDDEPOSITs.Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED)
                    //    .Where(fdr => fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_RENEWAL ? toDateBWH.Value < fdr.RENWALDATE : fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_ENCASH ? toDateBWH.Value < fdr.ENCASHMENTDATE : fdr.OPENINGDATE <= toDateBWH.Value).Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / totalPrincialAmount) * 100, 2),
                    //    Offerrate1 = t.OFFERRATE1,
                    //    Offerrate2 = t.OFFERRATE2,
                    //    Offerrate3 = t.OFFERRATE3,
                    //    NPL = t.NPLPERCENTAGE,
                    //    CamelsRating = t.CAMELRATING
                    //}).OrderByDescending(t => t.FDRNo).ToList();
                    #endregion

                    //Divided by 
                    var FIbankwise = (from fi in db.FINANCIALINSTITUTIONs.ToList()
                                      select new
                                      {
                                          BankName = fi.NAME,
                                          FDRNo = models.Where(t => t.FINANCIALINSTITUTION_REFERENCE == fi.REFERENCE).ToList().Count(),
                                          ExistingCapLimit = fi.CAPLIMIT.Value,

                                          PrincipalAmountTK = Math.Round((models.Where(t => t.FINANCIALINSTITUTION_REFERENCE == fi.REFERENCE).ToList().Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),
                                          PercentagetotalFDR = Math.Round(((models.Where(t => t.FINANCIALINSTITUTION_REFERENCE == fi.REFERENCE).ToList().Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / totalPrincialAmount) * 100, 2),

                                          Offerrate1 = fi.OFFERRATE1,
                                          Offerrate2 = fi.OFFERRATE2,
                                          Offerrate3 = fi.OFFERRATE3,
                                          NPL = fi.NPLPERCENTAGE,
                                          CamelsRating = fi.CAMELRATING,
                                          InstitutionType = fi.INSTITUTIONTYPE
                                      }).OrderByDescending(t => t.FDRNo).ToList();


                    //SearchDate

                    if (Option == "FB") //Only FDR Bank
                    {
                        FIbankwise = FIbankwise.Where(t => t.FDRNo > 0).ToList();
                    }
                    LocalReport lr = new LocalReport();

                    // string report_title = "FDR Encashment Tax Statement"; ;
                    string report_name = "FDRHistoryBankWise-" + toDateBWH.Value.ToString("dd-MMM-yy") + ".pdf";

                    ReportDataSource rd = new ReportDataSource();
                    DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(FIbankwise);
                    rd.Name = "StatementBankwise";
                    rd.Value = dtFDRStatement;

                    ReportParameter[] parameters = new ReportParameter[] 
                    {
                     new ReportParameter("SearchDate", toDateBWH.Value.ToString("dd-MMM-yy")),
                     new ReportParameter("ReportHeading","FDR Bank Wise History Statement")
                   
                    };

                    lr.ReportPath = Server.MapPath("~/Reports/FDRStatementBankWise.rdlc");
                    lr.SetParameters(parameters);

                    lr.DataSources.Add(rd);




                    string ContentType = "application/vnd.ms-excel";
                    string FileType;
                    string ReportName;

                    if (IsExcell == "true")
                    {
                        FileType = "Excel";
                        ReportName = "FDRHistoryBankWise-" + toDateBWH.Value.ToString("dd-MMM-yy") + ".{0}";
                    }
                    else
                    {
                        FileType = "PDF";
                        ReportName = "FDRHistoryBankWise-" + toDateBWH.Value.ToString("dd-MMM-yy") + ".pdf";

                    }


                    string reportType = FileType; // "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                        "  <OutputFormat>" + FileType + "</OutputFormat>" +
                        "  <PageWidth>11.5in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0.5in</MarginTop>" +
                        "  <MarginLeft>0.5in</MarginLeft>" +
                        "  <MarginRight>1in</MarginRight>" +
                        "  <MarginBottom>0.5in</MarginBottom>" +
                        "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;


                    if (IsExcell == "true")
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

                message = "From date or To date is required!!";
                return RedirectToAction("Index", "ErrorPage", new { message });


            }
            catch (NullReferenceException nex)
            {
                string message = nex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
            catch (ArgumentException aex)
            {
                string message = aex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }


        //Copy of FDRBankWiseStatement just take selected bank
        //public ActionResult FDRSelectedBankStatement(DateTime? toDateBWS, string Option, string IsExcell)
        //{
           
        //    try
        //    {
        //        if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
        //        {
        //            return RedirectToAction("LogOut", "Home");

        //        }
        //        toDateBWS = DateTime.Now.Date;

        //        List<FIXEDDEPOSIT> models = new List<FIXEDDEPOSIT>();

        //        if (toDateBWS != null)  //fromDateFS != null && 
        //        {
        //            Entities db = new Entities(Session["Connection"] as EntityConnection);
        //            //Sum of All Principal Amount of FixedDeposit table where Status= Approved

        //            var totalPrincialAmount = db.FIXEDDEPOSITs.Where(t => (t.STATUS == ConstantVariable.STATUS_APPROVED || t.STATUS == ConstantVariable.STATUS_PENDING) && EntityFunctions.TruncateTime(t.OPENINGDATE.Value) <= EntityFunctions.TruncateTime(toDateBWS.Value)).ToList().Sum(t => t.PRINCIPALAMOUNT ?? 0);  //t.MATURITYDATE.Value                     

        //            if (totalPrincialAmount == null)
        //                totalPrincialAmount = 1;
        //            if (totalPrincialAmount <= 0)
        //                totalPrincialAmount = 1;


        //            var FIbankwise = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(t => t.NAME).Select(t => new
        //            {
        //                BankName = t.NAME,
        //                FDRNo = t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_PENDING) && EntityFunctions.TruncateTime(x.OPENINGDATE) <= EntityFunctions.TruncateTime(toDateBWS.Value)).Count(),

        //                ExistingCapLimit = t.CAPLIMIT.Value,
        //                PrincipalAmountTK = Math.Round((t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_PENDING) && EntityFunctions.TruncateTime(x.OPENINGDATE.Value) <= EntityFunctions.TruncateTime(toDateBWS.Value)).Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),                          
        //                PercentagetotalFDR = Math.Round(((t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_PENDING) && EntityFunctions.TruncateTime(x.OPENINGDATE.Value) <= EntityFunctions.TruncateTime(toDateBWS.Value)).Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / totalPrincialAmount) * 100, 2),
        //                AvgRate = Math.Round((t.FIXEDDEPOSITs.Where(x => (x.STATUS == ConstantVariable.STATUS_APPROVED || x.STATUS == ConstantVariable.STATUS_PENDING) && EntityFunctions.TruncateTime(x.OPENINGDATE.Value) <= EntityFunctions.TruncateTime(toDateBWS.Value)).Average(x => (decimal?)x.RATEOFINTEREST.Value) ?? 0), 2),                          
        //                Offerrate1 = t.OFFERRATE1,
        //                Offerrate2 = t.OFFERRATE2,
        //                Offerrate3 = t.OFFERRATE3,
        //                NPL = t.NPLPERCENTAGE,
        //                CamelsRating = t.CAMELRATING,
        //                InstitutionType = t.INSTITUTIONTYPE,
        //                Selected = t.ISSELECT

        //            }).OrderByDescending(t => t.Offerrate1).ThenByDescending(t=>t.Offerrate2).ThenByDescending(t=>t.Offerrate3).ToList();



        //            if (Option == "FB") //Only FDR Bank
        //            {
        //                FIbankwise = FIbankwise.Where(t => t.Selected == "Y").ToList();
        //            }

        //            LocalReport lr = new LocalReport();

        //            ReportDataSource rd = new ReportDataSource();
        //            DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(FIbankwise);
        //            rd.Name = "StatementBankwise";
        //            rd.Value = dtFDRStatement;

        //            ReportParameter[] parameters = new ReportParameter[] 
        //            {
        //             new ReportParameter("SearchDate", toDateBWS.Value.ToString("dd-MMM-yy")),
        //             new ReportParameter("ReportHeading","FDR Investment Bank Statement")
                   
        //            };

        //            lr.ReportPath = Server.MapPath("~/Reports/FDRInvestmentBankWise.rdlc");
        //            lr.SetParameters(parameters);

        //            lr.DataSources.Add(rd);


        //            string ContentType = "application/vnd.ms-excel";
        //            string FileType;
        //            string ReportName;

        //            if (IsExcell == "true")
        //            {
        //                FileType = "Excel";
        //                ReportName = "FDRInvestmentBankWise-" + toDateBWS.Value.ToString("dd-MMM-yy") + ".{0}";
        //            }
        //            else
        //            {
        //                FileType = "PDF";
        //                ReportName = "FDRInvestmentBankWise-" + toDateBWS.Value.ToString("dd-MMM-yy") + ".pdf";

        //            }


        //            string reportType = FileType; // "PDF";
        //            string mimeType;
        //            string encoding;
        //            string fileNameExtension;

        //            string deviceInfo =

        //            "<DeviceInfo>" +
        //                "  <OutputFormat>" + FileType + "</OutputFormat>" +
        //                "  <PageWidth>11.5in</PageWidth>" +
        //                "  <PageHeight>11in</PageHeight>" +
        //                "  <MarginTop>0.5in</MarginTop>" +
        //                "  <MarginLeft>0.5in</MarginLeft>" +
        //                "  <MarginRight>1in</MarginRight>" +
        //                "  <MarginBottom>0.5in</MarginBottom>" +
        //                "</DeviceInfo>";

        //            Warning[] warnings;
        //            string[] streams;
        //            byte[] renderedBytes;


        //            if (IsExcell == "true")
        //            {
        //                renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
        //                return File(renderedBytes, ContentType, string.Format(ReportName, fileNameExtension));
        //            }
        //            else
        //            {
        //                renderedBytes = lr.Render(
        //                 reportType,
        //                 deviceInfo,
        //                 out mimeType,
        //                 out encoding,
        //                 out fileNameExtension,
        //                 out streams,
        //                 out warnings);

        //                renderedBytes = lr.Render(reportType);
        //                return File(renderedBytes, mimeType, ReportName);
        //            }

        //        }

        //        message = "From date or To date is required!!";
        //        return RedirectToAction("Index", "ErrorPage", new { message });


        //    }
        //    catch (NullReferenceException nex)
        //    {
        //        string message = nex.Message;
        //        return RedirectToAction("Index", "ErrorPage", new { message });

        //    }
        //    catch (ArgumentException aex)
        //    {
        //        string message = aex.Message;
        //        return RedirectToAction("Index", "ErrorPage", new { message });
        //    }

        //    catch (Exception ex)
        //    {
        //        string message = ex.Message;

        //        return RedirectToAction("Index", "ErrorPage", new { message });
        //    }


        //}

        //Copy as FDRBankWiseHistory
        public ActionResult FDRSelectedBankWiseHistory(DateTime? toDateBWH, string Option, string IsExcell)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                List<FIXEDDEPOSIT> models = new List<FIXEDDEPOSIT>();
                if (toDateBWH != null)  //fromDateFS != null && 
                {
                    Entities db = new Entities(Session["Connection"] as EntityConnection);
                    //Sum of All Principal Amount of FixedDeposit table where Status= Approved

                    //prev 23-3-17               
                    //For history many of FDR will be encash/renwed  so we should check status =Approved || Encashed || Renewed
                    //var totalPrincialAmount = db.FIXEDDEPOSITs.Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED)
                    //.Where(fdr => fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_RENEWAL ? toDateBWH.Value < fdr.RENWALDATE : fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_ENCASH ? toDateBWH.Value < fdr.ENCASHMENTDATE : fdr.OPENINGDATE <= toDateBWH.Value).ToList().Sum(fdr => fdr.PRINCIPALAMOUNT ?? 0);

                    //models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED || fdr.STATUS == ConstantVariable.STATUS_PENDING)
                    //           .Where(fdr => fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_RENEWAL ?
                    //            (EntityFunctions.TruncateTime(toDateBWH.Value) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value) && EntityFunctions.TruncateTime(toDateBWH.Value) < EntityFunctions.TruncateTime(fdr.RENWALDATE.Value)) : //accepted Date
                    //            fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_ENCASH ?
                    //            (EntityFunctions.TruncateTime(toDateBWH.Value) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value) && EntityFunctions.TruncateTime(toDateBWH.Value) < EntityFunctions.TruncateTime(fdr.ENCASHMENTDATE.Value)) : //Accepted Date
                    //            EntityFunctions.TruncateTime(toDateBWH.Value) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE.Value))
                    //           .OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();

                    models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED || fdr.STATUS == ConstantVariable.STATUS_PENDING)
                            .Where(fdr => (EntityFunctions.TruncateTime(toDateBWH.Value) >= EntityFunctions.TruncateTime(fdr.OPENINGDATE)) && (EntityFunctions.TruncateTime(toDateBWH.Value) < EntityFunctions.TruncateTime(fdr.MATURITYDATE)))   //fdr.RENWALDATE.Value
                             //this is for approved status //fdr.OPENINGDATE <= toDateFS                                                        
                            .OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();
                    
           

                    var totalPrincialAmount = models.ToList().Sum(fdr => fdr.PRINCIPALAMOUNT ?? 0);

                    if (totalPrincialAmount == null)
                        totalPrincialAmount = 1;
                    if (totalPrincialAmount <= 0)
                        totalPrincialAmount = 1;
              
                    var FIbankwise = (from fi in db.FINANCIALINSTITUTIONs.ToList()
                                      select new
                                      {
                                          BankName = fi.NAME,
                                          FDRNo = models.Where(t => t.FINANCIALINSTITUTION_REFERENCE == fi.REFERENCE).ToList().Count(),
                                          ExistingCapLimit = fi.CAPLIMIT.Value,

                                          PrincipalAmountTK = Math.Round((models.Where(t => t.FINANCIALINSTITUTION_REFERENCE == fi.REFERENCE).ToList().Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / 10000000, 2),
                                          PercentagetotalFDR = Math.Round(((models.Where(t => t.FINANCIALINSTITUTION_REFERENCE == fi.REFERENCE).ToList().Sum(x => (decimal?)x.PRINCIPALAMOUNT.Value) ?? 0) / totalPrincialAmount) * 100, 2),
                                          AvgRate = Math.Round((models.Where(t=>t.FINANCIALINSTITUTION_REFERENCE == fi.REFERENCE).ToList().Average(x => (decimal?)x.RATEOFINTEREST.Value) ?? 0), 2),                          
                     
                                          Offerrate1 = fi.OFFERRATE1,
                                          Offerrate2 = fi.OFFERRATE2,
                                          Offerrate3 = fi.OFFERRATE3,
                                          NPL = fi.NPLPERCENTAGE,
                                          CamelsRating = fi.CAMELRATING,
                                          InstitutionType = fi.INSTITUTIONTYPE,
                                          Selected = fi.ISSELECT

                                    }).OrderByDescending(t => t.Offerrate1).ThenByDescending(t=>t.Offerrate2).ThenByDescending(t=>t.Offerrate3).ToList();



                    if (Option == "FB") //Only FDR Bank
                    {
                        FIbankwise = FIbankwise.Where(t => t.Selected == "Y").ToList();
                    }

                    LocalReport lr = new LocalReport();

                    // string report_title = "FDR Encashment Tax Statement"; ;
                    string report_name = "FDRInvestmentBankWiseHistory-" + toDateBWH.Value.ToString("dd-MMM-yy") + ".pdf";

                    ReportDataSource rd = new ReportDataSource();
                    DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(FIbankwise);
                    rd.Name = "StatementBankwise";
                    rd.Value = dtFDRStatement;

                    ReportParameter[] parameters = new ReportParameter[] 
                    {
                     new ReportParameter("SearchDate", toDateBWH.Value.ToString("dd-MMM-yy")),
                     new ReportParameter("ReportHeading","FDR Investment Bank Wise History")
                   
                    };

                    lr.ReportPath = Server.MapPath("~/Reports/FDRInvestmentBankWise.rdlc");
                    lr.SetParameters(parameters);

                    lr.DataSources.Add(rd);




                    string ContentType = "application/vnd.ms-excel";
                    string FileType;
                    string ReportName;

                    if (IsExcell == "true")
                    {
                        FileType = "Excel";
                        ReportName = "FDRInvestmentBankWiseHistory-" + toDateBWH.Value.ToString("dd-MMM-yy") + ".{0}";
                    }
                    else
                    {
                        FileType = "PDF";
                        ReportName = "FDRInvestmentBankWiseHistory-" + toDateBWH.Value.ToString("dd-MMM-yy") + ".pdf";

                    }


                    string reportType = FileType; // "PDF";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                        "  <OutputFormat>" + FileType + "</OutputFormat>" +
                        "  <PageWidth>11.5in</PageWidth>" +
                        "  <PageHeight>11in</PageHeight>" +
                        "  <MarginTop>0.5in</MarginTop>" +
                        "  <MarginLeft>0.5in</MarginLeft>" +
                        "  <MarginRight>1in</MarginRight>" +
                        "  <MarginBottom>0.5in</MarginBottom>" +
                        "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;


                    if (IsExcell == "true")
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

                message = "From date or To date is required!!";
                return RedirectToAction("Index", "ErrorPage", new { message });


            }
            catch (NullReferenceException nex)
            {
                string message = nex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
            catch (ArgumentException aex)
            {
                string message = aex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }



        #endregion
        /// <summary>
        /// 
        /// </summary>

        public YearlyInterestSlot GetGrossSlot(FIXEDDEPOSIT oFIXEDDEPOSIT)
        {

            YearlyInterestSlot SingleReport = new YearlyInterestSlot();

            int opeingYear = oFIXEDDEPOSIT.OPENINGDATE.Value.Year;
            int MaturedYear = oFIXEDDEPOSIT.MATURITYDATE.Value.Year;

            //check is the year Same or only one slot when Opening Date and Maturity Date same
            //only one year slot so RecivableGross=0;
            //And gross,Tax,ED,PO,NET will be FixedDeposit value

            if (opeingYear == MaturedYear)
            {
                SingleReport.ReceivableGross = 0;
                SingleReport.Gross = oFIXEDDEPOSIT.GROSSINTEREST == null ? 0 : oFIXEDDEPOSIT.GROSSINTEREST.Value;
                SingleReport.SourceTax = oFIXEDDEPOSIT.SOURCETAX == null ? 0 : oFIXEDDEPOSIT.SOURCETAX.Value;
                SingleReport.ED = oFIXEDDEPOSIT.EXCISEDUTY == null ? 0 : oFIXEDDEPOSIT.EXCISEDUTY.Value;
                SingleReport.PO = oFIXEDDEPOSIT.OTHERCHARGE == null ? 0 : oFIXEDDEPOSIT.OTHERCHARGE.Value;
                SingleReport.Net = oFIXEDDEPOSIT.NETINTERESTRECEIVABLE == null ? 0 : oFIXEDDEPOSIT.NETINTERESTRECEIVABLE.Value;

            }
            else                //when many year slot 
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

                //get how many year slot
                int lastYearSlot = 0;
                decimal receivableGross = 0;

                for (int year = opeingYear; year <= MaturedYear; year++)
                {
                    if (year == MaturedYear)
                    {
                        ReceivedOn = oFIXEDDEPOSIT.MATURITYDATE.Value.ToString("dd-MMM-yy");
                        openDateDifference = oFIXEDDEPOSIT.MATURITYDATE.Value - OpeningYearFirstDate;
                        TotalDaysThisYear = openDateDifference.TotalDays;
                        lastYearSlot = 1;
                    }
                    else
                    {
                        OpeningYearLastDate = new DateTime(year, 12, 31);
                        ReceivedOn = new DateTime(year, 12, 31).ToString("dd-MMM-yy");

                        openDateDifference = OpeningYearLastDate - OpeningYearFirstDate;
                        TotalDaysThisYear = openDateDifference.TotalDays;
                    }

                    //incriment one day to get next Opening Year First Date
                    OpeningYearFirstDate = OpeningYearLastDate;
                    //.AddDays(1);

                    var NetInterestReceivable = Math.Round((PerDayNetInterest * Convert.ToDecimal(TotalDaysThisYear)), 2);
                    var SourceTax = Math.Round((PerDaySourceTax * Convert.ToDecimal(TotalDaysThisYear)), 2);
                    var GrossInterest = Math.Round((PerDayGrossInterest * Convert.ToDecimal(TotalDaysThisYear)), 2);

                    //RecivableGross=Summation of all GrossInterest except last year slot  
                    if (lastYearSlot == 0)
                    {
                        receivableGross += GrossInterest;
                    }
                    else
                    {
                        // this is the last year slot now calculate ReceivableGross,(Gross,Tax,ED,PO,Net) from last year slot

                        SingleReport.ReceivableGross = receivableGross;
                        SingleReport.Gross = GrossInterest; //last year slot Gross
                        SingleReport.SourceTax = SourceTax;//last year slot tax
                        SingleReport.ED = oFIXEDDEPOSIT.EXCISEDUTY;
                        SingleReport.PO = oFIXEDDEPOSIT.OTHERCHARGE;
                        SingleReport.Net = NetInterestReceivable;

                    }
                }
            }
            return SingleReport;

        }

        #region STOCK_REPORT

        public ActionResult StockBuySellByDate(string tradeCode, DateTime? fromDate, DateTime? toDate)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            var db = new Entities(Session["Connection"] as EntityConnection);

            List<BuySellstatement> models = new List<BuySellstatement>();
            ReportDataSource oPortfolioStatement = new ReportDataSource();

            decimal? commision = db.BROKERs.Where(t => t.CDBLID == ConstantVariable.CDBL_DP_ID).SingleOrDefault().COMMISSIONRATE; // "34200"

            //commision = commision.Value / 100;

            models = new Portfolio().GetBuySellStatement(fromDate.HasValue ? fromDate.Value.ToString("dd - MMM - yy") : null, toDate.HasValue ? toDate.Value.ToString("dd - MMM - yy") : null, tradeCode, commision.Value, null);

            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode) && models != null)
                models = models.Where(t => t.TradeCode == tradeCode).ToList();

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/StockBuySellStatement.rdlc");

            //string report_title = "FDR Main Statement";;
            string report_name = "Buy Sell " + fromDate.Value.ToString("dd-MMM-yy") + " to " + toDate.Value.ToString("dd-MMM-yy") + ".pdf";


            ReportDataSource rd = new ReportDataSource();
            //ReportDataSource dd = new ReportDataSource();

            DataTable dtBuySelltatement = oCommonFunction.ConvertToDataTable(models.ToList());
            rd.Name = "StockBuySell";              //"FixedDeposit";
            rd.Value = dtBuySelltatement;


            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("FromDate", fromDate.HasValue?fromDate.Value.ToString("dd-MMM-yy"):null),
                             new ReportParameter("ToDate",  toDate.HasValue? toDate.Value.ToString("dd-MMM-yy"):null)                                
                           };


            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/StockBuySellStatement.rdlc");
            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>10in</PageWidth>" +
                "  <PageHeight>8.5in</PageHeight>" +
                "  <MarginTop>0.75in</MarginTop>" +
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

        public ActionResult TradeOrderByDate(DateTime? OnDate, string tradeCode, string IsPending, string IsExcel)
        {
            if (OnDate == null)
                return RedirectToAction("Index", "ErrorPage", new { message = "Date is required!" });

            string FundPosition = "";
            ReportDataSource oPortfolioStatement = new ReportDataSource();

            Entities db = new Entities(Session["Connection"] as EntityConnection);
            List<TradeOrderViewModel> models = new List<TradeOrderViewModel>();
            string Status = "";

            if (IsPending == "true")
            {
                models = db.SCRIPTTRANSFERs.Where(t => (t.TRANSACTIONTYPE == "B" || t.TRANSACTIONTYPE == "S")).Select(i => new   //
                TradeOrderViewModel
                  {
                      TranDate = i.ENTRYDATE,
                      AccountNumber = i.ACCOUNTNUMBER,
                      Qty = i.SHAREQTY.Value,
                      ClosingPrice = i.RATE.Value,
                      Total = i.TOTAL.Value,

                      LowerLimit = i.LOWERLIMIT,
                      UpperLimit = i.UPPERLIMIT,
                      MaximumQty = i.MAXIMUMQTY,
                      TransactionType = i.TRANSACTIONTYPE,
                      Instrument = i.INSTRUMENT.SHORTNAME,
                      FundPosition = i.DESCRIPTION
                  }).ToList();

                Status = ConstantVariable.STATUS_PENDING;
            }
            else
            {
                models = db.SCRIPTTRANSFERs.Where(t => (t.TRANSACTIONTYPE == "B" || t.TRANSACTIONTYPE == "S") && t.STATUS == ConstantVariable.STATUS_APPROVED).Select(i => new   //
                  TradeOrderViewModel
                  {
                      TranDate = i.ENTRYDATE,
                      AccountNumber = i.ACCOUNTNUMBER,
                      Qty = i.SHAREQTY.Value,
                      ClosingPrice = i.RATE.Value,
                      Total = i.TOTAL.Value,

                      LowerLimit = i.LOWERLIMIT,
                      UpperLimit = i.UPPERLIMIT,
                      MaximumQty = i.MAXIMUMQTY,
                      TransactionType = i.TRANSACTIONTYPE,
                      Instrument = i.INSTRUMENT.SHORTNAME,
                      FundPosition = i.DESCRIPTION
                  }).ToList();
            }
            if (OnDate != null)
                models = models.Where(t => t.TranDate == OnDate.Value).ToList();

            var fundPosition = models.Where(t => (t.TransactionType == "B" || t.TransactionType == "S") && t.FundPosition != null).Take(1).SingleOrDefault();

            if (fundPosition != null)
                FundPosition = Convert.ToString(fundPosition.FundPosition);

            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode))
            {
                models = models.Where(t => t.Instrument == tradeCode).ToList();
            }

            var BuyStatement = models.Where(t => t.TransactionType == "B").OrderBy(t => t.Instrument).ToList();
            var SaleStatement = models.Where(t => t.TransactionType == "S").OrderBy(t => t.Instrument).ToList();


            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/TradeOrder.rdlc");

            //string report_title = "FDR Main Statement";;
            string report_name = "TradeOrder-" + OnDate.Value.ToString("dd-MMM-yy");
            
            string ContentType = "application/vnd.ms-excel"; ;
            string FileType;

            if (IsExcel == "true")
            {
                FileType = "Excel";
                report_name = report_name + ".{0}";
            }
            else
            {
                FileType = "PDF";
                report_name = report_name  + ".pdf";
            }

            ReportDataSource rd = new ReportDataSource();
            ReportDataSource dd = new ReportDataSource();

            // DataTable dtBuySelltatement = oCommonFunction.ConvertToDataTable(SaleStatement);
            rd.Name = "SaleStatement";
            rd.Value = oCommonFunction.ConvertToDataTable(SaleStatement);

            dd.Name = "BuyStatement";
            dd.Value = oCommonFunction.ConvertToDataTable(BuyStatement);

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("OnDate", OnDate.Value.ToString("dd-MMM-yy")),
                             new ReportParameter("FundPosition",FundPosition),   
                             new ReportParameter("Status",Status)
                           };


            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/TradeOrder.rdlc");
            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);
            lr.DataSources.Add(dd);

            // lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);

            string reportType = FileType; // "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>" + FileType + "</OutputFormat>" +
                "  <PageWidth>10in</PageWidth>" +
                "  <PageHeight>8.5in</PageHeight>" +
                "  <MarginTop>0.75in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            //renderedBytes = lr.Render(
            //    reportType,
            //    deviceInfo,
            //    out mimeType,
            //    out encoding,
            //    out fileNameExtension,
            //    out streams,
            //    out warnings);


            //renderedBytes = lr.Render(reportType);

            //return File(renderedBytes, mimeType, report_name);


            if (IsExcel == "true")
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

        public ActionResult fractionReceivedByDate(string tradeCode, DateTime? fromDate, DateTime? toDate)
        {

            if (fromDate == null || toDate == null)
                return RedirectToAction("Index", "ErrorPage", new { message = "From Date and To Date Date is required!" });


            ReportDataSource oPortfolioStatement = new ReportDataSource();
            Entities db = new Entities(Session["Connection"] as EntityConnection);

            var models = db.SCRIPTTRANSFERs.Where(t => t.TRANSACTIONTYPE == "F").Select(i => new
                  {
                      EntryDate = i.ENTRYDATE,
                      Instrument = i.INSTRUMENT.SHORTNAME,
                      Qty = i.SHAREQTY.Value,
                      Total = i.TOTAL.Value,
                      Reference = i.DESCRIPTION,
                      RecordDate = i.RECORDDATE

                  }).ToList();


            if (fromDate != null)
                models = models.Where(t => t.RecordDate >= fromDate.Value).ToList();
            if (toDate != null)
                models = models.Where(t => t.RecordDate <= toDate.Value).ToList();

            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode))
                models = models.Where(t => t.Instrument == tradeCode).ToList();

            LocalReport lr = new LocalReport();
       

            //string report_title = "FDR Main Statement";;
            string report_name = "FractionReceivedStatement-" + fromDate.Value.ToString("dd-MMM-yy") + "-" + toDate.Value.ToString("dd-MMM-yy") + ".pdf";
            ReportDataSource rd = new ReportDataSource();


            // DataTable dtBuySelltatement = oCommonFunction.ConvertToDataTable(SaleStatement);
            rd.Name = "FractionReceived";
            rd.Value = oCommonFunction.ConvertToDataTable(models);



            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("FromDate", fromDate.Value.ToString("dd-MMM-yy")),
                             new ReportParameter("ToDate", toDate.Value.ToString("dd-MMM-yy"))                                     
                           };


            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/FractionReceivedStatement.rdlc");
            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);


            // lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>10in</PageWidth>" +
                "  <PageHeight>8.5in</PageHeight>" +
                "  <MarginTop>0.75in</MarginTop>" +
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


        public ActionResult ExtraDividendReceivedByDate(string tradeCode, DateTime? fromDate, DateTime? toDate)
        {

            if (fromDate == null || toDate == null)
                return RedirectToAction("Index", "ErrorPage", new { message = "From Date and To Date Date is required!" });


            ReportDataSource oPortfolioStatement = new ReportDataSource();
            Entities db = new Entities(Session["Connection"] as EntityConnection);

           
            List<EXTRADIVIDENDRECEIVED> model = new List<EXTRADIVIDENDRECEIVED>();

            foreach (var list in GET_EXTRADIVIDEND_RECEIVED(fromDate.Value.ToString("dd-MMM-yy"), toDate.Value.ToString("dd-MMM-yy")).AsEnumerable().ToList())
            {
                model.Add(new EXTRADIVIDENDRECEIVED
                {
                    INSTRUMENTACREF =list["INSTRUMENTACREF"].ToString(),
                    TAXRATE = Convert.ToDecimal(list["TAXRATE"].ToString()),
                    CASHRECEIVEDDATE = Convert.ToDateTime(list["CASHRECEIVEDDATE"].ToString()),
                    RECORDDATE = Convert.ToDateTime(list["RECORDDATE"].ToString()),
                    GROSSAMOUNT = Convert.ToDecimal(list["GROSSAMOUNT"].ToString()),
                    NETCASHDIVIDEND = Convert.ToDecimal(list["NETCASHDIVIDEND"].ToString()),
                    FOLIONUMBER = list["FOLIONUMBER"].ToString()
                });
            }
           

            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode))
                model = model.Where(t => t.INSTRUMENTACREF == tradeCode).ToList();

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/ExtraDividendReceived.rdlc");

            //string report_title = "FDR Main Statement";;
            string report_name = "ExtraDividendReceived-" + fromDate.Value.ToString("dd-MMM-yy") + "-" + toDate.Value.ToString("dd-MMM-yy") + ".pdf";
            ReportDataSource rd = new ReportDataSource();


            // DataTable dtBuySelltatement = oCommonFunction.ConvertToDataTable(SaleStatement);
            rd.Name = "ExtraDividendReceived";
            rd.Value = oCommonFunction.ConvertToDataTable(model);



            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("FromDate", fromDate.Value.ToString("dd-MMM-yy")),
                             new ReportParameter("ToDate", toDate.Value.ToString("dd-MMM-yy"))                                     
                           };


            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/ExtraDividendReceived.rdlc");
            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);


            // lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>10in</PageWidth>" +
                "  <PageHeight>8.5in</PageHeight>" +
                "  <MarginTop>0.75in</MarginTop>" +
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





        public ActionResult CommissionStatementByDate(string tradeCode, DateTime? fromDate, DateTime? toDate)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            var db = new Entities(Session["Connection"] as EntityConnection);

            List<BuySellstatement> model = new List<BuySellstatement>();
            ReportDataSource oPortfolioStatement = new ReportDataSource();


            decimal? commision = db.BROKERs.Where(t => t.CDBLID == ConstantVariable.CDBL_DP_ID).SingleOrDefault().COMMISSIONRATE; // "34200"


            foreach (var dr in new Portfolio().GetCommisionStatement(fromDate.Value, toDate.Value).AsEnumerable().ToList())
            {
                model.Add(new BuySellstatement
                {

                    //AccountNumber = dr["AccountNumber"].ToString(),
                    TradeCode = dr["SHORTNAME"].ToString(),
                    TransactionType = dr["TRANTYPE"].ToString(),
                    BuyQty = Convert.ToDouble(dr["BUY"].ToString()),
                    BuyAmount = Convert.ToDouble(dr["AMOUNT"].ToString()),
                    SellAmount = Convert.ToDouble(dr["AMOUNT"].ToString()),
                    Net = Convert.ToDouble(dr["NETBUYAMOUNT"].ToString()),
                    Commission = double.Parse(dr["COMMISSION"].ToString()),
                    CommissionPercentage = Convert.ToDouble(commision.Value),

                });

            }

            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode) && model != null)
                model = model.Where(t => t.TradeCode == tradeCode).ToList();

            var BuyCommisssion = model.Where(t => t.TransactionType == ConstantVariable.TranTypeShareBought).OrderBy(t => t.TradeCode).ToList();

            var SaleCommission = model.Where(t => t.TransactionType == ConstantVariable.TranTypeShareSold).OrderBy(t => t.TradeCode).ToList();

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/CommissionStatement.rdlc");

            //string report_title = "FDR Main Statement";;
            string report_name = "Commission " + fromDate.Value.ToString("dd-MMM-yy") + " to " + toDate.Value.ToString("dd-MMM-yy") + ".pdf";

            ReportDataSource rd = new ReportDataSource();
            ReportDataSource dd = new ReportDataSource();

            DataTable dtBuySelltatement = oCommonFunction.ConvertToDataTable(BuyCommisssion); //
            rd.Name = "CommissionStatement";              
            rd.Value = dtBuySelltatement;

            dd.Name = "CommissionSaleStatement";
            dd.Value = oCommonFunction.ConvertToDataTable(SaleCommission);


            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("FromDate", fromDate.HasValue?fromDate.Value.ToString("dd-MMM-yy"):null),
                             new ReportParameter("ToDate",  toDate.HasValue? toDate.Value.ToString("dd-MMM-yy"):null)                                
                           };

            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/CommissionStatement.rdlc");
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
                "  <PageWidth>10in</PageWidth>" +
                "  <PageHeight>8.5in</PageHeight>" +
                "  <MarginTop>0.75in</MarginTop>" +
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

        public ActionResult PayInStatementByDate(string tradeCode, DateTime? fromDate, DateTime? toDate)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            var db = new Entities(Session["Connection"] as EntityConnection);

            List<BuySellstatement> models = new List<BuySellstatement>();
            ReportDataSource oPortfolioStatement = new ReportDataSource();

            decimal? commision = db.BROKERs.Where(t => t.CDBLID == "34200").SingleOrDefault().COMMISSIONRATE;
            models = new Portfolio().GetBuySellStatement(fromDate.HasValue ? fromDate.Value.ToString("dd - MMM - yy") : null, toDate.HasValue ? toDate.Value.ToString("dd - MMM - yy") : null, tradeCode, commision.Value, ConstantVariable.SettlementPayIn);

            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode) && models != null)
                models = models.Where(t => t.TradeCode == tradeCode).ToList();

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/PayInStatement.rdlc");

            //string report_title = "FDR Main Statement";;
            string report_name = "PayIn " + fromDate.Value.ToString("dd-MMM-yy") + " to " + toDate.Value.ToString("dd-MMM-yy") + ".pdf";

            ReportDataSource rd = new ReportDataSource();
            //ReportDataSource dd = new ReportDataSource();

            DataTable dtBuySelltatement = oCommonFunction.ConvertToDataTable(models.ToList());
            rd.Name = "PayInStatement";              //"FixedDeposit";
            rd.Value = dtBuySelltatement;

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("FromDate", fromDate.HasValue?fromDate.Value.ToString("dd-MMM-yy"):null),
                             new ReportParameter("ToDate",  toDate.HasValue? toDate.Value.ToString("dd-MMM-yy"):null)                                
                           };

            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/PayInStatement.rdlc");
            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>10in</PageWidth>" +
                "  <PageHeight>8.5in</PageHeight>" +
                "  <MarginTop>0.75in</MarginTop>" +
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

        //public ActionResult ProfitLossStatementByDate(string tradeCode, DateTime? fromDate, DateTime? toDate)
        //{

        //    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
        //    {
        //        return RedirectToAction("LogOut", "Home");
        //    }

        //    var db = new Entities(Session["Connection"] as EntityConnection);


        //    List<BuySellstatement> models = new List<BuySellstatement>();
        //    //List<PortfolioInstrument> getPortfolio = new List<PortfolioInstrument>();

        //    //ReportDataSource oPortfolioStatement = new ReportDataSource();

        //    //decimal? commision = db.BROKERs.Where(t => t.CDBLID == ConstantVariable.CDBL_DP_ID).SingleOrDefault().COMMISSIONRATE; // "34200"           
        //    //models = new Portfolio().GetBuySellStatement(fromDate.HasValue ? fromDate.Value.ToString("dd - MMM - yy") : null, toDate.HasValue ? toDate.Value.ToString("dd - MMM - yy") : null, tradeCode, commision.Value, null); //ConstantVariable.SettlementPayIn                    

        //    //if (tradeCode != null && !string.IsNullOrEmpty(tradeCode) && models != null)
        //    //{
        //    //    models = models.Where(t => t.TradeCode == tradeCode).ToList();
        //    //    getPortfolio = new Portfolio().GetInvestorPortfolio(toDate.Value.ToString("dd - MMM - yy"), Convert.ToString(tradeCode), null);
        //    //}
        //    //else
        //    //{
        //    //    getPortfolio = new Portfolio().GetInvestorPortfolio(toDate.Value.ToString("dd - MMM - yy"), null, null);
        //    //}

          
        //    ////Note  that GetBuySellStatement Share Bought and Sold are combained so if any instrument
        //    ////both have Bought and Sold but TranType may be Share Bought.But profit Loss will be Share Sold 
        //    ////so in this case TranType Share Sold filtering can not be applied on GetBuySellStatement
        //    ////such reason we can apply condition on SellQty >0 

        //    //models = (from e in models
        //    //          join pf in getPortfolio on e.TradeCode equals pf.Instrument
        //    //          where e.SellQty > 0
        //    //          select new BuySellstatement
        //    //          {
        //    //              AccountNumber = e.AccountNumber,
        //    //              TradeCode = e.TradeCode,
        //    //              SellQty = e.SellQty,
        //    //              BuyAmount = e.SellQty * pf.AverageCost,
        //    //              BuyPrice = e.BuyPrice,
        //    //              BuyQty = e.BuyQty,
        //    //              SellAmount = e.SellAmount, //  e.SellQty * pf.MarketRate,
        //    //              Commission = e.Commission,
        //    //              Gross = e.Gross,
        //    //              NetProfitLoss = e.NetProfitLoss

        //    //          }).ToList();



        //    foreach (var list in new Portfolio().GetProfitGainLossProcedure("00001", toDate.Value.ToString("dd-MMM-yy"), fromDate.Value.ToString("dd-MMM-yy"), tradeCode).AsEnumerable().ToList())
        //    {
        //        model.Add(new BuySellstatement
        //        {

        //            AccountNumber = list["AccountNumber"].ToString(),
        //            ShortName = list["ShortName"].ToString(),
        //            BuyQty = Convert.ToDecimal(list["BuyQty"]),
        //            NetBuyAmount = Convert.ToDecimal(list["NetBuyAmount"]),
        //            SaleQty = Convert.ToDecimal(list["SaleQty"]),
        //            NetSaleAmount = Convert.ToDecimal(list["NetSaleAmount"]),
        //            RealizedGain = Convert.ToDecimal(list["RealizedGain"])
        //        });
        //    }

        //    LocalReport lr = new LocalReport();
        //    lr.ReportPath = Server.MapPath("~/Reports/ShareReport/ProfitLossStatement.rdlc");

        //    //string report_title = "FDR Main Statement";;
        //    string report_name = "ProfitLossStatement-" + fromDate.Value.ToString("dd-MMM-yy") + "-" + toDate.Value.ToString("dd-MMM-yy") + ".pdf";

        //    ReportDataSource rd = new ReportDataSource();
        //    //ReportDataSource dd = new ReportDataSource();

        //    DataTable dtBuySelltatement = oCommonFunction.ConvertToDataTable(models.ToList());
        //    rd.Name = "ProfitLossStatement";
        //    rd.Value = dtBuySelltatement;

        //    ReportParameter[] parameters = new ReportParameter[] 
        //                   {
        //                     new ReportParameter("FromDate", fromDate.HasValue?fromDate.Value.ToString("dd-MMM-yy"):null),
        //                     new ReportParameter("ToDate",  toDate.HasValue? toDate.Value.ToString("dd-MMM-yy"):null)                                
        //                   };

        //    lr.ReportPath = Server.MapPath("~/Reports/ShareReport/ProfitLossStatement.rdlc");
        //    lr.SetParameters(parameters);
        //    lr.DataSources.Add(rd);

        //    string reportType = "PDF";
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension;

        //    string deviceInfo =

        //    "<DeviceInfo>" +
        //        "  <OutputFormat>PDF</OutputFormat>" +
        //        "  <PageWidth>10in</PageWidth>" +
        //        "  <PageHeight>8.5in</PageHeight>" +
        //        "  <MarginTop>0.75in</MarginTop>" +
        //        "  <MarginLeft>0.5in</MarginLeft>" +
        //        "  <MarginRight>0.5in</MarginRight>" +
        //        "  <MarginBottom>0.5in</MarginBottom>" +
        //        "</DeviceInfo>";

        //    Warning[] warnings;
        //    string[] streams;
        //    byte[] renderedBytes;

        //    renderedBytes = lr.Render(
        //        reportType,
        //        deviceInfo,
        //        out mimeType,
        //        out encoding,
        //        out fileNameExtension,
        //        out streams,
        //        out warnings);

        //    renderedBytes = lr.Render(reportType);
        //    return File(renderedBytes, mimeType, report_name);
        //}

       
        public ActionResult InstrumentWiseLeadger(string tradeCode, DateTime? fromDate, DateTime? toDate)
        {

            List<PortfolioViewModel> model = new List<PortfolioViewModel>();
            model = null;

            model = new Portfolio().GetPortfolioLedgerFN("00001", toDate.Value.ToString("dd-MMM-yy"), tradeCode);

            if (tradeCode == null && string.IsNullOrEmpty(tradeCode))
                tradeCode = "All";


            if (fromDate != null)
            {
                model = model.Where(t => t.TransactionDate >= fromDate).ToList();
            }
            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/InstrumentLedger.rdlc");

            ReportDataSource rd = new ReportDataSource();


            DataTable dtPortfolioStatement = oCommonFunction.ConvertToDataTable(model);
            rd.Name = "PortfolioLedgerInstrument";
            rd.Value = dtPortfolioStatement;

            ReportParameter[] portfolioParameter = new ReportParameter[] 
            {            
             new ReportParameter("Instrument",tradeCode),
             new ReportParameter("Date",toDate.Value.ToString("dd-MMM-yy")),
             new ReportParameter("FromDate",fromDate.HasValue ? fromDate.Value.ToString("dd-MMM-yy"):toDate.Value.ToString("dd-MMM-yy"))
            };

            lr.SetParameters(portfolioParameter);
            lr.DataSources.Add(rd);

            string Reportname = "InstrumentLedgerStatement-"+tradeCode+"-"+ toDate.Value.ToString("dd-MMM-yy") + ".Pdf";
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>11in</PageWidth>" +
                "  <PageHeight>8.5in</PageHeight>" +
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

            return File(renderedBytes, mimeType, Reportname);


        }

        public ActionResult InvestorProfitLossStatement(string tradeCode, string fromDate, string toDate)
        {

            //SELECT ACCOUNTNUMBER,SHORTNAME,SUM(BUYQTY),SUM(NETBUYAMOUNT),SUM(SALEQTY),SUM(NETSALEAMOUNT),SUM(CURRENTBALANCE),SUM(TOTALCOST), SUM(REALIZEDGAIN) 
            //FROM TABLE(GETPORTFOLIO_LEDGER_FN('00001','04-APR-17','ACI'))
            //GROUP BY ACCOUNTNUMBER,SHORTNAME
            //ORDER BY SHORTNAME
            //;

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }



            List<PortfolioViewModel> model = new List<PortfolioViewModel>();
          

            foreach (var list in new Portfolio().GetProfitGainLossProcedure("00001",toDate,fromDate,tradeCode).AsEnumerable().ToList())
            {
                model.Add(new PortfolioViewModel
                {

                  // TotalCost = Convert.ToDecimal(list["CostValue"]), //TotalCost
                   AccountNumber = list["AccountNumber"].ToString(),
                   ShortName = list["ShortName"].ToString(),
                   BuyQty = Convert.ToDecimal(list["BuyQty"]),
                   NetBuyAmount =Convert.ToDecimal(list["NetBuyAmount"]),
                   SaleQty = Convert.ToDecimal(list["SaleQty"]),
                   NetSaleAmount = Convert.ToDecimal(list["NetSaleAmount"]),
                   RealizedGain =Convert.ToDecimal(list["RealizedGain"])
                });
            }

            //Filter thought Profit gain Loss is only for Sale
            model = model.Where(t => t.SaleQty != 0).OrderBy(t=>t.ShortName).ToList();


            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/InvestorProfitLossStatement.rdlc");

            //string report_title = "FDR Main Statement";
            string report_name = "Profit Loss Statement "; //+ fromDate.Value.ToString("dd-MMM-yy") + "-" .ToString("dd-MMM-yy")

            if (fromDate != null && !string.IsNullOrEmpty(fromDate))
                report_name += fromDate + " to ";

            report_name += toDate + ".pdf";

            ReportDataSource rd = new ReportDataSource();

            rd.Name = "ProfitGainLoss";
            rd.Value = oCommonFunction.ConvertToDataTable(model.ToList()); ;

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new  ReportParameter("FromDate",fromDate),  //.Value.ToString("dd-MMM-yyyy")
                             new ReportParameter("ToDate",  toDate)                                 //.Value.ToString("dd-MMM-yyyy")
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
                "  <PageWidth>11in</PageWidth>" +
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

        public ActionResult StockCashReceivable(string tradeCode, DateTime? FromDate, DateTime? ToDate)
        {
            List<StockCashReceivable> model = new List<StockCashReceivable>();

            try
            {
                foreach (var list in GET_STOCK_CASH_RECEIVABLE("00001", FromDate.Value.ToString("dd-MMM-yy"), ToDate.Value.ToString("dd-MMM-yy")).AsEnumerable().ToList())
                { //loop Start

                    if (list != null)
                    {
                        model.Add(new StockCashReceivable
                        {

                            ISIN = list.IsNull("ISIN") ? "" : Convert.ToString(list["ISIN"]),  //.ToString(),
                            ShortName = list.IsNull("ShortName") ? "" : Convert.ToString(list["ShortName"]), //.ToString(),
                            Catype = list.IsNull("Catype") ? "" : Convert.ToString(list["Catype"]), //.ToString(),
                            
                            AmtBonus = Convert.ToDecimal(list["AmtBonus"]),
                            Amtdividend = Convert.ToDecimal(list["Amtdividend"]),
                            Holding = Convert.ToDecimal(list["Holding"]),                           
                            BShareReceivable = Convert.ToDecimal(list["BShareReceivable"]),
                            RShareReceivable = Convert.ToDecimal(list["RShareReceivable"]),
                            MarketPrice = list.IsNull("MarketPrice") ? 0 : Convert.ToDecimal(list["MarketPrice"]),
                            BonusMarketValue = list.IsNull("BonusMarketValue") ? 0 : Convert.ToDecimal(list["BonusMarketValue"]),

                            CashDividendReceivable = list.IsNull("CashDevidentRecivable") ? 0 : Convert.ToDecimal(list["CashDevidentRecivable"]),

                            EffectiveDate = Convert.ToDateTime(list["EffectiveDate"]),
                            RecordDate = Convert.ToDateTime(list["RecordDate"]),

                            Declaration = list.IsNull("Declaration") ? "" : Convert.ToString(list["Declaration"]), //.ToString(),                       
                            Remarks = list.IsNull("Remarks") ? "" : Convert.ToString(list["Remarks"]) //.ToString()


                        });

                    }

                } //End Loop




                if (tradeCode != null && !string.IsNullOrEmpty(tradeCode))
                    model = model.Where(t => t.ShortName == tradeCode).OrderBy(t => t.ShortName).ToList();

                LocalReport lr = new LocalReport();
                lr.ReportPath = Server.MapPath("~/Reports/ShareReport/StockCashReceivableStatement.rdlc");

                //string report_title = "FDR Main Statement";
                string report_name = "Receivable Statement ";

                if (FromDate != null && ToDate != null)
                    report_name += FromDate.Value.ToString("dd-MMM-yy") + " To " + ToDate.Value.ToString("dd-MMM-yy") + ".pdf";

                ReportDataSource rd = new ReportDataSource();

                rd.Name = "StockCashReceivable";
                rd.Value = oCommonFunction.ConvertToDataTable(model.ToList()); ;

                ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new  ReportParameter("FromDate",FromDate.Value.ToString("dd-MMM-yy")),   //.Value.ToString("dd-MMM-yyyy")
                             new ReportParameter("ToDate",  ToDate.Value.ToString("dd-MMM-yy"))       //.Value.ToString("dd-MMM-yyyy")
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
                    "  <PageWidth>11in</PageWidth>" +
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
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        public ActionResult CashDividendReceived(string tradeCode, DateTime? fromDate,DateTime? toDate)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            //Entities db= new Entities(Session["Connection"] as EntityConnection);
            List<DIVIDEND> newDividend = new Portfolio().getDividendList("Report").Where(t => t.STATUS == "Approved").ToList();

            var model = newDividend.Select(t => new
            {

                CashReceivedDate = t.CASHRECEIVEDDATE,
                TradeCode = t.BANKNAME,
                ISIN = t.ISIN,
                QtyAsRecordDate = t.BOHOLDING,
                NetCashDividend = t.NETCASHAMOUNT,
                GrossAmount = t.GROSSCASHAMOUNT,
                Tax = t.TAXAMOUNT,
                TaxRate = t.TAXRATE,
                ReferenceNo = t.REMARKS,
                InstrumentRef = t.BANKNAME

            }).OrderByDescending(t => t.CashReceivedDate).ToList();
                           

            if(fromDate !=null && toDate !=null)
                model = model.Where(t => t.CashReceivedDate >= fromDate && t.CashReceivedDate <= toDate).OrderByDescending(t => t.CashReceivedDate).ToList();

            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode))
                model = model.Where(t => t.TradeCode == tradeCode).OrderByDescending(t => t.CashReceivedDate).ToList();

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/CashDividendReceivedStatement.rdlc");

            //string report_title = "FDR Main Statement";
            string report_name = "Cash Received Statement " + fromDate.Value.ToString("dd-MMM-yyyy");

            if (toDate != null)
                report_name += toDate.Value.ToString("dd-MMM-yyyy") + ".pdf";

            ReportDataSource rd = new ReportDataSource();

            rd.Name = "CashDividendReceived";
            rd.Value = oCommonFunction.ConvertToDataTable(model.ToList()); ;

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new  ReportParameter("FromDate",fromDate.Value.ToString("dd-MMM-yyyy")),
                             new ReportParameter("ToDate", toDate.Value.ToString("dd-MMM-yyyy"))                                 //.Value.ToString("dd-MMM-yyyy")
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
                "  <PageWidth>11in</PageWidth>" +
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


        public ActionResult GetRightShareReceived(string tradeCode, DateTime? fromDate, DateTime? toDate)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            List<StockViewModel> model = new List<StockViewModel>();

            foreach (var list in GET_RIGHT_SHARE_RECEIVED(fromDate.Value.ToString("dd-MMM-yy"),toDate.Value.ToString("dd-MMM-yy")).AsEnumerable().ToList())
            {
                model.Add(new StockViewModel { 
                 EFFECTIVEDATE = Convert.ToDateTime(list["EFFECTIVEDATE"].ToString()),
                 RECORDDATE = Convert.ToDateTime(list["RECORDDATE"].ToString()),
                 SHORTNAME = list["SHORTNAME"].ToString(),
                 PARHOLDING =Convert.ToDecimal(list["PARHOLDING"].ToString()),
                 FREEBALANCE = Convert.ToDecimal(list["FREEBALANCE"].ToString()),
                 PREMIUM = Convert.ToDecimal(list["PREMIUM"].ToString()),
                 CATYPE = list["CATYPE"].ToString(),
                 CURRENTBALANCE = Convert.ToDecimal(list["CURRENTBALANCE"].ToString()),
                 AVGCOST = Convert.ToDecimal(list["AVGCOST"].ToString()),
                 MARKETPRICE = Convert.ToDecimal(list["MARKETPRICE"].ToString()),
                 MARKETVALUE = Convert.ToDecimal(list["MARKETVALUE"].ToString()),
                 REMARKS = Convert.ToString(list["REMARKS"])
                
                });
            }


            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode))
                model = model.Where(t => t.SHORTNAME == tradeCode).ToList();

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/RightShareReceivedStatement.rdlc");

            //string report_title = "FDR Main Statement";
            string report_name = "Right Share Statement " + fromDate.Value.ToString("dd-MMM-yyyy");

            if (toDate != null)
                report_name += toDate.Value.ToString("dd-MMM-yyyy") + ".pdf";

            ReportDataSource rd = new ReportDataSource();

            rd.Name = "RightShareStatement";
            rd.Value = oCommonFunction.ConvertToDataTable(model.ToList()); ;

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new  ReportParameter("FromDate",fromDate.Value.ToString("dd-MMM-yyyy")),
                             new ReportParameter("ToDate", toDate.Value.ToString("dd-MMM-yyyy"))    
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
                "  <PageWidth>11in</PageWidth>" +
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

        //Stock Dividend Received
        public ActionResult GetBonusShareReceived(string tradeCode, DateTime? fromDate, DateTime? toDate)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            List<StockViewModel> model = new List<StockViewModel>();

            foreach (var list in GET_BONUS_SHARE_RECEIVED(fromDate.Value.ToString("dd-MMM-yy"), toDate.Value.ToString("dd-MMM-yy")).AsEnumerable().ToList())
            {
                model.Add(new StockViewModel
                {
                    SHORTNAME = list["SHORTNAME"].ToString(),
                    ISIN = list["ISIN"].ToString(),
                    
                    RECORDDATE = Convert.ToDateTime(list["RECORDDATE"].ToString()),

                    EFFECTIVEDATE = Convert.ToDateTime(list["EFFECTIVEDATE"].ToString()),
                   
                    PARHOLDING = Convert.ToDecimal(list["PARHOLDING"].ToString()),
                    ENTITLEMENT = Convert.ToDecimal(list["ENTITLEMENT"].ToString()),
                    FREEBALANCE = Convert.ToDecimal(list["FREEBALANCE"].ToString()),
                   
                    CURRENTBALANCE = Convert.ToDecimal(list["CURRENTBALANCE"].ToString()),
                  
                    MARKETPRICE = Convert.ToDecimal(list["MARKETPRICE"].ToString()),
                    MARKETVALUE = Convert.ToDecimal(list["MARKETVALUE"].ToString()),
                    REMARKS = Convert.ToString(list["REMARKS"])

                });
            }


            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode))
                model = model.Where(t => t.SHORTNAME == tradeCode).ToList();

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/BonusShareReceivedStatement.rdlc");

            //string report_title = "FDR Main Statement";
            string report_name = "Bonus Share Statement " + fromDate.Value.ToString("dd-MMM-yyyy");

            if (toDate != null)
                report_name += toDate.Value.ToString("dd-MMM-yyyy") + ".pdf";

            ReportDataSource rd = new ReportDataSource();

            rd.Name = "BonusShareReceived";
            rd.Value = oCommonFunction.ConvertToDataTable(model.ToList()); ;

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new  ReportParameter("FromDate",fromDate.Value.ToString("dd-MMM-yyyy")),
                             new ReportParameter("ToDate", toDate.Value.ToString("dd-MMM-yyyy"))    
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
                "  <PageWidth>11in</PageWidth>" +
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

        //GetStockLedgerStatement
        public ActionResult GetStockLedgerStatement(string tradeCode, DateTime? fromDate, DateTime? toDate)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            List<StockViewModel> model = new List<StockViewModel>();

            var OpeningBalance = (from e in GET_CLIENT_OPENING_BALANCE(fromDate.Value.ToString("dd-MMM-yy"), "00001").AsEnumerable().ToList()
                                  select new
                                  {
                                      NetBalance = e.IsNull("NetBalance") ? 0 : Convert.ToDecimal(e["NetBalance"])
                                      //Convert.ToDecimal(e["NetBalance"])                                 

                                  }).SingleOrDefault().NetBalance;



            var LedgerList = (from e in GET_STOCK_LEDGER(fromDate.Value.ToString("dd-MMM-yy"), toDate.Value.ToString("dd-MMM-yy"), "00001").AsEnumerable().ToList()
                              select new
                              {
                               // AccountRef =e["AccountRef"].ToString(),
                               // Name = e["Name"].ToString(),
                                Date = e["Date"].ToString(),
                                Type = e["Type"].ToString(),
                                Description = e["Description"].ToString(),
                                Quantity = Convert.ToDecimal(e["Quantity"]),
                                Rate = Convert.ToDecimal(e["Rate"]),
                                Amount = Convert.ToDecimal(e["Amount"]),
                                Commission = Convert.ToDecimal(e["Commission"]),
                                Debit = Convert.ToDecimal(e["Debit"]),
                                Credit = Convert.ToDecimal(e["Credit"])

                              }).ToList();

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/LedgerStatement.rdlc");

            string report_name = "Bonus Share Statement " + fromDate.Value.ToString("dd-MMM-yyyy");

            if (toDate != null)
                report_name += toDate.Value.ToString("dd-MMM-yyyy") + ".pdf";

            ReportDataSource rd = new ReportDataSource();

            rd.Name = "LedgerStatement";
            rd.Value = oCommonFunction.ConvertToDataTable(LedgerList.ToList()); ;

            ReportParameter[] parameters = new ReportParameter[] 
               {                             
                new  ReportParameter("FromDate",fromDate.Value.ToString("dd-MMM-yyyy")),
                new ReportParameter("ToDate", toDate.Value.ToString("dd-MMM-yyyy")),
                new ReportParameter("OpeningBalance",OpeningBalance.ToString())
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
                "  <PageWidth>11in</PageWidth>" +
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


        #endregion


        #region BOND_REPORT
        public ActionResult TreasuryBondStatement(string tradeCode,string IsHistory, string IsExcel)  // DateTime? fromDate, DateTime? toDate ,
        {

            //SELECT ACCOUNTNUMBER,SHORTNAME,SUM(BUYQTY),SUM(NETBUYAMOUNT),SUM(SALEQTY),SUM(NETSALEAMOUNT),SUM(CURRENTBALANCE),SUM(TOTALCOST), SUM(REALIZEDGAIN) 
            //FROM TABLE(GETPORTFOLIO_LEDGER_FN('00001','04-APR-17','ACI'))
            //GROUP BY ACCOUNTNUMBER,SHORTNAME
            //ORDER BY SHORTNAME
            //;

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            var db = new Entities(Session["Connection"] as EntityConnection);

            List<BOND> model = new List<BOND>();
            
            DateTime currentDate= DateTime.Today;

            string report_name = "TreasuryBondStatement-";

            if (IsHistory == "true")
            {
                model = db.BONDs.Where(t=>t.STATUS == ConstantVariable.STATUS_ENCASHED || t.STATUS == ConstantVariable.STATUS_APPROVED)
                      .Where(t=>t.STATUS == ConstantVariable.STATUS_ENCASHED ? (EntityFunctions.TruncateTime(currentDate) >= EntityFunctions.TruncateTime(t.ACCEPTEDDATE.Value) && EntityFunctions.TruncateTime(currentDate) < EntityFunctions.TruncateTime(t.ENCASHEDDATE.Value) ) 
                      :                               
                      EntityFunctions.TruncateTime(currentDate) >= EntityFunctions.TruncateTime(t.ACCEPTEDDATE.Value))  //this is for approved status                                                     
                      .OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();

            }
            else
            {
                model = db.BONDs.Where(t => t.OPENINGDATE <= currentDate && t.STATUS == ConstantVariable.STATUS_APPROVED).OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();
            }

            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode))
                model = model.Where(t => t.FINANCIALINSTITUTION_REFERENCE == tradeCode).ToList();

            model = (from e in model
                     select new BOND
                     {
                         ACCEPTEDBY = e.ACCEPTEDBY,
                         ACCEPTEDDATE = e.ACCEPTEDDATE,
                         ANNUALDAYS = e.ANNUALDAYS,
                         AUCTION = e.AUCTION,
                         BONDID = e.BONDID,
                         BONDISSUEDATE = e.BONDISSUEDATE,
                         BONDTYPE = e.OPENINGDATE.Value.Year.ToString(),
                         BUYINGPRICE = e.BUYINGPRICE,
                         CHEQUEAMOUNT = e.CHEQUEAMOUNT,
                         CHEQUECLEARINGCHARGE = e.CHEQUECLEARINGCHARGE,
                         CHEQUEDATE = e.CHEQUEDATE,
                         CHEQUEREFERENCE = e.CHEQUEREFERENCE,
                         COMMISSION = e.COMMISSION,
                         COMPOUNDINTERESTINTERVAL = e.COMPOUNDINTERESTINTERVAL,
                         COMPOUNDINTERESTTYPE = e.COMPOUNDINTERESTTYPE,
                         COSTPRICE = e.COSTPRICE,
                         COUPONRATE = e.COUPONRATE,
                         CREATEDBY = e.CREATEDBY,
                         CREATEDDATE = e.CREATEDDATE,
                         DISCOUNT = e.DISCOUNT,
                         EXCISEDUTY = e.EXCISEDUTY,
                         FINANCIALINSTITUTION_REFERENCE = e.FINANCIALINSTITUTION.NAME,
                         FACEVALUE = e.FACEVALUE,
                         GROSSINTEREST = e.GROSSINTEREST,
                         HOLDINGINTERESTPAID = e.HOLDINGINTERESTPAID,
                         HOLDINGPERIOD = e.HOLDINGPERIOD,
                         INTERESTMODE = e.INTERESTMODE,
                         INTERESTPAYMENTPERIOD = e.INTERESTPAYMENTPERIOD,
                         MATURITYDATE = e.MATURITYDATE,
                         LASTUPDATED = e.LASTUPDATED,
                         LASTUPDATEDBY = e.LASTUPDATEDBY,
                         NETINTEREST = e.NETINTEREST,
                         OFFERRATE = e.OFFERRATE,
                         OPENINGDATE = e.OPENINGDATE,
                         OTHERCHARGE = e.OTHERCHARGE,
                         PREMIUMPAID = e.PREMIUMPAID,
                         REFERENCE = e.REFERENCE,
                         REMARKS = e.REMARKS,
                         SEQUENCENUMBER = e.SEQUENCENUMBER,
                         SOURCETAX = e.SOURCETAX,
                         STATUS = e.STATUS,
                         TAXRATE = e.TAXRATE,
                         TENURE = e.TENURE,
                         TENURETERM = e.TENURETERM,
                         TERMSINDAYS = e.TERMSINDAYS,
                         TOTALCOMMISSIONGAIN = e.TOTALCOMMISSIONGAIN,
                         TOTALPURCHASEAMOUNT = e.TOTALPURCHASEAMOUNT,
                         REJECTEDBY = e.REJECTEDBY,
                         REJECTEDDATE = e.REJECTEDDATE

                     }).ToList();


            string ContentType = "application/vnd.ms-excel"; ;
            string FileType;

            report_name += currentDate.ToString("dd-MMM-yy");  // 

            if (IsExcel == "true")
            {               
                FileType = "Excel";
                report_name = report_name + ".{0}";
            }
            else
            {
                FileType = "PDF";
                report_name = report_name + ".pdf";
            }

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/BondReport/TreasuryBondStatement.rdlc");

            //string report_title = "FDR Main Statement";;

            ReportDataSource rd = new ReportDataSource();

            rd.Name = "TreasuryBondStatement";
            rd.Value = oCommonFunction.ConvertToDataTable(model.ToList()); ;

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new  ReportParameter("FromDate",Convert.ToString(currentDate.ToString("dd-MMM-yyyy"))),
                             new ReportParameter("ToDate", currentDate.ToString("dd-MMM-yyyy"))                                
                           };


            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);
                     
           string reportType = FileType;
           string mimeType;
           string encoding;
           string fileNameExtension;
           //The DeviceInfo settings should be changed based on the reportType            
           //http://msdn2.microsoft.com/en-us/library/ms155397.aspx            
           string deviceInfo = "<DeviceInfo>" +
               "  <OutputFormat>" + FileType + "</OutputFormat>" +
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
            
           if (IsExcel == "true")
           {
               renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
               return File(renderedBytes, ContentType, string.Format( report_name, fileNameExtension)); //  return File(renderedBytes, ContentType, string.Format("NameOfFile.{0}", fileNameExtension));
           }
           else
           {      renderedBytes = lr.Render(
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


            //string reportType = "PDF";
            //string mimeType;
            //string encoding;
            //string fileNameExtension;

            //string deviceInfo =

            //"<DeviceInfo>" +
            //    "  <OutputFormat>PDF</OutputFormat>" +
            //    "  <PageWidth>14in</PageWidth>" +
            //    "  <PageHeight>10in</PageHeight>" +
            //    "  <MarginTop>0.5in</MarginTop>" +
            //    "  <MarginLeft>0.5in</MarginLeft>" +
            //    "  <MarginRight>0.5in</MarginRight>" +
            //    "  <MarginBottom>0.5in</MarginBottom>" +
            //    "</DeviceInfo>";

            //Warning[] warnings;
            //string[] streams;
            //byte[] renderedBytes;

            //renderedBytes = lr.Render(
            //    reportType,
            //    deviceInfo,
            //    out mimeType,
            //    out encoding,
            //    out fileNameExtension,
            //    out streams,
            //    out warnings);

            //renderedBytes = lr.Render(reportType);
            //return File(renderedBytes, mimeType, report_name);


        }

        public ActionResult HalfYearlyCouponStatement(string tradeCode, DateTime? fromDate, DateTime? toDate)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            var db = new Entities(Session["Connection"] as EntityConnection);

            List<BOND> model = new List<BOND>();

            string report_name = "halfYearlyCouponInterest-";

            //+fromDate.Value.ToString("dd-MMM-yy") + "-" + toDate.Value.ToString("dd-MMM-yy") + ".pdf";

           // model = db.BONDs.Where(t => t.MATURITYDATE <= toDate && t.STATUS == ConstantVariable.STATUS_APPROVED).ToList();

            List<GOVBONDINTERESTSCHEDULE> InterestSlot = new List<GOVBONDINTERESTSCHEDULE>();

            InterestSlot = db.GOVBONDINTERESTSCHEDULEs.Include("BOND").AsNoTracking().Where(t =>t.STATUS== ConstantVariable.STATUS_APPROVED &&(t.DUEDATE >= fromDate && t.DUEDATE <= toDate)).OrderBy(t=>t.BOND.OPENINGDATE).ToList();

            if (fromDate.HasValue)
            {
                report_name += fromDate.Value.ToString("dd-MMM-yy") + "-";
               // model = model.Where(t => t.MATURITYDATE >= fromDate).ToList();  //Opening Date is Bond buyingDate or Value Date
            }


            if (tradeCode != null && !string.IsNullOrEmpty(tradeCode))
                InterestSlot = InterestSlot.Where(t => t.BOND.FINANCIALINSTITUTION_REFERENCE == tradeCode).ToList();

              //  model = model.Where(t => t.FINANCIALINSTITUTION_REFERENCE == tradeCode).ToList();

            model = (from e in InterestSlot
                     select new BOND
                     {
                         ACCEPTEDBY = e.BOND.ACCEPTEDBY,
                         ACCEPTEDDATE = e.DUEDATE, //this is display in the field Interest Received Date
                         ANNUALDAYS = e.BOND.ANNUALDAYS,
                         AUCTION = e.BOND.AUCTION,
                         BONDID = e.BOND.BONDID,
                         BONDISSUEDATE = e.BOND.BONDISSUEDATE,
                         BONDTYPE = e.BOND.BONDTYPE,
                         BUYINGPRICE = e.BOND.BUYINGPRICE,
                         CHEQUEAMOUNT = e.BOND.CHEQUEAMOUNT,
                         CHEQUECLEARINGCHARGE = e.BOND.CHEQUECLEARINGCHARGE,
                         CHEQUEDATE = e.BOND.CHEQUEDATE,
                         CHEQUEREFERENCE = e.BOND.CHEQUEREFERENCE,
                         COMMISSION = e.BOND.COMMISSION,
                         COMPOUNDINTERESTINTERVAL = e.BOND.COMPOUNDINTERESTINTERVAL,
                         COMPOUNDINTERESTTYPE = e.BOND.COMPOUNDINTERESTTYPE,
                         COSTPRICE = e.BOND.COSTPRICE,
                         COUPONRATE = e.BOND.COUPONRATE,
                         CREATEDBY = e.BOND.CREATEDBY,
                         CREATEDDATE = e.BOND.CREATEDDATE,
                         DISCOUNT = e.BOND.DISCOUNT,
                         EXCISEDUTY = e.BOND.EXCISEDUTY,
                         FINANCIALINSTITUTION_REFERENCE = e.BOND.FINANCIALINSTITUTION.NAME,
                         FACEVALUE = e.BOND.FACEVALUE,

                         GROSSINTEREST = e.GROSSINTEREST,

                         HOLDINGINTERESTPAID = e.BOND.HOLDINGINTERESTPAID,
                         HOLDINGPERIOD = e.BOND.HOLDINGPERIOD,
                         INTERESTMODE = e.BOND.INTERESTMODE,
                         INTERESTPAYMENTPERIOD = e.BOND.INTERESTPAYMENTPERIOD,
                         MATURITYDATE = e.BOND.MATURITYDATE,
                         LASTUPDATED = e.BOND.LASTUPDATED,
                         LASTUPDATEDBY = e.BOND.LASTUPDATEDBY,

                         NETINTEREST =e.NETINTEREST,        // e.BOND.NETINTEREST,

                         OFFERRATE = e.BOND.OFFERRATE,
                         OPENINGDATE = e.BOND.OPENINGDATE,
                         OTHERCHARGE = e.BOND.OTHERCHARGE,
                         PREMIUMPAID = e.BOND.PREMIUMPAID,
                         REFERENCE = e.BOND.REFERENCE,
                         REMARKS = e.BOND.REMARKS,
                         SEQUENCENUMBER = e.BOND.SEQUENCENUMBER,
                         SOURCETAX = e.BOND.SOURCETAX,
                         STATUS = e.BOND.STATUS,
                         TAXRATE = e.BOND.TAXRATE,
                         TENURE = e.BOND.TENURE,
                         TENURETERM = e.BOND.TENURETERM,
                         TERMSINDAYS = e.BOND.TERMSINDAYS,
                         TOTALCOMMISSIONGAIN = e.BOND.TOTALCOMMISSIONGAIN,
                         TOTALPURCHASEAMOUNT = e.BOND.TOTALPURCHASEAMOUNT,
                         REJECTEDBY = e.BOND.REJECTEDBY,
                         REJECTEDDATE = e.BOND.REJECTEDDATE

                     }).ToList();

            #region PrevBond
            //model = (from e in model
            //         select new BOND
            //         {
            //             ACCEPTEDBY = e.ACCEPTEDBY,
            //             ACCEPTEDDATE = e.ACCEPTEDDATE,
            //             ANNUALDAYS = e.ANNUALDAYS,
            //             AUCTION = e.AUCTION,
            //             BONDID = e.BONDID,
            //             BONDISSUEDATE = e.BONDISSUEDATE,
            //             BONDTYPE = e.BONDTYPE,
            //             BUYINGPRICE = e.BUYINGPRICE,
            //             CHEQUEAMOUNT = e.CHEQUEAMOUNT,
            //             CHEQUECLEARINGCHARGE = e.CHEQUECLEARINGCHARGE,
            //             CHEQUEDATE = e.CHEQUEDATE,
            //             CHEQUEREFERENCE = e.CHEQUEREFERENCE,
            //             COMMISSION = e.COMMISSION,
            //             COMPOUNDINTERESTINTERVAL = e.COMPOUNDINTERESTINTERVAL,
            //             COMPOUNDINTERESTTYPE = e.COMPOUNDINTERESTTYPE,
            //             COSTPRICE = e.COSTPRICE,
            //             COUPONRATE = e.COUPONRATE,
            //             CREATEDBY = e.CREATEDBY,
            //             CREATEDDATE = e.CREATEDDATE,
            //             DISCOUNT = e.DISCOUNT,
            //             EXCISEDUTY = e.EXCISEDUTY,
            //             FINANCIALINSTITUTION_REFERENCE = e.FINANCIALINSTITUTION.NAME,
            //             FACEVALUE = e.FACEVALUE,
            //             GROSSINTEREST = e.GROSSINTEREST,
            //             HOLDINGINTERESTPAID = e.HOLDINGINTERESTPAID,
            //             HOLDINGPERIOD = e.HOLDINGPERIOD,
            //             INTERESTMODE = e.INTERESTMODE,
            //             INTERESTPAYMENTPERIOD = e.INTERESTPAYMENTPERIOD,
            //             MATURITYDATE = e.MATURITYDATE,
            //             LASTUPDATED = e.LASTUPDATED,
            //             LASTUPDATEDBY = e.LASTUPDATEDBY,
            //             NETINTEREST = e.NETINTEREST,
            //             OFFERRATE = e.OFFERRATE,
            //             OPENINGDATE = e.OPENINGDATE,
            //             OTHERCHARGE = e.OTHERCHARGE,
            //             PREMIUMPAID = e.PREMIUMPAID,
            //             REFERENCE = e.REFERENCE,
            //             REMARKS = e.REMARKS,
            //             SEQUENCENUMBER = e.SEQUENCENUMBER,
            //             SOURCETAX = e.SOURCETAX,
            //             STATUS = e.STATUS,
            //             TAXRATE = e.TAXRATE,
            //             TENURE = e.TENURE,
            //             TENURETERM = e.TENURETERM,
            //             TERMSINDAYS = e.TERMSINDAYS,
            //             TOTALCOMMISSIONGAIN = e.TOTALCOMMISSIONGAIN,
            //             TOTALPURCHASEAMOUNT = e.TOTALPURCHASEAMOUNT,
            //             REJECTEDBY = e.REJECTEDBY,
            //             REJECTEDDATE = e.REJECTEDDATE

            //         }).ToList();
            #endregion
            report_name += toDate.Value.ToString("dd-MMM-yy") + ".pdf";

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/BondReport/HafYearlyCouponInterest.rdlc");

            //string report_title = "FDR Main Statement";;

            ReportDataSource rd = new ReportDataSource();

            rd.Name = "halfYearlyCouponInterest";
            rd.Value = oCommonFunction.ConvertToDataTable(model.ToList()); ;

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new  ReportParameter("FromDate",Convert.ToString(fromDate.Value.ToString("dd-MMM-yyyy"))),
                             new ReportParameter("ToDate",  toDate.Value.ToString("dd-MMM-yyyy"))                                
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

        public ActionResult InterestIncomeBGTBStatement(string tradeCode, DateTime? SearchDate, string Option, string IsExcell)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            var db = new Entities(Session["Connection"] as EntityConnection);

            List<BOND> model = new List<BOND>();

            string report_name = "InterestIncomeBGTB-" + SearchDate.Value.ToString("dd-MMM-yy");

            DateTime interestDate;


            if (Option == ConstantVariable.COMPOUND_INTEREST_HALFYEARLY_MSG)
                interestDate = new DateTime(SearchDate.Value.Year, 6, 30); //30th-Jun of given year
            else if (Option == ConstantVariable.COMPOUND_INTEREST_QUARTERLY_MSG)
                interestDate = new DateTime(SearchDate.Value.Year, 3, 31); //31th-March of given year
            else if (Option == ConstantVariable.COMPOUND_INTEREST_3RD_QUARTERLY_MSG)
                interestDate = new DateTime(SearchDate.Value.Year, 9, 30);
            else if (Option == ConstantVariable.COMPOUND_INTEREST_YEARLY_MSG)
                interestDate = new DateTime(SearchDate.Value.Year, 12, 31); //31th-Dec of given year
            else
                interestDate = SearchDate.Value;

            List<InterestIncomeBGTBViewModel> interest = new List<InterestIncomeBGTBViewModel>();
            try
            {

                string UpToDate = Convert.ToString(new DateTime(interestDate.AddYears(-1).Year, 12, 31).ToString("dd-MMM-yyyy"));

                interest = new BondBGTB().GetInterestIncomeBGTB_FN(tradeCode, interestDate.ToString("dd-MMM-yy"), UpToDate, Option);

                #region prvCode
                //List<GOVBONDINTERESTSCHEDULE> InterestSlot = new List<GOVBONDINTERESTSCHEDULE>();

                //InterestSlot = db.GOVBONDINTERESTSCHEDULEs.AsNoTracking().Where(t => t.STATUS == ConstantVariable.STATUS_APPROVED && t.DUEDATE.Value.Year == SearchDate.Value.Year).OrderBy(t => t.SEQUENCENUMBER).ToList();                    
                //if (tradeCode != null && !string.IsNullOrEmpty(tradeCode))
                //    InterestSlot = InterestSlot.Where(t => t.BOND.FINANCIALINSTITUTION_REFERENCE == tradeCode).ToList();

                //model = (from e in InterestSlot
                //         join bond in db.BONDs.ToList() on e.BOND_REFERENCE equals bond.REFERENCE
                //         select new BOND
                //         {
                //             REFERENCE = bond.REFERENCE,
                //             AUCTION = bond.AUCTION,
                //             FINANCIALINSTITUTION_REFERENCE = bond.FINANCIALINSTITUTION.NAME,
                //             BONDISSUEDATE = bond.BONDISSUEDATE,
                //             FACEVALUE = bond.FACEVALUE,
                //             MATURITYDATE = bond.MATURITYDATE,
                //             COUPONRATE = bond.COUPONRATE,
                //             TENURE = bond.TENURE,
                //             TENURETERM = bond.TENURETERM,

                //             CHEQUEAMOUNT = bond.CHEQUEAMOUNT, // Receivable from Jan to Month of Search Date =Replace(FormatCurrency(Fields!CHEQUEAMOUNT.Value,2),"$","")

                //             CREATEDDATE = InterestSlot.Where(t => t.BOND_REFERENCE == bond.REFERENCE && t.DUEDATE.Value.Year == SearchDate.Value.Year).OrderBy(t=>t.SEQUENCENUMBER).Take(1).FirstOrDefault().DUEDATE,  //first due date 
                //             LASTUPDATED = InterestSlot.Where(t => t.BOND_REFERENCE == bond.REFERENCE && t.DUEDATE.Value.Year == SearchDate.Value.Year).OrderBy(t=>t.SEQUENCENUMBER).Take(1).FirstOrDefault().DUEDATE.Value.AddMonths(6),
                //             //InterestSlot.Where(t => t.BOND_REFERENCE == bond.REFERENCE && t.DUEDATE.Value.Year == SearchDate.Value.Year).Skip(1).First().DUEDATE,  //second due date

                //             //Dues Day                  
                //             TERMSINDAYS = 0, //by deafult zero ,set value in report file
                //             ANNUALDAYS = bond.ANNUALDAYS,

                //             GROSSINTEREST = e.GROSSINTEREST,
                //             SOURCETAX = e.SOURCETAX,
                //             NETINTEREST = e.NETINTEREST,

                //             CHEQUECLEARINGCHARGE =0, //bond.CHEQUECLEARINGCHARGE Receivable Up To previous Year slot

                //             EXCISEDUTY =  0, // e.GROSSINTEREST - bond.CHEQUECLEARINGCHARGE, //next gross set in report
                //             OTHERCHARGE = 0, //e.SOURCETAX - bond.CHEQUECLEARINGCHARGE, //next source set in report
                //             COMMISSION =  0, //e.NETINTEREST - bond.CHEQUECLEARINGCHARGE, // next NetInterest ,set in report
                //             REMARKS = e.BOND.REMARKS

                //         }).ToList().GroupBy(t => t.REFERENCE).Select(g => g.First()).ToList();
                #endregion

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
            if (interest == null)
            {
                return RedirectToAction("ListReport");
            }


            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/BondReport/InterestIncomeBGTB.rdlc");

            //string report_title = "FDR Main Statement";;

            ReportDataSource rd = new ReportDataSource();

            rd.Name = "InterestIncomeBGTB";
            rd.Value = oCommonFunction.ConvertToDataTable(interest.ToList()); ;

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new  ReportParameter("SearchDate",Convert.ToString(interestDate.ToString("dd-MMM-yyyy"))),
                             new ReportParameter("UpToDate",Convert.ToString(new DateTime(interestDate.AddYears(-1).Year,12,31).ToString("dd-MMM-yyyy"))),
                    
                                                         
                           };


            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);


            string ContentType = "application/vnd.ms-excel";
            string FileType;
            string ReportName;

            if (IsExcell == "true")
            {
                FileType = "Excel";
                report_name += ".{0}";
            }
            else
            {
                FileType = "PDF";
                report_name += ".pdf";

            }

            string reportType = FileType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>" + FileType + "</OutputFormat>" +
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


            if (IsExcell == "true")
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

        #endregion

        #region BLOCK_FDRMAIN_STATEMENT_29-5-17
        //public ActionResult FDRStatementReportByDate(DateTime? fromDateFS, DateTime? toDateFS, string FI_Ref, string HasHistory, int? TENURE, string TERMS)
        //{
        //    List<FDRMainViewModel> FDList = new List<FDRMainViewModel>();
        //    List<FIXEDDEPOSIT> models;
        //    try
        //    {
        //        if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
        //        {
        //            return RedirectToAction("LogOut", "Home");
        //        }

        //        Entities db = new Entities(Session["Connection"] as EntityConnection);


        //        models = new List<FIXEDDEPOSIT>();

        //        if (toDateFS == null)
        //        {
        //            return RedirectToAction("Index", "ErrorPage", new { message = "From date or To date is Required!!" });
        //        }
        //        // FDR Statement History wanted.Which FDR was Approved withing begining and toDateFs.May some one now Encashed/Renewed but within search date they were Approved.So we chcek  LASTUPDATED
        //        else if (HasHistory == "true")
        //        {
        //            //ACCEPTEDDATE is Approved date 
        //            models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION")
        //                    .Where(fdr => fdr.STATUS == ConstantVariable.STATUS_ENCASHED || fdr.STATUS == ConstantVariable.STATUS_RENEWED || fdr.STATUS == ConstantVariable.STATUS_APPROVED)
        //                    .Where(fdr => fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_RENEWAL ? (toDateFS >= fdr.ACCEPTEDDATE && toDateFS < fdr.RENWALDATE) : fdr.PROPOSEDACTION == ConstantVariable.STATUS_NOTETYPE_ENCASH ? (toDateFS >= fdr.ACCEPTEDDATE && toDateFS < fdr.ENCASHMENTDATE) : toDateFS >= fdr.ACCEPTEDDATE)                                                 //fdr.OPENINGDATE <= toDateFS                                                        
        //                    .OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();
        //        }
        //        else if (HasHistory == null)
        //        {
        //            models = db.FIXEDDEPOSITs.Include("FINANCIALINSTITUTION").Where(fdr => fdr.OPENINGDATE <= toDateFS && fdr.STATUS == ConstantVariable.STATUS_APPROVED).OrderBy(t => t.FINANCIALINSTITUTION.NAME).ThenByDescending(t => t.OPENINGDATE).ToList();
        //        }

        //        //Now filter FinancialInstitution if has

        //        if (!string.IsNullOrEmpty(FI_Ref) && FI_Ref != null)
        //        {
        //            models = models.Where(t => t.FINANCIALINSTITUTION_REFERENCE == FI_Ref).ToList();
        //        }

        //        if (!string.IsNullOrEmpty(TERMS) && TERMS != null && TENURE != null && TENURE > 0)
        //        {
        //            models = models.Where(t => t.TENURE == TENURE && t.TENURETERM == TERMS).ToList();
        //        }

        //        ReportDataSource oFDRStatement = new ReportDataSource();

        //        if (models != null)
        //        {
        //            try
        //            {
        //                //now create a FixedDeposit List list 
        //                string FinancialInstitution_Ref = "";

        //                decimal TotalPresentPrincipal = 0;
        //                decimal TotalPrincipal = 0;
        //                decimal ReceivableTill = 0;
        //                decimal AVGRateOfInterest = 0;
        //                decimal ReceivableUp = 0;
        //                int NumberCount = 0;
        //                int initialrow = 0;
        //                decimal HoldingPeriod = 0;
        //                int slNo = 0;
        //                foreach (var item in models)
        //                {

        //                    //assign FI_Ref if not aexists
        //                    if (FinancialInstitution_Ref != item.FINANCIALINSTITUTION_REFERENCE)
        //                    {
        //                        FinancialInstitution_Ref = item.FINANCIALINSTITUTION_REFERENCE;
        //                        NumberCount = 0;
        //                        initialrow = 0;
        //                        //Count number of a sepecific bank
        //                        NumberCount = (from e in models where e.FINANCIALINSTITUTION_REFERENCE == FinancialInstitution_Ref select e).ToList().Count();

        //                        TotalPresentPrincipal = 0;
        //                        TotalPrincipal = 0;
        //                        ReceivableTill = 0;
        //                        AVGRateOfInterest = 0;

        //                    }

        //                    if (FinancialInstitution_Ref == item.FINANCIALINSTITUTION_REFERENCE)
        //                    {
        //                        initialrow++;

        //                        slNo++;
        //                        //Holding Period
        //                        HoldingPeriod = Convert.ToDecimal((toDateFS.Value.Date - item.OPENINGDATE.Value.Date).TotalDays);

        //                        //Calculate receivable Till hear it calculate up on Gross not NetInterest Receivable according DLIC FDR department
        //                        ReceivableTill = 0; //first assign 0 then claculate 
        //                        if (item.GROSSINTEREST != null)
        //                        {
        //                            decimal TotalDays = Convert.ToDecimal((item.MATURITYDATE.Value - item.OPENINGDATE.Value).TotalDays); //if renew then renewal Date                                           
        //                            ReceivableTill = (item.GROSSINTEREST.Value / TotalDays) * HoldingPeriod;

        //                            //Receibale For the period Up to period
        //                            //Up To period =Gross Interest of (maturityDate-OpeningDate) Days=GrossInterest
        //                            //For the period= get openingDate and maturityDate years if years same then GrossInterest
        //                            //if not then create a date 01-Jan-MaturityYear as lastTime
        //                            //then calculate total days Todays- lastTime as lastSlotDays
        //                            //ReceivableUpPeriod= (item.GROSSINTEREST.Value / TotalDays)* lastSlotDays;     

        //                            int RenewedIssuedYear = item.OPENINGDATE.Value.Year;
        //                            int SearchedDateYear = toDateFS.Value.Year;
        //                            //item.MATURITYDATE.Value.Year;

        //                            if (RenewedIssuedYear == SearchedDateYear)
        //                                ReceivableUp = ReceivableTill;
        //                            //item.GROSSINTEREST.Value;
        //                            else if (RenewedIssuedYear < SearchedDateYear)
        //                            {
        //                                DateTime SearcedDateYearFirstDate = new DateTime(SearchedDateYear, 01, 01);
        //                                decimal lastYearSlotDays = Convert.ToDecimal((toDateFS.Value.Date - SearcedDateYearFirstDate).TotalDays);
        //                                //Convert.ToDecimal((item.MATURITYDATE.Value - MaturityYearFirstDate).TotalDays); //Total days of last year slot                                           

        //                                ReceivableUp = (item.GROSSINTEREST.Value / TotalDays) * lastYearSlotDays;
        //                            }
        //                            else
        //                                ReceivableUp = 0;


        //                        }



        //                        TotalPrincipal += item.RENEWALDEPOSITNUMBER == null ? item.PRINCIPALAMOUNT.Value : item.INITIALPRINCIPALAMOUNT; //same as parent deposit if renewed
        //                        //item.PRINCIPALAMOUNT ==null? 0 : item.PRINCIPALAMOUNT.Value;

        //                        TotalPresentPrincipal += item.STATUS == ConstantVariable.STATUS_APPROVED ? item.PRINCIPALAMOUNT.Value : item.PRESENTPRINCIPALAMOUNT.Value;
        //                        //item.PRESENTPRINCIPALAMOUNT ==null ? 0 :item.PRESENTPRINCIPALAMOUNT.Value;
        //                        //item.PRINCIPALAMOUNT == null ? 0 : item.PRINCIPALAMOUNT.Value;

        //                        AVGRateOfInterest += item.RATEOFINTEREST == null ? 0 : item.RATEOFINTEREST.Value;

        //                        //add data into list
        //                        FDList.Add(new FDRMainViewModel
        //                        {

        //                            SlNo = slNo.ToString(),
        //                            FDRNo = item.DEPOSITNUMBER,
        //                            BankName = item.FINANCIALINSTITUTION.NAME + "\n" + item.FIBRANCH.NAME,

        //                            //if RENEWALDEPOSITNUMBER=null then this FDR does not renewed so its openingdate and principal amount remain same
        //                            //else this FDR is renewed so its opening Date and Principal Amount will be parent FDR opening Date and Principal Amount
        //                            //that is store in INITIALOPENINGDATE and INITIALPRINCIPALAMOUNT

        //                            OpeningDate = item.RENEWALDEPOSITNUMBER == null ? item.OPENINGDATE.Value.ToString("dd-MMM-yy") : item.INITIALOPENINGDATE.Value.ToString("dd-MMM-yy"),  //same of parent deposit if renewed                                    
        //                            PrincipalAmount = item.RENEWALDEPOSITNUMBER == null ? item.PRINCIPALAMOUNT : item.INITIALPRINCIPALAMOUNT, //same as parent deposit if renewed

        //                            BankWisePA = null,
        //                            RenewedDate = item.OPENINGDATE.Value.ToString("dd-MMM-yy"),
        //                            //item.RENWALDATE == null ? item.OPENINGDATE.Value.ToString("dd-MMM-yy") : item.RENWALDATE.Value.ToString("dd-MMM-yy"),
        //                            Period = item.TENURE + " " + item.TENURETERM,

        //                            MaturityDate = item.MATURITYDATE == null ? "" : item.MATURITYDATE.Value.ToString("dd-MMM-yy"),

        //                            HoldingPeriod = HoldingPeriod,
        //                            PresentPrincipalAmount = item.STATUS == ConstantVariable.STATUS_APPROVED ? item.PRINCIPALAMOUNT : item.PRESENTPRINCIPALAMOUNT,
        //                            BankWisePPA = null,
        //                            ExistingCAPLimit = null,
        //                            PresentRateOfInterest = item.RATEOFINTEREST,
        //                            ReceivableTill = ReceivableTill,
        //                            ReceivableUp = ReceivableUp,
        //                            Remarks = item.REMARKS
        //                            //item.INTERESTMODE==ConstantVariable.INTERESTMODE_COMPOUND? item.COMPOUNDINTERESTINTERVAL+"\n Com.": null,
        //                        });


        //                        if (initialrow == NumberCount)
        //                        {
        //                            //add extra row into the list                                    
        //                            FDList.Add(new FDRMainViewModel
        //                            {
        //                                SlNo = null,
        //                                FDRNo = null,
        //                                BankName = null,
        //                                OpeningDate = "",  //same of parent deposit if renewed
        //                                PrincipalAmount = null, //same as parent deposit if renewed
        //                                BankWisePA = TotalPrincipal,
        //                                RenewedDate = "",
        //                                Period = null,
        //                                MaturityDate = "",
        //                                HoldingPeriod = null,
        //                                PresentPrincipalAmount = null,
        //                                BankWisePPA = TotalPresentPrincipal,
        //                                ExistingCAPLimit = item.FINANCIALINSTITUTION.CAPLIMIT,
        //                                PresentRateOfInterest = Convert.ToDecimal(AVGRateOfInterest / NumberCount),
        //                                ReceivableTill = null,
        //                                ReceivableUp = null,
        //                                Remarks = null
        //                            });
        //                        }
        //                    }
        //                }

        //                LocalReport lr = new LocalReport();
        //                lr.ReportPath = Server.MapPath("~/Reports/FDRMainStatement.rdlc");

        //                string report_name = "FDRMainStatement.pdf";

        //                ReportDataSource rd = new ReportDataSource();

        //                DataTable dtFDRStatement = oCommonFunction.ConvertToDataTable(FDList.ToList());
        //                rd.Name = "FDRMainStatement";
        //                rd.Value = dtFDRStatement;

        //                ReportParameter[] parameters = new ReportParameter[] 
        //                   {
        //                     new ReportParameter("SearchDate",toDateFS.Value.ToString("dd-MMM-yy"))                                
        //                   };


        //                lr.ReportPath = Server.MapPath("~/Reports/FDRMainStatement.rdlc");
        //                lr.SetParameters(parameters);
        //                lr.DataSources.Add(rd);


        //                string reportType = "PDF";
        //                string mimeType;
        //                string encoding;
        //                string fileNameExtension;

        //                string deviceInfo =

        //                "<DeviceInfo>" +
        //                    "  <OutputFormat>PDF</OutputFormat>" +
        //                    "  <PageWidth>12.5in</PageWidth>" +
        //                    "  <PageHeight>11in</PageHeight>" +
        //                    "  <MarginTop>0.5in</MarginTop>" +
        //                    "  <MarginLeft>0.75in</MarginLeft>" +
        //                    "  <MarginRight>0.25in</MarginRight>" +
        //                    "  <MarginBottom>0.5in</MarginBottom>" +
        //                    "</DeviceInfo>";

        //                Warning[] warnings;
        //                string[] streams;
        //                byte[] renderedBytes;

        //                renderedBytes = lr.Render(
        //                    reportType,
        //                    deviceInfo,
        //                    out mimeType,
        //                    out encoding,
        //                    out fileNameExtension,
        //                    out streams,
        //                    out warnings);

        //                renderedBytes = lr.Render(reportType);

        //                return File(renderedBytes, mimeType, report_name);
        //            }
        //            catch (Exception ex)
        //            {
        //                return RedirectToAction("Index", "ErrorPage", new { message = Convert.ToString(ex.Message) });
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "ErrorPage", new { message = "Sorry, we can't find any record you searched!!" });

        //        }

        //        //return RedirectToAction("GenerateFDRRegisterReport", "FixedDepositRegister");
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.Message + " Inner Error: " + ex.InnerException.Message; ;

        //        return RedirectToAction("Index", "ErrorPage", new { message });
        //    }

        //}

        #endregion

        public ActionResult BondLetter(string reference)
        {
            
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                var db = new Entities(Session["Connection"] as EntityConnection);
                List<BOND> model = new List<BOND>();
                string report_name = "BondLetter.pdf";
                
                LocalReport lr = new LocalReport();
                lr.ReportPath = Server.MapPath("~/Reports/BondReport/HalfYearInterestLetter.rdlc");

                //string report_title = "FDR Main Statement";;

                ReportDataSource rd = new ReportDataSource();
                decimal principal = Convert.ToDecimal(999500426.28);
                //rd.Name = "halfYearlyCouponInterest";
                // rd.Value = oCommonFunction.ConvertToDataTable(model.ToList()); ;
                ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new ReportParameter("Auction","11th Auctions"),
                             new ReportParameter("TenureTerm","10 Years"),
                             new ReportParameter("BoId","1234567890123456"),                                
                             new ReportParameter("principal",principal.ToString()),
                             new ReportParameter("OpeningDate",DateTime.Now.ToString("dd-MMM-yy")),
                             new ReportParameter("MaturedDate",DateTime.Now.ToString("dd-MMM-yy"))
                           };
                lr.SetParameters(parameters);
                //lr.DataSources.Add(rd);

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

        public ActionResult ParticularReportStatement(DateTime? SearchDate, string IsExcell)
        {

            //if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            //{
            //    return RedirectToAction("LogOut", "Home");
            //}

            var db = new Entities(Session["Connection"] as EntityConnection);
            List<ParticularViewModel> model = new List<ParticularViewModel>();

            string report_name = "Statement Of Total Investment-" + SearchDate.Value.ToString("dd-MMM-yy");

            DateTime interestDate;                      
            interestDate = new DateTime(SearchDate.Value.Year, 12, 31); //31th-Dec of given year          
            string PrvToDate = Convert.ToString(new DateTime(interestDate.AddYears(-1).Year, 12, 31).ToString("dd-MMM-yy"));      

            List<InterestIncomeBGTBViewModel> interest = new List<InterestIncomeBGTBViewModel>();
            try
            {

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(con))
                {

                    string query = "select * from table(GET_PARTICULARSLIST_FN('"+PrvToDate +"','"+SearchDate.Value.ToString("dd-MMM-yy")+"'))"; //order by shortname where currentbalance !=0                                      

                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dtPortfolioInstrument = new DataTable();
                    da.Fill(dtPortfolioInstrument);

                    model = (from DataRow row in dtPortfolioInstrument.Rows
                              select new ParticularViewModel
                              {
                                  GroupName = row["GroupName"].ToString(),
                                  SubGroupName = row["SubGroupName"].ToString(),
                                  Code = row["Code"].ToString(),
                                  ParticularsName = row["ParticularsName"].ToString(),
                                  PrincipalPrvYear = Convert.ToDecimal(row["PrincipalPrvYear"]),
                                  PrincipalCurrent= Convert.ToDecimal(row["PrincipalCurrent"]),     
                                  LimitMinimum = Convert.ToDecimal(row["LimitMinimum"]),
                                  Remarks = row["Remarks"].ToString(),

                              }).ToList();


                }

              
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
            if (model == null)
            {
                return RedirectToAction("ListReport");
            }


            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/Particulars/ParticularStatement.rdlc");

            //string report_title = "FDR Main Statement";;

            ReportDataSource rd = new ReportDataSource();

            rd.Name = "Particulars";
            rd.Value = oCommonFunction.ConvertToDataTable(model.ToList()); ;

            ReportParameter[] parameters = new ReportParameter[] 
                           {
                             new  ReportParameter("SearchDate",Convert.ToString(SearchDate.Value.ToString("dd-MMM-yy"))),
                             new  ReportParameter("PrvYearDate",PrvToDate),
                                                                                    
                           };


            lr.SetParameters(parameters);
            lr.DataSources.Add(rd);


            string ContentType = "application/vnd.ms-excel";
            string FileType;
           

            if (IsExcell == "true")
            {
                FileType = "Excel";
                report_name += ".{0}";
            }
            else
            {
                FileType = "PDF";
                report_name += ".pdf";

            }

            string reportType = FileType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>" + FileType + "</OutputFormat>" +
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


            if (IsExcell == "true")
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


        #region helper_to_call_StoredProcedure


        

      
        private DataTable GET_STOCK_CASH_RECEIVABLE(string AccountNumber, string FromDate,string ToDate)
        {           

            string con = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(con))
            {

                string query = "select * from table(Get_StockCashReceivable_FN('" + AccountNumber + "','" + FromDate + "','" + ToDate + "')) ORDER BY CATYPE,RECORDDATE,SHORTNAME";              

                OracleCommand cmd = new OracleCommand(query, conn);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;

            }
           
        }

        private DataTable GET_RIGHT_SHARE_RECEIVED(string FromDate,string ToDate)
        {
            string con = System.Configuration.ConfigurationManager.
             ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(con))
            {
                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GET_RIGHT_SHARE_RECEIVED";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("FromDate", OracleType.DateTime).Value = DateTime.Parse(FromDate);
                cmd.Parameters.Add("ToDate", OracleType.DateTime).Value = DateTime.Parse(ToDate);

                cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;

                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }


        //Stock Dividend Received
        private DataTable GET_BONUS_SHARE_RECEIVED(string FromDate, string ToDate)
        {
            string con = System.Configuration.ConfigurationManager.
             ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(con))
            {
                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GET_BONUS_SHARE_RECEIVED";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("FromDate", OracleType.DateTime).Value = DateTime.Parse(FromDate);
                cmd.Parameters.Add("Todate", OracleType.DateTime).Value = DateTime.Parse(ToDate);

                cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        //GetStockLedgerStatement

        private DataTable GET_STOCK_LEDGER(string FromDate, string ToDate, string AccountNumber)
        {
           
            string con = System.Configuration.ConfigurationManager.
            ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(con))
            {
                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "Get_InvestorLedgerStatement";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("FromDate", OracleType.DateTime).Value = DateTime.Parse(FromDate);
                cmd.Parameters.Add("ToDate", OracleType.DateTime).Value = DateTime.Parse(ToDate);
                cmd.Parameters.Add("AccountNumber", OracleType.VarChar).Value = AccountNumber;

                cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        private DataTable GET_CLIENT_OPENING_BALANCE(string FromDate, string AccountNumber)
        {
            string con = System.Configuration.ConfigurationManager.
             ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(con))
            {
                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GET_ClientAccountOpenBalance";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("fromDate", OracleType.DateTime).Value = DateTime.Parse(FromDate);              
                cmd.Parameters.Add("AccountNumber", OracleType.VarChar).Value = AccountNumber;

                cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        private DataTable GET_EXTRADIVIDEND_RECEIVED(string FromDate, string ToDate)
        {
            string con = System.Configuration.ConfigurationManager.
             ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(con))
            {
                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GET_EXTRADIVIDEND_RECEIVED";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("FromDate", OracleType.DateTime).Value = DateTime.Parse(FromDate);
                cmd.Parameters.Add("Todate", OracleType.DateTime).Value = DateTime.Parse(ToDate);

                cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        #endregion

    }
}
