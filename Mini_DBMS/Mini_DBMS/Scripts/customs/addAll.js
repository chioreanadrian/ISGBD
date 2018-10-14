function onModalError(error) {
    var oldError = $('#myModal').find('.alert-danger');
    if (oldError)
        oldError.remove();
    $('#myModal .modal-body').prepend('<div class="alert alert-dismissible alert-danger">\
		<button type= "button" class="close" data-dismiss="alert" >×</button>\
        ' + (error.responseText || error.statusText) + '</div>');
}

function onModalSuccess(result) {
    //location.reload();
    console.log("success");
}

function CreateDatabase() {
    $body = $("body");
    $.ajax({
        url: '/Home/CreateDatabase',
        type: 'GET',
        data: { },
        success: function (result) {
            $('#myModal .modal-body').html(result);



            //$.material.init();
            $('#myModal').modal('show');
        }
    }, this);
}

function AddTable() {
    $body = $("body");
    $.ajax({
        url: '/Home/CreateTable',
        type: 'GET',
        data: { },
        success: function (result) {
            $('#myModal .modal-body').html(result);



            //$.material.init();
            $('#myModal').modal('show');
        }
    }, this);
}

function AddFields() {
    $body = $("body");
    $.ajax({
        url: '/Home/CreateField',
        type: 'GET',
        data: { },
        success: function (result) {
            $('#myModal .modal-body').html(result);


            //$.material.init();
            $('#myModal').modal('show');
        }
    }, this);
}