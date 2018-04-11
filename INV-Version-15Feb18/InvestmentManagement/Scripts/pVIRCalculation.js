var btnCalculation = document.getElementById('btn-PRIRCalc');
btnCalculation.onclick = function () {
    (function () {

       // alert('Hitted');

        $.get("/Bond/PVIRCalculation?FACEVALUE=" + $("#FACEVALUE").val() + "&BUYINGPRICE=" + $("#BUYINGPRICE").val() + "&PREMIUMPAID=" + $("#PREMIUMPAID").val() + "&ANNUALDAYS=" + $("#ANNUALDAYS").val() +
            "&OFFERRATE=" + $("#OFFERRATE").val() + "&COUPONRATE=" + $("#COUPONRATE").val() + "&TAXRATE=" + $("#TAXRATE").val() + "&DISCOUNT=" + $("#DISCOUNT").val() +
            "&COMMISSION=" + $("#COMMISSION").val() + "&INTERESTMODE=" + $("#INTERESTMODE").val() + "&COMPOUNDINTERESTINTERVAL=" + $("#COMPOUNDINTERESTINTERVAL").val() +
            "&TENURE=" + $("#TENURE").val() + "&TENURETERM=" + $("#TENURETERM").val() + "&MATURITYDATE=" + $("#MATURITYDATE").val() + "&BONDISSUEDATE=" + $("#BONDISSUEDATE").val() +
            "&OPENINGDATE=" + $("#OPENINGDATE").val() + "&FINANCIALINSTITUTION_REFERENCE=" + $("#FINANCIALINSTITUTION_REFERENCE").val() + "", function (data) {


                if (data.message == "ok") {

                    var obj = JSON.parse(data.list);                  

                    //$('#MATURITYDATE').val(obj.MATURITYDATE.val);
                    $('#MATURITYDATE').val(obj.REJECTEDBY);
                    $('#COSTPRICE').val(obj.COSTPRICE);
                    $('#PREMIUMPAID').val(obj.PREMIUMPAID);
                    $('#TOTALPURCHASEAMOUNT').val(obj.TOTALPURCHASEAMOUNT);
                    $('#SOURCETAX').val(obj.SOURCETAX);
                    $('#OTHERCHARGE').val(obj.OTHERCHARGE);
                    $('#TOTALCOMMISSIONGAIN').val(obj.TOTALCOMMISSIONGAIN);
                    //$('#HOLDINGPERIOD').val(obj.HOLDINGPERIOD);
                    //$('#HOLDINGINTERESTPAID').val(obj.HOLDINGINTERESTPAID);
                    $('#GROSSINTEREST').val(obj.GROSSINTEREST);
                    $('#EXCISEDUTY').val(obj.EXCISEDUTY);
                    $('#NETINTEREST').val(obj.NETINTEREST);                 
                   
                }
                else
                    alert(data.message);
            
        });
    })();
};
