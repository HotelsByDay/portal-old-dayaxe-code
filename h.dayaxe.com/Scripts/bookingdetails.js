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
                classes: '',
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
                    break;
                }
            }

            if (isDefault) {
                switch (date.getDay()) {
                case 0: // Monday
                    returnValue.tooltip = defaultV[date.getDay()].T;
                    break;
                case 1: // Tuesday
                    returnValue.tooltip = defaultV[date.getDay()].T;
                    break;
                case 2: // Wednesday
                    returnValue.tooltip = defaultV[date.getDay()].T;
                    break;
                case 3: // Thursday
                    returnValue.tooltip = defaultV[date.getDay()].T;
                    break;
                case 4: // Friday
                    returnValue.tooltip = defaultV[date.getDay()].T;
                    break;
                case 5: // Saturday
                    returnValue.tooltip = defaultV[date.getDay()].T;
                    break;
                case 6: // Sunday
                    returnValue.tooltip = defaultV[date.getDay()].T;
                    break;
                default:
                    returnValue.tooltip = defaultV[date.getDay()].T;
                    break;
                }
            }
            return returnValue;
        }
    }
    return {};
}

function bindTitle() {
    // Insert more than one
    if ($('.datepicker .day[title]').find('.avail-ts').length == 0) {
        $('.datepicker .day[title]').each(function () {
            var title = $(this).attr('title');
            $(this).append('<span class="avail-ts">' + title + '</span>');
        });
    }
}

$(function() {
    $('.check-in-date').datepicker({
        todayHighlight: 'TRUE',
        autoclose: true,
        datesDisabled: window.blockoutDate,
        format: 'M dd, yyyy',
        beforeShowDay: function(date) {
            return checkTickets(date);
        }
    }).on('show',
        function(e) {
            bindTitle();
        }).on('changeDate',
        function (ev) {
            var start = moment(ev.date);
            $('#ExpiredDateText').data("DateTimePicker").date(start.add(1, 'days'));
        });;
    $('#ExpiredDateText').datetimepicker({
        format: 'MMM DD, YYYY h:mm a'
        });
    $('#ExpiredDateText').on('dp.show', function () {
        $(this).siblings('.alternative-tt').removeClass('hidden');
    });
    $('#ExpiredDateText').on('dp.change', function (e) {
        if (e.date) {
            $(this).siblings('.alternative-tt').removeClass('hidden');
        } else {
            $(this).siblings('.alternative-tt').addClass('hidden');
        }
    });
    $('#RedemptionDateText').datetimepicker({
        format: 'MMM DD, YYYY h:mm a'
    });
    $('#RedemptionDateText').on('dp.show', function () {
        $(this).siblings('.alternative-tt').removeClass('hidden');
    });
    $('#RedemptionDateText').on('dp.change', function (e) {
        if (e.date) {
            $(this).siblings('.alternative-tt').removeClass('hidden');
        } else {
            $(this).siblings('.alternative-tt').addClass('hidden');
        }
    });
});