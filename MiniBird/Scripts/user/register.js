$(document).ready(function () {
    $('#loginLink').on('click', function (event) {
        event.preventDefault();        
        $('#registerForm').slideUp(200, function () {
            $('#loginForm').removeClass('d-none');
            $('#emailLogin').focus();
        });
    });
});