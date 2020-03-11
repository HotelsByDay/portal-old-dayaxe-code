$(function () {
    $('.delete-user').on('click', function () {
        var userId = $(this).closest('td').data('id');
        var conf = confirm('Are you sure you want to delete user?');
        if (conf) {
            $.ajax({
                url: "/Handler/UpdateUser.ashx",
                method: "POST",
                data: { id: userId, isDelete: true },
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
});