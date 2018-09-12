﻿$(document).ready(function () {
    $('body').on('mouseenter mouseleave', '.dropdown-menu > a, .dropdown-menu > form > button, .list-group-item', function () {
        $(this).toggleClass('active');
    });

    $('body').on('mouseenter', '.post-actions-menu', function () {
        $(this).children('a').children('i').addClass('fas');
    }).on('mouseleave', '.post-actions-menu', function () {
        $(this).children('a').children('i').removeClass('fas');
    });

    $('body').on('click', '#postBtn', newPost);
    $('body').on('click', '#replyBtnFixed, #replyBtnDynamic', function () {
        newReply($(this));
    });

    $('body').on('click', '.post-images img', function () {
        var src = $(this).attr('src');
        var images = $(this).closest('.post-images').find('img');
        var srcArray = images.map(function () {
            return this.src;
        }).get();

        loadImagePreview(srcArray, src);
    });

    $('body').on('click', '.repost', function () {
        sendARepost($(this).parent('.interact-buttons'));
    });

    $('body').on('click', '.like', function () {
        giveALike($(this).parent('.interact-buttons'));
    });

    $('body').on('click', '.post, .answers', function (event) {
        var filterList = ".post-images img, .repost, .repost i, .repost span, .like, .like i, .like span, .card-link, .card-link img, .post-actions-menu a, .post-actions-menu i, .post-actions-menu .dropdown-menu";
        if (!$(event.target).is(filterList))
            loadPost($(this).data('postid'));
    });

    scanSpecialWords();

    $('body').on('click', '.copy-link', function (event) {
        event.preventDefault();
        copyLinkToClipboard($(this));
    });

    $('#search').on('click', search);
});


// FUNCIONES

function goTop() {
    $('body,html').animate({
        scrollTop: 0
    }, 800);
    return false;
}

function newPost() {
    $('#replyModal').remove();

    if ($('#postModal').length <= 0) {
        $.ajax({
            url: "/Account/DrawPublication",
            method: "GET",
            data: "call=post",
            success: function (data) {
                $('body').append(data);

                $.getScript("/Scripts/account/newPost.js", function () {                    
                    $('#postModal').modal('show');
                    $('#postModal').on('shown.bs.modal', function () {
                        $('#Comment').focus();
                    });
                });

                $('#NewPostForm').on('keydown', function (event) {
                    if (event.ctrlKey && event.keyCode === 13) {
                        $(this).trigger('submit');
                    }
                });
            },
            error: function () {
                alert("¡Error al cargar el panel de publicación!\nInténtelo de nuevo más tarde.");                
            }
        });
    }
    else {
        $('#postModal').modal('show');
    }
}

function newReply($replyBtn) {
    $('#postModal').remove();

    if ($('#replyModal').length <= 0) {
        $.ajax({
            url: "/Account/DrawPublication",
            method: "GET",
            data: "call=reply&updateTarget=" + $replyBtn.data('updatetarget'),
            success: function (data, textStatus, XMLHttpRequest) {
                $replyBtn.after(data);
                $('#replyModalTitle').text('En respuesta a ' + $replyBtn.data('replyto'));
                $('#InReplyTo').val($replyBtn.data('postid'));

                $.getScript("/Scripts/account/newPost.js", function () {
                    $('#replyModal').modal('show');
                    $('#replyModal').on('shown.bs.modal', function () {
                        $('#Comment').focus();
                    });
                });

                $('#NewReplyForm').on('keydown', function (event) {
                    if (event.ctrlKey && event.keyCode === 13) {
                        $(this).trigger('submit');
                    }
                });
            },
            error: function () {
                alert("¡Error al cargar el panel de publicación!\nInténtelo de nuevo más tarde.");
            }
        });
    }
    else {
        $('#replyModal').modal('show');
    }
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

function loadPost(postLink) {
    $('#replyModal').remove();

    $.ajax({
        url: "/Account/ViewPost",
        method: "GET",
        data: "postID=" + postLink,
        success: function (data) {            
            openPost(data);
            $('body').on('click', '#closePost', closePost);

            $('.view-post-container').on('click', function (event) {
                if ($(event.target).find('#viewingPostDynamic').length > 0)
                    closePost();
                else
                    return;
            });

            $('#replyModal').on('shown.bs.modal', function () {
                $('#Comment_Reply').focus();
            });

            scanSpecialWords();            
        },
        error: function () {
            alert("¡Ocurrió un error!");
        }
    });
}

function openPost(data) {
    $('.view-post-container').remove();
    $('body').append(data);
    $('body').css('overflow', 'hidden');
    $('#viewingPostDynamic').addClass('slide-in');
}

function closePost() {
    $('#viewingPostDynamic').addClass('slide-out');
    setTimeout(function () {
        $('.view-post-container').remove();
        $('body').css('overflow', 'visible');
        $('#replyModal').remove();
    }, 300);    
}

function giveALike(containerDiv) {
    $.ajax({
        url: "/Account/GiveALike",
        method: "POST",
        data: "postID=" + containerDiv.data('postid'),
        success: function (response) {
            containerDiv.html(response);
        }
    });
}

function sendARepost(containerDiv) {
    $.ajax({
        url: "/Account/SendRepost",
        method: "POST",
        data: "postID=" + containerDiv.data('postid'),
        success: function (response) {
            containerDiv.html(response);
        }
    });
}

function scanSpecialWords() {
    $.each($('.text-comment'), function (index, value) {
        var $words = $(value).text().split(' ');
        for (i in $words) {

            // search urls
            if ($words[i].startsWith('http://') || $words[i].startsWith('https://')) {
                $words[i] = '<a target="_blank" rel="noopener noreferrer" href="' + $words[i] + '">' + $words[i] + '</a>';
            }

            // search hashtags
            if ($words[i].length >= 3) {
                if ($words[i].startsWith('#') && $words[i].indexOf('#', 1) < 1) {
                    $words[i] = '<a href="/Home/Search?q=' + encodeURIComponent($words[i]) + '">' + $words[i] + '</a>';
                }
            }
        }

        $(value).html($words.join(' '));
    });
}

function copyLinkToClipboard($element) {
    var $temp = $("<input>");
    $("body").append($temp);
    $temp.val($element.data('copyurl')).select();
    document.execCommand("copy");
    $temp.remove();
    alert("¡Enlace copiado!");
}

function search() {
    $.ajax({
        url: "/Home/Search",
        method: "GET",
        success: function (data) {
            $('body').append(data);
            $('#searchModal').modal('show');

            $('#searchModal').on('shown.bs.modal', function () {
                $('#wordToSearch').focus();
            });            

            $('#searchModal').on('hidden.bs.modal', function () {
                $(this).remove();
            });

            $('#wordToSearch').on('keyup', function () {
                $.ajax({
                    url: "/Home/FindMatches",
                    method: "GET",
                    data: "q=" + $(this).val(),                    
                    success: function (data) {
                        $('#searchModal .modal-footer').remove();

                        if (data != null)
                            $('#searchModal .modal-content').append(data);
                    },
                    error: function () {
                        alert("Ups!");
                    }
                });
            });
        },
        error: function () {
            alert("Ups!");
        }
    });
}