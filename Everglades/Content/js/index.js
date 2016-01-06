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
        var data = [[0, 3], [4, 8], [8, 5], [9, 13], [14, 20]];
        draw_asset_graph("#asset-graph", data, assetname);
    });

    $(".asset-window > .close-button").click(function () {
        $(".asset-window").hide();
    });
});