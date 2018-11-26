
function AddData() {
    var valuesToSend = [];
    var values = document.getElementsByClassName("fieldValue");
    for (let value of values) {
        console.log(value.value + " ");
        valuesToSend.push(value.value);
    }
    console.log(typeof (values));
    console.log(typeof (valuesToSend));



    $body = $("body");
    $.ajax({
        url: '/Home/_AddData',
        type: 'POST',
        data: { values: valuesToSend },
        success: function (result) {
            alert("Data added");
            for (let value of values) {
                value.value = "";
            }
        }
    }, this);
}