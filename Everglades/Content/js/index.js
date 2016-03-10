// function "isInteger"
function isInteger(value) {
    return typeof value === "number" &&
      isFinite(value) &&
      Math.floor(value) === value;
};

// return date format from timestamp*1000
function formatDate(x) {
    date = new Date(Math.floor(x));
    return date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();// + " " + d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
}

// function to buy asset
function asset_buy(name, number) {
    var data = { operation: 'buy', asset: name, number: number };
    $.ajax({
        type: "POST",
        url: "/operations",
        data: data,
        datatype: "html",
        success: function (data) {
            alert(data);
            location.reload();
        }
    })
    .fail(function (jqXHR, textStatus) {
        alert("Connection error : could not buy asset");
    });
}

// function to sell asset
function asset_sell(name, number) {
    var data = { operation: 'sell', asset: name, number: number };
    $.ajax({
        type: "POST",
        url: "/operations",
        data: data,
        datatype: "html",
        success: function (data) {
            alert(data);
            location.reload();
        }
    })
    .fail(function (jqXHR, textStatus) {
        alert("Connection error : could not sell asset");
    });
}

// function to draw an asset's graph
function draw_graph(div, data, label) {
    var data_graph = [{ label: label, data: data }];
    var parameters = {
        series: {
            lines: { show: true },
            points: { show: true }
        },
        legend: {
            show: true,
            backgroundOpacity: 0,
        },
        grid: {
            hoverable: true
        },
        xaxis: {
            mode: "time"
        }
    };
    try {
        $(div).html("");
        $.plot(div, data_graph, parameters);
    } catch (e) {
        alert(e);
    }
}

// function to draw multi asset graph
function draw_multi_graph(div, data, label, nb_asset) {
    var data_graph = [];
    for (var i = 0; i < nb_asset; i++) {
        data_graph.push({ label: label[i], data: data[i] });
    }
    var parameters = {
        series: {
            lines: { show: true },
            points: { show: true }
        },
        legend: {
            show: true,
            backgroundOpacity: 0,
        },
        grid: {
            hoverable: true
        },
        xaxis: {
            mode: "time"
        }
    };
    try {
        $(div).html("");
        $.plot(div, data_graph, parameters);
    } catch (e) {
        alert(e);
    }
}




$(function () {
    // MAIN GRAPH PLOT (hedging portfolio + product)
    var data_graph = [
			{ label: label_graph_hedge, data: data_graph_hedge },
			{ label: label_graph_product, data: data_graph_product }
    ];
    var parameters = {
        series: {
            lines: { show: true },
            points: { show: true }
        },
        grid: {
            hoverable: true
        },
        legend: {
            show: true,
            backgroundOpacity: 0,
        },
        xaxis: {
            mode: "time"
        }
    };
    try {
        $.plot("#graph", data_graph, parameters);
    } catch (e) {

    }

    // DONUT PLOT
    var donut = $("#donut");
    $.plot(donut, data_donut, {
        series: {
            pie: {
                show: true,
                innerRadius: 0.25,
                label: {
                    show: true,
                    radius: 0.75,
                    background: {
                        opacity: 0.5,
                        color: '#000'
                    }
                }
            }
        },
        legend: {
            show: false
        },
        grid: {
            hoverable: true,
            clickable: true
        }
    });

    // function when clicking on an asset's "buy" (+)
    $(".buybutton").click(function () {
        $("#tooltip").hide();
        var assetname = $(this).attr("asset");
        var number = parseInt(window.prompt("How many of asset " + assetname + " do you want to buy ?", "1"));
        if (isInteger(number)) {
            asset_buy(assetname, number)
        } else {
            alert("Error : " + number + " is not an integer");
        }
    });

    // function when clicking on an asset's "sell" (-)
    $(".sellbutton").click(function () {
        $("#tooltip").hide();
        var assetname = $(this).attr("asset");
        var number = parseInt(window.prompt("How many of asset " + assetname + " do you want to sell ?", "1"));
        if (isInteger(number)) {
            asset_sell(assetname, number)
        } else {
            alert("Error : " + number + " is not an integer");
        }
    });

    // function when clicking on an asset (display a graph)
    $(".asset-clickable").click(function () {
        $("#tooltip").hide();
        var assetname = $(this).attr("asset");
        $(".asset-window").show();
        $("#asset-graph").html("Loading data ..");
        var data = { operation: 'getData', asset: assetname };
        //get_data
        $.ajax({
            type: "POST",
            url: "/operations",
            data: data,
            datatype: "html",
            success: function (data) {
                console.log(data);
                data = JSON.parse(data);
                draw_graph("#asset-graph", data, assetname);
            }
        })
        .fail(function (jqXHR, textStatus) {
            $("#asset-graph").html("Cannot get data");
        });
    });

    // close graph window
    $(".close-button").click(function () {
        $(this).parent().hide();
    });

    // function when mouse is over point of a graph (to display values in tooltip)
    $(".graph").bind("plothover", function (event, pos, item) {
        if (item) {
            var x = item.datapoint[0].toFixed(2),
                y = item.datapoint[1].toFixed(2);

            $("#tooltip").html(item.series.label + " value at " + formatDate(x) + " : " + y)
                .css({ top: item.pageY + 5, left: item.pageX + 5 })
                .fadeIn(200);
        } else {
            $("#tooltip").hide();
        }
    });

    // function when mouse is over point of a graph (to display values in tooltip)
    $(".mini-graph").bind("plothover", function (event, pos, item) {
        if (item) {
            var x = item.datapoint[0].toFixed(2),
                y = item.datapoint[1].toFixed(2);

            $("#tooltip").html(item.series.label + " value at " + formatDate(x) + " : " + y)
                .css({ top: item.pageY + 5, left: item.pageX + 5 })
                .fadeIn(200);
        } else {
            $("#tooltip").hide();
        }
    });

    // function to open a form to choose parameters of a derivative to buy or price
    $(".buyderivativebutton").click(function () {
        $("#tooltip").hide();
        $(".derivative-window").show();
        $("#derivative-form").html('loading ...');
        var data = { operation: 'getParam', derivative: $(this).attr("derivative") };
        $.ajax({
            type: "POST",
            url: "/operations",
            data: data,
            datatype: "html",
            success: function (data) {
                console.log(data);
                $("#derivative-form").html(data);
            }
        })
        .fail(function (jqXHR, textStatus) {
            alert("Connection error");
        });
    });

    // function to update price of derivative when an information on form change
    $("#derivative-form").change(function () {
        $("#price_derivative").val("Calculating...");
        var data = "operation=getPrice&" + $("#derivative-form").serialize();
        $.ajax({
            type: "POST",
            url: "/operations",
            data: data,
            datatype: "html",
            success: function (data) {
                $("#price_derivative").val(data);
            }
        })
        .fail(function (jqXHR, textStatus) {
            alert("Connection error : could not buy asset");
        });
    });

    // function to buy derivative
    $("#derivative-form").submit(function () {
        event.preventDefault();
        var data = "operation=buyDerivative&" + $("#derivative-form").serialize();
        $.ajax({
            type: "POST",
            url: "/operations",
            data: data,
            datatype: "html",
            success: function (data) {
                alert(data);
                location.reload();
            }
        })
        .fail(function (jqXHR, textStatus) {
            alert("Connection error");
        });
    });

    // function to open a window for hedging advices
    $(".advice-button").click(function () {
        $(".advice-window").show();
    });

    // simulation
    $("#simulate-button").click(function () {
        $(".simulation-window").show();
        $("#simulation-graph-prices").html("");
        $("#simulation-graph-trackingerror").html("loading ...");
        $("#simulation-graph-cash").html("");
        var data = "operation=simulation";
        $.ajax({
            type: "POST",
            url: "/operations",
            data: data,
            datatype: "html",
            success: function (data) {
                console.log(data);
                var data = JSON.parse(data);
                draw_multi_graph("#simulation-graph-prices", [data["simulation-graph-prices-everg"], data["simulation-graph-prices-hedge"]],
                    ["Everglades", "Hedging portfolio"], 2);
                draw_graph("#simulation-graph-trackingerror", data["simulation-graph-trackingerror"], "tracking error");
                draw_graph("#simulation-graph-cash", [data["simulation-graph-cash"], data["simulation-graph-soloport"]], ["cash", "couverture seule"], 2);
            }
        })
        .fail(function (jqXHR, textStatus) {
            alert("Connection error");
        });
    });
});
