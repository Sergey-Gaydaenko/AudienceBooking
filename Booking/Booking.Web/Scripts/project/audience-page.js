$(document)
    .ready(function () {
        $(".room-proxy").mouseenter(onRoomProxyMouseEnterChangeColor);
        $(".room-proxy").mouseleave(onRoomProxyMouseLeaveChangeColor);
        $(".room-proxy").click(onRoomProxyClickRedirectToAudiencePage);
    });