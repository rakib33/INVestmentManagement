var caSearch = document.getElementById('btn-CASearch');

caSearch.onclick = function () {
   

    $('.loadingImage').show();

    //alert(@grid.SortColumn);
    (function () {
        //var sale = $('#chkSale').checked ? "true" : "false";
        //alert(sale);
        //var buy = $('#chkBuy').checked ? "true" : "false";
        //alert(buy);


        //var sale;
        //var buy;

        //if ($('#chkSale').is(":checked")) {
        //    sale = true;
        //}
        //if ($('#chkBuy').is(":checked")) {
        //    buy = true;
        //}




        $.get("/CDBLFiles/ListCorporateAction?startDate=+" + $('#startDate').val() + "&endDate=" + $('#endDate').val() + "&acNumber=" + $('#acNumber').val() + "&isin=" + $('#isin').val() + "&CAType=" + $('#CAType').val(), function (data) {
            var getHTML = $(data);
            $('#CorporateActionWebGrid').empty();

            $('#CorporateActionWebGrid').html($('#CorporateActionWebGrid', getHTML));
            ChkAddClass();
            $('.loadingImage').hide();
        });
    })();
};




//

var carSearch = document.getElementById('btn-CARSearch');

carSearch.onclick = function () {
    alert('?')

    $('.loadingImage').show();

    //alert(@grid.SortColumn);
    (function () {
        //var sale = $('#chkSale').checked ? "true" : "false";
        //alert(sale);
        //var buy = $('#chkBuy').checked ? "true" : "false";
        //alert(buy);


        //var sale;
        //var buy;

        //if ($('#chkSale').is(":checked")) {
        //    sale = true;
        //}
        //if ($('#chkBuy').is(":checked")) {
        //    buy = true;
        //}




        $.get("/CDBLFiles/ListCorporateActionReceivable?startDate=+" + $('#startDate').val() + "&endDate=" + $('#endDate').val() + "&acNumber=" + $('#acNumber').val() + "&isin=" + $('#isin').val() + "&CAType=" + $('#CAType').val(), function (data) {
            var getHTML = $(data);
            $('#CorporateActionReceivableWebGrid').empty();

            $('#CorporateActionReceivableWebGrid').html($('#CorporateActionReceivableWebGrid', getHTML));
            ChkAddClass();
            $('.loadingImage').hide();
        });
    })();
};