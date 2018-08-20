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

                var leftPos = $('.img-preview').offset().left;
                var topPos = $('.img-preview-container').height() / 2;
                $('.prev-img-preview').css({ "left": leftPos + "px", "top": topPos + "px" });
                $('.next-img-preview').css({ "right": leftPos + "px", "top": topPos + "px" });

                $('.img-preview').hover(function () {                    
                    $('.prev-img-preview, .next-img-preview').removeClass('d-none');
                }, function (event) {
                    if ($('.prev-img-preview').is(':hover') || $('.next-img-preview').is(':hover')) {
                        $('.prev-img-preview, .next-img-preview').on('mouseleave', function () {
                            $('.prev-img-preview, .next-img-preview').addClass('d-none');
                        });
                    }                        
                    else
                        $('.prev-img-preview, .next-img-preview').addClass('d-none');
                });

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