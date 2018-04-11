using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.App_Code
{
    
    public static class ConstantVariable
    {
        public static string DELETE_MSG = "Do you want to delete this record parmanently!!";
        public static string DELETE_SUCCESS = "Deleted Success";


        public static string TRUE = "True";

        #region FixedDepositTenureTerms

        public static string TENURETERM_MONTHS = "Months";
        public static string TENURETERM_YEARS = "Years";
        public static string TENURETERM_DAYS = "Days";
        #endregion

        #region FixedDepositInterestMode

        public static string INTERESTMODE_FLAT = "Flat";
        public static string INTERESTMODE_COMPOUND = "Compound";
        #endregion


        #region FixedDepositCompoundInterestInterval

        public static string COMPOUND_INTEREST_MONTHLY_MSG = "Monthly";
        public static int COMPOUND_INTEREST_MONTHLY_ID = 12;
        public static int COMPOUND_INTEREST_MONTHLY_ADD_MONTHS = 1;

        public static string COMPOUND_INTEREST_YEARLY_MSG = "Yearly";
        public static int COMPOUND_INTEREST_YEARLY_ID = 1;
        public static int COMPOUND_INTEREST_YEARLY_ADD_YEARS = 1;

        public static string COMPOUND_INTEREST_HALFYEARLY_MSG = "HalfYearly";
        public static int COMPOUND_INTEREST_HALFYEARLY_ID = 2;
        public static int COMPOUND_INTEREST_HALFYEARLY_ADD_MONTHS = 6;

        public static string COMPOUND_INTEREST_QUARTERLY_MSG = "Quarterly";
        public static int COMPOUND_INTEREST_QUERTERLY_ID = 4;
        public static int COMPOUND_INTEREST_QUERTERLY_ADD_MONTHS = 3;

        public static string COMPOUND_INTEREST_3RD_QUARTERLY_MSG="3rdQuarterly";

        #endregion

        #region GeneratePurchaseNoteLetter_ReportParameter_Variable


        #endregion

        #region FDRPurchaseNoteNew
        public static string FDRPurchaseNoteNew = "FDRPurchaseNoteNew";
        #endregion

        #region DEPOSIT_STATUS

        public static string STATUS_ACTIVE = "Active";
        public static string STATUS_CLOSED = "Closed";
        public static string STATUS_OTC = "OTC";

        public static string STATUS_APPROVED = "Approved";
        public static string STATUS_ENCASHED = "Encashed";
        public static string STATUS_PENDING = "Pending";
        public static string STATUS_REJECTED = "Rejected";
        public static string STATUS_RENEWED = "Renewed";

        public static string STATUS_ACCEPTED = "Accepted";

        public static string STATUS_RECONCILLED = "Reconcilled";

        public static string STATUS_NOTETYPE_NEW = "New";
        public static string STATUS_NOTETYPE_ENCASH = "Encash";
        public static string STATUS_NOTETYPE_RENEWAL = "Renewal";

        public static string STATUS_OPENING = "Opening";
        public static string STATUS_POSTED = "Posted";
        public static string STATUS_OPENING_PORTFOLIO_TRANSACTION_TYPE = "B";

        #endregion

        #region REPORTTEXT
        #region FDREncashmentLetter

        public static string FDREncashLetterP1 = "Please Refer to our Fixed Deposit Receipt No. ";
        public static string FDREncashLetterP2 = " which was opened or renewed ";
        public static string FDREncashLetterP3 = " on for a period of ";
        public static string FDREncashLetterP4 = " with a principal amount of Tk. ";
        public static string FDREncashLetterP5 = " only with your organization.";

        public static string FDREncashLetterP6 = "We don't wish to renew the deposit on its maturity on ";
        public static string FDREncashLetterP7 = " and therefore request you to issue a pay order favouring Delta Life Insurance Co. Ltd. for the total amount deposited plus accumulated interest thereof.";

        #endregion

        #endregion


        #region EQUITY_PORTFOLIO
        public static string INVESTOR_ACCOUNT_NUMBER = "00001";

        #endregion

        #region WEEKEND_MESSAGE
        public static string WEEKEND1_Day = "Saturday";
        public static string WEEKEND2_Day = "Sunday";
        public static string WEEKEND3_Day = "Monday";
        public static string WEEKEND4_Day = "Tuesday";
        public static string WEEKEND5_Day = "Wednesday";
        public static string WEEKEND6_Day = "Thrusday";
        public static string WEEKEND7_Day = "Friday";
        #endregion

        #region ERROR_MSG
        public static string ERROR_OBJECT_REF_NOT_SET_OBJECT = "Object reference not set to an instance of an object.";
        #endregion


        public static string TranTypeShareBought = "Share Bought";
        public static string TranTypeShareSold = "Share Sold";

        public static string RightShare = "RIGHTS";

        public static string SettlementPayIn = "PayIn";
        public static string SettlementPayOut = "PayOut";

        public static string SettlementPayInPrefix = "01";
        public static string SettlementPayOutPrefix = "02";

        public static string SettlementPayInOutFileExt = ".01";

        public static string SettlementPayInTag = "I";
        public static string SettlementPayOutTag = "O";
        public static string SettlementpayInOutSpace = "           ";

       // public static string SettlementPayInFirstLineFirstPart = "00000770000000334995 ";   //must include space in suffix

      //  public static string SettlementPayOutFirstLineFirstPart = "00001220000000471466 ";  //must include space in suffix

        public static string SettlementAdminMsg = "admin";

        public static string CDBL_DP_ID = "34200";

        public static string Settlement_Success_msg = "Success";


        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        public static HoldingInterestModel GetHoldingPeriod(DateTime OpeningDate, DateTime IssueDate,decimal faceValue,decimal CouponRate,decimal annualdays)
        {
            HoldingInterestModel model =new HoldingInterestModel();
            int TotalMonth = GetMonthDifference(OpeningDate, IssueDate);
            int mod = TotalMonth % 6;
            int result = TotalMonth / 6;

      

            if (mod > 0)
            {
           
                DateTime lastInterestSlotDate = IssueDate.AddMonths(result * 6);
                model.holdingPeriod = (int)(OpeningDate - lastInterestSlotDate).TotalDays;

                result = 0;
                //get total days of Opening/Buying Date Interest Slot 6 month slot in result
                result = (int)(lastInterestSlotDate.AddMonths(6) -lastInterestSlotDate).TotalDays; 
                model.PerDayInterest = (faceValue * (CouponRate/100)) / annualdays;

               

                //get per day interest of opening Date interest Slot
                //model.HalfYearInterest / result;
                //model.HoldingInterest = model.holdingPeriod * model.PerDayInterest;
                model.HoldingInterest = (((faceValue * (CouponRate / 100)) / 2)/result) * model.holdingPeriod;
                 

            }
            else
            {
                if (result == 1)
                {
                    result = (int)(IssueDate.AddMonths(6) - IssueDate).TotalDays;
                    model.holdingPeriod = (int)(OpeningDate - IssueDate).TotalDays;
                    model.HoldingInterest = (((faceValue * (CouponRate / 100)) / 2) / result) * model.holdingPeriod;

                }
                else
                {
                    model.holdingPeriod = 0;
                    model.HoldingInterest = 0;
                }
            }

            return model;        
        }


       
    }

    public struct HoldingInterestModel
    {

        public int holdingPeriod { set; get; }
        public decimal HoldingInterest { set; get; }
        public decimal PerDayInterest { get; set; }

    }


}