namespace InvestmentManagement.InvestmentManagement.Models
{
    public class BalanceSheet
    {
        public string BSTYPE { get; set; }

        public string ACCOUNTTYPE { get; set; }

        public string SUBTITLE { get; set; }

        public string CODE { get; set; }

        public string ACCOUNTTITLE { get; set; }

        public decimal AMOUNT { get; set; }

        public decimal DEBIT { get; set; }

        public decimal CREDIT { get; set; }
    }
}