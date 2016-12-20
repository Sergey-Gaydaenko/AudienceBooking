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
    $.post(url);
    location.reload();
});

$("#edit-audience-" + id).click(function () {
    var url = $("#audience-edit-url").val();
    url += "?audienceId=" + id;
    $.get(url);
});
//var closeAudienceClosePopup = function() {
//    $("#CloseAudience").modal('hide');
//}