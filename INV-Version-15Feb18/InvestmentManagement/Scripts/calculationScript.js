function get(eid) { return document.getElementById(eid); };
(function () {
    $('#chkPrincpleNet').on('change', principalAmountWithNetInterest);
    $('#btn-calc').on('click', calculate());
})();

function principalAmountWithNetInterest() {


    //Submit button Display none 
    document.getElementById('update').style.display = 'none';

    var temp = parseInt($('#temp').val());
    var totalAmount = 0;
    var Principal = parseFloat($('#PRINCIPALAMOUNT').val());


    var netInterestrate = parseFloat($('#NETINTERESTRECEIVABLE1').val());
    if (temp == 0) {


        totalAmount = Principal + netInterestrate;

        $('#PRESENTPRINCIPALAMOUNT').val(totalAmount.toFixed(2));
        $('#temp').val(1);
        $('#chkPrincpleNet').val('true');

    } else {

        $('#PRESENTPRINCIPALAMOUNT').val(Principal.toFixed(2));
        $('#temp').val(0);
        $('#chkPrincpleNet').val('false');
    }

    //do gross Net source amount null on RenewDeposit page while check or uncheck added rakibul date<29th Feb 2016>
    document.getElementById('validation').value = 0;

    document.getElementById("GROSSINTEREST").value = null;
    document.getElementById("SOURCETAX").value = null;
    document.getElementById("NETINTERESTRECEIVABLE").value = null;
    document.getElementById("TOTALAMOUNTRECEIVABLE").value = null;
    document.getElementById("EXCISEDUTY").value =0;
    document.getElementById("OTHERCHARGE").value =0;
    
    //  alert(from);
};




function calculate() {

   // var Principal = parseFloat(get('PRESENTPRINCIPALAMOUNT').value);
   // var rate = parseFloat(get('RATEOFINTEREST').value);
   // var compounds = $('#COMPOUNDINTERESTINTERVAL').val();
   // var termsIndays = 0;
   // var ANNUALDAYS = $('#ANNUALDAYS').val();
   // //alert(ANNUALDAYS);
   // var openingDate = $('#OPENINGDATE').val().split('-');
   // var maturityDate = $('#MATURITYDATE').val().split('-');
   // var openingMonth = openingDate[1];
   // var matureMonth = maturityDate[1];
   // if (openingMonth == "Jan") {
   //     openingDate[1] = 1;
   // }

   // else if (openingMonth == "Feb") {
   //     openingDate[1] = 2;

   // }
   // else if (openingMonth == "Mar") {
   //     openingDate[1] = 3;

   // }
   // else if (openingMonth == "Apr") {
   //     openingDate[1] = 4;

   // }
   // else if (openingMonth == "May") {
   //     openingDate[1] = 5;

   // }
   // else if (openingMonth == "Jun") {
   //     openingDate[1] = 6;

   // }
   // else if (openingMonth == "Jul") {
   //     openingDate[1] = 7;

   // }
   // else if (openingMonth == "Aug") {
   //     openingDate[1] = 8;

   // }
   // else if (openingMonth == "Sep") {
   //     openingDate[1] = 9;

   // }
   // else if (openingMonth == "Oct") {
   //     openingDate[1] = 10;

   // }
   // else if (openingMonth == "Nov") {
   //     openingDate[1] = 11;

   // }
   // else if (openingMonth == "Dec") {
   //     openingDate[1] = 12;

   // }


   // if (matureMonth == "Jan") {
   //     maturityDate[1] = 1;
   // }

   // else if (matureMonth == "Feb") {
   //     maturityDate[1] = 2;

   // }
   // else if (matureMonth == "Mar") {
   //     maturityDate[1] = 3;

   // }
   // else if (matureMonth == "Apr") {
   //     maturityDate[1] = 4;

   // }
   // else if (matureMonth == "May") {
   //     maturityDate[1] = 5;

   // }
   // else if (matureMonth == "Jun") {
   //     maturityDate[1] = 6;

   // }
   // else if (matureMonth == "Jul") {
   //     maturityDate[1] = 7;

   // }
   // else if (matureMonth == "Aug") {
   //     maturityDate[1] = 8;

   // }
   // else if (matureMonth == "Sep") {
   //     maturityDate[1] = 9;

   // }
   // else if (matureMonth == "Oct") {
   //     maturityDate[1] = 10;

   // }
   // else if (matureMonth == "Nov") {
   //     maturityDate[1] = 11;

   // }
   // else if (matureMonth == "Dec") {
   //     maturityDate[1] = 12;

   // }

   // if (openingDate && maturityDate) {
   //     openingDate = new Date(openingDate[2], openingDate[1], openingDate[0]);
   //     maturityDate = new Date(maturityDate[2], maturityDate[1], maturityDate[0]);

   //     termsIndays = Math.abs(maturityDate.getTime() - openingDate.getTime());

   //     termsIndays = Math.ceil(termsIndays / (1000 * 3600 * 24));


   // }

   // var noOfCompounds;
   // if (compounds == 'Daily') {
   //     noOfCompounds = 365;
   // }
   // else if (compounds == 'Monthly') {
   //     noOfCompounds = 12;
   // }

   // else if (compounds == 'Quarterly') {
   //     noOfCompounds = 4;
   // }

   // else if (compounds == 'Yearly') {
   //     noOfCompounds = 1;

   // }

   // else {

   //     alert("please select Compounding Interval");
   //     return;
   // }
   // var TENURETERM = $('#TENURETERM').val();
   // var terms = parseFloat(get('TENURE').value);

   // if (TENURETERM == "Months") {

   //     terms = terms / 12;

   // }


   // if (isNaN(Principal) || isNaN(rate) || isNaN(noOfCompounds) || isNaN(terms)) {
   //     alert('Please fill up the necessary inputs before calculate');
   //     return;
   // }



   // var ammount = Principal * Math.pow((1 + (rate / 100) / noOfCompounds), (noOfCompounds * terms));

   // //Math.pow(A,n); means A^n A to the power n 

   // var interestType = $('#INTERESTMODE').val();
   // var grossInterest;
   // if (interestType == "Flat") {
   //     grossInterest = (Principal * (rate / 100) * termsIndays) / ANNUALDAYS;
   // }
   // else if (interestType == "Compound") {
   //     grossInterest = ammount - Principal;
   // }

   // else {

   //     alert("Please select interest type");
   // }

   // var taxRate = $('#taxRate').val();
   // var sourceTax = (grossInterest * taxRate) / 100;

   // $('#GROSSINTEREST').val(grossInterest.toFixed(2));
   // $('#SOURCETAX').val(sourceTax.toFixed(2));

   // // toFixed means get only two decimal places (1.175).toFixed(2) = 1.18 but  *(5.175).toFixed(2) = 5.17* so always use Math.round(5.175*100)/100
   //// http://stackoverflow.com/questions/21091727/javascript-tofixed-function

   // var tax = sourceTax;
   // var duty = $('#EXCISEDUTY').val();
   // var charge = $('#OTHERCHARGE').val();
   // var netInterestReceivable = grossInterest - tax - duty - charge;
   // //alert(netInterestReceivable);
   // $('#NETINTERESTRECEIVABLE').val(netInterestReceivable.toFixed(2));
   // var totalAmmount = Principal + netInterestReceivable;
   // //alert(totalAmmount);
   // $('#TOTALAMOUNTRECEIVABLE').val(totalAmmount.toFixed(2));
}



function IsMaturedDate() {
    var mDate = document.getElementById('MATURITYDATE').value;
    if (mDate == "") {
        alert("Matured Date is Required!");
        return false;
    }
    return true;
}
//added by rakibul 
function Calculation(from) {  
    
    document.getElementById("GROSSINTEREST").value = "";
    document.getElementById("SOURCETAX").value ="";
    document.getElementById("NETINTERESTRECEIVABLE").value ="";
    document.getElementById("TOTALAMOUNTRECEIVABLE").value = "";

   var result= CheckInterestMode();
 
   if(result == 1) {       

      //if CheckInterest Mode success or return 1 then enable update button at the last 

       var TaxRate = parseFloat(document.getElementById("TAXRATE").value);
       var PrincipleAmount = 0;

       var ExciseDuty = parseFloat(document.getElementById("EXCISEDUTY").value);
       var OthersCharge = parseFloat(document.getElementById("OTHERCHARGE").value);

       if (from == "renew") {

           document.getElementById('validation').value = 1; //this is needed for renew 
           PrincipleAmount = parseFloat(document.getElementById("PRESENTPRINCIPALAMOUNT").value);
           ExciseDuty = $('#EXCISEDUTY').val();
           OthersCharge = $('#OTHERCHARGE').val();
         //  alert(from);
       } else {
           PrincipleAmount = parseFloat(document.getElementById("PRINCIPALAMOUNT").value);

       }
       var Tenure = parseFloat(document.getElementById("TENURE").value);
       var TenureTerms = document.getElementById("TENURETERM").value;
       var InterestMode = document.getElementById("INTERESTMODE").value;
       var CompoundInterestInterval = document.getElementById("COMPOUNDINTERESTINTERVAL").value;
       var AnnualDays = parseFloat(document.getElementById("ANNUALDAYS").value);
       var RateofInterest = parseFloat(document.getElementById("RATEOFINTEREST").value);

      

       var OpeningDate = document.getElementById("RENWALDATE").value;
       //first check all data is valid or null
       var GrossInterest = 0;
       var SumofGrossInterest = 0;
       var SourceTax = 0;
       var SumOfSourceTax = 0;

       var NetInterestReceivable = 0;
       var SumOfNetInterestReceivable = 0;

       var NextPrinciple = 0;
       var TotalAmountReceivable = 0;
       var CompoundInterval = 0;

       var Interval = 0;

       //if (isNaN(ExciseDuty)) {
       //    document.getElementById("EXCISEDUTY").value = 0;

       //}
       //if (isNaN(OthersCharge)) {
       //    document.getElementById("OTHERCHARGE").value = 0;
       //}

       //if (TenureTerms == "" || Tenure < 1 || InterestMode == "" || PrincipleAmount == 0 || isNaN(PrincipleAmount) || isNaN(TaxRate) || isNaN(RateofInterest)) {
       //    alert('Please fill up the necessary inputs before calculate');
       //    return;
       //}

       //if (InterestMode == "Flat" && TenureTerms == "Days") {
       //    //check Annual Days
       //    if (isNaN(AnnualDays))
       //        alert('Annual Days is required !!');
       //    else if (AnnualDays < 360 || AnnualDays > 365)
       //        alert('Annual days must between 360 to 365 !!')
       //    return;
       //}

       //if (InterestMode == "Compound") {
       //    //check Interval
       //    if (CompoundInterestInterval == "")
       //        alert('Please select an Interval!!');
       //    return;
       //}

       //if all is ok then calculate
       //get one month rateofInterest
       var RateofInterestOneYear = parseFloat((PrincipleAmount * (RateofInterest / 100)));
       var RateOfInterest_OneMonth = parseFloat(RateofInterestOneYear / 12);

       //Flat Calculation

     //  alert(InterestMode);

       if (InterestMode == "Flat") {


           if (TenureTerms == "Months") {
               GrossInterest = parseFloat(RateOfInterest_OneMonth * Tenure);
           }

           else if (TenureTerms == "Years") {
               GrossInterest = parseFloat(RateofInterestOneYear * Tenure);

           }
           else if (TenureTerms == "Days") {
               //for annual days                
               var RateofInterestPerDay = parseFloat((PrincipleAmount * (RateofInterest / 100)) / AnnualDays);
               GrossInterest = parseFloat(RateofInterestPerDay * Tenure);
           }


           //now calculate Source tax .source tax is TaxRate% of GrossInterest
           SourceTax = parseFloat(GrossInterest * (TaxRate / 100));

           //calculate Net Interest Recivable =GrossInterest-(sourceTax + ExciesDuties + OthersCharges)
           NetInterestReceivable = parseFloat(GrossInterest - SourceTax - ExciseDuty - OthersCharge);
           TotalAmountReceivable = parseFloat(PrincipleAmount + NetInterestReceivable);

       }
       else if (InterestMode == "Compound") {
           if (TenureTerms == "Days") {
               alert('Days is Not Supported for Compound !!');
               return;
           } else {
               if (CompoundInterestInterval == "Quarterly") //Quarterly
               {
                   CompoundInterval = 4;
                   if (TenureTerms == "Months") {
                       Interval = parseInt((Tenure * 4) / 12);  //4

                   } else if (TenureTerms == "Years") {
                       Interval = parseInt(Tenure * 4);  //bacause 1 years contains 4 querter 
                   }
               }

               else if (CompoundInterestInterval == "HalfYearly") {
                   CompoundInterval = 2;
                   if (TenureTerms == "Months") {
                       Interval = (Tenure * 2) / 12;  //2 one years contains 2 half years
                   } else if (TenureTerms == "Years") {
                       Interval = parseInt(Tenure * 2);  //bacause 1 years contains 2 half years
                   }
               }
               else if (CompoundInterestInterval == "Yearly") {
                   CompoundInterval = 1;
                   if (TenureTerms == "Months") {
                       Interval = parseInt(Tenure / 12);  //12 month is 1 years 

                   } else if (TenureTerms == "Years") {
                      Interval = parseInt(Tenure);
                   }
               }
               else if (CompoundInterestInterval == "Monthly") {
                   CompoundInterval = 12;

                   if (TenureTerms == "Months") {
                       Interval = parseInt(Tenure);  // per one months

                   } else if (TenureTerms == "Years") {
                       Interval = parseInt(Tenure * 12);

                   }
               }


            //   alert('Interval' + Interval);
               //now calculate the compound
               RateofInterestOneYear = 0;
               RateOfInterest_OneMonth = 0;
               GrossInterest = 0;
               SourceTax = 0;
               NetInterestReceivable = 0;
               TotalAmountReceivable = 0;

               var Principle = 0;
               Principle = PrincipleAmount;
               
              // alert(Principle);

               for (var i = 1; i <= Interval; i++) {

                 //  alert(i);
                   try{
                       RateofInterestOneYear = parseFloat(Principle * (RateofInterest / 100));  //this is one years interest
                
                   
                       GrossInterest = parseFloat(RateofInterestOneYear / parseFloat(CompoundInterval)); //get gross interest 
                    
                       SourceTax = parseFloat(GrossInterest * TaxRate / 100);

                       NetInterestReceivable = parseFloat(GrossInterest - SourceTax);

                       TotalAmountReceivable = parseFloat(Principle + NetInterestReceivable);

                       //alert(SourceTax + ' ' + NetInterestReceivable + ' ' + TotalAmountReceivable);

                      SumofGrossInterest += GrossInterest;
                      SumOfSourceTax += SourceTax;
                      SumOfNetInterestReceivable += NetInterestReceivable;

                       Principle =parseFloat(TotalAmountReceivable);

                      // alert(GrossInterest+' S'+SourceTax+'N'+NetInterestReceivable+'amount'+Principle );
                   }
                   catch(err){
                       alert(err.message);
                   }
               }

              // alert('loop close');

               GrossInterest = 0;
               SourceTax = 0;
               NetInterestReceivable = 0;
               TotalAmountReceivable = 0;

               GrossInterest =parseFloat(SumofGrossInterest);
               SourceTax =parseFloat(SumOfSourceTax);
               NetInterestReceivable =parseFloat(SumofGrossInterest - SumOfSourceTax - ExciseDuty - OthersCharge);
               TotalAmountReceivable = parseFloat(PrincipleAmount + NetInterestReceivable);

               //now calculate the compound

           }//else close
       }//Compound Close

    //   alert(GrossInterest+' S'+SourceTax+'Net'+NetInterestReceivable+'tota'+TotalAmountReceivable);

       //Display the data
       document.getElementById("GROSSINTEREST").value=GrossInterest.toFixed(2);
       document.getElementById("SOURCETAX").value=SourceTax.toFixed(2);
       document.getElementById("NETINTERESTRECEIVABLE").value=NetInterestReceivable.toFixed(2);
       document.getElementById("TOTALAMOUNTRECEIVABLE").value = TotalAmountReceivable.toFixed(2);

       //ipdate id RenewDeposit Submit button Id 
       document.getElementById('update').style.display = 'block';

   }// if(result=="true")
    
    }





function CheckInterestMode() { 


    //in this method we check is InterestMode Compound or flat None can not be entry
    //if Flat then must has annual days
    //if compound then must have CompoundInterval(Quarterly,monthly,Yearly,HalfYearly)

    var intersetMode = document.getElementById('INTERESTMODE').value;
    var tenureTerms = document.getElementById('TENURETERM').value;
    var annualDays = document.getElementById('ANNUALDAYS').value;
    var Interval = document.getElementById('COMPOUNDINTERESTINTERVAL').value;

    //Nan value can not get If NanN * 1=0
    var IntannualDays = parseFloat(annualDays * 1);

    // alert('Intannual Days' + IntannualDays);
    var tenure = document.getElementById('TENURE').value;
    var IntTenure = 0.0;

    //IntTenure = parseFloat(tenure % 3);
    //alert('float tenure' +parseFloat(tenure));
    //alert('int annual days'+IntannualDays+'  IntTenure :'+IntTenure);

    if (intersetMode == "Flat" && tenureTerms == "Days") {

        $("#ANNUALDAYS").attr("required", true);
        $("#COMPOUNDINTERESTINTERVAL").attr("required", false);

        //must have annual days
        if (IntannualDays == 0) {
            alert('Annual Days is required!')
            return 0;
        } else if (IntannualDays < 360 || IntannualDays > 366) {

            alert('Annual Days must be 360 to 366!')
            return 0;
        }
    }
    else if (intersetMode == "Compound") {
        //must have CompoundInterval
        if (Interval == "Daily") {
            alert(Interval + ' is not available in this regions!');
            return 0;
        }
        else if (Interval == "Yearly" || Interval == "Quarterly" || Interval == "Monthly" || Interval == "HalfYearly") {

            if (isNaN(tenure) || tenure < 1) {
                alert('Tenure is required!');
                return 0;

            } else {
                if (Interval == "Quarterly") {
                    //check if terms =Months then is tenure %3=0 if not it is not supported Quarterly
                    if (tenureTerms == "Months") {
                        IntTenure = parseFloat(tenure % 3);
                        if (IntTenure != 0) {

                            alert(tenure + ' is not supported for Quarterly Interval!')
                            return 0;
                        }
                    }

                }
                else if (Interval == "HalfYearly") {

                    //check if terms =Months then is tenure %6=0 if not it is not supported HalfYearly
                    if (tenureTerms == "Months") {
                        IntTenure = parseFloat(tenure % 6);
                        if (IntTenure != 0) {

                            alert(tenure + ' is not supported for HalfYearly Interval!')
                            return 0;
                        }
                    }
                }

                else if (Interval == "Yearly") {
                    if (tenureTerms == "Months") {
                        IntTenure = parseFloat(tenure % 12);
                        if (IntTenure != 0) {

                            alert(tenure + ' is not supported for Yearly Interval!')
                            return 0;
                        }
                    }
                    else if (tenureTerms == "Days") {
                        alert('Days is not supported for Compound!')
                        return 0;
                    }

                }
            }
        }
        else {

            alert('Please select an Interval.');
            return 0;
        }



    }
    return 1;

}