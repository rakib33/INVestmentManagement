
$(function () {

    function formatDate(format) {
      
        if (format == 'long') {

            return ('dd-MMM-yy');
        }
    }



    function formatNumber(number) {
        number = number.toFixed(2) + '';
        x = number.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    }


});