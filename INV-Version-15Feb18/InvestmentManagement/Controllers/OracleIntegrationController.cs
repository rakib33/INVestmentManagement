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
using InvestmentManagement.App_Code;

using System.Data.OracleClient;
using System.Data.Entity.Validation;

namespace InvestmentManagement.Controllers
{
    public class OracleIntegrationController : Controller
    {
         CommonFunction oCommonFunction = new CommonFunction();

        public ActionResult ListOracleIntegration(string sortdir, DateTime? FromDate, DateTime? ToDate, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //Session["UserId"] = null;
                //Session["Connection"] = null;
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }


                List<XX_INVEST_GL_INTEGRATION_DATA> models = new List<XX_INVEST_GL_INTEGRATION_DATA>();
                GridModel<XX_INVEST_GL_INTEGRATION_DATA> gridModels = new GridModel<XX_INVEST_GL_INTEGRATION_DATA>();


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


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).XX_INVEST_GL_INTEGRATION_DATA.AsNoTracking().Where(t=>t.TRANSACTIONDETAILID=="1").OrderBy(t=>t.REF_NUMBER).ThenBy(t=>t.TRANSACTIONMASTERID).ThenBy(t=>t.TRANSACTIONDETAILID).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).XX_INVEST_GL_INTEGRATION_DATA.AsNoTracking().Where(w =>w.TRANSACTIONDETAILID =="1" && w.ACCOUNTCODE.Contains(filterstring)).OrderBy(t => t.REF_NUMBER).ThenBy(t => t.TRANSACTIONMASTERID).ThenBy(t => t.TRANSACTIONDETAILID).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();

                if (models != null && FromDate.HasValue)
                    models = models.Where(t => t.TRANSACTIONDATE >= FromDate).ToList();

                if (models != null && ToDate.HasValue)
                    models = models.Where(t => t.TRANSACTIONDATE <= ToDate).ToList();

                gridModels.DataModel = models;

                //oJournalHeadViewModel.JournalHeads = gridModels.DataModel;
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
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

                if (Convert.ToString(TempData["result"]) != null)
                {

                    ViewBag.Message = Convert.ToString(TempData["result"]);
                }

            
                return PartialView("ListOracleIntegration", gridModels);

            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }



        public ActionResult IntegrationOperation(string Code,string id) //code = FdrNumberUpdate,fixed deposit id
        {
           if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

           FIXEDDEPOSIT oFixedDeposit = new FIXEDDEPOSIT();
           XX_INVEST_GL_INTEGRATION_DATA EbsPostData = new XX_INVEST_GL_INTEGRATION_DATA();
           try
           {
               using (Entities db = new Entities(Session["Connection"] as EntityConnection))
               {

                   oFixedDeposit = db.FIXEDDEPOSITs.Where(t => t.REFERENCE == id).SingleOrDefault();

                  // var NAMCC = db.NOMINALACCOUNTs.Where(t=>t.CODE== ApplicationVariable.POSTGL_FDR_NUMBERUPDATE_CODE).SingleOrDefault();  //1003


                   ViewBag.code = ApplicationVariable.POSTGL_FROM_FIXEDDEPOSIT;
                   ViewBag.fDepositId = oFixedDeposit.REFERENCE;


                   ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                   ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                   ViewBag.Header = "Add " + Session["currentPage"];


                   if (Code == ApplicationVariable.POSTGL_FROM_NEWFDR)
                   {
                       var NAMCC = db.NOMINALACCOUNTs.Where(t => t.CODE == ApplicationVariable.POSTGL_FDR_NUMBERUPDATE_CODE).SingleOrDefault();  //1003
                       EbsPostData.TRANSACTIONTYPE = NAMCC.DESCRIPTION;
                       return PartialView("AddVoucherNumber", EbsPostData);
                                              
                   }
                   else if (Code == ApplicationVariable.POSTGL_FROM_FIXEDDEPOSIT)
                   {
                       //check how many code are in PostedMatrix

                       ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                       ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                       ViewBag.Header = "Add " + Session["currentPage"];
              

                       EbsPostData.TRANSACTIONTYPE = ApplicationVariable.POSTGL_FROM_FIXEDDEPOSIT;
                       EbsPostData.OU_CODE = "0001";
                       EbsPostData.UNITAGENCYCODE = "000000";
                       EbsPostData.COSTCENTERCODE = "1022";
                       //throw an add page here 

                       //ViewBag.code = ApplicationVariable.POSTGL_FROM_FIXEDDEPOSIT;
                       //ViewBag.fDepositId = oFixedDeposit.REFERENCE;
                       return PartialView("AddOracleIntegration", EbsPostData);
                      

                   }
                   else { 
                   
                   
                   }
                   

                   db.Entry(oFixedDeposit).State = EntityState.Modified;

                   db.SaveChanges();
               }
             
           }
           catch (Exception ex)
           {
               string message = ex.Message;
               TempData["result"] = message;
              //return RedirectToAction("Index", "ErrorPage", new { message });
           }
           return RedirectToAction("ListOracleIntegration");
        }


        [HttpPost]
        public ActionResult AddVoucherNumber(string Code,string FDepositId, XX_INVEST_GL_INTEGRATION_DATA model)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            Entities db = new Entities(Session["Connection"] as EntityConnection);

            FIXEDDEPOSIT oFixedDeposit = new FIXEDDEPOSIT();
            XX_INVEST_GL_INTEGRATION_DATA EbsPostData = new XX_INVEST_GL_INTEGRATION_DATA();
           
            try{

            oFixedDeposit = db.FIXEDDEPOSITs.Where(t => t.REFERENCE == FDepositId).SingleOrDefault();

          //  var NAMCC = db.NOMINALACCOUNTs.Where(t => t.CODE == ApplicationVariable.POSTGL_FDR_NUMBERUPDATE_CODE).SingleOrDefault();  //1003

            EbsPostData.REFERENCE = Guid.NewGuid().ToString();
            EbsPostData.CREATEDDATE = DateTime.Now;
            EbsPostData.CREATEDBY = Session["UserId"].ToString();

            EbsPostData.TRANSACTIONTYPE = model.TRANSACTIONTYPE;
            EbsPostData.TRANSACTIONMASTERID = model.TRANSACTIONMASTERID; //Voucher Number                              //getMaxMasterID(NAMCC.DESCRIPTION);
            EbsPostData.TRANSACTIONDETAILID = "1";

            EbsPostData.TRANSACTIONDATE = model.TRANSACTIONDATE ;

            EbsPostData.OU_CODE = "0001";
            EbsPostData.UNITAGENCYCODE = "000000";
            EbsPostData.REF_NUMBER = oFixedDeposit.DEPOSITNUMBER;
            EbsPostData.SOURCE_ID = ApplicationVariable.POSTGL_SOURCE_ID;

            EbsPostData.STATUS = ConstantVariable.STATUS_PENDING;

            EbsPostData.FIXEDDEPOSIT_REF = oFixedDeposit.REFERENCE;
            db.XX_INVEST_GL_INTEGRATION_DATA.Add(EbsPostData);

            oFixedDeposit.POSTGL_STATUS = ConstantVariable.STATUS_PENDING;
            oFixedDeposit.POSTGL_FROM = ApplicationVariable.POSTGL_FROM_NEWFDR;

            db.SaveChanges();
            TempData["result"] = "Successfully Saved.";
          }
            catch (Exception ex)
            {
                string message = ex.Message;
                TempData["result"] = message;              
            }
            //return RedirectToAction("ListOracleIntegration");
            return RedirectToAction("ListFixedDeposit", "FixedDeposit", new { lblbreadcum="New FDR"});
        }

        [HttpPost]
        public ActionResult AddOracleIntegration(string Code, string FDepositId, XX_INVEST_GL_INTEGRATION_DATA model) //code = FdrNumberUpdate,fixed deposit id
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            FIXEDDEPOSIT oFixedDeposit = new FIXEDDEPOSIT();
          
          EBSIntegrationViewModel newModel = new EBSIntegrationViewModel();
          TRANSACTIONPOSTINGMATRIX oPostMatrix = new TRANSACTIONPOSTINGMATRIX();

          List<XX_INVEST_GL_INTEGRATION_DATA> IntegrationModel = new List<XX_INVEST_GL_INTEGRATION_DATA>();

          try
          {
              using (Entities db = new Entities(Session["Connection"] as EntityConnection))
              {
                  //var NAMCC = db.NOMINALACCOUNTs.Where(t => t.CODE == ApplicationVariable.POSTGL_FDR_NUMBERUPDATE_CODE).SingleOrDefault();  //1003

                  oFixedDeposit = db.FIXEDDEPOSITs.Where(t => t.REFERENCE == FDepositId).SingleOrDefault();

                  // var newModel = db.TRANSACTIONPOSTINGMATRIces.Where(t => t.TRANSACTIONCODE == Code).ToList();

                  oPostMatrix = db.TRANSACTIONPOSTINGMATRIces.Where(t => t.TRANSACTIONCODE == Code).SingleOrDefault();

                  //GetTransactionMatrix(Code); //FDRIP
                  string Message = "";
                  decimal MaxMasterId = getMaxMasterID(Code);

                  decimal DebitAmount = 0;
                  decimal CreditAmount = 0;

                  int detailsId = 1;

                
                  if (oPostMatrix != null)
                  {

                    

                      if (oPostMatrix.BSDEBITCONTROL != null && !string.IsNullOrEmpty(oPostMatrix.BSDEBITCONTROL))
                      {
                          var NominalAcc = db.NOMINALACCOUNTs.Where(t => t.CODE == oPostMatrix.BSDEBITCONTROL).SingleOrDefault();

                          
                          //from proceduer
                          XX_INVEST_GL_INTEGRATION_DATA EbsPostData = new XX_INVEST_GL_INTEGRATION_DATA();
                          EbsPostData.REFERENCE = Guid.NewGuid().ToString();
                          EbsPostData.CREATEDDATE = DateTime.Now;
                          EbsPostData.CREATEDBY = Session["UserId"].ToString();
                          EbsPostData.STATUS = ConstantVariable.STATUS_PENDING;

                          ///////

                          EbsPostData.TRANSACTIONTYPE = oPostMatrix.DESCRIPTION;

                          EbsPostData.TRANSACTIONMASTERID = oFixedDeposit.REFERENCE;
                          EbsPostData.TRANSACTIONDETAILID = detailsId.ToString();
                          EbsPostData.TRANSACTIONDATE = model.TRANSACTIONDATE;

                          EbsPostData.OU_CODE = model.OU_CODE;
                          EbsPostData.UNITAGENCYCODE = model.UNITAGENCYCODE;
                          EbsPostData.COSTCENTERCODE = model.COSTCENTERCODE;

                          EbsPostData.REF_NUMBER = oFixedDeposit.DEPOSITNUMBER;

                          EbsPostData.BANKACCOUNTNUMBER = model.BANKACCOUNTNUMBER;
                          EbsPostData.SOURCE_ID = ApplicationVariable.POSTGL_SOURCE_ID;
                          EbsPostData.DESCRIPTION = model.DESCRIPTION;
                          EbsPostData.TRANSACTIONMASTERID = Convert.ToString(MaxMasterId);
                          EbsPostData.FIXEDDEPOSIT_REF = oFixedDeposit.REFERENCE;

                          EbsPostData.ACCOUNTCODE = NominalAcc.ORACLENOMINALCODE;
                          EbsPostData.ACCOUNTNAME = NominalAcc.ORACLENOMINALNAME;
                          EbsPostData.ACCOUNTEDDR = oFixedDeposit.ACTUALINTERESTRECEIVED;

                     

                          db.XX_INVEST_GL_INTEGRATION_DATA.Add(EbsPostData);

                          //for checking
                          DebitAmount += EbsPostData.ACCOUNTEDDR.Value;

                          detailsId += 1;
                      }
                      if (oPostMatrix.PLCREDITCONTROL != null && !string.IsNullOrEmpty(oPostMatrix.PLCREDITCONTROL))
                      {
                          var NominalAcc2 = db.NOMINALACCOUNTs.Where(t => t.CODE == oPostMatrix.PLCREDITCONTROL).SingleOrDefault();

                          XX_INVEST_GL_INTEGRATION_DATA EbsPostData = new XX_INVEST_GL_INTEGRATION_DATA();
                          EbsPostData.REFERENCE = Guid.NewGuid().ToString();

                          EbsPostData.CREATEDDATE = DateTime.Now;
                          EbsPostData.CREATEDBY = Session["UserId"].ToString();
                          EbsPostData.STATUS = ConstantVariable.STATUS_PENDING;

                          ///////

                          EbsPostData.TRANSACTIONTYPE = oPostMatrix.DESCRIPTION;

                          EbsPostData.TRANSACTIONMASTERID = oFixedDeposit.REFERENCE;
                          EbsPostData.TRANSACTIONDETAILID = detailsId.ToString();
                          EbsPostData.TRANSACTIONDATE = model.TRANSACTIONDATE;
                          //from proceduer

                          EbsPostData.ACCOUNTCODE = NominalAcc2.ORACLENOMINALCODE;
                          EbsPostData.ACCOUNTNAME = NominalAcc2.ORACLENOMINALNAME;

                          EbsPostData.REF_NUMBER = oFixedDeposit.DEPOSITNUMBER;


                          EbsPostData.BANKACCOUNTNUMBER = model.BANKACCOUNTNUMBER;
                          EbsPostData.SOURCE_ID = ApplicationVariable.POSTGL_SOURCE_ID;
                          EbsPostData.DESCRIPTION = model.DESCRIPTION;

                          EbsPostData.FIXEDDEPOSIT_REF = oFixedDeposit.REFERENCE;

                          EbsPostData.ACCOUNTEDCR = oFixedDeposit.ACTUALINTERESTRECEIVED;

                          db.XX_INVEST_GL_INTEGRATION_DATA.Add(EbsPostData);

                          CreditAmount += EbsPostData.ACCOUNTEDCR.Value;

                      }

                      else
                      {
                          Message = "Debit or Credit Control must have a value.";
                          //return
                      }

                      oFixedDeposit.POSTGL_STATUS = ConstantVariable.STATUS_APPROVED;

                      //now check is the debit and credit amount is same

                      if ((DebitAmount != 0 && CreditAmount != 0) && (DebitAmount == CreditAmount) && string.IsNullOrEmpty(Message))
                      {
                          db.SaveChanges();
                          TempData["result"] = "Successfully Saved.";
                      }
                      else
                      {
                          TempData["result"] = "Debit and Credit Amount may not match. " + Message;
                      }

                  } //End IF
                  else
                  {
                      TempData["result"] = "Posting Matrix Must have a value.";
                  }

              }
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
              TempData["result"] = message;
          }

            return RedirectToAction("ListFixedDepositRegister", "FixedDepositRegister");
        }



        public ActionResult View(string transaction, string fixedDeposit_Ref)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            Entities db = new Entities(Session["Connection"] as EntityConnection);
            List<XX_INVEST_GL_INTEGRATION_DATA> model = new List<XX_INVEST_GL_INTEGRATION_DATA>();

            try
            {
                model = db.XX_INVEST_GL_INTEGRATION_DATA.Where(t => t.TRANSACTIONTYPE == transaction && t.FIXEDDEPOSIT_REF == fixedDeposit_Ref).ToList();
                var TakeFirst = model.FirstOrDefault();                
                ViewBag.MasterInfo = TakeFirst;

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetDetailsAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "View " + Session["currentPage"];

                return PartialView(model);
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                TempData["result"] = message;
            }

            return RedirectToAction("ListOracleIntegration");
        }


        #region Helper_Method
        public List<EBSIntegrationViewModel> GetTransactionMatrix(string tranCode)
        {
            try
            {
                List<EBSIntegrationViewModel> ebsmodel = new List<EBSIntegrationViewModel>();
                DataTable dt = new DataTable();

                string con = System.Configuration.ConfigurationManager.
                ConnectionStrings["ConnectionString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(con))
                {
                    OracleDataAdapter da = new OracleDataAdapter();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "GETPOSTINGMATRIX";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("TranCode", OracleType.VarChar).Value = Convert.ToString(tranCode);
                    cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;

                    da.SelectCommand = cmd;
                  
                    da.Fill(dt);
                }
                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow dR in new DataView(dt).ToTable(true).Rows)
                    {
                        ebsmodel.Add(new EBSIntegrationViewModel {
                            TRANSACTIONCODE = dR["TRANSACTIONCODE"].ToString(),
                            TRANSACTIONTYPE = dR["TRANSACTIONTYPE"].ToString(),
                            BSCREDITCONTROL = dR["BSCREDITCONTROL"].ToString(),
                            BSDEBITCONTROL = dR["BSDEBITCONTROL"].ToString(),

                            PLDEBITCONTROL = dR["PLDEBITCONTROL"].ToString(),
                            PLCREDITCONTROL = dR["PLCREDITCONTROL"].ToString(),

                            BANKDEBITCONTROL = dR["BANKDEBITCONTROL"].ToString(),
                            BANKCREDITCONTROL = dR["BANKCREDITCONTROL"].ToString(),                            

                            CODE = dR["CODE"].ToString(),
                            DESCRIPTION = dR["DESCRIPTION"].ToString(),
                            ORACLENOMINALCODE = dR["ORACLENOMINALCODE"].ToString(),
                            ORACLENOMINALNAME = dR["ORACLENOMINALNAME"].ToString(),
                            STATUS = dR["STATUS"].ToString(),                           
                            ACCOUNTTYPE = dR["ACCOUNTTYPE"].ToString()
                        
                        });
                    }

                    //jView.Add();
                    // Marketrate = Convert.ToDouble(dtmarketPrice.Rows[0]["CLOSINGPRICE"].ToString());                   
                }

                return ebsmodel;

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }


        private decimal getMaxMasterID(string TransactionType)
        {

            decimal MaxRow=0;

            string con = System.Configuration.ConfigurationManager.
             ConnectionStrings["ConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(con))
            {
                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GETMAXPOSTINGMATRIX";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("TranType", OracleType.VarChar).Value = TransactionType;
                cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;

                da.SelectCommand = cmd;
               
                da.Fill(dt);

               // return dt;
            }
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dR in new DataView(dt).ToTable(true).Rows)
                {
                   if (! DBNull.Value.Equals(dR["TRANSACTIONMASTERID"])) 
                   {
                      //not null
                       MaxRow = Convert.ToDecimal(dR["TRANSACTIONMASTERID"]);
                   }

                   MaxRow += 1;
                   
                }

                //jView.Add();
                // Marketrate = Convert.ToDouble(dtmarketPrice.Rows[0]["CLOSINGPRICE"].ToString());                   
            }

            return MaxRow;
        }
        
        #endregion

    }


}
