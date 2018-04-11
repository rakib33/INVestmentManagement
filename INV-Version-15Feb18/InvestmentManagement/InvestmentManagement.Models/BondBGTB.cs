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
    public class BondBGTB
    {
        public List<InterestIncomeBGTBViewModel> GetInterestIncomeBGTB_FN(string BondRef, string ledgerDate,string UpToDate, string Option)
        {
            try
            {

                List<InterestIncomeBGTBViewModel> Result = new List<InterestIncomeBGTBViewModel>();
                string con = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(con))
                {

                    string query = "select * from table(BondIncomeBGTB_FN('" + BondRef + "','" + ledgerDate + "','" + UpToDate + "','" + Option + "'))"; //order by shortname where currentbalance !=0                                      

                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dtPortfolioInstrument = new DataTable();
                    da.Fill(dtPortfolioInstrument);

                    Result = (from DataRow row in dtPortfolioInstrument.Rows
                              select new InterestIncomeBGTBViewModel
                              {

                                  BONDID = row["BONDID"].ToString(),
                                  Auction = row["Auction"].ToString(),
                                  BankName = row["BankName"].ToString(),
                                  BondIssueDate = Convert.ToDateTime(row["BondIssueDate"]),
                                  FACEVALUE = Convert.ToDecimal(row["FACEVALUE"]),
                                  MATURITYDATE = Convert.ToDateTime(row["MATURITYDATE"]),
                                  COUPONRATE = Convert.ToDecimal(row["COUPONRATE"]),
                                  TENURE = row["TENURE"].ToString(),
                                  TENURETERM = row["TENURETERM"].ToString(),
                                  DUES = Convert.ToDecimal(row["DUES"]),
                                  ReceivableFrom = Convert.ToDecimal(row["ReceivableFrom"]),

                                  DueDate = row["DueDate"].ToString(),
                                  //(row["InterestDueDate1"].ToString() !=null && row["InterestDueDate2"].ToString() !=null) ? row["InterestDueDate1"].ToString()+"\n"+row["InterestDueDate2"].ToString() :row["InterestDueDate1"].ToString(),

                                  InterestDueDate1 = row["InterestDueDate1"].ToString(),
                                  InterestDueDate2 = row["InterestDueDate2"].ToString(),

                                  ReceivedGross = Convert.ToDecimal(row["ReceivedGross"]),
                                  ReceivedSource = Convert.ToDecimal(row["ReceivedSource"]),
                                  ReceivedNetInterest = Convert.ToDecimal(row["ReceivedNetInterest"]),
                                  ReceivableUpTo = Convert.ToDecimal(row["ReceivableUpTo"]),
                                  FromGross = Convert.ToDecimal(row["FromGross"]),
                                  FromSource = Convert.ToDecimal(row["FromSource"]),
                                  FromNetInterest = Convert.ToDecimal(row["FromNetInterest"]),
                                  Remarks = row["Remarks"].ToString(),

                              }).ToList();


                }

                return Result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
         
        }

    }
}