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

function valid_credit_card(value) {
    // accept only digits, dashes or spaces
    if (value.length < 9) return false;
    if (/[^0-9-\s]+/.test(value)) return false;

    // The Luhn Algorithm. It's so pretty.
    var nCheck = 0, nDigit = 0, bEven = false;
    value = value.replace(/\D/g, "");

    for (var n = value.length - 1; n >= 0; n--) {
        var cDigit = value.charAt(n),
              nDigit = parseInt(cDigit, 10);

        if (bEven) {
            if ((nDigit *= 2) > 9) nDigit -= 9;
        }

        nCheck += nDigit;
        bEven = !bEven;
    }

    return (nCheck % 10) == 0;
}

function GetCardType(number) {
    // AMEX
    re = new RegExp("^3[47]");

    if (number.match(re) != null)
        return "AMEX";

    return "";
}

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);

    return pattern.test(emailAddress);
}

function validname() {
    var errorCode = $("#errorCode");
    var purchaseButton = $("#AnchorButton");
    var firstName = $("#FirstName");
    var lastName = $("#LastName");
    var isAllowCheckout = $("#IsAllowCheckout");

    var errorMessage = "Please enter your name";
    if (errorCode.text() == errorMessage) {
        errorCode.text("");
    }

    if (firstName.length > 0 && firstName.val() == "") {
        firstName.addClass("errorborder");
        errorCode.text(errorMessage);
        purchaseButton.addClass("btngray");
        isAllowCheckout.val("False");
        return false;
    }

    if (lastName.length > 0 && lastName.val() == "") {
        lastName.addClass("errorborder");
        errorCode.text(errorMessage);
        purchaseButton.addClass("btngray");
        isAllowCheckout.val("False");
        return false;
    }

    return true;
}

//all validations hide the button if false
function validcc() {
    var errorCode = $("#errorCode");
    var purchaseButton = $("#AnchorButton");
    var isAllowCheckout = $("#IsAllowCheckout");

    var errorMessage = "Please enter a valid credit card number.";
    if (errorCode.text() == errorMessage) {
        errorCode.text("");
    }
    var nc = $("#cctextbox").val();

    if (nc.length > 19) {
        $("#cctextbox").addClass("errorborder");
        purchaseButton.addClass("btngray");
        isAllowCheckout.val("False");
        return false;
    }

    if (!valid_credit_card(nc)) {
        $("#cctextbox").addClass("errorborder");
        errorCode.text(errorMessage);
        purchaseButton.addClass("btngray");
        isAllowCheckout.val("False");
        return false;
    }
    return true;
}

function validexpdat() {
    var errorCode = $("#errorCode");
    var cardDate = $("#txtexpdat");
    var purchaseButton = $("#AnchorButton");
    var isAllowCheckout = $("#IsAllowCheckout");

    var errorMessage = "Please enter a valid expiration date";
    var errorMessage2 = "Please enter a valid month of expiration date.";

    if (errorCode.text() == errorMessage || errorCode.text() == errorMessage2) {
        errorCode.text("");
    }
    var nc = cardDate.val();
    var reg = /^(0?[1-9]|1[0-2])\s?\/\s?([0-9]{2})$/;

    var found = nc.match(reg);

    if (!reg.test(nc)) {
        errorCode.text(errorMessage);
        purchaseButton.addClass("btngray");
        isAllowCheckout.val("False");
        return false;
    }
    else {
        var currentDate = new Date();
        var month = parseInt(found[1], 10);
        var year = parseInt(found[2], 10);
        if (month > 12 || month < 1) {
            errorCode.text(errorMessage2);
            cardDate.addClass("errorborder");
            purchaseButton.addClass("btngray");
            isAllowCheckout.val("False");
            return false;
        }

        if (year < (currentDate.getFullYear() - 2000) || (year == currentDate.getFullYear() - 2000 && month <= currentDate.getMonth() + 1)) {
            errorCode.text(errorMessage);
            cardDate.addClass("errorborder");
            purchaseButton.addClass("btngray");
            isAllowCheckout.val("False");
            return false;
        }
        return true;
    }
}

function validsec() {
    var errorCode = $("#errorCode");
    var cvv = $("#txtseccode");
    var purchaseButton = $("#AnchorButton");
    var isAllowCheckout = $("#IsAllowCheckout");

    var errorMessage = "Please enter a valid security number.";
    var nc = cvv.val();
    var cctextbox = $("#cctextbox").val();

    if (errorCode.text() == errorMessage) {
        errorCode.text("");
    }

    if (GetCardType(cctextbox) == "AMEX" && nc.length != 4) {
        errorCode.text(errorMessage);
        purchaseButton.addClass("btngray");
        isAllowCheckout.val("False");
        return false;
    }
    else if (nc.length >= 3) {
        return true;
    }
}

function validEmail() {
    var errorCode = $("#errorCode");
    var purchaseButton = $("#AnchorButton");
    var emailaddress = $("#email");
    var isAllowCheckout = $("#IsAllowCheckout");

    var errorMessage = "Please enter a valid email address.";

    var validCheckout = isAllowCheckout.val();
    if (errorCode.text() == errorMessage) {
        emailaddress.removeClass("errorborder");
        errorCode.text("");
    }

    if (emailaddress.length > 0
        && ($.trim(emailaddress.val()) == ''
            || !isValidEmailAddress($.trim(emailaddress.val())))) {
        errorCode.text(errorMessage);
        emailaddress.addClass('errorborder');
        purchaseButton.addClass("btngray");
        if (validCheckout == "True") {
            isAllowCheckout.val("False");
        }
        return false;
    }
    return true;
}

// && ValidEmailAddress()
function allisok() {
    var purchaseButton = $("#AnchorButton");
    var isAllowCheckout = $("#IsAllowCheckout");

    if (validcc() && validsec() && validexpdat() && validname() && validEmail()) {
        purchaseButton.removeClass("btngray");
        isAllowCheckout.val("True");
    } else {
        isAllowCheckout.val("False");
    }
}

function showConfirm() {
    $('#newBookingModal').modal('show'); 
}

function setHeight() {
    var bookHeight = $('.book-wrap').height() - 200;
    var bookWrapHeight = $('.book-wrap').outerHeight() - 60;
    if (bookWrapHeight < bookHeight) {
        $('.purchase').height(bookWrapHeight);
    } else {
        $('.purchase').height('auto');
    }
}

function domReady() {
    $(document).on('blur', ".book-wrap input[type=text], .book-wrap input[type=tel]", function () {
        if (!$(this).hasClass("discount-text")) {
            if ($(this).val() != "") {
                $(this).removeClass("errorborder");
            } else {
                $(this).addClass("errorborder");
            }
        }
    });
    $(document).on('blur', '#cctextbox', function () {
        if (validcc()) { }
    });

    $(document).on('paste', '#cctextbox', function () {
        var element = this;
        setTimeout(function () {
            var text = $(element).val();
            // do something with text
            $(this).val(text.slice(0, 4) + " ");
            $(this).val($(this).val() + text.slice(4, 8) + " ");
            $(this).val($(this).val() + text.slice(8, 12) + " ");
            $(this).val($(this).val() + text.slice(12, 16));
        }, 100);
    });

    $(document).on('keyup', '#cctextbox', function (event) {
        var keyCode = event.keyCode || event.which;

        if (keyCode === 8 || (keyCode === 127 && keyCode === 46)) {
            return;
        }

        var nc = $(this).val();
        var exp;
        switch (nc.length) {
            case 1:
                exp = new RegExp("^[0-9]$", "m");
                if (!exp.test(nc)) {
                    $(this).val("");
                    event.preventDefault();
                    return false;
                }
                break;
            case 2:
                exp = new RegExp("^[0-9]{2}$", "m");

                if (!exp.test(nc)) {
                    $(this).val(nc[0]);
                    return false;
                } else {
                    $(this).val($(this).val());
                }
                break;
            case 3:
                exp = new RegExp("^[0-9]{3}$", "m");

                if (!exp.test(nc)) {
                    $(this).val(nc[0]);
                    return false;
                } else {
                    $(this).val($(this).val());
                }
                break;
            case 4:
                exp = new RegExp("^[0-9]{4}$", "m");

                if (!exp.test(nc)) {
                    $(this).val(nc[0]);
                    return false;
                } else {
                    $(this).val($(this).val() + " ");
                }
                break;

            case 9:
                $(this).val($(this).val() + " ");
                break;
            case 14:
                $(this).val($(this).val() + " ");
                break;
        }
    });

    $(document).on('keypress', '#cctextbox', function (event) {
        var keyCode = event.keyCode || event.which;
        if ((keyCode < 48 || keyCode > 57)) return false;
    });
    $(document).on('keyup', '#cctextbox', function () {
        var nc = $(this).val();
        var cardDate = $("#txtexpdat");
        if (GetCardType(nc) == "AMEX") {
            if (this.value.length >= 18) {
                this.value = this.value.slice(0, 18);
                cardDate.focus();
            }
        } else {
            if (this.value.length >= 19) {
                this.value = this.value.slice(0, 19);
                cardDate.focus();
            }
        }
    });
    $(document).on('blur', '#txtexpdat', function () {
        validexpdat();
    });
    $(document).on('keypress', '#txtseccode', function () {
        var keyCode = event.keyCode || event.which;
        if ((keyCode < 48 || keyCode > 57)) return false;
    });
    $(document).on('keyup', '#txtseccode', function () {
        var ccnum = $("#cctextbox").val();
        var txtzipcode = $("#txtzipcode");

        if (GetCardType(ccnum) == "AMEX") {
            if (this.value.length >= 4) {
                this.value = this.value.slice(0, 4);
                txtzipcode.focus();
            }
        } else {
            if (this.value.length >= 3) {
                this.value = this.value.slice(0, 3);
                txtzipcode.focus();
            }
        }
    });

    $(document).on('keypress', '#txtzipcode', function () {
        if ((event.keyCode < 48 || event.keyCode > 57)) return false;

        if (this.value.length > 5) {
            this.value = this.value.slice(0, 4);
        }
    });
    $(document).on('keyup', '#txtzipcode', function () {
        if (this.value.length > 3) {
            allisok();
        }
    });
    $(document).on('blur', '#txtzipcode', function () {
        if (this.value.length > 3) {
            allisok();
        }
    });
    $(document).on('keyup', '#txtexpdat', function (event) {
        var keyCode = event.keyCode || event.which;
        if (keyCode == 8 || (keyCode == 127 && keyCode == 46)) {
            return false;
        }
        var cardDate = $(this);
        var cvv = $("#txtseccode");

        var nc = cardDate.val();
        var month;
        var reg = /^(0?[1-9]|1[0-2])\s?\/\s?([0-9]{2})$/;

        if (nc.length === 1) {
            month = parseInt(nc, 10);
            if (month > 1) {
                cardDate.val(cardDate.val() + " / ");
                return false;
            }
            if (isNaN(month)) {
                cardDate.val("");
                return false;
            }
        }
        if (nc.length === 2) {
            month = parseInt(nc, 10);
            if (month >= 1) {
                cardDate.val(cardDate.val() + " / ");
                return false;
            }
            if (isNaN(month)) {
                cardDate.val("");
                return false;
            }
        }
        if (nc.length >= 6) {
            var found = nc.match(reg);
            if (found != undefined && found[2].length === 2) {
                cvv.focus();
            }
        }
    });
    $(document).on('blur', '#email', function() {
        validEmail();
    });

    // ENABLE - DISABLE PURCHASE BUTTON
    $('.input-group input').keyup(function () {
        var empty = false;
        var isValidZip = true;
        var txtzipcode = $("#txtzipcode");
        var purchaseButton = $("#AnchorButton");
        var isAllowCheckout = $("#IsAllowCheckout");

        $('.input-group input').each(function () {
            if (!$(this).hasClass("discount-text") && !$(this).hasClass('datepicker') && $(this).val() == '') {
                empty = true;
            }
        });

        if (txtzipcode.length > 0) {
            isValidZip = /(^\d{5}$)|(^\d{5}-\d{4}$)/.test(txtzipcode.val());
        }

        if (empty || !isValidZip) {
            purchaseButton.addClass("btngray").bind('click', true);
            isAllowCheckout.val("False");
        } else {
            purchaseButton.removeClass("btngray").unbind('click', false);
            isAllowCheckout.val("True");
        }
    });

    // FORM VALIDATON OF PAYMENT
    $(document).on('click', '#AnchorButton', function () {
        var err = false;
        var creditCard = $("#cctextbox");
        var txtzipcode = $("#txtzipcode");
        var cardDate = $("#txtexpdat");
        var cvv = $("#txtseccode");
        var emailaddress = $("#email");
        var firstName = $("#FirstName");
        var lastName = $("#LastName");
        var purchaseButton = $(this);

        creditCard.removeClass('errorborder');
        cardDate.removeClass('errorborder');
        cvv.removeClass('errorborder');
        txtzipcode.removeClass('errorborder');
        emailaddress.removeClass('errorborder');

        if (firstName.length > 0 && firstName.val() == "") {
            err = true;
            firstName.addClass('errorborder');
        }

        if (lastName.length > 0 && lastName.val() == "") {
            err = true;
            lastName.addClass('errorborder');
        }

        if (creditCard.length > 0 && creditCard.val() == "") {
            err = true;
            creditCard.addClass('errorborder');
        }

        if (cardDate.length > 0 && cardDate.val() == "") {
            err = true;
            cardDate.addClass('errorborder');
        }

        if (cvv.length > 0 && cvv.val() == "") {
            err = true;
            cvv.addClass('errorborder');
        }

        if (txtzipcode.length > 0) {
            var isValidZip = /(^\d{5}$)|(^\d{5}-\d{4}$)/.test(txtzipcode.val());
            if (!isValidZip) {
                err = true;
                txtzipcode.addClass('errorborder');
            }
        }

        if (emailaddress.length > 0
            && ($.trim(emailaddress.val()) == ''
                || !isValidEmailAddress($.trim(emailaddress.val())))) {
            err = true;
            emailaddress.addClass('errorborder');
        }

        if (err) {
            purchaseButton.addClass("btngray");
            return false;
        }
        $(".hoteloverlaywrap").show();
        $(".hotelmodal").css({ "padding-left": 10, "padding-right": 10 })
            .html("SIT TIGHT...WE'RE ACTIVATING<br/> YOUR SUBSCRIPTION<div class=\"imgspin text-center\"><img src=\"/images/loading.gif\" alt=\"\"></div>")
            .show();
    });

    if ($('#UseNewCard').length <= 0) {
        $('#AnchorButton').unbind('click', false);
    } else {
        $('#AnchorButton').removeClass("btngray").bind('click', true);
    }

    // ALTERNATIVE PLACEHOLDER FOR EMAIL ADDRESS
    var emailplaceholder = $(".emailplaceholder");
    if ($('#email').length > 0 && $('#email').val().length > 0) {
        emailplaceholder.hide();
    }

    $(document).on('focus', '#email', function () {
        emailplaceholder.hide();
    }).blur(function () {
        var getValEmail = $(this).val();
        emailplaceholder.show();
        if (getValEmail != "") {
            emailplaceholder.hide();
        }
    });
    emailplaceholder.click(function () {
        $('#email').focus();
        emailplaceholder.hide();
    });

    if ($("#cctextbox").length > 0) {
        $("#cctextbox").validateCreditCard(function (result) {
            var cardType = $('.cardtypewrap');
            if (result.card_type == null) {
                cardType.html('');
                return;
            }
            var getCardType = result.card_type.name;
            if (getCardType == "visa") {
                cardType.html('<div class="cardiconwrap cardicon_visa"></div>');
            } else if (getCardType == "amex") {
                cardType.html('<div class="cardiconwrap cardicon_amex"> </div>');
            } else if (getCardType == "mastercard") {
                cardType.html('<div class="cardiconwrap cardicon_master"> </div>');
            } else if (getCardType == "discover") {
                cardType.html('<div class="cardiconwrap cardicon_discover"> </div>');
            } else {
                cardType.html('');
            }
        });
    }

    // 5 MINUTES TIMER FUNCTIONALITY		  
    setHeight();
    $(window).resize(function () {
        setHeight();
    });
    setTimeout(function () {
        $('#compactCountdown, #compactCountdown2').countdown({
            until: '+5m +0s',
            compact: true,
            layout: 'YOUR ' + window.productType + ' WILL BE HELD FOR <span>{mnn}{sep}{snn}</span> {desc}',
            onExpiry: function () {
                runtimerdone();
            }
        });
        setHeight();
    }, 500);
    function runtimerdone() {
        window.location.href = window.searchPage;
    }
}

$(document).ready(function () {
    $(".hoteloverlaywrap").delay(500).fadeOut("slow");
    $(".hotelmodal").delay(500).fadeOut("slow");
});

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
        $('#overlayAuth').addClass('hidden');
        if (window.userSession) {
            $('.modal-backdrop').remove();
            if (window.mixpanel && window.useremail) {
                window.mixpanel.register_once({
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
        }
    }

    if (window.showAuth != undefined && window.showAuth === 'True') {
        window.pageLoaded = true;
        $('#authModal').modal('show');
        $('#authModal').on('hidden.bs.modal', function (e) {
            window.pageLoaded = false;
            window.showAuth = false;
        });
    }

    loadImageSequense();

    $(".hoteloverlaywrap").hide();
    $(".hotelmodal").hide();

    if (window.showConfirmM) {
        showConfirm();
    }

    domReady();
}