$(document).ready(function () {
    if ($('#loginForm').hasClass('activate')) {
        $('#registerForm').hide();
        $('#loginForm').removeClass('d-none');
    }        

    $('#loginLink').on('click', function (event) {
        event.preventDefault();        
        $('#registerForm').slideUp(200, function () {
            $('#loginForm').removeClass('d-none');
            $('#emailLogin').focus();
        });
    });

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