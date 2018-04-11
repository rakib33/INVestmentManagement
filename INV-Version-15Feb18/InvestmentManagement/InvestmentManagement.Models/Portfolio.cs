using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data.OracleClient;
using System.Data;
using System.Data.EntityClient;
using System.Globalization;
using InvestmentManagement.App_Code;
using System.Data.SqlClient;
using InvestmentManagement.ViewModel;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class Portfolio
    {

        List<PortfolioInstrument> oInstruments { get; set; }
        public string AccountNumber { get; set; }
        public string BONumber { get; set; }
        public string AccountName { get; set; }
        public double AvailableBalance { get; set; }
        public double ImmaturedBalance { get; set; }
        public double UnclearCheque { get; set; }
        public double LedgerBalance { get; set; }
        public double TotalDeposit { get; set; }
        public double RealizedGainLoss { get; set; }
        public double TotalWithdraw { get; set; }
        public double AccuredInterestCharge { get; set; }
        public double CurrentDepositTotal { get; set; }
        public double TotalCostOfSecurities { get; set; }
        public double MarketValueOfSecurities { get; set; }
        public double MVOfSaleableSecurities { get; set; }
        public double NetAssetValue { get; set; }
        public double Exposure { get; set; }
        public double MarginableExposure { get; set; }
        public double ExposureStatus { get; set; }
        public double TotalEquity { get; set; }
        public double MaginableEquity { get; set; }
        public double TotalEquityDebtRatio { get; set; }
        public double UnrealizedGainLoss { get; set; }
        public double NetGainLoss { get; set; }
        public double MarginEquityDebtRatio { get; set; }
        public double MarginRatio { get; set; }
        public double PurchasePower { get; set; }

        //get portfolio from function
        public List<PortfolioViewModel> GetPortfolioFn(string AccountNumber,string ledgerDate,string Instrument,Entities db)
        {
            try
            {              
                List<PortfolioViewModel> Result = new List<PortfolioViewModel>();
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(con))
                {

                    string query = "select * from table(GetPortfolio_FN('" + AccountNumber + "','" + ledgerDate + "')) where currentbalance !=0 order by shortname";
                    
                    if (!string.IsNullOrEmpty(Instrument) && Instrument != null)
                        query = "select * from table(GetPortfolio_FN('" + AccountNumber + "','" + ledgerDate + "')) where currentbalance !=0 and shortname='" + Instrument + "' order by shortname";


                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dtPortfolioInstrument = new DataTable();
                    da.Fill(dtPortfolioInstrument);

                    Result =    (from DataRow row in dtPortfolioInstrument.Rows
                                select new PortfolioViewModel
                                {
                                AccountNumber = row["AccountNumber"].ToString(),
                                ShortName = row["ShortName"].ToString(),
                                AvgPrice = Convert.ToDecimal(row["AvgPrice"]),
                                BuyQty = Convert.ToDecimal(row["BuyQty"]),
                                CurrentBalance = Convert.ToDecimal(row["CurrentBalance"]),
                                Issued = Convert.ToDecimal(row["Issued"]),
                                LockinBalance = Convert.ToDecimal(row["LockinBalance"]),
                                MarketPrice = Convert.ToDecimal(row["MarketPrice"]),
                                MarketValue = Convert.ToDecimal(row["MarketValue"]),
                                MaturedBalance = Convert.ToDecimal(row["MaturedBalance"]),
                                NetBuyAmount = Convert.ToDecimal(row["NetBuyAmount"]),
                                NetSaleAmount = Convert.ToDecimal(row["NetSaleAmount"]),
                                RealizedGain = Convert.ToDecimal(row["RealizedGain"]),
                                Receivable = Convert.ToDecimal(row["Receivable"]),
                                Received = Convert.ToDecimal(row["Received"]),
                                SaleQty = Convert.ToDecimal(row["SaleQty"]),
                                TotalCost = Convert.ToDecimal(row["TotalCost"]),
                                UnRealizedGain = Convert.ToDecimal(row["UnRealizedGain"]),
                                PercentageGain = Convert.ToDecimal(row["TotalCost"]) > 0 ? Convert.ToDecimal(row["UnRealizedGain"]) / Convert.ToDecimal(row["TotalCost"]) *100 : 0
                         }).OrderBy(t=>t.ShortName).ToList();


                }

                return Result;

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public List<PortfolioViewModel> GetPortfolioLedgerFN(string AccountNumber, string ledgerDate, string Instrument)
        {
            try
            {
               
                List<PortfolioViewModel> Result = new List<PortfolioViewModel>();
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(con))
                {

                    string query = "select * from table(GetPortfolio_Ledger_FN('" + AccountNumber + "','" + ledgerDate + "',''))"; //order by shortname where currentbalance !=0 

                    if (!string.IsNullOrEmpty(Instrument) && Instrument != null)
                        query = "select * from table(GetPortfolio_Ledger_FN('" + AccountNumber + "','" + ledgerDate + "','" + Instrument + "')) where shortname='" + Instrument + "'"; // order by shortname currentbalance !=0 and


                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dtPortfolioInstrument = new DataTable();
                    da.Fill(dtPortfolioInstrument);

                    Result = (from DataRow row in dtPortfolioInstrument.Rows
                              select new PortfolioViewModel
                              {
     
                                  AccountNumber = row["AccountNumber"].ToString(),
                                  ShortName = row["ShortName"].ToString(),
                                  TransactionDate = Convert.ToDateTime(row["TransactionDate"]),
                                  TransactionType =row["TransactionType"].ToString(),                                  
                                  BuyQty = Convert.ToDecimal(row["BuyQty"]),
                                  NetBuyAmount =Convert.ToDecimal(row["NetBuyAmount"]),
                                  SaleQty = Convert.ToDecimal(row["SaleQty"]),
                                  NetSaleAmount = Convert.ToDecimal(row["NetSaleAmount"]),
                                  Receivable = Convert.ToDecimal(row["Receivable"]),
                                  Received = Convert.ToDecimal(row["Received"]),
                                  Issued = Convert.ToDecimal(row["Issued"]),
                                  CurrentBalance = Convert.ToDecimal(row["CurrentBalance"]),
                                  AvgPrice = Convert.ToDecimal(row["AvgPrice"]),                                  
                                  TotalCost = Convert.ToDecimal(row["TotalCost"]),
                                  RealizedGain = Convert.ToDecimal(row["RealizedGain"]),
                                 
                                  }).OrderBy(t => t.ShortName).ToList();


                }

                return Result;

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public DataTable GetProfitGainLossProcedure(string AccountNumber, string ToDate, string FromDate, string Instrument)
        {
            try
            {

               string con = System.Configuration.ConfigurationManager.
               ConnectionStrings["ConnectionString"].ConnectionString;
               using (OracleConnection conn = new OracleConnection(con))
               {
                   OracleDataAdapter da = new OracleDataAdapter();
                   OracleCommand cmd = new OracleCommand();
                   cmd.Connection = conn;
                   cmd.CommandText = "GET_PROFITLOSS_STATEMENT";
                   cmd.CommandType = CommandType.StoredProcedure;
                   cmd.Parameters.Add("To_Date", OracleType.DateTime).Value = DateTime.Parse(ToDate.ToString());

                   cmd.Parameters.Add("From_Date", OracleType.DateTime).Value = DateTime.Parse(Convert.ToString(FromDate));
                   
                   cmd.Parameters.Add("AccountNumber", OracleType.VarChar).Value = Convert.ToString(AccountNumber);
                   cmd.Parameters.Add("ShortName", OracleType.VarChar).Value = Convert.ToString(Instrument);
                   cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;

                   da.SelectCommand = cmd;
                   DataTable dt = new DataTable();
                   da.Fill(dt);

                   return dt;

                 }                                      

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public List<PortfolioInstrument> GetInvestorPortfolio(string ledgerDate,string Instrument,string STATUS)
        {
            try
            {
                oInstruments = new List<PortfolioInstrument>();

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(con))
                {

                    string query = "select * from INSTRUMENTLEDGERALL where ACCOUNTNUMBER='00001' and TRANDATE<='"+ ledgerDate+"' order by ACCOUNTNUMBER,SHORTNAME,TRANDATE";                   
                    if(!string.IsNullOrEmpty(Instrument) && Instrument!=null)
                        query = "select * from INSTRUMENTLEDGERALL where ACCOUNTNUMBER='00001' and TRANDATE<='" + ledgerDate + "' and SHORTNAME='" + Instrument + "' order by ACCOUNTNUMBER,SHORTNAME,TRANDATE";
                   
                    
                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dtPortfolioInstrument = new DataTable();
                    da.Fill(dtPortfolioInstrument);
                   
                    foreach (DataRow dR in new DataView(dtPortfolioInstrument).ToTable(true,"SHORTNAME").Rows)
                    {

                        //DataRow[] instrumentLedger = dtPortfolioInstrument.Select("SHORTNAME='" + dR["SHORTNAME"].ToString() + "'");
                        //PortfolioInstrument stockLedger = new PortfolioInstrument();
                        //stockLedger = GetStockLedger(instrumentLedger, ledgerDate);
                        //oInstruments.Add(stockLedger);

                        #region TakePortfolioByInstrumentStatus
                        //now check is this instrument status is not Closed
                        
                        var ShortName = dR["SHORTNAME"].ToString();                      
                        string getInstrument ="select status from instrument where shortname='"+ShortName+"'";
                        OracleCommand cmdinstrumentQuery = new OracleCommand(getInstrument, conn);
                        OracleDataAdapter InstrumentData = new OracleDataAdapter(cmdinstrumentQuery);
                        DataTable InstrumentStatus = new DataTable();
                        InstrumentData.Fill(InstrumentStatus);

                        var status = new DataView(InstrumentStatus).ToTable(true, "STATUS").Rows[0].ItemArray[0].ToString();


                        if (status != ConstantVariable.STATUS_CLOSED || status == STATUS)
                        {
                            DataRow[] instrumentLedger = dtPortfolioInstrument.Select("SHORTNAME='" + dR["SHORTNAME"].ToString() + "'");
                            PortfolioInstrument stockLedger = new PortfolioInstrument();
                            stockLedger = GetStockLedger(instrumentLedger, ledgerDate);
                            oInstruments.Add(stockLedger);
                        }
                                               
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }

            return oInstruments;
        
        }


        public PortfolioInstrument GetStockLedger(DataRow[] instrumentLedger,string ledgerDate )
        {
            //string con = "User ID=HR;Password=tiger;Data Source=192.168.0.5:1522/xe;";
            string con = System.Configuration.ConfigurationManager.
            ConnectionStrings["ConnectionString"].ConnectionString;
            PortfolioInstrument stockLedger = new PortfolioInstrument();
            try
            {
                foreach (DataRow dr in instrumentLedger)
                {
                    
                    var Instrument = dr[2].ToString();
                    var tranType = dr["TRANTYPE"].ToString(); //dr[13].ToString();
                  

                    stockLedger.AccountNumber = dr[0].ToString();


                    using (OracleConnection conn = new OracleConnection(con))
                    {
                        
                        string marketPriceQuery = "Select CLOSINGPRICE from PRICEINDEX where INSTRUMENTREF = '" + dr["SHORTNAME"].ToString() + "' and TRADINGDATE = (SElect max(TRADINGDATE) from PRICEINDEX where INSTRUMENTREF ='" + dr["SHORTNAME"].ToString() + "' and Tradingdate <= '"+ledgerDate+"')";
                        OracleCommand cmdmarketPriceQuery = new OracleCommand(marketPriceQuery, conn);
                        OracleDataAdapter damarketPrice = new OracleDataAdapter(cmdmarketPriceQuery);
                        DataTable dtmarketPrice = new DataTable();
                        damarketPrice.Fill(dtmarketPrice);
                        
                   
                        if (dtmarketPrice.Rows.Count > 0)
                        {
                            stockLedger.MarketRate = Convert.ToDouble(dtmarketPrice.Rows[0]["CLOSINGPRICE"].ToString());
                            stockLedger.MarketValue = stockLedger.MarketRate * stockLedger.NetBalance;
                        }
                        
                       
                        string peratio = "Select PERATIO from Instrument Where ShortName = '" + dr["SHORTNAME"].ToString() + "'";
                        OracleCommand cmdperatio = new OracleCommand(peratio, conn);
                        OracleDataAdapter daperatio = new OracleDataAdapter(cmdperatio);
                        DataTable dtperatio = new DataTable();
                        daperatio.Fill(dtperatio);
                        if (dtperatio.Rows.Count > 0)
                            stockLedger.PERatio = Convert.ToDouble(dtperatio.Rows[0]["PERATIO"].ToString());

                    }
                    double buy = double.Parse(dr["BUY"].ToString());
                    double sale = double.Parse(dr["SALE"].ToString());
                    double received = double.Parse(dr["RECEIVED"].ToString());
                    double issued = double.Parse(dr["ISSUED"].ToString());

                    
                    stockLedger.Instrument = dr["SHORTNAME"].ToString();                  

                    double LockinQty = double.Parse(dr["LOCKINBALANCE"].ToString());
                    if (buy >= 0)
                    {
                        stockLedger.NetBalance = stockLedger.NetBalance + buy;
                   
                        if (!string.IsNullOrEmpty((dr["MATUREDDATE"].ToString())))
                        {
                            if (DateTime.Parse((dr["MATUREDDATE"].ToString()).ToString()) <= DateTime.Parse(ledgerDate))
                            {                    
                                                         
                             stockLedger.MaturedBalance = stockLedger.MaturedBalance + buy - LockinQty;                            

                            }
                        }

                        if (stockLedger.NetBalance != 0)
                        {
                            stockLedger.AverageCost = (stockLedger.TotalCost + double.Parse(dr["NETBUYAMOUNT"].ToString())) / stockLedger.NetBalance;
                        }
                        else
                            stockLedger.AverageCost = 0;                    
                    }

                    if (received > 0)
                    {
                        stockLedger.NetBalance = stockLedger.NetBalance + received;
                        if ( !string.IsNullOrEmpty((dr["MATUREDDATE"].ToString())))
                        {
                            if (DateTime.Parse(dr["MATUREDDATE"].ToString()) <= DateTime.Parse(ledgerDate))
                            {
                            stockLedger.MaturedBalance = stockLedger.MaturedBalance + received;
                            }
                        }
                        if (stockLedger.NetBalance != 0)
                        {
                         stockLedger.AverageCost = (stockLedger.TotalCost + double.Parse(dr["NETBUYAMOUNT"].ToString())) / stockLedger.NetBalance;
                        }
                    
                    }

                    if(sale > 0)
                    {
                        stockLedger.NetBalance = stockLedger.NetBalance - sale;
                        stockLedger.MaturedBalance = stockLedger.MaturedBalance - sale;
                        stockLedger.RealizedGain = stockLedger.RealizedGain + (double.Parse(dr["NETSALEAMOUNT"].ToString()) - (stockLedger.AverageCost * sale));
                    }


                    if (issued > 0)
                    {                       
                        var a = stockLedger.AverageCost * sale;
                        stockLedger.NetBalance = stockLedger.NetBalance - issued;
                        stockLedger.MaturedBalance = stockLedger.MaturedBalance - issued;

                        if (dr["TRANTYPE"].ToString()=="Share Transfer Out")
                        {
                            stockLedger.RealizedGain = stockLedger.RealizedGain + (double.Parse(dr["NETSALEAMOUNT"].ToString()) - (stockLedger.AverageCost * issued));
                        }
                    }


                    if (dr["TRANTYPE"].ToString()!="Stock Split <Received>" && dr["TRANTYPE"].ToString()!="Stock Split <Delivered>")
                    {                     
                      stockLedger.TotalCost = stockLedger.AverageCost * stockLedger.NetBalance;
                    }

                    stockLedger.MarketValue = (stockLedger.MarketRate * stockLedger.NetBalance);
                    stockLedger.UnrealizedGain=(stockLedger.MarketValue-stockLedger.TotalCost);
                    
                    if (stockLedger.TotalCost>0)
                    stockLedger.PercentageGain = ((stockLedger.MarketValue - stockLedger.TotalCost) / stockLedger.TotalCost) * 100;     
                }
                
               
            }
            catch (Exception)
            {
                
                throw;
            }
           
            return stockLedger;
        }

        #region BuySellStatement <Date 25-01-17 Rakibul>

        public List<BuySellstatement> GetBuySellStatement(string fromdate, string ToDate, string Instrument, decimal Commission, string Option)
        {
            List<BuySellstatement> oBuySellStatement = new List<BuySellstatement>();
            try
            {
                int BuySellCount=0;
             

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(con))
                {
                 
                    //select memo,transactiontype, sum(sharequantity) Qty,round(avg(rate),4) Price,sum(totalamount) Amount,sum(commission) Commission , (sum(totalamount) - sum(commission)) NetAmount from trade where transactiontype='S' and transactiondate>='1-FEB-17' and transactiondate<='8-FEB-17'
                    //group by memo,transactiontype
                    //union
                    //select memo,transactiontype, sum(sharequantity) Qty,round(avg(rate),4) Price,sum(totalamount) Amount,sum(commission) Commission , (sum(totalamount) + sum(commission)) NetAmount from trade where transactiontype='B' and transactiondate>='1-FEB-17' and transactiondate<='8-FEB-17'
                    //group by memo,transactiontype
                    //order by memo
                    //;
                    
                    string condition = "";
                    string query = "";
                    if (ToDate == null && string.IsNullOrEmpty(ToDate))
                    {
                        ToDate = DateTime.Now.ToString("dd - MMM - yy");
                    }
                    condition = " trandate<='" + ToDate + "'";
                  
                    if (fromdate != null && !string.IsNullOrEmpty(fromdate))
                        condition = " trandate>='" + fromdate + "' and " + condition + " ";

                    if (Instrument != null && !string.IsNullOrEmpty(Instrument))
                        condition = condition + " and SHORTNAME='" + Instrument + "'";

                    if (Option == ConstantVariable.SettlementPayIn)
                    {
                        query = "SELECT SHORTNAME,SUM(BUY) BUY,SUM(NETBUYAMOUNT) NETBUYAMOUNT,SUM(SALE) SALE,ROUND(SUM(NETSALEAMOUNT + COMMISSION)/SUM(SALE),4) AVGPRICE , SUM(NETSALEAMOUNT + COMMISSION) AMOUNT, SUM(COMMISSION) COMMISSION, SUM(NETSALEAMOUNT) NETSALEAMOUNT, TRANTYPE  FROM INSTRUMENTLEDGERALL WHERE ACCOUNTNUMBER='00001' AND TRANTYPE='" + ConstantVariable.TranTypeShareSold + "' AND " + condition + " GROUP BY SHORTNAME,TRANTYPE " +
                                "ORDER BY SHORTNAME";                          
                    }
                    else if (Option == ConstantVariable.SettlementPayOut)
                    {
                        query = "SELECT SHORTNAME,sum(BUY) BUY,SUM(NETBUYAMOUNT) NETBUYAMOUNT,SUM(SALE) SALE,ROUND(SUM(NETBUYAMOUNT - COMMISSION)/SUM(BUY),4) AVGPRICE, SUM(NETBUYAMOUNT -COMMISSION) AMOUNT, SUM(COMMISSION) COMMISSION, SUM(NETSALEAMOUNT) NETSALEAMOUNT, TRANTYPE   FROM INSTRUMENTLEDGERALL WHERE ACCOUNTNUMBER='00001' AND  TRANTYPE='"+ConstantVariable.TranTypeShareBought+"' AND " + condition + "  GROUP BY SHORTNAME,TRANTYPE ORDER BY SHORTNAME";
                        
                    }
                    else
                    {

                        query =    "SELECT SHORTNAME,SUM(BUY) BUY,SUM(NETBUYAMOUNT) NETBUYAMOUNT,SUM(SALE) SALE,ROUND(SUM(NETBUYAMOUNT - COMMISSION)/SUM(BUY),4) AVGPRICE, SUM(NETBUYAMOUNT - COMMISSION) AMOUNT, SUM(COMMISSION) COMMISSION, SUM(NETSALEAMOUNT) NETSALEAMOUNT, TRANTYPE  FROM INSTRUMENTLEDGERALL WHERE ACCOUNTNUMBER='00001' AND  TRANTYPE='" + ConstantVariable.TranTypeShareBought + "' AND " + condition + " GROUP BY SHORTNAME,TRANTYPE " +
                                   "UNION " +
                                   "SELECT SHORTNAME,SUM(BUY) BUY,SUM(NETBUYAMOUNT) NETBUYAMOUNT,SUM(SALE) SALE,ROUND(SUM(NETSALEAMOUNT + COMMISSION)/SUM(SALE),4) AVGPRICE , SUM(NETSALEAMOUNT + COMMISSION) AMOUNT, SUM(COMMISSION) COMMISSION, SUM(NETSALEAMOUNT) NETSALEAMOUNT, TRANTYPE  FROM INSTRUMENTLEDGERALL WHERE ACCOUNTNUMBER='00001' AND TRANTYPE='" + ConstantVariable.TranTypeShareSold + "' AND " + condition + " GROUP BY SHORTNAME,TRANTYPE " +
                                  "ORDER BY SHORTNAME";
                    }
                    
                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dtPortfolioInstrument = new DataTable();
                    da.Fill(dtPortfolioInstrument);

                    foreach (DataRow dR in new DataView(dtPortfolioInstrument).ToTable(true, "SHORTNAME").Rows)
                    {
                        DataRow[] instrumentLedger = dtPortfolioInstrument.Select("SHORTNAME='" + dR["SHORTNAME"].ToString() + "'");
                        BuySellstatement BuySellLedger = new BuySellstatement();

                        BuySellCount = 0;
                     

                        foreach (DataRow dr in instrumentLedger)
                        {
                          //  BuySellLedger.AccountNumber = dr["ACCOUNTNUMBER"].ToString();
                            BuySellLedger.TradeCode = dr["SHORTNAME"].ToString();
                            BuySellLedger.TransactionType = dr["TRANTYPE"].ToString();
                            BuySellLedger.Commission += double.Parse(dr["COMMISSION"].ToString());

                            if (BuySellLedger.TransactionType == ConstantVariable.TranTypeShareBought)  //  "B"
                            {
                                BuySellCount++;
                                BuySellLedger.BuyQty = double.Parse(dr["BUY"].ToString());
                                BuySellLedger.BuyPrice = double.Parse(dr["AVGPRICE"].ToString());
                                BuySellLedger.BuyAmount = double.Parse(dr["AMOUNT"].ToString());
                                BuySellLedger.Net = double.Parse(dr["NETBUYAMOUNT"].ToString());
                                BuySellLedger.Net = BuySellLedger.Net * -1; //because for Buy money is reduced
                            }
                            if (BuySellLedger.TransactionType == ConstantVariable.TranTypeShareSold) //"S"
                            {
                                BuySellCount++;
                                BuySellLedger.SellQty = double.Parse(dr["SALE"].ToString());
                                BuySellLedger.SellPrice = double.Parse(dr["AVGPRICE"].ToString());
                                BuySellLedger.SellAmount = double.Parse(dr["AMOUNT"].ToString());
                                BuySellLedger.Net = double.Parse(dr["NETSALEAMOUNT"].ToString());
                            }                         
                                                      
                       }

                        if (BuySellCount == 2) //if a instrument has both Share Bought and Sold within this date range
                        {
                            BuySellLedger.Commission = (BuySellLedger.BuyAmount + BuySellLedger.SellAmount) * Convert.ToDouble(Commission/100);
                            BuySellLedger.Net = BuySellLedger.SellAmount - BuySellLedger.BuyAmount - BuySellLedger.Commission;
                        }

                    
                        BuySellLedger.CommissionPercentage =Convert.ToDouble(Commission);                   
                        BuySellLedger.Gross = BuySellLedger.SellAmount - BuySellLedger.Commission;  //for ProfitLoss Report
                        BuySellLedger.NetProfitLoss = BuySellLedger.Gross - BuySellLedger.Commission;                       
                        
                        oBuySellStatement.Add(BuySellLedger);
                    }

                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw ex;
            }

            return oBuySellStatement;

        }



        public DataTable GetCommisionStatement(DateTime FromDate, DateTime ToDate) //, string Instrument, decimal Commission, string Option
        {
            //List<BuySellstatement> oBuySellStatement = new List<BuySellstatement>();

            //cal procedure here
            try
            {

                string con = System.Configuration.ConfigurationManager.
                ConnectionStrings["ConnectionString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(con))
                {
                    OracleDataAdapter da = new OracleDataAdapter();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "GET_COMMISSION_STATEMENT";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("ToDate", OracleType.DateTime).Value = ToDate; // DateTime.Parse(ToDate.ToString());

                    cmd.Parameters.Add("FromDate", OracleType.DateTime).Value = FromDate; // DateTime.Parse(Convert.ToString(FromDate));
                    //cmd.Parameters.Add("AccountNumber", OracleType.VarChar).Value = Convert.ToString(AccountNumber);
                    //cmd.Parameters.Add("ShortName", OracleType.VarChar).Value = Convert.ToString(Instrument);
                    cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;

                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return dt;

                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }

            //try
            //{
            //    int BuySellCount = 0;


            //    string con = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            //    using (OracleConnection conn = new OracleConnection(con))
            //    {


            //        string condition = "";
            //        string query = "";
            //        if (ToDate == null && string.IsNullOrEmpty(ToDate))
            //        {
            //            ToDate = DateTime.Now.ToString("dd - MMM - yy");
            //        }
            //        condition = " trandate<='" + ToDate + "'";

            //        if (fromdate != null && !string.IsNullOrEmpty(fromdate))
            //            condition = " trandate>='" + fromdate + "' and " + condition + " ";

            //        if (Instrument != null && !string.IsNullOrEmpty(Instrument))
            //            condition = condition + " and SHORTNAME='" + Instrument + "'";

                 

            //            query = "SELECT SHORTNAME,SUM(BUY) BUY,SUM(NETBUYAMOUNT) NETBUYAMOUNT,SUM(SALE) SALE,ROUND(SUM(NETBUYAMOUNT - COMMISSION)/SUM(BUY),4) AVGPRICE, SUM(NETBUYAMOUNT - COMMISSION) AMOUNT, SUM(COMMISSION) COMMISSION, SUM(NETSALEAMOUNT) NETSALEAMOUNT, TRANTYPE  FROM INSTRUMENTLEDGERALL WHERE ACCOUNTNUMBER='00001' AND  TRANTYPE='" + ConstantVariable.TranTypeShareBought + "' AND " + condition + " GROUP BY SHORTNAME,TRANTYPE " +
            //                       "UNION " +
            //                       "SELECT SHORTNAME,SUM(BUY) BUY,SUM(NETBUYAMOUNT) NETBUYAMOUNT,SUM(SALE) SALE,ROUND(SUM(NETSALEAMOUNT + COMMISSION)/SUM(SALE),4) AVGPRICE , SUM(NETSALEAMOUNT + COMMISSION) AMOUNT, SUM(COMMISSION) COMMISSION, SUM(NETSALEAMOUNT) NETSALEAMOUNT, TRANTYPE  FROM INSTRUMENTLEDGERALL WHERE ACCOUNTNUMBER='00001' AND TRANTYPE='" + ConstantVariable.TranTypeShareSold + "' AND " + condition + " GROUP BY SHORTNAME,TRANTYPE " +
            //                      "ORDER BY TRANTYPE";
                  

            //        OracleCommand cmd = new OracleCommand(query, conn);
            //        OracleDataAdapter da = new OracleDataAdapter(cmd);
            //        DataTable dtPortfolioInstrument = new DataTable();
            //        da.Fill(dtPortfolioInstrument);

            //        foreach (DataRow dr in new DataView(dtPortfolioInstrument).ToTable(true, "SHORTNAME").Rows)
            //        {
            //            //DataRow[] instrumentLedger = dtPortfolioInstrument.Select("SHORTNAME='" + dR["SHORTNAME"].ToString() + "'");
            //            BuySellstatement BuySellLedger = new BuySellstatement();

            //            BuySellCount = 0;


            //            //foreach (DataRow dr in instrumentLedger)
            //            //{
            //                //  BuySellLedger.AccountNumber = dr["ACCOUNTNUMBER"].ToString();
            //                BuySellLedger.TradeCode = dr["SHORTNAME"].ToString();
            //                BuySellLedger.TransactionType = dr["TRANTYPE"].ToString();
                           


            //               BuySellLedger.BuyQty = double.Parse(dr["BUY"].ToString());
            //               BuySellLedger.BuyPrice = double.Parse(dr["AVGPRICE"].ToString());

            //              if(BuySellLedger.TransactionType == ConstantVariable.TranTypeShareBought)
            //               BuySellLedger.BuyAmount = double.Parse(dr["AMOUNT"].ToString());
            //              else
            //                  BuySellLedger.SellAmount = double.Parse(dr["AMOUNT"].ToString());
                           
            //             //  BuySellLedger.Net = double.Parse(dr["NETBUYAMOUNT"].ToString());
            //              // BuySellLedger.Net = BuySellLedger.Net * -1; //because for Buy money is reduced
                           
            //               BuySellLedger.Commission = double.Parse(dr["COMMISSION"].ToString());
                          
                           
            //            //}

            //            BuySellLedger.CommissionPercentage = Convert.ToDouble(Commission);
            //            //BuySellLedger.Gross = BuySellLedger.SellAmount - BuySellLedger.Commission;  //for ProfitLoss Report
            //            //BuySellLedger.NetProfitLoss = BuySellLedger.Gross - BuySellLedger.Commission;

            //            oBuySellStatement.Add(BuySellLedger);
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    string msg = ex.Message;
            //    throw ex;
            //}

            //return oBuySellStatement;

        }
        
        #endregion

        #region SattlemetPayInPayOut<Date 2-2-17>

        public List<SettlementViewModel> GetSattlemetPayInPayOut(string date, string PayOption)
        {


            List<SettlementViewModel> oSettlementStatement = new List<SettlementViewModel>();
            try
            {

                if (string.IsNullOrEmpty(PayOption) && PayOption == null)
                    return null;

                string con = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
             
                using (OracleConnection conn = new OracleConnection(con))
                {

                    #region Query
    

                    #region PayIn_Query
                    //select trade.transactiondate BusinesDate, trade.transactiontype, trade.investoracref,investor.bonumber,broker.cdblid CdblDpId,broker.dseclearingbo,broker.memberid TradeNumber,broker.dseexchangeid, instrument.name,instrument.shortname,instrument.isin,sum(trade.sharequantity) as ShareQuantity 
                    //from trade,instrument,investor ,broker
                    //where trade.instrument_reference=instrument.reference  and trade.investoracref = investor.accountnumber and investor.bonumber = broker.bonumber and trade.transactiondate='6-OCT-16' and trade.transactiontype='S' 
                    //group by(trade.transactiontype,trade.investoracref,instrument.name,instrument.shortname, trade.transactiondate,instrument.isin,investor.bonumber,broker.cdblid,broker.dseclearingbo,broker.memberid,broker.dseexchangeid)
                    //order by (instrument.shortname); 
                    #endregion
                    #region PayOut_Query
                    //select trade.matureddate BusinesDate, trade.transactiontype, trade.investoracref,investor.bonumber,broker.cdblid CdblDpId,broker.dseclearingbo,broker.memberid TradeNumber,broker.dseexchangeid, instrument.name,instrument.shortname,instrument.isin,sum(trade.sharequantity) as ShareQuantity 
                    //from trade,instrument,investor ,broker
                    //where trade.instrument_reference=instrument.reference  and trade.investoracref = investor.accountnumber and investor.bonumber = broker.bonumber and trade.matureddate='2-OCT-16' and trade.transactiontype='B' 
                    //group by(trade.transactiontype,trade.investoracref,instrument.name,instrument.shortname, trade.matureddate,instrument.isin,investor.bonumber,broker.cdblid,broker.dseclearingbo,broker.memberid,broker.dseexchangeid)
                    //order by (instrument.shortname);
                    #endregion

                    // where trade.instrument_reference=instrument.reference  and trade.investoracref = investor.accountnumber and trade.matureddate='6-OCT-16' 
                    // ToDate = DateTime.Now.ToString("dd - MMM - yy");
                    #endregion

                    string selectFrom = "";
                    string Where = "";
                    string group = "";
                    if (PayOption==ConstantVariable.SettlementPayIn)
                    {
                        selectFrom = "trade.transactiondate BusinesDate,";
                        Where = "trade.transactiontype='S' and trade.transactiondate='" + date + "'";
                        group = "trade.transactiondate";
                    }

                    if (PayOption == ConstantVariable.SettlementPayOut)
                    {
                       
                        selectFrom = "trade.matureddate BusinesDate,";
                        Where = "trade.transactiontype='B' and trade.matureddate='" + date + "'";
                        group = "trade.matureddate";
                    }

                    string query = "select " + selectFrom + " trade.transactiontype,trade.investoracref,investor.bonumber,broker.cdblid CdblDpId,broker.dseclearingbo,broker.memberid TradeNumber,broker.dseexchangeid, instrument.name,instrument.shortname,instrument.isin,sum(trade.sharequantity) ShareQuantity from trade,instrument,investor,broker "
                                   + " where trade.instrument_reference=instrument.reference  and trade.investoracref = investor.accountnumber and investor.bonumber = broker.bonumber and "
                                   + Where + " group by(trade.transactiontype,trade.investoracref,investor.bonumber,broker.cdblid,broker.dseclearingbo,broker.memberid,broker.dseexchangeid,instrument.name,instrument.shortname, instrument.isin," + group + ") order by (instrument.shortname)";
                                        

                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dtPayInPayOut = new DataTable();
                    da.Fill(dtPayInPayOut);

                    foreach (DataRow dR in new DataView(dtPayInPayOut).ToTable(true, "SHORTNAME").Rows)
                    {
                        DataRow[] settlementLedger = dtPayInPayOut.Select("SHORTNAME='" + dR["SHORTNAME"].ToString() + "'");
                        SettlementViewModel Settlement = new SettlementViewModel();

                        foreach (DataRow dr in settlementLedger)
                        {
                            Settlement.BusinessDate   = DateTime.Parse(dr["BusinesDate"].ToString());
                            Settlement.transactiontype = dr["transactiontype"].ToString();
                            Settlement.investoracref = dr["investoracref"].ToString();
                            Settlement.bonumber = dr["bonumber"].ToString();
                            

                            Settlement.CdblDpId = dr["CdblDpId"].ToString();
                            Settlement.dseclearingbo = dr["dseclearingbo"].ToString();
                            Settlement.TradeNumber = dr["TradeNumber"].ToString();
                            Settlement.dseexchangeid = dr["dseexchangeid"].ToString();

                            Settlement.Name = dr["name"].ToString();
                            Settlement.shortname = dr["shortname"].ToString();                            
                            Settlement.isin = dr["isin"].ToString();
                           
                            Settlement.ShareQuantity = double.Parse(dr["ShareQuantity"].ToString());                           
                           
                        }
                        oSettlementStatement.Add(Settlement);
                      

                    }

                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }

            return oSettlementStatement;

        }

        #endregion
        

        public double GetClosingPrice(string shortName, string ledgerDate)
        {
            double Marketrate=-1;
            string con = System.Configuration.ConfigurationManager.
            ConnectionStrings["ConnectionString"].ConnectionString;

            using (OracleConnection conn = new OracleConnection(con))
            {
                string marketPriceQuery = "Select CLOSINGPRICE from PRICEINDEX where INSTRUMENTREF = '" + shortName + "' and TRADINGDATE = (SElect max(TRADINGDATE) from PRICEINDEX where INSTRUMENTREF ='" + shortName + "' and Tradingdate <= '" + ledgerDate + "')";
                OracleCommand cmdmarketPriceQuery = new OracleCommand(marketPriceQuery, conn);
                OracleDataAdapter damarketPrice = new OracleDataAdapter(cmdmarketPriceQuery);
                DataTable dtmarketPrice = new DataTable();
                damarketPrice.Fill(dtmarketPrice);

                if (dtmarketPrice.Rows.Count > 0)
                {
                    Marketrate = Convert.ToDouble(dtmarketPrice.Rows[0]["CLOSINGPRICE"].ToString());                   
                }               
            }
            return Marketrate;
        }


        public double GetFractionAmount(string InstrumentRef, string RecordDate)
        {
            double fractionQty = 0;
            string con = System.Configuration.ConfigurationManager.
            ConnectionStrings["ConnectionString"].ConnectionString;
            try
            {
                var _RecordDate = Convert.ToDateTime(RecordDate);
                RecordDate = _RecordDate.ToString("dd-MMM-yy");

                using (OracleConnection conn = new OracleConnection(con))
                {

                    string FractionQuery = "select SHAREQTY from SCRIPTTRANSFER WHERE TRANSACTIONTYPE='F' and RECORDDATE='"+RecordDate+"' and INSTRUMENTACREF='"+ InstrumentRef+"'";
                    OracleCommand cmdmarketPriceQuery = new OracleCommand(FractionQuery, conn);
                    OracleDataAdapter damarketPrice = new OracleDataAdapter(cmdmarketPriceQuery);
                    DataTable dtmarketPrice = new DataTable();
                    damarketPrice.Fill(dtmarketPrice);

                    if (dtmarketPrice.Rows.Count > 0)
                    {
                        fractionQty = Convert.ToDouble(dtmarketPrice.Rows[0]["SHAREQTY"].ToString());
                    }
                }
            }
            catch (Exception)
            { 
            
            }

            return fractionQty;
        }


        public List<DIVIDEND> getDividendList(string Status)
        {
             string CATYPE=null;
             string cashReceivedDate = null;
             string effectiveDate = null;
             DateTime? CashReceivedDate = null;
             DateTime? EffectiveDate = null;

            List<DIVIDEND> model = new List<DIVIDEND>();
            try
            {
                foreach (var list in  getDividendListTable(Status).AsEnumerable().ToList())
                {

                    cashReceivedDate = list["CASHRECEIVEDDATE"].ToString();
                    effectiveDate = list["EFFECTIVEDATE"].ToString();
                    CATYPE = Convert.ToString(list["CATYPE"]);

                    CashReceivedDate = null;
                    EffectiveDate = null;

                    if (cashReceivedDate != null && !string.IsNullOrEmpty(cashReceivedDate))
                    {
                        CashReceivedDate = Convert.ToDateTime(cashReceivedDate);
                    }

                    if (effectiveDate != null && !string.IsNullOrEmpty(effectiveDate))
                    {
                        EffectiveDate = Convert.ToDateTime(effectiveDate);
                    }
                  
                        model.Add(new DIVIDEND
                        {
                            REFERENCE = list["REFERENCE"].ToString(),
                            REMARKS = list["REMARKS"].ToString(),


                            IMPORTDATE = Convert.ToDateTime(Convert.ToString(list["IMPORTDATE"])),
                            BANKNAME = Convert.ToString(list["BANKNAME"]),
                            BOID = Convert.ToString(list["BOID"]),
                            ISIN = Convert.ToString(list["ISIN"]),

                            RECORDDATE = Convert.ToDateTime(Convert.ToString(list["RECORDDATE"])),

                            CASHRECEIVEDDATE = CashReceivedDate,                            
                            EFFECTIVEDATE = EffectiveDate,
                            
                            CASHPAYMENTDATE = Convert.ToDateTime(Convert.ToString(list["CASHPAYMENTDATE"])),

                            PROCFLAG = Convert.ToString(list["PROCFLAG"]),
                            BOHOLDING = Convert.ToDecimal(Convert.ToString(list["BOHOLDING"])),

                            TAXAMOUNT = Convert.ToDecimal(Convert.ToString(list["TAXAMOUNT"])),
                            TAXRATE = Convert.ToDecimal(Convert.ToString(list["TAXRATE"])),
                            NETCASHAMOUNT = Convert.ToDecimal(Convert.ToString(list["NETCASHAMOUNT"])),
                            GROSSCASHAMOUNT = Convert.ToDecimal(Convert.ToString(list["GROSSCASHAMOUNT"])),
                            STATUS = Convert.ToString(list["STATUS"]),
                            CATYPE = Convert.ToString(list["CATYPE"])

                        });
                    }                  
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                model = null;
            }
            return model;

        }

        private DataTable getDividendListTable(string Status)
        {
            string con = System.Configuration.ConfigurationManager.
             ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(con))
            {
                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GET_DIVIDEND_LIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("Status", OracleType.VarChar).Value = Status;
                cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;

                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
         }


        public List<JournalView> getJOURNAL_VIEW()
        {
            //
            List<JournalView> jView = new List<JournalView>();
            try
            {
                string con = System.Configuration.ConfigurationManager.
                ConnectionStrings["ConnectionString"].ConnectionString;

                using (OracleConnection conn = new OracleConnection(con))
                {
                    string marketPriceQuery = "select * from JOURNAL_VIEW";
                    OracleCommand cmdmarketPriceQuery = new OracleCommand(marketPriceQuery, conn);
                    OracleDataAdapter damarketPrice = new OracleDataAdapter(cmdmarketPriceQuery);
                    DataTable dtmarketPrice = new DataTable();
                    damarketPrice.Fill(dtmarketPrice);

                    if (dtmarketPrice.Rows.Count > 0)
                    {

                        foreach (DataRow dR in new DataView(dtmarketPrice).ToTable(true).Rows)
                        {

                            jView.Add(new JournalView
                            {
                                TRANSACTIONDATE = Convert.ToDateTime(dR["TRANSACTIONDATE"]),
                                CREATEDBY = Convert.ToString(dR["CREATEDBY"]),                               
                                DESCRIPTION = Convert.ToString(dR["DESCRIPTION"]),
                                FOLIONUMBER = Convert.ToString(dR["FOLIONUMBER"]),
                                NETAMOUNT = Convert.ToDecimal(dR["NETAMOUNT"]),
                                PAYMENTTYPE = Convert.ToString(dR["PAYMENTTYPE"]),
                                STATUS = Convert.ToString(dR["STATUS"]),

                            });
                        }

                        //jView.Add();
                        // Marketrate = Convert.ToDouble(dtmarketPrice.Rows[0]["CLOSINGPRICE"].ToString());                   
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jView;

        }
   }

    public class SelectedPortfolio {

        public string Instrument { get; set; }
        public double SellLimit { get; set; }
        public string Exception { get; set; }
    }

    public struct ControlSelectedPortfolio {

        public string Message { get; set; }
        public List<SelectedPortfolio> SelectedPortfolio { get; set; }
    
    }

  public class PortfolioViewModel { 
    
   public string  AccountNumber     {get; set;}
   public string  ShortName         {get; set;}	
   public decimal BuyQty            {get; set;}
   public decimal NetBuyAmount      {get; set;}
   public decimal SaleQty           {get; set;}
   public decimal NetSaleAmount     {get; set;}
   public decimal Receivable        {get; set;}
   public decimal Received          {get; set;}
   public decimal Issued            {get; set;}		
   public decimal CurrentBalance    {get; set;}
   public decimal MaturedBalance    {get; set;}
   public decimal MarketPrice       {get; set;}
   public decimal MarketValue       {get; set;}
   public decimal LockinBalance     {get; set;}		
   public decimal AvgPrice          {get; set;}
   public decimal TotalCost         {get; set;}  
   public decimal RealizedGain      {get; set;}
   public decimal UnRealizedGain    {get; set;}
   public decimal PercentageGain    {get; set;}  
  
  //Extra field for  IstrumentLedger
   public DateTime TransactionDate  {get; set;}
   public string   TransactionType  {get; set;}

  }     

}