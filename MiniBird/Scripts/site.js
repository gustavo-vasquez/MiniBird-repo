$(document).ready(function () {
    $('.dropdown-menu > a, .dropdown-menu > form > button').hover(function () {
        $(this).toggleClass('active');
    });

    $('#postModal').on('shown.bs.modal', function () {
        $('#Comment').focus();
    })
});

function goTop() {
    $('body,html').animate({
        scrollTop: 0
    }, 800);
    return false;
}