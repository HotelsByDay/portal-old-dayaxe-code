var promoType = $('#PromoTypeDdl');
function setPromo() {
    var amountOff = $('.amount-off');
    if (promoType.val() == '0') {
        amountOff.text('%');
    } else {
        amountOff.text('$');
    }
}

$(function () {
    var isCodeRequired = ($('#IsCodeRequiredHidden').val().toLowerCase() === 'true');
    var isCodeRequiredCtrl = $('#isCodeRequired').toggles({
        on: isCodeRequired,
        width: 100, 
        height: 30
    });
    $(isCodeRequiredCtrl).on('toggle', function (e, active) {
        $('#IsCodeRequiredHidden').val(active ? 'true' : 'false');
    });

    var isAllProducts = ($('#IsAllProductHidden').val().toLowerCase() === 'true');
    var isAllProductsCtrl = $('#isAllProduct').toggles({
        on: isAllProducts,
        width: 100, 
        height: 30
    });
    $(isAllProductsCtrl).on('toggle', function (e, active) {
        $('#IsAllProductHidden').val(active ? 'true' : 'false');
    });
    promoType.change(function() {
        setPromo();
    });

    setPromo();
});