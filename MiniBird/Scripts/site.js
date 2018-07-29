$(document).ready(function () {
    $('.dropdown-menu').on('mouseover', 'a', function () {
        $(this).addClass('active');
    }).on('mouseleave', 'a', function () {
        $(this).removeClass('active');
    });    
});