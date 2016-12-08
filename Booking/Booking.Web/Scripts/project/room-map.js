﻿function onRoomProxyMouseEnter() {
    var targetDivId = "#" + $(this).data("room-target");
    $(targetDivId).addClass("room-proxy-hover");
    var audienceId = $(targetDivId).data("audience-id");
    var audienceInfoUrl = $("#get-audience-info-url").val();
    var $div = $("<div class='audience-info'></div>");
    $("#room-map-container").append($div);
    $div.load(audienceInfoUrl + "?audienceId=" + audienceId);
}

function onRoomProxyMouseLeave() {
    var targetDivId = $(this).data("room-target");
    $("#" + targetDivId).removeClass("room-proxy-hover");
    $(".audience-info").remove();
}