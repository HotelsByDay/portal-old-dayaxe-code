searchFilter = {};

function showLoading() {
    $('#overlay').removeClass('hidden');
    $('body').css({ 'overflow': 'hidden' });
}

function setWidthSearch() {
    var mRight = $(window).width() - $('.navbar-right').outerWidth() - 130;
    var searchWidth = $('.nav-search').outerWidth();

    if (mRight < searchWidth && $(window).width() <= 1420) {
        $('.row-search').css({ 'max-width': mRight });
    }

    if ($(window).width() >= 1420) {
        $('.row-search').css({ 'max-width': 900 });
    }

    var pos = $('.access-amenities').offset().left;
    if (pos < 130) {
        pos = 130;
    }
    $('.row-search').css({ 'left': pos });
}

function clearPrice() {
    var minValue = $('#PriceText').data('slider-min');
    var maxValue = $('#PriceText').data('slider-max');
    $('#PriceText').slider('setValue', [minValue, maxValue]);
    clearFilter(null, true, null);
    setSearchFilter(null, null, null);
}

function clearDistance() {
    var minValue = $('#DistanceText').data('slider-min');
    var maxValue = $('#DistanceText').data('slider-max');
    $('#DistanceText').slider('setValue', [minValue, maxValue]);
    clearFilter(null, null, true);
    setSearchFilter(null, null, null);
}

function resetTypeProduct() {
    $('.btn-link-search').each(function () {
        if (!$(this).hasClass('active')) {
            $(this).addClass('active');
        }
    });

    if ($(window).width() < 992) {
        $('.btn-clear-all').css({ 'color': '#13C1F4', 'left': 0 });
    }
    clearFilter(true, null, null);
    setSearchFilter(null, null, null);
}
function setSearchFilter(amenities, price, distance) {
    var btnFilter = $('.btn-filter');
    var totalFilter = checkFilter(amenities, price, distance);
    if (totalFilter > 0) {
        btnFilter.addClass('has-filter');
        btnFilter.find('span').text(totalFilter);
    } else {
        $('.btn-filter').removeClass('has-filter');
    }
}

function checkFilter(amenities, price, distance) {
    var result = 0;
    if (amenities) {
        searchFilter.amenities = true;
    }
    if (price) {
        searchFilter.price = true;
    }
    if (distance) {
        searchFilter.distance = true;
    }

    if (searchFilter.amenities) {
        result += 1;
    }
    if (searchFilter.price) {
        result += 1;
    }
    if (searchFilter.distance) {
        result += 1;
    }
    return result;
}

function clearFilter(amenities, price, distance) {
    if (amenities) {
        delete searchFilter.amenities;
    }
    if (price) {
        delete searchFilter.price;
    }
    if (distance) {
        delete searchFilter.distance;
    }
}

function openFilter() {
    $('.btn-filter').click(function (e) {
        $('#NavHeader').addClass('hidden');
        $('body').css({ 'overflow': 'hidden' });

        $('.row-select-product').css({ 'display': 'block' });
        e.preventDefault();
    });
}

function closeFilter() {
    $('#NavHeader').removeClass('hidden');
    $('body').removeAttr('style');
    $('.row-select-product').removeAttr('style');
}

(function ($) {
    $(function () {
        $('#checkindate .datepicker').daterangepicker({
            autoApply: true,
            minDate: new Date(window.restrictDate),
            opens: 'center',
            locale: {
                format: 'ddd, MMM DD',
                "separator": " - "
            },
            "dateLimit": {
                "days": 15
            },
            startDate: moment(window.startDate),
            endDate: moment(window.endDate)
        }).on('show.daterangepicker', function (ev, picker) {
            $('#checkindate input').attr('readonly', 'readonly');
            if ($('.is-mobile').length > 0) {
                var offsetTop = $(document).scrollTop() - 10;
                $('.daterangepicker.dropdown-menu').css({ 'top': offsetTop });
            }
        }).on('apply.daterangepicker', function (ev, picker) {
            $('#checkindate input').removeAttr('readonly');
            showLoading();
            __doPostBack(window.hotelList, null);
        });

        $('.btn-clear-all').click(function (e) {
            resetTypeProduct();
            clearPrice();
            clearDistance();
            eval($(this).attr('href').replace('\'\'', 'true').replace('javascript:', ''));
            e.preventDefault();
        });

        $('.clear-amenities').click(function(e) {
            resetTypeProduct();
            eval($(this).attr('href').replace('javascript:', ''));
            e.preventDefault();
            setSearchFilter(null, null, null);
        });

        $('.clear-price').click(function (e) {
            clearPrice();
            eval($(this).attr('href').replace('javascript:', ''));
            e.preventDefault();
            setSearchFilter(null, null, null);
        });

        $('.clear-distance').click(function (e) {
            clearDistance();
            eval($(this).attr('href').replace('javascript:', ''));
            e.preventDefault();
            setSearchFilter(null, null, null);
        });

        $('.btn-link-search').click(function() {
            $(this).toggleClass('active');
            setSearchFilter(true, null, null);
        });

        setWidthSearch();
        $(window).resize(function () {
            setWidthSearch();
        });

        openFilter();
    });

    $('.guest-capacity a').click(function (e) {
        e.preventDefault();
        var currentValue = $(this).text();
        var currentGuest = currentValue.match(/\d+/g)[0];
        $('.guests .current-value').text(currentValue);
        $('.hid-guest').val(currentGuest);
        showLoading();
        $('.hid-guest').change();
        __doPostBack(window.hotelList, null);
    });
})(jQuery);