function AddForeignKey(field) {
    $body = $("body");
    $.ajax({
        url: '/Home/AddForeignKey',
        type: 'GET',
        data: { field: field },
        success: function (result) {
            $('#myModal .modal-body').html(result);

            $('#myModal').modal('show');
        }
    }, this);
}