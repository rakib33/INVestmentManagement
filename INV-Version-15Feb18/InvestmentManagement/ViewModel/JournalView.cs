using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.ViewModel
{
    public class JournalView
    {
        public DateTime? TRANSACTIONDATE { get; set; }
        public string FOLIONUMBER { get; set; }
        public string DESCRIPTION { get; set; }
        public string PAYMENTTYPE { get; set; }
        public string CREATEDBY { get; set; }       
        public string STATUS { get; set; }
        public decimal? NETAMOUNT { get; set; }
     
    }
}