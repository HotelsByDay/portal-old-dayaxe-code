var _learnq = _learnq || [];

function login() {
    window.pageLoaded = true;
    $('#authModal').modal('show');
    $('#authModal').on('hidden.bs.modal', function (e) {
        window.pageLoaded = false;
        window.showAuth = false;
    });
}

function pageLoad(sender, args) {
    var surveyModal = $('#surveyModal');
    if (window.pageLoaded && surveyModal.length <= 0) {
        $('.modal-backdrop').remove();
        $('#overlayAuth').addClass('hidden');
        if (window.userSession) {
            if (window.useremail) {
                _learnq.push(['identify', {
                    '$email': window.useremail,
                    "referral_code": '' + (window.register ? window.referralcode : '') + ''
                }]);
                if (window.register) {
                    console.log(window.useremail);
                    _learnq.push(['track', 'Signed Up',
                    {
                        "referrer": document.referrer,
                        "referral_code": '' + window.referralcode + ''
                    }]);
                } else {
                    _learnq.push(['track', 'Signed In'],
                    {
                        "referrer": document.referrer
                    });
                }
            }
            if (window.mixpanel && window.useremail) {
                mixpanel.register_once({
                    "Date Created": new Date(),
                    "Email": window.useremail,
                    "$email": window.useremail
                });
                if (window.register) {
                    window.mixpanel.alias(window.useremail);
                    window.mixpanel.track("Signed Up", { "referrer": document.referrer });
                } else {
                    window.mixpanel.identify(window.useremail);
                    window.mixpanel.track("Signed In", { "referrer": document.referrer });
                }
            }
            $('#authModal').hide();
            $('.modal-open').removeClass('modal-open');
            setTimeout(function() {
                location.reload();
            }, 100);
        }
        else if ($('#authModal').hasClass('modal')) {
            $('#authModal').modal('show');
            $('#authModal').on('hidden.bs.modal', function (e) {
                window.pageLoaded = false;
                window.showAuth = false;
            });
        }
    } else if (surveyModal.length > 0) {
        if (window.userSurvey === undefined) {
            reloadSurveyEvent();
            $('.modal-backdrop').remove();
            $('#surveyModal').modal('show');
        } else {
            $('#surveyModal').modal('hide');
        }
    }

    if ($("#owl-product-featured").length > 0) {
        $("#owl-product-featured").owlCarousel({
            itemsCustom: [
                [0, 1],
                [450, 1],
                [600, 2],
                [700, 2],
                [900, 3],
                [1000, 4],
                [1200, 4],
                [1400, 4],
                [1600, 4]
            ],
            navigation: true, // Show next and prev buttons
            slideSpeed: 300,
            paginationSpeed: 400,
            lazyLoad: true,
            navigationText: [
                "<img src='/images/icon_arrow_left_brown.png' class='img-responsive' alt='previous' />",
                "<img src='/images/icon_arrow_right_brown.png' class='img-responsive' alt='next' />"
            ],
            afterLazyLoad: function() {
                $('.lazyOwl').each(function(idx, item) {
                    $(item).attr('src', $(item).attr('data-src'));
                });
            }
        });
    }
    window.pageLoaded = true;
}

function checkValue(control) {
    if ($(control).val() !== '') {
        if (!$(control).hasClass('has-value')) {
            $(control).addClass('has-value');
        }
    } else {
        $(control).removeClass('has-value');
    }
}

function setBackgroundOverlay() {
    var headerOffset = $('#NavHeader').offset().top + $('#NavHeader').height();
    var imageTopHeight = $('.banner').height();
    if (headerOffset > imageTopHeight) {
        $('#NavHeader .color-overlay').css({ 'background-color': '#000000' });
    } else {
        $('#NavHeader .color-overlay').css({ 'background-color': 'transparent' });
    }
}

function setHeightPage() {
    var contentH1 = $('.content').prev('div');
    var contentH;
    if (contentH1.length > 0) {
        contentH = contentH1.outerHeight();
    } else {
        contentH = $('.content').outerHeight();
    }
    var footer = $('footer').outerHeight();
    var windowH = $(window).height();

    if (contentH + footer < windowH) {
        var navHeaderH = $('#NavHeader').height();
        if (contentH1.length > 0) {
            contentH1.height(windowH - footer - navHeaderH);
        } else {
            $('.content').height(windowH - footer - navHeaderH);
        }
        var height = windowH - footer - navHeaderH;
        if ($('.content #authModal').length > 0 && height < 650) {
            height = 650;
        }
        if (contentH1.length > 0) {
            contentH1.css({ 'height': height - 88, 'min-height': 650 });
        } else {
            $('.content').css({ 'height': height - 88, 'min-height': 650 });
        }
    }
}

function initDefault() {
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
        $('header').css({ 'z-index': 3 });
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
        $('header').removeAttr('style');
    });
}

(function ($) {
    if (window.showAuth != undefined && window.showAuth === 'True'){
        window.pageLoaded = true;
    }

    initDefault();

    $(window).scroll(function() {
        setBackgroundOverlay();
    });

    setHeightPage();

    // Credits Page
    $('.personal-code input').blur(function () {
        checkValue(this);
    });

    $('.personal-code input').keyup(function() {
        checkValue(this);
    });

    // Membership Details
    $('.btn-cancel-membership').click(function() {
        $('.cancelMembershipModal').modal('show');
    });

    $('.btn-active-membership').click(function() {
        
    });
})(jQuery);