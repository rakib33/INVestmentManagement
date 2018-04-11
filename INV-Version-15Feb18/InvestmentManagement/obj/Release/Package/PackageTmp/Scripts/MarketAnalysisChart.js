

var priceIndexList;
var btnSearch = document.getElementById('btn-Chart');
btnSearch.onclick = function () {
    (function() {
        $.get("/MarketAnalysis/PriceIndexList?instrumentName=" + $('#instrumentName').val() + "&fromDate=" + $('#fromDate').val() + "&toDate=" + $('#toDate').val(), function (data) {

            priceIndexList = data;
            (function () {
                google.load('visualization', '1.0', {
                    'packages': ['corechart'],
                    'callback': MarketPriceChart
                });
            })();

        });
    })();
    


};















function MarketPriceChart() {

    var data = new google.visualization.DataTable()

        data.addColumn('string', 'TRADINGDATE');
        data.addColumn('number', 'Price');
        data.addColumn('number', 'HIGHESTPRICE');
        data.addColumn('number', 'LOWESTPRICE');
        data.addColumn('number', 'CLOSINGPRICE');
    

    for (var i = 0; i < priceIndexList.length; i++) {
       
        var dateString = priceIndexList[i].TRADINGDATE.substr(6);
        var currentTime = new Date(parseInt(dateString));
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        var year = currentTime.getFullYear();
        var date = day + "-" + month + "-" + year;
        
        var trDate = moment(currentTime).format("DD-MMM-YY");
        
        data.addRow([trDate, priceIndexList[i].LOWESTPRICE, priceIndexList[i].OPENINGPRICE, priceIndexList[i].CLOSINGPRICE, priceIndexList[i].HIGHESTPRICE]);
        
        }

    var options = {
        legend: 'none',
       // backgroundColor: { stroke: '#000', strokeWidth: 4, fill: '#bbb' },
        //title: 'Most Popular Pies',
        //titleTextStyle: { fontSize: 18 },
        //tooltip: { showColorCode: true },
        //sliceVisibilityThreshold: .10,
        //pieResidueSliceColor: '#109618',
        //pieResidueSliceLabel: 'Everything else',
        is3D: true
    };

    var chart = new google.visualization.CandlestickChart(document.getElementById('MarketPrice'));
    chart.draw(data, options);

}


//Transaction Volume

function TransactionVolumeChart() {
    
    var data = new google.visualization.DataTable()

    data.addColumn('string', 'TRADINGDATE');
    data.addColumn('number', 'VOLUME');



    for (var i = 0; i < priceIndexList.length; i++) {

        var dateString = priceIndexList[i].TRADINGDATE.substr(6);
        var currentTime = new Date(parseInt(dateString));
        

        var trDate = moment(currentTime).format("DD-MMM-YY");

        data.addRow([trDate, priceIndexList[i].VOLUME]);

    }

    var options = {
        legend: 'none',
        
        is3D: true
    };

    var chart = new google.visualization.LineChart(document.getElementById('TransactionValue'));
    chart.draw(data, options);

}

var btnTransactionValue = document.getElementById('iTransactionValue');
btnTransactionValue.onclick = function () {

   

    
    (function() {
        google.load('visualization', '1.0', {
            'packages': ['corechart'],
            'callback': TransactionVolumeChart
        });
    })();



   

};


//Variation

function VariationChart() {
    
    var data = new google.visualization.DataTable()

    data.addColumn('string', 'TRADINGDATE');
    data.addColumn('number', 'VARIATION');



    for (var i = 0; i < priceIndexList.length; i++) {

        var dateString = priceIndexList[i].TRADINGDATE.substr(6);
        var currentTime = new Date(parseInt(dateString));


        var trDate = moment(currentTime).format("DD-MMM-YY");

        data.addRow([trDate, priceIndexList[i].VARIATION]);

    }

    var options = {
        legend: 'none',

        is3D: true
    };

    var chart = new google.visualization.LineChart(document.getElementById('Variation'));
    chart.draw(data, options);

}

var btnTransactionValue = document.getElementById('iVariation');
btnTransactionValue.onclick = function () {




    (function () {
        google.load('visualization', '1.0', {
            'packages': ['corechart'],
            'callback': VariationChart
        });
    })();





};

//Value

function ValueChart() {
    
    var data = new google.visualization.DataTable()

    data.addColumn('string', 'TRADINGDATE');
    data.addColumn('number', 'VOLUME');



    for (var i = 0; i < priceIndexList.length; i++) {

        var dateString = priceIndexList[i].TRADINGDATE.substr(6);
        var currentTime = new Date(parseInt(dateString));


        var trDate = moment(currentTime).format("DD-MMM-YY");

        data.addRow([trDate, priceIndexList[i].VALUE]);

    }

    var options = {
        legend: 'none',

        is3D: true
    };

    var chart = new google.visualization.LineChart(document.getElementById('Value'));
    chart.draw(data, options);

}

var btnTransactionValue = document.getElementById('iValue');
btnTransactionValue.onclick = function () {




    (function () {
        google.load('visualization', '1.0', {
            'packages': ['corechart'],
            'callback': ValueChart
        });
    })();





};




///Hard Coded Data of Dashboard Chart

