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
using System.Data.OracleClient;

namespace InvestmentManagement.Controllers
{
    public class InstrumentController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /Instrument/

        [HttpGet]
        public ActionResult ListInstrument(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 1000)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<INSTRUMENT> gridModels = new GridModel<INSTRUMENT>();
                List<INSTRUMENT> models = null;

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
                int skipcount = 0;
                //if (filterstring == null)
                //{
                //    filterstring = currentFilter;
                //}


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                     models = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Include("INSTRUMENTCATEGORY").AsNoTracking().OrderBy(t=>t.SHORTNAME).ToList();

                   // models = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Include("INSTRUMENTCATEGORY").AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Include("INSTRUMENTCATEGORY").AsNoTracking().Where(w => w.SHORTNAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).ToList();

                  // models = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Include("INSTRUMENTCATEGORY").AsNoTracking().Where(w => w.SHORTNAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();

                if (!string.IsNullOrEmpty(currentFilter))
                    models = models.Where(t => t.ISIN == currentFilter).ToList();



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

             
                return PartialView("ListInstrument", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddInstrument()
        {
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }   
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
                ViewBag.instrumentCategoryList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INSTRUMENTCATEGORies, "REFERENCE", "DESCRIPTION");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");

                var AppParameterSector = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(t => t.ENTITY == "Instrument Sector").SingleOrDefault();
                var AppParameterDetails = new SelectList(new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(t => t.APPLICATIONPARAMETER_REFERENCE == AppParameterSector.REFERENCE).ToList(), "CODE", "CODE");
                ViewBag.InstrumentSectorList = AppParameterDetails;

                //ViewBag.userGroupList = new Sel "Instrument Sector";

                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddInstrument(INSTRUMENT oINSTRUMENT)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                    {
                        return RedirectToAction("LogOut", "Home");

                    }                   
                   
                   
                    oINSTRUMENT.REFERENCE = Guid.NewGuid().ToString();
                    oINSTRUMENT.CREATEDBY = Session["UserId"].ToString();
                    oINSTRUMENT.CREATEDDATE = DateTime.Now;
                    oINSTRUMENT.LASTUPDATED = DateTime.Now;

                    oINSTRUMENT.SHORTNAME = oINSTRUMENT.SHORTNAME.ToUpper();


                    oCommonFunction.CustomObjectNullValidation<INSTRUMENT>(ref oINSTRUMENT);
                    //oINSTRUMENT.CREATEDBY = "BOSL";
                    //oINSTRUMENT.CREATEDDATE = DateTime.Now;
                    //oINSTRUMENT.LASTUPDATED = DateTime.Now;
                    //oINSTRUMENT.LASTUPDATEDBY = "BOSL";
                    oINSTRUMENT.ALLOWNETTING = oINSTRUMENT.ALLOWNETTING == "true" ? "Y" : "N";
                    oINSTRUMENT.ISNONMARGINABLE = oINSTRUMENT.ISNONMARGINABLE == "true" ? "Y" : "N";
                    oINSTRUMENT.ISSPOT = oINSTRUMENT.ISSPOT == "true" ? "Y" : "N";

                    //oINSTRUMENT.INSTRUMENTID = oINSTRUMENT.INSTRUMENTID == null ? "".ToString() : oINSTRUMENT.INSTRUMENTID;
                    //oINSTRUMENT.CSEID = oINSTRUMENT.CSEID == null ? "".ToString() : oINSTRUMENT.CSEID;
                    //oINSTRUMENT.INSTRUMENTTYPE = oINSTRUMENT.INSTRUMENTTYPE == null ? "".ToString() : oINSTRUMENT.INSTRUMENTTYPE;
                    //oINSTRUMENT.SHORTNAME = oINSTRUMENT.SHORTNAME == null ? "".ToString() : oINSTRUMENT.SHORTNAME;
                    //oINSTRUMENT.ISIN = oINSTRUMENT.ISIN == null ? "".ToString() : oINSTRUMENT.ISIN;
                    //oINSTRUMENT.NAME = oINSTRUMENT.NAME == null ? "".ToString() : oINSTRUMENT.NAME;
                    //oINSTRUMENT.CATEGORY = oINSTRUMENT.CATEGORY == null ? "".ToString() : oINSTRUMENT.CATEGORY;
                   
                    //oINSTRUMENT.DECLARATIONDATE = oINSTRUMENT.DECLARATIONDATE == null ? DateTime.Now : oINSTRUMENT.DECLARATIONDATE;
                    //oINSTRUMENT.FACEVALUE = oINSTRUMENT.FACEVALUE == null ? (decimal)0.00 : oINSTRUMENT.FACEVALUE;
                    //oINSTRUMENT.PREMIUM = oINSTRUMENT.PREMIUM == null ? (decimal)0.00 : oINSTRUMENT.PREMIUM;
                    //oINSTRUMENT.IPO = oINSTRUMENT.IPO == null ? (decimal)0.00 : oINSTRUMENT.IPO;
                    //oINSTRUMENT.TOTALSHARE = oINSTRUMENT.TOTALSHARE == null ? (decimal)0 : oINSTRUMENT.TOTALSHARE;
                    //oINSTRUMENT.PUBLICSHARE = oINSTRUMENT.PUBLICSHARE == null ? (decimal)0 : oINSTRUMENT.PUBLICSHARE;
                    //oINSTRUMENT.MARKETLOT = oINSTRUMENT.MARKETLOT == null ? (decimal)0 : oINSTRUMENT.MARKETLOT;
                    //oINSTRUMENT.NETASSETVALUE = oINSTRUMENT.NETASSETVALUE == null ? (decimal)0.00 : oINSTRUMENT.NETASSETVALUE;
                    //oINSTRUMENT.LATESTEPS = oINSTRUMENT.LATESTEPS == null ? (decimal)0.00 : oINSTRUMENT.LATESTEPS;
                    //oINSTRUMENT.PERATIO = oINSTRUMENT.PERATIO == null ? (decimal)0.00 : oINSTRUMENT.PERATIO;
                    //oINSTRUMENT.LASTMARKETPRICE = oINSTRUMENT.LASTMARKETPRICE == null ? (decimal)0.00 : oINSTRUMENT.LASTMARKETPRICE;
                    //oINSTRUMENT.STATUS = oINSTRUMENT.STATUS == null ? "".ToString() : oINSTRUMENT.STATUS;

                    //oFIXEDDEPOSIT.PRINCIPALAMOUNT = oFIXEDDEPOSIT.PRINCIPALAMOUNT == null ? (decimal)0.00 : oFIXEDDEPOSIT.PRINCIPALAMOUNT;
                    //oFIXEDDEPOSIT.TENURE = oFIXEDDEPOSIT.TENURE == null ? (int)0 : oFIXEDDEPOSIT.TENURE;
                    //oFIXEDDEPOSIT.TERMSINDAYS = oFIXEDDEPOSIT.TERMSINDAYS == null ? (decimal)0 : oFIXEDDEPOSIT.TERMSINDAYS;
                    //oFIXEDDEPOSIT.EXISTINGCAPLIMIT = oFIXEDDEPOSIT.EXISTINGCAPLIMIT == null ? (decimal)0.00 : oFIXEDDEPOSIT.EXISTINGCAPLIMIT;
                    
                    
                    //oUSERGROUP.LASTLOGIN = DateTime.Now;
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                         //Check the instrument is previously added <Dated 23-02-17>
                        var hasInstrument = db.INSTRUMENTs.Where(t => t.ISIN == oINSTRUMENT.ISIN).SingleOrDefault();

                        if (hasInstrument == null) //now Instrument so save new record
                        {
                            if (oINSTRUMENT.INSTRUMENTCATEGORY_REFERENCE != null)
                            {
                                db.INSTRUMENTs.Add(oINSTRUMENT);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "ErrorPage", new { message="Instrument "+oINSTRUMENT.SHORTNAME +" already added.Please See the list." });
                        }
                    }

                }
                return RedirectToAction("ListInstrument", "Instrument");


            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult EditInstrument(string id)
        {
            try
            {
                INSTRUMENT oINSTRUMENT = new INSTRUMENT();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oINSTRUMENT = db.INSTRUMENTs.Include("INSTRUMENTCATEGORY").SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.instrumentCategoryList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INSTRUMENTCATEGORies, "REFERENCE", "DESCRIPTION");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];


                var AppParameterSector = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.Where(t => t.ENTITY == "Instrument Sector").SingleOrDefault();
                var AppParameterDetails = new SelectList(new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Where(t => t.APPLICATIONPARAMETER_REFERENCE == AppParameterSector.REFERENCE).ToList(), "CODE", "CODE");
                ViewBag.InstrumentSectorList = AppParameterDetails;


                return PartialView("EditInstrument", oINSTRUMENT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult EditInstrument(INSTRUMENT oINSTRUMENT)
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
                oINSTRUMENT.SHORTNAME = oINSTRUMENT.SHORTNAME.ToUpper();
                oINSTRUMENT.LASTUPDATED = DateTime.Today;
                oINSTRUMENT.LASTUPDATEDBY = Session["UserId"].ToString();
                oCommonFunction.CustomObjectNullValidation<INSTRUMENT>(ref oINSTRUMENT);
                oINSTRUMENT.ISNONMARGINABLE = oINSTRUMENT.ISNONMARGINABLE == "true" ? "Y" : "N";
                oINSTRUMENT.ALLOWNETTING = oINSTRUMENT.ALLOWNETTING == "true" ? "Y" : "N";
                oINSTRUMENT.ISSPOT = oINSTRUMENT.ISSPOT == "true" ? "Y" : "N";
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    if (oINSTRUMENT.INSTRUMENTCATEGORY_REFERENCE !=null)
                    {
                        db.Entry(oINSTRUMENT).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    
                }

                return RedirectToAction("ListInstrument", "Instrument");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult MutualBonusReconcile(string isin)
        {

            string result="0";
            try{


                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                OracleConnection conn = new OracleConnection(ConnectionString);              
                  
                OracleCommand cmd = new OracleCommand("FIX_MutualFund_CAP_BONUS",conn);
                //cmd.Connection = conn;
                //cmd.CommandText = "FIX_MutualFund_CAP_BONUS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("Given_ISIN", "varchar2").Value = isin;
                conn.Open();    
                cmd.ExecuteNonQuery();
                conn.Close();



                #region In_LINQ_Command
                //Entities db = new Entities(Session["Connection"] as EntityConnection);             

                //    var Update= (from cap in db.CORPORATEACTIONs
                //                join car in db.CORPORATEACTIONRECEIVABLEs 
                //                on new {cap.ISIN,cap.RECORDDATE,cap.INVESTORREF,cap.BOID}
                //                equals new { car.ISIN, car.RECORDDATE, INVESTORREF = car.INVESTORACREF, BOID = car.BOID } //for type casting
                //                where cap.ISIN == isin && cap.CATYPE == "BONUS"
                //                select new {
                //                 FREEBALANCE = cap.FREEBALANCE,
                //                 EFFECTIVEDATE = cap.EFFECTIVEDATE,
                //                 CAR_REFERENCE = car.REFERENCE
                        
                //                }).ToList();

                //    foreach (var item in Update)
                //    {
                //        CORPORATEACTIONRECEIVABLE model =new CORPORATEACTIONRECEIVABLE();
                //        model = db.CORPORATEACTIONRECEIVABLEs.Where(t => t.REFERENCE == item.CAR_REFERENCE).SingleOrDefault();
                //        model.ENTITLEMENT = item.FREEBALANCE;
                //        model.EFFECTIVEDATE = item.EFFECTIVEDATE;

                //        db.Entry(model).State = EntityState.Modified;  
            
                //    }
           
                //    db.SaveChanges();
                #endregion

                result = "1";
            
            }
            catch(Exception ex)
            {
              result= ex.Message;
            }
               return Json(new { result},JsonRequestBehavior.AllowGet);
        }
    }
}
