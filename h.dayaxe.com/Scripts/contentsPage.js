$(function () {
    function getPermalink(val) {
        return val.trim().replace(/\s+/g, '-').toLowerCase();
    }

    if ($("#UrlSegmentText").length > 0) {
        $("#UrlSegmentText").val(getPermalink($("#PageNameText").val()));
        $("#UrlLabel").text(window.baseUrl + getPermalink($("#PageNameText").val()));
    }

    $("#PageNameText").keyup(function () {
        var tempValue = getPermalink($(this).val());
        $("#UrlSegmentText").val(tempValue);
        $("#UrlLabel").text(window.baseUrl + tempValue);
    });

    if ($('#ImageLandingDesktopFileUpload').length > 0) {
        $("#ImageLandingDesktopFileUpload").fileinput({
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
            uploadUrl: window.uploadUrl,// + '&type=' + $('#HidphotoType').val(), // server upload action
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

    if ($('#ImageLandingMobileFileUpload').length > 0) {
        $("#ImageLandingMobileFileUpload").fileinput({
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
            uploadUrl: window.uploadUrl,// + '&type=' + $('#HidphotoType').val(), // server upload action
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
});