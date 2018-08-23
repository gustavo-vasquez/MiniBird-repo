$(document).ready(function () {
    $('#removeList').on('click', function (event) {
        var answer = confirm("Esta acción NO podrá deshacerse.\n¿Está seguro?");
        if (answer != true)
            event.preventDefault();        
    });
});