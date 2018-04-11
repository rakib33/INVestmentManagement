using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;

namespace InvestmentManagement.ViewModel
{
    public class ViewModelBase
    {
    
        public List<CURRENCY> Currencies { get; set; }
        public List<FINANCIALINSTITUTION> FinancialInstitutions { get; set; }
        public List<MENU> Menus { get; set; }
       

    }
}