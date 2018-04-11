using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.ViewModel
{
    public class InterestIncomeBGTBViewModel
    {
    
    public string BONDID {get;set;}
    public string Auction {get;set;}
    public string BankName {get;set;}
    public DateTime? BondIssueDate {get;set;}  
    public decimal? FACEVALUE {get;set;}  
    public DateTime? MATURITYDATE {get;set;}
    public decimal? COUPONRATE {get;set;}
    public string TENURE {get;set;}
    public string TENURETERM {get;set;}
    public decimal? DUES {get;set;}
    public decimal? ReceivableFrom {get;set;}

    public string DueDate { get; set; }
    public string InterestDueDate1 {get;set;} //-- tow interest Due date
    public string InterestDueDate2 {get;set;} // -- tow interest Due date
     
    public decimal? ReceivedGross {get;set;}
    public decimal? ReceivedSource {get;set;}
    public decimal? ReceivedNetInterest {get;set;}
    public decimal ReceivableUpTo {get;set;}
    
    public decimal FromGross {get;set;}
    public decimal FromSource {get;set;}
    public decimal FromNetInterest {get;set;}

    public string Remarks { get; set; }
    public decimal ANNUALDAYS { get; set; }

    }
}