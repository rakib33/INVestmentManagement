
//// Price Market List

var btnUploadId = document.getElementById('btn-CARUpload');

function ProgressBar(initVal, maxVal) {
    var bar = document.getElementById('PriceIndexPB');

    var progressBarId = '#PriceIndexPB';
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

btnUploadId.onclick = function () {
    var formInputId = document.getElementById('ImportPriceIndex');
    var progressBarId = document.getElementById('PriceIndexPB');
    var btnUploadId = document.getElementById('btn-CARUpload');



    (function () {

        var formData = new FormData();

        var totalFiles = document.getElementById('ImportPriceIndex').files.length;


        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById('ImportPriceIndex').files[i];
            formData.append('ImportPriceIndex', file);
        }

        if (totalFiles > 0) {
            var xhr = new XMLHttpRequest();



            xhr.onreadystatechange = function () {
                //alert(xhr.status + "  " + xhr.readyState);
                if ((xhr.readyState == 1)) {
                    var initVal = 0;
                    var maxVal = 25;
                    //alert('ready s 1');
                    ProgressBar(initVal, maxVal);
                    //for (initVal; initVal <= maxVal; initVal++) 
                    //{
                    //    var progressBarId = '#PriceIndexPB';
                    //    var bar = $(progressBarId);

                    //    bar.width('100%');
                    //}


                } else if (xhr.readyState == 2) {
                    var initVal = 16;
                    var maxVal = 50;
                    ProgressBar(initVal, maxVal);

                } else if (xhr.readyState == 3) {
                    var initVal = 51;
                    var maxVal = 75;
                    ProgressBar(initVal, maxVal);

                } else if (xhr.readyState == 4 && xhr.status == 200 && xhr.responseText.trim() == "\"Upload Successful\"") {
                    var initVal = 76;
                    var maxVal = 100;
                    ProgressBar(initVal, maxVal);
                    $('#IsSuccess').text('Upload Successful');  //added by rakibul 22-11-16
                    //alert(xhr.responseText);
                    (function () {
                        //alert('calling Function');
                        $.get("/MarketPrice/ListMarketPrice", function (data) {
                            var getHTML = $(data);
                            $('#MarketPriceWebGrid').empty();
                            $('#MarketPriceWebGrid').andSelf().unbind();
                            $('#MarketPriceWebGrid').html($('#MarketPriceWebGrid', getHTML));
                            ChkAddClass();
                            //$('.loadingImage').hide();
                            $('#btn-CARUpload').prop("disabled", true);

                        });
                    })();


                } else if (xhr.readyState == 4 && xhr.status == 200) {
                    var initVal = 0;
                    var maxVal = 0;
                    ProgressBar(initVal, maxVal);

                    //alert(xhr.responseText);


                } else if (xhr.readyState == 4 && xhr.status == 500) {
                    //alert(xhr.response);
                   // $('#IsSuccess').text(xhr.response);  //added by rakibul 22-11-16
                }

                else {
                    alert("Upload Failed due to Server Error!");
                    $('#IsSuccess').text('Upload Failed');  //added by rakibul 22-11-16
                }


            };
            xhr.open('POST', '/MarketPrice/ImportPriceIndex', true);
            xhr.send(formData);
        }
        else {
            alert("Select a File!");
        }
    })();
};



//Price Market List End

//Search Button

var btnSearch = document.getElementById('btn-Search');
btnSearch.onclick = function () {
    $('.loadingImage').show();
    
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




        $.get("/MarketPrice/ListMarketPrice?tradingDate=+" + $('#tradingDate').val() + "&instrument=" + $('#instrument').val(), function (data) {
            var getHTML = $(data);
            $('#MarketPriceWebGrid').empty();

            $('#MarketPriceWebGrid').html($('#MarketPriceWebGrid', getHTML));
            ChkAddClass();
            $('.loadingImage').hide();
        });
    })();

};