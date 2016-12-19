function timestampIsBetween(time, left, right) {
    var timeMins = time.getHours() * 60 + time.getMinutes();
    var leftMins = left.getHours() * 60 + left.getMinutes();
    var rightMins = right.getHours() * 60 + right.getMinutes();
    return timeMins > leftMins && timeMins < rightMins;
}

urlBuilderCallback = function(date) {
    var authorIdParameter = "&authorId=" + $("#author-id").val();
    return $("#get-day-schedule-url").val() + "?date=" + date.toISOString() + authorIdParameter;
}

$(document)
    .ready(function() {
        rebuildTable(10, 19, [{ Id: 0, Name: "Empty" }]);
        tdWidth = parseInt($("#schedule-contents-table td").css("width"));
        tdHeight = parseInt($("#schedule-contents-table td").css("height"));
        thHeight = parseInt($("#schedule-contents-table th").css("height"));


        $("#slider-draggable")
            .draggable({
                axis: "x",
                containment: "parent",
                drag: function(event, ui) {
                    var t = posToTime(lowerHourBound, upperHourBound, tdWidth, ui.position.left);
                    setDraggableSliderCaption(timeToStringHHMM(t));
                    checkSliderNowPosition();
                },
                stop: checkAndSetDraggableSliderPosition
            });

        var time = new Date();
        var pos = timeToPos(lowerHourBound, upperHourBound, tdWidth, time);

        if (pos > (upperHourBound - lowerHourBound) * tdWidth) {
            pos = (upperHourBound - lowerHourBound) * tdWidth;
        } else if (pos < 0) {
            pos = 0;
        }

        moveSlider($("#slider-now"), pos);
        moveSlider($("#slider-draggable"), pos);
        $("#schedule-viewport-outer").scrollLeft(pos);

        setDraggableSliderCaption(timeToStringHHMM(time));

        setInterval(function() {
                moveSliderNow(lowerHourBound, upperHourBound, tdWidth);
            },
            60000);


        $("#schedule-mode-table-only").click(toggleTableOnlyMode);
        $("#schedule-mode-with-calendar").click(toggleWithCalendarMode);

        $("#decrement-date-button").click(decrementDate);
        $("#increment-date-button").click(incrementDate);

        var $datepicker = $("#datepicker");

        var disabledDays = {};

        function loadMonth(date) {
            var month = date.getMonth() + 1;
            var year = date.getFullYear();
            var url = $("#get-disabled-days-month-url").val() + "?month=" + month + "&year=" + year;
            $.get(url)
                .done(function(data) {
                    disabledDays[data.Month.toString() + data.Year.toString()] = data.Days;
                });
        }

        loadMonth(time);

        setTimeout(configureDatepicker, 300);

        function configureDatepicker() {
            $datepicker.datepicker({
                language: "ru",
                beforeShowDay:
                    function(dt) {
                        var month = dt.getMonth() + 1;
                        var key = month.toString() + dt.getFullYear().toString();
                        return disabledDays && key in disabledDays && !disabledDays[key].includes(dt.getDate());
                    },
                changeMonth: true,
                changeYear: false
            });

            $datepicker.on("changeDate",
                function() {
                    dateChangedEvent($datepicker.datepicker("getDate"),
                        function() {
                        });
                });

            $datepicker.on("changeMonth",
                function(dt) {
                    var time = new Date(dt.date.getTime());
                    time.setMonth(time.getMonth() + 1);
                    loadMonth(time);
                });

            setDateToday();

            moveSliderNow(lowerHourBound, upperHourBound, tdWidth);

            toggleWithCalendarMode();

            $(".btn-goto-today").click(function() { setDateToday(); });

            $(".btn-goto-now")
                .click(function() {
                    bindDraggableSliderToNow();
                });

            checkSliderNowPosition();
        }
    });

function forceScheduleReload() {
    dateChangedEvent($("#datepicker").datepicker("getDate"));
}