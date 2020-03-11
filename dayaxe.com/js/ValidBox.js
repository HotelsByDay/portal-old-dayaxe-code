var errorCode = $("#errorCode");
var txtzipcode = $("#txtzipcode");
var cardDate = $("#txtexpdat");
var cvv = $("#txtseccode");
var purchaseButton = $("#AnchorButton");
var creditCard = $("#cctextbox");
var emailaddress = $("#email");
var firstName = $("#FirstName");
var lastName = $("#LastName");
var isAllowCheckout = $("#IsAllowCheckout");

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
    var errorMessage = "Please enter a valid credit card number.";
    if (errorCode.text() == errorMessage) {
        errorCode.text("");
    }
    var nc = creditCard.val();

    if (nc.length > 19) {
        creditCard.addClass("errorborder");
        purchaseButton.addClass("btngray");
        isAllowCheckout.val("False");
        return false;
    }

    if (!valid_credit_card(nc)) {
        creditCard.addClass("errorborder");
        errorCode.text(errorMessage);
        purchaseButton.addClass("btngray");
        isAllowCheckout.val("False");
        return false;
    }
    return true;
}

function validexpdat() {
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
    var errorMessage = "Please enter a valid security number.";
    var nc = cvv.val();
    var cctextbox = creditCard.val();

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
    if (validcc() && validsec() && validexpdat() && validname() && validEmail()) {
        purchaseButton.removeClass("btngray");
        isAllowCheckout.val("True");
    } else {
        isAllowCheckout.val("False");
    }
}

$("document").ready(function () {
    $(".book-wrap input[type=text], .book-wrap input[type=tel]").blur(function () {
        var hidSelectedDate = $.jCookie('SelectedCheckInDate');
        if (!$(this).hasClass("discount-text") && !$(this).hasClass('datepicker')) {
            if ($(this).val() != "" || ($(this).hasClass('datepicker') && hidSelectedDate === "1")) {
                $(this).removeClass("errorborder");
            } else {
                $(this).addClass("errorborder");
            }
        }
    });
    creditCard.blur(function () {
        if (validcc()) { }
    });

    creditCard.on('paste', function () {
        var element = this;
        setTimeout(function () {
            var text = $(element).val();
            // do something with text
            creditCard.val(text.slice(0, 4) + " ");
            creditCard.val(creditCard.val() + text.slice(4, 8) + " ");
            creditCard.val(creditCard.val() + text.slice(8, 12) + " ");
            creditCard.val(creditCard.val() + text.slice(12, 16));
        }, 100);
    });

    creditCard.keyup(function (event) {
        var keyCode = event.keyCode || event.which

        if (keyCode == 8 || (keyCode == 127 && keyCode == 46)) {
            return;
        }

        var nc = creditCard.val();
        switch (nc.length) {
            case 1:
                exp = new RegExp("^[0-9]$", "m");
                if (!exp.test(nc)) {
                    creditCard.val("");
                    event.preventDefault();
                    return false;
                }
                break;
            case 2:
                exp = new RegExp("^[0-9]{2}$", "m");

                if (!exp.test(nc)) {
                    creditCard.val(nc[0]);
                    return false;
                } else {
                    creditCard.val(creditCard.val());
                }
                break;
            case 3:
                exp = new RegExp("^[0-9]{3}$", "m");

                if (!exp.test(nc)) {
                    creditCard.val(nc[0]);
                    return false;
                } else {
                    creditCard.val(creditCard.val());
                }
                break;
            case 4:
                exp = new RegExp("^[0-9]{4}$", "m");

                if (!exp.test(nc)) {
                    creditCard.val(nc[0]);
                    return false;
                } else {
                    creditCard.val(creditCard.val() + " ");
                }
                break;

            case 9:
                creditCard.val(creditCard.val() + " ");
                break;
            case 14:
                creditCard.val(creditCard.val() + " ");
                break;

        }
    });

    creditCard.keypress(function () {
        if ((event.keyCode < 48 || event.keyCode > 57)) return false;
    });
    creditCard.keyup(function () {
        var nc = creditCard.val();
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
    cardDate.blur(function () {
        validexpdat();
    });
    cvv.keypress(function () {
        if ((event.keyCode < 48 || event.keyCode > 57)) return false;
    });
    cvv.keyup(function () {
        var ccnum = creditCard.val();

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

    txtzipcode.keypress(function () {
        if ((event.keyCode < 48 || event.keyCode > 57)) return false;

        if (this.value.length > 5) {
            this.value = this.value.slice(0, 4);
        }
    });
    txtzipcode.keyup(function () {
        if (this.value.length > 3) {
            allisok();
        }
    });
    txtzipcode.blur(function () {
        if (this.value.length > 3) {
            allisok();
        }
    });
    cardDate.keyup(function (event) {
        var keyCode = event.keyCode || event.which;
        if (keyCode == 8 || (keyCode == 127 && keyCode == 46)) {
            return false;
        }

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
    emailaddress.blur(function() {
        validEmail();
    });

    // ENABLE - DISABLE PURCHASE BUTTON
    $('.input-group input').keyup(function () {
        var empty = false;
        var isValidZip = true;
        var hidSelectedDate = $.jCookie('SelectedCheckInDate');
        $('.input-group input').each(function () {
            if ((!$(this).hasClass("discount-text") && !$(this).hasClass('datepicker') && $(this).val() == '')
                || ($(this).hasClass('datepicker') && hidSelectedDate === "0" && $(this).val() == '')) {
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
    purchaseButton.on('click', function () {
        var err = false;

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
            .html("SIT TIGHT...WE'RE BOOKING<br/> YOUR DAY PASS<div class=\"imgspin text-center\"><img src=\"/images/loading.gif\" alt=\"\"></div>")
            .show();
    });

    if ($('#UseNewCard').length <= 0) {
        purchaseButton.unbind('click', false);
    } else {
        purchaseButton.removeClass("btngray").bind('click', true);
    }

    // ALTERNATIVE PLACEHOLDER FOR EMAIL ADDRESS
    var emailplaceholder = $(".emailplaceholder");
    if (emailaddress.length > 0 && emailaddress.val().length > 0) {
        emailplaceholder.hide();
    }

    emailaddress.focus(function () {
        emailplaceholder.hide();
    }).blur(function () {
        var getValEmail = emailaddress.val();
        emailplaceholder.show();
        if (getValEmail != "") {
            emailplaceholder.hide();
        }
    });
    emailplaceholder.click(function () {
        emailaddress.focus();
        emailplaceholder.hide();
    });

    if (creditCard.length > 0) {
        creditCard.validateCreditCard(function (result) {
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
});