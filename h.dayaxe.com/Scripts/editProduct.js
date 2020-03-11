$(function () {
    if ($("#ProductNameText").length > 0) {
        var hotelName = $("#cntrProductName");
        $("#ProductNameText").limiter(50, hotelName);
    }

    $('#MaxGuestControl a').click(function () {
        $('.maxguest-current-value').text($(this).text());
        $('#HidMaxGuest').val($(this).text());
    });

    $('#RedemptionPeriod a').click(function () {
        $('.redemp-period-value').text($(this).text());
        $('#HidRedemptionPeriod').val($(this).text());
    });

    //var isKidAllow = ($('#IsKidAllowHidden').val().toLowerCase() === 'true');
    //var isKidAllowCtrl = $('#isKidAllow').toggles({
    //    on: isKidAllow,
    //    width: 100,
    //    height: 30
    //});
    //$(isKidAllowCtrl).on('toggle', function (e, active) {
    //    $('#IsKidAllowHidden').val(active ? 'true' : 'false');
    //});

    var isCheckedInRequired = ($('#IsCheckedInRequiredHidden').val().toLowerCase() === 'true');
    var isCheckedInRequiredCtrl = $('#isCheckedInRequired').toggles({
        on: isCheckedInRequired,
        width: 100,
        height: 30
    });
    $(isCheckedInRequiredCtrl).on('toggle', function (e, active) {
        $('#IsCheckedInRequiredHidden').val(active ? 'true' : 'false');
    });

    var isFeaturedRequired = ($('#IsFeaturedRequiredHidden').val().toLowerCase() === 'true');
    var isFeaturednRequiredCtrl = $('#isFeaturedRequired').toggles({
        on: isFeaturedRequired,
        width: 100,
        height: 30
    });
    $(isFeaturednRequiredCtrl).on('toggle', function (e, active) {
        $('#IsFeaturedRequiredHidden').val(active ? 'true' : 'false');
    });

    if ($('#ProductImage').length > 0) {
        $("#ProductImage").fileinput({
            previewFileType: "image",
            browseClass: "btn btn-success",
            browseLabel: "Pick Image",
            browseIcon: "<i class=\"glyphicon glyphicon-picture\"></i> ",
            removeClass: "btn btn-danger",
            removeLabel: "Delete",
            removeIcon: "<i class=\"glyphicon glyphicon-trash\"></i> ",
            uploadClass: "",
            uploadLabel: "",
            uploadIcon: "",
            uploadUrl: window.uploadUrl,
            uploadAsync: true,
            showPreview: false,
            maxFileCount: 1,
            allowedFileExtensions: ['jpg', 'png', 'gif']
        });
    }

    $('.photo-list ul').sortable({
        update: function (event, ui) {
            var items = $(this).find('.photo-item');
            items.each(function (idx, item) {
                $($(item).find('input[type="hidden"]')[0]).val(idx + 1);
            });
        }
    });

    $('.remove-image').on('click', function () {
        var id = $(this).data('value');
        $.ajax({
            url: "/Handler/UpdateHotel.ashx",
            method: "POST",
            data: { productPhotoId: id },
            dataType: "json",
            success: function (data) {
                location.reload();
            },
            error: function (xhr, status, thrown) {
                location.reload();
            }
        });
        return false;
    });

    $('.delete-product').on('click', function () {
        return confirm('Are you sure you\'d like to delete ' + $('#ProductNameText').val() + '?');
    });

    $('#SaveButton').click(function () {
        var errors = $('.error-message');
        if (errors.length > 0) {
            errors.each(function () {
                var visibilityAttr = $(this).css('visibility');
                if (visibilityAttr == "visible") {
                    var tabPane = $(this).closest('.tab-pane');
                    if (!tabPane.hasClass('active')) {
                        $('a[href*=' + tabPane.attr('id') + ']').click();
                    }
                    var offsetTop = $(this).offset().top - 150;
                    $(window).scrollTop(offsetTop > 0 ? offsetTop : 0);
                }
            });

        }
    });
});