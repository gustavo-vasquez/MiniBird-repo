$(document).ready(function () {    
    $('.profile_screen-trends').marquee('pointer').mouseover(function () {
        $(this).trigger('stop');
    }).mouseout(function () {
        $(this).trigger('start');
    }).mousemove(function (event) {
        if ($(this).data('drag') == true) {
            this.scrollLeft = $(this).data('scrollX') + ($(this).data('x') - event.clientX);
        }
    }).mousedown(function (event) {
        $(this).data('drag', true).data('x', event.clientX).data('scrollX', this.scrollLeft);
    }).mouseup(function () {
        $(this).data('drag', false);
    });

    $('#profileScreen-nav a').hover(function () {
        $(this).not('.selected').toggleClass('active');
    });

    var $profileScreenNav = $('#profileScreen-nav');
    switch ($profileScreenNav.data('tab')) {
        case "following": $profileScreenNav.children('li:nth-child(2)').children('a').addClass('active selected');
            break;
        case "followers": $profileScreenNav.children('li:nth-child(3)').children('a').addClass('active selected');
            break;
        case "likes": $profileScreenNav.children('li:nth-child(4)').children('a').addClass('active selected');
            break;
        case "lists": $profileScreenNav.children('li:last').children('a').addClass('active selected');
            break;
        default: $profileScreenNav.children('li:first').children('a').addClass('active selected');
            break;
    }    

    document.getElementById("headerFile").onchange = function () {
        var data = new FormData();
        var files = $("#headerFile").get(0).files;
        if (files.length > 0) {
            data.append("ImageFile", files[0]);

            $.ajax({
                url: "/Account/ChangeHeader",
                type: "POST",
                processData: false,
                contentType: false,
                data: data,
                success: function (response) {
                    //code after success
                    $('.profile_screen-header').attr('src', response);
                },
                error: function (response) {
                    console.log("Ocurrió un error.");
                }
            });
        }
    };

    document.getElementById("avatarFile").onchange = function () {
        var data = new FormData();
        var files = $("#avatarFile").get(0).files;
        if (files.length > 0) {
            data.append("ImageFile", files[0]);

            $.ajax({
                url: "/Account/ChangeAvatar",
                type: "POST",
                processData: false,
                contentType: false,
                data: data,
                success: function (response) {
                    //code after success
                    $('.profile_screen-avatar').attr('src', response);
                },
                error: function (response) {
                    console.log("Ocurrió un error.");
                }
            });
        }
    };

    $('#editDetails').on('click', function () {
        $.ajax({
            url: "/Account/EditDetailsForm",
            method: "GET",
            success: function (response) {
                $('.profile-details').html(response);
                $.validator.unobtrusive.parse($('#editDetailsForm'));                

                function changeMade(data) {
                    console.log(data.personalDescription);
                }
            }
        });
    });
});