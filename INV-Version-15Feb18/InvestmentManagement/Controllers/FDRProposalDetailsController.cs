using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.ViewModel;
using System.Data;
using System.Data.EntityClient;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
//using Oracle.ManagedDataAccess.Client;

using InvestmentManagement.App_Code;

namespace InvestmentManagement.Controllers
{
    public class FDRProposalDetailsController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
       

        [HttpGet]
        public ActionResult ListFDRProposalDetails(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15, string reference = null)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<FDRPROPOSALDETAIL> gridModels = new GridModel<FDRPROPOSALDETAIL>();
                List<FDRPROPOSALDETAIL> models = null;
                string apReference = string.Empty;
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
                //int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALDETAILS.Include("FDRPROPOSAL").Include("FINANCIALINSTITUTION").AsNoTracking().OrderBy(sort + " " + sortdir).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALDETAILS.Include("FDRPROPOSAL").Include("FINANCIALINSTITUTION").AsNoTracking().Where(w => w.STATUS.Contains(filterstring)).OrderBy(sort + " " + sortdir).ToList();

    
          

                if (reference != null)
                {
                    ViewBag.Refference = reference;
                    models = models.Where(m => m.FDRPROPOSAL_REFERENCE == reference).ToList();

                }
                

                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                    Session["Path"] = oCommonFunction.GetDetailsPath(Session["Path"] as IHtmlString, this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), reference);
                }

                ViewBag.BreadCum = oCommonFunction.GetDetailsListPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                               

                return PartialView("ListFDRProposalDetails", gridModels);

            }

            catch (Exception ex)
            {
               
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }



        [HttpGet]
        public ActionResult AddFDRProposalDetails(string reference)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.Refference = reference;
                ViewBag.breadcum = oCommonFunction.GetDetailsAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];


                ViewBag.FDRProposalList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALs, "REFERENCE", "NAME");
                var sPDetailRef = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(w => w.ENTITY.ToLower() == "Fixed Deposit".ToLower() && w.PROPERTY.ToLower() == "Status".ToLower()).FirstOrDefault().REFERENCE.ToString();
                var statusList = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(w => w.STATUSPARAMETER_REFERENCE == sPDetailRef).ToList();
                ViewBag.StatusList = new SelectList(statusList, "DESCRIPTION", "DESCRIPTION");

                var tPDetailRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(w => w.ENTITY.ToLower() == "Fixed Deposit".ToLower() && w.PROPERTY.ToLower() == "TenureTerm".ToLower()).FirstOrDefault().REFERENCE.ToString();
                var termList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(w => w.APPLICATIONPARAMETER_REFERENCE == tPDetailRef).ToList();
                ViewBag.termList = new SelectList(termList, "DESCRIPTION", "DESCRIPTION");

                ViewBag.FINANCIALINSTITUTIONList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(p => p.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.FIBranch = new SelectList(new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");

                //added by Rakibul Date<4th Feb 2016>
                var interestType = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var COMPOUNDINTERESTTYPE = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestType).OrderBy(app => app.DESCRIPTION).ToList();
                ViewBag.COMPOUNDINTERESTTYPEList = new SelectList(COMPOUNDINTERESTTYPE, "DESCRIPTION", "DESCRIPTION");




                FDRPROPOSALDETAIL proposalDetails = new FDRPROPOSALDETAIL() { FDRPROPOSAL_REFERENCE = reference };


               



                return PartialView(proposalDetails);
            }

            catch (Exception ex)
            {
              
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

      
        /// <summary>
        /// while saving a Proposal Details compare the Offer rate,save the biggest Offer rate in OFFERRATE filed and coresponding Tenure aaaand Terms in Tenure and Terms field
        /// </summary>
        /// <param name="oFDRPROPOSALDETAIL"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFDRProposalDetails(FDRPROPOSALDETAIL oFDRPROPOSALDETAIL, string reference)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                    {
                        return RedirectToAction("LogOut", "Home");
                    }

                    if (oFDRPROPOSALDETAIL.OFFERRATE1 == null) { }
                    if (oFDRPROPOSALDETAIL.TENURE1 == null) { }
                    if (oFDRPROPOSALDETAIL.TERM1 == null) { }
                    if (oFDRPROPOSALDETAIL.INTERESTMODE1 == null) { }                     
                 

                    if (Request.IsAjaxRequest())
                    {
                        using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                        {
                            bool isUpdate = false;

                            if (!string.IsNullOrEmpty(oFDRPROPOSALDETAIL.REFERENCE) && db.FDRPROPOSALDETAILS.Any(details => details.REFERENCE == oFDRPROPOSALDETAIL.REFERENCE))
                                isUpdate = true;
                            
                            //Assign primary key
                            if (!isUpdate)
                            {
                                oFDRPROPOSALDETAIL.REFERENCE = Guid.NewGuid().ToString();
                                oFDRPROPOSALDETAIL.FDRPROPOSAL_REFERENCE = reference;

                            }

                            oFDRPROPOSALDETAIL.CREATEDBY = Session["UserId"].ToString();
                            oFDRPROPOSALDETAIL.CREATEDDATE = DateTime.Now;
                            oFDRPROPOSALDETAIL.LASTUPDATED = DateTime.Now;
                            oFDRPROPOSALDETAIL.LASTUPDATEDBY = Session["UserId"].ToString();
                            oFDRPROPOSALDETAIL.CURRENTHOLDING = oFDRPROPOSALDETAIL.CURRENTHOLDING;
                            oFDRPROPOSALDETAIL.PRINCIPALAMOUNT = oFDRPROPOSALDETAIL.PRINCIPALAMOUNT;
                            oFDRPROPOSALDETAIL.EXISTINGCAPLIMIT = oFDRPROPOSALDETAIL.EXISTINGCAPLIMIT;
                            oFDRPROPOSALDETAIL.PERCENTAGEOFTOTALFDR = oFDRPROPOSALDETAIL.PERCENTAGEOFTOTALFDR;
                            oFDRPROPOSALDETAIL.NPL = oFDRPROPOSALDETAIL.NPL;
                            oFDRPROPOSALDETAIL.TENURE = oFDRPROPOSALDETAIL.TENURE;
                            oFDRPROPOSALDETAIL.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";
                            oFDRPROPOSALDETAIL.OFFERRATE = oFDRPROPOSALDETAIL.OFFERRATE;
                            
                            oCommonFunction.CustomObjectNullValidation<FDRPROPOSALDETAIL>(ref oFDRPROPOSALDETAIL);


                            if (!isUpdate)
                                db.FDRPROPOSALDETAILS.Add(oFDRPROPOSALDETAIL);
                            else
                                db.Entry(oFDRPROPOSALDETAIL).State = EntityState.Modified;

                            db.SaveChanges();
                        }


                    }

                    ViewBag.Refference = oFDRPROPOSALDETAIL.FDRPROPOSAL_REFERENCE;

                    return RedirectToAction("ListFDRProposalDetails", "FDRProposalDetails", new { reference = oFDRPROPOSALDETAIL.FDRPROPOSAL_REFERENCE });


                }
                catch (Exception ex)
                {
                 //   throw ex;
                    string message = ex.Message;
                    return RedirectToAction("Index", "ErrorPage", new { message });

                }
            }
            return View(oFDRPROPOSALDETAIL);
        }


        [HttpGet]
        public ActionResult EditFDRProposalDetails(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                FDRPROPOSALDETAIL oFDRPROPOSALDETAIL = new FDRPROPOSALDETAIL();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
               

                oFDRPROPOSALDETAIL = db.FDRPROPOSALDETAILS.Include("FDRPROPOSAL").SingleOrDefault(i => i.REFERENCE == id);
                             
                
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.Refference = id;
                ViewBag.breadcum = oCommonFunction.GetDetailsEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());   //.GetDetailsAddPath

                ViewBag.Header = "Edit " + Session["currentPage"];
                ViewBag.FDRProposalList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALs, "REFERENCE", "NAME");
                var sPDetailRef = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(w => w.ENTITY.ToLower() == "Fixed Deposit".ToLower() && w.PROPERTY.ToLower() == "Status".ToLower()).FirstOrDefault().REFERENCE.ToString();
                var statusList = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(w => w.STATUSPARAMETER_REFERENCE == sPDetailRef).ToList();
                ViewBag.StatusList = new SelectList(statusList, "DESCRIPTION", "DESCRIPTION");

                var tPDetailRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(w => w.ENTITY.ToLower() == "Fixed Deposit".ToLower() && w.PROPERTY.ToLower() == "TenureTerm".ToLower()).FirstOrDefault().REFERENCE.ToString();
                var termList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(w => w.APPLICATIONPARAMETER_REFERENCE == tPDetailRef).ToList();
                ViewBag.termList = new SelectList(termList, "DESCRIPTION", "DESCRIPTION");

                ViewBag.FINANCIALINSTITUTIONList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(p => p.NAME).ToList(), "REFERENCE", "NAME");
               
                
                ViewBag.FIBranch = new SelectList(new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.Where(f=>f.FINANCIALINSTITUTION_REFERENCE==oFDRPROPOSALDETAIL.FINANCIALINSTITUTION_REFERENCE).OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                

                ViewBag.INTERESTMODE =oFDRPROPOSALDETAIL.INTERESTMODE== null ? "Select a list" :oFDRPROPOSALDETAIL.INTERESTMODE;

                var interestType = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY == "Fixed Deposit" && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var COMPOUNDINTERESTTYPE = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestType).OrderBy(app => app.DESCRIPTION).ToList();
                ViewBag.COMPOUNDINTERESTTYPEList = new SelectList(COMPOUNDINTERESTTYPE, "DESCRIPTION", "DESCRIPTION");

                ViewBag.InterestType = oFDRPROPOSALDETAIL.INTERESTMODE;
                ViewBag.InterestType1= oFDRPROPOSALDETAIL.INTERESTMODE1;
                ViewBag.InterestType2 = oFDRPROPOSALDETAIL.INTERESTMODE2;
                ViewBag.InterestType3 = oFDRPROPOSALDETAIL.INTERESTMODE3;
                ViewBag.InterestType4 = oFDRPROPOSALDETAIL.INTERESTMODE4;


                //added by rakibul 1/11/2016
                return PartialView("EditFDRProposalDetails", oFDRPROPOSALDETAIL);
                
            }

            catch (Exception ex)
            {
                //throw ex;
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });


            }
        }

        [HttpPost]
        public ActionResult EditFDRProposalDetails(FDRPROPOSALDETAIL oFDRPROPOSALDETAIL)
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
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                decimal offerrate1 = oFDRPROPOSALDETAIL.OFFERRATE.Value;
                decimal offerrate2 = oFDRPROPOSALDETAIL.OFFERRATE2.Value;
                decimal offerrate3 = oFDRPROPOSALDETAIL.OFFERRATE3.Value;
                decimal GreaterOffer = System.Math.Max(System.Math.Max(oFDRPROPOSALDETAIL.OFFERRATE.Value, oFDRPROPOSALDETAIL.OFFERRATE2.Value), oFDRPROPOSALDETAIL.OFFERRATE3.Value);


                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oFDRPROPOSALDETAIL.LASTUPDATEDBY = Session["UserId"].ToString();
                    oFDRPROPOSALDETAIL.LASTUPDATED = DateTime.Now;
                    oFDRPROPOSALDETAIL.CURRENTHOLDING = oFDRPROPOSALDETAIL.CURRENTHOLDING;
                    oFDRPROPOSALDETAIL.PRINCIPALAMOUNT = oFDRPROPOSALDETAIL.PRINCIPALAMOUNT;
                    oFDRPROPOSALDETAIL.EXISTINGCAPLIMIT = oFDRPROPOSALDETAIL.EXISTINGCAPLIMIT;
                    oFDRPROPOSALDETAIL.PERCENTAGEOFTOTALFDR = oFDRPROPOSALDETAIL.PERCENTAGEOFTOTALFDR;
                    oFDRPROPOSALDETAIL.NPL = oFDRPROPOSALDETAIL.NPL;
                    oFDRPROPOSALDETAIL.TENURE = oFDRPROPOSALDETAIL.TENURE;
                    oFDRPROPOSALDETAIL.OFFERRATE = oFDRPROPOSALDETAIL.OFFERRATE;
                    oCommonFunction.CustomObjectNullValidation<FDRPROPOSALDETAIL>(ref oFDRPROPOSALDETAIL);
                    db.Entry(oFDRPROPOSALDETAIL).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListFDRProposalDetails", "FDRProposalDetails", new { reference = oFDRPROPOSALDETAIL.FDRPROPOSAL_REFERENCE });

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


       

        //public ActionResult GenerateFDRProposalDetailsReport(string reference = null)
        //{
        //    List<FDRPROPOSALDETAIL> models = new List<FDRPROPOSALDETAIL>();
        //    models = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALDETAILS.Include("FDRPROPOSAL").Include("FINANCIALINSTITUTION").ToList();
        //    models = models.Where(m => m.FDRPROPOSAL_REFERENCE == reference).ToList();


        //    LocalReport lr = new LocalReport();

        //    lr.ReportPath = Server.MapPath("~/Reports/FDRproposal.rdlc");

        //    ReportDataSource rd = new ReportDataSource();
        //    rd.Name = "FDRProposalDetailsDS";
        //    rd.Value=models;
        //    lr.DataSources.Add(rd);
        //    string reportType = "PDF";
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension;

        //    string deviceInfo =

        //    "<DeviceInfo>" +
        //        "  <OutputFormat>PDF</OutputFormat>" +
        //        "  <PageWidth>8.5in</PageWidth>" +
        //        "  <PageHeight>11in</PageHeight>" +
        //        "  <MarginTop>0.5in</MarginTop>" +
        //        "  <MarginLeft>1in</MarginLeft>" +
        //        "  <MarginRight>1in</MarginRight>" +
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




        //    return File(renderedBytes, mimeType,"FDRProposalDetails.Pdf");


        //}


        //public ActionResult GenerateFDRProposalDetailsReport(string reference = null)
        //{


        //    List<FDRPROPOSALDETAIL> models = new List<FDRPROPOSALDETAIL>();
        //    FDRPROPOSAL oFDRPROPOSAL=new FDRPROPOSAL();
        //    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
        //    {

        //        models = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALDETAILS.ToList();
        //        models = models.Where(m => m.FDRPROPOSAL_REFERENCE == reference).ToList();
        //        oFDRPROPOSAL=db.FDRPROPOSALs.Where(f => f.REFERENCE == reference).FirstOrDefault();

        //    }
        //    LocalReport lr = new LocalReport();
        //    lr.ReportPath = Server.MapPath("~/Reports/FDRproposal.rdlc");

        //    ReportDataSource rd = new ReportDataSource();
        //    //ReportDataSource dd = new ReportDataSource();

        //    DataTable FDRPROPOSALDETAILDT = oCommonFunction.ConvertToDataTable(models);
        //    rd.Name = "FDRProposalDetailsDS";
        //    rd.Value = FDRPROPOSALDETAILDT;


        //    ReportParameter[] fDRPropsal = new ReportParameter[] 
        //    {
        //     new ReportParameter("FDRProposalName",oFDRPROPOSAL.NAME),
        //     new ReportParameter("FDRProposalId",oFDRPROPOSAL.PROPOSALID)

        //    };



        //    lr.SetParameters(fDRPropsal);
        //    lr.DataSources.Add(rd);

        //    string reportType = "PDF";
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension;

        //    string deviceInfo =

        //    "<DeviceInfo>" +
        //        "  <OutputFormat>PDF</OutputFormat>" +
        //        "  <PageWidth>8.5in</PageWidth>" +
        //        "  <PageHeight>11in</PageHeight>" +
        //        "  <MarginTop>0.5in</MarginTop>" +
        //        "  <MarginLeft>1in</MarginLeft>" +
        //        "  <MarginRight>1in</MarginRight>" +
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




        //    return File(renderedBytes, mimeType, "FDRProposal.Pdf");



        //}

        public ActionResult GenerateFDRProposalDetailsReport(string reference = null)
        {

            //List<FDRPROPOSALDETAIL> models = new List<FDRPROPOSALDETAIL>();
            FDRPROPOSAL oFDRPROPOSAL = new FDRPROPOSAL();
            using (Entities db = new Entities(Session["Connection"] as EntityConnection))
            {

                //models = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALDETAILS.ToList();
                //models = models.Where(m => m.FDRPROPOSAL_REFERENCE == reference).ToList();
                oFDRPROPOSAL = db.FDRPROPOSALs.Where(f => f.REFERENCE == reference).FirstOrDefault();

            }

            //string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["Entities"].ConnectionString;
            //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(conStr);
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            DataTable FDRPROPOSALDETAILDT = new DataTable();
            using (OracleConnection conn = new OracleConnection(connStr))
            {

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GETFDRProposalDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("FDRProposalRef", OracleType.NVarChar).Value = reference;
                cmd.Parameters.Add("FDRProposalDetailsList", OracleType.Cursor).Direction = ParameterDirection.Output;

                da.SelectCommand = cmd;

                da.Fill(FDRPROPOSALDETAILDT);


            }

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/FDRproposal.rdlc");


            //ReportDataSource dd = new ReportDataSource();

            //DataTable FDRPROPOSALDETAILDT = oCommonFunction.ConvertToDataTable(models);
            //rd.Name = "FDRProposalDetailsDS";
            //rd.Value = FDRPROPOSALDETAILDT;
            ReportDataSource rd = new ReportDataSource();
            rd.Name = "IVMDataSet";
            rd.Value = FDRPROPOSALDETAILDT;

            ReportParameter[] fDRPropsal = new ReportParameter[] 
            {
             new ReportParameter("FDRProposalName",oFDRPROPOSAL.NAME),
             new ReportParameter("FDRProposalId",oFDRPROPOSAL.PROPOSALID)

            };



            lr.SetParameters(fDRPropsal);
            lr.DataSources.Add(rd);

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


            return File(renderedBytes, mimeType, "FDRProposalDetails.Pdf");



        }

        /// <summary>
        /// Current Holding: The total FDR Principle Amount that is not matured and not encashed
        /// </summary>
      
        public ActionResult GetFIDetails(string reference = null)
        {

            string json;  
            Decimal? totalPrincialAmount=0;
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
                        totalPrincialAmount = db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.STATUS ==ConstantVariable.STATUS_APPROVED).Sum(fixDeposit => fixDeposit.PRINCIPALAMOUNT);
                    }
                    catch { totalPrincialAmount = 0; }
                    try
                    {
                        // currentDeposit or current Holding or Existing Deposit without corer // "Approved" sum of all  Principal Amount of given FI/Bank of FixedDeposit table where Status= Approved
                        currentDeposit = db.FIXEDDEPOSITs.Where(fixDeposit => fixDeposit.STATUS ==ConstantVariable.STATUS_APPROVED && fixDeposit.FINANCIALINSTITUTION_REFERENCE == reference).Sum(fixDeposit => fixDeposit.PRINCIPALAMOUNT);
                    }
                    catch { currentDeposit = 0; }

                    if (currentDeposit == null)
                        currentDeposit = 0;
                    if (totalPrincialAmount == null)
                        totalPrincialAmount = 1;

                    // FIInformation oFIInformation = new FIInformation();
                    oFIInformation.CAPLimit = oFI.existingCapLimit ==null ? 0 : oFI.existingCapLimit;
                    oFIInformation.ExitsingDeposit = currentDeposit==null? 0 : currentDeposit;

                    oFIInformation.FDRPerentage = Math.Round((currentDeposit.Value / totalPrincialAmount.Value) * 100, 2);

                        //totalPrincialAmount==null ? 0 : Math.Round((totalPrincialAmount != 0 ? (currentDeposit / totalPrincialAmount) * 100 : 0).Value, 2);
                    oFIInformation.NPL = oFI.NPL;
                    //converting corer
                    oFIInformation.ExitsingDeposit = oFIInformation.ExitsingDeposit > 0 ? (oFIInformation.ExitsingDeposit / 10000000) : 0;

                    var BranchList = from e in db.FIBRANCHes.ToList()
                                     where e.FINANCIALINSTITUTION_REFERENCE == reference
                                     select new
                                     {
                                         REFERENCE=e.REFERENCE,
                                         NAME=e.NAME,
                                         
                                         CAPLimit=oFIInformation.CAPLimit,
                                         ExitsingDeposit=oFIInformation.ExitsingDeposit,
                                         FDRPerentage=oFIInformation.FDRPerentage,
                                         NPL=oFI.NPL
                                     };
                     return Json(BranchList, JsonRequestBehavior.AllowGet); 
                //    json = new JavaScriptSerializer().Serialize(BranchList);
                   }
                
            
                catch (Exception ex)
                {
                    string message = ex.Message;
                    return Json(null, JsonRequestBehavior.AllowGet); 
                  
                }
               
              // json = new JavaScriptSerializer().Serialize(oFIInformation);
               
            }

           // return Json(, JsonRequestBehavior.AllowGet); 

            
         
        }

        public ActionResult FDRProposalDetailsChangeStatus(string fdrRef=null,string status=null)
        {
            string fdrProRef=null;
            if (status !=null)
            {
                
                string fdrProposalStatus;
                if (status =="Accepted")
                {
                    fdrProposalStatus = "Accepted";
                }
                else
                {
                    fdrProposalStatus = "Rejected";
                }
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    var fdrProp = db.FDRPROPOSALDETAILS.SingleOrDefault(f => f.REFERENCE == fdrRef);
                    fdrProRef = fdrProp.FDRPROPOSAL_REFERENCE.ToString();
                    fdrProp.STATUS = fdrProposalStatus;
                    db.Entry(fdrProp).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }


            return RedirectToAction("ListFDRProposalDetails", "FDRProposalDetails", new { reference = fdrProRef });
        }


        /// <summary>
        /// Added by Rakibul <12/01/2016>
        /// </summary>       
        #region Helper Method

        public decimal GetGrettestOfferRate(decimal oferrate1,decimal offerrate2,decimal offerrate3)
        {
            try
            {
                return System.Math.Max(System.Math.Max(oferrate1, offerrate2), offerrate3);
            }
            catch { 
            
            }
            return -1;
        }
        #endregion

    }
}
