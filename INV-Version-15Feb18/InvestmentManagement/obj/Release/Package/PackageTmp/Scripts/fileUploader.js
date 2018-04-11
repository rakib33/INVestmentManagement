function ProgressBar(initVal, maxVal, bar) {

    for (initVal; initVal <= maxVal; initVal++) {
        bar.width(initVal + '%');
        bar.html(initVal + '%');
    }

}




function btnUpload(intputId, pBId, btnId) {
    //alert(inputId);
    //alert(pBId);
    //alert(btnId);
    //alert(intputId + pBId + btnId);
    
    var formInputId = intputId;
    var progressBarId = '#' + pBId;
    var btnUploadId = '#' + btnId;

    var result = 0;

    (function () {

        var formData = new FormData();
        var totalFiles = document.getElementById(formInputId).files.length;

        var file = document.getElementById(formInputId).files[0];
       // alert(file);
        formData.append(formInputId, file);

        //for (var i = 0; i < totalFiles; i++) {
            //var file = document.getElementById(formInputId).files[i];
            //alert(file);

            //formData.append(formInputId, file);            
        //}

        if (totalFiles > 0) {
            var xhr = new XMLHttpRequest();
            var bar = $(progressBarId);

            xhr.onreadystatechange = function () {
                //alert(xhr.status + "  " + xhr.readyState);
                if ((xhr.readyState == 1)) {
                    var initVal = 0;
                    var maxVal = 25;
                    ProgressBar(initVal, maxVal, bar);
                    result = 4;

                } else if (xhr.readyState == 2) {
                    var initVal = 16;
                    var maxVal = 50;
                    ProgressBar(initVal, maxVal, bar);
                    result = 3;
                } else if (xhr.readyState == 3) {
                    var initVal = 51;
                    var maxVal = 75;
                    ProgressBar(initVal, maxVal, bar);
                    result = 2;
                } else if (xhr.readyState == 4 && xhr.status == 200 && xhr.responseText.trim() == "\"Upload Successful\"") {
                    var initVal = 76;
                    var maxVal = 100;
                    ProgressBar(initVal, maxVal, bar);
                    result = 1;
                   
                    $(btnUploadId).prop("disabled", true);

                } else if (xhr.readyState == 4 && xhr.status == 200) {
                    var initVal = 0;
                    var maxVal = 0;
                    ProgressBar(initVal, maxVal, bar);
                    result = 5;
                    alert(xhr.responseText);

                } else if (xhr.readyState == 4 && xhr.status == 500) {
                    result = 6;
                    alert(xhr.response);
                }

                else {
                    alert("Upload Failed due to Server Error!");
                    result = 7;
                }

                console.log(result);
                console.log(xhr.response, ' ' + xhr.responseText);
            };

            if (result == 1)
                alert("Upload Successfull.");

            xhr.open('POST', '/CDBLFiles/' + formInputId, true);
            xhr.send(formData);
        }
        else {
            alert("Select a File!");
        }
    })();

}



(function () {
    $(document).on("click", 'button', function () {
        var btnId = this.id;
      //  alert(btnId);        

        var childParentId = document.getElementById(btnId).parentNode;
        //alert(childParentId.id);
        var pTag = document.getElementById(childParentId.id).parentNode;
        //alert(pTag.id);
        var inputDivId = pTag.getElementsByTagName('div')[1].id;
        //alert(inputDivId);
        inputId = document.getElementById(inputDivId).firstElementChild.id;
        //alert(inputId);

        var pBDivId = pTag.getElementsByTagName('div')[3].id;
       // alert(pBDivId);
        pBId = document.getElementById(pBDivId).firstElementChild.id;
       // alert(pBId);

        btnUpload(inputId, pBId, btnId);
        
        //var pbId = pTag.getElementsByTagName('span')[0].id;
        //alert(pbId);
        //var inputId = pTag.getElementsByTagName('input')[0].id;
        //alert(inputId);
        //btnUpload(inputId, pbId, btnId);

    })




})();