$(function () {
    if ($("#SubscriptionNameText").length > 0) {
        var hotelName = $("#cntrProductName");
        $("#SubscriptionNameText").limiter(50, hotelName);
    }

    $('#MaxGuestControl a').click(function () {
        $('.maxguest-current-value').text($(this).text());
        $('#HidMaxGuest').val($(this).text());
    });

    if ($('#SubscriptionImage').length > 0) {
        $("#SubscriptionImage").fileinput({
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
            data: { sPhotoId: id },
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
        return confirm('Are you sure you\'d like to delete ' + $('#SubscriptionNameText').val() + '?');
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