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
    
    public partial class MIGRLOG
    {
        public MIGRLOG()
        {
            this.MIGRLOG1 = new HashSet<MIGRLOG>();
        }
    
        public decimal ID { get; set; }
        public Nullable<decimal> PARENT_LOG_ID { get; set; }
        public System.DateTime LOG_DATE { get; set; }
        public short SEVERITY { get; set; }
        public string LOGTEXT { get; set; }
        public string PHASE { get; set; }
        public Nullable<decimal> REF_OBJECT_ID { get; set; }
        public string REF_OBJECT_TYPE { get; set; }
        public Nullable<decimal> CONNECTION_ID_FK { get; set; }
    
        public virtual ICollection<MIGRLOG> MIGRLOG1 { get; set; }
        public virtual MIGRLOG MIGRLOG2 { get; set; }
    }
}