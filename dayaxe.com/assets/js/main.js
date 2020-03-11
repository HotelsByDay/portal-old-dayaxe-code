/*

	Spectral by HTML5 UP

	html5up.net | @n33co

	Free for personal and commercial use under the CCA 3.0 license (html5up.net/license)

*/

var _learnq = _learnq || [];

function login() {
    window.pageLoaded = true;
    $('#authModal').modal('show');
    $('#authModal').on('hidden.bs.modal', function (e) {
        window.pageLoaded = false;
        window.showAuth = false;
    });
}

function reloadSurveyEvent() {
    function getRateTitle() {
        switch (parseInt($('#Rating').val(), 10)) {
            case 0:
            case 1:
                $('.survey .title').text("Poor");
                break;
            case 2:
                $('.survey .title').text("Fair");
                break;
            case 3:
                $('.survey .title').text("Neutral");
                break;
            case 4:
                $('.survey .title').text("Very Good");
                break;
            case 5:
                $('.survey .title').text("Excellent");
                break;
        }
    }

    $("#Rating").rating({
        step: 1,
        starCaptions: { 1: 'Poor', 2: 'Fair', 3: 'Neutral', 4: 'Very Good', 5: 'Excellent' },
        min: 0,
        max: 5
    });
    $('#Rating').rating('refresh', { showClear: false, showCaption: false });
    $('#Rating').on('change', function () {
        $('.rate-commend').removeClass('hidden');
        $('#NextButton').removeClass('btngray');
        getRateTitle();
    });
    if ($('#Rating').length > 0 && $('#Rating').val() != 0) {
        $('.rate-commend').removeClass('hidden');
        $('#NextButton').removeClass('btngray');
    }

    $('#surveyModal .btn-use, #surveyModal .btn-yes, #surveyModal .btn-no').on('click touchstart', function () {
        $('#NextButton').removeClass('btngray');
    });

    if ($(".btn-use-selected").length > 0 || $('#surveyModal .title').text().toLowerCase().indexOf("thank") != -1) {
        $('#NextButton').removeClass('btngray');
    }

    $('#surveyModal .next-button').on('click touchstart', function () {
        if ($(this).hasClass('btngray')) {
            return false;
        }
        $('.progress').css({ 'visibility': 'inherit' });
    });
    // Tab 1
    $('#surveyModal .btn-use').on('click touchstart', function () {
        $(this).toggleClass('btn-use-selected');
        $(this).next('input').val($(this).hasClass('btn-use-selected'));
        return false;
    });

    // Tab 3
    $('#UseFoodDrinkText').slider({
        min: 10,
        max: 1000,
        scale: 'logarithmic',
        step: 5,
        tooltip: 'always',
        formatter: function (value) {
            return '$' + value;
        }
    });

    $('#surveyModal #useFoodDrinkButton').on('click touchstart', function () {
        $('.use-food-drink-price').removeClass('hidden');
        $(this).addClass('btn-use-selected');
        return false;
    });

    // Tab 5
    $('#UseSpaServiceText').slider({
        min: 10,
        max: 1000,
        scale: 'logarithmic',
        step: 5,
        tooltip: 'always',
        formatter: function (value) {
            return '$' + value;
        }
    });

    $('#surveyModal #UseSpaServiceButton').on('click touchstart', function () {
        $('.use-spa-service-price').removeClass('hidden');
        $(this).addClass('btn-use-selected');
        return false;
    });

    // Tab 6
    $('#UseAdditionalServiceText').slider({
        min: 10,
        max: 1000,
        scale: 'logarithmic',
        step: 5,
        tooltip: 'always',
        formatter: function (value) {
            return '$' + value;
        }
    });

    $('#surveyModal #UseAdditionalServiceButton').on('click touchstart', function () {
        $('.use-additional-service-price').removeClass('hidden');
        $(this).addClass('btn-use-selected');
        return false;
    });
}

function pageLoad(sender, args) {
    var surveyModal = $('#surveyModal');
    if (window.pageLoaded && surveyModal.length <= 0) {
        $('#overlayAuth').addClass('hidden');
        if (window.userSession) {
            $('.modal-backdrop').remove();
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
            if (window.useremail) {
                _learnq.push(['identify', {
                    '$email': window.useremail,
                    "referral_code": '' + (window.register ? window.referralcode : '') + ''
                }]);
                if (window.register) {
                    _learnq.push(['track', 'Signed Up',
                    {
                        "referrer": document.referrer,
                        "referral_code": '' + window.referralcode + ''
                    }]);
                } else {
                    _learnq.push(['track', 'Signed In',
                    {
                        "referrer": document.referrer
                    }]);
                }
            }
            $('#authModal').hide();
            $('.modal-open').removeClass('modal-open');

            setTimeout(function () {
                location.reload();
            }, 100);
        } else if ($('#authModal').length > 0) {
            if ($('.modal-backdrop').hasClass("in") && $('.modal-backdrop').length > 0) {
                $('.modal-backdrop').remove();
            }
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
    
    $('#overlay').addClass('hidden');
    $('.listings').show();
    $('body').removeAttr('style');

    var filterResult = $('#FilterResultHidden');
    if (filterResult.length > 0) {
        $('.filter-result').html(jQuery('<div></div>').html(filterResult.val()).text());
        $('.btn-view-passes').html('SEE ' + $('.filter-result b').text() + ' PASSES');
        $('.btn-view-passes').click(function (e) {
            closeFilter();
            e.preventDefault();
        });
    }

    loadImageSequense();
    setWidthSearch();

    for (var i = 0; i < $(".single_room_wrapper").length; i++) {
        var item = $(".single_room_wrapper:eq(" + i + ")");
        var child = item.children();
        child.click(function () {
            var atag = $(this).find('a:eq(0)');
            atag.click();
        });
    }
}
 
function loadImageSequense() {
    var img = $("img.lazy").first();
    var src = img.attr('data-original');
    if (img.length > 0) {
        $("<img />").load(src, function (response, status, xhr) {
            img.attr('src', src);
            img.removeClass('lazy');

            loadImageSequense();
        });
    }
}

function resetFilter() {
    resetTypeProduct();
    clearPrice();
    clearDistance();
}

function setBackgroundOverlay() {
    if ($('#NavHeader').length > 0) {
        var headerOffset = $('#NavHeader').offset().top + $('#NavHeader').height();
        var imageTopHeight = $('header.search').height();
        if (headerOffset > imageTopHeight) {
            $('#NavHeader .color-overlay').css({ 'background-color': '#000000' });
        } else {
            $('#NavHeader .color-overlay').css({ 'background-color': 'transparent' });
        }
    }
}

function TriggerPostBack(arg) {
    __doPostBack(window.hotelList, arg);
}

function initPriceAndDistance() {
    if ($('#PriceText').length > 0) {
        $('#PriceText').slider({
            value: $('#PriceText').data('slider-value'),
            scale: 'linear',
            range: true,
            tooltip_split: true,
            tooltip_position: 'bottom',
            step: 1,
            tooltip: 'always',
            formatter: function (value) {
                return '$' + value;
            }
        }).on('slideStop',
            function (e) {
                setSearchFilter(null, true, null);
                __doPostBack(window.hotelList, "false");
            });
    }

    if ($('#DistanceText').length > 0) {
        $('#DistanceText').slider({
            min: $('#DistanceText').data('slider-min'),
            max: $('#DistanceText').data('slider-max'),
            value: $('#DistanceText').data('slider-value'),
            scale: 'linear',
            range: true,
            tooltip_split: true,
            tooltip_position: 'bottom',
            step: 1,
            tooltip: 'always',
            formatter: function (value) {
                if (!isNaN(value)) {
                    return value + 'mi';
                }
            }
        }).on('slideStop',
            function (e) {
                setSearchFilter(null, null, true);
                __doPostBack(window.hotelList, "false");
            });
    }
}

function setLoadingStyle() {
    //$('#overlay').css({ 'width': $('#overlay').width() - 10, 'left': 6 });
}

(function($) {
    initPriceAndDistance();
	skel

		.breakpoints({
			xlarge:	'(max-width: 1680px)',
			large:	'(max-width: 1280px)',
			medium:	'(max-width: 980px)',
			small:	'(max-width: 736px)',
			xsmall:	'(max-width: 480px)'
		});


	$(function () {
	    var $window = $(window),

            $body = $('body'),
            $wrapper = $('#page-wrapper'),

	        $headerTag = $('header'),

	        $footerTag = $('footer'),

	        $loading = $('#overlay'),

            $visit = $('#hdVisit'),
            $amennities = $('.access-amenities'),

            $loadingmheight = $wrapper.height() - $amennities.outerHeight() - $footerTag.outerHeight() - 101,

	        $authControl = $('#authModal'),
	            
            $surveyModal = $('#surveyModal');

	        $(document).ajaxStart(function () {
	            $('.progress').removeClass('hidden');
	        });
	        $(document).ajaxStop(function () {
	            //$('.progress').addClass('hidden');
	        });
		    // Disable animations/transitions until the page has loaded.

			$body.addClass('is-loading');
        
			$window.on('load', function() {

				window.setTimeout(function() {

					$body.removeClass('is-loading');

				}, 100);

			});

			$(window).scroll(function () {
                setBackgroundOverlay();
			    setLoadingStyle();
			});

	    // Sign in popup show
			if (($surveyModal.length <= 0
                && $authControl.length > 0)
                && window.showAuth !== undefined
                && window.showAuth === 'True') {
			    window.pageLoaded = true;
			    $authControl.modal('show');
			}

	        // Survey popup show
		    if ($surveyModal.length > 0) {
		        $surveyModal.modal('show');
		    }
		// Mobile?

			if (skel.vars.mobile) {
			    $body.addClass('is-mobile');
			    $loadingmheight = $window.height() - $headerTag.outerHeight();
			}
			else

				skel

					.on('-medium !medium', function() {

						$body.removeClass('is-mobile');

					})

					.on('+medium', function() {

						$body.addClass('is-mobile');

					});

			if ($visit.val() == 'false') {
			    $visit.val('true');

			    //Hide content when load (code/ajax get data)
			    $loading.css({ 'min-height': $loadingmheight });
			    $('.listings').hide();
			    $loading.find('#loading').removeClass('hidden');
			}
			else {
			    $loading.addClass('hidden');
			    $('.listings').show();
                $(window).scroll();
			}
			if (typeof addToHomescreen !== "undefined")
			    addToHomescreen();

		// Fix: Placeholder polyfill.

			$('form').placeholder();



		// Prioritize "important" elements on medium.

			skel.on('+medium -medium', function() {

				$.prioritize(

					'.important\\28 medium\\29',

					skel.breakpoint('medium').active

				);

			});


		// Menu.

			$('#menu')

				.append('<a href="#menu" class="close"></a>')

				.appendTo($body)

				.panel({

					delay: 500,

					hideOnClick: true,

					hideOnSwipe: true,

					resetScroll: true,

					resetForms: true,

					side: 'right',

					target: $body,

					visibleClass: 'is-menu-visible'

				});



		// Header.

			if ($('#surveyModal').length > 0) {
			    $('#surveyModal').modal('show');
			    $('body').css({ 'overflow': 'hidden' });
			}

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
        });
        $('#header-nav').on('show.bs.collapse', function (event) {
            event.preventDefault();
        }).on('hide.bs.collapse', function (event) {
            event.preventDefault();
        });
        $('.close-menu').click(function (e) {
            e.preventDefault();
            $('#header-nav').css({ 'right': '-240px' });
            $('body').removeAttr('style');
            $('.overlay-page').hide();
        });

        // loading and click to menu
        if ($('.is-mobile').length > 0 && $('#header-nav').length > 0 && $('#header-nav').position().left < $(window).width() / 2) {
            $('body').css({ 'overflow': 'hidden' });
        }
        
        //if (!$('body').hasClass('is-mobile') && $('.faqs li a').length > 0) {
        //    $('.collapse:eq(0)').collapse();
        //}

	    loadImageSequense();
	    setLoadingStyle();
	});

})(jQuery);