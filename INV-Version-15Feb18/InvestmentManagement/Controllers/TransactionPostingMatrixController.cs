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
using System.Data.Entity.Validation;
namespace InvestmentManagement.Controllers
{
    public class TransactionPostingMatrixController : Controller
    {
        //
        // GET: /TransactionPostingMatrix/

        CommonFunction oCommonFunction = new CommonFunction();

        [HttpGet]
        public ActionResult ListTransactionPostingMatrix(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<TRANSACTIONPOSTINGMATRIX> gridModels = new GridModel<TRANSACTIONPOSTINGMATRIX>();
                List<TRANSACTIONPOSTINGMATRIX> models = null;

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
                sort = string.IsNullOrEmpty(sort) == true ? "TRANSACTIONCODE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).TRANSACTIONPOSTINGMATRIces.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();//.Include("NOMINALACCOUNT")
                else
                    models = new Entities(Session["Connection"] as EntityConnection).TRANSACTIONPOSTINGMATRIces.AsNoTracking().Where(w => w.DESCRIPTION.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList(); //.Include("NOMINALACCOUNT")


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
                return PartialView("ListTransactionPostingMatrix", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddTransactionPostingMatrix(string lblbreadcum)
        {
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);

                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                var bsNAList = db.NOMINALACCOUNTs.Where(na => na.ACCOUNTTYPE == "Balance Sheet").OrderBy(na => na.ORACLENOMINALCODE).ToList();
                var plNAList = db.NOMINALACCOUNTs.Where(na => na.ACCOUNTTYPE == "Profit and Loss").OrderBy(na => na.ORACLENOMINALCODE).ToList();

                //ViewBag.BSNAList = new SelectList(bsNAList, "REFERENCE", "DESCRIPTION");
                //ViewBag.PLNAList = new SelectList(plNAList, "REFERENCE", "DESCRIPTION");

                ViewBag.BSNAList = new SelectList(bsNAList, "CODE", "DESCRIPTION");
                ViewBag.PLNAList = new SelectList(plNAList, "CODE", "DESCRIPTION");


                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];


                TRANSACTIONPOSTINGMATRIX model = new TRANSACTIONPOSTINGMATRIX();
                return PartialView("AddTransactionPostingMatrix", model);


            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult AddTransactionPostingMatrix(TRANSACTIONPOSTINGMATRIX oTRANSACTIONPOSTINGMATRIX)
        {
           try
            {



                if (Request.IsAjaxRequest())
                {
                    //get Nominal Account from 

                    oTRANSACTIONPOSTINGMATRIX.REFERENCE = Guid.NewGuid().ToString();

                    oTRANSACTIONPOSTINGMATRIX.CREATEDBY = Session["UserId"].ToString();
                    oTRANSACTIONPOSTINGMATRIX.CREATEDDATE = DateTime.Today;
                    oTRANSACTIONPOSTINGMATRIX.LASTUPDATED = DateTime.Today;

                    oCommonFunction.CustomObjectNullValidation<TRANSACTIONPOSTINGMATRIX>(ref oTRANSACTIONPOSTINGMATRIX);


                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {                       

                        db.TRANSACTIONPOSTINGMATRIces.Add(oTRANSACTIONPOSTINGMATRIX);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListTransactionPostingMatrix", "TransactionPostingMatrix");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult EditTransactionPostingMatrix(string id)
        {
            try
            {
                TRANSACTIONPOSTINGMATRIX oTRANSACTIONPOSTINGMATRIX = new TRANSACTIONPOSTINGMATRIX();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();

                var bsNAList = db.NOMINALACCOUNTs.Where(na => na.ACCOUNTTYPE == "Balance Sheet").OrderBy(na => na.ORACLENOMINALCODE).ToList();
                var plNAList = db.NOMINALACCOUNTs.Where(na => na.ACCOUNTTYPE == "Profit and Loss").OrderBy(na => na.ORACLENOMINALCODE).ToList();



                oTRANSACTIONPOSTINGMATRIX = db.TRANSACTIONPOSTINGMATRIces.SingleOrDefault(i => i.REFERENCE == id);
                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                //ViewBag.BSNAList = new SelectList(bsNAList, "REFERENCE", "DESCRIPTION");
                //ViewBag.PLNAList = new SelectList(plNAList, "REFERENCE", "DESCRIPTION");

                ViewBag.BSNAList = new SelectList(bsNAList, "CODE", "DESCRIPTION");
                ViewBag.PLNAList = new SelectList(plNAList, "CODE", "DESCRIPTION");

                return PartialView("EditTransactionPostingMatrix", oTRANSACTIONPOSTINGMATRIX);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult EditTransactionPostingMatrix(TRANSACTIONPOSTINGMATRIX oTRANSACTIONPOSTINGMATRIX)
        {

            try
            {
                if (Request.IsAjaxRequest())
                {

                    NOMINALACCOUNT NOM = new NOMINALACCOUNT();

                    oTRANSACTIONPOSTINGMATRIX.LASTUPDATED = DateTime.Today;
                    oTRANSACTIONPOSTINGMATRIX.LASTUPDATEDBY = Session["UserId"].ToString();
                    oCommonFunction.CustomObjectNullValidation<TRANSACTIONPOSTINGMATRIX>(ref oTRANSACTIONPOSTINGMATRIX);
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {

                        //if (oTRANSACTIONPOSTINGMATRIX.BSDEBITCONTROL != null && !string.IsNullOrEmpty(oTRANSACTIONPOSTINGMATRIX.BSDEBITCONTROL))
                        //{
                        //    NOM = db.NOMINALACCOUNTs.Where(t => t.REFERENCE == oTRANSACTIONPOSTINGMATRIX.BSDEBITCONTROL).SingleOrDefault();

                        //    oTRANSACTIONPOSTINGMATRIX.BSDEBITCONTROL = NOM.DESCRIPTION;
                        //}
                        //else if (oTRANSACTIONPOSTINGMATRIX.BSCREDITCONTROL != null && !string.IsNullOrEmpty(oTRANSACTIONPOSTINGMATRIX.BSCREDITCONTROL))
                        //{
                        //    NOM = db.NOMINALACCOUNTs.Where(t => t.REFERENCE == oTRANSACTIONPOSTINGMATRIX.BSCREDITCONTROL).SingleOrDefault();

                        //    oTRANSACTIONPOSTINGMATRIX.BSCREDITCONTROL = NOM.DESCRIPTION;
                        //}
                        //else
                        //{
                        //    return RedirectToAction("Index", "ErrorPage", new { message = "Debit Control or Credit Control is Required !" });
                        //}
                        //oTRANSACTIONPOSTINGMATRIX.NOMINALACCOUNT = NOM;

                        // oTRANSACTIONPOSTINGMATRIX.NOMINALACC_REFERENCE = NOM.REFERENCE;

                     


                        db.Entry(oTRANSACTIONPOSTINGMATRIX).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("ListTransactionPostingMatrix", "TransactionPostingMatrix");

            }
            catch (DbEntityValidationException e)
            {
                string message = "";
                foreach (var eve in e.EntityValidationErrors)
                {                  
                    foreach (var ve in eve.ValidationErrors)
                    {
                         message += ve.PropertyName + " " + ve.ErrorMessage;
                        //Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                        //    ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}
