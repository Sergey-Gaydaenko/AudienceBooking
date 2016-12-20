$(document)
    .ready(function () {
        $(".room-proxy").mouseenter(onRoomProxyMouseEnterChangeColor);
        $(".room-proxy").mouseleave(onRoomProxyMouseLeaveChangeColor);
        $(".room-proxy").click(onRoomProxyClickRedirectToAudiencePage);
    });

var id = $("#Id").val();
$("#close-audience-" + id).click(function() {
    var url = $("#audience-cancel-page-url").val();
    url += "?audienceId=" + id;
    $("#close-audience-popup").load(url);
});

$("#open-audience-" + id).click(function () {
    var url = $("#audience-open-url").val();
    url += "?audienceId=" + id;
    $.post(url).done(function() {
        location.reload();
    });
    
});

$("#edit-audience-" + id).click(function () {
    var url = $("#audience-edit-url").val();
    url += "?audienceId=" + id;
    window.location.replace(url);
});

$("#cancel-event-edit-changes-" + id).click(function() {
    var url = $("#audience-go-to-index-url").val();
    url += "?audienceId=" + id;
    window.location.replace(url);
});

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if ((charCode < 48 || charCode > 57))
        return false;

    return true;
}