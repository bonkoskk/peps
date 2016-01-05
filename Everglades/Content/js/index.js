// function "isInteger"
function isInteger(value) {
    return typeof value === "number" &&
      isFinite(value) &&
      Math.floor(value) === value;
};

// function to buy asset
function asset_buy(name, number) {
    $("button").click(function () {
        var data = { operation: 'buy', asset: name, number: number };
        $.ajax({
            type: "POST",
            url: "/operations",
            data: data,
            datatype: "html",
            success: function (data) {
                console.log(data);
                alert(data);
            }
        })
        .fail(function (jqXHR, textStatus) {
            console.log("error : " + textStatus);
            alert("error");
        })
        .always(function () {
            console.log("complete !");
              alert("complete");
        });
    });
}

// function to sell asset
function asset_sell(name, number) {
    $.post("/operations", function (data) {
        alert(data);
    });
    alert("Operation sell done");
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
});