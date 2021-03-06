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
    
    public partial class BROKER
    {
        public BROKER()
        {
            this.INVESTORs = new HashSet<INVESTOR>();
            this.TRADEs = new HashSet<TRADE>();
        }
    
        public string REFERENCE { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public string CREATEDBY { get; set; }
        public Nullable<System.DateTime> LASTUPDATED { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public string MEMBERID { get; set; }
        public string CDBLID { get; set; }
        public string BONUMBER { get; set; }
        public string DSEEXCHANGEID { get; set; }
        public string CSEEXCHANGEID { get; set; }
        public string DSECLEARINGBO { get; set; }
        public string CSECLEARINGBO { get; set; }
        public string NAME { get; set; }
        public Nullable<decimal> COMMISSIONRATE { get; set; }
        public string DEFAULTTRADER { get; set; }
        public Nullable<decimal> MINIMUMAMOUNT { get; set; }
    
        public virtual ICollection<INVESTOR> INVESTORs { get; set; }
        public virtual ICollection<TRADE> TRADEs { get; set; }
    }
}
