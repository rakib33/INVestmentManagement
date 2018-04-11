function CheckInterestMode() { 


    //in this method we check is InterestMode Compound or flat None can not be entry
    //if Flat then must has annual days
    //if compound then must have CompoundInterval(Quarterly,monthly,Yearly,HalfYearly)

    var intersetId = document.getElementById('INTERESTMODE');
    var intersetMode = intersetId.options[intersetId.selectedIndex].value;

    var termId = document.getElementById('TENURETERM');
    var tenureTerms = termId.options[termId.selectedIndex].value;

    var IntervalId = document.getElementById('COMPOUNDINTERESTINTERVAL');
    var Interval = IntervalId.options[IntervalId.selectedIndex].value;


    var annualDays = document.getElementById('ANNUALDAYS').value;
    
    //Nan value can not get If NanN * 1=0
    var IntannualDays = parseFloat(annualDays * 1);

    // alert('Intannual Days' + IntannualDays);
    var tenure = document.getElementById('TENURE').value;
    var IntTenure = 0.0;      

    if (intersetMode == "Flat") {

        alert(IntannualDays);

        if (tenureTerms == "Days") {
            //must have annual days
            if (IntannualDays == 0) {
                alert('Annual Days is required!')
                return false;
            }
            if (IntannualDays < 360 || IntannualDays > 366) {

                alert('Annual Days must be 360 to 366!')
                return false;
            }           
        }
        return true;
    }
    else if (intersetMode == "Compound") {
        //must have CompoundInterval
        if (Interval == "Daily") {
            alert(Interval + ' is not available in this regions!');           
        }
        else if (Interval == "Yearly" || Interval == "Quarterly" || Interval == "Monthly" || Interval == "HalfYearly")
        {

            if (isNaN(tenure) || tenure < 1) {
                alert('Tenure is required!');              
            } else {

                if (tenureTerms == "Days") {
                    alert('Days is not supported for Compound!')
                    return false;
                }

                if (Interval == "Quarterly") {
                    //check if terms =Months then is tenure %3=0 if not it is not supported Quarterly
                    if (tenureTerms == "Months") {
                        IntTenure = parseFloat(tenure % 3);
                        if (IntTenure != 0) {
                            alert(tenure + ' is not supported for Quarterly Interval!')                         
                        }
                        else {
                            return true;
                        }
                    } else if (tenureTerms == "Years")
                    {
                        return true;
                    }

                }
                else if (Interval == "HalfYearly") {

                    //check if terms =Months then is tenure %6=0 if not it is not supported HalfYearly
                    if (tenureTerms == "Months") {
                        IntTenure = parseFloat(tenure % 6);
                        if (IntTenure != 0) {
                            alert(tenure + ' is not supported for HalfYearly Interval!')                           
                        }
                        else {
                            return true;
                        }
                    }
                    else if (tenureTerms == "Years") {
                        return true;
                    }
                }

                else if (Interval == "Yearly") {
                    if (tenureTerms == "Months") {
                        IntTenure = parseFloat(tenure % 12);
                        if (IntTenure != 0) {
                            alert(tenure + ' is not supported for Yearly Interval!')
                        }
                        else {
                            return true;
                        }
                    }
                    else if (tenureTerms == "Years") {
                        return true;
                    }

                }

               
            }
        }
        else {

            alert('Please select an Interval.');           
        }
    }
    else {

        alert('Select a Interest Mode!');       
    }

    return false;

}