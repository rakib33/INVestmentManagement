using System;
using System.ComponentModel.DataAnnotations;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class EntityBase
    {
        [Key]
        [ScaffoldColumn(false)]
        public string REFERENCE { get; set; }

        [ScaffoldColumn(false)]
        public string CREATEDBY { get; set; }

        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> CREATEDDATE { get; set; }

        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> LASTUPDATED { get; set; }

        [ScaffoldColumn(false)]
        public string LASTUPDATEDBY { get; set; }
    }
}