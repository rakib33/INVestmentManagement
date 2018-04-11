using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class FDRInterestViewModel
    {
        public string REFERENCE { get; set; }
        public string FIXEDDEPOSIT_REFERENCE { get; set; }
        public string PRINCIPALAMOUNT { get; set; }
        
        public Nullable<decimal> RATEOFINTEREST { get; set; }
        public Nullable<System.DateTime> FROMDATE { get; set; }
        public Nullable<System.DateTime> TODATE { get; set; }
        
        public string GROSSINTEREST { get; set; }
        
        public Nullable<decimal> TAXRATE { get; set; }
        
        public string SOURCETAX { get; set; }
       
        public Nullable<decimal> EXCISEDUTY { get; set; }
        public Nullable<decimal> OTHERCHARGE { get; set; }
        
        public string NETINTERESTRECEIVABLE { get; set; }
        public string AMOUNTRECEIVABLE { get; set; }
        
        public Nullable<decimal> COMPOUNDVALUE { get; set; }
    }

    //public class FDRINTEREST_Report
    //{
    // public string  RECEIVEDON { get; set; }
    // public string  GROSSINTEREST 	{ get; set; }
    // public string	SOURCETAX { get; set; }
    // public string	EXCISEDUTY { get; set; }
    // public string	OTHERCHARGE { get; set; }
    // public string	NETINTERESTRECEIVABLE { get; set; }
    // public string	AMOUNTRECEIVABLE { get; set; }
	
    //}

    public class FDRINTEREST_Report
    {
        public string RECEIVEDON { get; set; }
        public decimal GROSSINTEREST { get; set; }
        public decimal SOURCETAX { get; set; }
        public decimal EXCISEDUTY { get; set; }
        public decimal OTHERCHARGE { get; set; }
        public decimal NETINTERESTRECEIVABLE { get; set; }
        public decimal AMOUNTRECEIVABLE { get; set; }

    }
}