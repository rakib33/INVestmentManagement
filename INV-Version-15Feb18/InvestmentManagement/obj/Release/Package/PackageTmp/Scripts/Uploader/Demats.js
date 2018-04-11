var dematsSearch = document.getElementById('btn-DematSearch');

dematsSearch.onclick = function () {
   

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




        $.get("/CDBLFiles/ListDemats?startDate=+" + $('#startDate').val() + "&endDate=" + $('#endDate').val() + "&acNumber=" + $('#acNumber').val() + "&isin=" + $('#isin').val() + "&CAType=" + $('#CAType').val(), function (data) {
            var getHTML = $(data);
            $('#DematsWebGrid').empty();

            $('#DematsWebGrid').html($('#DematsWebGrid', getHTML));
            ChkAddClass();
            $('.loadingImage').hide();
        });
    })();
};