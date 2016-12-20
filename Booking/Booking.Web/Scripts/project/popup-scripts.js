function displayCancelEventPopup() {
    var id = $("#btn-show-cancel-popup").attr("data-eventid");
    var eventUrl = $("#cancel-event-popup-url").val();
    eventUrl += "?eventId=" + id;
    $("#cancel-event-popup").load(eventUrl);
}

function closeCancelEventPopup(id) {
    $("#CancelEvent").modal("toggle");
    forceScheduleReload();
    closeDisplayEventPopup(id);
}

function closeDisplayEventPopup(id) {
    $("#display-event-popup-container-" + id).remove();
}

function redirectToEventPage(id) {
    var url = $("#redirect-to-event-url").val() + "?eventId=" + id;
    window.location.replace(url);
}

function redirectToEditEventPage(id) {
    var url = $("#redirect-to-edit-event-url").val() + "?eventId=" + id;
    window.location.replace(url);
}

function joinEvent(id) {
    var formId = "join-event-form-" + id;
    $("#" + formId + " .join-event-submit").click();
}

function displayEventPopup(id) {
    var url = $("#display-event-popup-url").val() + "?eventId=" + id;
    $("#display-event-popup-container-" + id).load(url);
}