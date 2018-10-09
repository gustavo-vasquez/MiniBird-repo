$(document).ready(function () {
    $('#Comment').magicsize();
    $(document).on('click', '#newLinkBtn', addLinkToPost);
    $(document).on('keydown', '#Comment', calculateChars);    
    $(document).on('change', '#GifImage', { thumbnailsRowId: "#imgThumbnailsRow" }, generateThumbnail);
    //$('#NewPostForm, #NewReplyForm').data('unobtrusiveValidation');
});

function newReplySuccess() {
    $('#replyModal').remove();
    scanSpecialWords();
    $.validator.unobtrusive.parse($('#NewReplyForm'));
}

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
            message = "El peso total de imágenes supera los 200kb";
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

function generateThumbnail(event) {
    //Check File API support
    if (window.File && window.FileList && window.FileReader) {
        if ($(this).valid()) {
            var file = event.target.files[0]; //FileList object
            var $thumbnailsRow = $(event.data.thumbnailsRowId);
            
            $('#newImageBtn, #newVideoBtn').addClass("disabled");

            //Only pics
            if (!file.type.match('image'))
                return;

            var picReader = new FileReader();
            picReader.fileName = file.name;

            picReader.addEventListener("load", function (event) {
                console.log("estoy en LOAD");
                var picFile = event.target;

                if ($('.gif-upload-thumbnail').length > 0) {
                    $('.gif-upload-thumbnail').attr({ src: picFile.result, title: picFile.fileName });
                }
                else {
                    // Creo los elementos con su contenido
                    var $wrapper = $('<div></div>');
                    $wrapper.attr({ "class": "col col-md-5" });
                    var $figure = $('<figure></figure>');
                    var $img = $('<img/>');
                    $img.attr({ "class": "gif-upload-thumbnail", src: picFile.result, title: picFile.fileName });
                    var $removeButton = $('<button></button>');
                    $removeButton.attr({ "class": "gif-remove-thumbnail", type: "button", title: "Eliminar" });
                    $removeButton.html("&times;");

                    // Colocando los elementos creados
                    $img.appendTo($figure);
                    $removeButton.appendTo($figure);
                    $figure.appendTo($wrapper);
                    $wrapper.appendTo($thumbnailsRow);                    
                }

                $('.gif-remove-thumbnail').on('click', function () {
                    $(this).parent("figure").parent("div").remove();

                    if ($thumbnailsRow.is(':empty'))
                        $thumbnailsRow.addClass('d-none');

                    $('#newImageBtn, #newVideoBtn').removeClass("disabled");
                    $('#GifImage').val(null);
                });
            });

            //Read the image
            picReader.readAsDataURL(file);

            if ($thumbnailsRow.hasClass('d-none')) {
                $thumbnailsRow.removeClass('d-none');
            }
        }        
    }
    else
        alert("Tu navegador no soporta File API");
}

    //Check File API support
    if (window.File && window.FileList && window.FileReader) {
        var filesInput = document.getElementById("VideoFile");
        var $imgThumbnailsRow = $('#imgThumbnailsRow');

        filesInput.addEventListener("change", function (event) {
            if ($(this).valid()) {
                var file = event.target.files[0]; //FileList object
                var output = document.getElementById("imgThumbnailsRow");                

                $('#newImageBtn, #newGifBtn').addClass("disabled");

                //Only pics
                if (!file.type.match('video'))
                    return;

                var picReader = new FileReader();
                picReader.fileName = file.name;

                picReader.addEventListener("load", function (event) {
                    var picFile = event.target;                    

                    if ($('#videoToUpload').length > 0) {
                        $('#videoToUpload').attr({ src: picFile.result, title: picFile.fileName });
                    }
                    else {
                        var div = document.createElement("div");
                        div.classList.add("col");                        
                        var figure = document.createElement("figure");
                        figure.innerHTML = "<div class='alert alert-dismissible alert-secondary'><button type='button' class='close video-remove-thumbnail' title='Eliminar'>&times;</button><i class='fas fa-video'></i> " + picFile.fileName + "</div><div class='card embed-responsive embed-responsive-16by9'><video id='videoToUpload' src=" + picFile.result + " class='embed-responsive-item' controls></video></div>";

                        div.insertBefore(figure, null);
                        output.insertBefore(div, null);
                    }

                    $('.video-remove-thumbnail').on('click', function () {
                        $(this).closest("figure").parent("div").remove();

                        if ($imgThumbnailsRow.is(':empty'))
                            $imgThumbnailsRow.addClass('d-none');

                        $('#newImageBtn, #newGifBtn').removeClass("disabled");
                        $('#VideoFile').val(null);
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

    ////Check File API support
    //if (window.File && window.FileList && window.FileReader) {        
    //    //var filesInput = document.getElementById("UploadImage");
    //    var filesInput = document.getElementById("ImageFiles");
    //    var $imgThumbnailsRow = $('#imgThumbnailsRow');
    //    var filesTotalSize = 0;

    //    filesInput.addEventListener("change", function (event) {
    //        if ($(this).valid()) {
    //            var files = event.target.files; //FileList object
    //            var output = document.getElementById("imgThumbnailsRow");
    //            //var allowExt = ["jpg", "jpeg", "png"];                
    //            var filesCount = $('input[name=ImagesUploaded]').length + files.length;

    //            if (filesCount > 4)
    //                return imageErrorMsg("length");
    //            else
    //                $('.image-error-msg').remove();

    //            for (var i = 0; i < files.length; i++) {
    //                var file = files[i];
    //                //var fileExt = file.name.substring(file.name.lastIndexOf('.') + 1);
    //                filesTotalSize = filesTotalSize + files[i].size;                    

    //                if (filesTotalSize > 200*1024) {
    //                    // Permitido hasta 2MB
    //                    filesTotalSize = filesTotalSize - files[i].size;
    //                    return imageErrorMsg("size");
    //                }
    //                else
    //                    $('.image-error-msg').remove();

    //                //Only pics
    //                if (!file.type.match('image'))
    //                    continue;

    //                //if ($.inArray(fileExt, allowExt) == -1) {
    //                //    return imageErrorMsg("extension");
    //                //}

    //                var picReader = new FileReader();
    //                picReader.fileName = file.name;

    //                picReader.addEventListener("load", function (event) {
    //                    var picFile = event.target;

    //                    var div = document.createElement("div");
    //                    div.classList.add("col");
    //                    div.classList.add("col-md-3");
    //                    var figure = document.createElement("figure");

    //                    figure.innerHTML = "<input type='hidden' name='ImagesUploaded' value='" + picFile.result + "' /><img class='img-upload-newPost' src='" + picFile.result + "'" +
    //                            "title='" + picFile.fileName + "'/><button class='img-remove-newPost' type='button' title='Eliminar'>&times;</button>";

    //                    div.insertBefore(figure, null);
    //                    output.insertBefore(div, null);

    //                    $('.img-remove-newPost').on('click', function () {
    //                        $(this).parent("figure").parent("div").remove();

    //                        if ($imgThumbnailsRow.is(':empty'))
    //                            $imgThumbnailsRow.addClass('d-none');                                                       
    //                    });
    //                });

    //                //Read the image
    //                picReader.readAsDataURL(file);
    //            }

    //            if ($imgThumbnailsRow.hasClass('d-none')) {
    //                $imgThumbnailsRow.removeClass('d-none');
    //            }
    //        }            
    //    });
    //}
    //else {
    //    console.log("Tu navegador no soporta File API");
    //}

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

//# sourceURL=newPost.js