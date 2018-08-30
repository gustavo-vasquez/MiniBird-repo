$(document).on('keydown', function (event) {
    if (!$(document.activeElement).is('textarea')) {
        switch (event.keyCode) {
            case 77:
                $('#profileMenuLink').trigger('click');
                break; // key m
            case 78:
                newPost();
                break; // key n
        }

        //if (event.keyCode === 71 && event.keyCode === 72) // keys g h
        //    window.location.href = "/Account/Timeline";

        //if (event.keyCode === 71 && event.keyCode === 73) // keys g i
        //    window.location.href = "/Account/ProfileScreen/lists";

        //if (event.keyCode === 71 && event.keyCode === 78) // keys g n
        //    window.location.href = "/Account/Notifications";

        //if (event.keyCode === 71 && event.keyCode === 80) // keys g p
        //    window.location.href = "/Account/ProfileScreen";

        //if (event.keyCode === 71 && event.keyCode === 72) // keys g l
        //    window.location.href = "/Account/ProfileScreen/likes";

        if ($('#viewingPost').length > 0) {
            var containerDiv = $('#viewingPost > .card-body').find('.interact-buttons');

            switch (event.keyCode) {
                case 80:
                    sendARepost(containerDiv);
                    break; // key p
                case 76:
                    giveALike(containerDiv);
                    break; // key l
                case 82:
                    newReply();
                    break; // key r
                case 27:
                    closePost();
                    break; // key esc
            }            
        }
    }
});