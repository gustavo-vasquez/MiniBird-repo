$(document).on('keydown', { }, function (event) {
    if (!($(document.activeElement).is('textarea') || $(document.activeElement).is('input'))) {
        switch (event.keyCode) {
            case 77:
                $('#profileMenuLink').trigger('click');
                break; // key m
            case 78:
                event.data.action = "post";
                drawPublication(event);
                break; // key n
        }        

        if ($('#viewingPostDynamic').length > 0) {
            var containerDiv = $('#viewingPostDynamic > .card-body').find('.interact-buttons');

            switch (event.keyCode) {
                case 80:
                    sendARepost(containerDiv);
                    break; // key p
                case 76:
                    giveALike(containerDiv);
                    break; // key l
                case 82:
                    event.data.action = "reply";
                    event.data.thisElement = $('#replyBtnDynamic');
                    drawPublication(event);
                    break; // key r
                case 27:
                    closePost();
                    break; // key esc
            }
        }
        else if ($('#viewingPostFixed').length > 0) {
            var containerDiv = $('#viewingPostFixed > .card-body').find('.interact-buttons');

            switch (event.keyCode) {
                case 80:
                    sendARepost(containerDiv);
                    break; // key p
                case 76:
                    giveALike(containerDiv);
                    break; // key l
                case 82:
                    event.data.action = "reply";
                    event.data.thisElement = $('#replyBtnFixed');
                    drawPublication(event);
                    break; // key r
                case 27:
                    closePost();
                    break; // key esc
            }
        }        
    }
});

$(document).on('keydown keyup', { map: {}, activeUser: $('#profileMenuLink').data('active-user') }, function (event) {
    if (!($(document.activeElement).is('textarea') || $(document.activeElement).is('input'))) {
        if (twoPressedKeys(event, "g", "h", event.data.map))
            window.location.href = "/Account/Timeline";

        if (twoPressedKeys(event, "g", "i", event.data.map))
            window.location.href = "/Account/ProfileScreen/" + event.data.activeUser + "/lists";

        if (twoPressedKeys(event, "g", "p", event.data.map))
            window.location.href = "/Account/ProfileScreen/" + event.data.activeUser;

        if (twoPressedKeys(event, "g", "l", event.data.map))
            window.location.href = "/Account/ProfileScreen/" + event.data.activeUser + "/likes";
    }    
});