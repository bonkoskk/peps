// function "isInteger"
function isInteger(value) {
    return typeof value === "number" &&
      isFinite(value) &&
      Math.floor(value) === value;
};

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

function draw_asset_graph(div, data, label) {
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
        xaxis: {
            mode: "time"
        }
    };
    try {
        $.plot(div, data_graph, parameters);
    } catch (e) {
        alert(e);
    }
}

$(function () {
    $(".buybutton").click(function () {
        var assetname = $(this).attr("asset");
        var number = parseInt(window.prompt("How many of asset " + assetname + " do you want to buy ?", "1"));
        if (isInteger(number)) {
            asset_buy(assetname, number)
        } else {
            alert("Error : " + number + " is not an integer");
        }
    });

    $(".sellbutton").click(function () {
        var assetname = $(this).attr("asset");
        var number = parseInt(window.prompt("How many of asset " + assetname + " do you want to sell ?", "1"));
        if (isInteger(number)) {
            asset_sell(assetname, number)
        } else {
            alert("Error : " + number + " is not an integer");
        }
    });

    $(".asset-clickable").click(function () {
        var assetname = $(this).attr("asset");
        $(".asset-window").show();
        //var data = [[1282688250, 0], [1282788250, 4], [1282888250, 8], [1282988250, 9], [1283088250, 14]];

        //get_data
        $.ajax({
            type: "POST",
            url: "/operations",
            data: data,
            datatype: "html",
            success: function (data) {
                $("#asset-graph").html(data);
                //draw_asset_graph("#asset-graph", data, assetname);
            }
        })
        .fail(function (jqXHR, textStatus) {
            $("#asset-graph").html("Cannot get data");
        });
    });

    $(".asset-window > .close-button").click(function () {
        $(".asset-window").hide();
    });
});