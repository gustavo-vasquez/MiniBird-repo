$(document).ready(function () {
    //console.log($('#NewPostForm'));
    //var form = $('#NewReplyForm').removeData("validator").removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/    
    $('#Comment').magicsize();
    $('body').on('click', '#newLinkBtn', addLinkToPost);
    $('body').on('keydown', '#Comment', calculateChars);
    eventsforUploadImages();

    $.validator.setDefaults({
        ignore: ""
    })

    $.validator.unobtrusive.adapters.add('imagesize', ['maxsize'], function (options) {
        var params = {
            maxSize: options.params.maxsize
        };
        options.rules['imagesize'] = params;
        options.messages['imagesize'] = options.message;
    });

    $.validator.addMethod('imagesize', function (value, element, params) {        
        if (element.files.length > 0)
            return element.files[0].size <= params.maxSize;
        else
            return true;
    });

    $.validator.unobtrusive.adapters.add('imageextensions', ['extensions'], function (options) {
        var params = {
            extensions: JSON.parse(options.params.extensions)
        };
        options.rules['imageextensions'] = params;
        options.messages['imageextensions'] = options.message;
    });

    $.validator.addMethod('imageextensions', function (value, element, params) {
        var extension = value.substring(value.lastIndexOf(".") + 1).toLowerCase();
        return $.inArray(extension, params.extensions) != -1;
    });
        
    $.validator.unobtrusive.parse($('#NewPostForm, #NewReplyForm'));
    //$('#NewPostForm, #NewReplyForm').data('unobtrusiveValidation');
});

function addLinkToPost() {
    if ($('#linkGroup').length > 0) {
        $('#linkGroup').remove();
    }
    else {
        $('#NewPostForm .modal-body, #NewReplyForm .modal-body').append('<div id="linkGroup" class="input-group"><div class="input-group-prepend"><span class="input-group-text"><i class="fas fa-link"></i></span></div><input type="text" class="form-control" id="linkText" placeholder="Ingresa una dirección web"><div class="input-group-append"><button id="addLinkBtn" class="btn btn-outline-primary btn-primary" type="button" title="Añadir"><i class="fas fa-plus"></i></button></div></div>');
        $('#linkText').focus();

        $('#addLinkBtn').on('click', function () {
            $Comment = $('#Comment');
            $LinkToAdd = $('#linkText').val();

            if ($LinkToAdd.startsWith("http://") || $LinkToAdd.startsWith("https://")) {
                $Comment.val($Comment.val() + $LinkToAdd);
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

function imageErrorMsg(type) {
    var message;

    switch (type) {
        case "length":
            message = "Máximo 4 imágenes";
            break;
        case "extension":
            message = "Sólo imágenes jpg, jpeg, png";
            break;
        case "size":
            message = "Permitido hasta 4MB";
            break;
        default:
            return false;
    }

    if ($('.image-error-msg').length <= 0)        
        $('.modal-body').append('<span class="text-danger image-error-msg">' + message + '</span>');
    else
        $('.image-error-msg').text(message);

    return false;
}

function eventsforUploadImages() {
    $('#newImageBtn').on('click', function () {
        if(!$(this).hasClass('disabled'))
            $('#UploadImage').click();
    });

    $('#newGifBtn').on('click', function () {
        if (!$(this).hasClass('disabled'))
            $('#GifImage').click();
    });

    //Check File API support
    if (window.File && window.FileList && window.FileReader) {
        var filesInput = document.getElementById("GifImage");
        var $imgThumbnailsRow = $('#imgThumbnailsRow');

        filesInput.addEventListener("change", function (event) {
            if ($(this).valid()) {
                var file = event.target.files[0]; //FileList object
                var output = document.getElementById("imgThumbnailsRow");

                $('#newImageBtn, #newVideoBtn').addClass("disabled");

                //Only pics
                if (!file.type.match('image'))
                    return;

                var picReader = new FileReader();
                picReader.fileName = file.name;

                picReader.addEventListener("load", function (event) {
                    var picFile = event.target;

                    if ($('.gif-upload-thumbnail').length > 0) {
                        $('.gif-upload-thumbnail').attr({ src: picFile.result, title: picFile.fileName });
                    }
                    else {
                        var div = document.createElement("div");
                        div.classList.add("col");
                        div.classList.add("col-md-5");
                        var figure = document.createElement("figure");

                        figure.innerHTML = "<img class='gif-upload-thumbnail' src='" + picFile.result + "'" +
                                "title='" + picFile.fileName + "'/><button class='gif-remove-thumbnail' type='button' title='Eliminar'>&times;</button>";

                        div.insertBefore(figure, null);
                        output.insertBefore(div, null);                        
                    }

                    $('.gif-remove-thumbnail').on('click', function () {
                        $(this).parent("figure").parent("div").remove();

                        if ($imgThumbnailsRow.is(':empty'))
                            $imgThumbnailsRow.addClass('d-none');

                        $('#newImageBtn, #newVideoBtn').removeClass("disabled");
                        $('#GifImage').val(null);
                    });
                });

                //Read the image
                picReader.readAsDataURL(file);

                if ($imgThumbnailsRow.hasClass('d-none')) {
                    $imgThumbnailsRow.removeClass('d-none');
                }
            }
        });
    }
    else {
        console.log("Tu navegador no soporta File API");
    }

    //Check File API support
    if (window.File && window.FileList && window.FileReader) {        
        var filesInput = document.getElementById("UploadImage");
        var $imgThumbnailsRow = $('#imgThumbnailsRow');

        filesInput.addEventListener("change", function (event) {            
            var files = event.target.files; //FileList object
            var output = document.getElementById("imgThumbnailsRow");
            var allowExt = ["jpg", "jpeg", "png"];
            var filesTotalSize = 0;

            if ($('input[name=ImagesUploaded]').length > 3 || files.length > 4) {                
                return imageErrorMsg("length");                
            }

            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                var fileExt = file.name.substring(file.name.lastIndexOf('.') + 1);
                filesTotalSize = filesTotalSize + files[i].size;

                if (filesTotalSize > 2097152) {
                    // Permitido hasta 2MB
                    return imageErrorMsg("size");
                }

                //Only pics
                if (!file.type.match('image'))
                    continue;                

                if ($.inArray(fileExt, allowExt) == -1) {
                    return imageErrorMsg("extension");
                }

                var picReader = new FileReader();
                picReader.fileName = file.name;

                picReader.addEventListener("load", function (event) {                    
                    var picFile = event.target;

                    var div = document.createElement("div");
                    div.classList.add("col");
                    div.classList.add("col-md-3");
                    var figure = document.createElement("figure");

                    figure.innerHTML = "<input type='hidden' name='ImagesUploaded' value='" + picFile.result + "' /><img class='img-upload-newPost' src='" + picFile.result + "'" +
                            "title='" + picFile.fileName + "'/><button class='img-remove-newPost' type='button' title='Eliminar'>&times;</button>";

                    div.insertBefore(figure, null);
                    output.insertBefore(div, null);

                    $('.img-remove-newPost').on('click', function () {
                        $(this).parent("figure").parent("div").remove();

                        if ($imgThumbnailsRow.is(':empty'))
                            $imgThumbnailsRow.addClass('d-none');                        
                    });
                });

                //Read the image
                picReader.readAsDataURL(file);
            }

            if ($imgThumbnailsRow.hasClass('d-none')) {
                $imgThumbnailsRow.removeClass('d-none');
            }
        });
    }
    else {
        console.log("Tu navegador no soporta File API");
    }
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