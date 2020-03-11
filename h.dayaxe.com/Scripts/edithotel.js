$(function () {

    var isCommingSoon = ($('#IsCommingSoonHidden').val().toLowerCase() === 'true');
    var isCommingSoonCtr = $('#isCommingSoon').toggles({
        on: isCommingSoon,
        width: 100, 
        height: 30
    });
    $(isCommingSoonCtr).on('toggle', function (e, active) {
        $('#IsCommingSoonHidden').val(active ? 'true' : 'false');
    });

    var isPublished = ($('#IsPublishedHidden').val().toLowerCase() === 'true');
    var isPublishedCtrl = $('#isPublished').toggles({
        on: isPublished,
        width: 100,
        height: 30
    });
    $(isPublishedCtrl).on('toggle', function (e, active) {
        $('#IsPublishedHidden').val(active ? 'true' : 'false');
    });

    $('#RedemptionPeriod a').click(function () {
        $('.redemp-period-value').text($(this).text());
        $('#HidRedemptionPeriod').val($(this).text());
    });

    if ($("#HotelName").length > 0) {
        var hotelName = $("#cntrHotelName");
        $("#HotelName").limiter(50, hotelName);
    }

    if ($("#Recommendation").length > 0) {
        var recommend = $("#cntrRecommendation");
        $("#Recommendation").wordsLimiter(1000, recommend);
    }

    if ($('#Rating').length > 0) {
        $("#Rating").rating({ min: 1, max: 5, step: 0.5 });
        $('#Rating').rating('refresh', { showClear: false, showCaption: false });
    }

    if ($('#HotelImage').length > 0) {
        $("#HotelImage").fileinput({
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
            allowedFileExtensions: ['jpg', 'png', 'gif'],
            uploadExtraData: function () { // callback example
                return {
                    type: $('#HidphotoType').val()
                }
            }
        });
    }
    $('.hotel-type-grid .col-single').click(function () {
        $('.hotel-type-grid .col-single').removeClass('alter-target');
        $('#HotelTypeId').val($(this).attr('value'));
        $(this).addClass('alter-target');
    });

    $('.de-active').on('click', function () {
        var hotelId = $(this).closest('td').data('id');
        var conf = confirm('Are you sure you want to de-activate listing?');
        if (conf) {
            $.ajax({
                url: "/Handler/UpdateHotel.ashx",
                method: "POST",
                data: { id: hotelId, deActive: true },
                type: "json",
                success: function (data) {
                    location.reload();
                },
                error: function (xhr, status, thrown) {
                    location.reload();
                }
            });
        }
        return false;
    });
    $('.delete-hotel').on('click', function () {
        var hotelId = $(this).closest('td').data('id');
        var conf = confirm('Are you sure you want to delete listing?');
        if (conf) {
            $.ajax({
                url: "/Handler/UpdateHotel.ashx",
                method: "POST",
                data: { id: hotelId, isActive: true },
                dataType: "json",
                success: function (data) {
                    location.reload();
                },
                error: function (xhr, status, thrown) {
                    location.reload();
                }
            });
        }
        return false;
    });
    $('.remove-image').on('click', function () {
        var id = $(this).data('value');
        $.ajax({
            url: "/Handler/UpdateHotel.ashx",
            method: "POST",
            data: { photoId: id },
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
    $('#PhotoType a').click(function () {
        $('.photo-current-value').text($(this).text());
        $('#HidphotoType').val($(this).data('value'));
        $('.select-photo-type').removeClass('open');
        return false;
    });

    $('.amenties-list ul').sortable({
        update: function (event, ui) {
            var items = $(this).find('.amenity-item');
            items.each(function (idx, item) {
                $($(item).find('input[type="hidden"]')[0]).val(idx + 1);
            });
        }
    });

    $('.photo-list ul').sortable({
        update: function (event, ui) {
            var items = $(this).find('.photo-item');
            items.each(function (idx, item) {
                $($(item).find('input[type="hidden"]')[0]).val(idx + 1);
            });
        }
    });

    $('#SaveButton').click(function () {
        var errors = $('.error-message');
        if (errors.length > 0) {
            errors.each(function() {
                var visibilityAttr = $(this).css('visibility');
                if (visibilityAttr == "visible") {
                    var offsetTop = $(this).offset().top - 150;
                    $(window).scrollTop(offsetTop > 0 ? offsetTop : 0);
                }
            });
            
        }
    });

    $('.ddl-policies option').each(function () {
        $(this).html($(this).text());
    });
});