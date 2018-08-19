$(document).ready(function () {
    $('.dropdown-menu > a, .dropdown-menu > form > button').hover(function () {
        $(this).toggleClass('active');
    });

    $('#postModal').on('shown.bs.modal', function () {
        $('#Comment').focus();
    });

    $('.post-images img').on('click', function () {
        var $src = $(this).attr('src');

        $.ajax({
            url: "/Account/ImagePreview",
            method: "GET",
            success: function (response) {
                $('body').append(response);
                $('.img-preview').attr('src', $src);
                $('.img-preview-overlay').on('click', function (event) {
                    if (event.target.classList[0] == "img-preview")
                        return;
                    $('.img-preview-overlay').remove();
                });
            },
            error: function () {
                alert("¡Ups ocurrió un error!");
            }
        });
    });
});

function goTop() {
    $('body,html').animate({
        scrollTop: 0
    }, 800);
    return false;
}