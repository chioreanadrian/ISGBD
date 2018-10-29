function AddPrimaryKey() {
    $body = $("body");
    $.ajax({
        url: '/Home/AddPrimaryKey',
        type: 'GET',
        data: {},
        success: function (result) {
            $('#myModal .modal-body').html(result);


            //$.material.init();
            $('#myModal').modal('show');
        }
    }, this);
}