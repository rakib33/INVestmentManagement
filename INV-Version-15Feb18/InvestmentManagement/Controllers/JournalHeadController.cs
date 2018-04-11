using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using InvestmentManagement.ViewModel;

namespace InvestmentManagement.Controllers
{
    public class JournalHeadController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();

        public ActionResult ListJournalHead(string sortdir, DateTime? FromDate, DateTime? ToDate, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //Session["UserId"] = null;
                //Session["Connection"] = null;
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                //JournalHeadViewModel oJournalHeadViewModel = new JournalHeadViewModel();
                GridModel<JournalView> gridModels = new GridModel<JournalView>();
                List<JournalView> jVList = null;

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
                sort = string.IsNullOrEmpty(sort) == true ? "TRANSACTIONDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;

                jVList = new Portfolio().getJOURNAL_VIEW().Skip(skipcount).OrderByDescending(t=>t.TRANSACTIONDATE).Take(gridModels.RowsPerPage).ToList(); ;

                //if (string.IsNullOrEmpty(filterstring))
                //    models = new Entities(Session["Connection"] as EntityConnection).NOMINALACCOUNTs.AsNoTracking().OrderByDescending(t => t.CREATEDDATE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                //else
                //    models = new Entities(Session["Connection"] as EntityConnection).NOMINALACCOUNTs.AsNoTracking().Where(w => w.CAPTION.Contains(filterstring)).OrderByDescending(t => t.CREATEDDATE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();

                if (jVList != null && FromDate.HasValue)
                    jVList = jVList.Where(t => t.TRANSACTIONDATE >= FromDate).ToList();

                if (jVList != null && ToDate.HasValue)
                    jVList = jVList.Where(t => t.TRANSACTIONDATE <= ToDate).ToList();
                
                gridModels.DataModel = jVList;

                //oJournalHeadViewModel.JournalHeads = gridModels.DataModel;
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
               
                if (jVList.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }

                if (Convert.ToString(TempData["result"]) != null)
                {

                    ViewBag.Message = Convert.ToString(TempData["result"]);
                }

                return PartialView("ListJournalHead", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }


        public ActionResult AddJournalHead()
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                JournalHeadViewModel oJournalHeadViewModel = new JournalHeadViewModel();
                oJournalHeadViewModel.JournalHead = new JOURNALHEAD();
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
                ViewBag.JournalLine = new Entities(Session["Connection"] as EntityConnection).NOMINALACCOUNTs.ToList();
                //ViewBag.JournalLine = new SelectList(new Entities(Session["Connection"] as EntityConnection).NOMINALACCOUNTs, "REFERENCE", "CODE");
                ViewBag.FINANCIALYEAR = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALYEARs, "REFERENCE", "DESCRIPTION");
                ViewBag.PERIOD = new SelectList(new Entities(Session["Connection"] as EntityConnection).PERIODs, "REFERENCE", "DESCRIPTION");

                return PartialView("AddJournalHead");
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                TempData["result"] = ex.Message;
                return RedirectToAction("ListJournalHead", "JournalHead");
               // return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult AddJournalHead(JOURNALHEAD oJOURNALHEAD, string TRANSACTIONDATE, List<JOURNALLINE> JOURNALLINEs)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                if (Request.IsAjaxRequest())
                {
                    if (oJOURNALHEAD != null)
                    {
                        oJOURNALHEAD.REFERENCE = Guid.NewGuid().ToString();
                        oJOURNALHEAD.CREATEDBY = Session["UserId"].ToString();
                        oJOURNALHEAD.CREATEDDATE = DateTime.Now;
                        oJOURNALHEAD.LASTUPDATED = DateTime.Now;
                      
                      // TRANSACTIONDATE = DateTime.ParseExact(TRANSACTIONDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd/MM/yyyy");

                        oJOURNALHEAD.TRANSACTIONDATE = oJOURNALHEAD.TRANSACTIONDATE == null ? Convert.ToDateTime(TRANSACTIONDATE) : oJOURNALHEAD.TRANSACTIONDATE;

                        // oFDRPROPOSAL.LASTUPDATEDBY = "BOSL";
                        //oFDRPROPOSAL.PROPOSALID = Guid.NewGuid().ToString();

                        using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                        {
                            db.JOURNALHEADs.Add(oJOURNALHEAD);
                            db.SaveChanges();
                        }


                        if (JOURNALLINEs.Count > 0)
                        {
                            foreach (var oJOURNALLINE in JOURNALLINEs)
                            {
                                oJOURNALLINE.REFERENCE = Guid.NewGuid().ToString();
                                oJOURNALLINE.CREATEDBY = Session["UserId"].ToString();
                                oJOURNALLINE.CREATEDDATE = DateTime.Now;
                                oJOURNALLINE.LASTUPDATED = DateTime.Now;
                                oJOURNALLINE.JOURNALHEAD_REFERENCE = oJOURNALHEAD.REFERENCE;
                                

                                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                                {
                                    db.JOURNALLINEs.Add(oJOURNALLINE);
                                    db.SaveChanges();
                                }
                            }
                        
                        
                        }
                    
                    }

                

                }
                return RedirectToAction("ListJournalHead", "JournalHead");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }



        [HttpGet]
        public ActionResult EditJournalHead(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                double totalDebit=0;
                double totalCredit = 0;
                double totalBalance = 0;

                JOURNALHEAD oJOURNALHEAD = new JOURNALHEAD();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                //ViewModelBase oViewModelBase = new ViewModelBase();


                oJOURNALHEAD = db.JOURNALHEADs.SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.NOMINALACCOUNT = new Entities(Session["Connection"] as EntityConnection).NOMINALACCOUNTs.ToList();
               
               var jrLine = new Entities(Session["Connection"] as EntityConnection).JOURNALLINEs.Include("NOMINALACCOUNT").Where(jH => jH.JOURNALHEAD_REFERENCE == id).ToList();

               foreach (JOURNALLINE oJOURNALLINE in jrLine)
               {

                   totalDebit = totalDebit + Convert.ToDouble(oJOURNALLINE.DEBIT);
                   totalCredit = totalCredit + Convert.ToDouble(oJOURNALLINE.CREDIT);
                   totalBalance = totalBalance + Convert.ToDouble(oJOURNALLINE.NETAMOUNT);
               
               }

               ViewBag.totalDebit = totalDebit.ToString("N");
               ViewBag.totalCredit = totalCredit.ToString("N");
               ViewBag.totalBalance = totalBalance.ToString("N");

                ViewBag.JournalLine = new Entities(Session["Connection"] as EntityConnection).JOURNALLINEs.Include("NOMINALACCOUNT").Where(jH => jH.JOURNALHEAD_REFERENCE == id).OrderBy(jr=>jr.CREATEDDATE).ToList();
                //ViewBag.JournalLine = new SelectList(new Entities(Session["Connection"] as EntityConnection).NOMINALACCOUNTs, "REFERENCE", "CODE");
                ViewBag.FINANCIALYEAR = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALYEARs, "REFERENCE", "DESCRIPTION");
                ViewBag.PERIOD = new SelectList(new Entities(Session["Connection"] as EntityConnection).PERIODs, "REFERENCE", "DESCRIPTION");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];
                return PartialView("EditJournalHead", oJOURNALHEAD);
            }

            catch (Exception ex)
            {
                throw ex;

            }
        }


        [HttpPost]
        public ActionResult EditJournalHead(JOURNALHEAD oJOURNALHEAD, string TRANSACTIONDATE, List<JOURNALLINE> JOURNALLINEs)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oJOURNALHEAD.LASTUPDATED = DateTime.Now;
                    oJOURNALHEAD.LASTUPDATEDBY = Session["UserId"].ToString();
                 //TRANSACTIONDATE = DateTime.ParseExact(TRANSACTIONDATE,"dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy/MM/dd");
                   // TRANSACTIONDATE = TRANSACTIONDATE.ToString("yyyy/MM/dd");
                    oJOURNALHEAD.TRANSACTIONDATE = oJOURNALHEAD.TRANSACTIONDATE == null ? Convert.ToDateTime(TRANSACTIONDATE) : oJOURNALHEAD.TRANSACTIONDATE;
                    db.Entry(oJOURNALHEAD).State = EntityState.Modified;
                    db.SaveChanges();

                    if (JOURNALLINEs.Count > 0)
                    {
                        foreach (var oJOURNALLINE in JOURNALLINEs)
                        {
                           // var journalLine = db.JOURNALLINEs.Where(jrLine=>jrLine.REFERENCE==oJOURNALLINE.REFERENCE).SingleOrDefault();

                            if (oJOURNALLINE.REFERENCE != null)
                            {
                                oJOURNALLINE.LASTUPDATED = DateTime.Now;
                                oJOURNALLINE.LASTUPDATEDBY = Session["UserId"].ToString();
                                //oJOURNALLINE.JOURNALHEAD_REFERENCE = oJOURNALHEAD.REFERENCE;
                                using (Entities entity = new Entities(Session["Connection"] as EntityConnection))
                                {
                                    entity.Entry(oJOURNALLINE).State = EntityState.Modified; ;
                                    entity.SaveChanges();
                                }

                            }

                            else
                            {
                                oJOURNALLINE.REFERENCE = Guid.NewGuid().ToString();
                                oJOURNALLINE.CREATEDBY = Session["UserId"].ToString();
                                oJOURNALLINE.CREATEDDATE = DateTime.Now;
                                oJOURNALLINE.LASTUPDATED = DateTime.Now;
                                oJOURNALLINE.JOURNALHEAD_REFERENCE = oJOURNALHEAD.REFERENCE;

                                using (Entities entities = new Entities(Session["Connection"] as EntityConnection))
                                {
                                    entities.JOURNALLINEs.Add(oJOURNALLINE);
                                    entities.SaveChanges();
                                }
                            }

                         
                           
                        }


                    }
                }


                return RedirectToAction("ListJournalHead", "JournalHead");
            }

            catch (Exception ex)
            {
                throw ex;

            }
        }

    }
}
