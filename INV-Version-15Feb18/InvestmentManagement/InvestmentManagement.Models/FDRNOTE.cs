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
    
    public partial class FDRNOTE
    {
        public FDRNOTE()
        {
            this.CHEQUEDRAWNs = new HashSet<CHEQUEDRAWN>();
        }
    
        public string NOTEID { get; set; }
        public string FDRPROPOSALDETAILSREF { get; set; }
        public string NOTETYPE { get; set; }
        public string FDRNUMBER { get; set; }
        public string FINANCIALINSTITUTION_REFERENCE { get; set; }
        public string BRANCH_REFERENCE { get; set; }
        public Nullable<decimal> PRINCIPALAMOUNT { get; set; }
        public Nullable<decimal> TENURE { get; set; }
        public string TENURETERM { get; set; }
        public Nullable<decimal> OFFERRATE { get; set; }
        public Nullable<decimal> PROPOSEDRATE { get; set; }
        public Nullable<decimal> EXISTINGDEPOSIT { get; set; }
        public Nullable<decimal> CAPLIMIT { get; set; }
        public string CHEQUENO { get; set; }
        public Nullable<System.DateTime> CHEQUEDATE { get; set; }
        public string PROPOSEDACTION { get; set; }
        public string APPROVEDBY { get; set; }
        public Nullable<System.DateTime> APPROVEDDATE { get; set; }
        public string STATUS { get; set; }
        public string REFERENCE { get; set; }
        public string PROPOSALSUMMARY { get; set; }
        public string CREATEDBY { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public Nullable<System.DateTime> LASTUPDATED { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public string FIXEDDEPOSIT_REFERENCE { get; set; }
        public string CHEQUEDRAWNFROM { get; set; }
        public string CONTACTPERSON { get; set; }
        public string SIGNATORY1 { get; set; }
        public string SIGNATORY2 { get; set; }
        public Nullable<decimal> PERCENTAGEOFFDR { get; set; }
        public string INTERESTMODE { get; set; }
        public Nullable<int> ANNUALDAYS { get; set; }
        public string COMPOUNDINTERESTINTERVAL { get; set; }
        public Nullable<System.DateTime> OPENEDDATE { get; set; }
    
        public virtual ICollection<CHEQUEDRAWN> CHEQUEDRAWNs { get; set; }
        public virtual FIXEDDEPOSIT FIXEDDEPOSIT { get; set; }
        public virtual FINANCIALINSTITUTION FINANCIALINSTITUTION { get; set; }
        public virtual FIBRANCH FIBRANCH { get; set; }
    }
}
