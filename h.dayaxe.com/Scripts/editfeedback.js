$(function() {
    if ($('#SurveyRating').length > 0) {
        $("#SurveyRating").rating({ min: 1, max: 5, step: 0.5 });
        $('#SurveyRating').rating('refresh', { showClear: false, showCaption: false });
    }

    $('.delete-survey').on('click', function () {
        return confirm('Are you sure you want to delete this review?');
    });

    $('.buy-any input').change(function () {
        var input = $(this).closest('.row').find('input[type=text]');
        if ($(this).is(":checked")) {
            input.removeAttr('disabled');
            input.val(input.attr('data-value'));
        } else {
            input.attr('disabled', 'disabled');
            input.val('');
        }
    });
});