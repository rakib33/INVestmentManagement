//Generate Maturity Date based on Tenure

function maturityDateCalculation() {
    var tenure = document.getElementById('TENURE');
    var tenureTerm = document.getElementById('TENURETERM');
    var openingDate = document.getElementById('OPENINGDATE');
    var matureDate = document.getElementById('MATURITYDATE');
    
    alert(tenureTerm.value);
    if (tenure.value == "") {
        tenureTerm.value = "Months";
        //For Month Mature Date Calucation
        if (tenureTerm.value == "Months") {
            alert('2');
            tenure.value = '4';
            openingDate.value = '12-jun-15';

            var monthToAdd = tenure.value;
            
            var maturedate = new Date(openingDate.value);
            alert(maturedate.toDateString());
            maturedate.setMonth(maturedate.getMonth() + monthToAdd);
            alert(maturedate.toDateString());
            matureDate.value = maturedate.toString();                                

           }



    }
    else {
        alert("Enter a Tenure?");
    }
}