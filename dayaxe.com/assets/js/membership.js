$(function() {
    $('.dropdown').hover(function () {
        $(this).addClass('open');
    }, function () {
        $(this).removeClass('open');
    });

    $('.navbar-toggle').click(function (e) {
        $('.overlay-page').show();
        e.preventDefault();
        var target = $(this).attr('data-target');
        var height = $(window).height();
        $(target).css({ 'opacity': 1, 'right': '0px', 'min-height': height });
        $('body').css({ 'overflow': 'hidden' });
        $('header.reviews').css({ 'z-index': 3 });
    });
    $('#header-nav').on('show.bs.collapse', function (event) {
        event.preventDefault();
    }).on('hide.bs.collapse', function (event) {
        event.preventDefault();
    });
    $('.close-menu').click(function (e) {
        $('#header-nav').css({ 'right': '-240px' });
        $('body').removeAttr('style');
        $('.overlay-page').hide();
        $('header.reviews').removeAttr('style');
    });
    
    $('.btn-cancel-membership').click(function () {
        $('#cancelMembershipModal').modal('show');
    });

    $('.btn-active-membership').click(function () {
        $('#activeMembershipModal').modal('show');
    });
});