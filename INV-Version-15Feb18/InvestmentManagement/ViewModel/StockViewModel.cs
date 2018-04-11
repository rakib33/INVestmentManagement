using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.ViewModel
{
    public class StockViewModel
    {
        public string    SHORTNAME { get; set; }
        public string    ISIN { get; set; }
        public string    CATYPE { get; set; }
        public DateTime  RECORDDATE { get; set; }
        public DateTime  EFFECTIVEDATE { get; set; }
       
        public decimal?  PARHOLDING { get; set; }
        public decimal?  ENTITLEMENT { get; set; }
        public decimal?  FREEBALANCE { get; set; }
        public decimal?  PREMIUM { get; set; }
        public decimal?  CURRENTBALANCE { get; set; }
        public decimal?  MARKETPRICE { get; set; }
        public decimal?  MARKETVALUE { get; set; }     
        public decimal?  AVGCOST { get; set; }
        public string    REMARKS { get; set; }




    }
}