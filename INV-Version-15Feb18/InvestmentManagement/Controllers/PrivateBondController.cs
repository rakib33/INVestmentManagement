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

namespace InvestmentManagement.Controllers
{
    public class PrivateBondController : Controller
    {
        //
        // GET: /PrivateBond/
        CommonFunction oCommonFunction = new CommonFunction();
        [HttpGet]
        public ActionResult ListPrivateBond(string sortdir, string sort, string sortdefault, int? rows, string filterstring, string lblbreadcum, string PagingType, DateTime? openingDate, string FI_REFERENCE, string STATUS)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<PRIVATEBOND> gridModels = new GridModel<PRIVATEBOND>();
                List<PRIVATEBOND> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ? 15 : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;


                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
               
                ViewBag.CurrentFilter = filterstring;

                models = new Entities(Session["Connection"] as EntityConnection).PRIVATEBONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t=>t.STATUS == ConstantVariable.STATUS_PENDING).OrderBy(sort + " " + sortdir).ToList();


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


                if (Convert.ToString(TempData["Approved"]) == "Approved")
                {                   
          
                   ViewBag.Message = Convert.ToString(TempData["result"]);
                }
                
                return PartialView("ListPrivateBond", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddPrivateBond()
        {

            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY.ToLower() == "TenureTerm".ToLower()).FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();
                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
              
                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION", "Years");

                var interestModeRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var interestModeList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestModeRef).ToList();

                var compoundInterestIntervalRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
                var compoundInterestIntervalList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == compoundInterestIntervalRef).ToList();


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


                return PartialView("AddPrivateBond");
            }
            catch(Exception ex)
            {
                //throw ex;
                TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
            }

            return RedirectToAction("ListPrivateBond", "PrivateBond");
           
        }

        [HttpPost]
        public ActionResult AddPRIVATEBOND(PRIVATEBOND oPBond)  // BOND oBOND
        {
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (Request.IsAjaxRequest())
                {
                    TempData["Approved"] = "Approved";

                    oPBond.REFERENCE = Guid.NewGuid().ToString();
                    oPBond.CREATEDBY = Session["UserId"].ToString();
                    oPBond.CREATEDDATE = DateTime.Now;
                    oPBond.STATUS =ConstantVariable.STATUS_PENDING;
                 

                    if (oPBond.TENURETERM == ConstantVariable.TENURETERM_YEARS)
                        oPBond.MATURITYDATE =oPBond.BONDISSUEDATE.Value.AddYears(Convert.ToInt32(oPBond.TENURE.Value));

                    else if (oPBond.TENURETERM == ConstantVariable.TENURETERM_MONTHS)  // "Months"
                        oPBond.MATURITYDATE = oPBond.BONDISSUEDATE.Value.AddMonths(Convert.ToInt32(oPBond.TENURE.Value));

                    else if (oPBond.TENURETERM == ConstantVariable.TENURETERM_DAYS)
                       oPBond.MATURITYDATE = oPBond.BONDISSUEDATE.Value.AddDays(Convert.ToInt32(oPBond.TENURE.Value));       

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.PRIVATEBONDs.Add(oPBond);                        
                        db.SaveChanges();
                    }

                    TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">Save Successful !</div>");      
                }
             

            }
           catch(Exception ex)
            {
                //throw ex;
                TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
            }

            return RedirectToAction("ListPrivateBond", "PrivateBond");
        }

        [HttpGet]
        public ActionResult AcceptPBond(string id)
        {
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Accept " + Session["currentPage"];

            

                var oBOND = new Entities(Session["Connection"] as EntityConnection).PRIVATEBONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.REFERENCE == id).SingleOrDefault();

                
                
                var tenurelist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY.ToLower() == "TenureTerm".ToLower()).FirstOrDefault().REFERENCE.ToString();
                var tenureTermlist = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == tenurelist).ToList();

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME",oBOND.FINANCIALINSTITUTION_REFERENCE);

                ViewBag.TenureList = new SelectList(tenureTermlist, "DESCRIPTION", "DESCRIPTION",oBOND.TENURETERM);

                var interestModeRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY == "INTERESTMODE").FirstOrDefault().REFERENCE.ToString();
                var interestModeList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == interestModeRef).ToList();

                var compoundInterestIntervalRef = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(app => app.ENTITY.ToLower() == "Fixed Deposit".ToLower() && app.PROPERTY == "CompoundInterestInterval").FirstOrDefault().REFERENCE.ToString();
                var compoundInterestIntervalList = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(app => app.APPLICATIONPARAMETER_REFERENCE == compoundInterestIntervalRef).ToList();

              
                string DefaultId = "6";
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

                return PartialView("AcceptPBond", oBOND);
             
            }
            catch (Exception ex)
            {
                //throw ex;
                TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
            }

            return RedirectToAction("ListPrivateBond", "PrivateBond");
           
        }

        [HttpGet]
        public ActionResult RejectPBond(string id)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    PRIVATEBOND pBond = new Entities(Session["Connection"] as EntityConnection).PRIVATEBONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.REFERENCE == id).SingleOrDefault();
                    pBond.STATUS = ConstantVariable.STATUS_REJECTED;

                    pBond.REJECTEDDATE = DateTime.Now;
                    pBond.REJECTEDBY = Session["UserId"].ToString();
                    db.Entry(pBond).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
            }
            return RedirectToAction("ListPrivateBond", "PrivateBond");
        }


        public ActionResult EditPRIVATEBOND(PRIVATEBOND oPBond)
        {
            try
            {

                oPBond.STATUS = ConstantVariable.STATUS_ACCEPTED;
                oPBond.LASTUPDATED = DateTime.Now;
                oPBond.LASTUPDATEDBY = Session["UserId"].ToString();

                oPBond.GROSSINTEREST = Convert.ToDecimal(((oPBond.BONDSIZE * oPBond.COUPONRATE) / 100) * oPBond.TENURE);
                oPBond.SOURCETAX = (oPBond.GROSSINTEREST * oPBond.TAXRATE) / 100;
             
                oPBond.TOTALPURCHASEAMOUNT = oPBond.PURCHASEAMOUNT;

                oPBond.NETINTEREST = Convert.ToDecimal(oPBond.GROSSINTEREST - oPBond.SOURCETAX - oPBond.EXCISEDUTY - oPBond.OTHERCHARGE);
               

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    db.Entry(oPBond).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
            }

            return RedirectToAction("ListApprovedBond", "PrivateBond");
        
        }


        [HttpGet]
        public ActionResult ListApprovedBond(string sortdir, string sort, string sortdefault, int? rows, string filterstring, string lblbreadcum, string PagingType, DateTime? openingDate, string FI_REFERENCE, string STATUS)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<PRIVATEBOND> gridModels = new GridModel<PRIVATEBOND>();
                List<PRIVATEBOND> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ? 15 : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;


                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;

                ViewBag.CurrentFilter = filterstring;

                if (STATUS == null && string.IsNullOrEmpty(STATUS))
                    models = new Entities(Session["Connection"] as EntityConnection).PRIVATEBONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.STATUS == ConstantVariable.STATUS_ACCEPTED).OrderBy(sort + " " + sortdir).ToList();
                else
                {
                    models = new Entities(Session["Connection"] as EntityConnection).PRIVATEBONDs.Include("FINANCIALINSTITUTION").AsNoTracking().Where(t => t.STATUS ==STATUS).OrderBy(sort + " " + sortdir).ToList();
                }

                if (FI_REFERENCE != null && !string.IsNullOrEmpty(FI_REFERENCE))
                    models = models.Where(t => t.FINANCIALINSTITUTION_REFERENCE == FI_REFERENCE).ToList();


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



                var statusId = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "BOND" && app.PROPERTY == "STATUS").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == statusId).OrderBy(app => app.CODE).ToList();

                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME");
                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");


                if (Convert.ToString(TempData["Approved"]) == "Approved")
                {

                    ViewBag.Message = Convert.ToString(TempData["result"]);
                }

                return PartialView("ListApprovedBond", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

    }
}
