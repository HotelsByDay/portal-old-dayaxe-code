Date.prototype.sameDay = function (d) {
    return this.getFullYear() === d.getFullYear()
        && this.getDate() === d.getDate()
        && this.getMonth() === d.getMonth();
}

Date.prototype.greaterThanDate = function (date) {
    return this.getTime() > date.getTime();
}

function equalDate(date1, date2) {
    return date1.getFullYear() === date2.getFullYear()
        && date1.getDate() === date2.getDate()
        && date1.getMonth() === date2.getMonth();
}

function checkShowOut(date) {
    if (window.blockoutDate) {
        for (var i = 0; i < window.blockoutDate; i++) {
            var newD = new Date(window.blockoutDate[i]);
            if (equalDate(date, newD)) {
                return true;
            }
        }
    }
    return false;
}

function checkTickets(date) {
    if (window.tickets && (date.greaterThanDate(new Date()) || date.sameDay(new Date()))) {
        var isShowOut = checkShowOut(date);
        if (!isShowOut) {
            var returnValue = {
                enabled: true,
                classes: 'date-future',
                tooltip: ''
            };
            var dates = window.tickets.Key;
            var defaultV = window.tickets.Value;
            var isDefault = true;
            for (var i = 0; i < dates.length; i++) {
                var curItem = dates[i];
                var nD = new Date(curItem.D);
                if (equalDate(date, nD)) {
                    isDefault = false;
                    returnValue.tooltip = curItem.T;
                    if (parseInt(curItem.T, 10) <= 0) {
                        returnValue.enabled = false;
                    }
                    break;
                }
            }
            
            if (isDefault) {
                var defaultCapacity = defaultV[date.getDay()].T;
                returnValue.tooltip = defaultCapacity;
                if (parseInt(defaultCapacity, 10) <= 0) {
                    returnValue.enabled = false;
                }
                //switch (date.getDay()) {
                //    case 0: // Monday
                //        returnValue.tooltip = defaultV[date.getDay()].T;
                //        break;
                //    case 1: // Tuesday
                //        returnValue.tooltip = defaultV[date.getDay()].T;
                //        break;
                //    case 2: // Wednesday
                //        returnValue.tooltip = defaultV[date.getDay()].T;
                //        break;
                //    case 3: // Thursday
                //        returnValue.tooltip = defaultV[date.getDay()].T;
                //        break;
                //    case 4: // Friday
                //        returnValue.tooltip = defaultV[date.getDay()].T;
                //        break;
                //    case 5: // Saturday
                //        returnValue.tooltip = defaultV[date.getDay()].T;
                //        break;
                //    case 6: // Sunday
                //        returnValue.tooltip = defaultV[date.getDay()].T;
                //        break;
                //    default:
                //        returnValue.tooltip = defaultV[date.getDay()].T;
                //        break;
                //}
            }
            return returnValue;
        }
    }
    return {};
}

function bindTitle() {
    // Insert more than one
    if ($('.datepicker .day[title]').find('.avail-ts').length <= 0) {
        $('.datepicker .day[title]').each(function () {
            var title = $(this).attr('title');
            $(this).append('<span class="avail-ts">' + title + '</span>');
        });
    }
}

function setHeight() {
    $('.item-info').each(function() {
        var height = $(this).closest('.row').find('div:eq(0) img').height();
        $(this).height(height);
    });
}

$(document).ready(function () {
    var hidSelectedDate = $('#HidSelectedDate');
    var hidSelectedDateBefore = $('#HidSelectedDateBefore');

    $(".hoteloverlaywrap").delay(500).fadeOut("slow");
    $(".hotelmodal").delay(500).fadeOut("slow");

    //if ($.jCookie('CheckInDate')) {
    //    // Check In Date Required
    //    if ($('#checkindatem input').val() == "") {
    //        $('#checkindatem input').val($.jCookie('CheckInDate'));
    //        $('#checkindatem').datepicker('update');
    //        $('#checkindatem input').change();
    //        $('#checkindated input').val($.jCookie('CheckInDate'));
    //        $('#checkindated').datepicker('update');
    //    }
    //    $('.date-input:visible').val($.jCookie('CheckInDate'));
    //    hidSelectedDateBefore.val($.jCookie('CheckInDate'));
    //} else {
    //    $('.date-input:visible').val('');
    //}

    $(document).on('click', '.date-input:visible, .open-date:visible', function () {
        $('.date-input:visible').removeClass('selected');
        $('.open-date:visible').removeClass('selected');
        $(this).addClass('selected');
        if ($(this).hasClass('date-input')) {
            hidSelectedDate.val(0);
            //$.jCookie('SelectedCheckInDate', 0);
        } else {
            hidSelectedDate.val(1);
            //$.jCookie('SelectedCheckInDate', 1);
            //$.jCookie('CheckInDate', null);
            hidSelectedDateBefore.val('');
            $('.date-input:visible').val('');
        }
    });

    $('body').on('click', '.date-input:visible', function (e) {
        e.preventDefault();
        $(this).datepicker({
            startDate: "+0d",
            todayHighlight: 'TRUE',
            constrainInput: true,
            autoclose: true,
            clearBtn: true,
            datesDisabled: window.blockoutDate,
            templates: {
                leftArrow: '<img src="/images/arrow-left.png" alt="" class="img-responsive" />',
                rightArrow: '<img src="/images/arrow-right-b.png" alt="" class="img-responsive" />'
            },
            beforeShowDay: function (date) {
                return checkTickets(date);
            }
        }).on('changeDate', function (e) {
            if (e.dates[0] != $('.date-input:visible').datepicker('getDate')) {
                $('.date-input:visible').datepicker('update', e.dates[0]);
            }
        }).on('show', function (e) {
            setTimeout(function () {
                    bindTitle();
                },
                5);
            if ($('.is-mobile').length > 0) {
                var offsetTop = $(document).scrollTop() - 10;
                $('.datepicker-dropdown').css({ 'top': offsetTop });
                $('body').css({ 'overflow': 'hidden' });
            }
        });
        if ($('.date-input:visible').length > 0) {
            // add clearBtn ==> overflow:hidden ??
            $('#body').removeAttr('style');
        }
        $(this).focus();
    });
    jQuery(window).load(function() {
        setHeight();
    });
});

function pageLoad(sender, args) {
    var surveyModal = $('#surveyModal');
    var cancelledBookingModal = $('#cancelledBookingModal');
    if (window.pageLoaded && surveyModal.length == 0) {
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
                    _learnq.push(['track', 'Signed In'],
                        {
                            "referrer": document.referrer
                        });
                }
            }
            $('#authModal').hide();
            $('.modal-open').removeClass('modal-open');

            setTimeout(function () {
                location.reload();
            }, 100);
        } else {
            if ($('.modal-backdrop').hasClass("in")) {
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
            __doPostBack('HotelDetail', null);
            surveyModal.remove();
        }
    } else if (cancelledBookingModal.length > 0) {
        $('#cancelledBookingModal').modal('show');
    }

    if (window.searchCall !== undefined && window.searchCall.toLowerCase() === 'true') {
        $('.listings .row').each(function () {
            var col = $(this).find('.col-lg-6');
            if (col.length === 1) {
                col.addClass('col-odd');
            }
        });
        $('#overlay').addClass('hidden');
        $('.listings').show();
        $('body').removeAttr('style');
    }

    loadImageSequense();

    $("#owl-demo").owlCarousel({
        itemsCustom: [
            [0, 1],
            [450, 1],
            [600, 1],
            [700, 1],
            [1000, 1],
            [1200, 1],
            [1400, 1],
            [1600, 1]

        ],
        navigation: true, // Show next and prev buttons
        slideSpeed: 300,
        paginationSpeed: 400,
        lazyLoad: true,
        navigationText: [
            "<img src='/images/Arrow-Left copy.png' class='img-responsive' alt='previous' />",
            "<img src='/images/Arrow-Right.png' class='img-responsive' alt='next' />"
        ],
        afterLazyLoad: function() {
            $('.lazyOwl').each(function(idx, item) {
                $(item).attr('src', $(item).attr('data-src'));
            });
        }
    });

    $("#owl-product").owlCarousel({
        itemsCustom: [
            [0, 1],
            [450, 1],
            [600, 1],
            [700, 1],
            [1000, 1],
            [1200, 1],
            [1400, 1],
            [1600, 1]

        ],
        navigation: true, // Show next and prev buttons
        slideSpeed: 300,
        paginationSpeed: 400,
        lazyLoad: true,
        navigationText: [
            "<img src='/images/icon_arrow_left_brown.png' class='img-responsive' alt='previous' />",
            "<img src='/images/icon_arrow_right_brown.png' class='img-responsive' alt='next' />"
        ],
        afterLazyLoad: function () {
            $('.lazyOwl').each(function(idx, item) {
                $(item).attr('src', $(item).attr('data-src'));
            });
        }
    });

    $('.review-item .description').shorten({
        showChars: 180,
        onMore: function(e) {
            $('.allcontent:visible').css({'display': 'inline'});
        }
    });

    $('.date-input:visible').change(function () {
        //$.jCookie('CheckInDate', $(this).val());
        hidSelectedDateBefore.val($(this).val());
    });

    $('#checkindatem input').change(function () {
        //$.jCookie('CheckInDate', $('#checkindatem input').val());
        $('#HidSelectedDateBefore').val($('#checkindatem input').val());
    });

    $('#checkindated input').change(function () {
        //$.jCookie('CheckInDate', $('#checkindated input').val());
        $('#HidSelectedDateBefore').val($('#checkindated input').val());
    });

    //var hidSelectedDate = $('#HidSelectedDate');
    //if ($.jCookie('SelectedCheckInDate') == "0") {
    //    $('.open-date:visible').removeClass('selected');
    //    $('.date-input:visible').addClass('selected');
    //    hidSelectedDate.val(0);
    //} else if($.jCookie('SelectedCheckInDate') == "1") {
    //    $('.open-date:visible').addClass('selected');
    //    $('.date-input:visible').removeClass('selected');
    //    $.jCookie('CheckInDate', null);
    //    hidSelectedDate.val(1);
    //}

    $('#checkindatem').datepicker({
        startDate: window.restrictDate,
        todayHighlight: 'TRUE',
        autoclose: true,
        clearBtn: true,
        multidate: false,
        defaultViewDate: new Date(),
        datesDisabled: window.blockoutDate,
        templates: {
            leftArrow: '<img src="/images/arrow-left.png" alt="" class="img-responsive" />',
            rightArrow: '<img src="/images/arrow-right-b.png" alt="" class="img-responsive" />'
        },
        format: 'mm/dd/yyyy',
        beforeShowDay: function (date) {
            return checkTickets(date);
        }
    }).on('changeDate', function (e) {
        if (e.dates.length <= 0) {
            //$.jCookie('CheckInDate', null);
            $('#HidSelectedDateBefore').val('');
            $('#checkindatem input').change();
        }
    }).on('show', function (e) {
        setTimeout(function () {
            bindTitle();
        },
            5);
        
        if ($('.is-mobile').length > 0) {
            var offsetTop = $(document).scrollTop() - 10;
            $('.datepicker-dropdown').css({ 'top': offsetTop });
            $('body').css({ 'overflow': 'hidden' });
        }
    });

    $('#checkindated').datepicker({
        startDate: window.restrictDate,
        todayHighlight: 'TRUE',
        autoclose: true,
        clearBtn: true,
        multidate: false,
        datesDisabled: window.blockoutDate,
        templates: {
            leftArrow: '<img src="/images/arrow-left.png" alt="" class="img-responsive" />',
            rightArrow: '<img src="/images/arrow-right-b.png" alt="" class="img-responsive" />'
        },
        format: 'mm/dd/yyyy',
        beforeShowDay: function (date) {
            return checkTickets(date);
        }
    }).on('changeDate', function (e) {
        if (e.dates.length <= 0) {
            //$.jCookie('CheckInDate', null);
            $('#HidSelectedDateBefore').val('');
            $('#checkindated input').change();
        }
    }).on('show', function (e) {
        setTimeout(function () {
            bindTitle();
        },
            5);
        if ($('.is-mobile').length > 0) {
            var offsetTop = $(document).scrollTop() - 10;
            $('.datepicker-dropdown').css({ 'top': offsetTop });
            $('body').css({ 'overflow': 'hidden' });
        }
    });
    if ($('#checkindatem').length > 0 || $('#checkindated').length > 0) {
        // add clearBtn ==> overflow:hidden ??
        $('#body').removeAttr('style');
    }

    $('.ticket-capacity a').click(function (e) {
        var currentValue = $(this).text();
        var currentTicket = currentValue.match(/\d+/g)[0];
        $('.current-value').text(currentValue);
        //$.jCookie('CurrentTicket', currentTicket);
        //$.jCookie('CurrentTicketValue', currentValue);
        $('.total-tickets-text').val(currentTicket);
        $('.total-tickets-text').change();
    });

    var isWaitingList = ($('#IsWaitingListHidden').val().toLowerCase() === 'true');
    var isWaitingListCtrl = $('#isWaitingList').toggles({
        on: isWaitingList,
        width: 70,
        height: 30,
        drag: true,
        easing: 'linear',
        animate: 300
    });
    $(isWaitingListCtrl).on('toggle', function (e, active) {
        if (active) {
//            console.log('Toggle is now ON!');
            $('#addToWaitingListModal').modal('show');
        }
        $('#IsWaitingListHidden').val(active ? 'true' : 'false');
    });

    var isWaitingListM = ($('#IsWaitingListMHidden').val().toLowerCase() === 'true');
    var isWaitingListMCtrl = $('#isWaitingListM').toggles({
        on: isWaitingListM,
        width: 70,
        height: 30,
        drag: true,
        easing: 'linear',
        animate: 300
    });
    $(isWaitingListMCtrl).on('toggle', function (e, active) {
        if (active) {
//            console.log('Toggle is now ON!');
            $('#addToWaitingListModal').modal('show');
        }
        $('#IsWaitingListMHidden').val(active ? 'true' : 'false');
    });

    $('.toggle-blob').on('touchstart',
        function () {
            var toogleVal = ($('#IsWaitingListMHidden').val().toLowerCase() === 'true');
            $('#isWaitingListM').toggles(!toogleVal);
        });

    $('.waitlist-date').datepicker({
        startDate: "+0d",
        todayHighlight: 'TRUE',
        autoclose: true,
        format: 'MM dd, yyyy',
        templates: {
            leftArrow: '<img src="/images/arrow-left.png" alt="" class="img-responsive" />',
            rightArrow: '<img src="/images/arrow-right-b.png" alt="" class="img-responsive" />'
        }
    });

    $('#addToWaitingListModal').on('hidden.bs.modal', function () {
        if ($('#IsWaitingListMHidden').val() === 'false') {
            $('#isWaitingList').toggles(false);
            $('#isWaitingListM').toggles(false);
        }
    });
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