using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentManagement.InvestmentManagement.Models;

namespace InvestmentManagement.App_Code
{
    public class FDRCalculation
    {
        int interval;
        decimal? SumGrossInterest = 0;
        decimal? SumSourceTax = 0;
        decimal? SumNetInterest = 0;
        decimal? LastReceivableAmount = 0;

        decimal? grossInterest = 0;
        decimal? sourceTax = 0;
        decimal? netInterestReceivable = 0;
        decimal? totalReceivableAmount = 0;
        decimal? OneYearInterest = 0;
        decimal? OneMonthInterest = 0;

        DateTime? PrevDate = null;
        decimal? PrevPrinciple;

        int MonthNumber = 0;  
      

        public int GetCompoundInterval(int Tenure, string TenureTerms, string CompoundInterval)
        {
            interval = 0;

            if (CompoundInterval ==ConstantVariable.COMPOUND_INTEREST_QUARTERLY_MSG)
            {
                if (TenureTerms ==ConstantVariable.TENURETERM_MONTHS)
                {

                    interval = (Tenure * ConstantVariable.COMPOUND_INTEREST_QUERTERLY_ID) / 12;  //4

                }

                else if (TenureTerms ==ConstantVariable.TENURETERM_YEARS)
                {
                    interval = Tenure * ConstantVariable.COMPOUND_INTEREST_QUERTERLY_ID;  //bacause 1 years contains 4 querter 

                }


            }
            else if (CompoundInterval ==ConstantVariable.COMPOUND_INTEREST_HALFYEARLY_MSG)
            {
                if (TenureTerms ==ConstantVariable.TENURETERM_MONTHS)
                {

                    interval = (Tenure * ConstantVariable.COMPOUND_INTEREST_HALFYEARLY_ID) / 12;  //2 one years contains 2 half years

                }

                else if (TenureTerms ==ConstantVariable.TENURETERM_YEARS)
                {

                    interval = Tenure * ConstantVariable.COMPOUND_INTEREST_HALFYEARLY_ID;  //bacause 1 years contains 4 querter 
                }

            }
            else if (CompoundInterval ==ConstantVariable.COMPOUND_INTEREST_YEARLY_MSG)
            {
                if (TenureTerms ==ConstantVariable.TENURETERM_MONTHS)
                {

                    interval = Tenure / 12;  //12 month is 1 years 

                }

                else if (TenureTerms ==ConstantVariable.TENURETERM_YEARS)
                {

                    interval = Tenure;
                }

            }
            else if (CompoundInterval ==ConstantVariable.COMPOUND_INTEREST_MONTHLY_MSG)
            {
                if (TenureTerms ==ConstantVariable.TENURETERM_MONTHS)
                {

                    interval = Tenure;  // per one months

                }

                else if (TenureTerms ==ConstantVariable.TENURETERM_YEARS)
                {

                    interval = Tenure * ConstantVariable.COMPOUND_INTEREST_MONTHLY_ID;//12 one years has 12 months
                }

            }

            return interval;
        }

     /// <summary>
        /// if Flat we pass TenureTerms otherswise it null or "" and Interval=1 ClaculationValue helps to calculate flat or compound if Flat ClaculationValue will be the tenure value
        /// if compound it should be Interval Quarterly =4,or Yearly=1,or Monthly=12,or HalfYearly=2 
     /// </summary>
        int i = 1;
        public FDRCalculationValue GetInterestSlab(string FixedDepoReference, decimal? Principle, DateTime? OpeningDate,DateTime? MaturedDate, decimal? RateofInterest, decimal? TaxRate, decimal? ExciseDuty, Decimal? OthersCharge, int Interval, int CalculationValue, string TenureTerms,decimal? AnnualDays)
        {

           // Entities Db = FDiposit.GetConnectionDB();
            

            FDRCalculationValue obj = new FDRCalculationValue();

            //if (Db == null)
            //{
            //    return null;
            //}
            
            // FDInterestIntervalViewModel intervalList = new FDInterestIntervalViewModel();
            List<FDRINTEREST> IntervalList = new List<FDRINTEREST>();
            string Reference = "";
            try
            {
                //check is it has value then it flat
                if (TenureTerms != null && !string.IsNullOrEmpty(TenureTerms))
                {
                    //Flat Has only one row in FDR Interest Table
                    //try
                    //{
                    //    FDRINTEREST oFDRInterest = Db.FDRINTERESTs.Where(t => t.FIXEDDEPOSIT_REFERENCE == FixedDepoReference).SingleOrDefault();
                    //    Reference = oFDRInterest.REFERENCE;
                    //}
                    //catch { }
                    //calculate one month rate of interest
                     OneMonthInterest = (Principle.Value * (RateofInterest.Value / 100)) / Convert.ToDecimal(12);

                     if (TenureTerms == ConstantVariable.TENURETERM_MONTHS)
                     {                        
                         grossInterest = OneMonthInterest * CalculationValue;
                     }

                     else if (TenureTerms == ConstantVariable.TENURETERM_YEARS)
                     {                     
                         grossInterest =(RateofInterest.Value/100) * Principle.Value * CalculationValue;

                     }
                     else if (TenureTerms == ConstantVariable.TENURETERM_DAYS)
                     {
                         //for annual days                      
                         grossInterest = ((Principle.Value *  (RateofInterest.Value / 100))/AnnualDays) * CalculationValue;
                     }


                     //now calculate Source tax .source tax is 10% of GrossInterest                    
                     sourceTax = grossInterest.Value * Convert.ToDecimal(TaxRate / 100);

                     netInterestReceivable = grossInterest.Value - sourceTax.Value - ExciseDuty.Value - OthersCharge.Value;
                     totalReceivableAmount = Principle + netInterestReceivable;

                     obj.SumGrossInterest = grossInterest;
                     obj.SumSourceTax = sourceTax;
                     obj.SumNetInterest = netInterestReceivable;
                     obj.LastReceivableAmount = totalReceivableAmount;

                     IntervalList.Add(new FDRINTEREST
                     {
                         REFERENCE= Reference,
                         FIXEDDEPOSIT_REFERENCE = FixedDepoReference,
                         PRINCIPALAMOUNT = Principle,
                         FROMDATE = OpeningDate.Value,
                         TODATE = MaturedDate,  //3
                         COMPOUNDVALUE = CalculationValue,

                         RATEOFINTEREST = RateofInterest,
                         TAXRATE = TaxRate,
                         EXCISEDUTY = ExciseDuty,
                         OTHERCHARGE = OthersCharge,                       

                         GROSSINTEREST = grossInterest,
                         SOURCETAX = sourceTax,
                         NETINTERESTRECEIVABLE = netInterestReceivable,
                         AMOUNTRECEIVABLE = totalReceivableAmount                        
                         
                     });


               }
                else
                {
                    //try
                    //{
                    //  List<FDRINTEREST> oFDRInterestList = Db.FDRINTERESTs.Where(t => t.FIXEDDEPOSIT_REFERENCE == FixedDepoReference).ToList();
                    //  Reference = oFDRInterestList[0].REFERENCE;
                    //}
                    //catch { }                    

                    for (i = 1; i <= interval; i++)
                    {

                        OneYearInterest = Principle * (RateofInterest / 100);  //this is one years interest
                        grossInterest = OneYearInterest / CalculationValue; //get gross interest 

                        sourceTax = grossInterest * TaxRate / 100;
                        netInterestReceivable = grossInterest - sourceTax;
                        totalReceivableAmount = Principle + netInterestReceivable;

                        SumGrossInterest += grossInterest;
                        SumSourceTax += sourceTax;
                        SumNetInterest += netInterestReceivable;


                        //just assign to excape null value
                        if (i == 1)
                        {
                            PrevDate = OpeningDate;
                            PrevPrinciple = Principle.Value;
                        }
                        else
                        {
                            //get last row matured date from the IntervalList 
                            PrevDate = IntervalList.ToList().Last().TODATE.Value;

                        }

                        obj.SumGrossInterest = SumGrossInterest;
                        obj.SumSourceTax = SumSourceTax;
                        obj.SumNetInterest = SumNetInterest;
                        obj.LastReceivableAmount = totalReceivableAmount;


                        // fromDate ToDate
                        if (CalculationValue == ConstantVariable.COMPOUND_INTEREST_QUERTERLY_ID) // add 3 months to ToDate or matured date and it is the nest interval opening date
                        {
                            MonthNumber = ConstantVariable.COMPOUND_INTEREST_QUERTERLY_ADD_MONTHS;                         
                        }
                        else if (CalculationValue == ConstantVariable.COMPOUND_INTEREST_HALFYEARLY_ID) // add 6 months to ToDate or matured date and it is the nest interval opening date
                        {
                            MonthNumber = ConstantVariable.COMPOUND_INTEREST_HALFYEARLY_ADD_MONTHS;                            
                        }
                        else if (CalculationValue == ConstantVariable.COMPOUND_INTEREST_YEARLY_ID) // add 1 years to ToDate or matured date and it is the nest interval opening date
                        {
                            MonthNumber = ConstantVariable.COMPOUND_INTEREST_YEARLY_ADD_YEARS;                           
                        }
                        else if (CalculationValue == ConstantVariable.COMPOUND_INTEREST_MONTHLY_ID) // add 1 months to ToDate or matured date and it is the nest interval opening date
                        {
                            MonthNumber = ConstantVariable.COMPOUND_INTEREST_MONTHLY_ADD_MONTHS;                           
                        }


                        IntervalList.Add(new FDRINTEREST
                        {
                            FIXEDDEPOSIT_REFERENCE = FixedDepoReference,
                            FROMDATE = (i == 1 ? OpeningDate : PrevDate),
                            TODATE = (i == 1 ? OpeningDate.Value.AddMonths(MonthNumber) : PrevDate.Value.AddMonths(MonthNumber)),
                            COMPOUNDVALUE = CalculationValue,
                            GROSSINTEREST = grossInterest,
                            SOURCETAX = sourceTax,
                            NETINTERESTRECEIVABLE = netInterestReceivable,
                            AMOUNTRECEIVABLE = totalReceivableAmount,
                            TAXRATE = TaxRate,
                            EXCISEDUTY = ExciseDuty,
                            OTHERCHARGE = OthersCharge,
                            PRINCIPALAMOUNT = (i == 1 ? PrevPrinciple : Principle),
                            RATEOFINTEREST = RateofInterest


                        });

                        //next interval principle will be current  ReceivableAmount
                        Principle = totalReceivableAmount;

                    }
                }

                obj.InterestList = IntervalList;

                return obj;
            }
            catch (Exception) { }

            return null;
        }

    }
}