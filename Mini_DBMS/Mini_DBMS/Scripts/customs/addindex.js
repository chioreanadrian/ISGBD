function AddIndex() {
    $body = $("body");
    $.ajax({
        url: '/Home/AddIndex',
        type: 'GET',
        data: {},
        success: function (result) {
            $('#myModal .modal-body').html(result);


            //$.material.init();
            $('#myModal').modal('show');
        }
    }, this);
}