$(document).ready(function () {
    //console.log($('#NewPostForm'));
    //var form = $('#NewPostForm').removeData("validator").removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/
    //$.validator.unobtrusive.parse($('#NewPostForm'));
    //$(form).data('unobtrusiveValidation')      
    $('#Comment').magicsize();
    $('#newLinkBtn').on('click', addLinkToPost);
    $('#Comment').on('keyup', calculateChars);
});

function addLinkToPost() {
    if ($('#linkGroup').length <= 0) {
        $('#NewPostForm .modal-body').append('<div id="linkGroup" class="input-group"><div class="input-group-prepend"><span class="input-group-text"><i class="fas fa-link"></i></span></div><input type="text" class="form-control" id="linkText" placeholder="Ingresa una dirección web"><div class="input-group-append"><button id="addLinkBtn" class="btn btn-primary" type="button" title="Añadir"><i class="fas fa-plus"></i></button></div></div>');
        $('#linkText').focus();

        $('#addLinkBtn').on('click', function () {
            $Comment = $('#Comment');
            $LinkToAdd = $('#linkText').val();

            if ($LinkToAdd.startsWith("http://www.") || $LinkToAdd.startsWith("https://www.")) {
                if ($Comment.val().length > 0)
                    $Comment.val($Comment.val() + ' ' + $LinkToAdd + ' ');
                else
                    $Comment.val($Comment.val() + $LinkToAdd + ' ');
                                
                $('#linkGroup').remove();
                $Comment.focus();
                calculateChars();
            }
        });
    }
}

function calculateChars() {
    var label = $('.available-chars');    
    label.text(280 - $('#Comment').val().length);

    if (label.text() < 0)
        label.addClass('text-danger');
    else
        label.removeClass('text-danger');
}

(function ($) {
    $.fn.magicsize = function () {
        this.filter('textarea').each(function () {

            var observe;
            if (window.attachEvent) {
                observe = function (element, event, handler) {
                    element.attachEvent('on' + event, handler);
                };
            }
            else {
                observe = function (element, event, handler) {
                    element.addEventListener(event, handler, false);
                };
            }

            var text = document.getElementById('Comment');
            function resize() {
                text.style.height = 'auto';
                if (text.scrollHeight > 0)
                    text.style.height = (text.scrollHeight + 2) + 'px';
            }
            /* 0-timeout to get the already changed text */
            function delayedResize() {
                window.setTimeout(resize, 0);
            }
            observe(text, 'change', resize);
            observe(text, 'cut', delayedResize);
            observe(text, 'paste', delayedResize);
            observe(text, 'drop', delayedResize);
            observe(text, 'keydown', delayedResize);

            text.focus();
            //text.select();
            resize();
        });
    }
}(jQuery));