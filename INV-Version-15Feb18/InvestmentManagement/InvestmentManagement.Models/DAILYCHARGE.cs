//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InvestmentManagement.InvestmentManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DAILYCHARGE
    {
        public string REFERENCE { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public string CREATEDBY { get; set; }
        public Nullable<System.DateTime> LASTUPDATED { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public Nullable<System.DateTime> IMPORTDATE { get; set; }
        public string TRANSDESCRIPTION { get; set; }
        public Nullable<decimal> TOTALCHARGE { get; set; }
        public Nullable<decimal> TOTALAMOUNT { get; set; }
        public Nullable<decimal> SERVICETAX { get; set; }
        public Nullable<decimal> TRANSVALUE { get; set; }
        public Nullable<decimal> QUANTITY { get; set; }
        public Nullable<decimal> NOOFTRANS { get; set; }
        public string EXCEPTIONDETAILS { get; set; }
        public string STATUS { get; set; }
    }
}
