using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using System.Data;
using InvestmentManagement.ViewModel;
using InvestmentManagement.InvestmentManagement.Models;
using System.Data.EntityClient;

//added by rakibul
using InvestmentManagement.App_Code;
using System.Configuration;
using System.Globalization;

namespace InvestmentManagement.Controllers
{
    public class FinancialInstitutionController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();

        #region EX_Code
        //public string SortDir
        //{
        //    get
        //    {
        //        return Session["SortDir"] == null ? null : Session["SortDir"].ToString();
        //    }

        //    private set
        //    {
        //        if (Session["SortDir"] == null)
        //        {
        //            Session["SortDir"] = value;
        //        }
        //        else
        //        {
        //            if (value != null)
        //            {
        //                if (Session["SortDir"].ToString() != value.ToString())
        //                {
        //                    Session["SortDir"] = value;
        //                }
        //            }
        //        }
        //    }
        //}

        //public string SortCol
        //{
        //    get
        //    {
        //        return Session["SortCol"] == null ? null : Session["SortCol"].ToString();
        //    }

        //    private set
        //    {
        //        if (Session["SortCol"] == null)
        //        {
        //            Session["SortCol"] = value;
        //        }
        //        else
        //        {
        //            if (value != null)
        //            {
        //                if (Session["SortCol"].ToString() != value.ToString())
        //                {
        //                    Session["SortCol"] = value;
        //                }
        //            }
        //        }
        //    }
        //}

        //public int? RowsPerPageCount
        //{
        //    get
        //    {
        //        if (Session["RowsPerPageCount"] != null)
        //        {
        //            return (int)Session["RowsPerPageCount"];
        //        }
        //        return null;
        //    }
        //    private set
        //    {
        //        Session["RowsPerPageCount"] = value;
        //    }
        //}


        //[HttpGet]
        //public ActionResult ListFinancialInstitution_old(string sortdir, string sort, int? page, int? rows, string filterstring, string lblbreadcum, string PagingType)
        //{
        //    try
        //    {
        //        GridModel<FINANCIALINSTITUTION> oFinancialGridModel = new GridModel<FINANCIALINSTITUTION>();
        //        List<FINANCIALINSTITUTION> financialInstitution = null;

        //        if (PagingType == null)
        //        {
        //            SortDir = sortdir;
        //            SortCol = sort;
        //            RowsPerPageCount = rows;

        //            Session["pageNo"] = 1;
        //        }


        //        if (PagingType == "Prev")
        //        {
        //            Session["pageNo"] = (int)Session["pageNo"] - 1;
        //        }
        //        if (PagingType == "Next")
        //        {

        //            Session["pageNo"] = (int)Session["pageNo"] + 1;
        //        }
        //        //string a = TempData["page"].ToString();

        //        oFinancialGridModel.CurrentPage = (int)Session["pageNo"] == 1 ? 1 : (int)Session["pageNo"];
        //        oFinancialGridModel.RowsPerPage = RowsPerPageCount == null ? 15 : (int)RowsPerPageCount;


        //        //if (gridModel.CurrentPage > 1)
        //        //{
        //        //    if (SortDir == null)
        //        //        financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderByDescending(o => o.CREATEDDATED).Skip(gridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(gridModel.RowsPerPage).ToList();
        //        //    else if (SortDir == "DESC")
        //        //        financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(SortCol + " DESC").Skip(gridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(gridModel.RowsPerPage).ToList();
        //        //    else
        //        //        financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(SortCol).Skip(gridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(gridModel.RowsPerPage).ToList();



        //        //}
        //        //else
        //        //{
        //        //    if (!string.IsNullOrEmpty(filterstring))
        //        //    {

        //        //        financialInstitution = new Entities().FINANCIALINSTITUTIONs.Where(w => w.CODE == filterstring).ToList();

        //        //    }
        //        //    else if (SortDir == null)
        //        //        financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderByDescending(o => o.CREATEDDATED).Take(gridModel.RowsPerPage).ToList();
        //        //    else if (SortDir == "DESC")
        //        //        financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(SortCol + " DESC").Skip(gridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(gridModel.RowsPerPage).ToList();


        //        //    else
        //        //        financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(SortCol).Skip(gridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(gridModel.RowsPerPage).ToList();

        //        //}


        //        if (oFinancialGridModel.CurrentPage > 1)
        //        {
        //            if (!string.IsNullOrEmpty(filterstring))
        //            {
        //                if (SortDir == "DESC")
        //                    financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().Where(w => w.CODE == filterstring).OrderBy(SortCol + " DESC").Skip(oFinancialGridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(oFinancialGridModel.RowsPerPage).ToList();


        //                else if (SortDir == "ASC")
        //                    financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().Where(w => w.CODE == filterstring).OrderBy(SortCol).Skip(oFinancialGridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(oFinancialGridModel.RowsPerPage).ToList();

        //                else

        //                    financialInstitution = new Entities().FINANCIALINSTITUTIONs.Where(w => w.CODE == filterstring).Take(oFinancialGridModel.RowsPerPage).Skip(oFinancialGridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).ToList(); ;

        //            }
        //            else if (SortDir == null)
        //                financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderByDescending(o => o.CREATEDDATED).Skip(oFinancialGridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(oFinancialGridModel.RowsPerPage).ToList();
        //            else if (SortDir == "DESC")
        //                financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(SortCol + " DESC").Skip(oFinancialGridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(oFinancialGridModel.RowsPerPage).ToList();


        //            else
        //                financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(SortCol).Skip(oFinancialGridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(oFinancialGridModel.RowsPerPage).ToList();




        //            //if (SortDir == null)
        //            //    financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderByDescending(o => o.CREATEDDATED).Skip(gridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(gridModel.RowsPerPage).ToList();
        //            //else if (SortDir == "DESC")
        //            //    financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(SortCol + " DESC").Skip(gridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(gridModel.RowsPerPage).ToList();
        //            //else
        //            //    financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(SortCol).Skip(gridModel.RowsPerPage * ((int)Session["pageNo"] - 1)).Take(gridModel.RowsPerPage).ToList();



        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(filterstring))
        //            {
        //                if (SortDir == "DESC")
        //                    financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().Where(w => w.CODE == filterstring).OrderBy(SortCol + " DESC").Take(oFinancialGridModel.RowsPerPage).ToList();


        //                else if (SortDir == "ASC")
        //                    financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().Where(w => w.CODE == filterstring).OrderBy(SortCol).Take(oFinancialGridModel.RowsPerPage).ToList();

        //                else

        //                    financialInstitution = new Entities().FINANCIALINSTITUTIONs.Where(w => w.CODE == filterstring).Take(oFinancialGridModel.RowsPerPage).ToList(); ;

        //            }
        //            else if (SortDir == null)
        //                financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderByDescending(o => o.CREATEDDATED).Take(oFinancialGridModel.RowsPerPage).ToList();
        //            else if (SortDir == "DESC")
        //                financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(SortCol + " DESC").Take(oFinancialGridModel.RowsPerPage).ToList();


        //            else
        //                financialInstitution = new Entities().FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(SortCol).Take(oFinancialGridModel.RowsPerPage).ToList();

        //        }


        //        oFinancialGridModel.DataModel = financialInstitution;
        //        //GridViewModel<FINANCIALINSTITUTION> oFinancialGridModel = new GridViewModel<FINANCIALINSTITUTION>();
        //        //oFinancialGridModel.GridModels = gridModel;
        //        ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());

        //        if (lblbreadcum != null)
        //        {
        //            Session["currentPage"] = lblbreadcum;
        //        }

        //        if (financialInstitution.Count() < oFinancialGridModel.RowsPerPage)
        //        {

        //            ViewBag.Prev = "disabled";
        //            ViewBag.Next = "disabled";
        //            ViewBag.PrevNotActive = "not-active";
        //            ViewBag.NextNotActive = "not-active";
        //        }

        //        else if (oFinancialGridModel.CurrentPage == 1)
        //        {
        //            ViewBag.Prev = "disabled";
        //            ViewBag.PrevNotActive = "not-active";
        //        }


        //            return PartialView("ListFinancialInstitution", oFinancialGridModel);


        //    }

        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }



        //}

        #endregion

        [HttpGet]
        public ActionResult ListFinancialInstitution(string sortdir, string sort, string sortdefault, int? rows, string filterstring, string lblbreadcum, string PagingType)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
              
                //if (string.IsNullOrEmpty(sortdefault))
                //{
                //    switch (PagingType)
                //    {
                //        case "Next":
                //            Session["pageNo"] = (int)Session["pageNo"] + 1;

                //            break;
                //        case "Prev":
                //            Session["pageNo"] = (int)Session["pageNo"] - 1;
                //            break;
                //        default:
                //            Session["pageNo"] = 1;
                         
                //            break;
                //    }
                //}
                //else
                //{
                //    sort = sortdefault;
                //}
            
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<FINANCIALINSTITUTION> gridModels = new GridModel<FINANCIALINSTITUTION>();
                List<FINANCIALINSTITUTION> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ?15 : (int)rows;
                //ViewBag.currentRowPerPage = gridModels.RowsPerPage;

                //Paging direction
                int count = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.Count();


                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "NAME" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "ASC" : sortdir;
                //int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                //if (filterstring == null)
                //{
                //    filterstring = currentFilter;
                //}


                //ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(sort + " " + sortdir).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.AsNoTracking().Where(w => w.NAME.ToLower().Contains(filterstring.ToLower())).OrderBy(sort + " " + sortdir).ToList();


                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                //if ((int)Session["pageNo"] == 1)
                //{          
                //            ViewBag.Prev = "disabled";
                //            ViewBag.PrevNotActive = "not-active";
                //}
                //if (models.Count() < gridModels.RowsPerPage && (int)Session["pageNo"] == 1)
                //{

                //    ViewBag.Prev = "disabled";
                //    ViewBag.Next = "disabled";
                //    ViewBag.PrevNotActive = "not-active";
                //    ViewBag.NextNotActive = "not-active";
                //}
                //if (models.Count() < gridModels.RowsPerPage && (int)Session["pageNo"] > 1)
                //{
                //    ViewBag.Next = "disabled";
                //    ViewBag.NextNotActive = "not-active";
                
                //}

                    
                return PartialView("ListFinancialInstitution", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        public ActionResult ListFinancialInstitutionNew(string lblbreadcum)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    if (lblbreadcum != null)
                    {
                        Session["currentPage"] = lblbreadcum;
                    }
                    var financilaInstitutions = db.FINANCIALINSTITUTIONs.AsNoTracking().OrderBy(f => f.CREATEDDATE).Take(5000).ToList();
                    ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                    return PartialView("ListFinancialInstitutionNew", financilaInstitutions);
                }

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        
        
        }


        public ActionResult AddFinancialInstitution()
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                ViewModelBase oViewModelBase = new ViewModelBase();
                oViewModelBase.Currencies = new Entities(Session["Connection"] as EntityConnection).CURRENCies.ToList();
                // oViewModelBase.Menus = new Entities().MENUs.ToList();
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "Financial Institution" && app.PROPERTY == "Financial Institution").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app => app.DESCRIPTION).ToList();
                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");

                return PartialView("AddFinancialInstitution", oViewModelBase);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }

        }

        [HttpPost]
        public ActionResult AddFinancialInstitution(FINANCIALINSTITUTION oFINANCIALINSTITUTION)
        {
            // List<FINANCIALINSTITUTION> FinacialInstitutions = new List<FINANCIALINSTITUTION>();

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                if (Request.IsAjaxRequest())
                {

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        
                        oFINANCIALINSTITUTION.REFERENCE = Guid.NewGuid().ToString();
                        //oFINANCIALINSTITUTION.REFERENCE = Guid.NewGuid().ToString();
                        oFINANCIALINSTITUTION.CREATEDBY = Session["UserId"].ToString();
                        oFINANCIALINSTITUTION.CREATEDDATE = DateTime.Now;
                        oFINANCIALINSTITUTION.LASTUPDATED = DateTime.Now;

                        if (oFINANCIALINSTITUTION.ISSELECT == "true")
                        {
                            oFINANCIALINSTITUTION.ISSELECT = "Y";
                        }
                        else
                            oFINANCIALINSTITUTION.ISSELECT = "N";
                        

                        oCommonFunction.CustomObjectNullValidation<FINANCIALINSTITUTION>(ref oFINANCIALINSTITUTION);
                        db.FINANCIALINSTITUTIONs.Add(oFINANCIALINSTITUTION);

                       db.SaveChanges();

                    }

                }

                //Index(null, null, null, null, null);
                return RedirectToAction("ListFinancialInstitution");
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }

        [HttpGet]
        public ActionResult EditFinancialInstitution(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                FINANCIALINSTITUTION oFinancilaInstitution = new FINANCIALINSTITUTION();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();
                oViewModelBase.Currencies = new Entities(Session["Connection"] as EntityConnection).CURRENCies.ToList();

                oFinancilaInstitution = db.FINANCIALINSTITUTIONs.SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.Currency = new Entities(Session["Connection"] as EntityConnection).CURRENCies.ToList();
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                string[] InstitutionsTypes = new string[] { "Bank", "Leasing", "Merchant Bank" };
                //ViewBag.InstitutionsTypes=new string[] {"Bank","Leasing","Merchant Bank"};
             
                ViewBag.InstitutionsTypes = new SelectList(InstitutionsTypes, oFinancilaInstitution.INSTITUTIONTYPE, oFinancilaInstitution.INSTITUTIONTYPE);
                ViewBag.SelectedInstitutionType = oFinancilaInstitution.INSTITUTIONTYPE == null ? "Institution Type" : oFinancilaInstitution.INSTITUTIONTYPE;

                ViewBag.SelectedInstitutionType = oFinancilaInstitution.INSTITUTIONTYPE;

                var status = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.Where(app => app.ENTITY == "Financial Institution" && app.PROPERTY == "Financial Institution").FirstOrDefault().REFERENCE.ToString();
                var statuslist = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Where(app => app.STATUSPARAMETER_REFERENCE == status).OrderBy(app => app.DESCRIPTION).ToList();
                ViewBag.statuslist = new SelectList(statuslist, "DESCRIPTION", "DESCRIPTION");
           

                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                if (oFinancilaInstitution.ISSELECT == "Y")
                    ViewBag.Selected = "checked";
             
                    


                return PartialView("EditFinancialInstitution", oFinancilaInstitution);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }

        }

        [HttpPost]
        public ActionResult EditFinancialInstitution(FINANCIALINSTITUTION oFINANCIALINSTITUTION)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oFINANCIALINSTITUTION.LASTUPDATED = DateTime.Now;
                    oFINANCIALINSTITUTION.LASTUPDATEDBY = Session["UserId"].ToString();

                    if (oFINANCIALINSTITUTION.ISSELECT == "checked")
                    {
                        oFINANCIALINSTITUTION.ISSELECT = "Y";
                    }
                    else
                        oFINANCIALINSTITUTION.ISSELECT = "N";
                        

                    oCommonFunction.CustomObjectNullValidation<FINANCIALINSTITUTION>(ref oFINANCIALINSTITUTION);
                    db.Entry(oFINANCIALINSTITUTION).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListFinancialInstitution");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        public ActionResult Test()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ListOfBranch(int? rows, string FINANCIALINSTITUTION, string lblbreadcum, string sortdir, string sort, int? page, string filterstring, string currentFilter, string PagingType, int? currentRowPerPage = 50, string reference = null)
        {
            try
            {
                //added 
                var FinancialInsFromView=TempData["financialInstitution"] as string;
                ViewBag.PostBAck = "False";
                
                //if (!string.IsNullOrEmpty(FinancialInsFromView))
                //{
                //    FINANCIALINSTITUTION = FinancialInsFromView;
                //    ViewBag.PostBAck = "True";
                //}

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                var DeleteMsg = TempData["Deleted"] as string;

                if (!string.IsNullOrEmpty(DeleteMsg))
                {
                    ViewBag.Message = DeleteMsg;
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<FIBRANCH> gridModels = new GridModel<FIBRANCH>();
                List<FIBRANCH> models = new List<FIBRANCH>();

                //grid settings  paging              
               
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
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    //paging
                    if (!string.IsNullOrEmpty(FINANCIALINSTITUTION))
                    {
                        models = db.FIBRANCHes.ToList();

                        models=models.Where(t=>t.FINANCIALINSTITUTION_REFERENCE==FINANCIALINSTITUTION).ToList();

                       // models = new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.Where(w => w.FINANCIALINSTITUTION_REFERENCE == FINANCIALINSTITUTION).OrderBy(fiBranch => fiBranch.NAME).ToList();

                     

                        //var BranchList = (from branch in models.ToList()
                        //                  where branch.FINANCIALINSTITUTION_REFERENCE == FINANCIALINSTITUTION
                        //                  select branch).ToList();

                        // models = BranchList.ToList();

                        //added by rakibul
                        ViewBag.FI = FINANCIALINSTITUTION;
                        //end editing                
                    }
                }
                //else
                //    models = new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.AsNoTracking().Where(w => w.NAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).ToList();


                gridModels.DataModel = models;

              //  ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                ViewBag.financialInstitutionList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALINSTITUTIONs.OrderBy(f => f.NAME).ToList(), "REFERENCE", "NAME", FINANCIALINSTITUTION);
          
                return PartialView("ListOfBranch", gridModels);

            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        //[HttpGet]
        //public ActionResult AddFinancialBranch(string financialInstituteReference)
        //{
        //    try
        //    {
        //        if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
        //        {
        //            return RedirectToAction("LogOut", "Home");

        //        }

        //        FIBRANCH oFibranch = new FIBRANCH();
        //        oFibranch.CREATEDBY = Session["UserId"].ToString();
        //        oFibranch.FINANCIALINSTITUTION_REFERENCE = financialInstituteReference;
        //        oFibranch.CREATEDDATE = DateTime.Now;
        //        ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
        //        ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
        //        ViewBag.Header = "Add " + Session["currentPage"];

        //        //save selected bank reference by rakibul date 28/11/2015
        //        ViewBag.FI = financialInstituteReference;
        //        //ViewBag.Reference = reference;
        //        return PartialView("EditBranch", oFibranch);

        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.Message;

        //        return RedirectToAction("Index", "ErrorPage", new { message });

        //    }
        //}

        [HttpGet]
        public ActionResult AddFinancialBranch(string financialInstituteReference)
        {
           
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }



                FIBRANCH oFibranch = new FIBRANCH();
                oFibranch.CREATEDBY = Session["UserId"].ToString();
                oFibranch.FINANCIALINSTITUTION_REFERENCE = financialInstituteReference;
                oFibranch.CREATEDDATE = DateTime.Now;
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                //save selected bank reference by rakibul date 28/11/2015
                ViewBag.Refernec = financialInstituteReference;

                return PartialView("EditBranch", oFibranch);

            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
            // return PartialView("RiOntest");
        }

        
        [HttpGet]
        public ActionResult EditBranch(string branchReference)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null ||
                    Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                var financialInstituteReference = TempData["financialInstitution"] as string;

                FIBRANCH oBranch = new FIBRANCH();

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    
                    oBranch = db.FIBRANCHes.SingleOrDefault(i => i.REFERENCE == branchReference);

                    ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                    ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                    ViewBag.Header = "Update " + Session["currentPage"];

                    //save selected bank reference by rakibul date 28/11/2015
                    ViewBag.Refernec = financialInstituteReference;

                    return PartialView("EditBranch", oBranch);
                }

            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }

        }

        [HttpPost]
        public ActionResult EditBranch(FIBRANCH oFibranch)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    if (oFibranch.REFERENCE != null)
                    {
                        oFibranch.LASTUPDATED = DateTime.Now;
                        oFibranch.LASTUPDATEDBY = Session["UserId"].ToString();
                        oCommonFunction.CustomObjectNullValidation<FIBRANCH>(ref oFibranch);
                        db.Entry(oFibranch).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        String guid = Guid.NewGuid().ToString();
                        oFibranch.REFERENCE = guid;
                        db.FIBRANCHes.Add(oFibranch);
                        db.SaveChanges();
                    }
                    return RedirectToAction("ListOfBranch", new { FINANCIALINSTITUTION = oFibranch.FINANCIALINSTITUTION_REFERENCE });
                }

                
            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        /// <summary>
        /// added by rakibul date<23th november,2015>
        /// to delete a record 
        /// </summary>
        /// <param name="branchReference"></param>
        /// <returns></returns>


        [HttpGet]
        public ActionResult Delete(string branchReference)
        {
            try
            {
                var FI = TempData["financialInstitution"] as string;


                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null ||
                    Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                FIBRANCH oBranch = new FIBRANCH();

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {


                    oBranch = db.FIBRANCHes.SingleOrDefault(i => i.REFERENCE == branchReference);

                    ViewBag.Message = ConstantVariable.DELETE_MSG;
                    ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                    ViewBag.Header = "Delete" + Session["currentPage"];
                    ViewBag.Reference = oBranch.FINANCIALINSTITUTION_REFERENCE;

                    return PartialView("DeleteBranch", oBranch);
                }

            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult Delete(FIBRANCH model)
        {

            var FI = TempData["financialInstitution"] as string;


            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null ||
                Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            FIBRANCH oBranch = new FIBRANCH();

            using (Entities db = new Entities(Session["Connection"] as EntityConnection))
            {
                try
                {
                    oBranch = db.FIBRANCHes.SingleOrDefault(i => i.REFERENCE == model.REFERENCE);

                    if (oBranch != null)
                    {
                        db.FIBRANCHes.Attach(oBranch);
                        db.FIBRANCHes.Remove(oBranch);
                        db.SaveChanges();
                        TempData["Deleted"] = ConstantVariable.DELETE_SUCCESS;
                    }
                    return RedirectToAction("ListOfBranch", new { FINANCIALINSTITUTION = model.FINANCIALINSTITUTION_REFERENCE });

                }
                catch (Exception ex)
                {
                    string ms = ex.Message;
                }
            }

            return View(model);

        }

       
    }
}
