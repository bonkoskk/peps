$(function () {
    // Scrolls to the selected menu item on the page
	$('a[href*=#]:not([href=#])').click(function() {
		if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') || location.hostname == this.hostname) {

			var target = $(this.hash);
			target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
			if (target.length) {
				$('html,body').animate({
					scrollTop: target.offset().top
				}, 1000);
				return false;
			}
		}
	});

    // GRAPH
	var data_graph_1 = [[0, 3], [4, 8], [8, 5], [9, 13], [14, 20]];
	var label_graph_1 = "hedge";
	var data_graph_2 = [[0, 8], [4, 3], [8, 6], [9, 2], [14, 18]];
	var label_graph_2 = "portfolio";
	var data_graph = [
			{ label: label_graph_1, data: data_graph_1 },
			{ label: label_graph_2, data: data_graph_2 }
	];
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
        $.plot("#graph", data_graph, parameters);
    } catch(e) {

    }


    // DONUT
	var data_donut = [];
	var series = Math.floor(Math.random() * 6) + 3;
	for (var i = 0; i < series; i++) {
	    data_donut[i] = {
	        label: "Series" + (i + 1),
	        data: Math.floor(Math.random() * 100) + 1
	    }
	}
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

});