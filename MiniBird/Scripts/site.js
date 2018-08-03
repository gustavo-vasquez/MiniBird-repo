$(document).ready(function () {
    //$('.dropdown-menu').on('mouseover', 'a', function () {
    //    $(this).addClass('active');
    //}).on('mouseleave', 'a', function () {
    //    $(this).removeClass('active');
    //});

    $('.dropdown-menu > a, .dropdown-menu > button').hover(function () {
        $(this).toggleClass('active');
    });
});

function goTop() {
    $('body,html').animate({
        scrollTop: 0
    }, 800);
    return false;
}