$(function () {
    $('.edit-policy').click(function() {
        $('.policy-text input[type=hidden]').val($(this).attr('data-id'));
        $('.policy-text input[type=text]').val($(this).attr('data-name'));
        $('.policy-text').siblings('div:eq(0)').find('input').val('Update');
    });
});