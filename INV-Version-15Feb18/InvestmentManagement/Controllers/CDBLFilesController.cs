using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using InvestmentManagement.InvestmentManagement.Models;
using System.IO;
using System.Xml.Linq;
using System.Data;
using System.Text;
using System.Data.EntityClient;
using InvestmentManagement.App_Code;
using System.Data.OracleClient;

namespace InvestmentManagement.Controllers
{
    public class CDBLFilesController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
        
        

        public ActionResult Index()
        {
            return View();
        }
                
        [HttpGet]
        public ActionResult Upload()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            List<DEPOSITORYSETTING> oDEPOSITORYSETTINGs = new List<DEPOSITORYSETTING>();
            using (Entities db = new Entities(Session["Connection"] as EntityConnection))
            {
                oDEPOSITORYSETTINGs = db.DEPOSITORYSETTINGS.OrderBy(t=>t.SEQ).ToList();
            }

            ViewBag.DepositorySettingList = oDEPOSITORYSETTINGs;
            return PartialView();
        }

        [HttpPost]
        public ActionResult ImportCorporateAction()
         {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }

            int i = 0;
            string line;
            DateTime RecordDate =DateTime.Now;  //previous name ImportDate
            Entities db = new Entities(Session["Connection"] as EntityConnection);
            foreach (var item in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[i];
                var fileName = Path.GetFileName(file.FileName);
                
                string dateString = System.IO.Path.GetFileNameWithoutExtension(fileName).Substring(0, 8);
                //List<string> ISINList = new List<string>();

                List<StockISIN> ISINList = new List<StockISIN>();

                List<CORPORATEACTION> oCORPORATEACTIONs = new List<CORPORATEACTION>();
                try
                {
                    if (fileName == "17DP70UX.TXT")
                    {
                        StreamReader oCorporateActions = new StreamReader(file.InputStream);                        
                        
                        while ((line = oCorporateActions.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');
                            CORPORATEACTION oCorporateAction = new CORPORATEACTION();

                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;
                            oCorporateAction.REFERENCE = Guid.NewGuid().ToString();
                            oCorporateAction.CREATEDBY = Session["UserId"].ToString();
                            oCorporateAction.CATYPE = records[3] == null ? string.Empty : records[3];                           
                            oCorporateAction.RECORDDATE = records[4] == null ? DateTime.Now : DateTime.Parse(records[4]);       
                     
                            RecordDate = records[4] == null ? DateTime.Now : DateTime.Parse(records[4]);                                                     //

                            oCorporateAction.EFFECTIVEDATE = records[5] == null ? DateTime.Now : DateTime.Parse(records[5]);                           
                            oCorporateAction.ISIN = records[8] == null ? string.Empty : records[8];
                            oCorporateAction.INSTRUMENTNAME = records[9] == null ? string.Empty : records[9];
                            oCorporateAction.BOID = records[10] == null ? string.Empty : records[10];
                            //
                           // ISINList.Add(oCorporateAction.ISIN);

                            ISINList.Add(new StockISIN { ISIN= oCorporateAction.ISIN, BOID=oCorporateAction.BOID,RecordDate=oCorporateAction.RECORDDATE.Value });

                            INVESTOR oInvestor = new INVESTOR();

                            try
                            {
                                oInvestor = db.INVESTORs.Where(investor => investor.BONUMBER == oCorporateAction.BOID).FirstOrDefault();
                                if (oInvestor == null)
                                {
                                    oCorporateAction.ISEXCEPTION = "Y";
                                    oCorporateAction.EXCEPTION = "Investor Not Found!";
                                    return new JsonResult { Data = "Investor not found of BOID:" + oCorporateAction.BOID };  //Date 20-09-17
                                }
                               
                            }
                            catch (Exception ex)
                            {         
                                return new JsonResult { Data = "Investor not found of BOID:" + oCorporateAction.BOID + ". Error :" + ex.Message }; //Date 20-09-17
                            }
                          
                            oCorporateAction.PROCFLAG = records[12] == null ? string.Empty : records[12];
                            oCorporateAction.DRCRFLAG = records[13] == null ? string.Empty : records[13];
                            oCorporateAction.ISCREDIT = records[13] == null ? string.Empty : Convert.ToString(records[13].ToLower() == "CREDIT".ToLower() ? 'Y' : 'N');
                            oCorporateAction.FREEBALANCE = int.Parse(records[14].Split('.')[0]);
                            oCorporateAction.LOCKINBALANCE = int.Parse(records[15].Split('.')[0]);
                            oCorporateAction.FREEZEBALANCE = int.Parse(records[16].Split('.')[0]);

                            oCorporateAction.INVESTORREF = oInvestor.ACCOUNTNUMBER; // ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //Date 20-09-17

                            oCorporateAction.STATUS = ConstantVariable.STATUS_PENDING;               
                            oCorporateAction.IMPORTDATE = DateTime.Today;                            
                            oCorporateAction.LASTUPDATED = DateTime.Today;

                            if (oCorporateAction.CATYPE == ConstantVariable.RightShare)
                            {
                                try
                                {
                                    var list = db.RIGHTSHAREDECLARATIONs.Where(t => t.ISIN == oCorporateAction.ISIN && t.ENTRYDATE == oCorporateAction.EFFECTIVEDATE).OrderByDescending(t => t.ENTRYDATE).FirstOrDefault();
                                    oCorporateAction.PREMIUM = list.BUYRATE;
                                }
                                catch (Exception ex)
                                {
                                    return new JsonResult { Data = "Right Share Declaration not found of ISIN:" + oCorporateAction.ISIN + " and BUYRATE"+oCorporateAction.PREMIUM +" Error :" + ex.Message };            
                                }
                            }
                            oCommonFunction.CustomObjectNullValidation<CORPORATEACTION>(ref oCorporateAction);
                            oCORPORATEACTIONs.Add(oCorporateAction);                        
                         }                        
                    }
                    else
                    {
                    throw new Exception("Select 17DP70UX-Corporate Action Processing.txt File");                    
                    }
                    List<CORPORATEACTION> CAList = new List<CORPORATEACTION>();

                    CAList = (from old_ca in ISINList.ToList()
                              join ca in db.CORPORATEACTIONs.ToList() on  old_ca.ISIN equals ca.ISIN 
                              where ca.RECORDDATE ==  old_ca.RecordDate         // RecordDate
                              select ca).ToList();

                 
                    foreach (var ca in CAList)
                    {
                        try
                        {
                            db.CORPORATEACTIONs.Remove(ca);
                            db.SaveChanges();
                        }
                        catch (Exception)
                        { 
                        
                        }
                    }
                   

                    foreach (var ca in oCORPORATEACTIONs)
                    {
                        db.CORPORATEACTIONs.Add(ca);
                    }
                  
                    db.SaveChanges();
                  
                }
                catch (Exception ex)
                {
                    //Exception should be catch(DbEntityValidationException dbEx)
                    //foreach (var validationErrors in dbEx.EntityValidationErrors)
                    //{
                    //    foreach (var validationError in validationErrors.ValidationErrors)
                    //    {
                    //        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    //    }
                    //}
                    return new JsonResult { Data = ex.Message };
                }
            }

            return new JsonResult { Data = "Upload Successful" };
        }




        public ActionResult ImportCorporateActionReceivable()   
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };
            }

            int i = 0;
            string line;
            DateTime RecordDate = DateTime.Today;  //previous name ImportDate
            Entities db = new Entities(Session["Connection"] as EntityConnection);
            try
            {
               // List<string> ISINList = new List<string>();

                List<StockISIN> ISINList= new List<StockISIN>();

                List<CORPORATEACTIONRECEIVABLE> oCORPORATEACTIONRECEIVABLEs = new List<CORPORATEACTIONRECEIVABLE>();
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader oCorporateActionReceivables = new StreamReader(file.InputStream);
                    
                    if (fileName == "17DP64UX.TXT")
                    {
                        while ((line = oCorporateActionReceivables.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');
                            CORPORATEACTIONRECEIVABLE oCorporateActionReceivable = new CORPORATEACTIONRECEIVABLE();

                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;
                            oCorporateActionReceivable.REFERENCE = Guid.NewGuid().ToString();
                            oCorporateActionReceivable.CREATEDBY = Session["UserId"].ToString();
                            oCorporateActionReceivable.ISIN = records[0] == null ? string.Empty : records[0];
                            oCorporateActionReceivable.INSTRUMENTNAME = records[1] == null ? string.Empty : records[1];
                            oCorporateActionReceivable.SEQUENCENO = records[2] == null ? string.Empty : records[2];
                            oCorporateActionReceivable.CATYPE = records[3] == null ? string.Empty : records[3];
                            
                            //Record Date is the date the file given by CDBL for Corporate Action Process
                            oCorporateActionReceivable.RECORDDATE = records[4] == null ? DateTime.Now : DateTime.Parse(records[4]);
                            RecordDate = records[4] == null ? DateTime.Now : DateTime.Parse(records[4]); //Added 29/12/16         
                            //
                            //ISINList.Add(oCorporateActionReceivable.ISIN);
                            ISINList.Add(new StockISIN { ISIN= oCorporateActionReceivable.ISIN, BOID=oCorporateActionReceivable.BOID });

                            oCorporateActionReceivable.EFFECTIVEDATE = records[5] == null ? DateTime.Now : DateTime.Parse(records[5]);

                            oCorporateActionReceivable.CASHFRACAMOUNT =records[6] == null? (decimal)0 : Convert.ToDecimal(records[6]);
                            oCorporateActionReceivable.PARRATION = records[11] == null ? (decimal)0 : Convert.ToDecimal(records[11]);
                            oCorporateActionReceivable.NUMRATION = records[12] == null ? (decimal)0 : Convert.ToDecimal(records[12]);
                            oCorporateActionReceivable.BOID = records[13] == null ? string.Empty : records[13];
                           
                            INVESTOR oInvestor = new INVESTOR();

                            try
                            {
                                oInvestor = db.INVESTORs.Where(Investor => Investor.BONUMBER == oCorporateActionReceivable.BOID).FirstOrDefault();
                                if (oInvestor == null)
                                {
                                    oCorporateActionReceivable.ISEXCEPTION = "Y";
                                    oCorporateActionReceivable.EXCEPTIONDETAIL = "Investor Not Found!";
                                    return new JsonResult { Data = "Investor not found of BOID:" + oCorporateActionReceivable.BOID };
                                }
                            }                            
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Investor not found of BOID:" + oCorporateActionReceivable.BOID + ". Error :" + ex.Message };
                            }
                          
                            oCorporateActionReceivable.INVESTORNAME = records[14] == null ? string.Empty : records[14];
                            oCorporateActionReceivable.PARHOLDING = records[15] == null ? (decimal)0 : Convert.ToDecimal(records[15]);
                            oCorporateActionReceivable.ENTITLEMENT = records[16] == null ? (decimal)0 : Convert.ToDecimal(records[16]);
                            oCorporateActionReceivable.DRCRFLAG = records[17] == null ? string.Empty : records[17];
                            oCorporateActionReceivable.STATUS = ConstantVariable.STATUS_PENDING; 
                            oCorporateActionReceivable.IMPORTDATE = DateTime.Today;
                            oCorporateActionReceivable.LASTUPDATED = DateTime.Today;

                            oCorporateActionReceivable.INVESTORACREF = oInvestor.ACCOUNTNUMBER;  //DeltaLife Account Number 00001

                            oCommonFunction.CustomObjectNullValidation<CORPORATEACTIONRECEIVABLE>(ref oCorporateActionReceivable);
                            oCORPORATEACTIONRECEIVABLEs.Add(oCorporateActionReceivable);
                        }                        
                    }
                    else
                    {                       
                        throw new Exception(" Invalid File! Please Select 17DP64UX-Corporate Action Receivables ");
                    }
                    List<CORPORATEACTIONRECEIVABLE> CARList = new List<CORPORATEACTIONRECEIVABLE>();

                    //CARList = db.CORPORATEACTIONRECEIVABLEs.Where(ca => ca.RECORDDATE == RecordDate).ToList();

                    CARList =                      
                        (from old_ca in ISINList.ToList()
                               join ca in db.CORPORATEACTIONRECEIVABLEs.ToList() on old_ca.ISIN equals  ca.ISIN
                               where ca.RECORDDATE == RecordDate
                               select ca).ToList();
                    
                    foreach (var car in CARList)
                    {
                        try
                        {
                            db.CORPORATEACTIONRECEIVABLEs.Remove(car);
                            db.SaveChanges();
                        }
                        catch(Exception){
                        }
                    }
                    
                   
                    foreach (var ca in oCORPORATEACTIONRECEIVABLEs)
                    {
                        db.CORPORATEACTIONRECEIVABLEs.Add(ca);
                    }
                    db.SaveChanges();
                  
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }


            return new JsonResult { Data = "Upload Successful" };
        }
        


        public ActionResult ImportSettlements()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };
            }

            int i = 0;
            string line;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {

                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader oSettlementData = new StreamReader(file.InputStream);
                    INSTRUMENT oInstrument = new INSTRUMENT();
                    line = oSettlementData.ReadLine();
                    string[] brokerage = line.Split('~');

                    if (brokerage[0] == "01")
                    {
                        while ((line = oSettlementData.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');
                            if (records[0] == "01")
                                continue;
                            if (records[0] == "02")
                            {
                                oInstrument.ISIN = records[1];
                                oInstrument.NAME = records[2];
                            }
                            else if (records[0] == "04")
                            {
                                string[] settlementRecords = records[2].Split(' ');
                                if (settlementRecords[0] == "Transfer")
                                {
                                    SETTLEMENT oSettlement = new SETTLEMENT();
                                    oSettlement.BOID = settlementRecords[4]; // Assign Investor BOID

                                    string tranDateString = records[6];
                                    DateTime tranDate = new DateTime(int.Parse(tranDateString.Substring(4, 4)), int.Parse(tranDateString.Substring(2, 2)), int.Parse(tranDateString.Substring(0, 2)), 0, 0, 0);

                                    oSettlement.TRANDATE = tranDate;
                                    if (settlementRecords[2] == "CR")
                                    {
                                        oSettlement.QUANTITY = int.Parse(records[8].Replace(",", ""));
                                        oSettlement.TRANSACTIONTYPE = "PayOut";
                                    }
                                    else if (settlementRecords[2] == "DR")
                                    {
                                        //Modified by Asif. 17 Jun 2010
                                        oSettlement.QUANTITY = int.Parse(records[9].Replace(",", ""));
                                        oSettlement.TRANSACTIONTYPE = "PayIn";
                                    }
                                    oSettlement.REFERENCE = Guid.NewGuid().ToString();
                                    oSettlement.CREATEDBY = Session["UserId"].ToString();
                                    oSettlement.STATUS = "Pending";
                                    oSettlement.INSTRUMENTREF = oInstrument.NAME;
                                    oSettlement.ISIN = oInstrument.ISIN;
                                    oSettlement.LASTUPDATED = DateTime.Today;
                                    oSettlement.COUNTERBOID = brokerage[1];
                                    oCommonFunction.CustomObjectNullValidation<SETTLEMENT>(ref oSettlement);
                                    db.SETTLEMENTs.Add(oSettlement);
                                }
                            }
                        }
                        db.SaveChanges();
                    }


                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }


            return new JsonResult { Data = "Upload Successful" };
        }
        
        public ActionResult ImportDemats()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }
            int i = 0;
            string line;
            DateTime importDate = DateTime.Today;
            Entities db = new Entities(Session["Connection"] as EntityConnection);
            foreach (var item in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[i];
                var fileName = Path.GetFileName(file.FileName);
                try
                {
                    List<string> ISINList = new List<string>();
                    if (fileName == "16DP61UX.TXT")
                    {
                        StreamReader chargeFileStream = new StreamReader(file.InputStream);
                        List<DEMATCONFIRM> oDEMATCONFIRMs = new List<DEMATCONFIRM>();

                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');
                            DEMATCONFIRM oDemat = new DEMATCONFIRM();

                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;

                            oDemat.REFERENCE = Guid.NewGuid().ToString();
                            oDemat.CREATEDBY = Session["UserId"].ToString();
                            oDemat.DRN = records[0];
                            oDemat.DRF = records[1];
                            oDemat.ISIN = records[2];
                            oDemat.INSTRUMENTNAME = records[3];
                            oDemat.BOID = records[4];

                            ISINList.Add(oDemat.ISIN);

                            INVESTOR oInvestor = new INVESTOR();

                            try
                            {
                                oInvestor = db.INVESTORs.Where(Investor => Investor.BONUMBER == oDemat.BOID).FirstOrDefault();
                                if (oInvestor == null)
                                {
                                    oDemat.ISEXCEPTION = "Y";
                                    oDemat.EXCEPTIONDETAILS = "Investor Not Found!";
                                    return new JsonResult { Data = "Investor not found of BOID:" + oDemat.BOID  };
                                }
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Investor not found of BOID:" + oDemat.BOID + ". Error :" + ex.Message };
                            }
                            oDemat.INVESTORNAME = records[5];
                            oDemat.PROCESSEDSTATUS = records[6];
                            oDemat.REQUESTEDQTY = int.Parse(records[7].Trim().Split('.')[0]);
                            oDemat.ACCEPTEDQTY = int.Parse(records[8].Trim().Split('.')[0]);
                            oDemat.REJECTEDQTY = int.Parse(records[9].Trim().Split('.')[0]);
                            oDemat.BALANCETYPE = records[10];
                            oDemat.CERTNO = records[11];
                            oDemat.FOLIONO = records[12];
                            oDemat.CERTQTY = int.Parse(records[13].Trim().Split('.')[0]);
                            oDemat.CERTSTATUS = records[14];
                            oDemat.DNRSTART = records[15];
                            oDemat.DNREND = records[16];
                            oDemat.BUSINESSDATE = importDate;               //DateTime.Today;

                            oDemat.ISEXCEPTION = Convert.ToString('N');
                            oDemat.ISDEFAULTAVGCHANGE = Convert.ToString('N');
                            oDemat.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";

                            oDemat.INVESTORACREF = oInvestor.ACCOUNTNUMBER; //added 03-Jan-2017

                            oCommonFunction.CustomObjectNullValidation<DEMATCONFIRM>(ref oDemat);
                            oDEMATCONFIRMs.Add(oDemat);

                        }

                        List<DEMATCONFIRM> Dist = new List<DEMATCONFIRM>();

                        Dist =  (from old_ca in ISINList.ToList()
                                 join ca in db.DEMATCONFIRMs.ToList() on old_ca equals ca.ISIN
                                 where ca.BUSINESSDATE == importDate
                                 select ca).ToList();

                            //db.DEMATCONFIRMs.Where(dmtx => dmtx.BUSINESSDATE == importDate).ToList();

                        foreach (var d in Dist)
                        {
                            db.DEMATCONFIRMs.Remove(d);
                           
                        }
                        db.SaveChanges();
                       // Dist = db.DEMATCONFIRMs.Where(dmtx => dmtx.BUSINESSDATE == importDate).ToList();

                        foreach (var d in oDEMATCONFIRMs)
                        {
                            db.DEMATCONFIRMs.Add(d);
                        }

                        db.SaveChanges();

                    }
                    else
                    {
                        throw new Exception("Invalid File! Please Seclet 16DP61UX-Materilization & Dematerilization File");
                    }

                }
                catch (Exception ex)
                {
                    return new JsonResult { Data = ex.Message };
                }
            }
            return new JsonResult { Data = "Upload Successful" };
        }

        public ActionResult ImportHoldings()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
               // return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }
            DataTable oHoldings = new DataTable("Holding");

            //Column
            DataColumn InvestorName = new DataColumn("InvestorName");
            InvestorName.DataType = System.Type.GetType("System.String");
            oHoldings.Columns.Add(InvestorName);

            DataColumn Status = new DataColumn("Status");
            Status.DataType = System.Type.GetType("System.String");
            oHoldings.Columns.Add(Status);

            DataColumn InstrumentName = new DataColumn("InstrumentName");
            InstrumentName.DataType = System.Type.GetType("System.String");
            oHoldings.Columns.Add(InstrumentName);

            //System.DateTime
            DataColumn BusinessDate = new DataColumn("BusinessDate");
            BusinessDate.DataType = System.Type.GetType("System.DateTime");
            oHoldings.Columns.Add(BusinessDate);

            DataColumn Reference = new DataColumn("Reference");
            Reference.DataType = System.Type.GetType("System.String");
            Reference.Unique = true;
            oHoldings.Columns.Add(Reference);

            DataColumn colBOID = new DataColumn("BOID");
            colBOID.DataType = System.Type.GetType("System.String");
            oHoldings.Columns.Add(colBOID);

            DataColumn colACNumber = new DataColumn("InvestorACRef");
            colACNumber.DataType = System.Type.GetType("System.String");
            oHoldings.Columns.Add(colACNumber);

            DataColumn colISIN = new DataColumn("ISIN");
            colISIN.DataType = System.Type.GetType("System.String");
            oHoldings.Columns.Add(colISIN);

            DataColumn ExceptionDetails = new DataColumn("ExceptionDetails");
            ExceptionDetails.DataType = System.Type.GetType("System.String");
            oHoldings.Columns.Add(ExceptionDetails);

            DataColumn colMaturedBalance = new DataColumn("MaturedBalance");
            colMaturedBalance.DataType = System.Type.GetType("System.Double");
            oHoldings.Columns.Add(colMaturedBalance);

            DataColumn colNetBalance = new DataColumn("NetBalance");
            colNetBalance.DataType = System.Type.GetType("System.Double");
            oHoldings.Columns.Add(colNetBalance);

            DataColumn isLedgerFound = new DataColumn("IsLedgerFound");
            isLedgerFound.DataType = System.Type.GetType("System.Boolean");
            oHoldings.Columns.Add(isLedgerFound);

            DataColumn IsException = new DataColumn("IsException");
            IsException.DataType = System.Type.GetType("System.Boolean");
            oHoldings.Columns.Add(IsException);


            return new JsonResult { Data = "Upload Successful" };
        }

        public ActionResult ImportDailyCharge()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
               // return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };
            }
            int i = 0;
            string line;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader chargeFileStream = new StreamReader(file.InputStream);
                    while ((line = chargeFileStream.ReadLine()) != null)
                    {
                        string[] records = line.Split('~');
                        if (records.Length == 0)
                            continue;

                        if (records.Length > 0 && records[0].Contains("Version"))
                            continue;

                        DAILYCHARGE oDailyCharge = new DAILYCHARGE();
                        oDailyCharge.REFERENCE = Guid.NewGuid().ToString();
                        oDailyCharge.CREATEDBY = Session["UserId"].ToString();
                        oDailyCharge.TRANSDESCRIPTION = records[0];
                        oDailyCharge.NOOFTRANS = int.Parse(records[1]);
                        oDailyCharge.QUANTITY = Convert.ToDecimal(records[2]);
                        oDailyCharge.TRANSVALUE = Convert.ToDecimal(records[3]);
                        oDailyCharge.TOTALAMOUNT = Convert.ToDecimal(records[4]);
                        oDailyCharge.SERVICETAX = Convert.ToDecimal(records[5]);
                        oDailyCharge.TOTALCHARGE = Convert.ToDecimal(records[6]);
                        oDailyCharge.IMPORTDATE = DateTime.Today;
                        oDailyCharge.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";

                        oCommonFunction.CustomObjectNullValidation<DAILYCHARGE>(ref oDailyCharge);
                        db.DAILYCHARGEs.Add(oDailyCharge);

                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };
        }

        public ActionResult ImportDailyChargeDetails()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }
            int i = 0;
            string line;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader chargeFileStream = new StreamReader(file.InputStream);
                    while ((line = chargeFileStream.ReadLine()) != null)
                    {
                        string[] records = line.Split('~');


                        if (records.Length == 0)
                            continue;

                        if (records.Length > 0 && records[0].Contains("Version"))
                            continue;

                        DAILYCHARGEDETAIL oDailyChargeDetail = new DAILYCHARGEDETAIL();


                        //Get AccoutNumber
                        //DataView accountNumberView = DataService.Utility.RunQuery("Select accountNumber from Investor where BONUMBER ='" + oDailyChargeDetail.BOID + "'", new List<DataServiceParameter>());
                        //if (accountNumberView.Table.Rows.Count > 0)
                        //    oDailyChargeDetail.InvestorAcRef = accountNumberView[0]["accountNumber"].ToString();
                        oDailyChargeDetail.INVESTORACREF = Convert.ToString("");

                        oDailyChargeDetail.REFERENCE = Guid.NewGuid().ToString();
                        oDailyChargeDetail.CREATEDBY = Session["UserId"].ToString();
                        oDailyChargeDetail.TRANSDESCRIPTION = records[1];
                        oDailyChargeDetail.QUANTITY = int.Parse(records[2].Split('.')[0]);
                        oDailyChargeDetail.TOTALCHARGE = Convert.ToDecimal(records[3]);
                        oDailyChargeDetail.INVESTORNAME = records[4];
                        oDailyChargeDetail.TRANSREFNO = records[5];
                        oDailyChargeDetail.TRANSVALUE = Convert.ToDecimal(records[6]);
                        oDailyChargeDetail.SERVICETAX = Convert.ToDecimal(records[7]);
                        oDailyChargeDetail.INSTRUMENTNAME = records[8];
                        oDailyChargeDetail.CHARGERATEAMOUNT = records[9];
                        oDailyChargeDetail.TOTALAMOUNT = Convert.ToDecimal(records[10]);
                        oDailyChargeDetail.ISIN = records[13];
                        oDailyChargeDetail.LASTUPDATED = DateTime.Today;
                        oDailyChargeDetail.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";
                        oDailyChargeDetail.IMPORTDATE = DateTime.Today;

                        oCommonFunction.CustomObjectNullValidation<DAILYCHARGEDETAIL>(ref oDailyChargeDetail);
                        db.DAILYCHARGEDETAILs.Add(oDailyChargeDetail);

                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };
        }

        public ActionResult ImportIPOProcessing()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
               // return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }
            int i = 0;
            string line;
            DateTime RecordDate =DateTime.Now;
            decimal FaceValue = 0;
            decimal premium = 0;
            INSTRUMENT instrument=new INSTRUMENT();
            try
            {
                List<string> ISINList = new List<string>();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader IPOProcessingStream = new StreamReader(file.InputStream);

                    List<IPO> oIPOlist = new List<IPO>();

                    if (fileName == "16DP95UX.TXT")
                    {

                        while ((line = IPOProcessingStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');

                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;

                            IPO oIPO = new IPO();

                            oIPO.REFERENCE = Guid.NewGuid().ToString();
                            oIPO.CREATEDBY = Session["UserId"].ToString();
                            oIPO.ISIN = records[0];
                            oIPO.INSTRUMENTNAME = records[1];
                            
                            oIPO.SEQUENCENO = records[2];
                          
                            oIPO.EFFECTIVEDATE = DateTime.Parse(records[3]);
                            oIPO.BOID = records[4];
                            oIPO.INVESTORNAME = records[5];

                            oIPO.INVESTORACREF = ConstantVariable.INVESTOR_ACCOUNT_NUMBER;

                            oIPO.BOCATEGORY = records[6];
                            oIPO.BOACCOUNTSTATUS = records[7];
                            oIPO.CURRENTQTY = int.Parse(records[8].Trim().Split('.')[0]);
                            oIPO.LOCKINQTY = int.Parse(records[9].Trim().Split('.')[0]);
                            oIPO.PROCESSFLAG = records[10];

                            
                            ISINList.Add(oIPO.ISIN);

                            try
                            {
                                instrument = db.INSTRUMENTs.Where(t => t.ISIN == oIPO.ISIN).SingleOrDefault();
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Instrument not found of ISIN Number :" + oIPO.ISIN + ". Error :" + ex.Message };
                            }
                            if (instrument.FACEVALUE.HasValue)
                            {
                                FaceValue = Convert.ToDecimal(instrument.FACEVALUE.Value);
                            }
                            else
                            {
                                return new JsonResult { Data = "Instrument of ISIN Number :" + oIPO.ISIN + " Must have a face value.Please add a face value."};
                            }
                            if (instrument.PREMIUM.HasValue)
                            {
                                premium = Convert.ToDecimal(instrument.PREMIUM.Value);
                            }
                            else
                            {
                             return new JsonResult { Data = "Instrument of ISIN Number :" + oIPO.ISIN + " Must have a Premium.Please add a Premium value." };                       
                            }                                                                                

                            oIPO.RATE =Convert.ToDecimal(FaceValue + premium);
                            oIPO.FACEVALUE = FaceValue;
                            oIPO.PREMIUM = premium;

                            try
                            {
                                oIPO.LOCKINEXPIRY = records[11] == null ? DateTime.Now : DateTime.Parse(records[11]);
                            }
                            catch (Exception)
                            {
                                oIPO.LOCKINEXPIRY = null;
                            }
                            oIPO.BUSINESSDATE = records[12] == null ? DateTime.Now : DateTime.Parse(records[12]);
                            RecordDate = records[12] == null ? DateTime.Now : DateTime.Parse(records[12]);
                            
                            oCommonFunction.CustomObjectNullValidation<IPO>(ref oIPO);

                            oIPOlist.Add(oIPO);                         

                        }


                        List<IPO> IpoList = new List<IPO>();
                        IpoList = (from old_ca in ISINList.ToList()
                                  join ca in db.IPOes.ToList() on old_ca equals ca.ISIN
                                  where ca.BUSINESSDATE == RecordDate
                                  select ca).ToList();
                        //db.IPOes.Where(ipo => ipo.BUSINESSDATE == RecordDate).ToList();
                        foreach (var ca in IpoList)
                        {
                            db.IPOes.Remove(ca);                            
                        }
                        db.SaveChanges();

                       var list = db.IPOes.Where(dmtx => dmtx.BUSINESSDATE ==RecordDate).ToList();

                        foreach (var d in oIPOlist)
                        {
                            db.IPOes.Add(d);
                        }

                        db.SaveChanges();

                    }
                    else
                    {
                        throw new Exception("Select 16DP95UX.TXT IPO File");
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };
        }



        public ActionResult ImportIPOFreeProcessing()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
               // return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }
            int i = 0;
            string line;
            DateTime RecordDate = DateTime.Now;

            decimal FaceValue = 0;
            decimal premium = 0;
            INSTRUMENT instrument =new INSTRUMENT();
            try
            {
                List<string> ISINList = new List<string>();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader IPOProcessingStream = new StreamReader(file.InputStream);

                    List<IPO> oIPOlist = new List<IPO>();

                    if (fileName == "16DPB7UX.TXT")
                    {

                        while ((line = IPOProcessingStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');


                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;

                            IPO oIPO = new IPO();

                            oIPO.REFERENCE = Guid.NewGuid().ToString();
                            oIPO.CREATEDBY = Session["UserId"].ToString();

                            oIPO.BOID = records[0];
                            oIPO.INVESTORNAME = records[1];


                            INVESTOR oInvestor = new INVESTOR();
                            try
                            {
                                oInvestor = db.INVESTORs.Where(investor => investor.BONUMBER == oIPO.BOID).FirstOrDefault();
                                if (oInvestor == null)
                                {
                                    oIPO.EXCEPTIONDETAILS = "Investor Not Found!";
                                }
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Investor not found of BONUMBER :" + oIPO.BOID + ". Error :" + ex.Message };
                            }

                            oIPO.INVESTORACREF = oInvestor.ACCOUNTNUMBER;             //ConstantVariable.INVESTOR_ACCOUNT_NUMBER;

                            oIPO.ISIN = records[2];
                            oIPO.INSTRUMENTNAME = records[3];
                                                      

                            oIPO.EFFECTIVEDATE = DateTime.Parse(records[7]);
                            oIPO.LOCKINEXPIRY = DateTime.Parse(records[7]);
                            oIPO.BUSINESSDATE = DateTime.Parse(records[7]);
                            RecordDate =  DateTime.Parse(records[7]);

                            oIPO.CURRENTQTY = 0;
                            oIPO.LOCKINQTY = -(int.Parse(records[4].Trim().Split('.')[0]));

                            ISINList.Add(oIPO.ISIN);

                            // added for rate 
                            //var instrument = db.INSTRUMENTs.Where(t => t.ISIN == oIPO.ISIN).SingleOrDefault();
                            //decimal FaceValue = Convert.ToDecimal(instrument.FACEVALUE.Value);
                            //decimal premium = Convert.ToDecimal(instrument.PREMIUM.Value);

                            try
                            {
                                instrument = db.INSTRUMENTs.Where(t => t.ISIN == oIPO.ISIN).SingleOrDefault();
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Instrument not found of ISIN Number :" + oIPO.ISIN + ". Error :" + ex.Message };
                            }
                            if (instrument.FACEVALUE.HasValue)
                            {
                                FaceValue = Convert.ToDecimal(instrument.FACEVALUE.Value);
                            }
                            else
                            {
                                return new JsonResult { Data = "Instrument of ISIN Number :" + oIPO.ISIN + " Must have a face value.Please add a face value." };
                            }
                            if (instrument.PREMIUM.HasValue)
                            {
                                premium = Convert.ToDecimal(instrument.PREMIUM.Value);
                            }
                            else
                            {
                                return new JsonResult { Data = "Instrument of ISIN Number :" + oIPO.ISIN + " Must have a Premium.Please add a Premium value." };
                            }                                                                                



                            oIPO.RATE = Convert.ToDecimal(FaceValue + premium);
                            oIPO.FACEVALUE = FaceValue;
                            oIPO.PREMIUM = premium;                          
                          

                            oCommonFunction.CustomObjectNullValidation<IPO>(ref oIPO);

                            oIPOlist.Add(oIPO);
                            

                        }


                        List<IPO> IpoList = new List<IPO>();
                        IpoList = (from old_ca in ISINList.ToList()
                                   join ca in db.IPOes.ToList() on old_ca equals ca.ISIN
                                   where ca.BUSINESSDATE == RecordDate
                                   select ca).ToList();

                            
                            
                            //db.IPOes.Where(ipo => ipo.BUSINESSDATE == RecordDate).ToList();

                        foreach (var ca in IpoList)
                        {
                            db.IPOes.Remove(ca);                            
                        }
                        db.SaveChanges();

                        var list = db.IPOes.Where(dmtx => dmtx.BUSINESSDATE == RecordDate).ToList();

                        foreach (var d in oIPOlist)
                        {
                            db.IPOes.Add(d);
                        }

                        db.SaveChanges();

                    }
                    else
                    {
                        throw new Exception("Select 16DPB7UX.TXT IPO File");
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };
        }


        public ActionResult ImportTransmissionOut()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
               // return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }

            int i = 0;
            string line;
            DateTime importDate = DateTime.Today;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    List<TRANSFEROFSECURITy> oTRANSFEROFSECURITys = new List<TRANSFEROFSECURITy>();

                    List<string> ISINList = new List<string>();

                    if (fileName == "11DP39UX.TXT")
                    {
                        StreamReader chargeFileStream = new StreamReader(file.InputStream);
                       
                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');

                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;

                            TRANSFEROFSECURITy oTransferofSecurity = new TRANSFEROFSECURITy();
                            oTransferofSecurity.ISIN = records[0];

                            ISINList.Add(oTransferofSecurity.ISIN);

                            INSTRUMENT oInstrument = new INSTRUMENT() { ISIN = oTransferofSecurity.ISIN };
                            //if (oInstrument.Select())
                            oTransferofSecurity.AVGCOST = oInstrument.LASTMARKETPRICE;

                            oTransferofSecurity.REFERENCE = Guid.NewGuid().ToString();
                            oTransferofSecurity.CREATEDBY = Session["UserId"].ToString();
                            oTransferofSecurity.INSTRUMENTNAME =  records[1];
                            oTransferofSecurity.SEQNO =  records[2];
                            oTransferofSecurity.TRANSFERORBO =  records[3];
                            INVESTOR oInvestor =new INVESTOR();
                            try
                            {
                                oInvestor = db.INVESTORs.Where(investor => investor.BONUMBER == oTransferofSecurity.TRANSFERORBO).FirstOrDefault();                        
                                if (oInvestor == null)
                                {
                                    oTransferofSecurity.ISEXCEPTION = "Y";
                                    oTransferofSecurity.EXCEPTIONDETAILS = "Investor Not Found!";
                                }
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Investor not found of BONUMBER :" + oTransferofSecurity.TRANSFERORBO + ". Error :" + ex.Message };
                            }
                            oTransferofSecurity.TRANSFERORNAME =  records[4];
                            oTransferofSecurity.LINKEDBROKERID =  records[5];
                            oTransferofSecurity.LINKEDBROKERNAME = records[6];
                            oTransferofSecurity.TRANSFEREEBO =  records[7];
                            oTransferofSecurity.TRANSFEREENAME =  records[8];
                            oTransferofSecurity.TRANSFERREDQTY =  int.Parse(records[9].Trim());
                            oTransferofSecurity.INITIATEDBY =  records[11];
                            oTransferofSecurity.STATUS = "Pending";
                            oTransferofSecurity.BUSINESSDATE = DateTime.Today;
                            oTransferofSecurity.TRANSFERSTATUS = "OUT";
                            oTransferofSecurity.ISEXCEPTION = Convert.ToString('N');
                            oTransferofSecurity.TRANSFERTYPE = "Transmission";

                            oTransferofSecurity.INVESTORACREF = oInvestor.ACCOUNTNUMBER;// ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //added 03-Jan-2017

                            oCommonFunction.CustomObjectNullValidation<TRANSFEROFSECURITy>(ref oTransferofSecurity);
                            oTRANSFEROFSECURITys.Add(oTransferofSecurity);

                        }
                        
                    }
                    else
                    {
                        throw new Exception("Invalid File! Please Select 11DP39UX-Transmission Out.txt File");
                    }

                    List<TRANSFEROFSECURITy> ToutList = new List<TRANSFEROFSECURITy>();
                    ToutList = (from old_ca in ISINList.ToList()
                                join ca in db.TRANSFEROFSECURITIES.ToList() on old_ca equals ca.ISIN
                                where ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT"
                                select ca).ToList();

                        
                        
                        //db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT").ToList();
                    foreach (var tOut in ToutList)
                    {
                        db.TRANSFEROFSECURITIES.Remove(tOut);
                    }
                    db.SaveChanges();
                    //CAList = db.CORPORATEACTIONs.Where(ca => ca.RECORDDATE == importDate).ToList();
                    ToutList = db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT").ToList();
                    foreach (var tOut in oTRANSFEROFSECURITys)
                    {
                        db.TRANSFEROFSECURITIES.Add(tOut);
                    }
                    db.SaveChanges();
                    ToutList = db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT").ToList();

                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };

        }

        public ActionResult ImportTransmissionIn()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }
            int i = 0;
            string line;
            DateTime importDate = DateTime.Today;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                List<TRANSFEROFSECURITy> oTRANSFEROFSECURITys = new List<TRANSFEROFSECURITy>();
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    List<string> ISINList = new List<string>();

                    if (fileName == "11DP41UX.TXT")
                    {
                        StreamReader chargeFileStream = new StreamReader(file.InputStream);
                        
                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');


                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;


                            TRANSFEROFSECURITy oTransferofSecurity = new TRANSFEROFSECURITy();

                            oTransferofSecurity.ISIN = records[0];

                            ISINList.Add(oTransferofSecurity.ISIN); //added

                            INSTRUMENT oInstrument = new INSTRUMENT() { ISIN = oTransferofSecurity.ISIN };
                            //if (oInstrument.Select())
                            oTransferofSecurity.AVGCOST = oInstrument.LASTMARKETPRICE;

                            oTransferofSecurity.REFERENCE = Guid.NewGuid().ToString();
                            oTransferofSecurity.CREATEDBY = Session["UserId"].ToString();
                            oTransferofSecurity.INSTRUMENTNAME =  records[1];
                            oTransferofSecurity.SEQNO =  records[2];
                            oTransferofSecurity.TRANSFERORBO =  records[3];
                            INVESTOR oInvestor =new INVESTOR();
                            try
                            {
                                oInvestor = db.INVESTORs.Where(investor => investor.BONUMBER == oTransferofSecurity.TRANSFERORBO).FirstOrDefault();
                                if (oInvestor == null)
                                {
                                    oTransferofSecurity.ISEXCEPTION = "Y";
                                    oTransferofSecurity.EXCEPTIONDETAILS = "Investor Not Found!";
                                }
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Investor not found of BONUMBER :" + oTransferofSecurity.TRANSFERORBO + ". Error :" + ex.Message };
                            }

                            oTransferofSecurity.TRANSFERORNAME =  records[4];
                            oTransferofSecurity.LINKEDBROKERID =  records[5];
                            oTransferofSecurity.LINKEDBROKERNAME =  records[6];
                            oTransferofSecurity.TRANSFEREEBO =  records[7];
                            oTransferofSecurity.TRANSFEREENAME =  records[8];
                            oTransferofSecurity.TRANSFERREDQTY =  int.Parse(records[9].Trim());
                            oTransferofSecurity.INITIATEDBY =  records[11];
                            oTransferofSecurity.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";
                            oTransferofSecurity.BUSINESSDATE = DateTime.Today;
                            oTransferofSecurity.TRANSFERSTATUS ="IN";
                            oTransferofSecurity.ISEXCEPTION = Convert.ToString('N');
                            oTransferofSecurity.TRANSFERTYPE = "Transmission";

                            oTransferofSecurity.INVESTORACREF = oInvestor.ACCOUNTNUMBER; // ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //added 03-Jan-2017

                            oCommonFunction.CustomObjectNullValidation<TRANSFEROFSECURITy>(ref oTransferofSecurity);
                            oTRANSFEROFSECURITys.Add(oTransferofSecurity);

                        }
                       

                    }
                    else
                    {
                        throw new Exception("Invalid File! Please Select 11DP41UX-Transmission IN.txt File.");
                    }
                    List<TRANSFEROFSECURITy> TInList = new List<TRANSFEROFSECURITy>();
                    TInList = (from old_ca in ISINList.ToList()
                               join tIn in db.TRANSFEROFSECURITIES.ToList() on old_ca equals tIn.ISIN
                               where tIn.BUSINESSDATE == importDate && tIn.TRANSFERSTATUS == "IN" && tIn.TRANSFERTYPE == "Transmission"
                               select tIn).ToList();

                        
                        
                        //db.TRANSFEROFSECURITIES.Where(tIn => tIn.BUSINESSDATE == importDate && tIn.TRANSFERSTATUS == "IN" && tIn.TRANSFERTYPE == "Transmission").ToList();
                    foreach (var tIn in TInList)
                    {
                        db.TRANSFEROFSECURITIES.Remove(tIn);                        
                    }
                    db.SaveChanges();
                    //CAList = db.CORPORATEACTIONs.Where(ca => ca.RECORDDATE == importDate).ToList();
                    TInList = db.TRANSFEROFSECURITIES.Where(tIn => tIn.BUSINESSDATE == importDate && tIn.TRANSFERSTATUS == "IN" && tIn.TRANSFERTYPE == "Transmission").ToList();
                    foreach (var tIn in oTRANSFEROFSECURITys)
                    {
                        db.TRANSFEROFSECURITIES.Add(tIn);
                    }
                    db.SaveChanges();
                    TInList = db.TRANSFEROFSECURITIES.Where(tIn => tIn.BUSINESSDATE == importDate && tIn.TRANSFERSTATUS == "IN" && tIn.TRANSFERTYPE == "Transmission").ToList();
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };

        }

        public ActionResult ImportSecurityTransfer()
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }
            int i = 0;
            string line;
            DateTime importDate = DateTime.Today;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader chargeFileStream = new StreamReader(file.InputStream);
                    List<TRANSFEROFSECURITy> oTRANSFEROFSECURITys = new List<TRANSFEROFSECURITy>();

                    List<string> ISINList = new List<string>();
                    if (fileName == "11DP81UX.TXT")
                    {

                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');

                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;

                            TRANSFEROFSECURITy oTransferofSecurity = new TRANSFEROFSECURITy();

                            oTransferofSecurity.ISIN = records[0];

                            ISINList.Add(oTransferofSecurity.ISIN); //added 

                            INSTRUMENT oInstrument = new INSTRUMENT() { ISIN = oTransferofSecurity.ISIN };
                            //if (oInstrument.Select())
                            oTransferofSecurity.AVGCOST = oInstrument.LASTMARKETPRICE;

                            oTransferofSecurity.REFERENCE = Guid.NewGuid().ToString();
                            oTransferofSecurity.CREATEDBY = Session["UserId"].ToString();
                            oTransferofSecurity.INSTRUMENTNAME =records[1];
                            oTransferofSecurity.SEQNO =  records[2];
                            oTransferofSecurity.TRANSFERORBO =  records[3];

                            INVESTOR oInvestor =new INVESTOR();
                            try
                            {
                                oInvestor = db.INVESTORs.Where(investor => investor.BONUMBER == oTransferofSecurity.TRANSFERORBO).FirstOrDefault();
                                if (oInvestor == null)
                                {
                                    oTransferofSecurity.ISEXCEPTION = "Y";
                                    oTransferofSecurity.EXCEPTIONDETAILS = "Investor Not Found!";
                                    return new JsonResult { Data = "Investor not found of BONUMBER :" + oTransferofSecurity.TRANSFERORBO };
                     
                                }
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Investor not found of BONUMBER :" + oTransferofSecurity.TRANSFERORBO + ". Error :" + ex.Message };
                            }

                            oTransferofSecurity.TRANSFERORNAME =  records[4];
                            oTransferofSecurity.LINKEDBROKERID =  records[5];
                            oTransferofSecurity.LINKEDBROKERNAME =  records[6];
                            oTransferofSecurity.TRANSFEREEBO =  records[7];
                            oTransferofSecurity.TRANSFEREENAME =  records[8];
                            oTransferofSecurity.TRANSFERREDQTY =  int.Parse(records[9].Trim());
                            oTransferofSecurity.INITIATEDBY =  records[10];
                            oTransferofSecurity.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";
                            oTransferofSecurity.BUSINESSDATE = DateTime.Today;
                            oTransferofSecurity.TRANSFERSTATUS = "IN";
                            oTransferofSecurity.ISEXCEPTION = Convert.ToString('N');
                            oTransferofSecurity.TRANSFERTYPE = "Transfer";

                            oTransferofSecurity.INVESTORACREF = ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //added 03-Jan-2017

                            oCommonFunction.CustomObjectNullValidation<TRANSFEROFSECURITy>(ref oTransferofSecurity);
                            oTRANSFEROFSECURITys.Add(oTransferofSecurity);
                        }                        
                    }
                    else
                    {
                        throw new Exception("Invalid File! Plese Select 11DP81UX-Share Transfer (IN).");
                    }
                    List<TRANSFEROFSECURITy> SList = new List<TRANSFEROFSECURITy>();
                    SList = (from old_ca in ISINList.ToList()
                             join tIn in db.TRANSFEROFSECURITIES.ToList() on old_ca equals tIn.ISIN
                             where tIn.BUSINESSDATE == importDate && tIn.TRANSFERSTATUS == "IN" && tIn.TRANSFERTYPE == "Transfer"
                             select tIn).ToList();

                        
                        
                        //db.TRANSFEROFSECURITIES.Where(tIn => tIn.BUSINESSDATE == importDate && tIn.TRANSFERSTATUS == "IN" && tIn.TRANSFERTYPE == "Transfer").ToList();
                    
                    
                    foreach (var tIn in SList)
                    {
                        db.TRANSFEROFSECURITIES.Remove(tIn);                       
                    }
                    db.SaveChanges();

                    SList = db.TRANSFEROFSECURITIES.Where(tIn => tIn.BUSINESSDATE == importDate && tIn.TRANSFERSTATUS == "IN" && tIn.TRANSFERTYPE == "Transfer").ToList();
                    foreach (var tIn in oTRANSFEROFSECURITys)
                    {
                        db.TRANSFEROFSECURITIES.Add(tIn);
                    }
                    db.SaveChanges();
                    SList = db.TRANSFEROFSECURITIES.Where(tIn => tIn.BUSINESSDATE == importDate && tIn.TRANSFERSTATUS == "IN" && tIn.TRANSFERTYPE == "Transfer").ToList();
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };
        }

        public ActionResult ImportSecurityTransferOut()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };
            }

            int i = 0;
            string line;
            DateTime importDate = DateTime.Today;
            try
            {
                
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    List<TRANSFEROFSECURITy> oTRANSFEROFSECURITys = new List<TRANSFEROFSECURITy>();
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader chargeFileStream = new StreamReader(file.InputStream);

                    List<string> ISINList = new List<string>();

                    if (fileName == "11DP38UX.TXT")
                    {
                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');

                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;


                            TRANSFEROFSECURITy oTransferofSecurity = new TRANSFEROFSECURITy();
                            oTransferofSecurity.ISIN = records[0];

                            ISINList.Add(oTransferofSecurity.ISIN);

                            //I am not Understand this code.From where the last market price come.Still no CDBL file found.So i can not test.Rakibul <Date 2-03-17>
                            INSTRUMENT oInstrument = new INSTRUMENT() { ISIN = oTransferofSecurity.ISIN };                         
                            oTransferofSecurity.AVGCOST = oInstrument.LASTMARKETPRICE;
                            //

                            oTransferofSecurity.REFERENCE = Guid.NewGuid().ToString();
                            oTransferofSecurity.CREATEDBY = Session["UserId"].ToString();
                            oTransferofSecurity.INSTRUMENTNAME = records[1];
                            oTransferofSecurity.SEQNO =  records[2];
                            oTransferofSecurity.TRANSFERORBO =  records[3];
                            INVESTOR oInvestor =new INVESTOR();
                            try
                            {
                                oInvestor = db.INVESTORs.Where(investor => investor.BONUMBER == oTransferofSecurity.TRANSFERORBO).FirstOrDefault();
                                if (oInvestor == null)
                                {
                                    oTransferofSecurity.ISEXCEPTION = "Y";
                                    oTransferofSecurity.EXCEPTIONDETAILS = "Investor Not Found!";
                                    return new JsonResult { Data = "Investor not found of BONUMBER :" + oTransferofSecurity.TRANSFERORBO  };
               
                                }
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Investor not found of BONUMBER :" + oTransferofSecurity.TRANSFERORBO + ". Error :" + ex.Message };
                            }

                            oTransferofSecurity.TRANSFERORNAME = records[4] ;
                            oTransferofSecurity.LINKEDBROKERID = records[5] ;
                            oTransferofSecurity.LINKEDBROKERNAME = records[6];
                            oTransferofSecurity.TRANSFEREEBO = records[7] ;
                            oTransferofSecurity.TRANSFEREENAME = records[8];
                            oTransferofSecurity.TRANSFERREDQTY =  int.Parse(records[9].Trim());
                            oTransferofSecurity.INITIATEDBY = records[10];
                            oTransferofSecurity.STATUS = "Pending";
                            oTransferofSecurity.BUSINESSDATE = DateTime.Today;
                            oTransferofSecurity.TRANSFERSTATUS = "OUT";
                            oTransferofSecurity.ISEXCEPTION = Convert.ToString('N');
                            oTransferofSecurity.TRANSFERTYPE = "Transfer";

                            oTransferofSecurity.INVESTORACREF = oInvestor.ACCOUNTNUMBER; // ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //added 03-Jan-2017

                            oCommonFunction.CustomObjectNullValidation<TRANSFEROFSECURITy>(ref oTransferofSecurity);
                            oTRANSFEROFSECURITys.Add(oTransferofSecurity);

                        }
                        
                    }
                    else
                    {
                        throw new Exception("Invalid File! Please Select 11DP38UX-Share Transfer (OUT).");
                    }
                    List<TRANSFEROFSECURITy> SoutList = new List<TRANSFEROFSECURITy>();
                    SoutList = (from old_ca in ISINList.ToList()
                                join ca in db.TRANSFEROFSECURITIES.ToList() on old_ca equals ca.ISIN
                                where ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT" && ca.TRANSFERTYPE == "Transfer"
                                select ca).ToList();

                        
                        
                        //db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT" && ca.TRANSFERTYPE == "Transfer").ToList();
                   
                    foreach (var sOut in SoutList)
                    {
                        db.TRANSFEROFSECURITIES.Remove(sOut);                       
                    }
                    db.SaveChanges();

                    //CAList = db.CORPORATEACTIONs.Where(ca => ca.RECORDDATE == importDate).ToList();
                    SoutList = db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT" && ca.TRANSFERTYPE == "Transfer").ToList();

                    foreach (var sOut in oTRANSFEROFSECURITys)
                    {
                        db.TRANSFEROFSECURITIES.Add(sOut);
                    }
                    db.SaveChanges();
                    SoutList = db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT" && ca.TRANSFERTYPE == "Transfer").ToList();
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };
        }

        public ActionResult ImportChnageOfOwnershipIn()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
               // return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };
            }
            int i = 0;
            string line;
            DateTime BusinessDate = DateTime.Today;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader chargeFileStream = new StreamReader(file.InputStream);
                    List<TRANSFEROFSECURITy> oTRANSFEROFSECURITys = new List<TRANSFEROFSECURITy>();

                    List<string> ISINList = new List<string>();               

                    if (fileName == "11DP98UX.TXT")
                    {
                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');

                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;
                            
                            TRANSFEROFSECURITy oTransferofSecurity = new TRANSFEROFSECURITy();
                            oTransferofSecurity.ISIN = records[0];
                            ISINList.Add(oTransferofSecurity.ISIN); //added

                            //not understand below code, from where last market price come.Rakibul <2-03-17>
                            INSTRUMENT oInstrument = new INSTRUMENT() { ISIN = oTransferofSecurity.ISIN };                           
                            oTransferofSecurity.AVGCOST = oInstrument.LASTMARKETPRICE;
                            //

                            oTransferofSecurity.REFERENCE = Guid.NewGuid().ToString();
                            oTransferofSecurity.CREATEDBY = Session["UserId"].ToString();
                            oTransferofSecurity.INSTRUMENTNAME = records[1];
                            oTransferofSecurity.SEQNO = records[2];
                            oTransferofSecurity.TRANSFERORBO =  records[3];
                            INVESTOR oInvestor =new INVESTOR();
                            try
                            {
                                oInvestor = db.INVESTORs.Where(investor => investor.BONUMBER == oTransferofSecurity.TRANSFERORBO).FirstOrDefault();
                                if (oInvestor == null)
                                {
                                    oTransferofSecurity.ISEXCEPTION = "Y";
                                    oTransferofSecurity.EXCEPTIONDETAILS = "Investor Not Found!";
                                    return new JsonResult { Data = "Investor not found of BONUMBER :" + oTransferofSecurity.TRANSFERORBO};
               
                                }
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Investor not found of BONUMBER :" + oTransferofSecurity.TRANSFERORBO + ". Error :" + ex.Message };
                            }

                            oTransferofSecurity.TRANSFERORNAME = records[4];
                            oTransferofSecurity.LINKEDBROKERID = records[5];
                            oTransferofSecurity.LINKEDBROKERNAME = records[6];
                            oTransferofSecurity.TRANSFEREEBO = records[7];
                            oTransferofSecurity.TRANSFEREENAME = records[8];
                            oTransferofSecurity.TRANSFERREDQTY = int.Parse(records[9].Trim());
                            oTransferofSecurity.INITIATEDBY = records[10];
                            oTransferofSecurity.AUTHORIZATIONDATE =  DateTime.ParseExact(records[11], "dd-MMM-yyyy", null);
                            oTransferofSecurity.REASONFORTRANSFER =  records[12];
                            oTransferofSecurity.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";

                            //
                            oTransferofSecurity.BUSINESSDATE = records[13] == null ? DateTime.Now : DateTime.Parse(records[13]); //DateTime.Today;
                            BusinessDate = records[13] == null ? DateTime.Now : DateTime.Parse(records[13]); //Added 29/12/16      
                            //

                            oTransferofSecurity.TRANSFERSTATUS = "IN";
                            oTransferofSecurity.ISEXCEPTION = Convert.ToString('N');
                            oTransferofSecurity.TRANSFERTYPE = "Change of Ownership";

                            oTransferofSecurity.INVESTORACREF = oInvestor.ACCOUNTNUMBER; // ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //added 03-Jan-2017

                            oCommonFunction.CustomObjectNullValidation<TRANSFEROFSECURITy>(ref oTransferofSecurity);
                            db.TRANSFEROFSECURITIES.Add(oTransferofSecurity);

                        }
                        List<TRANSFEROFSECURITy> COFInList = new List<TRANSFEROFSECURITy>();
                        COFInList = (from old_ca in ISINList.ToList()
                                     join ca in db.TRANSFEROFSECURITIES.ToList() on old_ca equals ca.ISIN
                                     where ca.BUSINESSDATE == BusinessDate && ca.TRANSFERSTATUS == "IN" && ca.TRANSFERTYPE == "Change of Ownership"
                                     select ca).ToList();

                            
                            
                            
                            //db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == BusinessDate && ca.TRANSFERSTATUS == "IN" && ca.TRANSFERTYPE == "Change of Ownership").ToList();
                        foreach (var COFIn in COFInList)
                        {
                            db.TRANSFEROFSECURITIES.Remove(COFIn);                            
                        }

                        db.SaveChanges();
                     
                       // COFInList = db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == BusinessDate && ca.TRANSFERSTATUS == "IN" && ca.TRANSFERTYPE == "Change of Ownership").ToList(); //off
                        foreach (var sOut in oTRANSFEROFSECURITys)
                        {
                            db.TRANSFEROFSECURITIES.Add(sOut);
                        }
                        db.SaveChanges();
                       // COFInList = db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == BusinessDate && ca.TRANSFERSTATUS == "IN" && ca.TRANSFERTYPE == "Change of Ownership").ToList(); //off
                        
                    }
                    else
                    {
                        throw new Exception("Invalid File! 11DP98UX-Change of Ownership(IN)");
                    }

                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };
        }

        public ActionResult ImportChnageOfOwnershipOut()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }
            int i = 0;
            string line;
            DateTime importDate = DateTime.Today;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    //11DP40UX.TXT
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader chargeFileStream = new StreamReader(file.InputStream);
                    List<TRANSFEROFSECURITy> oTRANSFEROFSECURITys = new List<TRANSFEROFSECURITy>();

                    List<string> ISINList = new List<string>();

                    if (fileName == "11DP40UX.TXT")
                    {
                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');


                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;


                            TRANSFEROFSECURITy oTransferofSecurity = new TRANSFEROFSECURITy();
                            oTransferofSecurity.ISIN = records[0];

                            ISINList.Add(oTransferofSecurity.ISIN);
                            //not understand below code. Rakibul <03-02-17>
                            INSTRUMENT oInstrument = new INSTRUMENT() { ISIN = oTransferofSecurity.ISIN };
                            //if (oInstrument.Select())
                            oTransferofSecurity.AVGCOST = oInstrument.LASTMARKETPRICE;
                            //

                            oTransferofSecurity.REFERENCE = Guid.NewGuid().ToString();
                            oTransferofSecurity.CREATEDBY = Session["UserId"].ToString();
                            oTransferofSecurity.INSTRUMENTNAME = records[1];
                            oTransferofSecurity.SEQNO = records[2];
                            oTransferofSecurity.TRANSFERORBO = records[3];
                            INVESTOR oInvestor = new INVESTOR();
                            try
                            {
                                oInvestor = db.INVESTORs.Where(investor => investor.BONUMBER == oTransferofSecurity.TRANSFERORBO).FirstOrDefault();
                                if (oInvestor == null)
                                {
                                    oTransferofSecurity.ISEXCEPTION = "Y";
                                    oTransferofSecurity.EXCEPTIONDETAILS = "Investor Not Found!";

                                }
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Investor not found of BONUMBER :" + oTransferofSecurity.TRANSFERORBO + ". Error :" + ex.Message };
                            }
                            oTransferofSecurity.TRANSFERORNAME = records[4];
                            oTransferofSecurity.LINKEDBROKERID = records[5];
                            oTransferofSecurity.LINKEDBROKERNAME = records[6];
                            oTransferofSecurity.TRANSFEREEBO = records[7];
                            oTransferofSecurity.TRANSFEREENAME = records[8];
                            oTransferofSecurity.TRANSFERREDQTY = int.Parse(records[9].Trim());
                            oTransferofSecurity.QUANTITY = Int32.Parse(records[10].Trim());
                            oTransferofSecurity.INITIATEDBY = records[11];
                            oTransferofSecurity.AUTHORIZATIONDATE = DateTime.ParseExact(records[12], "dd-MMM-yyyy", null);
                            oTransferofSecurity.REASONFORTRANSFER = records[13];
                            oTransferofSecurity.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";
                            oTransferofSecurity.BUSINESSDATE = DateTime.Today;
                            oTransferofSecurity.TRANSFERSTATUS = "OUT";
                            oTransferofSecurity.ISEXCEPTION = Convert.ToString('N');
                            oTransferofSecurity.TRANSFERTYPE = "Change of Ownership";

                            oTransferofSecurity.INVESTORACREF = ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //added 01-Jan-2017

                            oCommonFunction.CustomObjectNullValidation<TRANSFEROFSECURITy>(ref oTransferofSecurity);
                            oTRANSFEROFSECURITys.Add(oTransferofSecurity);

                        }
                        
                    }
                    else
                    {
                        throw new Exception("Invalid file! Please Select 11DP40UX-Change of Ownership(OUT).");
                    }
                    List<TRANSFEROFSECURITy> COFInList = new List<TRANSFEROFSECURITy>();
                    COFInList = (from old_ca in ISINList.ToList()
                                 join ca in db.TRANSFEROFSECURITIES.ToList() on old_ca equals ca.ISIN
                                 where ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT" && ca.TRANSFERTYPE == "Change of Ownership"
                                 select ca).ToList();
    
                        //db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT" && ca.TRANSFERTYPE == "Change of Ownership").ToList();
                    foreach (var COFIn in COFInList)
                    {
                        db.TRANSFEROFSECURITIES.Remove(COFIn);                      
                    }
                    db.SaveChanges();
                    //CAList = db.CORPORATEACTIONs.Where(ca => ca.RECORDDATE == importDate).ToList();
                    COFInList = db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT" && ca.TRANSFERTYPE == "Change of Ownership").ToList();
                    foreach (var sOut in oTRANSFEROFSECURITys)
                    {
                        db.TRANSFEROFSECURITIES.Add(sOut);
                    }
                    db.SaveChanges();
                    COFInList = db.TRANSFEROFSECURITIES.Where(ca => ca.BUSINESSDATE == importDate && ca.TRANSFERSTATUS == "OUT" && ca.TRANSFERTYPE == "Change of Ownership").ToList();
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };
        }

        public ActionResult ImportPledge()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }
            int i = 0;
            string line;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    List<PLEDGE> oPledgeList = new List<PLEDGE>();

                    List<string> ISINList = new List<string>();

                    if (fileName == "40DP31UX.TXT")
                    {
                        StreamReader chargeFileStream = new StreamReader(file.InputStream);
                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');
                            if (records.Length == 0)
                                continue;
                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;

                            PLEDGE oPledge = new PLEDGE();
                            oPledge.REFERENCE = Guid.NewGuid().ToString();
                            oPledge.CREATEDBY = Session["UserId"].ToString();
                            oPledge.SEQNO = records[0];
                            oPledge.ISIN = records[1];
                            oPledge.ISINSHORTNAME = records[2];
                            oPledge.PLEDGORBOID = records[3];
                            oPledge.PLEDGORBOSHORTNAME = records[4];
                            oPledge.PLEDGEEBOID = records[5];

                            //INVESTOR oInvestor = new INVESTOR();                           
                            //oInvestor = db.INVESTORs.Where(investor => investor.BONUMBER == oPledge.PLEDGEEBOID).FirstOrDefault();
                            //oPledge. = "Y";
                            //oPledge.EXCEPTIONDETAILS = "Investor Not Found!";
                            ISINList.Add(oPledge.ISIN);
                           
                            oPledge.PLEDGEEBOSHORTNAME = records[6];
                            oPledge.PLEDGESTATUS = records[7];
                            oPledge.PLEDGEQTY = Convert.ToDecimal(records[8]);
                            oPledge.PLEDGEVALUE = Convert.ToDecimal(records[9]);
                            oPledge.SETUPDATE = DateTime.ParseExact(records[10], "dd-MMM-yyyy", null);
                            oPledge.EXPIRYDATE = DateTime.ParseExact(records[11], "dd-MMM-yyyy", null);
                            oPledge.TRANSACTIONTYPE = "Pledge";
                            oPledge.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";

                            oPledge.INVESTORACREF = ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //added 03-jan-2017

                            oCommonFunction.CustomObjectNullValidation<PLEDGE>(ref oPledge);

                            oPledgeList.Add(oPledge);
                           
                        }

                        //List<PLEDGE> PleadgeList = new List<PLEDGE>();
                        //PleadgeList = db.PLEDGEs.Where(p => po.BUSINESSDATE == RecordDate).ToList();
                        //foreach (var ca in PleadgeList)
                        //{
                        //    db.IPOes.Remove(ca);
                        //    db.SaveChanges();
                        //}
                        //var list = db.IPOes.Where(dmtx => dmtx.BUSINESSDATE == RecordDate).ToList();
                        foreach (var sOut in oPledgeList)
                        {
                            db.PLEDGEs.Add(sOut);
                        }
                        db.SaveChanges();

                    }
                    else
                    {
                        throw new Exception("Invalid File! Please Seclet 40DP31UX-Pledge File");
                    }

                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };

        }

        public ActionResult ImportUnPledge()
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
               // return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };
            }
            int i = 0;
            string line;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    StreamReader chargeFileStream = new StreamReader(file.InputStream);
                    while ((line = chargeFileStream.ReadLine()) != null)
                    {
                        string[] records = line.Split('~');


                        if (records.Length == 0)
                            continue;

                        if (records.Length > 0 && records[0].Contains("Version"))
                            continue;


                        PLEDGE oPledge = new PLEDGE();

                        oPledge.REFERENCE = Guid.NewGuid().ToString();
                        oPledge.CREATEDBY = Session["UserId"].ToString();
                        oPledge.SEQNO = records[0];
                        oPledge.ISIN = records[1];
                        oPledge.ISINSHORTNAME = records[2];
                        oPledge.PLEDGORBOID = records[3];
                        oPledge.PLEDGORBOSHORTNAME = records[4];
                        oPledge.PLEDGEEBOID = records[5];
                        oPledge.PLEDGEEBOSHORTNAME = records[6];
                        oPledge.PLEDGEQTY = Convert.ToDecimal(records[7]);
                        oPledge.PLEDGEVALUE = Convert.ToDecimal(records[8]);
                        oPledge.EXPIRYDATE = DateTime.ParseExact(records[9], "dd-MMM-yyyy", null);
                        oPledge.PLEDGESTATUS = records[11];
                        oPledge.UNPLEDGEQTY = Convert.ToDecimal(records[12]);
                        oPledge.SETUPDATE = DateTime.ParseExact(records[14], "dd-MMM-yyyy", null);
                        oPledge.BALANCEPLEDGEQTY = Convert.ToDecimal(records[15]);
                        oPledge.BALANCEPLEDGEVALUE = Convert.ToDecimal(records[16]);
                        oPledge.TRANSACTIONTYPE = "UnPledge";
                        oPledge.STATUS = "Pending";

                        oPledge.INVESTORACREF = ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //added 03-Jan-2017

                        oCommonFunction.CustomObjectNullValidation<PLEDGE>(ref oPledge);
                        db.PLEDGEs.Add(oPledge);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };
        }
        
        public ActionResult ImportDividendProcessed()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
               // return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };

            }

            int i = 0;
            string line;
            DateTime importDate =DateTime.Today;
            List<INSTRUMENT> instrument = new List<INSTRUMENT>();
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    List<DIVIDEND> oDIVIDENDs = new List<DIVIDEND>();
                    var fileName = Path.GetFileName(file.FileName);

                    List<string> ISINList = new List<string>();

                   
                    if (fileName == "17DP46UX.TXT")
                    {
                        
                        StreamReader chargeFileStream = new StreamReader(file.InputStream);
                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');

                            //check empty line
                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;


                            DIVIDEND oDividend = new DIVIDEND();

                            oDividend.REFERENCE = Guid.NewGuid().ToString();
                            oDividend.CREATEDBY = Session["UserId"].ToString();



                            //first check this on instrument table if not found then show 
                            //Instrument not found
                            oDividend.ISIN = records[0].Trim();
                            oDividend.ISINSHORTNAME = records[1].Trim();


                            try
                            {
                                instrument = db.INSTRUMENTs.Where(t => t.ISIN == oDividend.ISIN).ToList();
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Instrument not found of ISIN Number :" + oDividend.ISIN + ". Error :" + ex.Message };
                            }


                            oDividend.SEQUENCENO = int.Parse(records[2].Trim());
                            oDividend.CATYPE = records[3].Trim();
                            oDividend.RECORDDATE = DateTime.Parse(records[4].Trim());
                            oDividend.EFFECTIVEDATE = DateTime.Parse(records[5].Trim());
                            oDividend.DIVIDENDAMT = Convert.ToDecimal(records[6].Trim());
                            oDividend.CASHPAYMENTDATE = DateTime.Parse(records[7].Trim());
                            oDividend.BOID = records[8].Trim();
                            //oDividend.InvestorName = records[9].Trim();
                            oDividend.BOTYPE = records[10].Trim();
                            oDividend.BOCATEGORY = records[11].Trim();
                            oDividend.NATIONALITY = records[12].Trim();
                            oDividend.RESIDENCY = records[13].Trim();
                            oDividend.BANKNAME = records[14].Trim();
                            oDividend.BRANCHNAME = records[15].Trim();
                            oDividend.BANKACCOUNTNO = records[16].Trim();
                            oDividend.GROSSCASHAMOUNT = Convert.ToDecimal(records[17].Trim());
                            oDividend.TAXAMOUNT = Convert.ToDecimal(records[18].Trim());
                            oDividend.NETCASHAMOUNT = Convert.ToDecimal(records[19].Trim());
                            oDividend.IMPORTDATE = DateTime.Today;
                            oDividend.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";
                            oDividend.PROCFLAG = "P";

                            ISINList.Add(oDividend.ISIN);

                            oDividend.INVESTORACREF = ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //added 03-Jan-2017

                            oCommonFunction.CustomObjectNullValidation<DIVIDEND>(ref oDividend);
                            oDIVIDENDs.Add(oDividend);
                        }                                         
                       
                    }
                    else
                    {
                        throw new Exception("Invalid File! Please Seclet 17DP46UX File");
                    }
                    List<DIVIDEND> oDivList = new List<DIVIDEND>();

                    oDivList = (from old_ca in ISINList.ToList()
                                join d in db.DIVIDENDs.ToList() on old_ca equals d.ISIN
                                where d.IMPORTDATE == importDate && d.PROCFLAG.ToUpper() == "P".ToUpper()
                                select d).ToList();

                        
                        
                        //db.DIVIDENDs.Where(d => d.IMPORTDATE == importDate && d.PROCFLAG.ToUpper() == "P".ToUpper()).ToList();
                    foreach (var d in oDivList)
                    {
                        db.DIVIDENDs.Remove(d);                       
                    }
                    db.SaveChanges();

                    oDivList = db.DIVIDENDs.Where(d => d.IMPORTDATE == importDate && d.PROCFLAG.ToUpper() == "P".ToUpper()).ToList();
                    foreach (var d in oDIVIDENDs)
                    {
                        db.DIVIDENDs.Add(d);
                    }
                    db.SaveChanges();
                    oDivList = db.DIVIDENDs.Where(d => d.IMPORTDATE == importDate && d.PROCFLAG.ToUpper() == "P".ToUpper()).ToList();
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };

        }

        public ActionResult ImportDividendsReceivable()
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            int i = 0;
            string line;
            DateTime importDate = DateTime.Today;
            List<INSTRUMENT> instrument = new List<INSTRUMENT>();
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);

                    List<string> ISINList = new List<string>();                 

                    if (fileName == "17DP48UX.TXT")
                    {
                        List<DIVIDEND> oDIVIDENDs = new List<DIVIDEND>();
                        StreamReader chargeFileStream = new StreamReader(file.InputStream);
                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');

                            //check empty line
                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;


                            DIVIDEND oDividend = new DIVIDEND();

                            oDividend.REFERENCE = Guid.NewGuid().ToString();
                            oDividend.CREATEDBY = Session["UserId"].ToString();
                            
                            //first check this on instrument table if not found then show 
                            //Instrument not found

                            oDividend.ISIN = records[0].Trim();
                            oDividend.ISINSHORTNAME = records[1].Trim();


                            try
                            {
                                instrument = db.INSTRUMENTs.Where(t => t.ISIN == oDividend.ISIN).ToList(); // .SingleOrDefault();
                            }
                            catch (Exception ex)
                            {
                                return new JsonResult { Data = "Instrument not found of ISIN Number :" + oDividend.ISIN + ". Error :" + ex.Message };
                            }
                                                        
                            oDividend.SEQUENCENO = int.Parse(records[2].Trim());
                            oDividend.CATYPE = records[3].Trim();
                            oDividend.RECORDDATE = DateTime.Parse(records[4].Trim());
                            oDividend.EFFECTIVEDATE = DateTime.Parse(records[5].Trim());
                            oDividend.DIVIDENDAMT = Convert.ToDecimal(records[6].Trim());
                            oDividend.CASHPAYMENTDATE = DateTime.Parse(records[7].Trim());
                            oDividend.BOID = records[9].Trim();
                            //oDividend.InvestorName = records[10].Trim();
                            oDividend.BOTYPE = records[11].Trim();
                            oDividend.BOCATEGORY = records[12].Trim();
                            oDividend.NATIONALITY = records[13].Trim();
                            oDividend.RESIDENCY = records[14].Trim();
                            oDividend.GROSSCASHAMOUNT = Convert.ToDecimal(records[15].Trim());
                            oDividend.TAXAMOUNT = Convert.ToDecimal(records[16].Trim());
                            oDividend.NETCASHAMOUNT = Convert.ToDecimal(records[17].Trim());
                            oDividend.BOHOLDING = int.Parse(records[18].Trim().Split('.')[0]);
                            oDividend.IMPORTDATE = DateTime.Today;
                            oDividend.PROCFLAG = "R";
                            oDividend.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";
                            oDividend.INVESTORACREF = ConstantVariable.INVESTOR_ACCOUNT_NUMBER; //added 03-Jan-2017

                            oCommonFunction.CustomObjectNullValidation<DIVIDEND>(ref oDividend);

                            ISINList.Add(oDividend.ISIN);
                            oDIVIDENDs.Add(oDividend);
                        }

                        List<DIVIDEND> oDivList = new List<DIVIDEND>();
                        DateTime dateTime = DateTime.Now;
                        importDate = DateTime.Today;
                        oDivList = (from old_ca in ISINList.ToList()
                                    join d in db.DIVIDENDs.ToList() on old_ca equals d.ISIN
                                    where d.IMPORTDATE == importDate && d.PROCFLAG.ToUpper() == "R".ToUpper()
                                    select d).ToList();

                            
                            //db.DIVIDENDs.Where(d => d.IMPORTDATE == importDate && d.PROCFLAG.ToUpper() == "R".ToUpper()).ToList();
                        foreach (var d in oDivList)
                        {
                            db.DIVIDENDs.Remove(d);                           
                        }
                        db.SaveChanges();

                        foreach (var d in oDIVIDENDs)
                        {
                            db.DIVIDENDs.Add(d);
                        }
                        db.SaveChanges();
                     
                    }
                    else
                    {
                        throw new Exception("Invalid File! Please Seclet 17dp48ux.txt File");
                    }

                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message };
            }
            return new JsonResult { Data = "Upload Successful" };

        }
        
        public ActionResult ImportBOISN()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            int i = 0;
            string line;
            DateTime RecordDate = DateTime.Today;
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileName = Path.GetFileName(file.FileName);
                    
                    List<string> ISINList = new List<string>();                 


                    if (fileName == "11DPA6UX.TXT")
                    {
                        List<HOLDING> oHoldings = new List<HOLDING>(); 
                        StreamReader chargeFileStream = new StreamReader(file.InputStream);
                        while ((line = chargeFileStream.ReadLine()) != null)
                        {
                            string[] records = line.Split('~');

                            //check empty line
                            if (records.Length == 0)
                                continue;

                            if (records.Length > 0 && records[0].Contains("Version"))
                                continue;

                            HOLDING oHolding = new HOLDING();

                            oHolding.REFERENCE = Guid.NewGuid().ToString();

                            oHolding.CREATEDBY = Session["UserId"].ToString();
                            oHolding.CREATEDDATE = DateTime.Now;

                            oHolding.ISIN = records[0].Trim();
                            oHolding.INSTRUMENTNAME = records[1].Trim();
                            oHolding.BOID = records[2].Trim();

                            oHolding.CURRENTBALANCE = Convert.ToDecimal(records[4].Trim());
                            oHolding.MATUREDBALANCE = Convert.ToDecimal(records[5].Trim());


                            oHolding.BUSINESSDATE = records[7] == null ? DateTime.Now : DateTime.Parse(records[7]);
                            RecordDate = records[7] == null ? DateTime.Now : DateTime.Parse(records[7]);

                            oHolding.STATUS = ConstantVariable.STATUS_PENDING; // "Pending";

                            oHolding.INVESTORACREF = ConstantVariable.INVESTOR_ACCOUNT_NUMBER;

                            oCommonFunction.CustomObjectNullValidation<HOLDING>(ref oHolding);

                            ISINList.Add(oHolding.ISIN);

                            oHoldings.Add(oHolding);
                        }

                        List<HOLDING> oHoldingList = new List<HOLDING>();
                        DateTime dateTime = DateTime.Now;
                        //importDate = DateTime.Today; //off 29/12/16                       

                        //oDivList = db.DIVIDENDs.Where(d => d.IMPORTDATE == importDate && d.PROCFLAG.ToUpper()=="R".ToUpper()).ToList(); //off 29/12/16

                        oHoldingList = (from old_ca in ISINList.ToList()
                                        join t in db.HOLDINGs.ToList() on old_ca equals t.ISIN
                                        where t.BUSINESSDATE == RecordDate
                                        select t).ToList();                                                                             
                        
                        //db.HOLDINGs.Where(t => t.BUSINESSDATE == RecordDate).ToList();

                        foreach (var d in oHoldingList)
                        {
                            db.HOLDINGs.Remove(d);                            
                        }
                        db.SaveChanges();

                        foreach (var d in oHoldings)
                        {
                            db.HOLDINGs.Add(d);

                        }
                        db.SaveChanges();

                        //After saving I need a cross check with portfolio

                        List<ShareCrossCheck> CrossCheck = CrossCheckWithPortfolio(RecordDate);

                        if (CrossCheck.Count > 0)
                        {
                           // return View("CrossCheck", CrossCheck);
                            return new JsonResult { Data =CrossCheck };  //"Cross Check Failed.Some Instrument balance not matched."
                           
                        }
                        
                    }
                    else
                    {
                        throw new Exception("Invalid File! Please Seclet 17dp48ux.txt File");
                    }

                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex.Message+"inner "+ex.InnerException.Message  };
            }
            return new JsonResult { Data = "Upload Successful" };
        }
              
        public List<ShareCrossCheck> CrossCheckWithPortfolio(DateTime date)
        {

          
            Entities db = new Entities(Session["Connection"] as EntityConnection);
            //List<PortfolioInstrument> models = new List<PortfolioInstrument>();
            //List<ShareCrossCheck> ErrorList = new List<ShareCrossCheck>();           
            //models = new Portfolio().GetInvestorPortfolio(date.ToString("dd - MMM - yy"), "",null);
            
            ////Filter fortfolio by checking CurrentBalance=0 and remove it from list
            //models = models.Where(t => t.NetBalance != 0).ToList();
            //var Portfoliolist = (from e in models
            //                     select new SellLimit
            //                     {
            //                         AccountNumber = ConstantVariable.INVESTOR_ACCOUNT_NUMBER, // "00001",
            //                         ShortName = e.Instrument,
            //                         ISIN = db.INSTRUMENTs.Where(t => t.SHORTNAME == e.Instrument).SingleOrDefault().ISIN,
            //                         MaturedBalance = e.MaturedBalance,
            //                         TotalCost = e.TotalCost
            //                     }).ToList();


            //var check = Portfoliolist.Where(t => t.ISIN == null || t.ISIN == "").ToList();

            //if (check != null)
            //{
            //    foreach (var item in check)
            //    {
            //        ErrorList.Add(new ShareCrossCheck { ISIN = "not found", ShortName = item.ShortName, ErrorMessage = "Please add ISIN in Instrument Table" });
            //    }
            //    return ErrorList;
            //}



            List<ShareCrossCheck> ErrorList = new List<ShareCrossCheck>();           
            List<PortfolioViewModel> models = new List<PortfolioViewModel>();
            models = new Portfolio().GetPortfolioFn("00001",date.ToString("dd - MMM - yy"), null, db);

           var Portfoliolist = (from e in models
                                select new SellLimit
                                    {
                                    AccountNumber = e.AccountNumber,
                                    ShortName = e.ShortName,
                                    ISIN =  db.INSTRUMENTs.Where(t => t.SHORTNAME == e.ShortName).FirstOrDefault().ISIN,
                                    MaturedBalance =Convert.ToDouble(e.MaturedBalance),
                                    TotalCost =Convert.ToDouble(e.TotalCost)
                                    }).ToList();


           var check = Portfoliolist.Where(t => t.ISIN == null || t.ISIN == "").ToList();

           if (check != null)
           {
               foreach (var item in check)
               {
                   ErrorList.Add(new ShareCrossCheck { ISIN ="not found", ShortName =item.ShortName, ErrorMessage = "Please add ISIN in Instrument Table" });
               }
               return ErrorList;
           }
                   

           var holding = db.HOLDINGs.Where(t => t.BUSINESSDATE == date).GroupBy(l=>l.ISIN).Select(lg=> new{
                ISIN=lg.Key,
                MaturedBalance=lg.Sum(w=>w.MATUREDBALANCE)                                                       
              }).ToList();
            
           //now cross check holding with PortfolioList is maturedBalance is same or different

           string InstrumentName = "";
           double Diff=0;

           foreach (var data in holding)
           {             

               try
               {                  
                  var CrossCheck = Portfoliolist.Where(t => t.ISIN == data.ISIN).SingleOrDefault();             
                  
                  InstrumentName =Convert.ToString(CrossCheck.ShortName);
              
                  Diff=CrossCheck.MaturedBalance - Convert.ToDouble(data.MaturedBalance);
                                  
                   if (CrossCheck != null && CrossCheck.MaturedBalance !=Convert.ToDouble(data.MaturedBalance))
                   {
                       ErrorList.Add(new ShareCrossCheck { ISIN=data.ISIN, ShortName=InstrumentName, FreeBalanceDiff=Diff, ErrorMessage="Portfolio Free Balance "+CrossCheck.MaturedBalance+" BOISN Free Balance "+data.MaturedBalance+" not matched." });
                   }
                 
                   if(CrossCheck==null)
                       ErrorList.Add(new ShareCrossCheck { ISIN = data.ISIN, ShortName = InstrumentName, FreeBalanceDiff = Diff, ErrorMessage = "Instrument not found in Portfolio." });
             
               }
               catch (Exception ex)
               {
                   ErrorList.Add(new ShareCrossCheck { ISIN =data.ISIN , ShortName =InstrumentName , FreeBalanceDiff =Diff, ErrorMessage ="EXP :"+ ex.Message});               
               }
               Diff = 0;
               InstrumentName = "";
           }

           return ErrorList;
        }

        [HttpGet]
        public ActionResult ListCorporateAction(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 1000, string reference = null, DateTime? startDate = null, DateTime? endDate = null, string acNumber = null, string isin = null, string CAType = null)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            try
            {
                //select ISIN,SHORTNAME from INSTRUMENT;

               // var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();
                

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<CORPORATEACTION> gridModels = new GridModel<CORPORATEACTION>();
                List<CORPORATEACTION> models = null;

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
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).CORPORATEACTIONs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).CORPORATEACTIONs.AsNoTracking().Where(w => w.INSTRUMENTNAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                if (startDate != null && endDate != null)
                {
                    models = (from CA in models
                              where (CA.RECORDDATE >= startDate
                              && CA.RECORDDATE <= endDate) || (CA.EFFECTIVEDATE >= startDate && CA.RECORDDATE <=endDate)
                              select CA).ToList();
                }
                else if (acNumber == null && isin == null && CAType == null)
                {
                    models = models.Where(CA => CA.RECORDDATE == DateTime.Now).ToList();
                }

                //if (string.IsNullOrEmpty(acNumber) != true)
                //{
                //    model=models.Where(CA=>CA.)
                //}

                if (string.IsNullOrEmpty(isin) != true)
                {
                    models = models.Where(CA => CA.ISIN == isin).ToList();
                }

                if (string.IsNullOrEmpty(CAType) != true)
                {
                    models = models.Where(CA => CA.CATYPE == CAType).ToList();
                }



                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }


                var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();             
                ViewBag.InstrumentList = new SelectList(InstrumentList, "ISIN", "SHORTNAME");

                return PartialView("ListCorporateAction", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult ListCorporateActionReceivable(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 1000, string reference = null, DateTime? startDate = null, DateTime? endDate = null, string acNumber = null, string isin = null, string CARType = null)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            try
            {

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<CORPORATEACTIONRECEIVABLE> gridModels = new GridModel<CORPORATEACTIONRECEIVABLE>();
                List<CORPORATEACTIONRECEIVABLE> models = null;

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
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).CORPORATEACTIONRECEIVABLEs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).CORPORATEACTIONRECEIVABLEs.AsNoTracking().Where(w => w.INSTRUMENTNAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                if (startDate != null && endDate != null)
                {
                    models = (from CA in models
                              where CA.IMPORTDATE >= startDate
                              && CA.IMPORTDATE <= endDate
                              select CA).ToList();
                }
                else if (acNumber == null && isin == null && CARType == null)
                {
                    models = models.Where(CA => CA.IMPORTDATE == DateTime.Today).ToList();
                }

                //if (string.IsNullOrEmpty(acNumber) != true)
                //{
                //    model=models.Where(CA=>CA.)
                //}

                if (string.IsNullOrEmpty(isin) != true)
                {
                    models = models.Where(CA => CA.ISIN == isin).ToList();
                }

                if (string.IsNullOrEmpty(CARType) != true)
                {
                    models = models.Where(CA => CA.CATYPE.ToUpper() == CARType.ToUpper()).ToList();
                }



                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }


                var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();
                ViewBag.InstrumentList = new SelectList(InstrumentList, "ISIN", "SHORTNAME");

                return PartialView("ListCorporateActionReceivable", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }


        [HttpGet]
        public ActionResult ListDemats(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 1000, string reference = null, DateTime? startDate = null, DateTime? endDate = null, string acNumber = null, string isin = null, string CAType = null)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            try
            {

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<DEMATCONFIRM> gridModels = new GridModel<DEMATCONFIRM>();
                List<DEMATCONFIRM> models = null;

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
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).DEMATCONFIRMs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).DEMATCONFIRMs.AsNoTracking().Where(w => w.INSTRUMENTNAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                if (startDate != null && endDate != null)
                {
                    models = (from CA in models
                              where CA.BUSINESSDATE >= startDate
                              && CA.BUSINESSDATE <= endDate
                              select CA).ToList();
                }
                else if (acNumber == null && isin == null && CAType == null)
                {
                    models = models.Where(CA => CA.BUSINESSDATE == DateTime.Now).ToList();
                }


                if (string.IsNullOrEmpty(isin) != true)
                {
                    models = models.Where(CA => CA.ISIN == isin).ToList();
                }

              

                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }
                
                var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();
                ViewBag.InstrumentList = new SelectList(InstrumentList, "ISIN", "SHORTNAME");

                return PartialView("ListDemats", gridModels);

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult ListTransfer(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 1000, string reference = null, DateTime? startDate = null, DateTime? endDate = null, string acNumber = null, string isin = null, string TType = null)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            try
            {

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<TRANSFEROFSECURITy> gridModels = new GridModel<TRANSFEROFSECURITy>();
                List<TRANSFEROFSECURITy> models = null;

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
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).TRANSFEROFSECURITIES.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).TRANSFEROFSECURITIES.AsNoTracking().Where(w => w.INSTRUMENTNAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                if (startDate != null && endDate != null)
                {
                    models = (from tOut in models
                              where tOut.BUSINESSDATE >= startDate
                              && tOut.BUSINESSDATE <= endDate
                              select tOut).ToList();
                }
                else if (acNumber == null && isin == null && TType == null)
                {
                    models = models.Where(tOut => tOut.BUSINESSDATE == DateTime.Now).ToList();
                }

             
                if (string.IsNullOrEmpty(isin) != true)
                {
                    models = models.Where(tOut => tOut.ISIN == isin).ToList();
                }

                if (string.IsNullOrEmpty(TType) != true)
                {
                    models = models.Where(t => t.TRANSFERSTATUS.ToUpper() == TType.ToUpper()).ToList();
                }



                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }


                var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();
                ViewBag.InstrumentList = new SelectList(InstrumentList, "ISIN", "SHORTNAME");

                return PartialView("ListTransfer", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult ListPledge(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 1000, string reference = null, DateTime? startDate = null, DateTime? endDate = null, string acNumber = null, string isin = null, string TType = null)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {


                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<PLEDGE> gridModels = new GridModel<PLEDGE>();
                List<PLEDGE> models = null;

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
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).PLEDGEs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).PLEDGEs.AsNoTracking().Where(w => w.ISINSHORTNAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                if (startDate != null && endDate != null)
                {
                    models = (from p in models
                              where p.BUSINESSDATE >= startDate
                              && p.BUSINESSDATE <= endDate
                              select p).ToList();
                }
                else if (acNumber == null && isin == null && TType == null)
                {
                    models = models.Where(p => p.BUSINESSDATE == DateTime.Now).ToList();
                }
                             

                if (string.IsNullOrEmpty(isin) != true)
                {
                    models = models.Where(tOut => tOut.ISIN == isin).ToList();
                }

                
                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }

                var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();
                ViewBag.InstrumentList = new SelectList(InstrumentList, "ISIN", "SHORTNAME");

                return PartialView("ListPledge", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult ListDivident(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 1000, string reference = null, DateTime? startDate = null, DateTime? endDate = null, string acNumber = null, string isin = null, string DType = null)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                string IsExist = Convert.ToString(TempData["message"]);

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<DIVIDEND> gridModels = new GridModel<DIVIDEND>();
                List<DIVIDEND> models = null;

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
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }

                ViewBag.CurrentFilter = filterstring;
               
                if (string.IsNullOrEmpty(filterstring))
                    models = new Portfolio().getDividendList("").ToList().Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                        //new Entities(Session["Connection"] as EntityConnection).DIVIDENDs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models =new Portfolio().getDividendList("").Where(w => w.ISIN.Contains(filterstring)).ToList().Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                     //   new Entities(Session["Connection"] as EntityConnection).DIVIDENDs.AsNoTracking().Where(w => w.ISIN.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                if (startDate != null && endDate != null)
                {
                    models = (from p in models
                              where p.IMPORTDATE >= startDate
                              && p.IMPORTDATE <= endDate
                              select p).ToList();
                }
                else if (acNumber == null && isin == null && DType == null)
                {
                    models = models.Where(p => p.IMPORTDATE == DateTime.Parse("11-05-1980")).ToList();
                }

                if (string.IsNullOrEmpty(acNumber) != true)
                {
                    models = models.Where(d => d.BANKACCOUNTNO == acNumber).ToList();
                }

                if (string.IsNullOrEmpty(isin) != true)
                {
                    models = models.Where(tOut => tOut.ISIN == isin).ToList();
                }

                if (string.IsNullOrEmpty(DType) != true)
                {
                    models = models.Where(t => t.PROCFLAG.ToUpper() == DType.ToUpper()).ToList();
                }
                // now get its instrument name by isin

                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }


                var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();
                ViewBag.InstrumentList = new SelectList(InstrumentList, "ISIN", "SHORTNAME");



                if (!string.IsNullOrEmpty(IsExist) && IsExist != null)
                    ViewBag.HtmlStr = IsExist;

                return PartialView("ListDivident", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult ListIPO(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 1000, string reference = null, DateTime? startDate = null, DateTime? endDate = null, string acNumber = null, string isin = null, string DType = null)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {


                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<IPO> gridModels = new GridModel<IPO>();
                List<IPO> models = null;

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
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).IPOes.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).IPOes.AsNoTracking().Where(w => w.ISIN.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                if (startDate != null && endDate != null)
                {
                    models = (from p in models
                              where p.BUSINESSDATE >= startDate
                              && p.BUSINESSDATE <= endDate
                              select p).ToList();
                }
                //else if (acNumber == null && isin == null && DType == null)
                //{
                //    models = models.Where(p => p.IMPORTDATE == DateTime.Parse("11-05-1980")).ToList();
                //}

              

                if (string.IsNullOrEmpty(isin) != true)
                {
                    models = models.Where(tOut => tOut.ISIN == isin).ToList();
                }

                if (string.IsNullOrEmpty(DType) != true)
                {
                    models = models.Where(t => t.PROCESSFLAG == DType).ToList();
                }



                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }


                var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();
                ViewBag.InstrumentList = new SelectList(InstrumentList, "ISIN", "SHORTNAME");

                return PartialView("ListIPO", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }


        [HttpGet]
        public ActionResult EditIpoRate(string Ref)
        {
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Renew " + Session["currentPage"];

                IPO getIpo = new Entities(Session["Connection"] as EntityConnection).IPOes.Where(t => t.REFERENCE == Ref).SingleOrDefault();


                return PartialView("EditIpoRate", getIpo);

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            
            }        
        
        }

        [HttpPost]
        public ActionResult EditIpoRate(IPO oIPO)
        {

            try
            {
                    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                    {
                     return RedirectToAction("LogOut", "Home");
                    }

                Entities db = new Entities(Session["Connection"] as EntityConnection);

                IPO getIpo = db.IPOes.Where(t => t.REFERENCE == oIPO.REFERENCE).SingleOrDefault();
                getIpo.RATE = oIPO.RATE;
                getIpo.LASTUPDATED = DateTime.Now;
                getIpo.LASTUPDATEDBY = Session["UserId"].ToString();
                db.Entry(getIpo).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ListIPO", "CDBLFiles", new { lblbreadcum="List IPO" });

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }        
        
        }

        #region CASH_DIVIDEND


        [HttpGet]
        public ActionResult EditCashDividend(string DividendRef)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Edit " + Session["currentPage"];

                if (DividendRef == "AddOthers")
                {
                    ViewBag.OthersDividend = "OthersCashDividend";
                    
                    var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();
                    ViewBag.InstrumentList = new SelectList(InstrumentList, "ISIN", "SHORTNAME");

                    return PartialView("AddOthersCashDividend");
                }
                else
                {
                    Entities db = new Entities(Session["Connection"] as EntityConnection);
                    DIVIDEND models = new DIVIDEND();


                    models = new Portfolio().getDividendList("").Where(t => t.REFERENCE == DividendRef).SingleOrDefault();
                    return PartialView(models);
                }
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }

        }


        public ActionResult AddOthersCashDividend(DIVIDEND model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                    {
                        return RedirectToAction("LogOut", "Home");
                    }

                    Entities db = new Entities(Session["Connection"] as EntityConnection);
                    DIVIDEND AddOthersDividend = new DIVIDEND();
                    

                    AddOthersDividend.REFERENCE = Guid.NewGuid().ToString();

                    AddOthersDividend.ISIN = model.ISIN;

                    AddOthersDividend.CATYPE = "OthersDividend";
                    AddOthersDividend.BANKNAME = model.BANKNAME;

                    AddOthersDividend.GROSSCASHAMOUNT = model.GROSSCASHAMOUNT;
                    AddOthersDividend.BOHOLDING = model.BOHOLDING;

                    AddOthersDividend.TAXRATE = model.TAXRATE;
                    AddOthersDividend.TAXAMOUNT = model.TAXAMOUNT;
                    AddOthersDividend.NETCASHAMOUNT = model.NETCASHAMOUNT;

                    AddOthersDividend.IMPORTDATE = model.CASHRECEIVEDDATE;
                    AddOthersDividend.RECORDDATE = model.CASHRECEIVEDDATE;
                    AddOthersDividend.EFFECTIVEDATE = model.CASHRECEIVEDDATE;
                    AddOthersDividend.CASHPAYMENTDATE = model.CASHRECEIVEDDATE;

                    AddOthersDividend.CASHRECEIVEDDATE = model.CASHRECEIVEDDATE;

                    AddOthersDividend.PROCFLAG = "R";
                    AddOthersDividend.REMARKS = model.REMARKS;
                    AddOthersDividend.STATUS = model.STATUS;

                    AddOthersDividend.CREATEDDATE = DateTime.Today;
                    AddOthersDividend.CREATEDBY = Session["UserId"].ToString();

                    db.DIVIDENDs.Add(AddOthersDividend);
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    TempData["message"] = "<span style='color:red'>" + ex.Message + "</span>";
                }
            }

            return RedirectToAction("ListDivident", "CDBLFiles");        
        }


        [HttpPost]
        public ActionResult EditCashDividend(DIVIDEND model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                    {
                        return RedirectToAction("LogOut", "Home");
                    }

                    Entities db = new Entities(Session["Connection"] as EntityConnection);

                    DIVIDEND Update = db.DIVIDENDs.Where(t => t.REFERENCE == model.REFERENCE).SingleOrDefault();


                    if (Update.STATUS == "OthersPending" || Update.STATUS == "OthersApproved")
                    {
                        ViewBag.OthersDividend = "OthersCashDividend";
                    }

                    Update.TAXRATE = model.TAXRATE;
                    Update.TAXAMOUNT = model.TAXAMOUNT;
                    Update.NETCASHAMOUNT = model.NETCASHAMOUNT;

                    Update.CASHRECEIVEDDATE = model.CASHRECEIVEDDATE;
                    Update.REMARKS = model.REMARKS;
                    Update.STATUS = model.STATUS;

                    Update.LASTUPDATED = DateTime.Today;
                    Update.LASTUPDATEDBY = Session["UserId"].ToString();

                    db.Entry(Update).State = EntityState.Modified;
                    db.SaveChanges();
                   
                }
                catch (Exception ex)
                {
                    TempData["message"] = "<span style='color:red'>" + ex.Message + "</span>";
                  
                }
            }

            return RedirectToAction("ListDivident", "CDBLFiles");

        }

        #endregion


    }
}
