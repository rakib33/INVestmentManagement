using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.App_Code
{
    public static class ApplicationVariable
    {
        
        public static int accountPaddingLength = 5;
        public static int chequePaddingLengthLine1 = 37;
        public static int chequePaddingLengthLine2 = 53;
        public static char paddingChar = '0';
        public static char chequePaddingChar = '*';
        public static long fileSize = 51200;
        public static int transactionOurrefPaddingLength = 10;
        public static string databaseDateFormat = "dd-MMM-yy"; // Modified from "MM/dd/yyyy" 28-09-2012
        public static string ApplicationDateFormat = "dd/MM/yyyy";
        public static string ReportDateFormat = "dd-MMM-yyyy";
        public static string AppDecimalNumberFormat = "#,##0.00";
        public static string AppNumberFormat = "#,##0";
        public static string currencySign = "BDT";
        public static bool EnforceDayEnd = true;

        public static bool TrimPaddingCharForTrading = true;
        public static int BranchPrefixLength = 3;
        public static bool MarginInAccountNumber = true;

        public static string MemberCode="DLS";
        public static String GetConnectionString()
        {
            String connectionString = String.Empty;

            try
            {

                //string serverName = App.oHelper.GetKeyValue("servername");
                //string driver = App.oHelper.GetKeyValue("driver");
                //string dbName = App.oHelper.GetKeyValue("databaseName");
                //string dbUser = App.oHelper.GetKeyValue("DBUser");
                //string dbPassword = App.oHelper.GetKeyValue("Password");
                ////connectionString = "Driver={" + driver + "};Server=" + serverName + ";UID=" + dbUser + ";PWD=" + dbPassword + ";Database=" + dbPassword + ";";
               // connectionString = "server=" + serverName + ";database=" + dbName + ";uid=" + dbUser + ";pwd=" + dbPassword + "";

            }
            catch (Exception )
            {

            }

            return connectionString;
        }


        public static string User_SuperAdminUserName = "bosl";

        public static string User_GeneralUserStock = "Imtiaz";
        public static string User_GeneralUserFdr = "Burhan";
        public static string User_GeneralUserBond = "Moshiur";
        public static string User_AdminUserInvestment = "INV-ADMIN";

        #region POST_TO_GENERAL_LEDGER 

        public static string POSTGL_SOURCE_ID = "Investment";
        public static string POSTGL_FROM_NEWFDR = "NewFDR";
        public static string POSTGL_FROM_FIXEDDEPOSIT = "FDRIP";
        public static string POSTGL_FROM_GL = "OracleIntegration"; //when posted from Oracle Integration list(Final Posting to EBS)

        public static string POSTGL_FDR_NUMBERUPDATE_CODE = "1003";
        #endregion

    }
}