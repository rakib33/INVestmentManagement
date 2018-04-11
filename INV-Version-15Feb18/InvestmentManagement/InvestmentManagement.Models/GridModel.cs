using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentManagement.ViewModel;

namespace InvestmentManagement.Models
{
    
        public class GridModel<T> 
        {
            public int RowsPerPage { get; set; }
            public int TotalPageNo { get; set; }
            public int CurrentPage { get; set; }
            public List<T> DataModel { get; set; }
        }
   
}