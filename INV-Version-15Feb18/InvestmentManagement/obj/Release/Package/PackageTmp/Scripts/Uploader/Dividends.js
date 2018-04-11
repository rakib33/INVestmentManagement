var dividendSearch = document.getElementById('btn-DSearch');

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




        $.get("/CDBLFiles/ListDivident?startDate=+" + $('#startDate').val() + "&endDate=" + $('#endDate').val() + "&acNumber=" + $('#acNumber').val() + "&isin=" + $('#isin').val() + "&DType=" + $('#DType').val(), function (data) {
            var getHTML = $(data);
            $('#DividentWebGrid').empty();

            $('#DividentWebGrid').html($('#DividentWebGrid', getHTML));
            ChkAddClass();
            $('.loadingImage').hide();
        });
    //})();
};