$(function() {
    $('#TargetAudienceDiv .pointer, #GenderDiv .pointer, #EducationDiv .pointer').click(function () {
        $(this).toggleClass('alter-target');
    });

    function getObjectTarget(selector) {
        var targets = '';
        $(selector).each(function () {
            if ($(this).hasClass('alter-target')) {
                targets = targets + $(this).data('value') + '|';
            }
        });
        return targets;
    }

    $('#Save').click(function () {
        var targets = getObjectTarget('#TargetAudienceDiv .pointer');
        var genders = getObjectTarget('#GenderDiv .pointer');
        var educations = getObjectTarget('#EducationDiv .pointer');
        $('.saving').text('Saving...').removeClass('hidden');
        $.ajax({
            type: "POST",
            url: "/Handler/SaveData.ashx",
            context: window,
            data: {
                "id": $('#HidHotelId').val(),
                "userName": window.userName,
                "target": targets,
                "gender": genders,
                "education": educations,
                "incomCurrent": $('.income-current-value').text(),
                "distanceCurrent": $('.distance-current-value').text(),
                "ageFromCurrent": $('.age-from-current-value').text(),
                "ageToCurrent": $('.age-to-current-value').text()
            },
            success: function (response) {
                $('.saving').text('Saved!').animate({
                    opacity: 0
                }, 1500, function () { });
            }
        });
        return false;
    });
});