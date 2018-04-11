 //Trade  List Start
var btnTradeId = document.getElementById('btn-TradeUpload');

function ProgressBar(initVal, maxVal) {
    var bar = document.getElementById('TradePB');

    var progressBarId = '#TradePB';
    var bar = $(progressBarId);

    for (initVal; initVal <= maxVal; initVal++) {


        bar.width(initVal + '%');
        bar.html(initVal + '%');
    }
    //for (initVal; initVal <= maxVal; initVal++) {

    //    //alert(bar.className);
    //    alert('in pb');
    //    bar.style.width(initVal + '%');
    //    //bar.html(initVal + '%');
    //}

}

btnTradeId.onclick = function () {
    var formInputId = document.getElementById('ImportTrade');
    var progressBarId = document.getElementById('TradePB');
    var btnUploadId = document.getElementById('btn-TradeUpload');

   

    (function () {

        var formData = new FormData();

        var totalFiles = document.getElementById('ImportTrade').files.length;
        var fileName = document.getElementById('ImportTrade').nodeValue;
        
        //var stockExchange=

        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById('ImportTrade').files[i];
            
            formData.append('ImportTrade', file);
        }

        if (totalFiles > 0) {
            var xhr = new XMLHttpRequest();

            console.log('1');

            xhr.onreadystatechange = function () {
                //alert(xhr.status + "  " + xhr.readyState);
                if ((xhr.readyState == 1)) {
                    var initVal = 0;
                    var maxVal = 25;

                    ProgressBar(initVal, maxVal);
                    //for (initVal; initVal <= maxVal; initVal++) 
                    //{
                    //    var progressBarId = '#PriceIndexPB';
                    //    var bar = $(progressBarId);

                    //    bar.width('100%');
                    //}
                    console.log('2');

                } else if (xhr.readyState == 2) {

                    console.log('3');
                    var initVal = 16;
                    var maxVal = 50;
                    ProgressBar(initVal, maxVal);

                } else if (xhr.readyState == 3) {
                    console.log('4');
                    var initVal = 51;
                    var maxVal = 75;
                    ProgressBar(initVal, maxVal);

                } else if (xhr.readyState == 4 && xhr.status == 200 && xhr.responseText.trim() == "\"Upload Successful\"") {

                    console.log('5');

                    var initVal = 76;
                    var maxVal = 100;
                    ProgressBar(initVal, maxVal);
                    $('#IsTradeUpload').text('Upload Successful'); //added by rakibul 22-11-16                    
                    //alert(xhr.responseText);
                    (function () {
                        //alert('calling Function');
                        $.get("/Trade/ListTrade", function (data) {
                           
                        
                            var getHTML = $(data);
                            $('#TradeWebGrid').empty();
                            $('#TradeWebGrid').andSelf().unbind();
                            $('#TradeWebGrid').html($('#TradeWebGrid', getHTML));
                            ChkAddClass();
                            //$('.loadingImage').hide();
                            $('#btn-TradeUpload').prop("disabled", true);

                        });
                    })();


                } else if (xhr.readyState == 4 && xhr.status == 200) {

                    console.log('6');
                    var initVal = 0;
                    var maxVal = 0;
                    ProgressBar(initVal, maxVal);

                    $('#IsTradeUpload').text(xhr.responseText.trim());
                    alert(xhr.responseText);


                } else if (xhr.readyState == 4 && xhr.status == 500) {

                    console.log('7');
                    $('#IsTradeUpload').text(xhr.responseText.trim());
                    alert(xhr.response);

                }
                else if (xhr.responseText.trim() == "\"Please Select a Stock Exchage(DSE or CSE)\"")
                {
                    $('#IsTradeUpload').text('Please Select a Stock Exchage(DSE or CSE)'); //added by rakibul 21-12-16          
                }
                else {
                    alert("Upload Failed due to Server Error!");
                    $('#IsTradeUpload').text('Upload Failed'); //added by rakibul 22-11-16
                }


            };
            xhr.open('POST', '/Trade/ImportTrade?stockExchange=' + $('#stockExchange').val() + '&broker=' + $('#broker').val(), true);
            xhr.send(formData);
        }
        else {
            alert("Select a File!");
        }
    })();
};
//Trade Upload List End


//Search Button Area Start

var btnSearch = document.getElementById('btn-Search');
btnSearch.onclick = function () {
    $('.loadingImage').show();
    $('#btn-TradeUpload').prop("disabled", true);
    //alert(@grid.SortColumn);
    (function () {
        //var sale = $('#chkSale').checked ? "true" : "false";
        //alert(sale);
        //var buy = $('#chkBuy').checked ? "true" : "false";
        //alert(buy);


        var sale;
        var buy;

        if ($('#chkSale').is(":checked")) {
            sale = true;
        }
        if ($('#chkBuy').is(":checked")) {
            buy = true;
        }

        


        $.get("/Trade/ListTrade?tradingDate=+" + $('#datepicker').val() + "&stockExchangeSA=" + $('#stockExchangeSA').val() + "&brokerSA=" + $('#brokerSA').val() + "&instrument=" + $('#instrument').val() + "&investorAccount=" + $('#investorAccount').val() + "&chkSale=" + sale + "&chkBuy=" + buy, function (data) {
            var getHTML = $(data);
            $('#TradeWebGrid').empty();

            $('#TradeWebGrid').html($('#TradeWebGrid', getHTML));
            ChkAddClass();
            $('.loadingImage').hide();
        });
    })();
   
};
//Search Button Area End