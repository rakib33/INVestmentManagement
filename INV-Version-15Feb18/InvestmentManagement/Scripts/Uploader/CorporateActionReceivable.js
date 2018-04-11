
var carSearch = document.getElementById('btn-CARSearch');

carSearch.onclick = function () {
    

    $('.loadingImage').show();

    //alert(@grid.SortColumn);
  
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




    $.get("/CDBLFiles/ListCorporateActionReceivable?startDate=+" + $('#CARstartDate').val() + "&endDate=" + $('#CARendDate').val() + "&acNumber=" + $('#acNumber').val() + "&isin=" + $('#isin').val() + "&CARType=" + $('#CARType').val(), function (data) {
            var getHTML = $(data);
            $('#CorporateActionReceivableWebGrid').empty();

            $('#CorporateActionReceivableWebGrid').html($('#CorporateActionReceivableWebGrid', getHTML));
            ChkAddClass();
            $('.loadingImage').hide();
        });
    
};