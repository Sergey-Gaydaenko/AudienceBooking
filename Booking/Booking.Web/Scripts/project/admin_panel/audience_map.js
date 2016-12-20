$(document)
    .ready(function () {
        $("#new-audience-btn")
            .click(function () {
                var $mapContainer = $("#map-container");
                var children = $("#map-container").children("#new-audience-div");
                if (children.length === 0) {
                    var $modeBtn = $("<button id='mode-btn' class='btn-primary'>m</button>");
                    var $audience = $("<div id='new-audience-div'></div>");
                    $audience.append($modeBtn);
                    $audience.draggable();
                    $mapContainer.append($audience);

                    var $newAudienceForm = $("#new-audience-form");
                    $newAudienceForm.css("visibility", "visible");
                    $("#new-audience-btn").css("visibility", "hidden");
                    $newAudienceForm.submit(function () {
                        $("#Left").val(parseInt($audience.css("left")));
                        $("#Top").val(parseInt($audience.css("top")));
                        $("#Width").val(parseInt($audience.css("width")));
                        $("#Height").val(parseInt($audience.css("height")));
                    });

                    $modeBtn.click(function () {
                        var $elem = $("#new-audience-div");
                        if ($elem.data("uiDraggable")) {
                            $elem.draggable("destroy");
                            $elem.resizable();
                        } else {
                            $elem.resizable("destroy");
                            $elem.draggable();
                        }
                    });
                }
            });
    });