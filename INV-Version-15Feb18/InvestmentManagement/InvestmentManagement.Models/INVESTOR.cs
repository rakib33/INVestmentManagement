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
    
    public partial class INVESTOR
    {
        public string REFERENCE { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public string CREATEDBY { get; set; }
        public Nullable<System.DateTime> LASTUPDATED { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public string ACCOUNTNUMBER { get; set; }
        public string NAME { get; set; }
        public string BONUMBER { get; set; }
        public string BROKER_REFERENCE { get; set; }
        public string STATUS { get; set; }
        public string ACCOUNTTYPE { get; set; }
    
        public virtual BROKER BROKER { get; set; }
    }
}