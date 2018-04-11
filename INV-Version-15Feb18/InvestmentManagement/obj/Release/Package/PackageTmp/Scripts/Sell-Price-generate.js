
$("#btn-SellUpload").click(function () {
   
    var value = $('#DirectoryPath').val();
    var instrument = $('#UserIDs').val();
    var date = $("#date").val();
    if (value == null || value == '')
        alert('Please give a path where file saved.');
    else if(date ==null || date =='')
        alert('please add a Sell Limit date');   
    else {
        alert('ok');
        try{
            $.ajax({
                url: '/Portfolio/GenerateSellLimit/',
                type: 'POST',
                dataType: 'json',
                cache: false,
                data: {'UserIDs':instrument,'DirectoryPath': value,'date':date },
                success: function (color) {

                    if (color.result == 'Success')
                        alert('file generate Success.');
                    else
                        alert(color.result);
                   
                },
                error: function (xhr, resp, text) {
                    console.log(xhr, resp, text);
                    alert('Error occured');
                }
            });
        }
        catch (ex)
        {
            alert(ex.message);
        }
    }
});


