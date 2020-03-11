$(function () {
    $('.hotel-item').on('click', function () {
        var editHotel = $(this).find('.edit-hotel');
        if (editHotel.length > 0) {
            location.href = editHotel.attr('href');
        } else {
            location.href = $(this).data('href');
        }
    });

    $('.delete-product').on('click', function () {
        return confirm('Are you sure you\'d like to delete ' + $(this).closest('tr').find('td:eq(1)').text().replace(/(\r\n|\n|\r)/gm, "").trim() + '?');
    });
});