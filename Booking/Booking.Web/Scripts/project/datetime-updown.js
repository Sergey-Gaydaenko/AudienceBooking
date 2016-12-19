function configureDatetimeUpdown(containerId, startDateId, endDateId) {
    var nextAvailableDateUrl = $("#get-next-available-date-url").val();
    var previousAvailableDateUrl = $("#get-previous-available-date-url").val();

    function parseMvcDate(dateStr) {
        return new Date(parseInt(dateStr.replace("/Date(", "").replace(")/", ""), 10));
    }

    var $container = $("#" + containerId);
    var $startDate = $("#" + startDateId);
    var $endDate = $("#" + endDateId);

    var $day = $container.find(".datetime-updown-day").first();
    var $month = $container.find(".datetime-updown-month").first();
    var $year = $container.find(".datetime-updown-year").first();
    var $startHour = $container.find(".datetime-updown-start-hour").first();
    var $startMinute = $container.find(".datetime-updown-start-minute").first();
    var $endHour = $container.find(".datetime-updown-end-hour").first();
    var $endMinute = $container.find(".datetime-updown-end-minute").first();

    var $dayView = $day.find("span").first();
    var $monthView = $month.find("span").first();
    var $yearView = $year.find("span").first();
    var $startHourView = $startHour.find("span").first();
    var $startMinuteView = $startMinute.find("span").first();
    var $endHourView = $endHour.find("span").first();
    var $endMinuteView = $endMinute.find("span").first();

    var monthDisplayedValues;
    var displayMonthName = $month.hasClass("datetime-updown-month-name");
    if (displayMonthName) {
        monthDisplayedValues = [
            "Января", "Февраля", "Марта", "Апреля",
            "Мая", "Июня", "Июля", "Августа", "Сентября",
            "Октября", "Ноября", "Декабря"
        ];
    } else {
        monthDisplayedValues = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"];
    }

    var lowerHoursBound, upperHoursBound;

    var startDate = new Date($startDate.val());
    var endDate = new Date($endDate.val());

    function setStartDate() {
        $startDate.val(startDate.toISOString()).trigger("change");
    }

    function setEndDate() {
        $endDate.val(endDate.toISOString()).trigger("change");
    }

    function updateView() {
        var day = startDate.getDate();
        day = (day >= 10 ? day : "0" + day) + (displayMonthName ? "" : ".");
        $dayView.text(day);
        var month = startDate.getMonth();
        month = monthDisplayedValues[month];
        $monthView.text(month);
        $yearView.text(startDate.getFullYear().toString().substr(2, 2));

        var hours = startDate.getHours();
        hours = (hours >= 10 ? hours : "0" + hours) + ":";
        $startHourView.text(hours);
        var minutes = startDate.getMinutes();
        minutes = (minutes >= 10 ? minutes : "0" + minutes);
        $startMinuteView.text(minutes);

        hours = endDate.getHours();
        hours = (hours >= 10 ? hours : "0" + hours) + ":";
        $endHourView.text(hours);
        minutes = endDate.getMinutes();
        minutes = (minutes >= 10 ? minutes : "0" + minutes);
        $endMinuteView.text(minutes);
    }


    function setAndUpdate() {
        setStartDate();
        setEndDate();
        updateView();
    }

    function addMinutes(date, minutes) {
        return new Date(date.getTime() + minutes * 60000);
    }

    function diffInMinutes(date1, date2) {
        return Math.floor((date1 - date2) / 60000);
    }

    function getUrlWithDate(url, date) {
        return url + "?date=" + date.toISOString();
    }

    function setNextAvailableDate(date) {
        $.get(getUrlWithDate(nextAvailableDateUrl, date))
            .done(function(data) {
                startDate = parseMvcDate(data.Date);
                endDate = addMinutes(startDate, 20);
                lowerHoursBound = data.StartHour;
                upperHoursBound = data.EndHour;
                setAndUpdate();
            });
    }

    function setPreviousAvailableDate(date) {
        $.get(getUrlWithDate(previousAvailableDateUrl, date))
            .done(function(data) {
                startDate = parseMvcDate(data.Date);
                endDate = addMinutes(startDate, 20);
                lowerHoursBound = data.StartHour;
                upperHoursBound = data.EndHour;
                setAndUpdate();
            });
    }

    if (!isDateValid(endDate, startDate)) {
        setNextAvailableDate(new Date);
    } else {
        var getScheduleRuleUrl = getUrlWithDate($("#get-schedule-rule-url").val(), startDate);
        $.get(getScheduleRuleUrl)
            .done(function (data) {
                lowerHoursBound = data.StartHour;
                upperHoursBound = data.EndHour;
                setAndUpdate();
            });
    }

    function isDateValid(eventStart, eventEnd) {
        var startHours = eventStart.getHours();
        var endHours = eventEnd.getHours();
        if (eventEnd.getMinutes()) {
            endHours ++;
        }
        if (startHours < lowerHoursBound || startHours > upperHoursBound) {
            return false;
        }
        if (endHours > upperHoursBound || endHours < lowerHoursBound) {
            return false;
        }

        var dateNow = new Date();
        return dateNow < eventStart;
    }

    function setMaxDayTimeOnlyEnd() {
        endDate.setHours(upperHoursBound);
        endDate.setMinutes(0);
    }

    function setMaxDayTime() {
        endDate.setHours(upperHoursBound);
        endDate.setMinutes(0);
        startDate = addMinutes(endDate, -20);
    }

    function setMinDayTimeOnlyStart() {
        startDate.setHours(lowerHoursBound);
        startDate.setMinutes(0);
        var now = new Date();
        if (startDate < now) {
            startDate = now;
        }
    }

    function setMinDayTime() {
        var now = new Date();
        startDate.setHours(lowerHoursBound);
        startDate.setMinutes(0);
        if (startDate < now) {
            startDate = now;
        }
        endDate = addMinutes(startDate, 20);
    }

    $day.find(".fa-caret-up")
        .click(function() {
            setNextAvailableDate(startDate);
            setAndUpdate();
        });
    $day.find(".fa-caret-down")
        .click(function() {
            setPreviousAvailableDate(startDate);
            setAndUpdate();
        });

    $month.find(".fa-caret-up")
        .click(function() {
            startDate.setMonth(startDate.getMonth() + 1);
            startDate.setDate(startDate.getDate() - 1);
            setNextAvailableDate(startDate);
            setAndUpdate();
        });
    $month.find(".fa-caret-down")
        .click(function() {
            startDate.setMonth(startDate.getMonth() - 1);
            startDate.setDate(startDate.getDate() - 1);
            setPreviousAvailableDate(startDate);
            setAndUpdate();
        });

    $year.find(".fa-caret-up")
        .click(function() {
            startDate.setYear(startDate.setYear() + 1);
            startDate.setDate(startDate.getDate() - 1);
            setNextAvailableDate(startDate);
            setAndUpdate();
        });
    $year.find(".fa-caret-down")
        .click(function() {
            startDate.setYear(startDate.setYear() - 1);
            startDate.setDate(startDate.getDate() - 1);
            setPreviousAvailableDate(startDate);
            setAndUpdate();
        });

    $startHour.find(".fa-caret-up")
        .click(function() {
            startDate.setHours(startDate.getHours() + 1);
            var diff = diffInMinutes(endDate, startDate);
            if (diff < 20) {
                endDate = addMinutes(startDate, 20);
            };
            if (!isDateValid(startDate, endDate)) {
                setMaxDayTime();
            }
            setAndUpdate();
        });
    $startHour.find(".fa-caret-down")
        .click(function() {
            startDate.setHours(startDate.getHours() - 1);
            if (!isDateValid(startDate, endDate)) {
                setMinDayTimeOnlyStart();
            }
            setAndUpdate();
        });

    $endHour.find(".fa-caret-up")
        .click(function() {
            endDate.setHours(endDate.getHours() + 1);
            if (!isDateValid(startDate, endDate)) {
                setMaxDayTimeOnlyEnd();
            }
            setAndUpdate();
        });
    $endHour.find(".fa-caret-down")
        .click(function() {
            endDate.setHours(endDate.getHours() - 1);
            var diff = diffInMinutes(endDate, startDate);
            if (diff < 20) {
                startDate = addMinutes(endDate, -20);
                if (!isDateValid(startDate, endDate)) {
                    setMinDayTime();
                }
            };
            setAndUpdate();
        });

    $startMinute.find(".fa-caret-up")
        .click(function() {
            startDate.setMinutes(startDate.getMinutes() + 10);
            var diff = diffInMinutes(endDate, startDate);
            if (diff < 20) {
                endDate = addMinutes(startDate, 20);
            };
            if (!isDateValid(startDate, endDate)) {
                setMaxDayTime();
            }
            setAndUpdate();
        });
    $startMinute.find(".fa-caret-down")
        .click(function() {
            startDate.setMinutes(startDate.getMinutes() - 10);
            if (!isDateValid(startDate, endDate)) {
                setMinDayTimeOnlyStart();
            }
            setAndUpdate();
        });

    $endMinute.find(".fa-caret-up")
        .click(function() {
            endDate.setMinutes(endDate.getMinutes() + 10);
            if (!isDateValid(startDate, endDate)) {
                setMaxDayTimeOnlyEnd();
            }
            setAndUpdate();
        });
    $endMinute.find(".fa-caret-down")
        .click(function() {
            endDate.setMinutes(endDate.getMinutes() - 10);
            var diff = diffInMinutes(endDate, startDate);
            if (diff < 20) {
                startDate = addMinutes(endDate, -20);
                if (!isDateValid(startDate, endDate)) {
                    setMinDayTime();
                }
            };
            setAndUpdate();
        });
}