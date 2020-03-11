DatePicker = {
    hideOldDays: function () { // hide days for previous month
        var x = $('.datepicker .datepicker-days tr td.old');
        if (x.length > 0) {
            x.css('visibility', 'hidden');
            if (x.length === 7) {
                x.parent().hide();
            }
        }
    },
    hideNewDays: function () { // hide days for next month
        var x = $('.datepicker .datepicker-days tr td.new');
        if (x.length > 0) {
            x.hide();
        }
    },
    hideOtherMonthDays: function () { // hide days not for current month
        DatePicker.hideOldDays();
        DatePicker.hideNewDays();
    }
};

Date.prototype.greaterThanDate = function (date) {
    return this.getTime() > date.getTime();
}

function hideSelectDate(e) {
    $('.manage-blackout').show();
    $('#selectedDate').addClass('hidden');
    $('.product-list-calendar a.active').click();
    e.preventDefault();
}

//function viewProduct(item, productId) {
//    $('.product-list-calendar .active').removeClass('active');
//    $(item).addClass('active');
//    reInitCalendar(productId);
//}

$(function () {
    function equalDate(date1, date2) {
        return date1.getFullYear() === date2.getFullYear()
            && date1.getDate() === date2.getDate()
            && date1.getMonth() === date2.getMonth();
    }

    function dateContains(dates, date) {
        var isContain = false;
        for (var i = 0; i < dates.length; i++) {
            if (equalDate(new Date(dates[i]), date)) {
                isContain = true;
            }
        }
        return isContain;
    }

    function bindTitle() {
        DatePicker.hideOtherMonthDays();
        if ($('#datepicker .day[title]').find('.product-i').length == 0) {
            $('#datepicker .day[title]').each(function () {
                var title = $(this).attr('title');

                if (title.indexOf('!') !== -1) {
                    title = title.split('!');
                    $(this).html('<b>' + title[1] + '</b>');
                    title = title[0];
                }

                if (title.indexOf('|')) {
                    title = title.split('|');
                    var dailyPass = parseInt(title[0], 10);
                    var totalBookings = parseInt(title[1], 10);
                    var totalRedemptions = parseInt(title[2], 10);
                    //var capacity = parseInt(title[3], 10);
                    //var customPrice = parseFloat(title[4], 10);

                    // Products with Type
                    if (title.length > 5) {
                        for (var i = 6; i <= title.length - 1; i++) {
                            $(this).append('<span class="product-i">' + title[i].split('?')[0] + '</span>');
                        }
                    }

                    if (dailyPass > 0) {
                        $(this).append('<span class="highlight-span">' + dailyPass + '</span>');
                    }

                    if (dailyPass == 0) {
                        $(this).addClass('disabled-date');
                    }

                    // Bookings
                    if (!totalBookings.isNaN && totalBookings > 0) {
                        $(this).append('<span class="highlight-span price">' + totalBookings + '</span>');
                    }
                    // Redemptions
                    if (!totalRedemptions.isNaN && totalRedemptions > 0) {
                        $(this).append('<span class="highlight-span price redeem">' + totalRedemptions + '</span>');
                    }
                }
            });
        }
    }

    function bindAddBlackOutIcon() {
        $('td.active').removeClass('first-item').removeClass('last-item');
        $('td.active').first().addClass('first-item');
        $('td.active').last().addClass('last-item');
    }

    function removeClassDisabledWithGreaterThanToday() {
        $('.disabled-date').each(function () {
            if (!$(this).hasClass('less-than-today')) {
                $(this).removeClass('disabled');
            }
        });
    }

    function checkOpenBlockedDate() {
        if ($('.day.active').hasClass('disabled-date')) {
            $('a.btn-tab').first().click();
            $('.custom-price').show();
        }
        else {
            $('a.btn-tab').last().click();
        }
        removeClassDisabledWithGreaterThanToday();
    }   

    function checkDateOutside() {
        var trFirst = $('.datepicker-days tbody tr').first();
        if (!(trFirst.text().indexOf('1') !== -1)) {
            trFirst.hide();
        }
        var trLast = $('.datepicker-days tbody tr').last();
        if (!(trLast.text().indexOf('1') !== -1)) {
            trLast.hide();
        }
    }

    var updateByControl = false;
    window.selectedDateRanges = [];

    function getValidRange(newSelectedDate, firstSelectedDate, dates, dateDisabled) {
        var selectedDate = [];
        if (newSelectedDate == firstSelectedDate) {
            selectedDate.push(firstSelectedDate);
        }
        else if (newSelectedDate < firstSelectedDate) {
            var timeSpand = firstSelectedDate.getDate() - newSelectedDate.getDate();
            for (var i = 0; i <= timeSpand; i++) {
                var currentSelectedDate = new Date(newSelectedDate.getFullYear(), newSelectedDate.getMonth(), newSelectedDate.getDate() + i);
                selectedDate.push(currentSelectedDate);
            }
        }
        else {
            var timeSpand1 = newSelectedDate.getDate() - firstSelectedDate.getDate();
            for (var j = 0; j <= timeSpand1; j++) {
                var currentSelectedDate1 = new Date(firstSelectedDate.getFullYear(), firstSelectedDate.getMonth(), firstSelectedDate.getDate() + j);
                if (!$('.day.active').hasClass('disabled-date')) {
                    if (dateContains(dates, currentSelectedDate1) || dateContains(dateDisabled, currentSelectedDate1)) {
                        selectedDate = [];
                        break;
                    }
                }
                selectedDate.push(currentSelectedDate1);
            }
        }
        return selectedDate;
    }

    function getCustomPriceByDate(date, customPrice) {
        for (var i = 0; i < customPrice.length; i++) {
            var cDate = new Date(customPrice[i].Date);
            if (equalDate(cDate, date)) {
                return customPrice[i];
            }
        }
    }

    function getDefaultPrice(defaultPrice, date) {
        var dPrice = [];
        for (var i = 0; i < defaultPrice.length; i++) {
            var effectiveDate = new Date(defaultPrice[i].EffectiveDate);
            var notContains = true;
            if (dPrice.length > 0) {
                for (var j = 0; j < dPrice.length; j++) {
                    if (dPrice[j][4] === defaultPrice[i].ProductId) {
                        notContains = false;
                    }
                }
            }
            if (date.greaterThanDate(effectiveDate) && notContains) {
                if (defaultPrice[i]) {
                    var day = date.getDay();
                    switch (day) {
                        case 0:
                            dPrice.push([defaultPrice[i].PriceSun, defaultPrice[i].UpgradeDiscountSun, defaultPrice[i].PassCapacitySun, defaultPrice[i].ProductNameAcronym, defaultPrice[i].ProductId]);
                            break;
                        case 1:
                            dPrice.push([defaultPrice[i].PriceMon, defaultPrice[i].UpgradeDiscountMon, defaultPrice[i].PassCapacityMon, defaultPrice[i].ProductNameAcronym, defaultPrice[i].ProductId]);
                            break;
                        case 2:
                            dPrice.push([defaultPrice[i].PriceTue, defaultPrice[i].UpgradeDiscountTue, defaultPrice[i].PassCapacityTue, defaultPrice[i].ProductNameAcronym, defaultPrice[i].ProductId]);
                            break;
                        case 3:
                            dPrice.push([defaultPrice[i].PriceWed, defaultPrice[i].UpgradeDiscountWed, defaultPrice[i].PassCapacityWed, defaultPrice[i].ProductNameAcronym, defaultPrice[i].ProductId]);
                            break;
                        case 4:
                            dPrice.push([defaultPrice[i].PriceThu, defaultPrice[i].UpgradeDiscountThu, defaultPrice[i].PassCapacityThu, defaultPrice[i].ProductNameAcronym, defaultPrice[i].ProductId]);
                            break;
                        case 5:
                            dPrice.push([defaultPrice[i].PriceFri, defaultPrice[i].UpgradeDiscountFri, defaultPrice[i].PassCapacityFri, defaultPrice[i].ProductNameAcronym, defaultPrice[i].ProductId]);
                            break;
                        case 6:
                            dPrice.push([defaultPrice[i].PriceSat, defaultPrice[i].UpgradeDiscountSat, defaultPrice[i].PassCapacitySat, defaultPrice[i].ProductNameAcronym, defaultPrice[i].ProductId]);
                            break;
                    }
                }
            }
        }
        return dPrice;
    }

    function getBookByDate(date, bookData) {
        for (var i = 0; i < bookData.length; i++) {
            var sDate = new Date(bookData[i].BookedDate);
            if (equalDate(sDate, date)) {
                return bookData[i];
            }
        }
    }

    function getRedeemedByDate(date, redeemedData) {
        for (var i = 0; i < redeemedData.length; i++) {
            var sDate = new Date(redeemedData[i].RedeemedDate);
            if (equalDate(sDate, date)) {
                return redeemedData[i];
            }
        }
    }

    window.reInitCalendar = function (dateDisabledParam, customPriceParam, defaultPriceParam, bookDataParam, redemptionDataParam) {
        //Calendar
        if ($('#datepicker').length > 0) {
            dateDisabledParam = JSON.parse(dateDisabledParam);
            $('#datepicker').datepicker('destroy');

            $('#datepicker').datepicker({
                multidate: true,
                changeMonth: true,
                datesDisabled: dateDisabledParam,
                defaultViewDate: {
                    year: window.selectedYear,
                    month: window.selectedMonth
                },
                templates: {
                    leftArrow: '<button class="btn btn-prev"><img src="/images/arrow-left.png" alt="" class="img-responsive" /></button>',
                    rightArrow: '<button class="btn btn-next"><img src="/images/arrow-right.png" alt="" class="img-responsive" /></button>'
                },
                stepMonths: 0,
                showCurrentAtPos: 1,
                minViewMode: "days",
                beforeShowDay: function (date) {
                    var returnValue = {
                        enabled: true,
                        classes: '',
                        tooltip: ''
                    };
                    console.log(window.selectedMonth);
                    if (date.lessThanTodayDate(new Date())) {
                        returnValue.enabled = false;
                        returnValue.classes += ' less-than-today';
                    }
                    if (equalDate(new Date(), date)) {
                        returnValue.enabled = true;
                        returnValue.classes += ' current-date';
                    }
                    //for (var i = 0; i < window.dateCalendarHighlight.length; i++) {
                    //    var currentDate = new Date(window.dateCalendarHighlight[i]);
                    //    if (equalDate(currentDate, date)) {
                    //        returnValue.enabled = false;
                    //        returnValue.classes += ' highlight';
                    //    }
                    //}

                    var customPrice = getCustomPriceByDate(date, customPriceParam);
                    var dPrice = getDefaultPrice(defaultPriceParam, date);
                    var book;
                    var redeemed;
                    if (customPrice) {
                        book = getBookByDate(date, bookDataParam);
                        redeemed = getRedeemedByDate(date, redemptionDataParam);
                        if (customPrice.Capacity != -1) {
                            returnValue.tooltip = customPrice.Capacity;
                        } else {
                            returnValue.tooltip = dPrice[0][2];
                        }
                        returnValue.tooltip += '|' + (book ? book.CountBook : 0);
                        returnValue.tooltip += '|' + (redeemed ? redeemed.CountBook : 0);
                        returnValue.tooltip += '|' + customPrice.RegularPrice;
                        returnValue.tooltip += '|' + '0'; // customPrice.UpgradePrice;
                        if (customPrice.Capacity != -1) {
                            returnValue.tooltip += '|' + customPrice.Capacity;
                        } else {
                            returnValue.tooltip += '|' + dPrice[0][2];
                        }
                        //var dailyPrice = '';
                        //if (customPrice.ProductsDailyPrice.length > 0) {
                        //    for (var j = 0; j < customPrice.ProductsDailyPrice.length; j++) {
                        //        dailyPrice += '|' + customPrice.ProductsDailyPrice[j];
                        //    }
                        //}
                        if (customPrice.RegularPrice != 0) {
                            returnValue.tooltip += '| $' + customPrice.RegularPrice;
                        } else {
                            returnValue.tooltip += '| $' + dPrice[0][0];
                        }
                    } else {
                        // Default Value here
                        if (dPrice.length > 0) {
                            book = getBookByDate(date, bookDataParam);
                            redeemed = getRedeemedByDate(date, redemptionDataParam);
                            returnValue.tooltip = dPrice[0][2];
                            returnValue.tooltip += '|' + (book ? book.CountBook : 0);
                            returnValue.tooltip += '|' + (redeemed ? redeemed.CountBook : 0);
                            returnValue.tooltip += '|' + dPrice[0][0]; // price
                            returnValue.tooltip += '|' + dPrice[0][1]; // upgrade price
                            returnValue.tooltip += '|' + dPrice[0][2]; // capacity

                            var productName = [];
                            for (var l = 0; l < dPrice.length; l++) {
                                if (productName.indexOf(dPrice[l][3]) == -1) {
                                    productName.push(dPrice[l][3]);
                                    returnValue.tooltip += '| $' + dPrice[l][0];
                                }
                            }
                        }
                    }

                    for (var i = 0; i < dateDisabledParam.length; i++) {
                        var disabledDate = new Date(dateDisabledParam[i]);
                        if (equalDate(disabledDate, date)) {
                            if (disabledDate.lessThanTodayDate(new Date())) {
                                returnValue.enabled = false;
                                returnValue.classes += ' disabled-date';
                                returnValue.tooltip = null;
                            }
                            else {
                                returnValue.enabled = true;
                                returnValue.classes = ' disabled-date';
                                returnValue.tooltip = null;
                            }
                        }
                    }

                    if (date.getDate() === 1) {
                        returnValue.tooltip += "!" + date.getMonthNameShort().toUpperCase() + ' ' + date.getDate();
                    }
                    if (date.equalDate(new Date())) {
                        returnValue.tooltip += "!<span>" + date.getMonthNameShort().toUpperCase() + '</span> ' + date.getDate();   //"TODAY";
                    }
                    return returnValue;
                }
            }).on('changeDate', function (e) {
                if (e.date && e.date.lessThanTodayDate(new Date())) {
                    $('.current-month').text(e.date.getMonthName().toUpperCase() + ' ' + e.date.getFullYear());
                }
                if (e.dates.length > 0) {
                    if (e.dates.length >= 1) {
                        var newSelectedDate = e.dates[e.dates.length - 1];
                        var firstSelectedDate = e.dates[0];
                        var dateRanges = getValidRange(newSelectedDate, firstSelectedDate, [], dateDisabledParam);
                        if ((window.selectedDateRanges && window.selectedDateRanges.length != e.dates.length) ||
                                window.selectedDateRanges.length == 0 ||
                                (window.selectedDateRanges.length == 1 && e.dates.length == 1 && window.selectedDateRanges[0] != e.dates[0])) {
                            updateByControl = true;
                            if (dateRanges.length == 0) {
                                dateRanges = e.dates.splice(0, e.dates.length - 1);
                            }
                            window.selectedDateRanges = dateRanges;
                        }

                        // Set Regular Price and Disount Price when selected first Date
                        if (e.dates.length === 1) {
                            getPrice(firstSelectedDate);
                            //var customPrice = getCustomPriceByDate(firstSelectedDate, customPriceParam);
                            //if (customPrice) {
                            //    for (var j = 0; j < customPrice.ProductsDailyPrice.length; j++) {
                            //        var nPrice = customPrice.ProductsDailyPrice[j];
                            //        nPrice = nPrice.split(' - ')[1].split('?');
                            //        var regularPrice = parseFloat(nPrice[0].replace('$', ''), 10);
                            //        var capacity = parseInt(nPrice[1].replace('$', ''), 10);
                            //        $('.regular-price').eq(j).val(regularPrice.toFixed(2));
                            //        $('.product-item').eq(j).val(capacity);
                            //    }
                            //} else {
                            //    // Default value here
                            //    var dPrice = getDefaultPrice(defaultPriceParam, e.dates[0]);
                            //    if (dPrice.length > 0) {
                            //        for (var l = 0; l < dPrice.length; l++) {
                            //            $('.regular-price-' + dPrice[l][4]).val(parseFloat(dPrice[l][0], 10).toFixed(2));
                            //            $('.product-' + dPrice[l][4]).val(dPrice[l][2] / 2);
                            //        }
                            //    }
                            //}
                        }

                        if (e.dates.length > 1) {
                            var itemPrices = $('.regular-price').length;
                            for (var k = 0; k < itemPrices; k++) {
                                $('.regular-price').eq(k).val('');
                                $('.product-item').eq(k).val('-1');
                            }
                        }

                        while (updateByControl) {
                            updateByControl = false;
                            //$('#datepicker').datepicker('setDates', dateRanges);
                            var startDate = dateRanges[0];
                            var endDate = dateRanges[dateRanges.length - 1];
                            if (startDate.getDate() < endDate.getDate()) {
                                $('#startDate').val(startDate.getDateMMDDYYYY());
                                $('#endDate').val(endDate.getDateMMDDYYYY());
                            } else {
                                $('#startDate').val(endDate.getDateMMDDYYYY());
                                $('#endDate').val(startDate.getDateMMDDYYYY());
                            }
                            checkOpenBlockedDate();
                            bindAddBlackOutIcon();
                        }
                    }

                    $('.manage-blackout').hide();
                    $('#selectedDate').removeClass('hidden');
                }
                else {
                    window.selectedDateRanges = [];
                    $('.manage-blackout').show();
                    $('#selectedDate').addClass('hidden');
                }
                bindTitle();
            });
            bindTitle();
            $('.datepicker-switch').click(function (e) {
                e.preventDefault();
                bindAddBlackOutIcon();
                return false;
            });
            $('.prev, .next').click(function () {
                setTimeout(function () {
                    var currentDate = $("#datepicker").datepicker().data('datepicker').viewDate;
                    window.selectedYear = currentDate.getFullYear();
                    window.selectedMonth = currentDate.getMonth();
                    bindTitle();
                    removeClassDisabledWithGreaterThanToday();
                    checkDateOutside();
                }, 1);
            });
            removeClassDisabledWithGreaterThanToday();
            checkDateOutside();
        }
    }

    $('.product-list-calendar li:eq(0) a').addClass('active');
});