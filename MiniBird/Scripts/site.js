$(document).ready(function () {
    $('.dropdown-menu > a, .dropdown-menu > form > button').hover(function () {
        $(this).toggleClass('active');
    });

    $('#postModal').on('shown.bs.modal', function () {
        $('#Comment').focus();
    });

    $('.post-images img').on('click', function () {
        var src = $(this).attr('src');
        var images = $(this).closest('.post-images').find('img');
        var srcArray = images.map(function () {
            return this.src;
        }).get();

        loadImagePreview(srcArray, src);
    });

    $('.interact-buttons').on('click', '.repost', function () {
        var $interactButtonsDiv = $(this).parent('div');

        $.ajax({
            url: "/Account/SendRepost",
            method: "POST",
            data: "postID=" + $interactButtonsDiv.data('postid'),
            success: function (response) {
                $interactButtonsDiv.html(response);
            }
        });
    });

    $('.interact-buttons').on('click', '.like', function () {
        var $interactButtonsDiv = $(this).parent('div');

        $.ajax({
            url: "/Account/GiveALike",
            method: "POST",
            data: "postID=" + $interactButtonsDiv.data('postid'),
            success: function (response) {
                $interactButtonsDiv.html(response);                
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

function loadImagePreview(srcArray, src) {
    var index = srcArray.indexOf(src);

    $.ajax({
        url: "/Account/ImagePreview",
        method: "GET",
        success: function (response) {
            $('body').append(response);
            var $imgPreview = $('.img-preview');
            $imgPreview.attr('src', src);

            var leftPos = $imgPreview.offset().left;
            var topPos = $('.img-preview-container').height() / 2;
            var $prevNextCombination = $('.prev-img-preview, .next-img-preview');

            $('.prev-img-preview').css({ "left": leftPos + "px", "top": topPos + "px" });
            $('.next-img-preview').css({ "right": leftPos + "px", "top": topPos + "px" });

            $imgPreview.on('mouseenter', function () {
                $prevNextCombination.removeClass('d-none');
            }).on('mouseleave', function () {
                if ($('.prev-img-preview').is(':hover') || $('.next-img-preview').is(':hover')) {
                    $prevNextCombination.on('mouseleave', function () {
                        $prevNextCombination.addClass('d-none');
                    });
                }
                else
                    $prevNextCombination.addClass('d-none');
            });

            if (index <= 0)
                $('.prev-img-preview').hide();

            if (index >= srcArray.length - 1)
                $('.next-img-preview').hide();

            $('.img-preview-overlay').on('click', function (event) {
                if (event.target.classList[0] == "img-preview")
                    return;
                $('.img-preview-overlay').remove();
            });

            $('.prev-img-preview').on('click', function () {
                var prevIndex = index - 1;

                if (prevIndex >= 0) {
                    $('.img-preview-overlay').remove();                    
                    loadImagePreview(srcArray, srcArray[prevIndex]);
                }
            });

            $('.next-img-preview').on('click', function () {
                var nextIndex = index + 1;

                if (nextIndex <= (srcArray.length - 1)) {
                    $('.img-preview-overlay').remove();
                    loadImagePreview(srcArray, srcArray[nextIndex]);
                }
            });
        },
        error: function () {
            alert("¡Ups ocurrió un error!");
        }
    });
}