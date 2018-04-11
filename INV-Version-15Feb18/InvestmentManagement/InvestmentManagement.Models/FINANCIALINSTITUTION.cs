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
    
    public partial class FINANCIALINSTITUTION
    {
        public FINANCIALINSTITUTION()
        {
            this.BONDs = new HashSet<BOND>();
            this.FDRNOTEs = new HashSet<FDRNOTE>();
            this.FDRPROPOSALDETAILS = new HashSet<FDRPROPOSALDETAIL>();
            this.FIBRANCHes = new HashSet<FIBRANCH>();
            this.PRIVATEBONDs = new HashSet<PRIVATEBOND>();
            this.FIXEDDEPOSITs = new HashSet<FIXEDDEPOSIT>();
        }
    
        public string REFERENCE { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string SWIFTCODE { get; set; }
        public string INSTITUTIONTYPE { get; set; }
        public string CREATEDBY { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public Nullable<System.DateTime> LASTUPDATED { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public string CONTACTNO { get; set; }
        public string CONTACTPERSON { get; set; }
        public Nullable<decimal> CAPLIMIT { get; set; }
        public Nullable<decimal> NPLPERCENTAGE { get; set; }
        public Nullable<decimal> OTHERCHARGE { get; set; }
        public Nullable<decimal> EXCISEDUTY { get; set; }
        public Nullable<decimal> TAXRATE { get; set; }
        public string CAMELRATING { get; set; }
        public Nullable<decimal> OFFERRATE1 { get; set; }
        public Nullable<decimal> OFFERRATE2 { get; set; }
        public Nullable<decimal> OFFERRATE3 { get; set; }
        public string ADDRESSLINE1 { get; set; }
        public string ADDRESSLINE2 { get; set; }
        public string ISSELECT { get; set; }
    
        public virtual ICollection<BOND> BONDs { get; set; }
        public virtual ICollection<FDRNOTE> FDRNOTEs { get; set; }
        public virtual ICollection<FDRPROPOSALDETAIL> FDRPROPOSALDETAILS { get; set; }
        public virtual ICollection<FIBRANCH> FIBRANCHes { get; set; }
        public virtual ICollection<PRIVATEBOND> PRIVATEBONDs { get; set; }
        public virtual ICollection<FIXEDDEPOSIT> FIXEDDEPOSITs { get; set; }
    }
}
