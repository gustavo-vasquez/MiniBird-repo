var filesTotalSize = 0;

$(document).ready(function () {    
    $(document).on('click', '#newLinkBtn', addLinkToPost);
    $(document).on('keydown', '#Comment', calculateChars);    
    $(document).on('change', '#ImageFiles, #GifImage, #VideoFile', { thumbnailsRowId: "#imgThumbnailsRow", filesTotalSize: 0 }, generateThumbnail);    

    $(document).on('submit', '#NewReplyForm', function (event) {
        // Prevent submit
        event.preventDefault();
        var formData = new FormData($(this)[0]);        

        $.ajax({
            url: "/Account/NewReply",
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function () {
                $('#replying').removeClass('d-none');                
            },
            success: newReplySuccess,
            error: function () {
                alert("Ocurrió un error al procesar la réplica.");
            }
        });
    });
});

function newReplySuccess(data) {
    if ($("#repliesDynamic").length > 0)
        $("#repliesDynamic").html(data);
    else
        $("#repliesFixed").html(data);

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
        $this = $(this);

        if ($this.valid()) {            
            var $thumbnailsRow = $(event.data.thumbnailsRowId);
            var buttonsToDisable,multimediaElement,createHTMLElements,filetype;
            
            switch ($this.attr('id')) {
                case "ImageFiles":
                    buttonsToDisable = "#newGifBtn, #newVideoBtn";
                    multimediaElement = ".images-upload-thumbnail";
                    filetype = "image";
                    createHTMLElements = function (picFile) {
                        // Creo los elementos con su contenido
                        var $wrapper = $('<div></div>');
                        $wrapper.attr({ "class": "col col-md-3" });
                        var $figure = $('<figure></figure>');
                        var $hidden = $('<input/>');
                        $hidden.attr({ type: "hidden", name: "ImagesUploaded", value: picFile.result });
                        var $img = $('<img/>');
                        $img.attr({ "class": multimediaElement.substring(1), src: picFile.result, title: picFile.fileName });
                        var $removeButton = $('<button></button>');
                        $removeButton.attr({ "class": "image-remove-thumbnail", type: "button", title: "Eliminar" });
                        $removeButton.html("&times;");

                        // Colocando los elementos creados
                        $hidden.appendTo($figure);
                        $img.appendTo($figure);
                        $removeButton.appendTo($figure);
                        $figure.appendTo($wrapper);
                        $wrapper.appendTo($thumbnailsRow);
                    };
                    break;
                case "GifImage":
                    buttonsToDisable = "#newImageBtn, #newVideoBtn";
                    multimediaElement = ".gif-upload-thumbnail";
                    filetype = "image";
                    createHTMLElements = function (picFile) {
                        // Creo los elementos con su contenido
                        var $wrapper = $('<div></div>');
                        $wrapper.attr({ "class": "col col-md-5" });
                        var $figure = $('<figure></figure>');
                        var $img = $('<img/>');
                        $img.attr({ "class": multimediaElement.substring(1), src: picFile.result, title: picFile.fileName });
                        var $removeButton = $('<button></button>');
                        $removeButton.attr({ "class": "gif-remove-thumbnail", type: "button", title: "Eliminar" });
                        $removeButton.html("&times;");

                        // Colocando los elementos creados
                        $img.appendTo($figure);
                        $removeButton.appendTo($figure);
                        $figure.appendTo($wrapper);
                        $wrapper.appendTo($thumbnailsRow);
                    };
                    break;
                case "VideoFile":
                    buttonsToDisable = "#newImageBtn, #newGifBtn";
                    multimediaElement = ".video-upload-thumbnail";
                    filetype = "video";
                    createHTMLElements = function (picFile) {
                        // Creo los elementos con su contenido
                        var $wrapper = $('<div></div>');
                        $wrapper.attr({ "class": "col" });
                        var $figure = $('<figure></figure>');
                        var $description = $('<div></div>');
                        $description.attr({ "class": "alert alert-dismissible alert-secondary" });
                        $description.html("<button type='button' class='close video-remove-thumbnail' title='Eliminar'>&times;</button><i class='fas fa-video'></i> " + picFile.fileName);                        
                        var $containerVideo = $('<div></div>');
                        $containerVideo.attr({ "class": "card embed-responsive embed-responsive-16by9" });
                        var $video = $('<video></video>');
                        $video.attr({ "class": multimediaElement.substring(1) + " embed-responsive-item", src: picFile.result, title: picFile.fileName, controls: "controls" });                                               

                        // Colocando los elementos creados                        
                        $description.appendTo($figure);
                        $video.appendTo($containerVideo);
                        $containerVideo.appendTo($figure);
                        $figure.appendTo($wrapper);
                        $wrapper.appendTo($thumbnailsRow);
                    };
                    break;
                default:
                    return;
            }

            $(buttonsToDisable).addClass("disabled label-off");

            if ($this.attr('id') == "ImageFiles") {
                var files = event.target.files; //FileList object
                var filesCount = $('input[name=ImagesUploaded]').length + files.length;

                if (filesCount > 4)
                    return imageErrorMsg("length");
                else
                    $('.image-error-msg').remove();

                for (var i = 0; i < files.length; i++) {
                    var file = files[i];

                    if (!file.type.match(filetype))
                        continue;

                    filesTotalSize = filesTotalSize + files[i].size;

                    if (filesTotalSize > (2*1024*1024)) {
                        // Permitido hasta 2MB
                        filesTotalSize = filesTotalSize - files[i].size;                        
                        return imageErrorMsg("size");
                    }
                    else
                        $('.image-error-msg').remove();                    

                    var picReader = new FileReader();
                    picReader.fileName = file.name;

                    picReader.addEventListener("load", function (event) {                        
                        var picFile = event.target;
                        createHTMLElements(picFile);

                        $('.image-remove-thumbnail').on('click', function () {
                            $(this).closest("figure").parent("div").remove();

                            if ($thumbnailsRow.is(':empty')) {
                                $thumbnailsRow.addClass('d-none');
                                $(buttonsToDisable).removeClass("disabled label-off");
                            }
                                
                            $this.val(null);
                        });
                    });

                    //Read the image
                    picReader.readAsDataURL(file);
                }

                if ($thumbnailsRow.hasClass('d-none')) {
                    $thumbnailsRow.removeClass('d-none');
                }
                return;
            }                       
            
            var file = event.target.files[0]; //FileList object

            if (!file.type.match(filetype))
                return;

            var picReader = new FileReader();
            picReader.fileName = file.name;

            picReader.addEventListener("load", function (event) {                
                var picFile = event.target;
                
                if ($(multimediaElement).length > 0)                    
                    $(multimediaElement).attr({ src: picFile.result, title: picFile.fileName });                
                else
                    createHTMLElements(picFile);

                $('.gif-remove-thumbnail, .video-remove-thumbnail').on('click', function () {
                    $(this).closest("figure").parent("div").remove();

                    if ($thumbnailsRow.is(':empty'))
                        $thumbnailsRow.addClass('d-none');

                    $(buttonsToDisable).removeClass("disabled label-off");
                    $this.val(null);
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

//# sourceURL=newPost.js