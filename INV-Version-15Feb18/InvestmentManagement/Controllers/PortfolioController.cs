using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.OracleClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using Microsoft.Reporting.WebForms;
using InvestmentManagement.App_Code;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Globalization;
using System.Data.SqlClient;


namespace InvestmentManagement.Controllers
{
    public class PortfolioController : Controller
    {
        public StreamWriter oStream { get; set; }
        CommonFunction oCommonFunction = new CommonFunction();
      
        /// <summary>
        /// Portfolio comes from function. Here I mention online link how Oracle Function return multiple row 
        /// http://www.adp-gmbh.ch/ora/plsql/coll/return_table.html (this is basic)
        /// http://stackoverflow.com/questions/36112518/oracle-insert-into-table-values-of-object-type-collection
        /// select * from table(test3('00001','00001')); test3 is function name and its 2 parameter if any 
        /// </summary>      
        public ActionResult  ListPortfolio(string instrument, string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }             

                Entities db = new Entities(Session["Connection"] as EntityConnection);

                var message = Convert.ToString(TempData["Result"]);
                var date = Convert.ToString(TempData["Date"]);                  
                

                if (message != "")
                {
                    GlobalVar.GlobalValue=message +".Date "+date;
                }          
                
                if (oCommonFunction.CheckSession() == true)
                {
                 return RedirectToAction("LogOut", "Home");
                }
                           
               

                GridModel<PortfolioViewModel> gridModels = new GridModel<PortfolioViewModel>();
                List<PortfolioViewModel> models = new List<PortfolioViewModel>();

                // GetPortfolioFn(string AccountNumber,string ledgerDate,Entities db)

                //grid settings                
                gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;
                             
                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
             
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }

                ViewBag.CurrentFilter = filterstring;

                if (filterstring == null)
                    models = new Portfolio().GetPortfolioFn("00001",DateTime.Now.ToString("dd - MMM - yy"),null,db);
                else
                    models = new Portfolio().GetPortfolioFn("00001",filterstring, instrument,db);
              

                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
               
                ViewBag.IslayOut = "";

                var InstrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.Where(t => t.STATUS != ConstantVariable.STATUS_CLOSED).OrderBy(t => t.SHORTNAME).ToList();
                ViewBag.InstrumentList = new SelectList(InstrumentList, "SHORTNAME", "SHORTNAME");


                if (message == "")
                {
                    var value= GlobalVar.GlobalValue;
                    if (value != null || value != "")
                    {
                        ViewBag.Result = GlobalVar.GlobalValue;
                        GlobalVar.GlobalValue = null;
                    }
                    ViewBag.IslayOut = "False";
                    return PartialView("ListPortfolio", gridModels);
                }
                else
                {                  
                    ViewBag.Result =GlobalVar.GlobalValue ;                                 
                    return View("ListPortfolio", gridModels);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        [HttpPost]
        public ActionResult GenerateSellLimit(List<string> SellLimit, DateTime date) //UserIDs, HttpPostedFileBase file, string[] UserIDs,List<string> UserIDs
        {

            //using (StreamWriter w = new StreamWriter(Server.MapPath("~/Reports/ShareReport/SellLimitFile.txt"), true))
            //{
            //    w.WriteLine("This Is a Dummy Text"); // Write the text
            //    w.WriteLine("This is secound line of text");
            //    w.Close();
            //    w.Dispose();
            //    string filePath = Path.Combine(Server.MapPath("~/Reports/ShareReport/SellLimitFile.txt"));
            //    return File(filePath, "text/xml","NewFile.txt");                
            //}
            
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            Entities db = new Entities(Session["Connection"] as EntityConnection);
            List<PortfolioInstrument> models = new List<PortfolioInstrument>();                       
            ControlSelectedPortfolio newPortfolio;

            
            string result = "Filed to create.";
            int flag = 0;
            try
            {
              newPortfolio = FilterSellLimit(SellLimit);
             if (newPortfolio.Message != "Success")
             {
               return RedirectToAction("Index", "ErrorPage", new { message = newPortfolio.Message });
             }
            }
            catch (Exception ex)
            {
             return RedirectToAction("Index", "ErrorPage", new { message = "Sell Limit Error. Exp:" + ex.Message });
            }
            
            try
            {
               //List<SelectedPortfolio> selected = new List<SelectedPortfolio>();                             
               // models = new Portfolio().GetInvestorPortfolio(DateTime.Now.ToString("dd - MMM - yy"), Convert.ToString(instrument), null);
                models = new Portfolio().GetInvestorPortfolio(date.ToString("dd - MMM - yy"), null,null);
               
                string brokerCodePos = ApplicationVariable.MemberCode;

                string positionTimeStamp = date.ToString("yyyyMMdd") + DateTime.Now.ToString("-hhmmss"); //DateTime.Today.ToString("yyyyMMdd") + DateTime.Now.ToString("-hhmmss");
                //ApplicationVariable.OpenDay.ToString("yyyyMMdd") + DateTime.Now.ToString("-hhmmss");

                string postionfileName = positionTimeStamp + "-positions-" + brokerCodePos + ".xml";
                string postionControlFileName = positionTimeStamp + "-positions-" + brokerCodePos + "-ctrl.xml";

                string positionfilePath = Server.MapPath("~/Reports/ShareReport/SellLimit-positions-DLS.xml");                     
                string postionControlFilePath = Server.MapPath("~/Reports/ShareReport/SellLimit-positions-DLS-ctrl.xml");             

                //Filter fortfolio by checking CurrentBalance=0 and remove it from list
                models = models.Where(t => t.NetBalance != 0).ToList();

                //now check is any selected instrument Sellimit is greater then ist matured balance                
                var list = (from e in models
                            join sn in newPortfolio.SelectedPortfolio.ToList() on e.Instrument equals sn.Instrument
                            select new SellLimit
                            {
                                AccountNumber = e.AccountNumber, // ConstantVariable.INVESTOR_ACCOUNT_NUMBER, // "00001",
                                ShortName = e.Instrument,
                                ISIN = db.INSTRUMENTs.Where(t => t.SHORTNAME == e.Instrument).SingleOrDefault().ISIN,
                                MaturedBalance = sn.SellLimit,
                                TotalCost =e.AverageCost * sn.SellLimit,
                                SellLimitCostvalue = (e.AverageCost * sn.SellLimit).ToString("#.0000000", CultureInfo.InvariantCulture),
                                SellimitBalance = sn.SellLimit > 0 ? sn.SellLimit > e.MaturedBalance ? "OverFlow" : "" :"UnderFlow",
                                
                            }).ToList();
                 
                var err = list.Where(t => t.SellimitBalance == "OverFlow" || t.SellimitBalance == "UnderFlow").ToList();

                if (err.Count != 0)
                {
                    //TempData["Result"] = "Some Sell Limit exceed its Matured balance." + err.Count();
                    //return RedirectToAction("ListPortfolio", "Portfolio");
                    return RedirectToAction("Index", "ErrorPage", new { message = "Some Sell Limit exceed its Matured balance." + err.Count() });
                }
                   // return RedirectToAction("Index", "ErrorPage", new { message = "Some Sell Limit exceed its Matured balance."+ err.Count() });
                
                if (list != null)
                    new FlexTradeFileGenerator().GeneratePositionFile(list, positionfilePath, "BatchInsertOrUpdate", brokerCodePos, false, false, false);
                else
                    new FlexTradeFileGenerator().GeneratePositionFile(list, positionfilePath, "IncrementQuantity", brokerCodePos, false, false, false);

                new FlexTradeFileGenerator().GenerateControlFile(positionfilePath, postionControlFilePath);

                result = "File create success.Location: ";

                using (var memoryStream = new MemoryStream())
                {
                    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        
                        ziparchive.CreateEntryFromFile(Server.MapPath("~/Reports/ShareReport/SellLimit-positions-DLS.xml"), postionfileName);
                        ziparchive.CreateEntryFromFile(Server.MapPath("~/Reports/ShareReport/SellLimit-positions-DLS-ctrl.xml"), postionControlFileName);
                       
                    }
                    string ZipFileName = "SellLimit-" + date.ToString("dd-MMM-yy")+".zip";
                    return File(memoryStream.ToArray(), "application/zip", ZipFileName);
                }
             
              
                #region BlockCode
                //Validating generated XML file
                //oGenerateLimit.ReportProgress(0, "Validating XML with scheme .. .. ..");
                //string positionXSDFilePath =System.IO.Path.GetDirectoryName(fullFilePath) +"\\"+"Flextrade-BOS-Positions.xsd";  // AppDomain.CurrentDomain.BaseDirectory
                //string error = new FlexTradeFileGenerator().XMLValidation(positionfilePath, positionXSDFilePath);

                //if (error != string.Empty)
                //{
                //    //write the error log into a text file in same folder location.
                //    oStream = new StreamWriter(System.IO.Path.GetDirectoryName(fullFilePath) + "\\XML File Error log.txt", false);
                //    oStream.Write(error);
                //    oStream.Close();
                //    oStream.Dispose();

                //    result = "XML file validation has failed. Please check it in log file.";
                //}
                #endregion
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        
            TempData["Date"] = date.ToString("dd-MMM-yyyy");
            TempData["Result"] = result;
            return RedirectToAction("ListPortfolio", "Portfolio");
      }

        public ControlSelectedPortfolio FilterSellLimit(List<string> SellLimit)
        {
            ControlSelectedPortfolio cportfolio = new ControlSelectedPortfolio();
            cportfolio.SelectedPortfolio = new List<SelectedPortfolio>();

            cportfolio.Message = "Success";
            var flag = 0;

           // var counter = 0;
            var Sequence = 0;
            var increment = 0;
            var checker = 0;
            double Limit =0;
            string _instrument = null;

            foreach (var item in SellLimit)
            {
                increment++;             
                if (item != null && !string.IsNullOrEmpty(item))
                {                   
                    try
                    {
                        var IsNumber = double.Parse(item);

                        if (checker == increment && Sequence == 1)  
                        {
                            Limit = IsNumber;
                            Sequence = 2;
                        }
                        else
                        {
                            cportfolio.Message = "One or more element has invalid sequence formate.";
                            break;
                        }
                                             
                    }
                    catch (Exception ex)
                    {
                        Sequence = 1;
                        checker = increment +1; //track the increment the next increment value must have a number 
                        string err = ex.Message; // ex Message = "Input string was not in a correct format."
                        _instrument = item;                    
                    }
                }
                if (Sequence == 2)
                {
                    flag = 1;
                    Sequence = 0;
                    checker = 0;                   
                    cportfolio.SelectedPortfolio.Add(new SelectedPortfolio { Instrument = _instrument, SellLimit = Limit });
                }                   
               
            }


            if(flag == 0)
                 cportfolio.Message = "One or more element has null value.";
            return cportfolio;
        
        }


        public ActionResult GeneratePortfolioReport(string date, FormCollection formData, string IsExcell)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }            

            string dater = formData["date"];            
            string txtInstrument = formData["txtInstrument"];
           

            Session["Date"] = date;
            Entities db = new Entities(Session["Connection"] as EntityConnection);
           

            List<PortfolioViewModel> models = new List<PortfolioViewModel>();
            ReportDataSource oPortfolioStatement = new ReportDataSource();

            models = new Portfolio().GetPortfolioFn("00001",date,null,db);

            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/Portfolio.rdlc");

            List<PortfolioBonus> bonus = new List<PortfolioBonus>();
            List<DividendBonus> DividendBonus = new List<DividendBonus>();

            foreach (var list in GetBonusReceivable().AsEnumerable().ToList())
            {
                    bonus.Add(new PortfolioBonus { 
                    ISIN = list["ISIN"].ToString(),
                    BOID = list["BOID"].ToString(),
                    Catype =list["CATYPE"].ToString(),
                    EffectiveDate =list["EFFECTIVEDATE"].ToString(),
                    Investoracref = list["Investoracref"].ToString(),
                    Entitlement = list["Entitlement"].ToString(),
                    InstrumentName = list["InstrumentName"].ToString(),
                    LastMarketPrice = Convert.ToString(new Portfolio().GetClosingPrice(list["ShortName"].ToString(),date)), //list["LastMarketPrice"].ToString(),
                    MarketValue =  list["MarketValue"].ToString(),
                    ParHolding = list["ParHolding"].ToString(),
                    RecordDate = list["RecordDate"].ToString(),
                    ShortName = list["ShortName"].ToString()
 
                });   
            }


            foreach (var dividend in GetDividendReceivable().AsEnumerable().ToList())
            {

                DividendBonus.Add(new DividendBonus
                {
                    ISIN = dividend["ISIN"].ToString(),
                    BOHOLDING = Convert.ToDecimal(dividend["BOHOLDING"]),
                    CASHPAYMENTDATE = dividend["CASHPAYMENTDATE"].ToString(),
                    CATYPE = dividend["CATYPE"].ToString(),
                    NETCASHAMOUNT =Convert.ToDecimal(dividend["NETCASHAMOUNT"]),
                    RECORDDATE = dividend["RECORDDATE"].ToString(),
                    SHORTNAME = dividend["SHORTNAME"].ToString()

                });
            
            }
           
            ReportDataSource rd = new ReportDataSource();
            ReportDataSource dd = new ReportDataSource();
            ReportDataSource div = new ReportDataSource();

            DataTable dtPortfolioStatement = oCommonFunction.ConvertToDataTable(models);
            rd.Name = "PortfolioInstrument";
            rd.Value = dtPortfolioStatement;


            dd.Name = "PortfolioBonus";
            dd.Value = oCommonFunction.ConvertToDataTable(bonus);

            div.Name = "DividendBonus";
            div.Value = oCommonFunction.ConvertToDataTable(DividendBonus);




            string ContentType = "application/vnd.ms-excel";
            string FileType;
            string ReportName;

            if (IsExcell == "true")
            {
                FileType = "Excel";
                ReportName = "Portfolio " + date + ".{0}";
            }
            else
            {
                FileType = "PDF";
                ReportName = "Portfolio " + date + ".pdf";

            }
                        


            ReportParameter[] portfolioParameter = new ReportParameter[] 
            {
             new ReportParameter("CompanyName","DLIC"),
             new ReportParameter("Address","Gulshan-2, Dhaka"),
             new ReportParameter("ReportTitle","Portfolio Statement"),
             new ReportParameter("PortfolioDate",date)
            };

            lr.SetParameters(portfolioParameter);
            lr.DataSources.Add(rd);         
            lr.DataSources.Add(dd);
            lr.DataSources.Add(div);

           // lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);

          //  string Reportname = "Portfolio " + date + ".Pdf";
            string reportType = FileType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
                "  <OutputFormat>" + FileType + "</OutputFormat>" +
                "  <PageWidth>9.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.75in</MarginLeft>" +
                "  <MarginRight>0.25in</MarginRight>" +
                "  <MarginBottom>0.25in</MarginBottom>" +
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
           // return File(renderedBytes, mimeType, Reportname); // "PortfolioStatement.Pdf"
      
        }

        public ActionResult GetInstrumentLedger(string date,string instrument,string FromDate)
        {
            List<PortfolioViewModel> model =new List<PortfolioViewModel>();
            model = null;
           
            model = new Portfolio().GetPortfolioLedgerFN("00001", date, instrument);

            if (instrument == null && string.IsNullOrEmpty(instrument))
                instrument = "All";


            if (FromDate != null && !string.IsNullOrEmpty(FromDate))
            {
                model = model.Where(t => t.TransactionDate >= Convert.ToDateTime(FromDate)).ToList();
            }
            LocalReport lr = new LocalReport();
            lr.ReportPath = Server.MapPath("~/Reports/ShareReport/InstrumentLedger.rdlc");

            ReportDataSource rd = new ReportDataSource();
          

            DataTable dtPortfolioStatement = oCommonFunction.ConvertToDataTable(model);
            rd.Name = "PortfolioLedgerInstrument";
            rd.Value = dtPortfolioStatement;

            ReportParameter[] portfolioParameter = new ReportParameter[] 
            {            
             new ReportParameter("Instrument",instrument),
             new ReportParameter("Date",date),
             new ReportParameter("FromDate",FromDate)
            };

            lr.SetParameters(portfolioParameter);
            lr.DataSources.Add(rd);
           
            string Reportname = "InstrumentLedgerStatement-" + date + ".Pdf";
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
        
        private DataTable GetBonusReceivable()
        {
            string con = System.Configuration.ConfigurationManager.
             ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(con))
            {
                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GETPORTFOLIOCARECEIVABLE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("Effective_DATE", OracleType.DateTime).Value = DateTime.Parse(Session["Date"].ToString());
                cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;

                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }


        private DataTable GetDividendReceivable()
        {
            string con = System.Configuration.ConfigurationManager.
             ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(con))
            {
                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GET_DIVIDEND_RECEIVABLE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("Effective_DATE", OracleType.DateTime).Value = DateTime.Parse(Session["Date"].ToString());
                cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;

                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }





        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("IVMDataSetSource", GetBonusReceivable()));
        }                      
   
        #region OpeningPortfolio<29-09-2016>

        public ActionResult ListOpeningPortfolio(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
               
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<TRADE> gridModels = new GridModel<TRADE>();
                List<TRADE> models = new List<TRADE>();

                //grid settings                
                gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;             


                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
                //int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;
                //DateTime dt = DateTime.Parse("12-MAY-15");

                models = new Entities(Session["Connection"] as EntityConnection).TRADEs.Where(t => t.FILENAME == ConstantVariable.STATUS_OPENING  ).OrderByDescending(t => t.CREATEDDATE).ToList();  //
               
                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
              
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                



                return PartialView("ListOpeningPortfolio", gridModels);
                
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult AddOpeningPortfolio()
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

                ViewBag.InstrumentList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.OrderBy(p => p.SHORTNAME).ToList(), "REFERENCE", "SHORTNAME");

                ViewBag.transactionDate = DateTime.Today;

                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddOpeningPortfolio(TRADE OTrade, decimal? REALIZEDLOSS)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                    {
                        return RedirectToAction("LogOut", "Home");
                    }

                   
                        using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                        {
                            //bool isUpdate = false;


                            var Instrument = db.INSTRUMENTs.Where(t => t.REFERENCE == OTrade.INSTRUMENT_REFERENCE).SingleOrDefault();                           

                            TRADE tradObj = new TRADE();

                            tradObj.REFERENCE = Guid.NewGuid().ToString();
                            tradObj.INVESTORACREF = ConstantVariable.INVESTOR_ACCOUNT_NUMBER;
                            tradObj.INSTRUMENT = Instrument;
                            tradObj.SHAREQUANTITY = OTrade.SHAREQUANTITY;
                            tradObj.RATE = OTrade.RATE;
                            
                            tradObj.TRANSACTIONDATE = OTrade.TRANSACTIONDATE.Value.Date;
                            tradObj.TRANSACTIONTIME = OTrade.TRANSACTIONDATE;

                            //It should be same as the transactiondate.For opening portfolio, when uploading the stock file the matured date should be same as the transaction date.(26-12-16 Skype talk)
                            tradObj.MATUREDDATE = OTrade.TRANSACTIONDATE.Value;  
                                          
                        
                            tradObj.REALIZEDGAIN = OTrade.REALIZEDGAIN==null? REALIZEDLOSS<0 ? REALIZEDLOSS:-REALIZEDLOSS :OTrade.REALIZEDGAIN;

                            tradObj.TOTALAMOUNT = OTrade.RATE.Value * OTrade.SHAREQUANTITY.Value;
                            tradObj.NETAMOUNT= OTrade.RATE.Value * OTrade.SHAREQUANTITY.Value;                        
                            tradObj.TRANSACTIONTYPE = ConstantVariable.STATUS_OPENING_PORTFOLIO_TRANSACTION_TYPE;
                            tradObj.FILENAME = ConstantVariable.STATUS_OPENING;
                            tradObj.STATUS = ConstantVariable.STATUS_POSTED;
                            tradObj.CREATEDBY = Session["UserId"].ToString();
                            tradObj.CREATEDDATE = DateTime.Now;

                            oCommonFunction.CustomObjectNullValidation<TRADE>(ref tradObj);

                            db.TRADEs.Add(tradObj);
                         

                            db.SaveChanges();
                        }


                    return RedirectToAction("ListOpeningPortfolio", "Portfolio");

                }
                catch (Exception ex)
                {
                    //   throw ex;
                    string message = ex.Message;
                    return RedirectToAction("Index", "ErrorPage", new { message });

                }
            }
            return View(OTrade);
            
        }

        [HttpGet]
        public ActionResult EditOpeningPortfolio(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                TRADE otrade = new TRADE();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
            


                otrade = db.TRADEs.Where(i => i.REFERENCE == id).SingleOrDefault();
                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];


                ViewBag.InstrumentList = new SelectList(db.INSTRUMENTs.OrderBy(p => p.SHORTNAME).ToList(), "REFERENCE", "SHORTNAME", otrade.INSTRUMENT_REFERENCE);
               
                

                return PartialView("EditOpeningPortfolio", otrade);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult EditOpeningPortfolio(TRADE oTrade, decimal? REALIZEDLOSS)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                TRADE otradeUpdate = new TRADE();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                                
                otradeUpdate = db.TRADEs.Where(i => i.REFERENCE == oTrade.REFERENCE).SingleOrDefault();

                var Instrument = db.INSTRUMENTs.Where(t => t.REFERENCE == oTrade.INSTRUMENT_REFERENCE).SingleOrDefault();

                
                otradeUpdate.INSTRUMENT = Instrument;
                otradeUpdate.LASTUPDATED = DateTime.Now;
                otradeUpdate.LASTUPDATEDBY = Session["UserId"].ToString();
                otradeUpdate.SHAREQUANTITY = oTrade.SHAREQUANTITY;
                otradeUpdate.RATE = oTrade.RATE;
                otradeUpdate.TRANSACTIONDATE = oTrade.TRANSACTIONDATE;
                otradeUpdate.REALIZEDGAIN = oTrade.REALIZEDGAIN == null ? REALIZEDLOSS < 0 ? REALIZEDLOSS : -REALIZEDLOSS : oTrade.REALIZEDGAIN;

                otradeUpdate.TOTALAMOUNT = oTrade.RATE.Value * oTrade.SHAREQUANTITY.Value;
                otradeUpdate.NETAMOUNT = oTrade.RATE.Value * oTrade.SHAREQUANTITY.Value;

                otradeUpdate.FILENAME = ConstantVariable.STATUS_OPENING;
                otradeUpdate.STATUS = ConstantVariable.STATUS_POSTED;

                oCommonFunction.CustomObjectNullValidation<TRADE>(ref otradeUpdate);
                db.Entry(otradeUpdate).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ListOpeningPortfolio", "Portfolio");
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }

        }

        #endregion
                
    }
}
