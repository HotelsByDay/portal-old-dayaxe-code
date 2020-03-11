$(function () {
    $('.de-active').on('click', function() {
        var hotelId = $(this).closest('td').data('id');
        var conf = confirm('Are you sure you want to de-activate listing?');
        if (conf) {
            $.ajax({
                url: window.updateUrl,
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
    $('.delete-hotel').on('click', function() {
        var hotelId = $(this).closest('td').data('id');
        var conf = confirm('Are you sure you want to delete listing?');
        if (conf) {
            $.ajax({
                url: window.updateUrl,
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
    $('.hotel-item').on('click', function () {
        var editHotel = $(this).find('.edit-hotel');
        if (editHotel.length > 0) {
            location.href = editHotel.attr('href');
        } else {
            location.href = $(this).data('href');
        }
    });
});