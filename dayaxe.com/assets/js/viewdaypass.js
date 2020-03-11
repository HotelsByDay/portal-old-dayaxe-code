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
    if ($('.datepicker .day[title]').find('.avail-ts').length == 0) {
        $('.datepicker .day[title]').each(function () {
            var title = $(this).attr('title');
            $(this).append('<span class="avail-ts">' + title + '</span>');
        });
    }
}

function onSwitcher1Click() {
    $('#switcher1').hide();
    $('#page1').hide();
    $('#switcher2').show();
    $('#page2').show();
    var height = $('.content-area-fixed').outerHeight();
    $('#page2').css({ 'padding-top': height });
}

function onSwitcher2Click() {
    $('#switcher2').hide();
    $('#page2').hide();
    $('#switcher1').show();
    $('#page1').show();
    var height = $('.content-area-fixed').outerHeight();
    $('#page1').css({ 'padding-top': height });
}

function pageLoad(sender, args) {
    var needValidate = true;

    $('#StaffValidate').click(function () {
        $('#validateModal').modal('show');
    });
    var height = $('.content-area-fixed').outerHeight();
    $('#page2').css({ 'padding-top': height });

    $('#validateModal').on("hide.bs.modal", function () {
        $('#errorMessage').html("");
        $("#pinCode").val("");
        $('#validate').text('Staff Validate');
    });

    $("#pinCode").on("change", function () {
        $('#errorMessage').html("");
    });

    $('#validate').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        if (needValidate) {
            $('.progress').removeClass('hidden');
            var pinCode = $('#validateModal #pinCode').val();
            $.ajax({
                url: "/Handler/ValidatePin.ashx",
                type: "POST",
                dataType: 'json',
                data: { bookId: window.bookId, code: pinCode },
                success: function (result) {
                    $('#errorMessage').removeClass('hidden').html(result.Message);
                    if (result.IsSuccess) {
                        $('#errorMessage').addClass('alert-success');
                        $('#validate').text('Done');
                        needValidate = false;
                    } else {
                        $('#errorMessage').addClass('alert-danger');
                        $("#pinCode").val("");
                        $('#validate').text('TRY AGAIN');
                    }
                    setTimeout(function () {
                        $('.progress').addClass('hidden');
                    }, 500);
                }
            });
        } else {
            $('#validateModal').modal('hide');
            location.reload();
        }
        return false;
    });

    $(document).on('click', '.addtocalendar a', function () {
        $(this).toggleClass('clicked');
        if ($(this).hasClass('clicked')) {
            $('.atcb-list').css({ 'visibility': 'inherit' });
        } else {
            $('.atcb-list').css({ 'visibility': 'hidden' });
        }
    });

    $(document).on('change', '.check-in-date-d input', function () {
        // 48 hours = 2 days
        //var today = ((new Date()).setDate((new Date()).getDate() + 2));

        //var newCheckInDate = $('.check-in-date-d').datepicker("getDate").getTime();
        $(this).siblings('.input-group-addon').text($(this).val());
        $('#updateFail .modal-body b.check-in-date').text($(this).val());
        $('.select-checkin-date').html('UPDATE<br>CHECK-IN DATE').addClass('update');
    });

    $(document).on('click', '.select-checkin-date', function (e) {
        if ($(this).closest('.btn-save').hasClass('disabled')) {
            $('#updateNotPossible').modal('show');
        } else {
            $('.check-in-date-d').datepicker('show');
        }
        e.preventDefault();
    });

    $(document).on('click', '#confirmUpdate .btn-save', function () {
        var control = $('.update-checkin-date');
        if (control.length == 0) {
            control = $('.select-checkin-date');
        }
        eval(control.attr('href'));
    });
    $(document).on('click', '#confirmUpdate .btn-cancel', function () {
        location.href = location.href;
    });
    $('#updateFail .btn-save').click(function () {
        location.href = location.href;
    });
    //$(document).on('click', '.update-checkin-date', function (e) {
    //    if ($(this).closest('.btn-save').hasClass('disabled')) {
    //        $('#updateNotPossible').modal('show');
    //    } else {
    //        $('#confirmUpdate').modal('show');
    //    }
    //    e.preventDefault();
    //});

    var checkinDateInput = $('.check-in-date-d input');
    if (checkinDateInput.val() !== "") {
        $('.check-in-date-d .input-group-addon').text(checkinDateInput.val());
    }

    $('.check-in-date-d').datepicker({
        startDate: "+0d",
        endDate: "+30d",
        todayHighlight: 'TRUE',
        autoclose: true,
        datesDisabled: window.blockoutDate,
        templates: {
            leftArrow: '<img src="/images/arrow-left.png" alt="" class="img-responsive" />',
            rightArrow: '<img src="/images/arrow-right-b.png" alt="" class="img-responsive" />'
        },
        format: 'M dd, yyyy',
        beforeShowDay: function (date) {
            return checkTickets(date);
        }
    }).on('show', function (e) {
        setTimeout(function () {
            bindTitle();
        },
            5);
        if ($('.is-mobile').length > 0) {
            var offsetTop = $(document).scrollTop() - 10;
            $('.datepicker-dropdown').css({ 'top': offsetTop });
        }
    });
    $('.check-in-date-d .input-group-addon').on('click', function (e) {
        e.preventDefault();
        $('.check-in-date-d').datepicker('hide');
        return false;
    });
}