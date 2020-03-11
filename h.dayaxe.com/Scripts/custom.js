//Config Date
//window.notAllowChangeStateDay = 0;

//Prototype
Date.prototype.getDateMMDDYYYY = function () {
    var mm = this.getMonth() + 1;
    var dd = this.getDate();

    return [mm >= 10 ? mm : '0' + mm, '/', dd >= 10 ? dd : '0' + dd, '/', this.getFullYear()].join('');
};

Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
}

Date.prototype.equalDate = function (date) {
    return this.getFullYear() === date.getFullYear()
            && this.getDate() === date.getDate()
            && this.getMonth() === date.getMonth();
}

Date.prototype.lessThanToday = function (date) {
    return this.getTime() < date.getTime();
}

Date.prototype.lessThanTodayDate = function (date) {
    return this.getFullYear() === date.getFullYear()
        && this.getDate() < date.getDate()
        && this.getMonth() === date.getMonth();
}

Date.prototype.getMonthName = function (lang) {
    lang = lang && (lang in Date.locale) ? lang : 'en';
    return Date.locale[lang].month_names[this.getMonth()];
};

Date.prototype.getMonthNameShort = function (lang) {
    lang = lang && (lang in Date.locale) ? lang : 'en';
    return Date.locale[lang].month_names_short[this.getMonth()];
};

Date.locale = {
    en: {
        month_names: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        month_names_short: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec']
    }
};

$.fn.extend({
    limiter: function (limit, elem) {
        function setCount(src, elem) {
            var chars = src.value.length;
            if (chars > limit) {
                src.value = src.value.substr(0, limit);
                chars = limit;
            }
            elem.html(limit - chars);
        }
        $(this).on("keyup focus", function () {
            setCount(this, elem);
        });
        setCount($(this)[0], elem);
    }
});

$.fn.extend({
    wordsLimiter: function (limit, elem) {
        function setCount(src, elem) {
            var spaces = src.value.split(" ").length;
            if (spaces > limit) {
                src.value = src.value.substr(0, limit);
                spaces = limit;
            }
            elem.html(limit - spaces);
        }
        $(this).on("keyup focus", function () {
            setCount(this, elem);
        });
        setCount($(this)[0], elem);
    }
});



$('#DailyCapacity a').click(function () {
    $('.daily-current-value').text($(this).text());
    $('#HidPass').val($(this).text());
});

//$('#CabanaDailyLimit a').click(function () {
//    $('.cabana-current-value').text($(this).text());
//    $('#HidCabana').val($(this).text());
//});

//$('#SpaPassDailyLimit a').click(function () {
//    $('.spaPass-current-value').text($(this).text());
//    $('#HidSpaPass').val($(this).text());
//});

//$('#DayBedDailyLimit a').click(function () {
//    $('.dayBed-current-value').text($(this).text());
//    $('#HidDayBed').val($(this).text());
//});

$('#Distance a').click(function() {
    $('.distance-current-value').text($(this).text());
});

$('#DropdownIncome a').click(function () {
    $('.income-current-value').text($(this).text());
});


$('#AgeFrom a').click(function () {
    $('.age-from-current-value').text($(this).text());
});


$('#AgeTo a').click(function () {
    $('.age-to-current-value').text($(this).text());
});

$('a.btn-tab').click(function() {
    $('a.btn-tab').removeClass('active');
    $(this).addClass('active');
});

$('.revenue-select-date .dropdown-menu a').click(function() {
    $('#SelectedFilterDdl').val($(this).data('val')).trigger('change');
});

$('.revenue-select-product-type .dropdown-menu a').click(function () {
    $('#ProductTypeDdl').val($(this).data('val')).trigger('change');
});

// Add-Ons Change in Redemption Settings
$('.add-ons ul a').click(function (e) {
    var addOns = $(this).closest('.add-ons');
    var currentDailySales = addOns.find('.current-daily-sales-value');
    var hidDailySales = addOns.find('input[type=hidden]:eq(0)');

    currentDailySales.text($(this).text());
    hidDailySales.val($(this).text());

    e.preventDefault();
});