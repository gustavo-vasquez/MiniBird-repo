$(document).ready(function () {
    $('#welcome').parent('div').removeClass('container');    
    $('footer, hr').remove();

    if ($('#loginForm').hasClass('activate')) {
        $('#registerForm').hide();
        $('#loginForm').removeClass('d-none');
    }    

    $('#Register_UserName').on('blur', function () {
        var userNameInput = $(this);

        if (userNameInput.val().length > 0) {
            $.ajax({
                url: "/Account/CheckUserName",
                type: "GET",
                data: "username=" + $(this).val(),
                success: function (response) {
                    if (response.userExists) {
                        $('<small id="inUse" class="form-text text-danger">El nombre ya está en uso.</small>').insertBefore(userNameInput);
                    }
                    else {
                        $('#inUse').remove();
                    }
                }
            });
        }        
    });
});