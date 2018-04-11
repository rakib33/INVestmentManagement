using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class MetaData
    {
    }

    [MetadataType(typeof(APPLICATIONUSERMetaData))]
    public partial class APPLICATIONUSER
    { }

    public class APPLICATIONUSERMetaData
    {
        [Display(Name = "Name")]
        public string NAME { get; set; }
        [Display(Name = "Password")]
        public string PASSWORD { get; set; }
        [Display(Name = "User Group")]
        public string USERGROUP_REFERENCE { get; set; }
        [Display(Name = "Email Address")]
        public string EMAILADDRESS { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }
        [Display(Name = "Department")]
        public string DEPARTMENT_REFERENCE { get; set; }
        [Display(Name = "User Id")]
        public string USERID { get; set; }

    }


    [MetadataType(typeof(DepartmentMetaData))]
    public partial class DEPARTMENT
    { }

    public class DepartmentMetaData
    {
        [Display(Name = "Name")]
        public string NAME { get; set; }
        [Display(Name = "Code")]
        public string CODE { get; set; }

    }


    [MetadataType(typeof(TRANSACTIONPOSTINGMATRIXMetaData))]
    public partial class TRANSACTIONPOSTINGMATRIX
    { }

    public class TRANSACTIONPOSTINGMATRIXMetaData
    {
        [Display(Name = "Transaction Code")]
        public string TRANSACTIONCODE { get; set; }
         [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }
         [Display(Name = "BS Credit Control")]
        public string BSCREDITCONTROL { get; set; }
         [Display(Name = "BS Debit Control")]
        public string BSDEBITCONTROL { get; set; }
         [Display(Name = "PL Credit Control")]
        public string PLCREDITCONTROL { get; set; }
         [Display(Name = "PL Debit Control")]
        public string PLDEBITCONTROL { get; set; }
         [Display(Name = "Bank Credit Control")]
        public string BANKCREDITCONTROL { get; set; }
         [Display(Name = "Bank Debit Control")]
        public string BANKDEBITCONTROL { get; set; }
         [Display(Name = "Remarks")]
        public string REMARKS { get; set; }

    }



     
       


    [MetadataType(typeof(USERGROUPMetaData))]
    public partial class USERGROUP
    { }

    public class USERGROUPMetaData
    {
        [Display(Name = "Name")]
        public string NAME { get; set; }
        [Display(Name = "Code")]
        public string CODE { get; set; }
        [Display(Name = "Department")]
        public string DEPARTMENT_REFERENCE { get; set; }
        [Display(Name = "Level")]
        public Nullable<decimal> USERLEVEL { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }

    }


    [MetadataType(typeof(COMPANYMetaData))]
    public partial class COMPANY
    { }

    public class COMPANYMetaData
    {
        [Display(Name = "Code")]
        public string CODE { get; set; }
        [Display(Name = "Name")]
        public string NAME { get; set; }
        [Display(Name = "Registration No")]
        public string REGISTRATIONNO { get; set; }
        [Display(Name = "TIN")]
        public string TIN { get; set; }
        [Display(Name = "Fax Number")]
        public string FAXNUMBER { get; set; }
        [Display(Name = "Website")]
        public string WEBSITE { get; set; }
        [Display(Name = "Currency")]
        public string CURRENCY { get; set; }
        [Display(Name = "Address Line 1")]
        public string ADDRESSLINE1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string ADDRESSLINE2 { get; set; }
        [Display(Name = "City")]
        public string CITY { get; set; }
        [Display(Name = "Post Code")]
        public string POSTCODE { get; set; }
        [Display(Name = "Country")]
        public string COUNTRY { get; set; }
        [Display(Name = "Email")]
        public string EMAIL { get; set; }

    }

    [MetadataType(typeof(BROKERMetaData))]
    public partial class BROKER
    { }

    public class BROKERMetaData
    {
        [Display(Name = "Member Id")]
        public string MEMBERID { get; set; }
        [Display(Name = "CDBL Id")]
        public string CDBLID { get; set; }
        [Display(Name = "BO Number")]
        public string BONUMBER { get; set; }
        [Display(Name = "DSE Exchange Id")]
        public string DSEEXCHANGEID { get; set; }
        [Display(Name = "CSE Exchange Id")]
        public string CSEEXCHANGEID { get; set; }
        [Display(Name = "DSE Clearing BO")]
        public string DSECLEARINGBO { get; set; }
        [Display(Name = "CSE Clearing BO")]
        public string CSECLEARINGBO { get; set; }
        [Display(Name = "Name")]
        public string NAME { get; set; }
        [Display(Name = "Commission Rate")]
        public Nullable<decimal> COMMISSIONRATE { get; set; }
        [Display(Name = "Default Trader")]
        public string DEFAULTTRADER { get; set; }

    }

    [MetadataType(typeof(INVESTORMetaData))]
    public partial class INVESTOR
    { }

    public class INVESTORMetaData
    {
        [Display(Name = "Account Number")]
        public string ACCOUNTNUMBER { get; set; }
        [Display(Name = "Name")]
        public string NAME { get; set; }
        [Display(Name = "BO Number")]
        public string BONUMBER { get; set; }
        [Display(Name = "Broker")]
        public string BROKER_REFERENCE { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }
        [Display(Name = "Account Type")]
        public string ACCOUNTTYPE { get; set; }

    }


    [MetadataType(typeof(DEPOSITORYSETTINGMetaData))]
    public partial class DEPOSITORYSETTING
    { }

    public class DEPOSITORYSETTINGMetaData
    {
        [Display(Name = "Code")]
        public string CODE { get; set; }
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }
        [Display(Name = "File Name")]
        public string FILENAME { get; set; }
        [Display(Name = "Charge Rate")]
        public Nullable<decimal> CHARGERATE { get; set; }
        [Display(Name = "Minimum Fee")]
        public Nullable<decimal> MINIMUMFEE { get; set; }
        [Display(Name = "Method Name")]
        public string METHODNAME { get; set; }

    }


    [MetadataType(typeof(STATUSPARAMETERMetaData))]
    public partial class STATUSPARAMETER
    { }

    public class STATUSPARAMETERMetaData
    {
        [Display(Name = "Entity")]
        public string ENTITY { get; set; }
        [Display(Name = "Property")]
        public string PROPERTY { get; set; }
        [Display(Name = "Is Hidden")]
        public string ISHIDDEN { get; set; }
        [Display(Name = "Is Active")]
        public string ISACTIVE { get; set; }
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }

    }


    [MetadataType(typeof(STATUSPARAMETERDETAILMetaData))]
    public partial class STATUSPARAMETERDETAIL
    { }

    public class STATUSPARAMETERDETAILMetaData
    {
        [Display(Name = "Status Parameter")]
        public string STATUSPARAMETER_REFERENCE { get; set; }
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }
        [Display(Name = "Is Hidden")]
        public string ISHIDDEN { get; set; }
        [Display(Name = "Is Active")]
        public string ISACTIVE { get; set; }
        [Display(Name = "Is Default")]
        public string ISDEFAULT { get; set; }
        [Display(Name = "Code")]
        public string CODE { get; set; }
    }

    [MetadataType(typeof(APPLICATIONPARAMETERMetaData))]
    public partial class APPLICATIONPARAMETER
    { }

    public class APPLICATIONPARAMETERMetaData
    {
        [Display(Name = "Entity")]
        public string ENTITY { get; set; }
        [Display(Name = "Property")]
        public string PROPERTY { get; set; }
        [Display(Name = "Is Hidden")]
        public string ISHIDDEN { get; set; }
        [Display(Name = "Is Active")]
        public string ISACTIVE { get; set; }
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }

    }

    [MetadataType(typeof(APPLICATIONPARAMETERDETAILMetaData))]
    public partial class APPLICATIONPARAMETERDETAIL
    { }

    public class APPLICATIONPARAMETERDETAILMetaData
    {
        [Display(Name = "Application Parameter")]
        public string APPLICATIONPARAMETER_REFERENCE { get; set; }
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }
        [Display(Name = "Is Hidden")]
        public string ISHIDDEN { get; set; }
        [Display(Name = "Is Active")]
        public string ISACTIVE { get; set; }
        [Display(Name = "Is Default")]
        public string ISDEFAULT { get; set; }
        [Display(Name = "Code")]
        public string CODE { get; set; }

    }


    [MetadataType(typeof(FIXEDDEPOSITMetaData))]
    public partial class FIXEDDEPOSIT
    { }

    public class FIXEDDEPOSITMetaData
    {
        [Display(Name = "Deposit Number")]
        public string DEPOSITNUMBER { get; set; }
        [Display(Name = "Financial Institution")]
        public string FINANCIALINSTITUTION_REFERENCE { get; set; }
        [Display(Name = "Branch")]
        public string BRANCH_REFERENCE { get; set; }
        [Display(Name = "First Signatory")]
        public string SIGNATORY1 { get; set; }
        [Display(Name = "Second Signatory")]
        public string SIGNATORY2 { get; set; }
        [Display(Name = "Principal Amount")]
        public Nullable<decimal> PRINCIPALAMOUNT { get; set; }
        [Display(Name = "Cheque Date")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CHEQUEDATE { get; set; }
        [Display(Name = "Cheque Reference")]
        public string CHEQUEREFERENCE { get; set; }
        [Display(Name = "Tenure")]
        public Nullable<decimal> TENURE { get; set; }
        [Display(Name = "Tenure Term")]
        public string TENURETERM { get; set; }
        [Display(Name = "Term In Days")]
        public Nullable<decimal> TERMSINDAYS { get; set; }
        [Display(Name = "Interest Receiving Period")]
        public string INTERESTRECEIVINGPERIOD { get; set; }
        [Display(Name = "Maturity Date")]
        public Nullable<System.DateTime> MATURITYDATE { get; set; }
        [Display(Name = "Existing Cap Limit (Core)")]
        public Nullable<decimal> EXISTINGCAPLIMIT { get; set; }
        [Display(Name = "Rate Of Interest")]
        public Nullable<decimal> RATEOFINTEREST { get; set; }
        [Display(Name = "Advanced Interest Rate")]
        public Nullable<decimal> ADVANCEDINTERESTRATE { get; set; }
        [Display(Name = "Interest Type")]
        public string INTERESTMODE { get; set; }
        [Display(Name = "Interest Type")]
        public string COMPOUNDINTERESTTYPE { get; set; }
        [Display(Name = "Compounding Interval")]
        public string COMPOUNDINTERESTINTERVAL { get; set; }
        [Display(Name = "Annual Days")]
        public Nullable<decimal> ANNUALDAYS { get; set; }     
        [Display(Name = "Status")]
        public string STATUS { get; set; }
        [Display(Name = "Accepted By")]
        public string ACCEPTEDBY { get; set; }
        [Display(Name = "Accepted Date")]
        public Nullable<System.DateTime> ACCEPTEDDATE { get; set; }
        [Display(Name = "Rejected By")]
        public string REJECTEDBY { get; set; }
        [Display(Name = "Rejected Date")]
        public Nullable<System.DateTime> REJECTEDDATE { get; set; }
        [Display(Name = "Opening Date")]
        public Nullable<System.DateTime> OPENINGDATE { get; set; }
        [Display(Name = "Renwal Date")]
        public Nullable<System.DateTime> RENWALDATE { get; set; }
        [Display(Name = "Renwal Deposit Number")]
        public string RENEWALDEPOSITNUMBER { get; set; }
        [Display(Name = "Tax Deduction Criteria")]
        public string TAXDEDUCTIONCRITERIA { get; set; }
        [Display(Name = "Holding Period")]
        public Nullable<decimal> HOLDINGPERIOD { get; set; }
        [Display(Name = "Gross Interest")]
        public Nullable<decimal> GROSSINTEREST { get; set; }
        [Display(Name = "Source Tax")]
        public Nullable<decimal> SOURCETAX { get; set; }
        [Display(Name = "Excise Duty")]
        public Nullable<decimal> EXCISEDUTY { get; set; }
        [Display(Name = "Other Charge")]
        public Nullable<decimal> OTHERCHARGE { get; set; }
        [Display(Name = "Net Interest Receivable")]
        public Nullable<decimal> NETINTERESTRECEIVABLE { get; set; }
        [Display(Name = "Present Principal Amount")]
        public Nullable<decimal> PRESENTPRINCIPALAMOUNT { get; set; }
        [Display(Name = "Remarks")]
        public string REMARKS { get; set; }
        [Display(Name = "Actual Interest Received")]
        public Nullable<decimal> ACTUALINTERESTRECEIVED { get; set; }
        [Display(Name = "MR No")]
        public string MRNO { get; set; }
        [Display(Name = "MR Date")]
        public Nullable<System.DateTime> MRDATE { get; set; }
        [Display(Name = "Encashment Date")]
        public Nullable<System.DateTime> ENCASHMENTDATE { get; set; }
        
     
    }


    [MetadataType(typeof(FDRPROPOSALDETAILMetaData))]
    public partial class FDRPROPOSALDETAIL
    { }

    public class FDRPROPOSALDETAILMetaData
    {
        [Display(Name = "FDR Proposal")]
        public string FDRPROPOSAL_REFERENCE { get; set; }
        [Display(Name = "Financial Institution")]
        public string FINANCIALINSTITUTION_REFERENCE { get; set; }
        [Display(Name = "BRANCH")]
        public string BRANCH_REFERENCE { get; set; }
        [Display(Name = "Current Holding")]
        public Nullable<decimal> CURRENTHOLDING { get; set; }
        [Display(Name = "Principal Amount")]
        public Nullable<decimal> PRINCIPALAMOUNT { get; set; }
        [Display(Name = "Existing Caplimit (Core)")]
        public Nullable<decimal> EXISTINGCAPLIMIT { get; set; }
        [Display(Name = "% Of Total FDR")]
        public Nullable<decimal> PERCENTAGEOFTOTALFDR { get; set; }
        [Display(Name = "NPL")]
        public Nullable<decimal> NPL { get; set; }
        [Display(Name = "Terms")]
        public string TERMS { get; set; }
        [Display(Name = "Tenure")]
        public Nullable<decimal> TENURE { get; set; }
        [Display(Name = "Offer Rate")]
        public Nullable<decimal> OFFERRATE { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }
        [ScaffoldColumn(false)]
        public string ACCEPTEDBY { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> ACCEPTEDDATE { get; set; }
        [ScaffoldColumn(false)]
        public string REJECTEDBY { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> REJECTEDDATE { get; set; }

    }

    [MetadataType(typeof(FDRPROPOSALMetaData))]
    public partial class FDRPROPOSAL
    { }

    public class FDRPROPOSALMetaData
    {
        [Display(Name = "Proposal Id")]
        public string PROPOSALID { get; set; }
        [Display(Name = "Name")]
        public string NAME { get; set; }
        [Display(Name = "Remarks")]
        public string REMARKS { get; set; }

    }




    //[MetadataType(typeof(FINANCIALINSTITUTIONMetaData))]
    //public partial class FINANCIALINSTITUTION
    //{ }

    //public class FINANCIALINSTITUTIONMetaData
    //{
        
    //    [Display(Name = "Financial Institution")]
    //    public string NAME { get; set; }
       

    //}






    [MetadataType(typeof(FIBRANCHMetaData))]
    public partial class FIBRANCH
    { }

    public class FIBRANCHMetaData
    {
        [Display(Name = "Code")]
        public string CODE { get; set; }
        [Display(Name = "Routing Number")]
        public string ROUTINGNUMBER { get; set; }
        [Display(Name = "Swift Code")]
        public string SWIFTCODE { get; set; }
        [Display(Name = "District Code")]
        public string DISTRICTCODE { get; set; }
        [Display(Name = "District Name")]
        public string DISTRICTNAME { get; set; }
        [Display(Name = "Address Line1")]
        public string ADDRESSLINE1 { get; set; }
        [Display(Name = "Address Line2")]
        public string ADDRESSLINE2 { get; set; }
        [Display(Name = "Post Code")]
        public string POSTCODE { get; set; }
        [Display(Name = "Contact Person")]
        public string CONTACTPERSON { get; set; }
        [ScaffoldColumn(false)]
        public string FINANCIALINSTITUTION_REFERENCE { get; set; }
        [Display(Name = "Name")]
        public string NAME { get; set; }

    }


    [MetadataType(typeof(INSTRUMENTCATEGORYMetaData))]
    public partial class INSTRUMENTCATEGORY
    { }

    public class INSTRUMENTCATEGORYMetaData
    {
        [Display(Name = "Code")]
        public string CODE { get; set; }
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }
        [Display(Name = "Stock Exchange")]
        public string STOCKEXCHANGE { get; set; }
        [Display(Name = "Allow Netting")]
        public string ALLOWNETTING { get; set; }
        [Display(Name = "Is Nonmarginable")]
        public string ISNONMARGINABLE { get; set; }
        [Display(Name = "Settlement Days")]
        public Nullable<long> SETTLEMENTDAYS { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }

    }


    [MetadataType(typeof(INSTRUMENTMetaData))]
    public partial class INSTRUMENT
    { }

    public class INSTRUMENTMetaData
    {
        [Display(Name = "Instrument Id")]
        public string INSTRUMENTID { get; set; }
        [Display(Name = "CSE Id")]
        public string CSEID { get; set; }
        [Display(Name = "Instrument Type")]
        public string INSTRUMENTTYPE { get; set; }
        [Display(Name = "Short Name")]
        public string SHORTNAME { get; set; }
        [Display(Name = "ISIN")]
        public string ISIN { get; set; }
        [Display(Name = "Name")]
        public string NAME { get; set; }
        [Display(Name = "Instrument Category")]
        public string INSTRUMENTCATEGORY_REFERENCE { get; set; }
        [Display(Name = "Category")]
        public string CATEGORY { get; set; }
        [Display(Name = "Is Nonmarginable")]
        public string ISNONMARGINABLE { get; set; }
        [Display(Name = "Allow Netting")]
        public string ALLOWNETTING { get; set; }
        [Display(Name = "Is Spot")]
        public string ISSPOT { get; set; }
        [Display(Name = "Declaration Date")]
        public Nullable<System.DateTime> DECLARATIONDATE { get; set; }
        [Display(Name = "Face Value")]
        public Nullable<decimal> FACEVALUE { get; set; }
        [Display(Name = "Premium")]
        public Nullable<decimal> PREMIUM { get; set; }
        [Display(Name = "IPO")]
        public Nullable<decimal> IPO { get; set; }
        [Display(Name = "Total Share")]
        public Nullable<decimal> TOTALSHARE { get; set; }
        [Display(Name = "Public Share")]
        public Nullable<decimal> PUBLICSHARE { get; set; }
        [Display(Name = "Market Lot")]
        public Nullable<decimal> MARKETLOT { get; set; }
        [Display(Name = "Net Asset Value")]
        public Nullable<decimal> NETASSETVALUE { get; set; }
        [Display(Name = "Latest EPS")]
        public Nullable<decimal> LATESTEPS { get; set; }
        [Display(Name = "PE Ratio")]
        public Nullable<decimal> PERATIO { get; set; }
        [Display(Name = "Last Market Price")]
        public Nullable<decimal> LASTMARKETPRICE { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }

    }

    [MetadataType(typeof(TRADEMetaData))]
    public partial class TRADE
    { }

    public class TRADEMetaData
    {

        [ScaffoldColumn(false)]
        [Display(Name = "Net Amount")]
        public Nullable<decimal> NETAMOUNT { get; set; }
        //[Display(Name = "Instrument")]
        //public string INSTRUMENT { get; set; }
        public Nullable<decimal> VAT { get; set; }
        [Display(Name = "Instrument Category")]
        public string INSTRUMENTCATEGORY { get; set; }
        [Display(Name = "Realized Gain")]
        public Nullable<decimal> REALIZEDGAIN { get; set; }
        public string BROKERREF { get; set; }
        public string BRANCHREF { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }
        [Display(Name = "Share Quantity")]
        public Nullable<decimal> SHAREQUANTITY { get; set; }
        [Display(Name = "Transaction Time")]
        public Nullable<System.DateTime> TRANSACTIONTIME { get; set; }

        public string MODIFICATIONLOGS { get; set; }
        [Display(Name = "Total Charge")]
        public Nullable<decimal> TOTALCHARGE { get; set; }
        [Display(Name = "Market")]
        public string MARKET { get; set; }
        [Display(Name = "Transaction Date")]
        public Nullable<System.DateTime> TRANSACTIONDATE { get; set; }
        [Display(Name = "Rate")]
        public Nullable<decimal> RATE { get; set; }
        public string ISEXCEPTION { get; set; }
        [Display(Name = "Commision")]
        public Nullable<decimal> COMMISSION { get; set; }
        [Display(Name = "Tax")]
        public Nullable<decimal> TAX { get; set; }

        public string FILENAME { get; set; }

        public Nullable<decimal> INVESTORPAYABLE { get; set; }
        public Nullable<decimal> LAGA { get; set; }
        public string TRANSACTIONTYPE { get; set; }
        public string MEMO { get; set; }
        [Display(Name = "Exception Details")]
        public string EXCEPTIONDETAILS { get; set; }
        public Nullable<decimal> BROKERPAYABLE { get; set; }
        [Display(Name = "Trader")]
        public string TRADER { get; set; }
        [Display(Name = "Instrument")]
        public string INSTRUMENT_REFERENCE { get; set; }
        [Display(Name = "Broker")]
        public string BROKER_REFERENCE { get; set; }
        [Display(Name = "Order No")]
        public string ORDERNO { get; set; }
        [Display(Name = "Total Amount")]
        public Nullable<decimal> TOTALAMOUNT { get; set; }
        public string EXCEPTIONCODE { get; set; }
        public string CONTRACTNO { get; set; }
        public string HOWLA { get; set; }
        public string INVESTORACREF { get; set; }
        public Nullable<decimal> BORN { get; set; }
        public string FILETYPE { get; set; }
        [Display(Name = "Howla Type")]
        public string HOWLATYPE { get; set; }
        public string FOREIGNFLAG { get; set; }
        public string COMPSPOTID { get; set; }
        [Display(Name = "Mature Date")]
        public Nullable<System.DateTime> MATUREDDATE { get; set; }
        public string ISDEALER { get; set; }
        public string AGENCY { get; set; }
        public Nullable<decimal> AGENCYCOMMISION { get; set; }
        [Display(Name = "Stock Exchange")]
        public string STOCKEXCHANGE { get; set; }

    }

    [MetadataType(typeof(PRICEINDEXMetaData))]
    public partial class PRICEINDEX
    { }

    public class PRICEINDEXMetaData
    {
        [Display(Name = "Value")]
        public Nullable<decimal> VALUE { get; set; }
        [Display(Name = "Trading Date")]
        public Nullable<System.DateTime> TRADINGDATE { get; set; }
        [Display(Name = "Number Of Trade")]
        public Nullable<decimal> NOOFTRADES { get; set; }
        [Display(Name = "Instrument Reference")]
        [ScaffoldColumn(false)]
        public string INSTRUMENTREF { get; set; }
        [Display(Name = "Highest Price")]
        public Nullable<decimal> HIGHESTPRICE { get; set; }
        [Display(Name = "Volume")]
        public Nullable<decimal> VOLUME { get; set; }
        //[Display(Name = "Price File Setting Reference")]
        //[ScaffoldColumn(false)]
        //public string PRICEFILESETTINGREF { get; set; }
        [Display(Name = "Lowest Price")]
        public Nullable<decimal> LOWESTPRICE { get; set; }
        [Display(Name = "Market")]
        public string MARKET { get; set; }
        [Display(Name = "Variation")]
        public Nullable<decimal> VARIATION { get; set; }
        [Display(Name = "Closing Price")]
        public Nullable<decimal> CLOSINGPRICE { get; set; }
        [Display(Name = "Opening Price")]
        public Nullable<decimal> OPENINGPRICE { get; set; }

    }

    [MetadataType(typeof(FINANCIALYEARMetaData))]
    public partial class FINANCIALYEAR
    { }

    public class FINANCIALYEARMetaData
    {
        [Display(Name = "Year Index")]
        public Nullable<decimal> YEARINDEX { get; set; }
        [Display(Name = "Data From")]
        public Nullable<System.DateTime> DATEFROM { get; set; }
        [Display(Name = "Date To")]
        public Nullable<System.DateTime> DATETO { get; set; }
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }

    }


    [MetadataType(typeof(PERIODMetaData))]
    public partial class PERIOD
    { }

    public class PERIODMetaData
    {
        [Display(Name = "Financial Year")]
        public string FINANCIALYEAR_REFERENCE { get; set; }
        [Display(Name = "Period Index")]
        public Nullable<decimal> PERIODINDEX { get; set; }
        [Display(Name = "Date From")]
        public Nullable<System.DateTime> DATEFROM { get; set; }
        [Display(Name = "Date To")]
        public Nullable<System.DateTime> DATETO { get; set; }
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }

    }

    [MetadataType(typeof(NOMINALACCOUNTMetaData))]
    public partial class NOMINALACCOUNT
    { }

    public class NOMINALACCOUNTMetaData
    {
        [Display(Name = "Code")]
        public string CODE { get; set; }
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }
        [Display(Name = "Caption")]
        public string CAPTION { get; set; }
        [Display(Name = "Account Type")]
        public string ACCOUNTTYPE { get; set; }
        [Display(Name = "Header")]
        public string HEADER { get; set; }
        [Display(Name = "Sub Header")]
        public string SUBHEADER { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }

    }

    [MetadataType(typeof(CONTROLACCOUNTMetaData))]
    public partial class CONTROLACCOUNT
    { }

    public class CONTROLACCOUNTMetaData
    {
        [Display(Name = "Code")]
        public string CODE { get; set; }
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }
        [Display(Name = "Nominal Account")]
        public string NOMINALACCOUNT_REFERENCE { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }

    }
[MetadataType(typeof (JOURNALHEADMetaData))]
    public partial class JOURNALHEAD
    {}

public class JOURNALHEADMetaData
{
    [Display(Name = "Account Ref")]
    public string ACCOUNTREF { get; set; }
    [Display(Name = "Transaction Date")]
    public Nullable<System.DateTime> TRANSACTIONDATE { get; set; }
    [Display(Name = "Journal Type")]
    public string JOURNALTYPE { get; set; }
    [Display(Name = "Folio Number")]
    public string FOLIONUMBER { get; set; }
    [Display(Name = "Description")]
    public string DESCRIPTION { get; set; }
    [Display(Name = "Financial Year")]
    public string FINANCIALYEAR_REFERENCE { get; set; }
    [Display(Name = "Financial Period")]
    public string FINANCIALPERIOD_REFERENCE { get; set; }
    [Display(Name = "Payment Type")]
    public string PAYMENTTYPE { get; set; }
    [Display(Name = "Remarks")]
    public string REMARKS { get; set; }
}


[MetadataType(typeof(BONDMetaData))]
public partial class BOND
{ }

public class BONDMetaData
{
    [Display(Name = "Bond Id")]
    public string BONDID { get; set; }
    [Display(Name = "Bond Type")]
    public string BONDTYPE { get; set; }
    [Display(Name = "Financial Institution")]
    public string FINANCIALINSTITUTION_REFERENCE { get; set; }
    [Display(Name = "Sequence Number")]
    public string SEQUENCENUMBER { get; set; }
    [Display(Name = "Face Value")]
    public Nullable<decimal> FACEVALUE { get; set; }
    [Display(Name = "Cheque Date")]
    public Nullable<System.DateTime> CHEQUEDATE { get; set; }
    [Display(Name = "Cheque Reference")]
    public string CHEQUEREFERENCE { get; set; }
    [Display(Name = "Bond Issue Date")]
    public Nullable<System.DateTime> BONDISSUEDATE { get; set; }
    [Display(Name = "Tenure")]
    public Nullable<decimal> TENURE { get; set; }
    [Display(Name = "Tenure Term")]
    public string TENURETERM { get; set; }
    [Display(Name = "Term In Days")]
    public Nullable<decimal> TERMSINDAYS { get; set; }
    [Display(Name = "Interest Payment Period")]
    public string INTERESTPAYMENTPERIOD { get; set; }
    [Display(Name = "Maturity Date")]
    public Nullable<System.DateTime> MATURITYDATE { get; set; }
    public Nullable<decimal> AUCTION { get; set; }
    [Display(Name = "Offer Rate")]
    public Nullable<decimal> OFFERRATE { get; set; }
    [Display(Name = "Coupon Rate")]
    public Nullable<decimal> COUPONRATE { get; set; }
    [Display(Name = "Buying Price")]
    public Nullable<decimal> BUYINGPRICE { get; set; }
    [Display(Name = "Cost Price")]
    public Nullable<decimal> COSTPRICE { get; set; }
    [Display(Name = "Premium Paid")]
    public Nullable<decimal> PREMIUMPAID { get; set; }
    [Display(Name = "Holding Interest Paid")]
    public Nullable<decimal> HOLDINGINTERESTPAID { get; set; }
    [Display(Name = "Total Purchase Amount")]
    public Nullable<decimal> TOTALPURCHASEAMOUNT { get; set; }
    [Display(Name = "Total Commission Gain")]
    public Nullable<decimal> TOTALCOMMISSIONGAIN { get; set; }
    [Display(Name = "Interest Mode")]
    public string INTERESTMODE { get; set; }
    [Display(Name = "Gross Interest")]
    public Nullable<decimal> GROSSINTEREST { get; set; }
    [Display(Name = "Source Tax")]
    public Nullable<decimal> SOURCETAX { get; set; }
    [Display(Name = "Excise Duty")]
    public Nullable<decimal> EXCISEDUTY { get; set; }
    [Display(Name = "Other Charge")]
    public Nullable<decimal> OTHERCHARGE { get; set; }
    [Display(Name = "Compound Interest Type")]
    public string COMPOUNDINTERESTTYPE { get; set; }
    [Display(Name = "Compound Interest Interval")]
    public string COMPOUNDINTERESTINTERVAL { get; set; }
    [Display(Name = "Annual Days")]
    public Nullable<decimal> ANNUALDAYS { get; set; }
}


[MetadataType(typeof(FDRNOTEMetaData))]
public partial class FDRNOTE
{ }

public class FDRNOTEMetaData
{
    [Display(Name = "FDR NUMBER")]
    public string FDRNUMBER { get; set; }
    [Display(Name = "FINANCIAL INSTITUTION")]
    public string FINANCIALINSTITUTION_REFERENCE { get; set; }
    [Display(Name = "BRANCH")]
    public string BRANCH_REFERENCE { get; set; }
    [Display(Name = "PRINCIPAL AMOUNT")]
    public Nullable<decimal> PRINCIPALAMOUNT { get; set; }
    [Display(Name = "TENURE")]
    public Nullable<decimal> TENURE { get; set; }
    [Display(Name = "TENURE TERM")]
    public string TENURETERM { get; set; }
    [Display(Name = "PROPOSED RATE")]
    public Nullable<decimal> PROPOSEDRATE { get; set; }
    [Display(Name = "OFFER RATE")]
    public Nullable<decimal> OFFERRATE { get; set; }
    [Display(Name = "EXISTING DEPOSIT")]
    public Nullable<decimal> EXISTINGDEPOSIT { get; set; }
    [Display(Name = "CAP LIMIT (Core)")]
    public Nullable<decimal> CAPLIMIT { get; set; }
    [Display(Name = "% OF FDR")]
    public Nullable<decimal> PERCENTAGEOFFDR { get; set; }
    [Display(Name = "CHEQUE NO")]
    public string CHEQUENO { get; set; }
    [Display(Name = "CHEQUE DATE")]
    public Nullable<System.DateTime> CHEQUEDATE { get; set; }
    [Display(Name = "PROPOSED ACTION")]
    public string PROPOSEDACTION { get; set; }
    [Display(Name = "PROPOSAL SUMMARY")]
    public string PROPOSALSUMMARY { get; set; }
    [Display(Name = "CONTACT PERSON")]
    public string CONTACTPERSON { get; set; }
    [Display(Name = "CHEQUE DRAWN FROM")]
    public string CHEQUEDRAWNFROM { get; set; }
    [Display(Name = "First Signatory")]
    public string SIGNATORY1 { get; set; }
    [Display(Name = "Second Signatory")]
    public string SIGNATORY2 { get; set; }

}

    [MetadataType(typeof(PRIVATEBONDMetaData))]
public partial class PRIVATEBOND
{ }

    public class PRIVATEBONDMetaData
    {
        [Display(Name = "Bond Id")]
        public string BONDID { get; set; }
        [Display(Name = "Name")]
        public string NAME { get; set; }
        [Display(Name = "Instrument")]
        public string INSTRUMENT { get; set; }
        [Display(Name = "Financial Institutionn")]
        public string FINANCIALINSTITUTION_REFERENCE { get; set; }
        [Display(Name = "Bond Size")]
        public Nullable<decimal> BONDSIZE { get; set; }
        [Display(Name = "Purchase Amount")]
        public Nullable<decimal> PURCHASEAMOUNT { get; set; }
        [Display(Name = "Cheque Date")]
        public Nullable<System.DateTime> CHEQUEDATE { get; set; }
        [Display(Name = "Cheque Reference")]
        public string CHEQUEREFERENCE { get; set; }
        [Display(Name = "Bond Issue Date")]
        public Nullable<System.DateTime> BONDISSUEDATE { get; set; }
        [Display(Name = "Tenure")]
        public Nullable<decimal> TENURE { get; set; }
        [Display(Name = "Tenure Term")]
        public string TENURETERM { get; set; }
        [Display(Name = "Term In Days")]
        public Nullable<decimal> TERMSINDAYS { get; set; }
        [Display(Name = "Interest Rate")]
        public Nullable<decimal> INTERESTRATE { get; set; }
        [Display(Name = "Interest Payment Period")]
        public string INTERESTPAYMENTPERIOD { get; set; }
        [Display(Name = "Maturity Date")]
        public Nullable<System.DateTime> MATURITYDATE { get; set; }
        [Display(Name = "Interest Mode")]
        public string INTERESTMODE { get; set; }
        [Display(Name = "Compound Interest Interval")]
        public string COMPOUNDINTERESTINTERVAL { get; set; }
        [Display(Name = "Annual Days")]
        public Nullable<decimal> ANNUALDAYS { get; set; }
        
    }


    public class FIInformation
    {
        public Nullable<decimal> NPL { get; set; }
        public Nullable<decimal> CAPLimit { get; set; }
        public Nullable<decimal> ExitsingDeposit { get; set; }
        public decimal FDRPerentage { get; set; }
        //public List<FIBRANCH> FiBranch { get; set; }

      //  List<SelectListItem> FiBranch { get; set; }
    }

}