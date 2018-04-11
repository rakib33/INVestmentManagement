var dividendSearch = document.getElementById('btn-IPOSearch');

dividendSearch.onclick = function () {


    $('.loadingImage').show();

    //alert(@grid.SortColumn);
    //(function () {
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




    $.get("/CDBLFiles/ListIPO?startDate=+" + $('#startDate').val() + "&endDate=" + $('#endDate').val() + "&acNumber=" + $('#acNumber').val() + "&isin=" + $('#isin').val() + "&DType=" + $('#DType').val(), function (data) {
        var getHTML = $(data);
        $('#IPOWebGrid').empty();

        $('#IPOWebGrid').html($('#IPOWebGrid', getHTML));
        ChkAddClass();
        $('.loadingImage').hide();
    });
    //})();
};