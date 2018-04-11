
//function get(eid) { return document.getElementById(eid); };

//(function () {
   
//    $('#btn-mcalc').on('click', calculate);
//    $('#EXCISEDUTY').on('change', netReceivable);
//})();



//        function get(eid) { return document.getElementById(eid); };
//        function calculate() {
            
//    var Principal = parseFloat(get('PRINCIPALAMOUNT').value);
//    var rate = parseFloat(get('RATEOFINTEREST').value);
//    var compounds = $(COMPOUNDINTERESTINTERVAL).val();
//    var termsIndays=0;
//    var ANNUALDAYS = $('#ANNUALDAYS').val();
//    var openingDate = $('#OPENINGDATE').val().split('-');
//    var maturityDate = $('#MATURITYDATE').val().split('-');
//    var openingMonth = openingDate[1];
//    var matureMonth = maturityDate[1];
//    if (openingMonth == "Jan") {
//        openingDate[1] = 1;
//    }

//    else if (openingMonth == "Feb") {
//        openingDate[1] = 2;

//    }
//    else if (openingMonth == "Mar") {
//        openingDate[1] = 3;

//    }
//    else if (openingMonth == "Apr") {
//        openingDate[1] = 4;

//    }
//    else if (openingMonth == "May") {
//        openingDate[1] = 5;

//    }
//    else if (openingMonth == "Jun") {
//        openingDate[1] = 6;

//    }
//    else if (openingMonth == "Jul") {
//        openingDate[1] = 7;

//    }
//    else if (openingMonth == "Aug") {
//        openingDate[1] = 8;

//    }
//    else if (openingMonth == "Sep") {
//        openingDate[1] = 9;

//    }
//    else if (openingMonth == "Oct") {
//        openingDate[1] = 10;

//    }
//    else if (openingMonth == "Nov") {
//        openingDate[1] = 11;

//    }
//    else if (openingMonth == "Dec") {
//        openingDate[1] = 12;

//    }


//    if (matureMonth == "Jan") {
//        maturityDate[1] = 1;
//    }

//    else if (matureMonth == "Feb") {
//        maturityDate[1] = 2;

//    }
//    else if (matureMonth == "Mar") {
//        maturityDate[1] = 3;

//    }
//    else if (matureMonth == "Apr") {
//        maturityDate[1] = 4;

//    }
//    else if (matureMonth == "May") {
//        maturityDate[1] = 5;

//    }
//    else if (matureMonth == "Jun") {
//        maturityDate[1] = 6;

//    }
//    else if (matureMonth == "Jul") {
//        maturityDate[1] = 7;

//    }
//    else if (matureMonth == "Aug") {
//        maturityDate[1] = 8;

//    }
//    else if (matureMonth == "Sep") {
//        maturityDate[1] = 9;

//    }
//    else if (matureMonth == "Oct") {
//        maturityDate[1] = 10;

//    }
//    else if (matureMonth == "Nov") {
//        maturityDate[1] = 11;

//    }
//    else if (matureMonth == "Dec") {
//        maturityDate[1] = 12;

//    }

//    if (openingDate && maturityDate) {
//        openingDate = new Date(openingDate[2], openingDate[1], openingDate[0]);
//        maturityDate = new Date(maturityDate[2], maturityDate[1], maturityDate[0]);

//        termsIndays = Math.abs(maturityDate.getTime() - openingDate.getTime());

//        termsIndays = Math.ceil(termsIndays / (1000 * 3600 * 24));

       
//    }

//    var noOfCompounds;
//    if (compounds == 'Daily') {
//        noOfCompounds = 365;
//    }
//    else if (compounds == 'Monthly') {
//        noOfCompounds = 12;
//    }

//    else if (compounds == 'Quarterly') {
//        noOfCompounds = 4;
//    }

//    else if (compounds == 'Yearly') {
//        noOfCompounds = 1;

//    }

//    else {

//        alert("please select Compounding Interval");
//        return;
//    }
//    var TENURETERM = $('#TENURETERM').val();
//    var terms = parseFloat(get('TENURE').value);

//    if (TENURETERM == "Months") {

//        terms = terms / 12;
        
//    }


//    if (isNaN(Principal) || isNaN(rate) || isNaN(noOfCompounds) || isNaN(terms)) {
//        alert('Please fill up the necessary inputs before calculate');
//        return;
//    }



//    var ammount = Principal * Math.pow((1 + (rate / 100) / noOfCompounds), (noOfCompounds * terms));

//    var interestType = $('#INTERESTMODE').val();
//    var grossInterest;
//    if (interestType == "Flat") {
//        grossInterest = (Principal * (rate / 100) * termsIndays) / ANNUALDAYS;
//    }
//    else if (interestType == "Compound") {
//        grossInterest = ammount - Principal;
//    }

//    else {

//        alert("Please select interest type");
//    }

//    var taxRate = $('#taxRate').val();
//    var sourceTax = (grossInterest * taxRate) / 100;
//    $('#GROSSINTEREST').val(grossInterest.toFixed(2));
//    $('#SOURCETAX').val(sourceTax.toFixed(2));
//    var tax = sourceTax;
//    var duty = $('#EXCISEDUTY').val();
//    var charge = $('#OTHERCHARGE').val();
//    var netInterestReceivable = grossInterest - tax - duty - charge;
//    alert(netInterestReceivable);
//    $('#NETINTERESTRECEIVABLE').val(netInterestReceivable.toFixed(2));
//    var totalAmmount = Principal + netInterestReceivable;
//    $('#TOTALAMOUNTRECEIVABLE').val(totalAmmount.toFixed(2));
//        };

//        function netReceivable() {          

//            var grossInterest = $('#GROSSINTEREST').val();
//            var sourceTax = $('#SOURCETAX').val();
//            var exciseDuty = $('#EXCISEDUTY').val();
//            var otherCharge = $('#OTHERCHARGE').val();
//            var principalAmount = $('#PRINCIPALAMOUNT').val();

//            var netInterest = grossInterest - sourceTax - exciseDuty - otherCharge;

//            var totalAmount = parseFloat(principalAmount) +netInterest;

//           // alert(totalAmount);

//            $('#NETINTERESTRECEIVABLE').val(netInterest.toFixed(2));
//            $('#TOTALAMOUNTRECEIVABLE').val(totalAmount.toFixed(2));

//        }

