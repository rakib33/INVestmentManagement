/// <reference path="jquery-2.0.3.min.js" />
/// <reference path="../Scripts/Controls/jquery-ui.min.js" />
/// <reference path="Controls/date.js" />
$(function () {

    //$(".ddclass").bind("focus", function () {
    //    alert('1');
    //    $(this).datepicker();
    //    alert('2');
    //});

    function Add() {

        //$.datepicker.setDefaults({ dateFormat: 'dd-mm-yy' });
        //$('.datepicker').datepicker();
        var newRow = $('#initRow tbody>tr:first').clone(true);
        //alert(newRow.html());

        //$('input', newRow).val('').
        //      filter('.hasDatepicker').removeClass('hasDatepicker').datepicker();
        $('#tblGrid tbody').append(newRow);
        //$('#tblGrid tbody').prepend(newRow);
        newRow.appendTo('#tblGrid tbody').show('slow');

        debugger;
        //var row = $(newRow).closest('tr');

        //var prev = row.prev();      

        //var par = row.prev(); //tr

        //var tdNoiminal = par.children("td:nth-child(1)");
        //var ddlNoiminal = par.children("td:nth-child(2)");
        //var tdDESCRIPTION = par.children("td:nth-child(3)");
        //var tdDEBIT = par.children("td:nth-child(4)");
        //var tdCREDIT = par.children("td:nth-child(5)");
        //var tdNETAMOUNT = par.children("td:nth-child(6)");
        //var tdIcon = par.children("td:nth-child(7)");
        //tdNoiminal.html(tdNoiminal.children("select").text());
        //ddlNoiminal.html(ddlNoiminal.children("select").val());
        //tdDESCRIPTION.html(tdDESCRIPTION.children("input[type=text]").val());
        //tdDEBIT.html(tdDEBIT.children("input[type=text]").val());
        //tdCREDIT.html(tdCREDIT.children("input[type=text]").val());
        //tdNETAMOUNT.html(tdNETAMOUNT.children("input[type=text]").val());
        //tdIcon.html("<img src='../../images/update.png' class='btnEdit'/><img src='../../images/delete.png' class='btnDelete'/>");


        $('select', newRow).filter('.dropDown').attr('id', 'o1');
        $('div', newRow).filter('.chzn-container').attr('id', 'o1');

        $(".btnEdit").bind("click", Edit);
        $(".btnDelete").bind("click", Delete);
        (function() {
            datePickerBind();
        })();
        //$('.ddclass').datepicker();
            //$("#initRow").delegate(".datepicker", "focusin", function () { $(this).datepicker(); });
            //
        

        //$("#btnAdd").unbind("click", Add);
        //$('#btnAdd').disable(true);
    };

    function Edit() {



        //tdACCOUNTREF.html(tdACCOUNTREF.children("input[type=text]").val());

        //tdDESCRIPTION.html(tdDESCRIPTION.children("input[type=text]").val());
        //tdDEBIT.html(tdCREDIT.children("input[type=text]").val());

        //tdCREDIT.html(tdCREDIT.children("input[type=text]").val());
        //tdNoiminal.html(tdNoiminal.children("select").text());
        //tdNETAMOUNT.html(tdNETAMOUNT.children("input[type=text]").val());

        var par = $(this).parent().parent(); //tr
        //alert(par.html());
        var tdNoiminal = par.children("td:nth-child(1)");
        var nominal = $('#ddlNoiminal').html();
        var tdDESCRIPTION = par.children("td:nth-child(3)");
        var tdDEBIT = par.children("td:nth-child(4)");
        var tdCREDIT = par.children("td:nth-child(5)");
        var tdNETAMOUNT = par.children("td:nth-child(6)");
        var tdIcon = par.children("td:nth-child(7)");
        //tdDepartment.html(a);
        tdNoiminal.html(nominal);
        tdDESCRIPTION.html("<input type='text' class='form-control' value='" + tdDESCRIPTION.html() + "'/>");
        tdDEBIT.html("<input type='text' class='form-control decimal' value='" + tdDEBIT.html() + "'/>");
        tdCREDIT.html("<input type='text' class='form-control decimal' value='" + tdCREDIT.html() + "'/>");
        //tdNETAMOUNT.html("<input type='text' class='form-control decimal' value='" + tdNETAMOUNT.html() + "'/>");

        //tdACCOUNTREF.html("<input type='text' class='form-control' value='" + tdACCOUNTREF.html() + "'/>");
        //tdDESCRIPTION.html("<input type='text' class='form-control' value='" + tdDESCRIPTION.html() + "'/>");
        //tdDEBIT.html("<input type='text' class='form-control decimal' value='" + tdDEBIT.html() + "'/>");
        //tdCREDIT.html("<input type='text' class='form-control decimal' value='" + tdCREDIT.html() + "'/>");
        //tdNoiminal.html(nominal);
        //tdNETAMOUNT.html("<input type='text' class='form-control decimal' value='" + tdNETAMOUNT.html() + "'/>");


        //tdName.html("<input type='text' value='" + tdName.html() + "'/>");
        //tdEmail.html("<input type='text' id='txtEmail'  value='" + tdEmail.html() + "'/>");
        //tdDate.html("<input type='text' id='txtDate' class='datepicker' value='" + tdDate.html() + "'/>");

        tdIcon.html("<img src='../../images/save.png' class='btnSave'/>");

        $(".btnSave").bind("click", Save);
        $(".btnEdit").bind("click", Edit);
        $(".btnDelete").bind("click", Delete);
        //alert(  $('#forEdit').html());

    };
    function btnEditJournalLine() {



        //tdACCOUNTREF.html(tdACCOUNTREF.children("input[type=text]").val());

        //tdDESCRIPTION.html(tdDESCRIPTION.children("input[type=text]").val());
        //tdDEBIT.html(tdCREDIT.children("input[type=text]").val());

        //tdCREDIT.html(tdCREDIT.children("input[type=text]").val());
        //tdNoiminal.html(tdNoiminal.children("select").text());
        //tdNETAMOUNT.html(tdNETAMOUNT.children("input[type=text]").val());

        var par = $(this).parent().parent(); //tr
        //alert(par.html());
        var tdNoiminal = par.children("td:nth-child(1)");
        var nominal = $('#ddlNoiminal').html();
        var tdDESCRIPTION = par.children("td:nth-child(3)");
        var tdDEBIT = par.children("td:nth-child(4)");
        var tdCREDIT = par.children("td:nth-child(5)");
        var tdNETAMOUNT = par.children("td:nth-child(6)");
        var tdIcon = par.children("td:nth-child(7)");
        //tdDepartment.html(a);
        tdNoiminal.html(nominal);
        tdDESCRIPTION.html("<input type='text' class='form-control' value='" + tdDESCRIPTION.html() + "'/>");
        tdDEBIT.html("<input type='text' class='form-control decimal' value='" + tdDEBIT.html() + "'/>");
        tdCREDIT.html("<input type='text' class='form-control decimal' value='" + tdCREDIT.html() + "'/>");


        //tdName.html("<input type='text' value='" + tdName.html() + "'/>");
        //tdEmail.html("<input type='text' id='txtEmail'  value='" + tdEmail.html() + "'/>");
        //tdDate.html("<input type='text' id='txtDate' class='datepicker' value='" + tdDate.html() + "'/>");

        tdIcon.html("<img src='../../images/save.png' class='btnSave'/>");

        $(".btnSave").bind("click", SaveJournal);
        $(".btnEditJournalLine").bind("click", btnEditJournalLine);

        //alert(  $('#forEdit').html());

    };

    function SaveJournal() {
        var par = $(this).parent().parent(); //tr

        var tdNoiminal = par.children("td:nth-child(1)");
        var ddlNoiminal = par.children("td:nth-child(2)");
        var tdDESCRIPTION = par.children("td:nth-child(3)");
        var tdDEBIT = par.children("td:nth-child(4)");
        var tdCREDIT = par.children("td:nth-child(5)");
        var tdNETAMOUNT = par.children("td:nth-child(6)");
        var tdIcon = par.children("td:nth-child(7)");
        var formatedDebit = tdDEBIT.children("input[type=text]").val();
        var formatedCredit = tdCREDIT.children("input[type=text]").val();
        if (tdDEBIT.children("input[type=text]").val()) {

            var chkComa = formatedDebit.indexOf(",");
            if (chkComa == -1)
                formatedDebit = formatNumber(parseFloat(tdDEBIT.children("input[type=text]").val()));
        }

        else {
            formatedDebit = '0.00';
        }

        if (tdCREDIT.children("input[type=text]").val()) {
            var chkComa = formatedCredit.indexOf(",");
            if (chkComa == -1)
                formatedCredit = formatNumber(parseFloat(tdCREDIT.children("input[type=text]").val()));
        }

        else {
            formatedCredit = '0.00';
        }



        var v = tdNoiminal.children("select").val();
        tdNoiminal.html($(tdNoiminal).find("option:selected").text());
        ddlNoiminal.html(v);
        tdDESCRIPTION.html(tdDESCRIPTION.children("input[type=text]").val());
        tdDEBIT.html(formatedDebit);
        tdCREDIT.html(formatedCredit);
        formatedDebit = NeumericNumber(formatedDebit);
        formatedCredit = NeumericNumber(formatedCredit);
        var formatedBalance = parseFloat(formatedDebit) - parseFloat(formatedCredit);
        formatedBalance = formatNumber(parseFloat(formatedBalance));
        tdNETAMOUNT.html(formatedBalance);
        tdIcon.html("<img src='../../images/update.png' class='btnEditJournalLine'/>");
        //tdDate.html(tdDate.children("input[type=text]").val());



        var count = $('#tblGrid tbody tr').length;

        var totalDebit = 0;
        var totalCredit = 0;
        var totalBalance = 0;
        for (var i = 1; i <= count; i++) {
            var debit = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(3)').text();
            var credit = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(4)').text();
            var balance = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(5)').text();

            if (!debit) {

                debit = '0.00';
            }

            if (!credit) {
                credit = '0.00';

            }


            if (!balance) {
                balance = '0.00';

            }
            debit = NeumericNumber(debit);
            credit = NeumericNumber(credit);
            balance = NeumericNumber(balance);

            totalDebit = totalDebit + parseFloat(debit);
            totalCredit = totalCredit + parseFloat(credit);
            totalBalance = totalBalance + parseFloat(balance);


        }
        totalDebit = formatNumber(totalDebit);
        totalCredit = formatNumber(totalCredit);
        totalBalance = formatNumber(totalBalance);

        $('#TotalDebit').text(totalDebit);
        $('#TotalCredit').text(totalCredit);
        $('#TotalBalance').text(totalBalance);



        $(".btnEditJournalLine").bind("click", btnEditJournalLine);


    };


    function Save() {
        var par = $(this).parent().parent(); //tr

        var tdNoiminal = par.children("td:nth-child(1)");
        var ddlNoiminal = par.children("td:nth-child(2)");
        var tdDESCRIPTION = par.children("td:nth-child(3)");
        var tdDEBIT = par.children("td:nth-child(4)");
        var tdCREDIT = par.children("td:nth-child(5)");
        var tdNETAMOUNT = par.children("td:nth-child(6)");
        var tdIcon = par.children("td:nth-child(7)");
        var formatedDebit = tdDEBIT.children("input[type=text]").val();
        var formatedCredit = tdCREDIT.children("input[type=text]").val();
        if (tdDEBIT.children("input[type=text]").val()) {

            var chkComa = formatedDebit.indexOf(",");
            if (chkComa == -1)
                formatedDebit = formatNumber(parseFloat(tdDEBIT.children("input[type=text]").val()));
        }

        else {
            formatedDebit = '0.00';
        }

        if (tdCREDIT.children("input[type=text]").val()) {
            var chkComa = formatedCredit.indexOf(",");
            if (chkComa == -1)
                formatedCredit = formatNumber(parseFloat(tdCREDIT.children("input[type=text]").val()));
        }

        else {
            formatedCredit = '0.00';
        }


        var v = tdNoiminal.children("select").val();
        tdNoiminal.html($(tdNoiminal).find("option:selected").text());
        ddlNoiminal.html(v);
        tdDESCRIPTION.html(tdDESCRIPTION.children("input[type=text]").val());
        tdDEBIT.html(formatedDebit);
        tdCREDIT.html(formatedCredit);
        formatedDebit = NeumericNumber(formatedDebit);
        formatedCredit = NeumericNumber(formatedCredit);
        var formatedBalance = parseFloat(formatedDebit) - parseFloat(formatedCredit);
        formatedBalance = formatNumber(parseFloat(formatedBalance));
        tdNETAMOUNT.html(formatedBalance);
        tdIcon.html("<img src='../../images/update.png' class='btnEdit'/><img src='../../images/delete.png' class='btnDelete'/>");
        //tdDate.html(tdDate.children("input[type=text]").val());



        var count = $('#tblGrid tbody tr').length;

        var totalDebit = 0;
        var totalCredit = 0;
        var totalBalance = 0;
        for (var i = 1; i <= count; i++) {
            var debit = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(3)').text();
            var credit = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(4)').text();
            var balance = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(5)').text();

            if (!debit) {

                debit = '0.00';
            }

            if (!credit) {
                credit = '0.00';

            }


            if (!balance) {
                balance = '0.00';

            }
            debit = NeumericNumber(debit);
            credit = NeumericNumber(credit);
            balance = NeumericNumber(balance);

            totalDebit = totalDebit + parseFloat(debit);
            totalCredit = totalCredit + parseFloat(credit);
            totalBalance = totalBalance + parseFloat(balance);


        }
        totalDebit = formatNumber(totalDebit);
        totalCredit = formatNumber(totalCredit);
        totalBalance = formatNumber(totalBalance);

        $('#TotalDebit').text(totalDebit);
        $('#TotalCredit').text(totalCredit);
        $('#TotalBalance').text(totalBalance);



        $(".btnEdit").bind("click", Edit);
        $(".btnDelete").bind("click", Delete);
    };

    function Delete() {


        var par = $(this).parent().parent();
        par.remove();

        var count = $('#tblGrid tbody tr').length;

        var totalDebit = 0;
        var totalCredit = 0;
        var totalBalance = 0;
        for (var i = 1; i <= count; i++) {
            var debit = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(3)').text();
            var credit = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(4)').text();
            var balance = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(5)').text();

            if (!debit) {

                debit = '0.00';
            }

            if (!credit) {
                credit = '0.00';

            }


            if (!balance) {
                balance = '0.00';

            }
            debit = NeumericNumber(debit);
            credit = NeumericNumber(credit);
            balance = NeumericNumber(balance);

            totalDebit = totalDebit + parseFloat(debit);
            totalCredit = totalCredit + parseFloat(credit);
            totalBalance = totalBalance + parseFloat(balance);


        }

        totalDebit = formatNumber(totalDebit);
        totalCredit = formatNumber(totalCredit);
        totalBalance = formatNumber(totalBalance);

        $('#TotalDebit').text(totalDebit);
        $('#TotalCredit').text(totalCredit);
        $('#TotalBalance').text(totalBalance);


    };

    $(".btnSave").bind("click", Save);
    $(".btnEditJournalLine").bind("click", btnEditJournalLine);
    $(".btnEdit").bind("click", Edit);
    $(".btnDelete").bind("click", Delete);
    $("#btnAdd").bind("click", Add);
    $("#btnSaveAll").bind("click", SaveAll);
    $('#Update').bind("click", Update)
    $("#tblGrid").on("click", "#txtDate", function (event) {

        // $('.datepicker').removeClass('hasDatepicker').datepicker();
        $('.datepicker').removeClass('hasDatepicker').attr('id', '').datepicker();
        event.preventDefault();
    });

    function SaveAll() {

        if ($('#TotalBalance').text() == '0.00') {


            var JOURNALLINEs = [];

            var count = $('#tblGrid tbody tr').length;

            //for (var i = 2; i <= count; i++) {



            //   var a= $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(0)').text();
            //   var b = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(1)').text();

            //   if (!b.trim()) {
            //       alert("sd");
            //   }


            //}

            for (var i = 1; i <= count; i++) {
                var JOURNALLINE = new Object();
                JOURNALLINE.NOMINALACCOUNT_REFERENCE = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(1)').text();
                JOURNALLINE.DESCRIPTION = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(2)').text();
                JOURNALLINE.DEBIT = NeumericNumber($('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(3)').text());
                JOURNALLINE.CREDIT = NeumericNumber($('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(4)').text());
                JOURNALLINE.NETAMOUNT = NeumericNumber($('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(5)').text());

                //JOURNALLINE.ACCOUNTREF = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(0)').text();
                //JOURNALLINE.DESCRIPTION = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(1)').text();
                //JOURNALLINE.DEBIT = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(2)').text();
                //JOURNALLINE.CREDIT = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(3)').text();
                //JOURNALLINE.NOMINALACCOUNT_REFERENCE = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(5)').text();
                //JOURNALLINE.NETAMOUN = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(6)').text();



                JOURNALLINEs.push(JOURNALLINE);

            }
            var oJOURNALHEAD = new Object();

            //oJOURNALHEAD.ACCOUNTREF = $('#ACCOUNTREF').val();
            oJOURNALHEAD.TRANSACTIONDATE = $('#TRANSACTIONDATE').val();
            //oJOURNALHEAD.JOURNALTYPE = $('#JOURNALTYPE').val();
            oJOURNALHEAD.FOLIONUMBER = $('#FOLIONUMBER').val();
            oJOURNALHEAD.DESCRIPTION = $('#DESCRIPTION').val();
            oJOURNALHEAD.FINANCIALYEAR_REFERENCE = $('#FINANCIALYEAR_REFERENCE').val();
            oJOURNALHEAD.FINANCIALPERIOD_REFERENCE = $('#FINANCIALPERIOD_REFERENCE').val();
            oJOURNALHEAD.PAYMENTTYPE = $('#PAYMENTTYPE').val();
            oJOURNALHEAD.REMARKS = $('#REMARKS').val();
            var TRANSACTIONDATE = $('#TRANSACTIONDATE').val();


            $.ajax({
                url: 'JournalHead/AddJournalHead',
                async: true,
                type: 'POST',
                InsertionMode: 'InsertionMode.Replace',
                dataType: 'json',
                UpdateTargetId: 'Replace',

                data: JSON.stringify({ oJOURNALHEAD: oJOURNALHEAD, TRANSACTIONDATE: TRANSACTIONDATE, JOURNALLINEs: JOURNALLINEs }),
                contentType: 'application/json',
                success: function (result) {
                    window.location.replace("/#/JournalHead/ListJournalHead");
                },
                error: function (err) {
                    if (err.responseText == "success") {
                        window.location.replace("/#/JournalHead/ListJournalHead");

                    }
                    else {
                        window.location.replace("/#/JournalHead/ListJournalHead");
                    }
                },
                complete: function () {
                }

            });


        }

        else {

            alert("Transactions does not match");
        }



    };


    function TotalCalculation() {
        alert("");
        var count = $('#tblGrid tbody tr').length;

        var totalDebit = 0;
        var totalCredit = 0;
        var totalBalance = 0;
        for (var i = 1; i <= count; i++) {
            var debit = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(3)').text();
            alert(debit);

            //alert(parseFloat(debit));
            var credit = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(4)').text();
            alert(credit);
            if (!debit) {

                debit = 0;
            }

            if (!credit) {
                credit = 0;

            }

            tdNETAMOUNT.html(parseFloat(debit) - parseFloat(credit));
            var balance = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(5)').text();
            alert(balance);
            if (!balance) {
                balance = 0;

            }


            //if (debit) {

            //    totalCredit = totalCredit + parseFloat(credit);
            //}

            //if (debit) {

            //    totalDebit = totalDebit + parseFloat(debit);
            //}
            totalDebit = totalDebit + parseFloat(debit);
            totalCredit = totalCredit + parseFloat(credit);
            totalBalance = totalBalance + parseFloat(balance);
            alert(totalDebit);
        }


        $('#TotalDebit').text(totalDebit);
        $('#TotalCredit').text(totalCredit);
        $('#TotalBalance').text(totalBalance);

    }


    function Update() {

        if ($('#TotalBalance').text() == '0.00') {
            var JOURNALLINEs = [];

            var count = $('#tblGrid tbody tr').length;

            for (var i = 1; i <= count; i++) {
                var JOURNALLINE = new Object();

                var JOURNALLINE = new Object();

                JOURNALLINE.NOMINALACCOUNT_REFERENCE = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(1)').text();
                JOURNALLINE.DESCRIPTION = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(2)').text();
                JOURNALLINE.DEBIT = NeumericNumber($('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(3)').text());

                JOURNALLINE.CREDIT = NeumericNumber($('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(4)').text());
                JOURNALLINE.NETAMOUNT = NeumericNumber($('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(5)').text());
                JOURNALLINE.REFERENCE = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(7)').text();
                JOURNALLINE.JOURNALHEAD_REFERENCE = $('#REFERENCE').val();
                JOURNALLINE.CREATEDDATE = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(8)').text();
                JOURNALLINE.CREATEDBY = $('#tblGrid tbody >tr:nth-child(' + i + ')').find('td:eq(9)').text();
                JOURNALLINEs.push(JOURNALLINE);

            }
            var oJOURNALHEAD = new Object();
            oJOURNALHEAD.REFERENCE = $('#REFERENCE').val();
            oJOURNALHEAD.CREATEDDATE = $('#CREATEDDATE').val();
            oJOURNALHEAD.CREATEDBY = $('#CREATEDBY').val();
            oJOURNALHEAD.ACCOUNTREF = $('#ACCOUNTREF').val();
            oJOURNALHEAD.TRANSACTIONDATE = $('#TRANSACTIONDATE').val();
            oJOURNALHEAD.JOURNALTYPE = $('#JOURNALTYPE').val();
            oJOURNALHEAD.FOLIONUMBER = $('#FOLIONUMBER').val();
            oJOURNALHEAD.DESCRIPTION = $('#DESCRIPTION').val();
            oJOURNALHEAD.FINANCIALYEAR_REFERENCE = $('#FINANCIALYEAR_REFERENCE').val();
            oJOURNALHEAD.FINANCIALPERIOD_REFERENCE = $('#FINANCIALPERIOD_REFERENCE').val();
            oJOURNALHEAD.PAYMENTTYPE = $('#PAYMENTTYPE').val();
            oJOURNALHEAD.REMARKS = $('#REMARKS').val();
            var TRANSACTIONDATE = $('#TRANSACTIONDATE').val();


            $.ajax({
                url: 'JournalHead/EditJournalHead',
                async: true,
                type: 'POST',
                InsertionMode: 'InsertionMode.Replace',
                dataType: 'json',
                UpdateTargetId: 'Replace',

                data: JSON.stringify({ oJOURNALHEAD: oJOURNALHEAD, TRANSACTIONDATE: TRANSACTIONDATE, JOURNALLINEs: JOURNALLINEs }),
                contentType: 'application/json',
                success: function (result) {
                    window.location.replace("/#/JournalHead/ListJournalHead");
                },
                error: function (err) {
                    if (err.responseText == "success") {
                        window.location.replace("/#/JournalHead/ListJournalHead");

                    }
                    else {
                        window.location.replace("/#/JournalHead/ListJournalHead");
                    }
                },
                complete: function () {
                }

            });
        }


        else {

            alert("");
        }



    };



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

    function NeumericNumber(number) {

        var formatedNumber = '';

        var numbers = number.split(',');

        if (numbers.length == 1) {

            return number;
        }

        else {

            for (var i = 0; i < numbers.length; i++) {
                formatedNumber = formatedNumber + numbers[i];

            }


            return formatedNumber.trim();
        }


    }


});
