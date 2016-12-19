var onEventCreateSucceeded = function() {
    $("#CreateEventModal").modal('hide');
    forceScheduleReload();
}
$(document)
    .ready(function() {
        $("#btn-new-event-popup")
            .click(function() {
                var eventUrl = $("#get-new-event-popup-url").val();
                $("#new-event-popup")
                    .load(eventUrl,
                        function() {
                            $("#new-event-popup-end-date, #AudienceId").change(checkIfAudienceIsFree);

                            configureDatetimeUpdown("new-event-popup-event-date","new-event-popup-start-date","new-event-popup-end-date");

                            checkIfAudienceIsFree();
                        });
            });
    });

function checkIfAudienceIsFree() {
    var audienceId = document.getElementById("AudienceId").value;

    var eventStartDate = $("#new-event-popup-start-date").val();
    var eventEndDate = $("#new-event-popup-end-date").val();

    var url = $("#audience-is-free-url").val() +
        "?audienceId=" +
        audienceId +
        "&startEvent=" +
        eventStartDate +
        "&endEvent=" +
        eventEndDate;

    $.get(url)
        .done(function(isFree) {
            toggleIsFreeMessage(isFree);
        });
}

function toggleIsFreeMessage(isFree) {
    if (isFree === 'False') {
        $(".audience-not-free-message").css("visibility", 'visible');
    } else {
        $(".audience-not-free-message").css("visibility", 'hidden');
    }

}