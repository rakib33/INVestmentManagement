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
    
    public partial class CHEQUEDRAWN
    {
        public string REFERENCE { get; set; }
        public string FIXEDDEPOSIT_REFERENCE { get; set; }
        public string FDRNOTE_REFERENCE { get; set; }
        public string CHEQUENO { get; set; }
        public Nullable<decimal> CHEQUEAMOUNT { get; set; }
        public string PROPOSEDACTION { get; set; }
        public string APPROVEDBY { get; set; }
        public Nullable<System.DateTime> APPROVEDDATE { get; set; }
        public string STATUS { get; set; }
        public string CHEQUEDRAWNFROM { get; set; }
        public Nullable<System.DateTime> CHEQUEDATE { get; set; }
        public string STRCHEQUEDATE { get; set; }
    
        public virtual FIXEDDEPOSIT FIXEDDEPOSIT { get; set; }
        public virtual FDRNOTE FDRNOTE { get; set; }
    }
}
