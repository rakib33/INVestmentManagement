using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class BondChequeViewModel
    {
        public BOND bonds { get; set; }
        public List<BOND_CHEQUEDRAWN> bondChequelist { get; set; }

    }
}