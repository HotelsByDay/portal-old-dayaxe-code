$(function () {
    function getPermalink(val) {
        return val.toLowerCase().replace(" ", "-").trim();
    }

    $("#PermalinkText").val(getPermalink($("#LocationNameText").val()));

    $("#LocationNameText").keyup(function () {
        var tempValue = getPermalink($(this).val());
        $("#PermalinkText").val(tempValue);
    });

    $("#MarketImage").fileinput({
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
        allowedFileExtensions: ['jpg', 'png', 'gif', 'jpeg']
    });
});