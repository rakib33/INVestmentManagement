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
    
    public partial class JOURNALLINE
    {
        public string REFERENCE { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public string CREATEDBY { get; set; }
        public Nullable<System.DateTime> LASTUPDATED { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public string JOURNALHEAD_REFERENCE { get; set; }
        public string NOMINALACCOUNT_REFERENCE { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<decimal> DEBIT { get; set; }
        public Nullable<decimal> CREDIT { get; set; }
        public Nullable<decimal> NETAMOUNT { get; set; }
        public string ACCOUNTREF { get; set; }
    
        public virtual JOURNALHEAD JOURNALHEAD { get; set; }
        public virtual NOMINALACCOUNT NOMINALACCOUNT { get; set; }
    }
}
