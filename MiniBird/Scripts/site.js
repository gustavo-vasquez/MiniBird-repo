$(document).ready(function () {
    $('body').on('mouseenter mouseleave', '.dropdown-menu > a, .dropdown-menu > form > button, .list-group-item', function () {
        $(this).toggleClass('active');
    });

    $('body').on('mouseenter', '.post-actions-menu', function () {
        $(this).children('a').children('i').addClass('fas');
    }).on('mouseleave', '.post-actions-menu', function () {
        $(this).children('a').children('i').removeClass('fas');
    });

    $(document).on('click', '#postBtn', { action: "post" }, drawPublication);
    $(document).on('click', '#replyBtnFixed, #replyBtnDynamic', { action: "reply" }, drawPublication);    

    $('body').on('click', '.post-images img', function () {
        var src = $(this).attr('src');
        var images = $(this).closest('.post-images').find('img');
        var srcArray = images.map(function () {
            return this.getAttribute("src");
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
        var filterList = ".post a, .post b, .post img, .post-images img, .repost, .repost i, .repost span, .like, .like i, .like span, .card-link, .card-link img, .post-actions-menu a, .post-actions-menu i, .post-actions-menu .dropdown-menu";
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

function drawPublication(event) {
    var $cleanModal, $setModal, queryString, successfulModalLoading;

    switch (event.data.action) {
        case "post":
            $cleanModal = $('#replyModal');
            $setModal = $('#postModal');
            queryString = "call=post";
            successfulModalLoading = function (data, textStatus, XMLHttpRequest) {                
                $('body').append(data);
                $('#postModal').modal('show');
                $('#postModal').on('shown.bs.modal', function () {
                    $('#Comment').focus();
                });

                $('#NewPostForm').on('keydown keyup', { map: {} }, function (event) {
                    if (twoPressedKeys(event, "Control", "Enter", event.data.map))
                        $(this).trigger('submit');
                });

                $.validator.unobtrusive.parse($('#NewPostForm'));
            }
            break;
        case "reply":
            $cleanModal = $('#postModal');
            $setModal = $('#replyModal');

            if(event.data.thisElement === undefined)
                $this = $(this);
            else
                $this = event.data.thisElement;

            queryString = "call=reply&updateTarget=" + $this.data('updatetarget');
            successfulModalLoading = function (data, textStatus, XMLHttpRequest) {                
                    $this.after(data);
                    $('#replyModalTitle').text('En respuesta a ' + $this.data('replyto'));
                    $('#InReplyTo').val($this.data('postid'));
                    $('#replyModal').modal('show');
                    $('#replyModal').on('shown.bs.modal', function () {
                        if ($this.attr('id') == "replyBtnFixed")
                            $('body').css('overflow', 'visible');

                        $('#Comment').focus();
                    });
                    
                    $('#NewReplyForm').on('keydown keyup', { map: {} }, function (event) {
                        if (twoPressedKeys(event, "Control", "Enter", event.data.map))
                            $(this).trigger('submit');
                });

                $.validator.unobtrusive.parse($('#NewReplyForm'));
            }
            break;
        default:
            return;
            break;
    }

    if ($cleanModal.length > 0)
        $cleanModal.remove();

    filesTotalSize = 0; // vuelve a inicializar el peso total de imágenes para la validación           

    var existNewPostJs = $('script').filter(function (index) {
        return $(this)[0].src.includes("/newPost.js");
    }).length;

    if (existNewPostJs === 0) {
        var fileref = document.createElement('script')
        fileref.setAttribute("type", "text/javascript")
        fileref.setAttribute("src", '/Scripts/account/newPost.js')

        if (typeof fileref != "undefined")
            document.getElementsByTagName("body")[0].appendChild(fileref);
    }

    if ($setModal.length <= 0) {
        $.ajax({
            url: "/Account/DrawPublication",
            method: "GET",
            data: queryString,
            success: successfulModalLoading,
            error: function () {
                alert("¡Error al cargar el panel de publicación!\nInténtelo de nuevo más tarde.");
            }
        });
    }
    else
        $setModal.modal('show');
}   

function twoPressedKeys(event, firstKey, secondKey, map) {    
    map[event.key] = event.type == 'keydown';    
    
    if (map[firstKey] && map[secondKey])
        return true;
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
    var $words;

    $.each($('.text-comment'), function (index, value) {
        //var separators = ['\r', '\n', ' '];
        //var $words = $(value).text().split(new RegExp(separators.join('|'), 'g'));
        $words = $(value).text().split('\n');

        for (i in $words) {
            if ($words[i].length >= 3) {
                // search urls
                if ($words[i].startsWith('http://') || $words[i].startsWith('https://')) {
                    $words[i] = '<a target="_blank" rel="noopener noreferrer" href="' + $words[i] + '">' + $words[i] + '</a>';
                }
                else if ($words[i].startsWith('#') && $words[i].indexOf('#', 1) < 1) {
                    // search hashtags
                    $words[i] = '<a href="/Home/Hashtag?name=' + encodeURIComponent($words[i]) + '">' + $words[i] + '</a>';
                }
            }
        }
                
        $words = $words.join('\n');

        var $words = $words.split(' ');
        var newText = '';

        for (i in $words) {
            if ($words[i].length >= 3) {
                // search urls
                if ($words[i].startsWith('http://') || $words[i].startsWith('https://')) {
                    $words[i] = '<a target="_blank" rel="noopener noreferrer" href="' + $words[i] + '">' + $words[i] + '</a>';
                }
                else if ($words[i].startsWith('#') && $words[i].indexOf('#', 1) < 1) {
                    // search hashtags
                    $words[i] = '<a href="/Home/Hashtag?name=' + encodeURIComponent($words[i]) + '">' + $words[i] + '</a>';
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
        url: "/Home/DrawSearch",
        method: "GET",
        success: function (data) {
            $('body').append(data);
            $('#searchModal').modal('show');

            $('#searchModal').on('shown.bs.modal', function () {
                $('#WordToSearch').focus();
            });            

            $('#searchModal').on('hidden.bs.modal', function () {
                $(this).remove();
            });

            $('#WordToSearch').on('keyup', function () {
                $.ajax({
                    url: "/Home/Search",
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