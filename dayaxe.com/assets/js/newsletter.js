$(function() {
    $('#SubmitNewsLetter').click(function(e) {
        e.preventDefault();
        $.ajax({
            url: "/Handler/NewsLetter.ashx",
            type: "POST",
            dataType: 'json',
            data: { e: $(".email-newsletter").val() },
            success: function (result) {
                if (result.IsSuccess) {
                    $('.row-newsletter > .col-md-5').hide();
                    $('.row-newsletter > .col-md-7').removeClass('col-md-7').addClass('col-md-12');
                    $(".row-newsletter .text span").addClass('message-newsletter success').text(result.Message);
                } else {
                    $('.message-newsletter').addClass('error').text(result.Message);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $(".row-newsletter .text span").addClass('error').text('We are sorry, there was an error processing your request.');
            }
        });

    });
});