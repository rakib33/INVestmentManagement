var TSearch = document.getElementById('btn-TSearch');

TSearch.onclick = function () {
  

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




        $.get("/CDBLFiles/ListTransfer?startDate=+" + $('#startDate').val() + "&endDate=" + $('#endDate').val() + "&acNumber=" + $('#acNumber').val() + "&isin=" + $('#isin').val() + "&TType=" + $('#TType').val(), function (data) {
            var getHTML = $(data);
            $('#ListTransferWebGrid').empty();

            $('#ListTransferWebGrid').html($('#ListTransferWebGrid', getHTML));
            ChkAddClass();
            $('.loadingImage').hide();
        });
    })();
};